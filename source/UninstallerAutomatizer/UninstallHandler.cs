/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Klocman.Extensions;
using TestStack.White;
using UninstallerAutomatizer.Properties;
using UninstallTools;
using Debug = System.Diagnostics.Debug;

namespace UninstallerAutomatizer
{
    public class UninstallHandler
    {
        public bool KillOnFail { get; private set; }

        public string UninstallTarget { get; private set; }

        public bool IsDaemon { get; private set; }

        public event EventHandler<UninstallHandlerUpdateArgs> StatusUpdate;

        public void Pause()
        {
            throw new NotImplementedException();
        }

        public void Resume()
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            var args = Environment.GetCommandLineArgs().Skip(1).ToArray();

            if (args.Length == 1 && args[0].Equals("/d", StringComparison.OrdinalIgnoreCase))
            {
                StartDaemon();
                return;
            }

            IsDaemon = false;

            if (args.Length < 2)
            {
                Program.ReturnValue = ReturnValue.InvalidArgument;
                OnStatusUpdate(new UninstallHandlerUpdateArgs(UninstallHandlerUpdateKind.Failed, Localization.Error_Invalid_number_of_arguments));
                return;
            }

            UninstallerType uType;
            if (!Enum.TryParse(args[0], out uType))
            {
                Program.ReturnValue = ReturnValue.InvalidArgument;
                OnStatusUpdate(new UninstallHandlerUpdateArgs(UninstallHandlerUpdateKind.Failed, string.Format(Localization.Error_UnknownUninstallerType, args[0])));
                return;
            }

            args = args.Skip(1).ToArray();

            if (args[0].Equals("/k", StringComparison.InvariantCultureIgnoreCase))
            {
                args = args.Skip(1).ToArray();
                KillOnFail = true;
            }

            UninstallTarget = string.Join(" ", args);

            if (!File.Exists(UninstallTarget))
            {
                Program.ReturnValue = ReturnValue.InvalidArgument;
                OnStatusUpdate(new UninstallHandlerUpdateArgs(UninstallHandlerUpdateKind.Failed, string.Format(Localization.Error_InvalidPath, UninstallTarget)));
                return;
            }

            if (uType != UninstallerType.Nsis)
            {
                Program.ReturnValue = ReturnValue.InvalidArgument;
                OnStatusUpdate(new UninstallHandlerUpdateArgs(UninstallHandlerUpdateKind.Failed, string.Format(Localization.Error_NotSupported, uType)));
                return;
            }

            OnStatusUpdate(new UninstallHandlerUpdateArgs(UninstallHandlerUpdateKind.Normal, string.Format(Localization.Message_Starting, UninstallTarget)));

            _automationThread = new Thread(AutomationThread) { Name = "AutomationThread", IsBackground = false, Priority = ThreadPriority.AboveNormal };
            _automationThread.Start();
        }

        private void StartDaemon()
        {
            IsDaemon = true;
            OnStatusUpdate(new UninstallHandlerUpdateArgs(UninstallHandlerUpdateKind.Normal, "Starting automatizer as a deamon."));

            _automationThread = new Thread(DaemonThread) { Name = "AutomationThread", IsBackground = false, Priority = ThreadPriority.AboveNormal };
            _automationThread.Start();
        }

        readonly Dictionary<int, Task> _runningHooks = new Dictionary<int, Task>();

        /// <summary>
        /// Run in background as a daemon. Receive application PIDs to monitor, "stop" to exit.
        /// </summary>
        private void DaemonThread()
        {
            try
            {
                using (var server = new NamedPipeServerStream("UninstallAutomatizerDaemon"))
                using (var reader = new StreamReader(server))
                {
                    server.WaitForConnection();
                    while (true)
                    {
                        var line = reader.ReadLine()?.ToLowerInvariant();
                        if (line == null || line == "stop")
                            return;

                        int pid;
                        if (!int.TryParse(line, out pid))
                            throw new ArgumentException(pid + " is not a valid number");

                        try
                        {
                            lock (_runningHooks)
                            {
                                if (_runningHooks.ContainsKey(pid) && !_runningHooks[pid].IsCompleted)
                                    continue;

                                var target = Process.GetProcessById(pid);

                                var app = Application.Attach(target);

                                var t = Task.Factory.StartNew(() =>
                                {
                                    try
                                    {
                                        AutomatedUninstallManager.AutomatizeApplication(app, AutomatizeStatusCallback);
                                    }
                                    catch (Exception ex)
                                    {
                                        OnStatusUpdate(new UninstallHandlerUpdateArgs(UninstallHandlerUpdateKind.Normal,
                                            string.Format(Localization.Message_UninstallFailed,
                                                ex.InnerException?.Message ?? ex.Message)));
                                    }
                                    finally
                                    {
                                        lock (_runningHooks)
                                        {
                                            _runningHooks.Remove(pid);
                                        }
                                    }
                                });
                                
                                _runningHooks.Add(pid, t);
                            }
                        }
                        catch (SystemException) { }
                    }
                }
            }
            catch (Exception ex)
            {
                OnStatusUpdate(new UninstallHandlerUpdateArgs(UninstallHandlerUpdateKind.Normal,
                    string.Format("Daemon stopped because of: {0}", ex.InnerException?.Message ?? ex.Message)));
            }
            
            OnStatusUpdate(new UninstallHandlerUpdateArgs(UninstallHandlerUpdateKind.Succeeded, Localization.Message_Success));
        }

        Thread _automationThread;
        private bool _abort;

        private void AutomationThread()
        {
            try
            {
                AutomatedUninstallManager.UninstallNsisQuietly(UninstallTarget, AutomatizeStatusCallback);

                OnStatusUpdate(new UninstallHandlerUpdateArgs(UninstallHandlerUpdateKind.Succeeded, Localization.Message_Success));
            }
            catch (AutomatedUninstallManager.AutomatedUninstallException ex)
            {
                Debug.Assert(ex.InnerException != null, "ex.InnerException != null");

                OnStatusUpdate(new UninstallHandlerUpdateArgs(UninstallHandlerUpdateKind.Failed, string.Format(Localization.Message_UninstallFailed, ex.InnerException?.Message ?? ex.Message)));

                // todo grace period / move to window?
                if (ex.UninstallerProcess != null && KillOnFail)
                {
                    try
                    {
                        ex.UninstallerProcess.Kill(true);
                    }
                    catch
                    {
                        // Ignore process errors, can't do anything about it
                    }
                }

                Program.ReturnValue = ReturnValue.FunctionFailedCode;
            }
        }

        private void AutomatizeStatusCallback(string s)
        {
            if (_abort) throw new OperationCanceledException(Localization.Message_UserCancelled);
            OnStatusUpdate(new UninstallHandlerUpdateArgs(UninstallHandlerUpdateKind.Normal, s));
        }

        private UninstallHandlerUpdateArgs _previousArgs;

        protected virtual void OnStatusUpdate(UninstallHandlerUpdateArgs e)
        {
            // Filter out repeated updates
            if (Equals(_previousArgs, e)) return;
            _previousArgs = e;

            StatusUpdate?.Invoke(this, e);
            Console.WriteLine(e.Message);
        }

        public void Abort()
        {
            _abort = true;
        }
    }
}
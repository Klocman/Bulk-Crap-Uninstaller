/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Klocman.Extensions;
using UninstallTools;

namespace UninstallerAutomatizer
{
    public class UninstallHandler
    {
        public bool KillOnFail { get; private set; }

        public string UninstallTarget { get; private set; }

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

            if (args.Length < 2)
            {
                Program.ReturnValue = ReturnValue.InvalidArgument;
                OnStatusUpdate(new UninstallHandlerUpdateArgs(UninstallHandlerUpdateKind.Failed, @"Invalid number of arguments"));
                return;
            }

            UninstallerType uType;
            if (!Enum.TryParse(args[0], out uType))
            {
                Program.ReturnValue = ReturnValue.InvalidArgument;
                OnStatusUpdate(new UninstallHandlerUpdateArgs(UninstallHandlerUpdateKind.Failed, "Unknown uninstaller type: " + args[0]));
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
                OnStatusUpdate(new UninstallHandlerUpdateArgs(UninstallHandlerUpdateKind.Failed, "Invalid path or file doesn't exist: " + UninstallTarget));
                return;
            }

            if (uType != UninstallerType.Nsis)
            {
                Program.ReturnValue = ReturnValue.InvalidArgument;
                OnStatusUpdate(new UninstallHandlerUpdateArgs(UninstallHandlerUpdateKind.Failed, "Automation of " + uType + " uninstallers is not supported"));
                return;
            }

            OnStatusUpdate(new UninstallHandlerUpdateArgs(UninstallHandlerUpdateKind.Normal, "Automatically uninstalling " + UninstallTarget));

            _automationThread = new Thread(AutomationThread) { Name = "AutomationThread", IsBackground = false, Priority = ThreadPriority.AboveNormal };
            _automationThread.Start();
        }

        Thread _automationThread;
        private bool _abort;

        private void AutomationThread()
        {
            try
            {
                AutomatedUninstallManager.UninstallNsisQuietly(UninstallTarget, s =>
                {
                    if (_abort) throw new OperationCanceledException("User cancelled the operation");
                    OnStatusUpdate(new UninstallHandlerUpdateArgs(UninstallHandlerUpdateKind.Normal, s));
                });

                OnStatusUpdate(new UninstallHandlerUpdateArgs(UninstallHandlerUpdateKind.Succeeded, "Automation was successful"));
            }
            catch (AutomatedUninstallManager.AutomatedUninstallException ex)
            {
                Debug.Assert(ex.InnerException != null, "ex.InnerException != null");

                OnStatusUpdate(new UninstallHandlerUpdateArgs(UninstallHandlerUpdateKind.Failed, "Automatic uninstallation failed. Reason:" + ex.InnerException.Message));

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
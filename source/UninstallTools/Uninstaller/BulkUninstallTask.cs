/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Threading;
using Klocman.Extensions;
using Klocman.Localising;

namespace UninstallTools.Uninstaller
{
    public sealed class BulkUninstallTask : IDisposable
    {
        private readonly object _operationLock = new object();
        private int _concurrentUninstallerCount;
        private bool _finished;
        private Thread _workerThread;

        /// <exception cref="ArgumentNullException"><paramref name="taskList" /> is null.</exception>
        /// <exception cref="OverflowException">
        ///     The number of elements in <paramref name="taskList" /> is larger than
        ///     <see cref="F:System.Int32.MaxValue" />.
        /// </exception>
        internal BulkUninstallTask(IList<BulkUninstallEntry> taskList, BulkUninstallConfiguration configuration)
        {
            if (taskList == null)
                throw new ArgumentNullException(nameof(taskList));
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            if (taskList.Count < 1)
                throw new ArgumentException("Task list can't be empty");

            AllUninstallersList = new List<BulkUninstallEntry>();

            for (var index = 0; index < taskList.Count; index++)
            {
                var bulkUninstallEntry = taskList[index];
                bulkUninstallEntry.Id = index + 1;
                AllUninstallersList.Add(bulkUninstallEntry);
            }

            Configuration = configuration;

            _finished = false;
            Aborted = false;
        }

        public bool Aborted { get; set; }
        public BulkUninstallConfiguration Configuration { get; }

        public bool Finished
        {
            get { return _finished; }
            private set
            {
                if (_finished == value) return;

                _finished = value;
                OnStatusChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public IList<BulkUninstallEntry> AllUninstallersList { get; }

        public int ConcurrentUninstallerCount
        {
            get { return _concurrentUninstallerCount; }
            set { _concurrentUninstallerCount = Math.Min(1000, Math.Max(1, value)); }
        }

        public bool OneLoudLimit { get; set; } = true;

        public void Dispose()
        {
            OnStatusChanged = null;
            _finished = true;
        }

        public event EventHandler OnStatusChanged;

        public static object DisplayNameAspectGetter(object rowObj)
        {
            var temp = rowObj as BulkUninstallEntry;
            return temp?.UninstallerEntry.DisplayName;
        }

        public static object IsSilentAspectGetter(object rowObj)
        {
            var temp = rowObj as BulkUninstallEntry;
            return temp?.IsSilent.ToYesNo();
        }

        public static object StatusAspectGetter(object rowObj)
        {
            var temp = rowObj as BulkUninstallEntry;
            if (temp == null) return null;

            var name = temp.CurrentStatus.GetLocalisedName();
            if (temp.CurrentError != null)
                name = string.Concat(name, " - ", temp.CurrentError.Message);
            return name;
        }

        public void Start()
        {
            lock (_operationLock)
            {
                if (_workerThread != null && _workerThread.IsAlive)
                {
                    if (Finished)
                    {
                        if (!_workerThread.Join(TimeSpan.FromSeconds(10)))
                            _workerThread.Abort();
                        else
                            return;
                    }
                    else
                        return;
                }

                Aborted = false;
                Finished = false;

                _workerThread = new Thread(UninstallWorkerThread) { Name = "RunBulkUninstall_Worker" };
                _workerThread.Start();
            }
        }

        private void UninstallWorkerThread()
        {
            var targetList = AllUninstallersList;
            var configuration = Configuration;
            if (targetList == null || configuration == null)
                throw new ArgumentException("BulkUninstallTask is incomplete, this should not have happened.");

            if (configuration.PreferQuiet && AllUninstallersList.Any(x => x.IsSilent))
                StartAutomationDaemon();

            try
            {
                StartOfLoop:
                while (AllUninstallersList.Any(x => x.CurrentStatus == UninstallStatus.Waiting || x.IsRunning))
                {
                    do
                    {
                        if (Aborted)
                        {
                            AllUninstallersList.ForEach(x => x.SkipWaiting(false));
                            break;
                        }
                        Thread.Sleep(500);
                    } while (AllUninstallersList.Count(x => x.IsRunning) >= ConcurrentUninstallerCount);

                    var running =
                        AllUninstallersList.Where(x => x.CurrentStatus == UninstallStatus.Uninstalling).ToList();
                    var runningTypes = running.Select(y => y.UninstallerEntry.UninstallerKind).ToList();
                    var loudBlocked = OneLoudLimit && running.Any(y => !y.IsSilent);

                    var result = AllUninstallersList.FirstOrDefault(x =>
                    {
                        if (x.CurrentStatus != UninstallStatus.Waiting || (loudBlocked && !x.IsSilent))
                            return false;

                        if (CheckForTypeCollisions(x.UninstallerEntry.UninstallerKind, runningTypes))
                            return false;

                        if (CheckForAdvancedCollisions(x.UninstallerEntry, running.Select(y => y.UninstallerEntry)))
                            return false;

                        return true;
                    });

                    if (result != null)
                    {
                        result.RunUninstaller(
                            new BulkUninstallEntry.RunUninstallerOptions(configuration.AutoKillStuckQuiet,
                                configuration.RetryFailedQuiet, configuration.PreferQuiet, configuration.Simulate, this));
                        // Fire the event now so the interface can be updated
                        OnStatusChanged?.Invoke(this, EventArgs.Empty);
                    }
                }

                if (AllUninstallersList.Any(x => x.CurrentStatus == UninstallStatus.Paused))
                {
                    Thread.Sleep(100);
                    goto StartOfLoop;
                }
            }
            finally
            {
                StopAutomationDaemon();
                Finished = true;
            }
        }

        private void StopAutomationDaemon()
        {
            try
            {
                using (client)
                using (writer)
                {
                    writer?.WriteLine(@"stop");
                    client = null;
                    writer = null;
                }

                if (_quietUninstallDaemonProcess != null && !_quietUninstallDaemonProcess.HasExited)
                    _quietUninstallDaemonProcess.WaitForExit(7000);
            }
            catch (SystemException ex)
            {
                Console.WriteLine(@"Failed to peacefully close automatizer daemon");
                Console.WriteLine(ex);

                try { _quietUninstallDaemonProcess?.Kill(); }
                catch (SystemException) { }
            }

            _quietUninstallDaemonProcess = null;
        }

        Process _quietUninstallDaemonProcess;

        private void StartAutomationDaemon()
        {
            if (UninstallToolsGlobalConfig.UseQuietUninstallDaemon)
            {
                if (!UninstallToolsGlobalConfig.UninstallerAutomatizerExists)
                    UninstallToolsGlobalConfig.UseQuietUninstallDaemon = false;
                else
                {
                    try
                    {
                        _quietUninstallDaemonProcess = Process.Start(UninstallToolsGlobalConfig.UninstallerAutomatizerPath, "/d");

                        try
                        {
                            client = new NamedPipeClientStream(".", "UninstallAutomatizerDaemon", PipeDirection.Out);
                            writer = new StreamWriter(client);

                            client.Connect(7000);
                            writer.AutoFlush = true;
                        }
                        catch (SystemException ex)
                        {
                            UninstallToolsGlobalConfig.UseQuietUninstallDaemon = false;

                            Console.WriteLine(@"Failed to connect to automatization daemon");
                            Console.WriteLine(ex);

                            StopAutomationDaemon();
                        }
                    }
                    catch (SystemException ex)
                    {
                        UninstallToolsGlobalConfig.UseQuietUninstallDaemon = false;

                        Console.WriteLine(@"Failed to start automatization daemon");
                        Console.WriteLine(ex);
                    }
                }
            }
        }

        NamedPipeClientStream client;
        StreamWriter writer;

        internal void SendProcessesToWatchToDeamon(IEnumerable<int> processIdsToAutomate)
        {
            if (writer == null || client == null || !client.IsConnected) return;

            var pidList = processIdsToAutomate.ToList();

            foreach (var pid in pidList)
            {
                Debug.WriteLine("Sending pid: " + pid + " to automatizer daemon");
                writer.WriteLine(pid.ToString(CultureInfo.InvariantCulture));
            }
        }

        private static bool CheckForAdvancedCollisions(ApplicationUninstallerEntry target,
            IEnumerable<ApplicationUninstallerEntry> running)
        {
            var entries = running.ToList();

            if (entries.Any(x => x.PublisherTrimmed.Equals(
                target.PublisherTrimmed, StringComparison.InvariantCultureIgnoreCase)))
                return true;

            if (target.InstallLocation.IsNotEmpty() &&
                entries.Any(x => x.InstallLocation.IsNotEmpty() &&
                                 (x.InstallLocation.StartsWith(target.InstallLocation, StringComparison.InvariantCultureIgnoreCase) ||
                                  target.InstallLocation.StartsWith(x.InstallLocation, StringComparison.InvariantCultureIgnoreCase))))
                return true;

            if (target.UninstallerKind == UninstallerType.Msiexec)
            {
                var processes = Process.GetProcessesByName("msiexec");
                if (processes.Length > 1)
                    return true;
            }

            return false;
        }

        private static bool CheckForTypeCollisions(UninstallerType target, IEnumerable<UninstallerType> running)
        {
            switch (target)
            {
                // Might cause collisions, don't run concurrently
                case UninstallerType.InstallShield:
                case UninstallerType.SdbInst:
                case UninstallerType.WindowsFeature:
                case UninstallerType.WindowsUpdate:
                case UninstallerType.Unknown:
                // Chocolatey can use app's original uninstaller, so it's essentially unknown
                case UninstallerType.Chocolatey:
                    target = UninstallerType.Msiexec;
                    break;

                // Can be ran concurrently
                case UninstallerType.Msiexec:
                case UninstallerType.InnoSetup:
                case UninstallerType.Steam:
                case UninstallerType.Nsis:
                case UninstallerType.StoreApp:
                case UninstallerType.SimpleDelete:
                    break;

                default:
                    Debug.Fail("Unhandled UninstallerType - " + target);
                    goto case UninstallerType.Unknown;
            }

            foreach (var item in running)
            {
                var x = item;
                if (x == UninstallerType.InstallShield || x == UninstallerType.WindowsFeature || x == UninstallerType.SdbInst || x == UninstallerType.Unknown)
                    x = UninstallerType.Msiexec;

                if (x == target)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
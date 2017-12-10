/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                                configuration.RetryFailedQuiet, configuration.PreferQuiet, configuration.Simulate));
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
                Finished = true;
            }
        }

        /*public bool RunSingle(BulkUninstallEntry entry, bool disableCollisionDetection)
        {
            if (!disableCollisionDetection)
            {
                var running = AllUninstallersList.Where(x => x.CurrentStatus == UninstallStatus.Uninstalling).ToList();
                var runningTypes = running.Select(y => y.UninstallerEntry.UninstallerKind).ToList();

                if (CheckForTypeCollisions(entry.UninstallerEntry.UninstallerKind, runningTypes))
                    return false;

                if (CheckForAdvancedCollisions(entry.UninstallerEntry, running.Select(y => y.UninstallerEntry)))
                    return false;
            }

            entry.RunUninstaller(new BulkUninstallEntry.RunUninstallerOptions(Configuration.AutoKillStuckQuiet,
                    Configuration.RetryFailedQuiet, Configuration.PreferQuiet, Configuration.Simulate));
            return true;
        }*/

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
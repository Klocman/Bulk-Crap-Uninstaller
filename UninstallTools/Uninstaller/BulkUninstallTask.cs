using System;
using System.Collections.Generic;
using System.Linq;
using Klocman.Extensions;
using Klocman.Localising;

namespace UninstallTools.Uninstaller
{
    public sealed class BulkUninstallTask : IDisposable
    {
        private bool _finished;
        internal SkipCurrentLevel SkipCurrent = SkipCurrentLevel.None;

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

            AllUninstallersList = taskList;
            Configuration = configuration;

            TaskCount = AllUninstallEntries.Count();
            _finished = false;
            Aborted = false;

            CurrentTask = 0;
            CurrentUninstallerStatus = taskList[0];
        }

        public bool Aborted { get; set; }
        public IEnumerable<BulkUninstallEntry> AllUninstallEntries => AllUninstallersList;
        public BulkUninstallConfiguration Configuration { get; private set; }
        public int CurrentTask { get; internal set; }
        public BulkUninstallEntry CurrentUninstallerStatus { get; internal set; }

        public bool Finished
        {
            get { return _finished; }
            internal set
            {
                _finished = value;
                OnStatusChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public int TaskCount { get; private set; }
        internal IList<BulkUninstallEntry> AllUninstallersList { get; }

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

        public void SkipWaitingForCurrent(bool terminate)
        {
            SkipCurrent = terminate ? SkipCurrentLevel.Terminate : SkipCurrentLevel.Skip;
        }

        internal void FireOnStatusChanged()
        {
            OnStatusChanged?.Invoke(this, EventArgs.Empty);
        }

        internal enum SkipCurrentLevel
        {
            None = 0,
            Terminate,
            Skip
        }
    }
}
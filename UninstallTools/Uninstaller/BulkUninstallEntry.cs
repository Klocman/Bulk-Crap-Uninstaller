using System;

namespace UninstallTools.Uninstaller
{
    public class BulkUninstallEntry
    {
        internal BulkUninstallEntry(ApplicationUninstallerEntry uninstallerEntry, bool isSilent,
            UninstallStatus startingStatus)
        {
            CurrentStatus = startingStatus;
            IsSilent = isSilent;
            UninstallerEntry = uninstallerEntry;
        }

        public Exception CurrentError { get; set; }
        public UninstallStatus CurrentStatus { get; set; }
        public bool IsSilent { get; }
        public ApplicationUninstallerEntry UninstallerEntry { get; }
    }
}
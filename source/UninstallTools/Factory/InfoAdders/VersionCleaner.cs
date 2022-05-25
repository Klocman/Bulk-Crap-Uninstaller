/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

namespace UninstallTools.Factory.InfoAdders
{
    public class VersionCleaner : IMissingInfoAdder
    {
        public void AddMissingInformation(ApplicationUninstallerEntry target)
        {
            if (string.IsNullOrEmpty(target.DisplayVersion)) return;

            target.DisplayVersion = ApplicationEntryTools.CleanupDisplayVersion(target.DisplayVersion);
        }

        public string[] RequiredValueNames { get; } = {
            nameof(ApplicationUninstallerEntry.DisplayVersion)
        };

        public bool RequiresAllValues { get; } = true;
        public bool AlwaysRun { get; } = true;
        public string[] CanProduceValueNames { get; } = {};
        public InfoAdderPriority Priority { get; } = InfoAdderPriority.RunDeadLast;
    }
}
/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

namespace UninstallTools.Factory.InfoAdders
{
    public class QuietUninstallStringCopier : IMissingInfoAdder
    {
        public void AddMissingInformation(ApplicationUninstallerEntry target)
        {
            target.UninstallString = target.QuietUninstallString;
        }

        public string[] RequiredValueNames { get; } = {
            nameof(ApplicationUninstallerEntry.QuietUninstallString)
        };
        public bool RequiresAllValues { get; } = true;

        public string[] CanProduceValueNames { get; } = {
            nameof(ApplicationUninstallerEntry.UninstallString)
        };

        public InfoAdderPriority Priority { get; } = InfoAdderPriority.RunFirst;
    }
}
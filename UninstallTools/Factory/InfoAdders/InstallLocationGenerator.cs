/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

namespace UninstallTools.Factory.InfoAdders
{
    public class InstallLocationGenerator : IMissingInfoAdder
    {
        public void AddMissingInformation(ApplicationUninstallerEntry target)
        {
            if (target.UninstallerKind == UninstallerType.Nsis || target.UninstallerKind == UninstallerType.InnoSetup)
            {
                if(!string.IsNullOrEmpty(target.UninstallerLocation))
                    target.InstallLocation = target.UninstallerLocation;
            }
        }

        public string[] RequiredValueNames { get; } = {
            nameof(ApplicationUninstallerEntry.UninstallerKind),
            nameof(ApplicationUninstallerEntry.UninstallerLocation)
        };
        public bool RequiresAllValues { get; } = true;

        public string[] CanProduceValueNames { get; } = {
            nameof(ApplicationUninstallerEntry.InstallLocation)
        };

        public InfoAdderPriority Priority { get; } = InfoAdderPriority.RunFirst;
    }
}
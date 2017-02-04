/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using UninstallTools.Uninstaller;

namespace UninstallTools.Factory.InfoAdders
{
    public class MsiUninstallStringGenerator : IMissingInfoAdder
    {
        public void AddMissingInformation(ApplicationUninstallerEntry target)
        {
            if (target.UninstallerKind != UninstallerType.Msiexec)
                return;

            if (string.IsNullOrEmpty(target.UninstallString))
                target.UninstallString = UninstallManager.GetMsiString(target.BundleProviderKey,
                    MsiUninstallModes.Uninstall);

            if (string.IsNullOrEmpty(target.QuietUninstallString))
                target.QuietUninstallString = UninstallManager.GetMsiString(target.BundleProviderKey,
                    MsiUninstallModes.QuietUninstall);
        }

        public string[] RequiredValueNames { get; } = {
            nameof(ApplicationUninstallerEntry.UninstallerKind),
            nameof(ApplicationUninstallerEntry.BundleProviderKey)
        };

        public bool RequiresAllValues { get; } = true;
        public string[] CanProduceValueNames { get; } = {
            nameof(ApplicationUninstallerEntry.QuietUninstallString),
            nameof(ApplicationUninstallerEntry.UninstallString)
        };
        public InfoAdderPriority Priority { get; } = InfoAdderPriority.RunLast;
    }
}

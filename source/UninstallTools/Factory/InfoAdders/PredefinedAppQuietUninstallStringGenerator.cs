/*
    Copyright (c) 2019 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;

namespace UninstallTools.Factory.InfoAdders
{
    /// <summary>
    /// Hardcoded quiet string generators for specific applications
    /// </summary>
    public class PredefinedAppQuietUninstallStringGenerator : IMissingInfoAdder
    {
        public void AddMissingInformation(ApplicationUninstallerEntry target)
        {
            if (target.QuietUninstallPossible || !target.UninstallPossible)
                return;

            var uninstallString = target.UninstallString;

            // MS Edge developer builds
            if (uninstallString.Contains("--msedge-beta", StringComparison.Ordinal) && uninstallString.Contains("--uninstall", StringComparison.Ordinal))
                target.QuietUninstallString = uninstallString.Replace("--uninstall", "--force-uninstall");
        }

        public string[] RequiredValueNames { get; } = {
            nameof(ApplicationUninstallerEntry.UninstallString)
        };

        public bool RequiresAllValues { get; } = true;
        public bool AlwaysRun { get; } = false;

        public string[] CanProduceValueNames { get; } = {
            nameof(ApplicationUninstallerEntry.QuietUninstallString)
        };

        public InfoAdderPriority Priority { get; } = InfoAdderPriority.Normal;
    }
}

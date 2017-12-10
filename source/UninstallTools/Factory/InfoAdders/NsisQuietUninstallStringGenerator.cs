/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.IO;
using Klocman.Extensions;

namespace UninstallTools.Factory.InfoAdders
{
    public class NsisQuietUninstallStringGenerator : IMissingInfoAdder
    {
        public void AddMissingInformation(ApplicationUninstallerEntry target)
        {
            if (target.QuietUninstallPossible || target.UninstallerKind != UninstallerType.Nsis ||
                string.IsNullOrEmpty(target.UninstallString))
                return;

            if (!UninstallToolsGlobalConfig.QuietAutomatization || !UninstallToolsGlobalConfig.UninstallerAutomatizerExists)
                return;

            var nsisCommandStart = $"\"{UninstallToolsGlobalConfig.UninstallerAutomatizerPath}\" {nameof(UninstallerType.Nsis)} ";

            nsisCommandStart = nsisCommandStart.AppendIf(UninstallToolsGlobalConfig.QuietAutomatizationKillStuck, "/K ");

            target.QuietUninstallString = nsisCommandStart.Append(target.UninstallString);
        }

        public string[] RequiredValueNames { get; } = {
            nameof(ApplicationUninstallerEntry.UninstallString),
            nameof(ApplicationUninstallerEntry.UninstallerKind)
        };

        public bool RequiresAllValues { get; } = true;
        public bool AlwaysRun { get; } = false;

        public string[] CanProduceValueNames { get; } = {
            nameof(ApplicationUninstallerEntry.QuietUninstallString)
        };

        public InfoAdderPriority Priority { get; } = InfoAdderPriority.Normal;
    }
}
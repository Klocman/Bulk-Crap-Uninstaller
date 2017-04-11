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
        private static string UninstallerAutomatizerPath
            => Path.Combine(UninstallToolsGlobalConfig.AssemblyLocation, @"UninstallerAutomatizer.exe");

        public void AddMissingInformation(ApplicationUninstallerEntry target)
        {
            if (target.QuietUninstallPossible || target.UninstallerKind != UninstallerType.Nsis ||
                string.IsNullOrEmpty(target.UninstallString))
                return;

            if (!UninstallToolsGlobalConfig.QuietAutomatization || !File.Exists(UninstallerAutomatizerPath))
                return;

            var nsisCommandStart = $"\"{UninstallerAutomatizerPath}\" {UninstallerType.Nsis} ";
            if (UninstallToolsGlobalConfig.QuietAutomatizationKillStuck)
                nsisCommandStart = nsisCommandStart.Append("/K ");
            target.QuietUninstallString = nsisCommandStart + target.UninstallString;
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
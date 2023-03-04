/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Linq;

namespace UninstallTools.Factory.InfoAdders
{
    public class GenerateSteamHelperStrings : IMissingInfoAdder
    {
        public void AddMissingInformation(ApplicationUninstallerEntry target)
        {
            if (target.UninstallerKind != UninstallerType.Steam) return;
            
            var appId = target.RatingId.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Last();
            if (!int.TryParse(appId, out _)) return;

            if(!target.UninstallPossible || UninstallToolsGlobalConfig.QuietAutomatization)
                target.UninstallString = $"\"{SteamFactory.SteamHelperPath}\" uninstall {appId}";

            if (UninstallToolsGlobalConfig.QuietAutomatization)
                target.QuietUninstallString = $"\"{SteamFactory.SteamHelperPath}\" uninstall /silent {appId}";
        }

        public string[] RequiredValueNames { get; } = {
            nameof(ApplicationUninstallerEntry.UninstallerKind),
            nameof(ApplicationUninstallerEntry.RatingId)
        };
        public bool RequiresAllValues { get; } = true;
        public bool AlwaysRun { get; } = true;

        public string[] CanProduceValueNames { get; } = {
            nameof(ApplicationUninstallerEntry.UninstallString),
            nameof(ApplicationUninstallerEntry.QuietUninstallString)
        };
        public InfoAdderPriority Priority { get; } = InfoAdderPriority.RunLast;
    }
}
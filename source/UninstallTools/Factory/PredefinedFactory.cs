/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.Collections.Generic;
using System.IO;
using Klocman.IO;
using UninstallTools.Properties;

namespace UninstallTools.Factory
{
    /// <summary>
    /// Get uninstallers that were manually pre-defined.
    /// </summary>
    public class PredefinedFactory : IIndependantUninstallerFactory
    {
        public IList<ApplicationUninstallerEntry> GetUninstallerEntries(
            ListGenerationProgress.ListGenerationCallback progressCallback)
        {
            var items = new List<ApplicationUninstallerEntry>();

            var s = GetSteamUninstallerEntry();
            if (s != null)
                items.Add(s);

            return items;
        }

        private static ApplicationUninstallerEntry GetSteamUninstallerEntry()
        {
            if (string.IsNullOrEmpty(SteamFactory.SteamLocation)) return null;

            return new ApplicationUninstallerEntry
            {
                AboutUrl = @"http://store.steampowered.com/about/",
                InstallLocation = SteamFactory.SteamLocation,
                DisplayIcon = Path.Combine(SteamFactory.SteamLocation, "Steam.exe"),
                DisplayName = "Steam",
                UninstallerKind = UninstallerType.Nsis,
                UninstallString = Path.Combine(SteamFactory.SteamLocation, "uninstall.exe"),
                IsOrphaned = true,
                IsValid = File.Exists(Path.Combine(SteamFactory.SteamLocation, "uninstall.exe")),
                InstallDate = Directory.GetCreationTime(SteamFactory.SteamLocation),
                Publisher = "Valve Corporation",
                // Prevent very long size scan in case of many games, the install itself is about 600-800MB
                EstimatedSize = FileSize.FromKilobytes(1024 * 700)
            };
        }

        public bool IsEnabled() => UninstallToolsGlobalConfig.ScanPreDefined;
        public string DisplayName => Localisation.Progress_AppStores_Templates;
    }
}
/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.IO;
using Klocman.IO;
using Klocman.Tools;
using UninstallTools.Properties;

namespace UninstallTools.Factory
{
    public class SteamFactory : IIndependantUninstallerFactory
    {
        private static bool? _steamHelperIsAvailable;
        private static string _steamLocation;

        internal static string SteamLocation
        {
            get
            {
                if (_steamLocation == null)
                    GetSteamInfo();
                return _steamLocation;
            }
            private set { _steamLocation = value; }
        }

        internal static bool SteamHelperIsAvailable
        {
            get
            {
                if (!_steamHelperIsAvailable.HasValue)
                {
                    _steamHelperIsAvailable = false;
                    GetSteamInfo();
                }
                return _steamHelperIsAvailable.Value;
            }
        }

        private static void GetSteamInfo()
        {
            _steamHelperIsAvailable = false;

            if (File.Exists(SteamHelperPath) && WindowsTools.CheckNetFramework4Installed(true) != null)
            {
                var output = FactoryTools.StartHelperAndReadOutput(SteamHelperPath, "steam");
                if (!string.IsNullOrEmpty(output)
                    && !output.Contains("error", StringComparison.InvariantCultureIgnoreCase)
                    && Directory.Exists(output = output.Trim().TrimEnd('\\', '/')))
                {
                    _steamHelperIsAvailable = true;
                    SteamLocation = output;
                }
            }
        }

        internal static string SteamHelperPath
            => Path.Combine(UninstallToolsGlobalConfig.AssemblyLocation, @"SteamHelper.exe");

        public IList<ApplicationUninstallerEntry> GetUninstallerEntries(
            ListGenerationProgress.ListGenerationCallback progressCallback)
        {
            var results = new List<ApplicationUninstallerEntry>();
            if (!SteamHelperIsAvailable) return results;

            var output = FactoryTools.StartHelperAndReadOutput(SteamHelperPath, "l /i");
            if (string.IsNullOrEmpty(output) || output.Contains("error", StringComparison.InvariantCultureIgnoreCase)) return results;

            foreach (var data in FactoryTools.ExtractAppDataSetsFromHelperOutput(output))
            {
                if (!int.TryParse(data["AppId"], out var appId)) continue;

                var entry = new ApplicationUninstallerEntry
                {
                    DisplayName = data["Name"],
                    UninstallString = data["UninstallString"],
                    InstallLocation = data["InstallDirectory"],
                    UninstallerKind = UninstallerType.Steam,
                    IsValid = true,
                    IsOrphaned = true,
                    RatingId = "Steam App " + appId.ToString("G")
                };

                if (long.TryParse(data["SizeOnDisk"], out var bytes))
                    entry.EstimatedSize = FileSize.FromBytes(bytes);

                results.Add(entry);
            }

            return results;
        }

        public bool IsEnabled() => UninstallToolsGlobalConfig.ScanSteam;
        public string DisplayName => Localisation.Progress_AppStores_Steam;
    }
}
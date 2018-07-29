/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Klocman.Extensions;
using Klocman.IO;
using Klocman.Tools;

namespace UninstallTools.Factory
{
    public class SteamFactory : IUninstallerFactory
    {
        private static bool? _steamHelperIsAvailable;
        private static string _steamLocation;

        internal static string SteamLocation
        {
            get
            {
                if(_steamLocation == null)
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

            if (File.Exists(SteamHelperPath) && WindowsTools.CheckNetFramework4Installed(true))
            {
                var output = FactoryTools.StartProcessAndReadOutput(SteamHelperPath, "steam");
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

        public IEnumerable<ApplicationUninstallerEntry> GetUninstallerEntries(
            ListGenerationProgress.ListGenerationCallback progressCallback)
        {
            return GetSteamApps();
        }

        private static IEnumerable<ApplicationUninstallerEntry> GetSteamApps()
        {
            if (!SteamHelperIsAvailable)
                yield break;

            var output = FactoryTools.StartProcessAndReadOutput(SteamHelperPath, "list");
            if (string.IsNullOrEmpty(output) || output.Contains("error", StringComparison.InvariantCultureIgnoreCase))
                yield break;

            foreach (var idString in output.SplitNewlines(StringSplitOptions.RemoveEmptyEntries))
            {
                int appId;
                if (!int.TryParse(idString, out appId)) continue;

                output = FactoryTools.StartProcessAndReadOutput(SteamHelperPath,
                    "info " + appId.ToString("G"));
                if (string.IsNullOrEmpty(output) ||
                    output.Contains("error", StringComparison.InvariantCultureIgnoreCase))
                    continue;

                var lines = output.SplitNewlines(StringSplitOptions.RemoveEmptyEntries).Select(x =>
                {
                    var o = x.Split(new[] {" - "}, StringSplitOptions.None);
                    return new KeyValuePair<string, string>(o[0], o[1]);
                }).ToList();

                var entry = new ApplicationUninstallerEntry
                {
                    DisplayName =
                        lines.Single(x => x.Key.Equals("Name", StringComparison.InvariantCultureIgnoreCase)).Value,
                    UninstallString =
                        lines.Single(x => x.Key.Equals("UninstallString", StringComparison.InvariantCultureIgnoreCase))
                            .Value,
                    InstallLocation =
                        lines.Single(x => x.Key.Equals("InstallDirectory", StringComparison.InvariantCultureIgnoreCase))
                            .Value,
                    UninstallerKind = UninstallerType.Steam,
                    IsValid = true,
                    IsOrphaned = true,
                    RatingId = "Steam App " + appId.ToString("G")
                };

                long bytes;
                if (
                    long.TryParse(
                        lines.Single(x => x.Key.Equals("SizeOnDisk", StringComparison.InvariantCultureIgnoreCase)).Value,
                        out bytes))
                    entry.EstimatedSize = FileSize.FromBytes(bytes);

                yield return entry;
            }
        }
    }
}
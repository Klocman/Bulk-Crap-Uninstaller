/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Klocman.Extensions;
using Klocman.IO;
using Klocman.Tools;

namespace UninstallTools.Factory
{
    public class SteamFactory : IUninstallerFactory
    {
        public static string StartProcessAndReadOutput(string filename, string args)
        {
            using (var process = Process.Start(new ProcessStartInfo(filename, args)
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = false,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.Unicode
            })) return process?.StandardOutput.ReadToEnd();
        }

        private static bool? _steamHelperIsAvailable;
        private static string _steamLocation;

        private static bool SteamHelperIsAvailable
        {
            get
            {
                if (!_steamHelperIsAvailable.HasValue)
                {
                    _steamHelperIsAvailable = false;

                    if (File.Exists(SteamHelperPath) && WindowsTools.CheckNetFramework4Installed(true))
                    {
                        var output = StartProcessAndReadOutput(SteamHelperPath, "steam");
                        if (!string.IsNullOrEmpty(output)
                            && !output.Contains("error", StringComparison.InvariantCultureIgnoreCase)
                            && Directory.Exists(output = output.Trim().TrimEnd('\\', '/')))
                        {
                            _steamHelperIsAvailable = true;
                            _steamLocation = output;
                        }
                    }
                }
                return _steamHelperIsAvailable.Value;
            }
        }

        private static string SteamHelperPath
            => Path.Combine(UninstallToolsGlobalConfig.AssemblyLocation, @"SteamHelper.exe");

        public IEnumerable<ApplicationUninstallerEntry> GetUninstallerEntries()
        {
            var applicationUninstallers = new List<ApplicationUninstallerEntry>();

            if (SteamHelperIsAvailable)
            {
                var steamAppsOnDisk = GetSteamApps().ToList();

                foreach (var steamApp in
                    // bug not actually there
                    applicationUninstallers.Where(x => x.UninstallerKind == UninstallerType.Steam))
                {
                    var toRemove =
                        steamAppsOnDisk.FindAll(
                            x =>
                                x.InstallLocation.Equals(steamApp.InstallLocation,
                                    StringComparison.InvariantCultureIgnoreCase));
                    // todo merge data later on
                    steamAppsOnDisk.RemoveAll(toRemove);
                    ChangeSteamAppUninstallStringToHelper(steamApp);

                    if (steamApp.EstimatedSize.IsDefault() && toRemove.Any())
                        steamApp.EstimatedSize = toRemove.First().EstimatedSize;
                }

                foreach (var steamApp in steamAppsOnDisk)
                {
                    ChangeSteamAppUninstallStringToHelper(steamApp);
                }

                applicationUninstallers.AddRange(steamAppsOnDisk);
            }

            return applicationUninstallers;
        }

        /// <summary>
        ///     Use our helper instead of the built-in Steam uninstaller
        /// </summary>
        private static void ChangeSteamAppUninstallStringToHelper(ApplicationUninstallerEntry entryToModify)
        {
            if (entryToModify.UninstallerKind != UninstallerType.Steam || !SteamHelperIsAvailable) return;

            var appId = entryToModify.RatingId.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries).Last();
            entryToModify.UninstallString = $"\"{SteamHelperPath}\" uninstall {appId}";
            entryToModify.QuietUninstallString = $"\"{SteamHelperPath}\" uninstall /silent {appId}";
        }

        private static IEnumerable<ApplicationUninstallerEntry> GetSteamApps()
        {
            if (!SteamHelperIsAvailable)
                yield break;

            var output = StartProcessAndReadOutput(SteamHelperPath, "list");
            if (string.IsNullOrEmpty(output) || output.Contains("error", StringComparison.InvariantCultureIgnoreCase))
                yield break;

            foreach (var idString in output.SplitNewlines(StringSplitOptions.RemoveEmptyEntries))
            {
                int appId;
                if (!int.TryParse(idString, out appId)) continue;

                output = StartProcessAndReadOutput(SteamHelperPath,
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

        public static ApplicationUninstallerEntry GetSteamUninstallerEntry()
        {
            if (string.IsNullOrEmpty(_steamLocation)) return null;

            return new ApplicationUninstallerEntry
            {
                AboutUrl = @"http://store.steampowered.com/about/",
                InstallLocation = _steamLocation,
                DisplayIcon = Path.Combine(_steamLocation, "Steam.exe"),
                DisplayName = "Steam",
                UninstallerKind = UninstallerType.Nsis,
                UninstallString = Path.Combine(_steamLocation, "uninstall.exe"),
                IsOrphaned = true,
                IsValid = File.Exists(Path.Combine(_steamLocation, "uninstall.exe")),
                InstallDate = Directory.GetCreationTime(_steamLocation),
                Publisher = "Valve Software"
            };
        }
    }
}
/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Klocman.Extensions;
using Microsoft.Win32;

namespace SteamHelper
{
    internal class SteamInstallation
    {
        private static SteamInstallation _instance;
        private IReadOnlyList<string> _steamAppsLocations;

        private SteamInstallation()
        {
            InstallationDirectory = FindSteamInstallationLocation();
            MainExecutableFilename = Path.Combine(InstallationDirectory, "Steam.exe");
        }

        public static SteamInstallation Instance => _instance ??= new SteamInstallation();

        public string InstallationDirectory { get; }

        public IReadOnlyList<string> SteamAppsLocations => _steamAppsLocations ??= FindSteamAppsLocations(InstallationDirectory).ToList().AsReadOnly();

        public string MainExecutableFilename { get; }

        private static IEnumerable<string> FindSteamAppsLocations(string installationDirectory)
        {
            var libraryLocations = new List<string> { Path.Combine(installationDirectory, @"SteamApps") };

            // The libraryfolders seems to appear in multiple locations. To be safe gather info from all of them
            foreach (var vdfPath in new[] { @"config\libraryfolders.vdf", @"SteamApps\libraryfolders.vdf" }.Select(x => Path.Combine(installationDirectory, x)))
            {
                if (!File.Exists(vdfPath)) continue;

                foreach (var line in File.ReadAllLines(vdfPath))
                {
                    // Gather key/value pairs from the file. It seems to be in a proprietary format
                    var pieces = line.Split('\"').Where(p => !string.IsNullOrWhiteSpace(p.Trim())).ToList();
                    if (pieces.Count != 2) continue;
                    // Only path matters, it specifies absolute path to the library folder
                    if (pieces[0] == "path")
                    {
                        var path = Path.Combine(pieces[1].Replace(@"\\", @"\"), "steamapps");
                        libraryLocations.Add(path);
                    }
                }
            }

            return libraryLocations.Distinct(StringComparer.CurrentCultureIgnoreCase).Where(Directory.Exists);
        }

        private static string FindSteamInstallationLocation()
        {
            foreach (var keyPath in new[] { @"SOFTWARE\Valve\Steam", @"SOFTWARE\WOW6432Node\Valve\Steam" })
            {
                using (var key = Registry.LocalMachine.OpenSubKey(keyPath))
                {
                    if (key == null) continue;

                    var path = key.GetStringSafe(@"InstallPath");
                    if (path != null && Directory.Exists(path))
                        return path;
                }
            }

            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Classes\steam\Shell\Open\Command"))
                {
                    var command = key?.GetStringSafe(null);
                    var path = Path.GetDirectoryName(Misc.SeparateArgsFromCommand(command).Key);

                    if (path != null && Directory.Exists(path))
                        return path;
                }
                throw new IOException();
            }
            catch (Exception ex)
            {
                throw new IOException(
                    "Failed to detect your Steam installation. Launch Steam, exit it gracefully, and try again.", ex);
            }
        }
    }
}
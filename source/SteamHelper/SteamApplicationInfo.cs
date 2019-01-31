/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.IO;
using System.Linq;

namespace SteamHelper
{
    internal class SteamApplicationInfo
    {
        private SteamApplicationInfo(int appId)
        {
            AppId = appId;
        }

        public static SteamApplicationInfo FromAppId(int appId)
        {
            var steam = SteamInstallation.Instance;

            DirectoryInfo dir = null;
            var appIdStr = appId.ToString("G");

            var output = new SteamApplicationInfo(appId);

            foreach (var steamAppsLocation in steam.SteamAppsLocations)
            {
                var result = Directory.GetFiles(steamAppsLocation, @"appmanifest_*.acf")
                    .FirstOrDefault(x => appIdStr.Equals(Path.GetFileNameWithoutExtension(x).Substring(12), StringComparison.InvariantCulture));
                if (!string.IsNullOrEmpty(result))
                {
                    output.ManifestPath = result;
                    dir = new DirectoryInfo(steamAppsLocation);
                    break;
                }
            }

            if (dir == null)
                throw new ArgumentException("Could not find Steam App with the ID " + appIdStr);

            //"C:\Steam\steam.exe" steam://uninstall/123
            output.UninstallString = $"\"{steam.MainExecutableFilename}\" steam://uninstall/{appIdStr}";

            var manifestStrings = File.ReadAllLines(output.ManifestPath);
            output.Name = GetManifestValue(manifestStrings, "name");
            output.SizeOnDisk = GetManifestValue(manifestStrings, "SizeOnDisk");
            var installDirName = GetManifestValue(manifestStrings, "installdir");
            if (!string.IsNullOrEmpty(installDirName))
            {
                var path = Path.Combine(Path.Combine(dir.FullName, @"common"), installDirName);
                if (Directory.Exists(path))
                    output.InstallDirectory = path;

                if (output.Name == null)
                    output.Name = installDirName;
            }

            output.DownloadDirectory = dir.GetDirectories(@"downloading").SingleOrDefault()?.GetDirectories(appIdStr).SingleOrDefault()?.FullName;

            var workshopDir = dir.GetDirectories(@"workshop").SingleOrDefault();
            if (workshopDir != null)
            {
                output.WorkshopManifestPath = workshopDir.GetFiles($@"appworkshop_{appIdStr}.acf").SingleOrDefault()?.FullName;
                output.WorkshopDirectory = workshopDir.GetDirectories(@"content").SingleOrDefault()?.GetDirectories(appIdStr).SingleOrDefault()?.FullName;
            }

            return output;
        }

        public static string GetManifestValue(string[] manifestStrings, string keyName)
        {
            var targetLine = manifestStrings
                .FirstOrDefault(x => x.IndexOf($"\"{keyName.Trim().Trim('\"')}\"", StringComparison.InvariantCultureIgnoreCase) >= 0);

            return string.IsNullOrEmpty(targetLine) ? null : targetLine.Split('\"').Select(x => x.Trim()).LastOrDefault(p => !string.IsNullOrEmpty(p?.Trim()));
        }

        public int AppId { get; }
        public string Name { get; private set; }
        public string SizeOnDisk { get; private set; }
        public string UninstallString { get; private set; }
        public string ManifestPath { get; private set; }
        public string InstallDirectory { get; private set; }
        public string DownloadDirectory { get; private set; }
        public string WorkshopManifestPath { get; private set; }
        public string WorkshopDirectory { get; private set; }
    }
}
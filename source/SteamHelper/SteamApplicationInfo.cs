/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SteamHelper
{
    internal class SteamApplicationInfo
    {
        private SteamApplicationInfo(int appId)
        {
            AppId = appId;
        }

        public static SteamApplicationInfo FromAppManifest(FileInfo manifestFile, DirectoryInfo containingAppsDir)
        {
            var m = Regex.Match(manifestFile.Name, @"appmanifest_(\d+)\.acf");
            if (m.Success)
            {
                var idStr = m.Groups[1].Value;
                if (!string.IsNullOrEmpty(idStr))
                {
                    return FromParams(int.Parse(idStr, CultureInfo.InvariantCulture), manifestFile, containingAppsDir);
                }
            }
            return null;
        }

        public static SteamApplicationInfo FromAppId(int appId)
        {
            DirectoryInfo dir = null;
            FileInfo manifestFile = null;
            var appIdStr = appId.ToString("G");

            foreach (var steamAppsLocation in SteamInstallation.Instance.SteamAppsLocations)
            {
                var result = Directory.GetFiles(steamAppsLocation, @"appmanifest_*.acf")
                    .FirstOrDefault(x => appIdStr.Equals(Path.GetFileNameWithoutExtension(x).Substring(12), StringComparison.InvariantCulture));
                if (!string.IsNullOrEmpty(result))
                {
                    manifestFile = new FileInfo(result);
                    dir = new DirectoryInfo(steamAppsLocation);
                    break;
                }
            }

            if (dir == null)
                throw new ArgumentException("Could not find Steam App with the ID " + appIdStr);

            return FromParams(appId, manifestFile, dir);
        }

        private static SteamApplicationInfo FromParams(int appId, FileInfo manifestFile, DirectoryInfo containingAppsDir)
        {
            var appIdStr = appId.ToString("G");
            var output = new SteamApplicationInfo(appId);
            output.ManifestPath = manifestFile.FullName;

            //"C:\Steam\steam.exe" steam://uninstall/123
            output.UninstallString = $"\"{SteamInstallation.Instance.MainExecutableFilename}\" steam://uninstall/{appIdStr}";

            var manifestStrings = File.ReadAllLines(output.ManifestPath);
            output.Name = GetManifestValue(manifestStrings, "name");
            output.SizeOnDisk = GetManifestValue(manifestStrings, "SizeOnDisk");
            var installDirName = GetManifestValue(manifestStrings, "installdir");
            if (!string.IsNullOrEmpty(installDirName))
            {
                var path = Path.Combine(Path.Combine(containingAppsDir.FullName, @"common"), installDirName);
                if (Directory.Exists(path))
                    output.InstallDirectory = path;

                if (output.Name == null)
                    output.Name = installDirName;
            }

            output.DownloadDirectory = containingAppsDir.GetDirectories(@"downloading").SingleOrDefault()?.GetDirectories(appIdStr).SingleOrDefault()?.FullName;

            var workshopDir = containingAppsDir.GetDirectories(@"workshop").SingleOrDefault();
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
                .FirstOrDefault(x => x.Contains($"\"{keyName.Trim().Trim('\"')}\"", StringComparison.InvariantCultureIgnoreCase));

            return string.IsNullOrEmpty(targetLine) ? null : targetLine.Split('\"').Select(x => x.Trim()).LastOrDefault(x => x.Length > 0);
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

        public void WriteTo(TextWriter wr)
        {
            foreach (var property in typeof(SteamApplicationInfo).GetProperties(BindingFlags.Public | BindingFlags.Instance))
                wr.WriteLine("{0} - {1}", property.Name, property.GetValue(this, null) ?? "N/A");
        }
    }
}
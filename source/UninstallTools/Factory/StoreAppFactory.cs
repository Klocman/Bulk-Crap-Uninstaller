/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using Klocman.Native;
using Klocman.Tools;
using UninstallTools.Properties;

namespace UninstallTools.Factory
{
    public class StoreAppFactory : IIndependantUninstallerFactory
    {
        public static string GetPowerShellRemoveCommand(string fullName)
        {
            return $"Remove-AppxPackage -package {fullName} -confirm:$false";
        }

        /// <summary>
        /// Convert supplied store app entries to power shell uninstall commands. Non-store app entries are ignored.
        /// </summary>
        public static string[] ToPowerShellRemoveCommands(IEnumerable<ApplicationUninstallerEntry> appEntries)
        {
            return appEntries.Where(x => x.UninstallerKind == UninstallerType.StoreApp)
                .Select(x => GetPowerShellRemoveCommand(x.Comment))
                .ToArray();
        }

        private static string StoreAppHelperPath => Path.Combine(UninstallToolsGlobalConfig.AssemblyLocation, @"StoreAppHelper.exe");

        public IEnumerable<ApplicationUninstallerEntry> GetUninstallerEntries(
            ListGenerationProgress.ListGenerationCallback progressCallback)
        {
            if (!WindowsTools.CheckNetFramework4Installed(true) || !File.Exists(StoreAppHelperPath))
                yield break;

            var output = FactoryTools.StartProcessAndReadOutput(StoreAppHelperPath, "/query");
            if (string.IsNullOrEmpty(output))
                yield break;

            var windowsPath = WindowsTools.GetEnvironmentPath(CSIDL.CSIDL_WINDOWS);
            
            foreach (var data in FactoryTools.ExtractAppDataSetsFromHelperOutput(output))
            {
                if (!data.ContainsKey("InstalledLocation") || !Directory.Exists(data["InstalledLocation"])) continue;

                var fullName = data["FullName"];
                var uninstallStr = $"\"{StoreAppHelperPath}\" /uninstall \"{fullName}\"";
                var isProtected = data.ContainsKey("IsProtected") && Convert.ToBoolean(data["IsProtected"], CultureInfo.InvariantCulture);
                var result = new ApplicationUninstallerEntry
                {
                    Comment = fullName,
                    CacheIdOverride = fullName,
                    RatingId = fullName.Substring(0, fullName.IndexOf("_", StringComparison.Ordinal)),
                    UninstallString = uninstallStr,
                    QuietUninstallString = uninstallStr,
                    RawDisplayName = string.IsNullOrEmpty(data["DisplayName"]) ? fullName : data["DisplayName"],
                    Publisher = data["PublisherDisplayName"],
                    IsValid = true,
                    UninstallerKind = UninstallerType.StoreApp,
                    InstallLocation = data["InstalledLocation"],
                    InstallDate = Directory.GetCreationTime(data["InstalledLocation"]),
                    IsProtected = isProtected,
                    SystemComponent = isProtected
                };

                if (File.Exists(data["Logo"]))
                {
                    try
                    {
                        result.DisplayIcon = data["Logo"];
                        result.IconBitmap = DrawingTools.IconFromImage(new Bitmap(data["Logo"]));
                    }
                    catch
                    {
                        result.DisplayIcon = null;
                        result.IconBitmap = null;
                    }
                }

                if (result.InstallLocation.StartsWith(windowsPath, StringComparison.InvariantCultureIgnoreCase))
                {
                    result.SystemComponent = true;
                    //result.IsProtected = true;
                }

                yield return result;
            }
        }

        public bool IsEnabled() => UninstallToolsGlobalConfig.ScanStoreApps;
        public string DisplayName => Localisation.Progress_AppStores_WinStore;
    }
}
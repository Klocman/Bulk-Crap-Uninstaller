/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private static string HelperPath { get; } = Path.Combine(UninstallToolsGlobalConfig.AssemblyLocation, @"StoreAppHelper.exe");
        private static bool IsHelperAvailable() => File.Exists(HelperPath);

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

        public IList<ApplicationUninstallerEntry> GetUninstallerEntries(
            ListGenerationProgress.ListGenerationCallback progressCallback)
        {
            var results = new List<ApplicationUninstallerEntry>();
            if (!IsHelperAvailable()) return results;

            var output = FactoryTools.StartHelperAndReadOutput(HelperPath, "/query");
            if (string.IsNullOrEmpty(output)) return results;

            var windowsPath = WindowsTools.GetEnvironmentPath(CSIDL.CSIDL_WINDOWS);

            foreach (var data in FactoryTools.ExtractAppDataSetsFromHelperOutput(output))
            {
                try
                {
                    if (!data.TryGetValue("InstalledLocation", out var installLocation) || !Directory.Exists(installLocation))
                        continue;
                    if (!data.TryGetValue("FullName", out var fullName) || string.IsNullOrWhiteSpace(fullName))
                        continue;

                    var separatorIndex = fullName.IndexOf("_", StringComparison.Ordinal);
                    var uninstallStr = $"\"{HelperPath}\" /uninstall \"{fullName}\"";
                    var isProtected = data.TryGetValue("IsProtected", out var isProtectedValue) &&
                                      Convert.ToBoolean(isProtectedValue, CultureInfo.InvariantCulture);

                    data.TryGetValue("DisplayName", out var displayName);
                    data.TryGetValue("PublisherDisplayName", out var publisher);

                    var result = new ApplicationUninstallerEntry
                    {
                        Comment = fullName,
                        CacheIdOverride = fullName,
                        RatingId = separatorIndex >= 0 ? fullName.Substring(0, separatorIndex) : fullName,
                        UninstallString = uninstallStr,
                        QuietUninstallString = uninstallStr,
                        RawDisplayName = string.IsNullOrEmpty(displayName) ? fullName : displayName,
                        Publisher = publisher,
                        IsValid = true,
                        UninstallerKind = UninstallerType.StoreApp,
                        InstallLocation = installLocation,
                        InstallDate = Directory.GetCreationTime(installLocation),
                        IsProtected = isProtected,
                        SystemComponent = isProtected
                    };

                    if (data.TryGetValue("Logo", out var logoPath) && File.Exists(logoPath))
                    {
                        try
                        {
                            result.DisplayIcon = logoPath;
                            result.IconBitmap = DrawingTools.IconFromImage(new Bitmap(logoPath));
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

                    results.Add(result);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine($"[Factory] Failed to parse a Store app entry from helper output: {ex}");
                }
            }

            return results;
        }

        public bool IsEnabled() => UninstallToolsGlobalConfig.ScanStoreApps;
        public string DisplayName => Localisation.Progress_AppStores_WinStore;
    }
}

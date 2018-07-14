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
using Klocman.Extensions;
using Klocman.Native;
using Klocman.Tools;

namespace UninstallTools.Factory
{
    public class StoreAppFactory : IUninstallerFactory
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
                .Select(x => GetPowerShellRemoveCommand(x.RatingId))
                .ToArray();
        }

        private static string StoreAppHelperPath => Path.Combine(UninstallToolsGlobalConfig.AssemblyLocation, @"StoreAppHelper.exe");

        public IEnumerable<ApplicationUninstallerEntry> GetUninstallerEntries(
            ListGenerationProgress.ListGenerationCallback progressCallback)
        {
            if (!WindowsTools.CheckNetFramework4Installed(true) || !File.Exists(StoreAppHelperPath))
                yield break;

            var output = SteamFactory.StartProcessAndReadOutput(StoreAppHelperPath, "/query");
            if (string.IsNullOrEmpty(output))
                yield break;

            var windowsPath = WindowsTools.GetEnvironmentPath(CSIDL.CSIDL_WINDOWS);

            // Apps are separated by empty lines
            var allParts = output.SplitNewlines(StringSplitOptions.None);
            
            while (true)
            {
                var singleAppParts = allParts.TakeWhile(x => !string.IsNullOrEmpty(x)).ToList();
                allParts = allParts.Skip(singleAppParts.Count + 1).ToArray();

                if (!singleAppParts.Any())
                {
                    if (allParts.Length == 0)
                        break;
                    continue;
                }

                var data = singleAppParts.Where(x=>x.Contains(':')).ToDictionary(
                    x => x.Substring(0, x.IndexOf(":", StringComparison.Ordinal)).Trim(),
                    x => x.Substring(x.IndexOf(":", StringComparison.Ordinal) + 1).Trim());
                
                if (data.ContainsKey("InstalledLocation") && Directory.Exists(data["InstalledLocation"]))
                {
                    var uninstallStr = $"\"{StoreAppHelperPath}\" /uninstall \"{data["FullName"]}\"";
                    var isProtected = data.ContainsKey("IsProtected") && Convert.ToBoolean(data["IsProtected"], CultureInfo.InvariantCulture);
                    var result = new ApplicationUninstallerEntry
                    {
                        RatingId = data["FullName"],
                        UninstallString = uninstallStr,
                        QuietUninstallString = uninstallStr,
                        RawDisplayName = string.IsNullOrEmpty(data["DisplayName"]) ? data["FullName"] : data["DisplayName"],
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
        }
    }
}
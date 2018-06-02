/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Drawing;
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

            var parts = output.SplitNewlines(StringSplitOptions.None);
            var current = parts.Take(5).ToList();
            while (current.Count == 5)
            {
                /*
               @"FullName: "
               @"DisplayName: "
               @"PublisherDisplayName: "
               @"Logo: "
               @"InstalledLocation: "
               */

                //Trim the labels
                for (var i = 0; i < current.Count; i++)
                    current[i] = current[i].Substring(current[i].IndexOf(" ", StringComparison.Ordinal)).Trim();

                if (Directory.Exists(current[4]))
                {
                    var uninstallStr = $"\"{StoreAppHelperPath}\" /uninstall \"{current[0]}\"";
                    var result = new ApplicationUninstallerEntry
                    {
                        RatingId = current[0],
                        UninstallString = uninstallStr,
                        QuietUninstallString = uninstallStr,
                        RawDisplayName = string.IsNullOrEmpty(current[1]) ? current[0] : current[1],
                        Publisher = current[2],
                        IsValid = true,
                        UninstallerKind = UninstallerType.StoreApp,
                        InstallLocation = current[4],
                        InstallDate = Directory.GetCreationTime(current[4])
                    };

                    if (File.Exists(current[3]))
                    {
                        try
                        {
                            result.DisplayIcon = current[3];
                            result.IconBitmap = DrawingTools.IconFromImage(new Bitmap(current[3]));
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

                parts = parts.Skip(5).ToArray();
                current = parts.Take(5).ToList();
            }
        }
    }
}
/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.Collections.Generic;
using System.IO;
using Klocman.Extensions;
using Klocman.IO;
using Klocman.Native;
using Klocman.Tools;

namespace UninstallTools.Factory
{
    /// <summary>
    /// Get uninstallers that were manually pre-defined.
    /// </summary>
    public class PredefinedFactory : IUninstallerFactory
    {
        public IEnumerable<ApplicationUninstallerEntry> GetUninstallerEntries(
            ListGenerationProgress.ListGenerationCallback progressCallback)
        {
            var items = new List<ApplicationUninstallerEntry>();
            var i = GetOneDrive();
            if (i != null)
                items.Add(i);

            var s = GetSteamUninstallerEntry();
            if (s != null)
                items.Add(s);

            return items;
        }

        private static ApplicationUninstallerEntry GetOneDrive()
        {
            var result = new ApplicationUninstallerEntry();

            // Check if installed
            try
            {
                using (var key = RegistryTools.OpenRegistryKey(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\OneDrive", false))
                {
                    result.RegistryPath = key.Name;
                    result.RegistryKeyName = key.GetKeyName();

                    result.InstallLocation = key.GetValue("CurrentVersionPath") as string;
                    if (result.InstallLocation == null || !Directory.Exists(result.InstallLocation))
                        return null;

                    result.DisplayIcon = key.GetValue("OneDriveTrigger") as string;
                    result.DisplayVersion = ApplicationEntryTools.CleanupDisplayVersion(key.GetValue("Version") as string);
                }
            }
            catch
            {
                return null;
            }

            // Check if the uninstaller is available
            var systemRoot = WindowsTools.GetEnvironmentPath(CSIDL.CSIDL_WINDOWS);
            var uninstallPath = Path.Combine(systemRoot, @"System32\OneDriveSetup.exe");
            if (!File.Exists(uninstallPath))
            {
                uninstallPath = Path.Combine(systemRoot, @"SysWOW64\OneDriveSetup.exe");
                if (!File.Exists(uninstallPath))
                    uninstallPath = null;
            }

            if (uninstallPath != null)
            {
                result.IsValid = true;
                result.UninstallString = $"\"{uninstallPath}\" /uninstall";
                result.QuietUninstallString = result.UninstallString;
                if (!File.Exists(result.DisplayIcon))
                    result.DisplayIcon = uninstallPath;
            }

            result.AboutUrl = @"https://onedrive.live.com/";
            result.RawDisplayName = "OneDrive";
            result.Publisher = "Microsoft Corporation";
            result.EstimatedSize = FileSize.FromKilobytes(1024 * 90);
            result.Is64Bit = MachineType.X86;
            result.IsRegistered = true;

            result.UninstallerKind = UninstallerType.Unknown;

            result.InstallDate = Directory.GetCreationTime(result.InstallLocation);

            if (!string.IsNullOrEmpty(result.DisplayIcon))
                result.IconBitmap = UninstallToolsGlobalConfig.TryExtractAssociatedIcon(result.DisplayIcon);

            return result;
        }

        private static ApplicationUninstallerEntry GetSteamUninstallerEntry()
        {
            if (string.IsNullOrEmpty(SteamFactory.SteamLocation)) return null;

            return new ApplicationUninstallerEntry
            {
                AboutUrl = @"http://store.steampowered.com/about/",
                InstallLocation = SteamFactory.SteamLocation,
                DisplayIcon = Path.Combine(SteamFactory.SteamLocation, "Steam.exe"),
                DisplayName = "Steam",
                UninstallerKind = UninstallerType.Nsis,
                UninstallString = Path.Combine(SteamFactory.SteamLocation, "uninstall.exe"),
                IsOrphaned = true,
                IsValid = File.Exists(Path.Combine(SteamFactory.SteamLocation, "uninstall.exe")),
                InstallDate = Directory.GetCreationTime(SteamFactory.SteamLocation),
                Publisher = "Valve Software"
            };
        }
    }
}
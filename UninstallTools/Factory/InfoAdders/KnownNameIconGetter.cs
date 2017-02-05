/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security;

namespace UninstallTools.Factory.InfoAdders
{
    public class KnownNameIconGetter : IMissingInfoAdder
    {
        private static readonly string[] IconNames =
        {
            "DisplayIcon.ico", "Icon.ico", "app.ico", "appicon.ico",
            "application.ico", "logo.ico"
        };

        public string[] RequiredValueNames { get; } = {
            nameof(ApplicationUninstallerEntry.UninstallerLocation),
            nameof(ApplicationUninstallerEntry.InstallLocation),
        };

        public bool RequiresAllValues { get; } = false;
        public bool AlwaysRun { get; } = false;

        public string[] CanProduceValueNames { get; } = {
            nameof(ApplicationUninstallerEntry.DisplayIcon),
            nameof(ApplicationUninstallerEntry.IconBitmap)
        };
        public InfoAdderPriority Priority { get; } = InfoAdderPriority.Normal;
        
        public void AddMissingInformation(ApplicationUninstallerEntry entry)
        {
            if (entry.IconBitmap != null)
                return;

            // SdbInst uninstallers do not have any executables to check
            if (entry.UninstallerKind == UninstallerType.SdbInst)
                return;

            try
            {
                // Look for icons with known names in InstallLocation and UninstallerLocation
                var query = from targetDir in new[] { entry.InstallLocation, entry.UninstallerLocation }
                    where !string.IsNullOrEmpty(targetDir) && Directory.Exists(targetDir)
                    from iconName in IconNames
                    let combinedIconPath = Path.Combine(targetDir, iconName)
                    where File.Exists(combinedIconPath)
                    select combinedIconPath;

                foreach (var iconPath in query)
                {
                    try
                    {
                        var icon = Icon.ExtractAssociatedIcon(iconPath);
                        if (icon != null)
                        {
                            entry.IconBitmap = icon;
                            entry.DisplayIcon = iconPath;
                            return;
                        }
                    }
                    catch (ArgumentException) { }
                }
            }
            catch (SecurityException) { }
            catch (UnauthorizedAccessException) { }
        }
    }
}
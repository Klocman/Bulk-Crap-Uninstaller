/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security;
using Klocman.Extensions;
using Klocman.Tools;

namespace UninstallTools.Factory.InfoAdders
{
    public class IconGetter : IMissingInfoAdder
    {
        private static readonly string[] IconNames =
        {
            "DisplayIcon.ico", "Icon.ico", "app.ico", "appicon.ico",
            "application.ico", "logo.ico"
        };

        public string[] RequiredValueNames { get; } = {
            nameof(ApplicationUninstallerEntry.DisplayIcon),
            nameof(ApplicationUninstallerEntry.UninstallerKind),
            nameof(ApplicationUninstallerEntry.UninstallerFullFilename),
            nameof(ApplicationUninstallerEntry.InstallLocation),
            nameof(ApplicationUninstallerEntry.UninstallerLocation)
        };

        public bool RequiresAllValues { get; } = false;

        public string[] CanProduceValueNames { get; } = {
            nameof(ApplicationUninstallerEntry.DisplayIcon),
            nameof(ApplicationUninstallerEntry.IconBitmap)
        };
        public InfoAdderPriority Priority { get; } = InfoAdderPriority.RunLast;

        /// <summary>
        ///     Run after DisplayIcon, DisplayName, UninstallerKind, InstallLocation, UninstallString have been initialized.
        /// </summary>
        public void AddMissingInformation(ApplicationUninstallerEntry entry)
        {
            if (entry.IconBitmap != null)
                return;

            // Check for any specified icons
            if (!string.IsNullOrEmpty(entry.DisplayIcon) && !ApplicationUninstallerFactory.PathPointsToMsiExec(entry.DisplayIcon))
            {
                string resultFilename = null;

                if (File.Exists(entry.DisplayIcon))
                    resultFilename = entry.DisplayIcon;

                if (resultFilename == null)
                {
                    try
                    {
                        var fName = ProcessTools.SeparateArgsFromCommand(entry.DisplayIcon).FileName;
                        if (fName != null && File.Exists(fName))
                        {
                            resultFilename = fName;
                        }
                    }
                    catch
                    {
                        // Ignore error and try another method
                    }
                }

                if (resultFilename != null)
                {
                    try
                    {
                        var icon = Icon.ExtractAssociatedIcon(resultFilename);
                        if (icon != null)
                        {
                            entry.DisplayIcon = resultFilename;
                            entry.IconBitmap = icon;
                            return;
                        }
                    }
                    catch (ArgumentException) { }
                }
            }

            // SdbInst uninstallers do not have any executables to check
            if (entry.UninstallerKind == UninstallerType.SdbInst)
            {
                entry.DisplayIcon = null;
                entry.IconBitmap = null;
                return;
            }

            string resultPath;
            // Check the install location first, it is most likely to have the program executables
            if (entry.IsInstallLocationValid())
            {
                var result = TryGetIconHelper(entry, out resultPath);
                if (result != null)
                {
                    entry.DisplayIcon = resultPath;
                    entry.IconBitmap = result;
                    return;
                }
            }

            // If install dir is not provided try extracting icon from the uninstaller
            if (entry.UninstallerFullFilename.IsNotEmpty() && !ApplicationUninstallerFactory.PathPointsToMsiExec(entry.UninstallerFullFilename)
                && File.Exists(entry.UninstallerFullFilename))
            {
                try
                {
                    var icon = Icon.ExtractAssociatedIcon(entry.UninstallerFullFilename);
                    if (icon != null)
                    {
                        entry.DisplayIcon = entry.UninstallerFullFilename;
                        entry.IconBitmap = icon;
                        return;
                    }
                }
                catch (ArgumentException) { }
            }

            // Finally try finding other executables in the uninstaller's dir. 
            // Check the InstallLocation again to prevent TryGetIconHelper from running twice
            if (!entry.IsInstallLocationValid())
            {
                var result = TryGetIconHelper(entry, out resultPath);
                if (result != null)
                {
                    entry.DisplayIcon = resultPath;
                    entry.IconBitmap = result;
                    return;
                }
            }

            // Nothing was found
            entry.DisplayIcon = null;
            entry.IconBitmap = null;
        }

        private static Icon TryGetIconHelper(ApplicationUninstallerEntry entry, out string path)
        {
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
                        path = iconPath;
                        return Icon.ExtractAssociatedIcon(iconPath);
                    }
                    catch (ArgumentException) { }
                }
            }
            catch (SecurityException) { }
            catch (UnauthorizedAccessException) { }

            try
            {
                // Try getting icon from the app's executables
                foreach (var executablePath in entry.GetMainExecutableCandidates())
                {
                    var icon = Icon.ExtractAssociatedIcon(executablePath);
                    if (icon == null) continue;
                    path = executablePath;
                    return icon;
                }
            }
            catch (ArgumentException) { }
            catch (SecurityException) { }
            catch (UnauthorizedAccessException) { }

            path = null;
            return null;
        }
    }
}
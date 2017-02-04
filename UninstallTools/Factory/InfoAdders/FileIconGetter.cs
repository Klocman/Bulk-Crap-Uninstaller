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
    public class FileIconGetter : IMissingInfoAdder
    {
        public string[] RequiredValueNames { get; } = {
            nameof(ApplicationUninstallerEntry.UninstallerFullFilename),
            nameof(ApplicationUninstallerEntry.SortedExecutables)
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

            // SdbInst uninstallers do not have any executables to check
            if (entry.UninstallerKind == UninstallerType.SdbInst)
                return;

            try
            {
                // Try getting an icon from the app's executables
                if (entry.SortedExecutables != null)
                {
                    foreach (var executablePath in entry.SortedExecutables.Take(2))
                    {
                        var icon = Icon.ExtractAssociatedIcon(executablePath);
                        if (icon != null)
                        {
                            entry.DisplayIcon = executablePath;
                            entry.IconBitmap = icon;
                            return;
                        }
                    }
                }
            }
            catch (ArgumentException) { }
            catch (SecurityException) { }
            catch (UnauthorizedAccessException) { }
            
            // Extract icon from the uninstaller
            if (File.Exists(entry.UninstallerFullFilename))
            {
                try
                {
                    var icon = Icon.ExtractAssociatedIcon(entry.UninstallerFullFilename);
                    if (icon != null)
                    {
                        entry.DisplayIcon = entry.UninstallerFullFilename;
                        entry.IconBitmap = icon;
                    }
                }
                catch (ArgumentException) { }
            }
        }
    }
}
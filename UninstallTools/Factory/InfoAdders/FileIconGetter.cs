/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.Linq;

namespace UninstallTools.Factory.InfoAdders
{
    public class FileIconGetter : IMissingInfoAdder
    {
        public string[] RequiredValueNames { get; } = {
            nameof(ApplicationUninstallerEntry.UninstallerFullFilename),
            nameof(ApplicationUninstallerEntry.SortedExecutables)
        };

        public bool RequiresAllValues { get; } = false;
        public bool AlwaysRun { get; } = false;

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

            // Try getting an icon from the app's executables
            if (entry.SortedExecutables != null)
            {
                foreach (var executablePath in entry.SortedExecutables.Take(2))
                {
                    var exeIcon = UninstallToolsGlobalConfig.TryExtractAssociatedIcon(executablePath);
                    if (exeIcon != null)
                    {
                        entry.DisplayIcon = executablePath;
                        entry.IconBitmap = exeIcon;
                        return;
                    }
                }
            }

            var uninsIcon = UninstallToolsGlobalConfig.TryExtractAssociatedIcon(entry.UninstallerFullFilename);
            if (uninsIcon != null)
            {
                entry.DisplayIcon = entry.UninstallerFullFilename;
                entry.IconBitmap = uninsIcon;
            }
        }
    }
}
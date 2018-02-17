/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.IO;
using Klocman.Tools;

namespace UninstallTools.Factory.InfoAdders
{
    public class BasicIconGetter :IMissingInfoAdder
    {
        public void AddMissingInformation(ApplicationUninstallerEntry entry)
        {
            if (entry.IconBitmap != null)
                return;

            // Check for any specified icons
            if (!string.IsNullOrEmpty(entry.DisplayIcon) &&
                !ApplicationEntryTools.PathPointsToMsiExec(entry.DisplayIcon))
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

                var icon = UninstallToolsGlobalConfig.TryExtractAssociatedIcon(resultFilename);
                if (icon != null)
                {
                    entry.DisplayIcon = resultFilename;
                    entry.IconBitmap = icon;
                }
            }
        }

        public string[] RequiredValueNames { get; } = {
            nameof(ApplicationUninstallerEntry.DisplayIcon)
        };

        public bool RequiresAllValues { get; } = true;
        public bool AlwaysRun { get; } = false;

        public string[] CanProduceValueNames { get; } = {
            nameof(ApplicationUninstallerEntry.DisplayIcon),
            nameof(ApplicationUninstallerEntry.IconBitmap)
        };
        public InfoAdderPriority Priority { get; } = InfoAdderPriority.RunFirst;
    }
}
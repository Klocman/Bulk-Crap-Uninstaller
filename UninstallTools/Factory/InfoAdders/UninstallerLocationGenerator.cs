/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.IO;

namespace UninstallTools.Factory.InfoAdders
{
    public class UninstallerLocationGenerator : IMissingInfoAdder
    {
        public void AddMissingInformation(ApplicationUninstallerEntry target)
        {
            if (string.IsNullOrEmpty(target.UninstallerFullFilename))
                return;

            try
            {
                target.UninstallerLocation = Path.GetDirectoryName(target.UninstallerFullFilename);
            }
            catch (ArgumentException)
            {
            }
            catch (PathTooLongException)
            {
            }
        }

        public string[] RequiredValueNames { get; } = { nameof(ApplicationUninstallerEntry.UninstallerFullFilename) };
        public bool RequiresAllValues { get; } = true;
        public bool AlwaysRun { get; } = false;
        public string[] CanProduceValueNames { get; } = { nameof(ApplicationUninstallerEntry.UninstallerLocation) };
        public InfoAdderPriority Priority { get; } = InfoAdderPriority.Normal;
    }
}

/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.IO;

namespace UninstallTools.Factory.InfoAdders
{
    public class InstallDateAdder : IMissingInfoAdder
    {
        public void AddMissingInformation(ApplicationUninstallerEntry target)
        {
            try
            {
                if (File.Exists(target.UninstallerFullFilename))
                    target.InstallDate = File.GetCreationTime(target.UninstallerFullFilename);
                else if (Directory.Exists(target.InstallLocation))
                    target.InstallDate = Directory.GetCreationTime(target.InstallLocation);
            }
            catch
            {
                target.InstallDate = DateTime.MinValue;
            }
        }

        public string[] RequiredValueNames { get; } = {
            nameof(ApplicationUninstallerEntry.InstallLocation),
            nameof(ApplicationUninstallerEntry.UninstallerFullFilename)
        };
        public bool RequiresAllValues { get; } = false;

        public string[] CanProduceValueNames { get; } = {
            nameof(ApplicationUninstallerEntry.InstallDate)
        };

        public InfoAdderPriority Priority { get; } = InfoAdderPriority.RunLast;
    }
}
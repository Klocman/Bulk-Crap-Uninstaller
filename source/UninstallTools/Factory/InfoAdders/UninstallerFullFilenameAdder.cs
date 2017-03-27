/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Klocman.Tools;

namespace UninstallTools.Factory.InfoAdders
{

    public class UninstallerFullFilenameAdder : IMissingInfoAdder
    {
        public void AddMissingInformation(ApplicationUninstallerEntry target)
        {
            target.UninstallerFullFilename =
                GetUninstallerFilename(target.UninstallString ?? target.QuietUninstallString);
        }

        public string[] RequiredValueNames { get; } = {
            nameof(ApplicationUninstallerEntry.UninstallString),
            nameof(ApplicationUninstallerEntry.QuietUninstallString)
        };
        public bool RequiresAllValues { get; } = false;
        public bool AlwaysRun { get; } = false;

        public string[] CanProduceValueNames { get; } = {
            nameof(ApplicationUninstallerEntry.UninstallerFullFilename)
        };

        public InfoAdderPriority Priority { get; } = InfoAdderPriority.Normal;

        private static string GetUninstallerFilename(string uninstallString)
        {
            if (!string.IsNullOrEmpty(uninstallString))
            {
                try
                {
                    var fileName = ProcessTools.SeparateArgsFromCommand(uninstallString).FileName;

                    Debug.Assert(!fileName.Contains(' ') || File.Exists(fileName));

                    return fileName;
                }
                catch (ArgumentException) { }
                catch (FormatException) { }
            }

            return string.Empty;
        }
    }
}
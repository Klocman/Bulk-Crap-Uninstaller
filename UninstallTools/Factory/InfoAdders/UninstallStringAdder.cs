/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using Klocman.Tools;

namespace UninstallTools.Factory.InfoAdders
{
    public static class UninstallStringAdder
    {
        /// <summary>
        /// Add missing uninstall strings based on UninstallerKind of the targetEntry
        /// TODO combine with other stuff, has ApplyMsiInfo call
        /// </summary>
        public static void GenerateUninstallStrings(ApplicationUninstallerEntry targetEntry)
        {
            switch (targetEntry.UninstallerKind)
            {
                case UninstallerType.InnoSetup:
                    targetEntry.QuietUninstallString = $"\"{targetEntry.UninstallString}\" /SILENT";
                    break;

                case UninstallerType.Msiexec:
                    Guid resultGuid;
                    if (GuidTools.TryExtractGuid(targetEntry.UninstallString, out resultGuid))
                    {
                        targetEntry.BundleProviderKey = resultGuid;
                        MsiInfoAdder.ApplyMsiInfo(targetEntry, resultGuid);
                    }
                    break;

                default:
                    // Generate uninstall commands if no uninstaller has been found
                    if (String.IsNullOrEmpty(targetEntry.UninstallString))
                    {
                        targetEntry.UninstallString = $"cmd.exe /C del /S \"{targetEntry.InstallLocation}\\\" && pause";
                        targetEntry.QuietUninstallString = $"cmd.exe /C del /F /S /Q \"{targetEntry.InstallLocation}\\\"";
                    }
                    break;
            }
        }
    }
}
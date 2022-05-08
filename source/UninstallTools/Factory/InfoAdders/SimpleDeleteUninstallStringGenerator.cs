/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Diagnostics;
using System.IO;

namespace UninstallTools.Factory.InfoAdders
{
    public class SimpleDeleteUninstallStringGenerator : IMissingInfoAdder
    {
        static SimpleDeleteUninstallStringGenerator()
        {
            try
            {
                UniversalUninstallerFilename = new FileInfo(
                    Path.Combine(UninstallToolsGlobalConfig.AssemblyLocation, "UniversalUninstaller.exe"));

                UniversalUninstallerIsAvailable = UniversalUninstallerFilename.Exists;
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);

                UniversalUninstallerFilename = null;
                UniversalUninstallerIsAvailable = false;
            }
        }

        public static FileInfo UniversalUninstallerFilename { get; }
        public static bool UniversalUninstallerIsAvailable { get; }

        public void AddMissingInformation(ApplicationUninstallerEntry target)
        {
            if (target.UninstallerKind != UninstallerType.SimpleDelete)
                return;

            if (target.UninstallString == null)
            {
                target.UninstallString = UniversalUninstallerIsAvailable
                    ? GetNewUninstallString(target.InstallLocation, false)
                    : GetOldSimpleDeleteString(target.InstallLocation, false);
            }

            if (target.QuietUninstallString == null)
            {
                target.QuietUninstallString = UniversalUninstallerIsAvailable
                    ? GetNewUninstallString(target.InstallLocation, true)
                    : GetOldSimpleDeleteString(target.InstallLocation, true);
            }
        }

        public string[] RequiredValueNames { get; } = {
            nameof(ApplicationUninstallerEntry.UninstallerKind),
            nameof(ApplicationUninstallerEntry.InstallLocation)
        };

        public bool RequiresAllValues { get; } = true;
        public bool AlwaysRun { get; } = false;

        public string[] CanProduceValueNames { get; } = {
            nameof(ApplicationUninstallerEntry.UninstallString),
            nameof(ApplicationUninstallerEntry.QuietUninstallString)
        };

        public InfoAdderPriority Priority { get; } = InfoAdderPriority.RunDeadLast;

        private static string GetNewUninstallString(string installLocation, bool quiet)
        {
            return quiet
                ? $"\"{UniversalUninstallerFilename.FullName}\" /Q \"{installLocation}\\\""
                : $"\"{UniversalUninstallerFilename.FullName}\" \"{installLocation}\\\"";
        }

        private static string GetOldSimpleDeleteString(string installLocation, bool quiet)
        {
            return quiet
                ? $"cmd.exe /C del /F /S /Q \"{installLocation}\\\""
                : $"cmd.exe /C del /S \"{installLocation}\\\" && pause";
        }
    }
}
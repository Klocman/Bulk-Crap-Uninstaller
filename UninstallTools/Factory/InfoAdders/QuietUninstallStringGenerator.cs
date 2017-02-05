/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.IO;
using Klocman.Extensions;
using Klocman.Tools;

namespace UninstallTools.Factory.InfoAdders
{
    public class QuietUninstallStringGenerator : IMissingInfoAdder
    {
        private static string UninstallerAutomatizerPath
            => Path.Combine(UninstallToolsGlobalConfig.AssemblyLocation, @"UninstallerAutomatizer.exe");
        
        public void AddMissingInformation(ApplicationUninstallerEntry target)
        {
            if (target.QuietUninstallPossible || !target.UninstallPossible)
                return;

            switch (target.UninstallerKind)
            {
                case UninstallerType.Nsis:
                    if (!UninstallToolsGlobalConfig.QuietAutomatization || !File.Exists(UninstallerAutomatizerPath))
                        return;

                    var nsisCommandStart = $"\"{UninstallerAutomatizerPath}\" {UninstallerType.Nsis} ";
                    if (UninstallToolsGlobalConfig.QuietAutomatizationKillStuck)
                        nsisCommandStart = nsisCommandStart.Append("/K ");
                    target.QuietUninstallString = nsisCommandStart + target.UninstallString;
                    break;

                case UninstallerType.InnoSetup:
                    try
                    {
                        // Get rid of quotes and arguments that are already there for some weird reason (InnoSetup doesn't need any arguments)
                        target.QuietUninstallString =
                            $"\"{ProcessTools.SeparateArgsFromCommand(target.UninstallString).FileName}\" /SILENT";
                    }
                    catch (ArgumentException)
                    {
                    }
                    catch (FormatException)
                    {
                    }
                    break;
            }
        }

        public string[] RequiredValueNames { get; } = {
            nameof(ApplicationUninstallerEntry.UninstallString),
            nameof(ApplicationUninstallerEntry.UninstallerKind)
        };

        public bool RequiresAllValues { get; } = true;
        public bool AlwaysRun { get; } = false;

        public string[] CanProduceValueNames { get; } = {
            nameof(ApplicationUninstallerEntry.QuietUninstallString)
        };

        public InfoAdderPriority Priority { get; } = InfoAdderPriority.Normal;
    }
}
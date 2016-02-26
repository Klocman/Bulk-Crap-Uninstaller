using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Klocman.Extensions;
using Klocman.Tools;

namespace UninstallTools.Uninstaller
{
    public static class QuietUninstallTools
    {
        private static string _assemblyLocation;
        private static string AssemblyLocation
        {
            get
            {
                if (_assemblyLocation == null)
                {
                    _assemblyLocation = Assembly.GetExecutingAssembly().Location;
                    if (_assemblyLocation.ContainsAny(new[] { ".dll", ".exe" }, StringComparison.OrdinalIgnoreCase))
                        _assemblyLocation = PathTools.GetDirectory(_assemblyLocation);
                }
                return _assemblyLocation;
            }
        }

        private static string UninstallerAutomatizerPath => Path.Combine(AssemblyLocation, @"UninstallerAutomatizer.exe");

        /// <summary>
        /// Generate missing quiet commands if possible
        /// </summary>
        /// <param name="entries">Entries to generate the quiet commands for</param>
        /// <param name="automatizerKillstuck">Generated entries should kill the process if it gets stuck waiting for user input or the process otherwise fails</param>
        public static void GenerateQuietCommands(IEnumerable<ApplicationUninstallerEntry> entries, bool automatizerKillstuck)
        {
            var nsisCommandStart = $"\"{UninstallerAutomatizerPath}\" {UninstallerType.Nsis} ";
            if (automatizerKillstuck)
                nsisCommandStart = nsisCommandStart.Append("/K ");

            foreach (var uninstallerEntry in entries)
            {
                if (uninstallerEntry.QuietUninstallPossible || !uninstallerEntry.UninstallPossible)
                    continue;

                switch (uninstallerEntry.UninstallerKind)
                {
                    case UninstallerType.Nsis:
                        uninstallerEntry.QuietUninstallString = nsisCommandStart + uninstallerEntry.UninstallString;
                        break;

                        case UninstallerType.InnoSetup:
                        try
                        {
                            uninstallerEntry.QuietUninstallString =
                                $"\"{ProcessTools.SeparateArgsFromCommand(uninstallerEntry.UninstallString).FileName}\" /SILENT";
                        }
                        catch (ArgumentException)
                        { }
                        catch (FormatException)
                        { }
                        break;

                }
            }
        }
    }
}

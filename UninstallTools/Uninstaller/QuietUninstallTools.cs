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
        /// <param name="automaticallyKillstuck">Generated entries should kill the process if it gets stuck waiting for user input or the process otherwise fails</param>
        public static void GenerateQuietCommands(IEnumerable<ApplicationUninstallerEntry> entries, bool automaticallyKillstuck)
        {
            var commandStart = $"\"{UninstallerAutomatizerPath}\" {UninstallerType.Nsis} ";
            if (automaticallyKillstuck)
                commandStart = commandStart.Append("/K ");

            foreach (var uninstallerEntry in entries)
            {
                if (uninstallerEntry.UninstallerKind != UninstallerType.Nsis ||
                    uninstallerEntry.QuietUninstallPossible ||
                    !uninstallerEntry.UninstallPossible)
                    continue;

                uninstallerEntry.QuietUninstallString = commandStart + uninstallerEntry.UninstallString;
            }
        }
    }
}

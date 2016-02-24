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

        public static void GenerateQuietCommands(IEnumerable<ApplicationUninstallerEntry> entries)
        {
            var commandStart = $"\"{UninstallerAutomatizerPath}\" {UninstallerType.Nsis}";

            foreach (var uninstallerEntry in entries)
            {
                if (uninstallerEntry.UninstallerKind != UninstallerType.Nsis || 
                    uninstallerEntry.QuietUninstallPossible || 
                    !uninstallerEntry.UninstallPossible)
                    continue;

                uninstallerEntry.QuietUninstallString = commandStart + " " + uninstallerEntry.UninstallString;
            }
        }
    }
}

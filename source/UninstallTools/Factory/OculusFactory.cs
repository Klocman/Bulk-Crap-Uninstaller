/*
    Copyright (c) 2018 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.IO;
using Klocman.Extensions;
using Klocman.Tools;
using UninstallTools.Factory.InfoAdders;

namespace UninstallTools.Factory
{
    public class OculusFactory : IUninstallerFactory
    {
        private static bool? _helperAvailable;
        private static string HelperPath => Path.Combine(UninstallToolsGlobalConfig.AssemblyLocation, @"OculusHelper.exe");

        private static bool HelperAvailable
        {
            get
            {
                if (!_helperAvailable.HasValue)
                    _helperAvailable = WindowsTools.CheckNetFramework4Installed(true) && File.Exists(HelperPath);

                return _helperAvailable.Value;
            }
        }

        public IEnumerable<ApplicationUninstallerEntry> GetUninstallerEntries(ListGenerationProgress.ListGenerationCallback progressCallback)
        {
            if (!HelperAvailable) yield break;

            var output = FactoryTools.StartProcessAndReadOutput(HelperPath, "/query");
            if (string.IsNullOrEmpty(output))
                yield break;

            foreach (var data in FactoryTools.ExtractAppDataSetsFromHelperOutput(output))
            {
                if (!data.ContainsKey("CanonicalName")) continue;
                var name = data["CanonicalName"];
                if (string.IsNullOrEmpty(name)) continue;

                var uninstallStr = $"\"{HelperPath}\" /uninstall {name}";

                var entry = new ApplicationUninstallerEntry
                {
                    RatingId = name,
                    RegistryKeyName = name,
                    UninstallString = uninstallStr,
                    QuietUninstallString = uninstallStr,
                    IsValid = true,
                    UninstallerKind = UninstallerType.Oculus,
                    InstallLocation = data["InstallLocation"],
                    InstallDate = Directory.GetCreationTime(data["InstallLocation"]),
                    DisplayVersion = data["Version"],
                    IsProtected = "true".Equals(data["IsCore"], StringComparison.OrdinalIgnoreCase),
                };

                var executable = data["LaunchFile"];
                if (File.Exists(executable))
                    ExecutableAttributeExtractor.FillInformationFromFileAttribs(entry, executable, true);

                if (string.IsNullOrEmpty(entry.RawDisplayName))
                    entry.RawDisplayName = name.Replace('-', ' ').ToTitleCase();

                yield return entry;
            }
        }
    }
}
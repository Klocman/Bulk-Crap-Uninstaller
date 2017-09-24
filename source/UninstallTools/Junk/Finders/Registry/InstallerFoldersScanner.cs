/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;
using UninstallTools.Properties;

namespace UninstallTools.Junk
{
    public class InstallerFoldersScanner : JunkCreatorBase
    {
        public override IEnumerable<IJunkResult> FindJunk(ApplicationUninstallerEntry target)
        {
            var installLocation = target.InstallLocation;
            if (string.IsNullOrEmpty(installLocation)) yield break;

            using (var key = Registry.LocalMachine.OpenSubKey(
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Installer\Folders"))
            {
                if (key == null) yield break;

                foreach (var valueName in key.GetValueNames())
                {
                    if (!SubPathIsInsideBasePath(installLocation, valueName)) continue;

                    var node = new RegistryValueJunk(key.Name, valueName, target, this);
                    node.Confidence.Add(ConfidenceRecord.ExplicitConnection);

                    if (GetOtherInstallLocations(target).Any(x => SubPathIsInsideBasePath(x, valueName)))
                        node.Confidence.Add(ConfidenceRecord.DirectoryStillUsed);

                    yield return node;
                }
            }
        }

        public override string CategoryName => Localisation.Junk_Drive_GroupName;
    }
}
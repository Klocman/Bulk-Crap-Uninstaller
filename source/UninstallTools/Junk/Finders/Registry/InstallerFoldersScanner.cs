/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.Collections.Generic;
using System.Linq;
using Klocman.Tools;
using UninstallTools.Junk.Confidence;
using UninstallTools.Junk.Containers;
using UninstallTools.Properties;

namespace UninstallTools.Junk.Finders.Registry
{
    public class InstallerFoldersScanner : JunkCreatorBase
    {
        public override IEnumerable<IJunkResult> FindJunk(ApplicationUninstallerEntry target)
        {
            var installLocation = target.InstallLocation;
            if (string.IsNullOrEmpty(installLocation)) yield break;

            using (var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Installer\Folders"))
            {
                if (key == null) yield break;

                foreach (var valueName in key.GetValueNames())
                {
                    if (!PathTools.SubPathIsInsideBasePath(installLocation, valueName, true)) continue;

                    var node = new RegistryValueJunk(key.Name, valueName, target, this);
                    node.Confidence.Add(ConfidenceRecords.ExplicitConnection);

                    if (GetOtherInstallLocations(target).Any(x => PathTools.SubPathIsInsideBasePath(x, valueName, true)))
                        node.Confidence.Add(ConfidenceRecords.DirectoryStillUsed);

                    yield return node;
                }
            }
        }

        public override string CategoryName => Localisation.Junk_InstalledFolders_GroupName;
    }
}
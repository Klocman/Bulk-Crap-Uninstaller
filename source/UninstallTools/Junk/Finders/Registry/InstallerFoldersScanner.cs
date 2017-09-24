/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;

namespace UninstallTools.Junk
{
    public class InstallerFoldersScanner : JunkCreatorBase
    {
        public override IEnumerable<JunkNode> FindJunk(ApplicationUninstallerEntry target)
        {
            var installLocation = target.InstallLocation;
            if (string.IsNullOrEmpty(installLocation)) yield break;

            using (var key = Registry.LocalMachine.OpenSubKey(
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Installer\Folders"))
            {
                if (key == null) yield break;

                foreach (var path in key.GetValueNames())
                {
                    if (!SubPathIsInsideBasePath(installLocation, path)) continue;

                    var node = new RegistryValueJunkNode(key.Name, path, target.DisplayName);
                    node.Confidence.Add(ConfidenceRecord.ExplicitConnection);

                    if (GetOtherInstallLocations(target).Any(x => SubPathIsInsideBasePath(x, path)))
                        node.Confidence.Add(ConfidenceRecord.DirectoryStillUsed);

                    yield return node;
                }
            }
        }
    }
}
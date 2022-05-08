/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Klocman.Extensions;
using UninstallTools.Junk.Confidence;
using UninstallTools.Junk.Containers;
using UninstallTools.Properties;

namespace UninstallTools.Junk.Finders.Drive
{
    public class UninstallerLocationScanner : JunkCreatorBase
    {
        private IEnumerable<string> _allProgramFiles;

        public override void Setup(ICollection<ApplicationUninstallerEntry> allUninstallers)
        {
            _allProgramFiles = UninstallToolsGlobalConfig.GetAllProgramFiles().ToList();
            base.Setup(allUninstallers);
        }

        public override IEnumerable<IJunkResult> FindJunk(ApplicationUninstallerEntry target)
        {
            var uninLoc = target.UninstallerLocation;
            if (string.IsNullOrEmpty(uninLoc)) yield break;
            
            if (_allProgramFiles.Any(x => uninLoc.StartsWith(x, StringComparison.InvariantCultureIgnoreCase))
                && !CheckIfDirIsStillUsed(uninLoc, GetOtherInstallLocations(target)))
            {
                var resultNode = GetJunkNodeFromLocation(Enumerable.Empty<string>(), uninLoc, target);
                if (resultNode != null)
                    yield return resultNode;
            }
            else if (target.UninstallerKind == UninstallerType.Msiexec && !target.BundleProviderKey.IsEmpty())
            {
                FileSystemInfo[] matchedItems;
                try
                {
                    var winInstallerDir = new DirectoryInfo(uninLoc);

                    matchedItems = winInstallerDir.GetFileSystemInfos($"*{target.BundleProviderKey}*");
                }
                catch (SystemException e)
                {
                    Trace.WriteLine(e);
                    yield break;
                }

                foreach (var fileSystemInfo in matchedItems)
                {
                    var junk = new FileSystemJunk(fileSystemInfo, target, this);
                    junk.Confidence.Add(ConfidenceRecords.ExplicitConnection);
                    yield return junk;
                }
            }
        }

        public override string CategoryName => Localisation.Junk_UninstallerLocation_GroupName;
    }
}
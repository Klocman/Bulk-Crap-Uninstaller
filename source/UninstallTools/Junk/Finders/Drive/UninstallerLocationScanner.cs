/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Linq;
using Klocman.Extensions;
using UninstallTools.Junk.Containers;
using UninstallTools.Properties;

namespace UninstallTools.Junk.Finders.Drive
{
    public class UninstallerLocationScanner : JunkCreatorBase
    {
        public override IEnumerable<IJunkResult> FindJunk(ApplicationUninstallerEntry target)
        {
            var uninLoc = target.UninstallerLocation;
            if (!uninLoc.IsNotEmpty()) yield break;

            if (UninstallToolsGlobalConfig.GetAllProgramFiles().Any(x => uninLoc.StartsWith(x, StringComparison.InvariantCultureIgnoreCase))
                && !CheckIfDirIsStillUsed(uninLoc, GetOtherInstallLocations(target)))
            {
                var resultNode = GetJunkNodeFromLocation(Enumerable.Empty<string>(), uninLoc, target);
                if (resultNode != null)
                    yield return resultNode;
            }
        }

        public override string CategoryName => Localisation.Junk_UninstallerLocation_GroupName;
    }
}
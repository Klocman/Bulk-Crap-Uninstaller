/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.Collections.Generic;

namespace UninstallTools.Junk
{
    public class InstallLocationScanner : JunkCreatorBase
    {
        public override IEnumerable<JunkNode> FindJunk(ApplicationUninstallerEntry target)
        {
            if (!target.IsInstallLocationValid()) yield break;

            var resultNode = DriveJunk.GetJunkNodeFromLocation(GetOtherInstallLocations(target), target.InstallLocation, target.DisplayName);
            if (resultNode != null)
            {
                if (target.UninstallerKind == UninstallerType.StoreApp)
                    resultNode.Confidence.Add(ConfidencePart.IsStoreApp);
                yield return (resultNode);
            }
        }
    }
}
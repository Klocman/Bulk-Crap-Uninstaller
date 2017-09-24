/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.Collections.Generic;
using Klocman.Tools;

namespace UninstallTools.Junk
{
    public class UninstallerKeySearcher : IJunkCreator
    {
        public void Setup(ICollection<ApplicationUninstallerEntry> allUninstallers)
        {
        }

        public IEnumerable<JunkNode> FindJunk(ApplicationUninstallerEntry target)
        {
            if (!target.RegKeyStillExists()) yield break;

            var regKeyNode = new RegistryKeyJunkNode(PathTools.GetDirectory(target.RegistryPath),
                target.RegistryKeyName, target.DisplayName);
            regKeyNode.Confidence.Add(ConfidenceRecord.IsUninstallerRegistryKey);
            yield return regKeyNode;
        }
    }
}
/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.Collections.Generic;
using UninstallTools.Junk.Confidence;
using UninstallTools.Junk.Containers;
using UninstallTools.Properties;

namespace UninstallTools.Junk.Finders.Registry
{
    public class UninstallerKeySearcher : IJunkCreator
    {
        public void Setup(ICollection<ApplicationUninstallerEntry> allUninstallers)
        {
        }

        public IEnumerable<IJunkResult> FindJunk(ApplicationUninstallerEntry target)
        {
            if (!target.RegKeyStillExists()) yield break;

            var regKeyNode = new RegistryKeyJunk(target.RegistryPath, target, this);
            regKeyNode.Confidence.Add(ConfidenceRecord.IsUninstallerRegistryKey);
            yield return regKeyNode;
        }

        public string CategoryName => Localisation.Junk_UninstallerKey_GroupName;
    }
}
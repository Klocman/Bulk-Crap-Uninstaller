/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.IO;
using Klocman.Extensions;
using Klocman.Tools;

namespace UninstallTools.Junk
{
    public class AudioPolicyConfigScanner:IJunkCreator
    {
        public void Setup(ICollection<ApplicationUninstallerEntry> allUninstallers)
        {
        }

        public IEnumerable<JunkNode> FindJunk(ApplicationUninstallerEntry target)
        {
            var returnList = new List<JunkNode>();

            if (string.IsNullOrEmpty(target.InstallLocation))
                return returnList;

            var pathRoot = Path.GetPathRoot(target.InstallLocation);
            var unrootedLocation = pathRoot.Length >= 1
                ? target.InstallLocation.Replace(pathRoot, string.Empty)
                : target.InstallLocation;

            if (string.IsNullOrEmpty(unrootedLocation.Trim()))
                return returnList;

            using (var key = RegistryTools.OpenRegistryKey(Path.Combine(SoftwareRegKeyScanner.KeyCu,
                @"Microsoft\Internet Explorer\LowRegistry\Audio\PolicyConfig\PropertyStore")))
            {
                if (key == null)
                    return returnList;

                foreach (var subKeyName in key.GetSubKeyNames())
                {
                    using (var subKey = key.OpenSubKey(subKeyName))
                    {
                        if (subKey == null) continue;

                        var defVal = subKey.GetValue(null) as string;
                        if (defVal != null &&
                            defVal.Contains(unrootedLocation, StringComparison.InvariantCultureIgnoreCase))
                        {
                            var junk = new RegistryKeyJunkNode(key.Name, subKeyName, target.DisplayName);
                            junk.Confidence.Add(ConfidencePart.ExplicitConnection);
                            returnList.Add(junk);
                        }
                    }
                }
            }

            return returnList;
        }
    }
}
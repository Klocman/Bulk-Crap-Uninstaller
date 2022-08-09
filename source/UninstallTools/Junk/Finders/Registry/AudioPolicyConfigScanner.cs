/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Klocman.Extensions;
using Klocman.Tools;
using UninstallTools.Junk.Confidence;
using UninstallTools.Junk.Containers;
using UninstallTools.Properties;

namespace UninstallTools.Junk.Finders.Registry
{
    public class AudioPolicyConfigScanner : IJunkCreator
    {
        private static readonly string AudioPolicyConfigSubkey = @"Microsoft\Internet Explorer\LowRegistry\Audio\PolicyConfig\PropertyStore";

        public void Setup(ICollection<ApplicationUninstallerEntry> allUninstallers)
        {
        }

        public IEnumerable<IJunkResult> FindJunk(ApplicationUninstallerEntry target)
        {
            var returnList = new List<IJunkResult>();

            if (string.IsNullOrEmpty(target.InstallLocation))
                return returnList;
            
            string pathRoot;

            try
            {
                pathRoot = Path.GetPathRoot(target.InstallLocation) ?? throw new ArgumentException("No path root for " + target.InstallLocation);
            }
            catch (SystemException ex)
            {
                Trace.WriteLine(ex);
                return returnList;
            }

            var unrootedLocation = pathRoot.Length >= 1
                ? target.InstallLocation.Replace(pathRoot, string.Empty)
                : target.InstallLocation;

            if (string.IsNullOrEmpty(unrootedLocation.Trim()))
                return returnList;

            using (var key = RegistryTools.OpenRegistryKey(Path.Combine(SoftwareRegKeyScanner.KeyCu, AudioPolicyConfigSubkey)))
            {
                if (key == null)
                    return returnList;

                foreach (var subKeyName in key.GetSubKeyNames())
                {
                    using (var subKey = key.OpenSubKey(subKeyName))
                    {
                        if (subKey == null) continue;

                        var defVal = subKey.GetStringSafe(null);
                        if (defVal != null &&
                            defVal.Contains(unrootedLocation, StringComparison.InvariantCultureIgnoreCase))
                        {
                            var junk = new RegistryKeyJunk(subKey.Name, target, this);
                            junk.Confidence.Add(ConfidenceRecords.ExplicitConnection);
                            returnList.Add(junk);
                        }
                    }
                }
            }

            return returnList;
        }

        public string CategoryName => Localisation.Junk_AudioPolicy_GroupName;
    }
}
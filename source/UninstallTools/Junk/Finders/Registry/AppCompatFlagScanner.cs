/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Linq;
using Klocman.Tools;
using UninstallTools.Junk.Confidence;
using UninstallTools.Junk.Containers;
using UninstallTools.Properties;

namespace UninstallTools.Junk.Finders.Registry
{
    public class AppCompatFlagScanner : IJunkCreator
    {
        private static readonly IEnumerable<string> AppCompatFlags = new[]
        {
            @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\AppCompatFlags",
            @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows NT\CurrentVersion\AppCompatFlags"
        };

        public void Setup(ICollection<ApplicationUninstallerEntry> allUninstallers)
        {
        }

        public IEnumerable<IJunkResult> FindJunk(ApplicationUninstallerEntry target)
        {
            if (string.IsNullOrEmpty(target.InstallLocation))
                yield break;

            foreach (var fullCompatKey in AppCompatFlags.SelectMany(compatKey => new[]
            {
                compatKey + @"\Layers",
                compatKey + @"\Compatibility Assistant\Store"
            }))
            {
                using (var key = RegistryTools.OpenRegistryKey(fullCompatKey))
                {
                    if (key == null)
                        continue;

                    foreach (var valueName in key.GetValueNames())
                    {
                        // Check for matches
                        if (valueName.StartsWith(target.InstallLocation,
                            StringComparison.InvariantCultureIgnoreCase))
                        {
                            var junk = new RegistryValueJunk(key.Name, valueName, target, this);
                            junk.Confidence.Add(ConfidenceRecords.ExplicitConnection);
                            yield return junk;
                        }
                    }
                }
            }
        }

        public string CategoryName => Localisation.Junk_AppCompat_GroupName;
    }
}
/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Klocman.Extensions;
using Klocman.Tools;
using UninstallTools.Junk.Confidence;
using UninstallTools.Junk.Containers;
using UninstallTools.Properties;

namespace UninstallTools.Junk.Finders.Registry
{
    public class FirewallRuleScanner : JunkCreatorBase
    {
        private const string FirewallRulesKey = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\SharedAccess\Parameters\FirewallPolicy\FirewallRules";

        public override IEnumerable<IJunkResult> FindJunk(ApplicationUninstallerEntry target)
        {
            var results = new List<IJunkResult>();
            if (string.IsNullOrEmpty(target.InstallLocation))
                return results;

            using (var key = GetFirewallRulesKey())
            {
                if (key != null)
                {
                    foreach (var valueName in key.TryGetValueNames())
                    {
                        var value = key.GetStringSafe(valueName);
                        if (string.IsNullOrEmpty(value)) continue;

                        var start = value.IndexOf("|App=", StringComparison.InvariantCultureIgnoreCase) + 5;
                        var charCount = value.IndexOf('|', start) - start;
                        var fullPath = Environment.ExpandEnvironmentVariables(value.Substring(start, charCount));
                        if (fullPath.StartsWith(target.InstallLocation, StringComparison.InvariantCultureIgnoreCase))
                        {
                            var node = new RegistryValueJunk(FirewallRulesKey, valueName,
                                target, this);
                            node.Confidence.Add(ConfidenceRecords.ExplicitConnection);
                            results.Add(node);
                        }
                    }
                }
            }

            return results;
        }

        private static Microsoft.Win32.RegistryKey GetFirewallRulesKey()
        {
            try
            {
                return RegistryTools.OpenRegistryKey(FirewallRulesKey);
            }
            catch (SystemException ex)
            {
                Trace.WriteLine("Failed to get firewall rule registry key: " + ex);
                return null;
            }
        }

        public override string CategoryName => Localisation.Junk_FirewallRule_GroupName;
    }
}
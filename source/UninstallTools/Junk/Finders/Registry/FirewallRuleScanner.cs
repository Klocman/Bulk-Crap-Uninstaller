/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using Klocman.Extensions;
using UninstallTools.Junk.Confidence;
using UninstallTools.Junk.Containers;
using UninstallTools.Properties;

namespace UninstallTools.Junk.Finders.Registry
{
    public class FirewallRuleScanner : JunkCreatorBase
    {
        public override IEnumerable<IJunkResult> FindJunk(ApplicationUninstallerEntry target)
        {
            const string firewallRulesKey =
                @"SYSTEM\CurrentControlSet\Services\SharedAccess\Parameters\FirewallPolicy\FirewallRules";
            const string fullFirewallRulesKey = @"HKEY_LOCAL_MACHINE\" + firewallRulesKey;

            var results = new List<IJunkResult>();
            if (string.IsNullOrEmpty(target.InstallLocation))
                return results;

            using (var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(firewallRulesKey))
            {
                if (key != null)
                {
                    foreach (var valueName in key.TryGetValueNames())
                    {
                        var value = key.GetValue(valueName) as string;
                        if (string.IsNullOrEmpty(value)) continue;

                        var start = value.IndexOf("|App=", StringComparison.InvariantCultureIgnoreCase) + 5;
                        var charCount = value.IndexOf('|', start) - start;
                        var fullPath = Environment.ExpandEnvironmentVariables(value.Substring(start, charCount));
                        if (fullPath.StartsWith(target.InstallLocation, StringComparison.InvariantCultureIgnoreCase))
                        {
                            var node = new RegistryValueJunk(fullFirewallRulesKey, valueName,
                                target, this);
                            node.Confidence.Add(ConfidenceRecords.ExplicitConnection);
                            results.Add(node);
                        }
                    }
                }
            }

            return results;
        }

        public override string CategoryName => Localisation.Junk_FirewallRule_GroupName;
    }
}
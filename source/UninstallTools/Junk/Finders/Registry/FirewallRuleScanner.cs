/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using Klocman.Extensions;
using Microsoft.Win32;

namespace UninstallTools.Junk
{
    public class FirewallRuleScanner :JunkCreatorBase
    {
        public override IEnumerable<JunkNode> FindJunk(ApplicationUninstallerEntry target)
        {
            const string firewallRulesKey =
                @"SYSTEM\CurrentControlSet\Services\SharedAccess\Parameters\FirewallPolicy\FirewallRules";
            const string fullFirewallRulesKey = @"HKEY_LOCAL_MACHINE\" + firewallRulesKey;

            var results = new List<JunkNode>();
            if (string.IsNullOrEmpty(target.InstallLocation))
                return results;

            using (var key = Registry.LocalMachine.OpenSubKey(firewallRulesKey))
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
                            var node = new RegistryValueJunkNode(fullFirewallRulesKey, valueName,
                                target.DisplayName);
                            node.Confidence.Add(ConfidencePart.ExplicitConnection);
                            results.Add(node);
                        }
                    }
                }
            }

            return results;
        }
    }
}
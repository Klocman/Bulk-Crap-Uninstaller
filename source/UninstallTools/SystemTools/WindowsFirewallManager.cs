/*
    EBUninstaller Pro - Windows Firewall Rules & Orphan Cleaner
    Enumeration, orphan detection, and cleanup of stale firewall rules left by uninstalled software.
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using UninstallTools.Core;

namespace UninstallTools.SystemTools
{
    public enum FirewallRuleAction
    {
        Allow,
        Block,
        Unknown
    }

    public enum FirewallRuleDirection
    {
        Inbound,
        Outbound,
        Unknown
    }

    public class FirewallRuleItem
    {
        public string RuleId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ApplicationPath { get; set; } = string.Empty;
        public FirewallRuleDirection Direction { get; set; } = FirewallRuleDirection.Unknown;
        public FirewallRuleAction Action { get; set; } = FirewallRuleAction.Unknown;
        public string Protocol { get; set; } = "Any";
        public string Ports { get; set; } = "Any";
        public bool IsEnabled { get; set; } = true;
        public bool IsOrphaned { get; set; }
        public bool IsSystemRule { get; set; }
    }

    public static class WindowsFirewallManager
    {
        private const string FirewallRulesKey = @"SYSTEM\CurrentControlSet\Services\SharedAccess\Parameters\FirewallPolicy\FirewallRules";

        public static List<FirewallRuleItem> GetFirewallRules(bool orphanedOnly = false)
        {
            var results = new List<FirewallRuleItem>();

            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(FirewallRulesKey, false);
                if (key == null) return results;

                foreach (var valName in key.GetValueNames())
                {
                    try
                    {
                        string rawValue = key.GetValue(valName) as string ?? string.Empty;
                        if (string.IsNullOrWhiteSpace(rawValue)) continue;

                        var rule = ParseRuleString(valName, rawValue);
                        if (rule != null)
                        {
                            if (orphanedOnly && !rule.IsOrphaned)
                                continue;

                            results.Add(rule);
                        }
                    }
                    catch
                    {
                        // Ignore individual parse errors
                    }
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Log(LogLevel.Warning, "WindowsFirewallManager", $"Failed to scan firewall rules: {ex.Message}");
            }

            return results.OrderBy(r => r.Name).ToList();
        }

        public static FirewallRuleItem? ParseRuleString(string ruleId, string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return null;

            // Format: v2.10|Action=Allow|Active=TRUE|Dir=In|Protocol=6|LPort=80|App=C:\App\app.exe|Name=App Rule|Desc=...|
            var parts = raw.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var part in parts)
            {
                int eq = part.IndexOf('=');
                if (eq > 0)
                {
                    string k = part.Substring(0, eq).Trim();
                    string v = part.Substring(eq + 1).Trim();
                    dict[k] = v;
                }
            }

            string name = dict.GetValueOrDefault("Name", ruleId);
            string appPath = dict.GetValueOrDefault("App", string.Empty);
            string dirStr = dict.GetValueOrDefault("Dir", string.Empty);
            string actStr = dict.GetValueOrDefault("Action", string.Empty);
            string activeStr = dict.GetValueOrDefault("Active", "TRUE");
            string protocol = dict.GetValueOrDefault("Protocol", "Any");
            string lport = dict.GetValueOrDefault("LPort", "Any");
            string desc = dict.GetValueOrDefault("Desc", string.Empty);

            if (!string.IsNullOrEmpty(appPath))
            {
                appPath = Environment.ExpandEnvironmentVariables(appPath);
                if (appPath.StartsWith(@"\??\"))
                    appPath = appPath.Substring(4);
            }

            bool hasApp = !string.IsNullOrEmpty(appPath);
            bool fileExists = hasApp && File.Exists(appPath);
            bool isOrphaned = hasApp && !fileExists;

            bool isSystem = false;
            if (hasApp && SecurityGuard.IsPathProtected(appPath))
                isSystem = true;
            if (name.StartsWith("@") || name.StartsWith("Core Networking", StringComparison.OrdinalIgnoreCase) ||
                name.StartsWith("Windows Defender", StringComparison.OrdinalIgnoreCase))
                isSystem = true;

            var dir = dirStr.Equals("In", StringComparison.OrdinalIgnoreCase) ? FirewallRuleDirection.Inbound :
                      (dirStr.Equals("Out", StringComparison.OrdinalIgnoreCase) ? FirewallRuleDirection.Outbound : FirewallRuleDirection.Unknown);

            var act = actStr.Equals("Allow", StringComparison.OrdinalIgnoreCase) ? FirewallRuleAction.Allow :
                      (actStr.Equals("Block", StringComparison.OrdinalIgnoreCase) ? FirewallRuleAction.Block : FirewallRuleAction.Unknown);

            return new FirewallRuleItem
            {
                RuleId = ruleId,
                Name = name,
                Description = desc,
                ApplicationPath = appPath,
                Direction = dir,
                Action = act,
                Protocol = protocol,
                Ports = lport,
                IsEnabled = activeStr.Equals("TRUE", StringComparison.OrdinalIgnoreCase),
                IsOrphaned = isOrphaned,
                IsSystemRule = isSystem
            };
        }

        public static bool DeleteFirewallRule(FirewallRuleItem rule)
        {
            if (rule == null || string.IsNullOrWhiteSpace(rule.RuleId)) return false;
            if (rule.IsSystemRule) return false;

            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(FirewallRulesKey, true);
                if (key == null) return false;

                key.DeleteValue(rule.RuleId, false);
                StructuredLogger.Log(LogLevel.Info, "WindowsFirewallManager", $"Deleted firewall rule '{rule.Name}' ({rule.RuleId})");
                return true;
            }
            catch (Exception ex)
            {
                StructuredLogger.Log(LogLevel.Error, "WindowsFirewallManager", $"Failed to delete firewall rule '{rule.RuleId}': {ex.Message}");
                return false;
            }
        }
    }
}

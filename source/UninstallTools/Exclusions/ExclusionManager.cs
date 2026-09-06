/*
    EBUninstaller Pro - Open Source Professional Windows Uninstaller
    Exclusion and Whitelist Manager Subsystem
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using UninstallTools.Core;

namespace UninstallTools.Exclusions
{
    public static class ExclusionManager
    {
        private static readonly List<ExclusionRule> _rules = new();
        private static readonly object _lock = new();
        private static string _storageFilePath;

        public static string StorageFilePath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_storageFilePath))
                {
                    try
                    {
                        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                        _storageFilePath = Path.Combine(localAppData, "EBUninstallerPro", "Exclusions.json");
                    }
                    catch
                    {
                        _storageFilePath = Path.Combine(Path.GetTempPath(), "EBUninstallerPro", "Exclusions.json");
                    }
                }
                return _storageFilePath;
            }
            set => _storageFilePath = value;
        }

        static ExclusionManager()
        {
            LoadRules();
        }

        public static IReadOnlyList<ExclusionRule> GetRules()
        {
            lock (_lock)
            {
                return _rules.ToList();
            }
        }

        public static void AddRule(ExclusionRule rule)
        {
            if (rule == null || string.IsNullOrWhiteSpace(rule.Value)) return;

            lock (_lock)
            {
                _rules.RemoveAll(r => r.RuleType == rule.RuleType && string.Equals(r.Value, rule.Value, StringComparison.OrdinalIgnoreCase));
                _rules.Add(rule);
                SaveRules();
            }

            StructuredLogger.Info(LogCategory.General, $"Added exclusion rule: {rule}");
        }

        public static void RemoveRule(string ruleId)
        {
            lock (_lock)
            {
                _rules.RemoveAll(r => r.RuleId == ruleId);
                SaveRules();
            }
        }

        /// <summary>
        /// Checks if a file path, registry key, service name, app name, or publisher is covered by an active exclusion rule.
        /// </summary>
        public static bool IsExcluded(
            string applicationName = null,
            string publisher = null,
            string filePath = null,
            string folderPath = null,
            string registryPath = null,
            string serviceName = null)
        {
            lock (_lock)
            {
                foreach (var rule in _rules.Where(r => r.IsEnabled))
                {
                    switch (rule.RuleType)
                    {
                        case ExclusionRuleType.ApplicationName:
                            if (!string.IsNullOrEmpty(applicationName) &&
                                MatchesPattern(applicationName, rule.Value))
                                return true;
                            break;

                        case ExclusionRuleType.Publisher:
                            if (!string.IsNullOrEmpty(publisher) &&
                                MatchesPattern(publisher, rule.Value))
                                return true;
                            break;

                        case ExclusionRuleType.FilePath:
                            if (!string.IsNullOrEmpty(filePath) &&
                                (MatchesPattern(filePath, rule.Value) || MatchesPath(filePath, rule.Value)))
                                return true;
                            break;

                        case ExclusionRuleType.FolderPath:
                            if (!string.IsNullOrEmpty(folderPath) &&
                                (MatchesPattern(folderPath, rule.Value) || MatchesPath(folderPath, rule.Value)))
                                return true;
                            if (!string.IsNullOrEmpty(filePath) &&
                                filePath.StartsWith(rule.Value.TrimEnd('\\', '/'), StringComparison.OrdinalIgnoreCase))
                                return true;
                            break;

                        case ExclusionRuleType.RegistryPath:
                            if (!string.IsNullOrEmpty(registryPath) &&
                                (MatchesPattern(registryPath, rule.Value) || registryPath.StartsWith(rule.Value.TrimEnd('\\'), StringComparison.OrdinalIgnoreCase)))
                                return true;
                            break;

                        case ExclusionRuleType.ServiceName:
                            if (!string.IsNullOrEmpty(serviceName) &&
                                MatchesPattern(serviceName, rule.Value))
                                return true;
                            break;
                    }
                }
            }

            return false;
        }

        private static bool MatchesPattern(string text, string pattern)
        {
            if (string.Equals(text, pattern, StringComparison.OrdinalIgnoreCase)) return true;
            if (pattern.Contains("*") || pattern.Contains("?"))
            {
                var regex = "^" + System.Text.RegularExpressions.Regex.Escape(pattern).Replace("\\*", ".*").Replace("\\?", ".") + "$";
                return System.Text.RegularExpressions.Regex.IsMatch(text, regex, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            }
            return false;
        }

        private static bool MatchesPath(string actualPath, string rulePath)
        {
            var normActual = SecurityGuard.NormalizePath(actualPath);
            var normRule = SecurityGuard.NormalizePath(rulePath);
            return string.Equals(normActual, normRule, StringComparison.OrdinalIgnoreCase);
        }

        public static void SaveRules(string customPath = null)
        {
            var path = customPath ?? StorageFilePath;
            try
            {
                var dir = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                var json = JsonSerializer.Serialize(_rules, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(path, json, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                StructuredLogger.Warning(LogCategory.General, "Failed to save exclusions", ex.Message);
            }
        }

        public static void LoadRules(string customPath = null)
        {
            var path = customPath ?? StorageFilePath;
            if (!File.Exists(path)) return;

            try
            {
                var json = File.ReadAllText(path);
                var loaded = JsonSerializer.Deserialize<List<ExclusionRule>>(json);
                if (loaded != null)
                {
                    lock (_lock)
                    {
                        _rules.Clear();
                        _rules.AddRange(loaded);
                    }
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Warning(LogCategory.General, "Failed to load exclusions", ex.Message);
            }
        }

        public static string ExportRulesToJson()
        {
            lock (_lock)
            {
                return JsonSerializer.Serialize(_rules, new JsonSerializerOptions { WriteIndented = true });
            }
        }

        public static bool ImportRulesFromJson(string json)
        {
            try
            {
                var loaded = JsonSerializer.Deserialize<List<ExclusionRule>>(json);
                if (loaded != null && loaded.Count > 0)
                {
                    lock (_lock)
                    {
                        foreach (var rule in loaded)
                        {
                            _rules.RemoveAll(r => r.RuleType == rule.RuleType && string.Equals(r.Value, rule.Value, StringComparison.OrdinalIgnoreCase));
                            _rules.Add(rule);
                        }
                        SaveRules();
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Error(LogCategory.General, "Failed importing exclusions", ex.Message);
            }
            return false;
        }
    }
}

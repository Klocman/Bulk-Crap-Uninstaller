/*
 * EBUninstaller Pro - Application Uninstaller & System Optimization Suite
 * Copyright (C) 2026 EBUninstaller Development Team & Contributors
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using UninstallTools.Core;
using UninstallTools.RegistryEngine;

namespace UninstallTools.PrivacyCleaner
{
    /// <summary>
    /// Category of diagnostic and telemetry configuration.
    /// </summary>
    public enum TelemetryCategory
    {
        DiagnosticData,
        UserTracking,
        AdvertisingId,
        ErrorReporting,
        CortanaAndSearch,
        LocationServices
    }

    /// <summary>
    /// Represents an individual privacy/telemetry toggle item.
    /// </summary>
    public class TelemetrySettingItem
    {
        public string Name { get; set; } = string.Empty;
        public TelemetryCategory Category { get; set; }
        public string Description { get; set; } = string.Empty;
        public string RegistryRoot { get; set; } = "HKLM"; // HKLM or HKCU
        public string SubKeyPath { get; set; } = string.Empty;
        public string ValueName { get; set; } = string.Empty;
        public object OptimizedValue { get; set; }
        public object DefaultValue { get; set; }
        public bool IsOptimized { get; set; }
        public bool IsSelected { get; set; } = true;
    }

    /// <summary>
    /// Windows Privacy & Telemetry Optimizer Engine.
    /// Manages diagnostic data levels, Customer Experience Improvement Program (CEIP), advertising IDs,
    /// and location tracking with automated transaction backups.
    /// </summary>
    public static class WindowsTelemetryOptimizer
    {
        private static readonly List<TelemetrySettingItem> KnownTelemetrySettings = new List<TelemetrySettingItem>
        {
            new TelemetrySettingItem
            {
                Name = "Windows Diagnostic Telemetry Level",
                Category = TelemetryCategory.DiagnosticData,
                Description = "Restricts diagnostic data collection to security-only or basic tier.",
                RegistryRoot = "HKLM",
                SubKeyPath = @"SOFTWARE\Policies\Microsoft\Windows\DataCollection",
                ValueName = "AllowTelemetry",
                OptimizedValue = 0,
                DefaultValue = 3
            },
            new TelemetrySettingItem
            {
                Name = "Customer Experience Improvement Program (CEIP)",
                Category = TelemetryCategory.DiagnosticData,
                Description = "Disables automated CEIP data harvesting and telemetry submissions.",
                RegistryRoot = "HKLM",
                SubKeyPath = @"SOFTWARE\Policies\Microsoft\SQMClient\Windows",
                ValueName = "CEIPEnable",
                OptimizedValue = 0,
                DefaultValue = 1
            },
            new TelemetrySettingItem
            {
                Name = "Advertising ID Tracking",
                Category = TelemetryCategory.AdvertisingId,
                Description = "Disables personalized advertising ID tracking across Windows apps.",
                RegistryRoot = "HKCU",
                SubKeyPath = @"Software\Microsoft\Windows\CurrentVersion\AdvertisingInfo",
                ValueName = "Enabled",
                OptimizedValue = 0,
                DefaultValue = 1
            },
            new TelemetrySettingItem
            {
                Name = "Windows Error Reporting (WER) Queue Uploads",
                Category = TelemetryCategory.ErrorReporting,
                Description = "Prevents automated uploading of application crash dumps to cloud servers.",
                RegistryRoot = "HKLM",
                SubKeyPath = @"SOFTWARE\Policies\Microsoft\Windows\Windows Error Reporting",
                ValueName = "Disabled",
                OptimizedValue = 1,
                DefaultValue = 0
            },
            new TelemetrySettingItem
            {
                Name = "Feedback Frequency Prompts",
                Category = TelemetryCategory.UserTracking,
                Description = "Disables Windows feedback survey popups.",
                RegistryRoot = "HKCU",
                SubKeyPath = @"Software\Microsoft\Siuf\Rules",
                ValueName = "NumberOfSIUFInPeriod",
                OptimizedValue = 0,
                DefaultValue = 1
            },
            new TelemetrySettingItem
            {
                Name = "Bing Web Search in Windows Start Menu",
                Category = TelemetryCategory.CortanaAndSearch,
                Description = "Disables cloud Bing search queries when searching local programs in Start Menu.",
                RegistryRoot = "HKCU",
                SubKeyPath = @"Software\Policies\Microsoft\Windows\Explorer",
                ValueName = "DisableSearchBoxSuggestions",
                OptimizedValue = 1,
                DefaultValue = 0
            },
            new TelemetrySettingItem
            {
                Name = "App Diagnostics Access",
                Category = TelemetryCategory.UserTracking,
                Description = "Blocks third-party apps from querying diagnostic information from other apps.",
                RegistryRoot = "HKLM",
                SubKeyPath = @"SOFTWARE\Policies\Microsoft\Windows\AppPrivacy",
                ValueName = "LetAppsGetDiagnosticInfo",
                OptimizedValue = 2, // 2 = Deny
                DefaultValue = 0
            }
        };

        /// <summary>
        /// Scans the current state of all privacy and telemetry settings.
        /// </summary>
        public static List<TelemetrySettingItem> ScanTelemetrySettings()
        {
            var results = new List<TelemetrySettingItem>();

            foreach (var template in KnownTelemetrySettings)
            {
                var item = new TelemetrySettingItem
                {
                    Name = template.Name,
                    Category = template.Category,
                    Description = template.Description,
                    RegistryRoot = template.RegistryRoot,
                    SubKeyPath = template.SubKeyPath,
                    ValueName = template.ValueName,
                    OptimizedValue = template.OptimizedValue,
                    DefaultValue = template.DefaultValue
                };

                try
                {
                    var root = item.RegistryRoot == "HKCU" ? Registry.CurrentUser : Registry.LocalMachine;
                    using var key = root.OpenSubKey(item.SubKeyPath);
                    if (key != null)
                    {
                        var val = key.GetValue(item.ValueName);
                        if (val != null)
                        {
                            item.IsOptimized = val.ToString() == item.OptimizedValue.ToString();
                        }
                    }
                }
                catch { }

                results.Add(item);
            }

            return results;
        }

        /// <summary>
        /// Applies privacy and telemetry optimizations with automated registry backup.
        /// </summary>
        public static int ApplyOptimizations(IEnumerable<TelemetrySettingItem> items, string backupDirectory = null)
        {
            var targets = items?.Where(i => i.IsSelected && !i.IsOptimized).ToList() ?? new List<TelemetrySettingItem>();
            if (!targets.Any()) return 0;

            int applied = 0;

            if (!string.IsNullOrEmpty(backupDirectory))
            {
                Directory.CreateDirectory(backupDirectory);
                var backupFile = Path.Combine(backupDirectory, $"Telemetry_Backup_{DateTime.UtcNow:yyyyMMdd_HHmmss}.reg");
                var keyList = targets.Select(t => $@"{t.RegistryRoot}\{t.SubKeyPath}").Distinct().ToList();
                SafeRegistryEngine.ExportRegistryKeys(keyList, backupFile);
            }

            foreach (var item in targets)
            {
                try
                {
                    var root = item.RegistryRoot == "HKCU" ? Registry.CurrentUser : Registry.LocalMachine;
                    using var key = root.CreateSubKey(item.SubKeyPath);
                    if (key != null)
                    {
                        if (item.OptimizedValue is int intVal)
                        {
                            key.SetValue(item.ValueName, intVal, RegistryValueKind.DWord);
                        }
                        else
                        {
                            key.SetValue(item.ValueName, item.OptimizedValue.ToString());
                        }
                        item.IsOptimized = true;
                        applied++;
                        StructuredLogger.Info($"Optimized privacy setting: {item.Name}");
                    }
                }
                catch (Exception ex)
                {
                    StructuredLogger.Error($"Failed to apply privacy setting {item.Name}: {ex.Message}");
                }
            }

            return applied;
        }
    }
}

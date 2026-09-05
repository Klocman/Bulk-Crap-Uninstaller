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
using System.Text.Json;
using UninstallTools.Core;
using UninstallTools.History;

namespace UninstallTools.Exclusions
{
    public class EBUserProfilePackage
    {
        public string Header { get; set; } = "EBUninstaller Pro Profile Package";
        public string ProductVersion { get; set; } = "7.0.0";
        public DateTime ExportDateUtc { get; set; } = DateTime.UtcNow;
        public string MachineName { get; set; } = Environment.MachineName;
        public List<string> ExclusionPaths { get; set; } = new List<string>();
        public List<string> ExclusionRegistryKeys { get; set; } = new List<string>();
        public Dictionary<string, string> UserPreferences { get; set; } = new Dictionary<string, string>();
        public List<HistoryEntry> OperationHistory { get; set; } = new List<HistoryEntry>();
    }

    /// <summary>
    /// Handles exporting, importing, and restoring user preferences, exclusions, and maintenance profiles.
    /// </summary>
    public static class SettingsTransferEngine
    {
        /// <summary>
        /// Exports all application settings, exclusion lists, and history into a JSON profile file.
        /// </summary>
        public static bool ExportProfile(string targetFilePath, bool includeHistory = true)
        {
            if (string.IsNullOrWhiteSpace(targetFilePath)) return false;

            try
            {
                var pkg = new EBUserProfilePackage
                {
                    ExportDateUtc = DateTime.UtcNow,
                    MachineName = Environment.MachineName,
                    ProductVersion = "7.0.0"
                };

                // Populate exclusions
                var exclusions = ExclusionManager.GetExclusions();
                if (exclusions != null)
                {
                    foreach (var exc in exclusions)
                    {
                        if (!string.IsNullOrWhiteSpace(exc))
                            pkg.ExclusionPaths.Add(exc);
                    }
                }

                // Populate history if requested
                if (includeHistory)
                {
                    var history = OperationHistoryManager.GetHistory();
                    if (history != null)
                    {
                        pkg.OperationHistory.AddRange(history);
                    }
                }

                // Set default preferences
                pkg.UserPreferences["AutoCreateRestorePoints"] = "true";
                pkg.UserPreferences["DeepScanLeftovers"] = "true";
                pkg.UserPreferences["SafetyGuardEnforced"] = "true";
                pkg.UserPreferences["TelemetryDisabled"] = "true";

                var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(pkg, jsonOptions);

                var dir = Path.GetDirectoryName(targetFilePath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                File.WriteAllText(targetFilePath, json, System.Text.Encoding.UTF8);
                StructuredLogger.Info(LogCategory.General, $"Profile exported successfully to: {targetFilePath}");
                return true;
            }
            catch (Exception ex)
            {
                StructuredLogger.Error(LogCategory.General, $"Failed to export profile to {targetFilePath}", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Validates and imports settings and exclusion lists from an existing profile file.
        /// </summary>
        public static bool ImportProfile(string sourceFilePath, bool overwriteExisting = false)
        {
            if (string.IsNullOrWhiteSpace(sourceFilePath) || !File.Exists(sourceFilePath)) return false;

            try
            {
                var json = File.ReadAllText(sourceFilePath, System.Text.Encoding.UTF8);
                var pkg = JsonSerializer.Deserialize<EBUserProfilePackage>(json);

                if (pkg == null) return false;

                if (overwriteExisting)
                {
                    ExclusionManager.ClearExclusions();
                }

                if (pkg.ExclusionPaths != null)
                {
                    foreach (var path in pkg.ExclusionPaths)
                    {
                        ExclusionManager.AddExclusion(path);
                    }
                }

                StructuredLogger.Info(LogCategory.General, $"Profile imported successfully from: {sourceFilePath}");
                return true;
            }
            catch (Exception ex)
            {
                StructuredLogger.Error(LogCategory.General, $"Failed to import profile from {sourceFilePath}", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Validates whether a file is a valid EBUninstaller Pro profile package.
        /// </summary>
        public static bool ValidateProfile(string sourceFilePath, out string validationSummary)
        {
            validationSummary = string.Empty;
            if (string.IsNullOrWhiteSpace(sourceFilePath) || !File.Exists(sourceFilePath))
            {
                validationSummary = "File does not exist.";
                return false;
            }

            try
            {
                var json = File.ReadAllText(sourceFilePath, System.Text.Encoding.UTF8);
                var pkg = JsonSerializer.Deserialize<EBUserProfilePackage>(json);

                if (pkg == null || string.IsNullOrWhiteSpace(pkg.Header))
                {
                    validationSummary = "Invalid profile header format.";
                    return false;
                }

                validationSummary = $"Profile valid (Version: {pkg.ProductVersion}, Exported: {pkg.ExportDateUtc:yyyy-MM-dd}, Exclusions: {pkg.ExclusionPaths?.Count ?? 0})";
                return true;
            }
            catch (Exception ex)
            {
                validationSummary = $"JSON parse error: {ex.Message}";
                return false;
            }
        }
    }
}

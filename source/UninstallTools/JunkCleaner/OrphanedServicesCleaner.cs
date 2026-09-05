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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using UninstallTools.Core;
using UninstallTools.RegistryEngine;

namespace UninstallTools.JunkCleaner
{
    /// <summary>
    /// Model representing an orphaned or broken Windows service.
    /// </summary>
    public class OrphanedServiceItem
    {
        public string ServiceName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public string ParsedExecutablePath { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int StartType { get; set; } // 2=Auto, 3=Manual, 4=Disabled
        public string StartTypeName => StartType switch
        {
            2 => "Automatic",
            3 => "Manual",
            4 => "Disabled",
            _ => "Unknown"
        };
        public string RegistryPath { get; set; } = string.Empty;
        public bool IsOrphaned { get; set; }
        public bool IsProtected { get; set; }
    }

    /// <summary>
    /// Result of an orphaned service cleanup operation.
    /// </summary>
    public class ServiceCleanupResult
    {
        public bool Success { get; set; }
        public int CleanedCount { get; set; }
        public string BackupRegPath { get; set; } = string.Empty;
        public List<string> RemovedServices { get; } = new List<string>();
        public List<string> Errors { get; } = new List<string>();
    }

    /// <summary>
    /// Scans for Windows Services whose underlying binary image no longer exists on disk.
    /// </summary>
    public static class OrphanedServicesCleaner
    {
        private static readonly HashSet<string> ProtectedServices = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "RpcSs", "DcomLaunch", "EventLog", "WinDefend", "LanmanServer", "LanmanWorkstation",
            "PlugPlay", "Power", "ProfSvc", "Schedule", "SENS", "ShellHWDetection", "Spooler",
            "TermService", "Themes", "wuauserv", "CryptSvc", "Dhcp", "Dnscache", "EventSystem",
            "MpsSvc", "nsi", "wscsvc", "SecurityHealthService", "SamSs", "LSM", "CoreMessagingRegistrar"
        };

        /// <summary>
        /// Scans all registered Windows services in HKLM\SYSTEM\CurrentControlSet\Services.
        /// </summary>
        public static List<OrphanedServiceItem> ScanOrphanedServices()
        {
            var list = new List<OrphanedServiceItem>();

            try
            {
                using var servicesKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services");
                if (servicesKey == null) return list;

                foreach (var serviceName in servicesKey.GetSubKeyNames())
                {
                    try
                    {
                        using var sub = servicesKey.OpenSubKey(serviceName);
                        if (sub == null) continue;

                        var imagePath = sub.GetValue("ImagePath")?.ToString() ?? string.Empty;
                        if (string.IsNullOrWhiteSpace(imagePath)) continue;

                        var displayName = sub.GetValue("DisplayName")?.ToString() ?? serviceName;
                        var description = sub.GetValue("Description")?.ToString() ?? string.Empty;
                        var startType = sub.GetValue("Start") is int st ? st : 3;

                        var exePath = ExtractExecutablePath(imagePath);
                        var isProtected = ProtectedServices.Contains(serviceName) || SecurityGuard.IsCriticalService(serviceName);

                        bool isOrphaned = false;
                        if (!string.IsNullOrEmpty(exePath) && !exePath.StartsWith(@"\SystemRoot\", StringComparison.OrdinalIgnoreCase) && !exePath.StartsWith(@"system32\", StringComparison.OrdinalIgnoreCase))
                        {
                            if (!File.Exists(exePath))
                            {
                                isOrphaned = true;
                            }
                        }

                        if (isOrphaned)
                        {
                            list.Add(new OrphanedServiceItem
                            {
                                ServiceName = serviceName,
                                DisplayName = displayName,
                                ImagePath = imagePath,
                                ParsedExecutablePath = exePath,
                                Description = description,
                                StartType = startType,
                                RegistryPath = $@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\{serviceName}",
                                IsOrphaned = true,
                                IsProtected = isProtected
                            });
                        }
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Error($"Error scanning orphaned services: {ex.Message}");
            }

            return list;
        }

        /// <summary>
        /// Safely removes orphaned services after creating a registry backup.
        /// </summary>
        public static ServiceCleanupResult RemoveOrphanedServices(IEnumerable<OrphanedServiceItem> servicesToRemove, string backupDirectory = null)
        {
            var result = new ServiceCleanupResult { Success = true };
            var targets = servicesToRemove?.Where(s => s != null && !s.IsProtected).ToList() ?? new List<OrphanedServiceItem>();

            if (!targets.Any()) return result;

            try
            {
                // Create backup directory if specified
                if (!string.IsNullOrEmpty(backupDirectory))
                {
                    Directory.CreateDirectory(backupDirectory);
                    var backupFile = Path.Combine(backupDirectory, $"Services_Backup_{DateTime.UtcNow:yyyyMMdd_HHmmss}.reg");
                    var keysToBackup = targets.Select(t => t.RegistryPath).ToList();
                    SafeRegistryEngine.ExportRegistryKeys(keysToBackup, backupFile);
                    result.BackupRegPath = backupFile;
                }

                using var servicesKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services", true);
                if (servicesKey != null)
                {
                    foreach (var target in targets)
                    {
                        try
                        {
                            // Delete subkey tree safely
                            servicesKey.DeleteSubKeyTree(target.ServiceName, false);
                            result.RemovedServices.Add(target.ServiceName);
                            result.CleanedCount++;
                            StructuredLogger.Info($"Removed orphaned service: {target.ServiceName}");
                        }
                        catch (Exception ex)
                        {
                            result.Errors.Add($"Failed to remove service {target.ServiceName}: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Errors.Add($"Service cleanup error: {ex.Message}");
                StructuredLogger.Error($"Orphaned service cleanup failed: {ex.Message}");
            }

            return result;
        }

        /// <summary>
        /// Parses the raw ImagePath string to extract the clean executable file path.
        /// </summary>
        public static string ExtractExecutablePath(string imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath)) return string.Empty;

            var expanded = Environment.ExpandEnvironmentVariables(imagePath).Trim();

            // Handle quoted paths: "C:\Program Files\App\service.exe" -arg
            if (expanded.StartsWith("\""))
            {
                var nextQuote = expanded.IndexOf('\"', 1);
                if (nextQuote > 1)
                {
                    return expanded.Substring(1, nextQuote - 1);
                }
            }

            // Handle svchost.exe -k group (skip Windows svchost services)
            if (expanded.IndexOf("svchost.exe", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return Path.Combine(Environment.SystemDirectory, "svchost.exe");
            }

            // Unquoted path with arguments: C:\Program Files\App\service.exe /run
            var match = Regex.Match(expanded, @"^[a-zA-Z]:\\[^/\:\*\?""<>\|]+\.exe", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return match.Value;
            }

            // Split on space fallback
            var parts = expanded.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return parts.Length > 0 ? parts[0] : expanded;
        }
    }
}

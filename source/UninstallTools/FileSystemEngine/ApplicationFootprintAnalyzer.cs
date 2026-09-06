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

namespace UninstallTools.FileSystemEngine
{
    /// <summary>
    /// Represents an individual location where an application stores data or binaries.
    /// </summary>
    public class FootprintLocation
    {
        public string LocationType { get; set; } = string.Empty; // "Binaries", "UserData", "CommonData", "Registry"
        public string PathOrKey { get; set; } = string.Empty;
        public long SizeBytes { get; set; }
        public int ItemCount { get; set; }
        public bool Exists { get; set; }
    }

    /// <summary>
    /// Comprehensive footprint breakdown of an installed application.
    /// </summary>
    public class ApplicationFootprintReport
    {
        public string ApplicationName { get; set; } = string.Empty;
        public string InstallLocation { get; set; } = string.Empty;
        public long TotalDiskSizeBytes => Locations.Where(l => l.LocationType != "Registry").Sum(l => l.SizeBytes);
        public int TotalFileCount => Locations.Where(l => l.LocationType != "Registry").Sum(l => l.ItemCount);
        public int TotalRegistryKeysCount => Locations.Where(l => l.LocationType == "Registry").Sum(l => l.ItemCount);
        public List<FootprintLocation> Locations { get; } = new List<FootprintLocation>();
        public List<string> TopLargestFiles { get; } = new List<string>();
        public TimeSpan AnalysisDuration { get; set; }
    }

    /// <summary>
    /// Analyzes the comprehensive storage and registry footprint of an installed software product across all system tiers.
    /// </summary>
    public static class ApplicationFootprintAnalyzer
    {
        /// <summary>
        /// Analyzes the full file system and registry footprint for a target application.
        /// </summary>
        public static ApplicationFootprintReport AnalyzeFootprint(string appName, string installLocation, string publisher = null)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            var report = new ApplicationFootprintReport
            {
                ApplicationName = appName,
                InstallLocation = installLocation
            };

            try
            {
                // 1. Primary installation directory
                if (!string.IsNullOrWhiteSpace(installLocation) && Directory.Exists(installLocation))
                {
                    InspectDirectory(installLocation, "Main Installation Folder", report);
                }

                // 2. User AppData Roaming / Local / LocalLow
                var safeName = SanitizeForFolderName(appName);
                var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                var appDataRoaming = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), safeName);
                var appDataLocal = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), safeName);
                var appDataLocalLow = Path.Combine(userProfile, "AppData", "LocalLow", safeName);
                var commonData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), safeName);

                InspectDirectory(appDataRoaming, "AppData (Roaming)", report);
                InspectDirectory(appDataLocal, "AppData (Local)", report);
                InspectDirectory(appDataLocalLow, "AppData (LocalLow)", report);
                InspectDirectory(commonData, "ProgramData", report);

                // Check with publisher prefix if available
                if (!string.IsNullOrWhiteSpace(publisher))
                {
                    var safePub = SanitizeForFolderName(publisher);
                    var pubRoaming = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), safePub, safeName);
                    var pubLocal = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), safePub, safeName);
                    var pubCommon = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), safePub, safeName);

                    InspectDirectory(pubRoaming, "Publisher AppData (Roaming)", report);
                    InspectDirectory(pubLocal, "Publisher AppData (Local)", report);
                    InspectDirectory(pubCommon, "Publisher ProgramData", report);
                }

                // 3. Inspect Registry Keys in HKCU and HKLM
                InspectRegistryKey(Registry.CurrentUser, $@"Software\{safeName}", "Registry (HKCU)", report);
                InspectRegistryKey(Registry.LocalMachine, $@"SOFTWARE\{safeName}", "Registry (HKLM)", report);
                if (Environment.Is64BitOperatingSystem)
                {
                    InspectRegistryKey(Registry.LocalMachine, $@"SOFTWARE\WOW6432Node\{safeName}", "Registry (HKLM 32-bit)", report);
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Error($"Footprint analysis error: {ex.Message}");
            }
            finally
            {
                sw.Stop();
                report.AnalysisDuration = sw.Elapsed;
            }

            return report;
        }

        private static void InspectDirectory(string dirPath, string label, ApplicationFootprintReport report)
        {
            if (string.IsNullOrWhiteSpace(dirPath) || !Directory.Exists(dirPath)) return;

            try
            {
                var di = new DirectoryInfo(dirPath);
                var files = di.GetFiles("*", SearchOption.AllDirectories);
                var size = files.Sum(f => f.Length);

                report.Locations.Add(new FootprintLocation
                {
                    LocationType = label,
                    PathOrKey = dirPath,
                    SizeBytes = size,
                    ItemCount = files.Length,
                    Exists = true
                });

                // Record largest files
                foreach (var f in files.OrderByDescending(f => f.Length).Take(5))
                {
                    report.TopLargestFiles.Add($"{f.FullName} ({f.Length / (1024.0 * 1024.0):F2} MB)");
                }
            }
            catch { }
        }

        private static void InspectRegistryKey(RegistryKey rootKey, string subKeyPath, string label, ApplicationFootprintReport report)
        {
            try
            {
                using var key = rootKey.OpenSubKey(subKeyPath);
                if (key != null)
                {
                    var count = 1 + key.SubKeyCount + key.ValueCount;
                    report.Locations.Add(new FootprintLocation
                    {
                        LocationType = "Registry",
                        PathOrKey = $@"{rootKey.Name}\{subKeyPath}",
                        ItemCount = count,
                        Exists = true
                    });
                }
            }
            catch { }
        }

        private static string SanitizeForFolderName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return string.Empty;
            var invalid = Path.GetInvalidFileNameChars();
            return new string(name.Where(c => !invalid.Contains(c)).ToArray()).Trim();
        }
    }
}

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
using Microsoft.Win32;
using UninstallTools.Core;

namespace UninstallTools.RegistryEngine
{
    /// <summary>
    /// Type of registry bloat detected.
    /// </summary>
    public enum RegistryBloatCategory
    {
        OrphanedClsid,
        StaleAppPath,
        InvalidSharedDll,
        ObsoleteMuiCache,
        StaleHelpEntry,
        EmptySubkey
    }

    /// <summary>
    /// Represents an individual registry bloat item.
    /// </summary>
    public class RegistryBloatItem
    {
        public RegistryBloatCategory Category { get; set; }
        public string RootKeyName { get; set; } = "HKEY_LOCAL_MACHINE"; // HKLM, HKCU, HKCR
        public string SubKeyPath { get; set; } = string.Empty;
        public string ValueName { get; set; } = string.Empty;
        public string TargetPath { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public string FullRegistryPath => $@"{RootKeyName}\{SubKeyPath}";
        public bool IsSelected { get; set; } = true;
    }

    /// <summary>
    /// Result of registry bloat analysis.
    /// </summary>
    public class RegistryBloatScanResult
    {
        public List<RegistryBloatItem> Items { get; } = new List<RegistryBloatItem>();
        public TimeSpan Duration { get; set; }
        public int TotalCount => Items.Count;
        public int OrphanedClsidsCount => Items.Count(i => i.Category == RegistryBloatCategory.OrphanedClsid);
        public int StaleAppPathsCount => Items.Count(i => i.Category == RegistryBloatCategory.StaleAppPath);
        public int InvalidSharedDllsCount => Items.Count(i => i.Category == RegistryBloatCategory.InvalidSharedDll);
        public int ObsoleteMuiCacheCount => Items.Count(i => i.Category == RegistryBloatCategory.ObsoleteMuiCache);
    }

    /// <summary>
    /// Analyzes the Windows Registry for orphaned CLSIDs, invalid SharedDLLs, stale App Paths,
    /// and obsolete MUICache entries left behind by uninstalled programs.
    /// </summary>
    public static class RegistryBloatAnalyzer
    {
        /// <summary>
        /// Performs a comprehensive scan for all categories of registry bloat.
        /// </summary>
        public static RegistryBloatScanResult ScanAllBloat()
        {
            var sw = Stopwatch.StartNew();
            var result = new RegistryBloatScanResult();

            try
            {
                ScanStaleAppPaths(result.Items);
                ScanInvalidSharedDlls(result.Items);
                ScanObsoleteMuiCache(result.Items);
                ScanOrphanedClsids(result.Items);
            }
            catch (Exception ex)
            {
                StructuredLogger.Error($"Registry bloat scan error: {ex.Message}");
            }
            finally
            {
                sw.Stop();
                result.Duration = sw.Elapsed;
            }

            return result;
        }

        private static void ScanStaleAppPaths(List<RegistryBloatItem> list)
        {
            var appPathsKeys = new[]
            {
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths",
                @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\App Paths"
            };

            foreach (var basePath in appPathsKeys)
            {
                try
                {
                    using var key = Registry.LocalMachine.OpenSubKey(basePath);
                    if (key == null) continue;

                    foreach (var exeKeyName in key.GetSubKeyNames())
                    {
                        using var sub = key.OpenSubKey(exeKeyName);
                        if (sub == null) continue;

                        var defaultVal = sub.GetValue(null)?.ToString() ?? string.Empty;
                        if (!string.IsNullOrWhiteSpace(defaultVal))
                        {
                            var cleanPath = defaultVal.Trim('\"');
                            if (Path.IsPathRooted(cleanPath) && !File.Exists(cleanPath) && !SecurityGuard.IsCriticalPath(cleanPath))
                            {
                                list.Add(new RegistryBloatItem
                                {
                                    Category = RegistryBloatCategory.StaleAppPath,
                                    RootKeyName = "HKEY_LOCAL_MACHINE",
                                    SubKeyPath = $@"{basePath}\{exeKeyName}",
                                    TargetPath = cleanPath,
                                    Reason = "Target executable file no longer exists."
                                });
                            }
                        }
                    }
                }
                catch { }
            }
        }

        private static void ScanInvalidSharedDlls(List<RegistryBloatItem> list)
        {
            var sharedDllsPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\SharedDLLs";
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(sharedDllsPath);
                if (key == null) return;

                foreach (var valName in key.GetValueNames())
                {
                    if (string.IsNullOrWhiteSpace(valName)) continue;

                    var cleanPath = valName.Trim('\"');
                    if (Path.IsPathRooted(cleanPath) && !File.Exists(cleanPath) && !SecurityGuard.IsCriticalPath(cleanPath))
                    {
                        list.Add(new RegistryBloatItem
                        {
                            Category = RegistryBloatCategory.InvalidSharedDll,
                            RootKeyName = "HKEY_LOCAL_MACHINE",
                            SubKeyPath = sharedDllsPath,
                            ValueName = valName,
                            TargetPath = cleanPath,
                            Reason = "Shared DLL referenced in registry does not exist on disk."
                        });
                    }
                }
            }
            catch { }
        }

        private static void ScanObsoleteMuiCache(List<RegistryBloatItem> list)
        {
            var muiCachePath = @"Software\Classes\Local Settings\Software\Microsoft\Windows\Shell\MuiCache";
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(muiCachePath);
                if (key == null) return;

                foreach (var valName in key.GetValueNames())
                {
                    if (string.IsNullOrWhiteSpace(valName)) continue;

                    var clean = valName.Split(new[] { ".FriendlyAppName", ".ApplicationCompany" }, StringSplitOptions.None)[0].Trim('\"');
                    if (Path.IsPathRooted(clean) && clean.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!File.Exists(clean) && !SecurityGuard.IsCriticalPath(clean))
                        {
                            list.Add(new RegistryBloatItem
                            {
                                Category = RegistryBloatCategory.ObsoleteMuiCache,
                                RootKeyName = "HKEY_CURRENT_USER",
                                SubKeyPath = muiCachePath,
                                ValueName = valName,
                                TargetPath = clean,
                                Reason = "Cached application name for non-existent executable."
                            });
                        }
                    }
                }
            }
            catch { }
        }

        private static void ScanOrphanedClsids(List<RegistryBloatItem> list)
        {
            try
            {
                using var clsidKey = Registry.ClassesRoot.OpenSubKey("CLSID");
                if (clsidKey == null) return;

                int count = 0;
                foreach (var subKeyName in clsidKey.GetSubKeyNames())
                {
                    if (++count > 2000) break; // limit scan depth for responsiveness

                    try
                    {
                        using var serverKey = clsidKey.OpenSubKey($@"{subKeyName}\InprocServer32") ?? clsidKey.OpenSubKey($@"{subKeyName}\LocalServer32");
                        if (serverKey == null) continue;

                        var serverPath = serverKey.GetValue(null)?.ToString() ?? string.Empty;
                        if (string.IsNullOrWhiteSpace(serverPath)) continue;

                        var clean = serverPath.Trim('\"').Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0];
                        if (Path.IsPathRooted(clean) && !File.Exists(clean) && !clean.StartsWith(@"\SystemRoot", StringComparison.OrdinalIgnoreCase) && !SecurityGuard.IsCriticalPath(clean))
                        {
                            list.Add(new RegistryBloatItem
                            {
                                Category = RegistryBloatCategory.OrphanedClsid,
                                RootKeyName = "HKEY_CLASSES_ROOT",
                                SubKeyPath = $@"CLSID\{subKeyName}",
                                TargetPath = clean,
                                Reason = "COM server module file does not exist."
                            });
                        }
                    }
                    catch { }
                }
            }
            catch { }
        }

        /// <summary>
        /// Cleans the selected registry bloat items after creating a transaction backup.
        /// </summary>
        public static int CleanBloatItems(IEnumerable<RegistryBloatItem> items, string backupDirectory = null)
        {
            var targets = items?.Where(i => i.IsSelected).ToList() ?? new List<RegistryBloatItem>();
            if (!targets.Any()) return 0;

            int cleanedCount = 0;

            if (!string.IsNullOrEmpty(backupDirectory))
            {
                Directory.CreateDirectory(backupDirectory);
                var backupFile = Path.Combine(backupDirectory, $"RegistryBloat_Backup_{DateTime.UtcNow:yyyyMMdd_HHmmss}.reg");
                var keyList = targets.Select(t => t.FullRegistryPath).Distinct().ToList();
                SafeRegistryEngine.ExportRegistryKeys(keyList, backupFile);
            }

            foreach (var item in targets)
            {
                try
                {
                    if (item.Category == RegistryBloatCategory.InvalidSharedDll || item.Category == RegistryBloatCategory.ObsoleteMuiCache)
                    {
                        // Delete specific value
                        var root = item.RootKeyName == "HKEY_CURRENT_USER" ? Registry.CurrentUser : Registry.LocalMachine;
                        using var sub = root.OpenSubKey(item.SubKeyPath, true);
                        if (sub != null && !string.IsNullOrEmpty(item.ValueName))
                        {
                            sub.DeleteValue(item.ValueName, false);
                            cleanedCount++;
                        }
                    }
                    else
                    {
                        // Delete subkey
                        var parts = item.SubKeyPath.Split(new[] { '\\' }, 2);
                        if (parts.Length == 2)
                        {
                            var root = item.RootKeyName == "HKEY_CLASSES_ROOT" ? Registry.ClassesRoot :
                                       item.RootKeyName == "HKEY_CURRENT_USER" ? Registry.CurrentUser : Registry.LocalMachine;
                            using var parent = root.OpenSubKey(parts[0], true);
                            if (parent != null)
                            {
                                parent.DeleteSubKeyTree(parts[1], false);
                                cleanedCount++;
                            }
                        }
                    }
                }
                catch { }
            }

            StructuredLogger.Info($"Cleaned {cleanedCount} registry bloat entries.");
            return cleanedCount;
        }
    }
}

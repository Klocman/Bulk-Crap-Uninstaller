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

namespace UninstallTools.JunkCleaner
{
    public class PatchCacheItem
    {
        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public long FileSizeBytes { get; set; }
        public bool IsOrphaned { get; set; }
        public string PackageType { get; set; } = "MSP Patch";
        public bool IsSelected { get; set; } = true;
    }

    /// <summary>
    /// Scans and purges orphaned Windows Installer (.msi/.msp) patch cache files
    /// that no longer correspond to any registered software installation.
    /// </summary>
    public static class PatchCacheResidualsCleaner
    {
        private const string InstallerUserDataKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Installer\UserData";

        /// <summary>
        /// Scans Windows Installer cache directories for orphaned patch packages.
        /// </summary>
        public static List<PatchCacheItem> ScanPatchCache()
        {
            var list = new List<PatchCacheItem>();

            try
            {
                var registeredPatches = GetRegisteredPatchFiles();
                var winDir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
                var installerDir = Path.Combine(winDir, "Installer");

                if (Directory.Exists(installerDir))
                {
                    var files = Directory.GetFiles(installerDir, "*.msp", SearchOption.TopDirectoryOnly)
                                  .Concat(Directory.GetFiles(installerDir, "*.msi", SearchOption.TopDirectoryOnly));

                    foreach (var file in files)
                    {
                        var fileName = Path.GetFileName(file);
                        var isRegistered = registeredPatches.Contains(fileName);
                        long size = 0;
                        try { size = new FileInfo(file).Length; } catch { }

                        list.Add(new PatchCacheItem
                        {
                            FilePath = file,
                            FileName = fileName,
                            FileSizeBytes = size,
                            IsOrphaned = !isRegistered,
                            PackageType = file.EndsWith(".msp", StringComparison.OrdinalIgnoreCase) ? "MSP Patch" : "MSI Package"
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Error(LogCategory.JunkCleaner, "Failed to scan installer patch cache: " + ex.Message);
            }

            return list.OrderByDescending(p => p.FileSizeBytes).ToList();
        }

        private static HashSet<string> GetRegisteredPatchFiles()
        {
            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            try
            {
                using var root = Registry.LocalMachine.OpenSubKey(InstallerUserDataKey, false);
                if (root == null) return set;

                foreach (var userSub in root.GetSubKeyNames())
                {
                    try
                    {
                        using var patchesKey = root.OpenSubKey(userSub + @"\Patches", false);
                        if (patchesKey != null)
                        {
                            foreach (var patchGuid in patchesKey.GetSubKeyNames())
                            {
                                try
                                {
                                    using var pKey = patchesKey.OpenSubKey(patchGuid);
                                    var localPackage = pKey?.GetValue("LocalPackage")?.ToString();
                                    if (!string.IsNullOrEmpty(localPackage))
                                    {
                                        set.Add(Path.GetFileName(localPackage));
                                    }
                                }
                                catch { }
                            }
                        }
                    }
                    catch { }
                }
            }
            catch { }

            return set;
        }

        /// <summary>
        /// Cleans orphaned patch files from the installer cache.
        /// </summary>
        public static int CleanOrphanedPatches(IEnumerable<PatchCacheItem> items)
        {
            var targets = items?.Where(i => i.IsSelected && i.IsOrphaned).ToList() ?? new List<PatchCacheItem>();
            if (targets.Count == 0) return 0;

            int cleaned = 0;

            foreach (var t in targets)
            {
                try
                {
                    if (File.Exists(t.FilePath) && !SecurityGuard.IsCriticalPath(t.FilePath))
                    {
                        File.Delete(t.FilePath);
                        cleaned++;
                        StructuredLogger.Info(LogCategory.JunkCleaner, "Deleted orphaned patch cache: " + t.FilePath);
                    }
                }
                catch { }
            }

            return cleaned;
        }
    }
}

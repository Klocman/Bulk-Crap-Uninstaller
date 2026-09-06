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

namespace UninstallTools.RegistryEngine
{
    public class SharedDllRecord
    {
        public string FilePath { get; set; } = string.Empty;
        public int ReferenceCount { get; set; }
        public bool FileExistsOnDisk { get; set; }
        public bool IsOrphanedReference => !FileExistsOnDisk;
        public string RegistryRoot { get; set; } = "HKLM64";
        public bool IsSelected { get; set; } = true;
    }

    /// <summary>
    /// Audits Windows SharedDLLs reference counters to prevent runtime library corruption
    /// and safely clean orphaned references pointing to deleted binaries.
    /// </summary>
    public static class SharedDllAuditorEngine
    {
        private const string SharedDllKey64 = @"SOFTWARE\Microsoft\Windows\CurrentVersion\SharedDLLs";
        private const string SharedDllKey32 = @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\SharedDLLs";

        /// <summary>
        /// Scans all registered shared DLLs across 64-bit and 32-bit registry hives.
        /// </summary>
        public static List<SharedDllRecord> ScanSharedDlls()
        {
            var list = new List<SharedDllRecord>();

            ScanKey(SharedDllKey64, "HKLM_64", list);
            ScanKey(SharedDllKey32, "HKLM_32", list);

            return list.OrderBy(d => d.FilePath).ToList();
        }

        private static void ScanKey(string subKeyPath, string rootLabel, List<SharedDllRecord> list)
        {
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(subKeyPath, false);
                if (key == null) return;

                foreach (var valName in key.GetValueNames())
                {
                    if (string.IsNullOrWhiteSpace(valName)) continue;

                    var refCount = Convert.ToInt32(key.GetValue(valName, 1));
                    bool exists = File.Exists(valName);

                    list.Add(new SharedDllRecord
                    {
                        FilePath = valName,
                        ReferenceCount = refCount,
                        FileExistsOnDisk = exists,
                        RegistryRoot = rootLabel
                    });
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Error(LogCategory.Registry, $"Failed to scan SharedDLLs key {subKeyPath}: {ex.Message}");
            }
        }

        /// <summary>
        /// Cleans orphaned SharedDLL registrations where the referenced file no longer exists on disk.
        /// </summary>
        public static int CleanOrphanedSharedDlls(IEnumerable<SharedDllRecord> records)
        {
            var targets = records?.Where(r => r.IsSelected && r.IsOrphanedReference).ToList() ?? new List<SharedDllRecord>();
            if (!targets.Any()) return 0;

            int cleaned = 0;

            foreach (var t in targets)
            {
                try
                {
                    var subPath = t.RegistryRoot == "HKLM_32" ? SharedDllKey32 : SharedDllKey64;
                    using var key = Registry.LocalMachine.OpenSubKey(subPath, true);
                    if (key != null)
                    {
                        key.DeleteValue(t.FilePath, false);
                        cleaned++;
                        StructuredLogger.Info(LogCategory.Registry, $"Removed orphaned SharedDLL entry: {t.FilePath}");
                    }
                }
                catch { }
            }

            return cleaned;
        }
    }
}

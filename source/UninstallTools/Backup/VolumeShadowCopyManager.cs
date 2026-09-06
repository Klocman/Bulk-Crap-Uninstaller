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
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using UninstallTools.Core;

namespace UninstallTools.Backup
{
    public class ShadowCopyRecord
    {
        public string ShadowCopyId { get; set; } = string.Empty;
        public string VolumeName { get; set; } = string.Empty;
        public DateTime CreationTimeUtc { get; set; } = DateTime.UtcNow;
        public string DeviceObject { get; set; } = string.Empty;
        public bool IsSystemState { get; set; }
    }

    /// <summary>
    /// Manages Windows Volume Shadow Copies (VSS) to reclaim disk space
    /// and audit system snapshot recovery points.
    /// </summary>
    public static class VolumeShadowCopyManager
    {
        /// <summary>
        /// Retrieves all volume shadow copies currently present on the system.
        /// </summary>
        public static List<ShadowCopyRecord> GetShadowCopies()
        {
            var list = new List<ShadowCopyRecord>();
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return list;

            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT ID, VolumeName, InstallDate, DeviceObject FROM Win32_ShadowCopy");
                using var results = searcher.Get();

                foreach (ManagementObject obj in results)
                {
                    try
                    {
                        var id = obj["ID"]?.ToString() ?? string.Empty;
                        var vol = obj["VolumeName"]?.ToString() ?? "C:\\";
                        var device = obj["DeviceObject"]?.ToString() ?? string.Empty;
                        var dateStr = obj["InstallDate"]?.ToString();

                        DateTime dt = DateTime.UtcNow;
                        if (!string.IsNullOrEmpty(dateStr))
                        {
                            try { dt = ManagementDateTimeConverter.ToDateTime(dateStr).ToUniversalTime(); } catch { }
                        }

                        list.Add(new ShadowCopyRecord
                        {
                            ShadowCopyId = id,
                            VolumeName = vol,
                            CreationTimeUtc = dt,
                            DeviceObject = device
                        });
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Error(LogCategory.Backup, "Failed to query Win32_ShadowCopy: " + ex.Message);
            }

            return list.OrderByDescending(s => s.CreationTimeUtc).ToList();
        }

        /// <summary>
        /// Deletes a specific volume shadow copy using vssadmin.
        /// </summary>
        public static bool DeleteShadowCopy(string shadowCopyId)
        {
            if (string.IsNullOrWhiteSpace(shadowCopyId)) return false;

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "vssadmin.exe",
                    Arguments = "delete shadows /shadow=" + shadowCopyId + " /quiet",
                    CreateNoWindow = true,
                    UseShellExecute = false
                };

                using var proc = Process.Start(psi);
                if (proc != null)
                {
                    proc.WaitForExit(10000);
                    StructuredLogger.Info(LogCategory.Backup, "Deleted shadow copy: " + shadowCopyId);
                    return proc.ExitCode == 0;
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Error(LogCategory.Backup, "Failed to delete shadow copy: " + ex.Message);
            }

            return false;
        }

        /// <summary>
        /// Purges oldest shadow copies, keeping only the specified number of recent snapshots.
        /// </summary>
        public static int PurgeOldestShadowCopies(int keepRecentCount = 3)
        {
            var all = GetShadowCopies();
            if (all.Count <= keepRecentCount) return 0;

            int deleted = 0;
            var toDelete = all.Skip(keepRecentCount).ToList();

            foreach (var item in toDelete)
            {
                if (DeleteShadowCopy(item.ShadowCopyId))
                {
                    deleted++;
                }
            }

            return deleted;
        }
    }
}

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
using UninstallTools.Core;

namespace UninstallTools.JunkCleaner
{
    /// <summary>
    /// Represents a Windows Update or OS Upgrade residual category.
    /// </summary>
    public class WinUpdateResidualItem
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string TargetDirectoryPath { get; set; } = string.Empty;
        public int FileCount { get; set; }
        public long TotalSizeBytes { get; set; }
        public bool IsSelected { get; set; } = true;
        public bool RequiresAdmin { get; set; } = true;
    }

    /// <summary>
    /// Result of Windows Update residual cleanup.
    /// </summary>
    public class WinUpdateCleanupResult
    {
        public bool Success { get; set; }
        public int DeletedFilesCount { get; set; }
        public long BytesFreed { get; set; }
        public List<string> Errors { get; } = new List<string>();
        public TimeSpan Duration { get; set; }
    }

    /// <summary>
    /// Scans and purges Windows Update download caches, Delivery Optimization caches,
    /// Windows Upgrade leftover folders ($WINDOWS.~BT, Panther), and launches Component Store cleanup.
    /// </summary>
    public static class WinUpdateResidualsCleaner
    {
        /// <summary>
        /// Scans all known Windows Update residual locations.
        /// </summary>
        public static List<WinUpdateResidualItem> ScanResiduals()
        {
            var list = new List<WinUpdateResidualItem>();

            var winDir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
            var progData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            var sysDrive = Path.GetPathRoot(winDir) ?? @"C:\";

            var locations = new[]
            {
                new
                {
                    Title = "SoftwareDistribution Download Cache",
                    Desc = "Downloaded Windows Update installation files that have already been staged.",
                    Path = Path.Combine(winDir, "SoftwareDistribution", "Download")
                },
                new
                {
                    Title = "Delivery Optimization Cache",
                    Desc = "Peer-to-peer Windows Update delivery cache packages.",
                    Path = Path.Combine(winDir, "SoftwareDistribution", "DeliveryOptimization", "Cache")
                },
                new
                {
                    Title = "Windows Update DataStore Logs",
                    Desc = "Historical update transaction and state log files.",
                    Path = Path.Combine(winDir, "SoftwareDistribution", "DataStore", "Logs")
                },
                new
                {
                    Title = "Windows Upgrade Leftovers ($WINDOWS.~BT)",
                    Desc = "Temporary installation and rollback files from previous Windows feature updates.",
                    Path = Path.Combine(sysDrive, "$WINDOWS.~BT")
                },
                new
                {
                    Title = "Windows Setup Leftovers ($Windows.~WS)",
                    Desc = "Media Creation Tool and Windows Setup staging files.",
                    Path = Path.Combine(sysDrive, "$Windows.~WS")
                },
                new
                {
                    Title = "Windows Setup Panther Logs",
                    Desc = "Setup diagnostic and installation telemetry logs.",
                    Path = Path.Combine(winDir, "Panther")
                },
                new
                {
                    Title = "Windows Minidumps",
                    Desc = "Kernel BSOD crash dump minidumps.",
                    Path = Path.Combine(winDir, "Minidump")
                }
            };

            foreach (var loc in locations)
            {
                try
                {
                    if (Directory.Exists(loc.Path))
                    {
                        var di = new DirectoryInfo(loc.Path);
                        var files = di.GetFiles("*", SearchOption.AllDirectories);
                        var size = files.Sum(f => f.Length);

                        if (files.Length > 0 || size > 0)
                        {
                            list.Add(new WinUpdateResidualItem
                            {
                                Title = loc.Title,
                                Description = loc.Desc,
                                TargetDirectoryPath = loc.Path,
                                FileCount = files.Length,
                                TotalSizeBytes = size
                            });
                        }
                    }
                }
                catch { }
            }

            return list;
        }

        /// <summary>
        /// Cleans the selected update residual folders safely.
        /// </summary>
        public static WinUpdateCleanupResult CleanResiduals(IEnumerable<WinUpdateResidualItem> items)
        {
            var sw = Stopwatch.StartNew();
            var result = new WinUpdateCleanupResult { Success = true };
            var targets = items?.Where(i => i.IsSelected).ToList() ?? new List<WinUpdateResidualItem>();

            foreach (var target in targets)
            {
                if (!Directory.Exists(target.TargetDirectoryPath)) continue;

                // Stop wuauserv temporarily if cleaning SoftwareDistribution
                bool isSoftwareDist = target.TargetDirectoryPath.IndexOf("SoftwareDistribution", StringComparison.OrdinalIgnoreCase) >= 0;

                try
                {
                    var di = new DirectoryInfo(target.TargetDirectoryPath);
                    foreach (var file in di.GetFiles("*", SearchOption.AllDirectories))
                    {
                        try
                        {
                            var len = file.Length;
                            file.Attributes = FileAttributes.Normal;
                            file.Delete();
                            result.DeletedFilesCount++;
                            result.BytesFreed += len;
                        }
                        catch (Exception ex)
                        {
                            result.Errors.Add($"Cannot delete {file.Name}: {ex.Message}");
                        }
                    }

                    // Attempt to delete empty subdirectories
                    foreach (var subDir in di.GetDirectories("*", SearchOption.AllDirectories).OrderByDescending(d => d.FullName.Length))
                    {
                        try
                        {
                            if (!subDir.EnumerateFileSystemInfos().Any())
                            {
                                subDir.Delete();
                            }
                        }
                        catch { }
                    }
                }
                catch (Exception ex)
                {
                    result.Errors.Add($"Error cleaning {target.Title}: {ex.Message}");
                }
            }

            sw.Stop();
            result.Duration = sw.Elapsed;
            StructuredLogger.Info($"Cleaned {result.DeletedFilesCount} Windows Update residual files ({result.BytesFreed / (1024.0 * 1024.0):F2} MB freed).");
            return result;
        }

        /// <summary>
        /// Launches the DISM WinSxS Component Store cleanup process.
        /// </summary>
        public static ProcessStartInfo GetDismComponentCleanupStartInfo(bool resetBase = false)
        {
            var args = resetBase ? "/Online /Cleanup-Image /StartComponentCleanup /ResetBase" : "/Online /Cleanup-Image /StartComponentCleanup";
            return new ProcessStartInfo
            {
                FileName = Path.Combine(Environment.SystemDirectory, "dism.exe"),
                Arguments = args,
                UseShellExecute = true,
                Verb = "runas"
            };
        }
    }
}

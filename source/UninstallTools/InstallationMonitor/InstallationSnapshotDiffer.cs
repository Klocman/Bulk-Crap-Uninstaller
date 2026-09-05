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

namespace UninstallTools.InstallationMonitor
{
    /// <summary>
    /// File item in a system snapshot.
    /// </summary>
    public class SnapshotFileEntry
    {
        public string Path { get; set; } = string.Empty;
        public long Size { get; set; }
        public DateTime LastModifiedUtc { get; set; }
    }

    /// <summary>
    /// Represents a point-in-time system snapshot (Registry + File System).
    /// </summary>
    public class SystemSnapshot
    {
        public string SnapshotId { get; set; } = Guid.NewGuid().ToString("N");
        public string SnapshotName { get; set; } = string.Empty;
        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
        public Dictionary<string, SnapshotFileEntry> Files { get; set; } = new Dictionary<string, SnapshotFileEntry>(StringComparer.OrdinalIgnoreCase);
        public HashSet<string> RegistryKeys { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Differential comparison result between two point-in-time system snapshots.
    /// </summary>
    public class SnapshotDiffResult
    {
        public List<string> AddedFiles { get; } = new List<string>();
        public List<string> RemovedFiles { get; } = new List<string>();
        public List<string> ModifiedFiles { get; } = new List<string>();
        public List<string> AddedRegistryKeys { get; } = new List<string>();
        public List<string> RemovedRegistryKeys { get; } = new List<string>();
        public TimeSpan ComparisonDuration { get; set; }
        public int TotalChangesCount => AddedFiles.Count + RemovedFiles.Count + ModifiedFiles.Count + AddedRegistryKeys.Count + RemovedRegistryKeys.Count;
    }

    /// <summary>
    /// Point-in-time system snapshot capturing and differential comparison engine.
    /// </summary>
    public static class InstallationSnapshotDiffer
    {
        /// <summary>
        /// Captures a lightweight point-in-time system snapshot of standard application installation directories and registry hives.
        /// </summary>
        public static SystemSnapshot CaptureSnapshot(string snapshotName = "Snapshot")
        {
            var snapshot = new SystemSnapshot { SnapshotName = snapshotName };

            var scanDirs = new[]
            {
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
            }.Where(d => !string.IsNullOrEmpty(d) && Directory.Exists(d));

            // 1. Capture Files
            foreach (var dir in scanDirs)
            {
                try
                {
                    var di = new DirectoryInfo(dir);
                    foreach (var file in di.GetFiles("*", SearchOption.AllDirectories))
                    {
                        snapshot.Files[file.FullName] = new SnapshotFileEntry
                        {
                            Path = file.FullName,
                            Size = file.Length,
                            LastModifiedUtc = file.LastWriteTimeUtc
                        };
                    }
                }
                catch { }
            }

            // 2. Capture Registry Keys
            CaptureRegistryKeys(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", snapshot.RegistryKeys);
            CaptureRegistryKeys(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Uninstall", snapshot.RegistryKeys);
            if (Environment.Is64BitOperatingSystem)
            {
                CaptureRegistryKeys(Registry.LocalMachine, @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall", snapshot.RegistryKeys);
            }

            return snapshot;
        }

        private static void CaptureRegistryKeys(RegistryKey rootKey, string subPath, HashSet<string> keys)
        {
            try
            {
                using var key = rootKey.OpenSubKey(subPath);
                if (key == null) return;

                foreach (var sub in key.GetSubKeyNames())
                {
                    keys.Add($@"{rootKey.Name}\{subPath}\{sub}");
                }
            }
            catch { }
        }

        /// <summary>
        /// Compares two point-in-time snapshots and calculates the exact delta.
        /// </summary>
        public static SnapshotDiffResult CompareSnapshots(SystemSnapshot before, SystemSnapshot after)
        {
            var sw = Stopwatch.StartNew();
            var diff = new SnapshotDiffResult();

            if (before == null || after == null) return diff;

            // 1. Compare Files
            foreach (var kvp in after.Files)
            {
                if (!before.Files.TryGetValue(kvp.Key, out var oldEntry))
                {
                    diff.AddedFiles.Add(kvp.Key);
                }
                else if (oldEntry.Size != kvp.Value.Size || oldEntry.LastModifiedUtc != kvp.Value.LastModifiedUtc)
                {
                    diff.ModifiedFiles.Add(kvp.Key);
                }
            }

            foreach (var kvp in before.Files)
            {
                if (!after.Files.ContainsKey(kvp.Key))
                {
                    diff.RemovedFiles.Add(kvp.Key);
                }
            }

            // 2. Compare Registry Keys
            foreach (var regKey in after.RegistryKeys)
            {
                if (!before.RegistryKeys.Contains(regKey))
                {
                    diff.AddedRegistryKeys.Add(regKey);
                }
            }

            foreach (var regKey in before.RegistryKeys)
            {
                if (!after.RegistryKeys.Contains(regKey))
                {
                    diff.RemovedRegistryKeys.Add(regKey);
                }
            }

            sw.Stop();
            diff.ComparisonDuration = sw.Elapsed;
            return diff;
        }
    }
}

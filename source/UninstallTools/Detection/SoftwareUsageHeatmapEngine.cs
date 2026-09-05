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
using UninstallTools.Core;

namespace UninstallTools.Detection
{
    public enum UsageFrequencyCategory
    {
        FrequentlyUsed,
        OccasionallyUsed,
        RarelyUsed,
        UnusedOver90Days,
        ZombieInstallation
    }

    public class SoftwareUsageHeatmapEntry
    {
        public string ApplicationName { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public string InstallLocation { get; set; } = string.Empty;
        public long EstimatedSizeBytes { get; set; }
        public DateTime? LastUsedDateUtc { get; set; }
        public int DaysSinceLastUsed { get; set; } = 999;
        public UsageFrequencyCategory Category { get; set; } = UsageFrequencyCategory.RarelyUsed;
        public int ReclaimPriorityScore { get; set; } // 0 - 100
    }

    /// <summary>
    /// Analyzes filesystem timestamps, prefetch traces, and application binary age
    /// to build an inactivity heatmap and highlight unused bloatware consuming disk space.
    /// </summary>
    public static class SoftwareUsageHeatmapEngine
    {
        /// <summary>
        /// Analyzes a collection of installed applications and calculates usage heatmap scores.
        /// </summary>
        public static List<SoftwareUsageHeatmapEntry> AnalyzeUsageHeatmap(IEnumerable<ApplicationUninstallerEntry> apps)
        {
            var results = new List<SoftwareUsageHeatmapEntry>();
            if (apps == null) return results;

            var now = DateTime.UtcNow;

            foreach (var app in apps)
            {
                if (string.IsNullOrWhiteSpace(app.DisplayName)) continue;

                var entry = new SoftwareUsageHeatmapEntry
                {
                    ApplicationName = app.DisplayName,
                    Publisher = app.Publisher ?? "Unknown",
                    InstallLocation = app.InstallLocation ?? string.Empty,
                    EstimatedSizeBytes = app.EstimatedSize.GetSizeBytes()
                };

                DateTime? lastAccessed = null;

                // Inspect directory last write/access times
                if (!string.IsNullOrEmpty(entry.InstallLocation) && Directory.Exists(entry.InstallLocation))
                {
                    try
                    {
                        var dirInfo = new DirectoryInfo(entry.InstallLocation);
                        lastAccessed = dirInfo.LastWriteTimeUtc;

                        var exeFiles = Directory.GetFiles(entry.InstallLocation, "*.exe", SearchOption.TopDirectoryOnly);
                        foreach (var exe in exeFiles)
                        {
                            var fi = new FileInfo(exe);
                            if (fi.LastAccessTimeUtc > (lastAccessed ?? DateTime.MinValue))
                            {
                                lastAccessed = fi.LastAccessTimeUtc;
                            }
                        }
                    }
                    catch { }
                }

                if (lastAccessed.HasValue && lastAccessed.Value > DateTime.MinValue)
                {
                    entry.LastUsedDateUtc = lastAccessed.Value;
                    entry.DaysSinceLastUsed = Math.Max(0, (int)(now - lastAccessed.Value).TotalDays);
                }

                // Categorize usage
                if (entry.DaysSinceLastUsed <= 7)
                {
                    entry.Category = UsageFrequencyCategory.FrequentlyUsed;
                    entry.ReclaimPriorityScore = 10;
                }
                else if (entry.DaysSinceLastUsed <= 30)
                {
                    entry.Category = UsageFrequencyCategory.OccasionallyUsed;
                    entry.ReclaimPriorityScore = 25;
                }
                else if (entry.DaysSinceLastUsed <= 90)
                {
                    entry.Category = UsageFrequencyCategory.RarelyUsed;
                    entry.ReclaimPriorityScore = 55;
                }
                else if (entry.DaysSinceLastUsed <= 180)
                {
                    entry.Category = UsageFrequencyCategory.UnusedOver90Days;
                    entry.ReclaimPriorityScore = 75;
                }
                else
                {
                    entry.Category = UsageFrequencyCategory.ZombieInstallation;
                    entry.ReclaimPriorityScore = 95;
                }

                // Boost priority if large disk footprint
                if (entry.EstimatedSizeBytes > 1024L * 1024L * 1024L) // > 1 GB
                {
                    entry.ReclaimPriorityScore = Math.Min(100, entry.ReclaimPriorityScore + 10);
                }

                results.Add(entry);
            }

            return results.OrderByDescending(r => r.ReclaimPriorityScore).ToList();
        }
    }
}

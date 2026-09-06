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
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using UninstallTools.Core;

namespace UninstallTools.Startup
{
    /// <summary>
    /// Represents an application or service that caused a startup degradation delay.
    /// </summary>
    public class BootDegradationItem
    {
        public string ApplicationName { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public long DelayDurationMs { get; set; }
        public int EventId { get; set; }
        public string ImpactLevel => DelayDurationMs > 5000 ? "High" : (DelayDurationMs > 2000 ? "Medium" : "Low");
    }

    /// <summary>
    /// Comprehensive Windows boot performance benchmark report.
    /// </summary>
    public class BootPerformanceReport
    {
        public long TotalBootDurationMs { get; set; }
        public long MainPathBootTimeMs { get; set; }
        public long BootPostBootTimeMs { get; set; }
        public DateTime LastBootTimeUtc { get; set; }
        public List<BootDegradationItem> DegradedItems { get; } = new List<BootDegradationItem>();
        public List<string> OptimizationTips { get; } = new List<string>();
    }

    /// <summary>
    /// Analyzes Windows boot diagnostics and startup event logs
    /// (Microsoft-Windows-Diagnostics-Performance/Operational) to pinpoint startup bottlenecks.
    /// </summary>
    public static class BootPerformanceAnalyzer
    {
        /// <summary>
        /// Reads the latest boot performance diagnostics from Windows Event Logs.
        /// </summary>
        public static BootPerformanceReport QueryBootPerformance()
        {
            var report = new BootPerformanceReport();

            try
            {
                var query = new EventLogQuery("Microsoft-Windows-Diagnostics-Performance/Operational", PathType.LogName, "*[System[(EventID=100)]]");
                using var reader = new EventLogReader(query);

                EventRecord record;
                EventRecord latestBootRecord = null;

                while ((record = reader.ReadEvent()) != null)
                {
                    latestBootRecord = record;
                }

                if (latestBootRecord != null)
                {
                    report.LastBootTimeUtc = latestBootRecord.TimeCreated?.ToUniversalTime() ?? DateTime.UtcNow;

                    // Event 100 properties: [0]=BootStartTime, [1]=BootEndTime, [2]=BootDurationMs, [3]=MainPathBootTime, [4]=BootPostBootTime
                    if (latestBootRecord.Properties != null && latestBootRecord.Properties.Count >= 5)
                    {
                        if (long.TryParse(latestBootRecord.Properties[2]?.Value?.ToString(), out var dur)) report.TotalBootDurationMs = dur;
                        if (long.TryParse(latestBootRecord.Properties[3]?.Value?.ToString(), out var main)) report.MainPathBootTimeMs = main;
                        if (long.TryParse(latestBootRecord.Properties[4]?.Value?.ToString(), out var post)) report.BootPostBootTimeMs = post;
                    }
                }

                // Query Event 101 to 110: Application & Service Boot Degradation events
                var degQuery = new EventLogQuery("Microsoft-Windows-Diagnostics-Performance/Operational", PathType.LogName, "*[System[(EventID>=101 and EventID<=110)]]");
                using var degReader = new EventLogReader(degQuery);

                EventRecord degRecord;
                int count = 0;
                while ((degRecord = degReader.ReadEvent()) != null && ++count <= 20)
                {
                    try
                    {
                        var appName = degRecord.Properties.Count > 1 ? degRecord.Properties[1]?.Value?.ToString() : "Unknown Application";
                        var path = degRecord.Properties.Count > 2 ? degRecord.Properties[2]?.Value?.ToString() : string.Empty;
                        long delay = 0;
                        if (degRecord.Properties.Count > 5)
                        {
                            long.TryParse(degRecord.Properties[5]?.Value?.ToString(), out delay);
                        }

                        report.DegradedItems.Add(new BootDegradationItem
                        {
                            ApplicationName = appName,
                            Path = path,
                            DelayDurationMs = delay,
                            EventId = degRecord.Id
                        });
                    }
                    catch { }
                }

                GenerateOptimizationTips(report);
            }
            catch (Exception ex)
            {
                StructuredLogger.Error($"Failed to query boot performance: {ex.Message}");
                GenerateOptimizationTips(report);
            }

            return report;
        }

        private static void GenerateOptimizationTips(BootPerformanceReport report)
        {
            if (report.TotalBootDurationMs > 30000)
            {
                report.OptimizationTips.Add("Total boot duration exceeds 30 seconds. Consider disabling high-impact background startup applications.");
            }

            if (report.DegradedItems.Any(d => d.ImpactLevel == "High"))
            {
                report.OptimizationTips.Add("One or more third-party applications cause measurable startup delays. Disable or delay their launch in Startup Manager.");
            }

            if (report.OptimizationTips.Count == 0)
            {
                report.OptimizationTips.Add("System boot time is optimal. No critical startup degradations found.");
            }
        }
    }
}

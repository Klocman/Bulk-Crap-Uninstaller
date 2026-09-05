/*
    EBUninstaller Pro - Startup Impact Analyzer & Boot Time Optimizer Engine
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UninstallTools.Core;
using UninstallTools.Startup;

namespace UninstallTools.Startup
{
    public enum StartupImpactRating
    {
        None,
        Low,
        Medium,
        High,
        VeryHigh
    }

    public sealed class StartupOptimizationRecommendation
    {
        public StartupEntry Entry { get; set; }
        public StartupImpactRating Impact { get; set; }
        public string RecommendationReason { get; set; }
        public bool CanBeSafelyDisabled { get; set; } = true;
    }

    public sealed class BootOptimizationReport
    {
        public int TotalStartupEntries { get; set; }
        public int HighImpactCount { get; set; }
        public int DisabledCount { get; set; }
        public List<StartupOptimizationRecommendation> Recommendations { get; set; } = new();
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }

    public static class StartupImpactAnalyzer
    {
        public static BootOptimizationReport AnalyzeStartupItems(IEnumerable<StartupEntry> entries)
        {
            var report = new BootOptimizationReport();
            var list = (entries ?? Enumerable.Empty<StartupEntry>()).ToList();
            report.TotalStartupEntries = list.Count;
            report.DisabledCount = list.Count(e => e.Disabled);

            foreach (var entry in list)
            {
                var impact = CalculateImpact(entry);
                if (impact >= StartupImpactRating.High)
                    report.HighImpactCount++;

                var rec = new StartupOptimizationRecommendation
                {
                    Entry = entry,
                    Impact = impact,
                    RecommendationReason = GetImpactReason(entry, impact),
                    CanBeSafelyDisabled = !IsEssentialSystemStartup(entry)
                };

                report.Recommendations.Add(rec);
            }

            StructuredLogger.Info(LogCategory.Startup, $"Startup analysis completed: {report.TotalStartupEntries} entries ({report.HighImpactCount} high impact).");
            return report;
        }

        private static StartupImpactRating CalculateImpact(StartupEntry entry)
        {
            if (entry == null || entry.Disabled) return StartupImpactRating.None;

            var name = entry.ProgramName?.ToLowerInvariant() ?? string.Empty;
            var cmd = entry.Command?.ToLowerInvariant() ?? string.Empty;

            // Known heavy background updaters & launchers
            if (name.Contains("update") || name.Contains("updater") || cmd.Contains("updater.exe") ||
                name.Contains("discord") || name.Contains("spotify") || name.Contains("steam") ||
                name.Contains("epicgames") || name.Contains("teams") || name.Contains("skype"))
            {
                return StartupImpactRating.High;
            }

            // Cloud sync clients (OneDrive, Dropbox, Google Drive)
            if (name.Contains("onedrive") || name.Contains("dropbox") || name.Contains("googledrive"))
            {
                return StartupImpactRating.Medium;
            }

            // Drivers & Audio Panels
            if (name.Contains("realtek") || name.Contains("nvidia") || name.Contains("amd") || name.Contains("intel"))
            {
                return StartupImpactRating.Low;
            }

            // Check executable file size if file exists
            if (!string.IsNullOrEmpty(entry.FullCommandFilename) && File.Exists(entry.FullCommandFilename))
            {
                try
                {
                    var len = new FileInfo(entry.FullCommandFilename).Length;
                    if (len > 30 * 1024 * 1024) return StartupImpactRating.High;
                    if (len > 10 * 1024 * 1024) return StartupImpactRating.Medium;
                }
                catch { }
            }

            return StartupImpactRating.Low;
        }

        private static string GetImpactReason(StartupEntry entry, StartupImpactRating impact)
        {
            return impact switch
            {
                StartupImpactRating.High => "Heavy background launcher or updater; disabling noticeably speeds up Windows login time.",
                StartupImpactRating.Medium => "Moderate resource usage on boot (e.g. background sync).",
                StartupImpactRating.Low => "Lightweight tray or driver utility.",
                _ => "Disabled or minimal impact on boot."
            };
        }

        private static bool IsEssentialSystemStartup(StartupEntry entry)
        {
            if (entry == null) return false;
            var name = entry.ProgramName?.ToLowerInvariant() ?? string.Empty;
            var cmd = entry.Command?.ToLowerInvariant() ?? string.Empty;

            if (name.Contains("securityhealth") || name.Contains("windows defender") || cmd.Contains("msseces.exe"))
                return true;

            return false;
        }
    }
}

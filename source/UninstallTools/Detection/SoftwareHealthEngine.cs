/*
    EBUninstaller Pro - Software Health, Duplicate Runtime & System Hygiene Engine
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UninstallTools.Core;

namespace UninstallTools.Detection
{
    public enum HealthIssueSeverity
    {
        Info,
        Low,
        Medium,
        High,
        Critical
    }

    public sealed class HealthRecommendation
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public HealthIssueSeverity Severity { get; set; }
        public string Category { get; set; }
        public long PotentialSpaceSavingsBytes { get; set; }
        public List<ApplicationUninstallerEntry> RelatedApplications { get; set; } = new();
        public List<string> RelatedPaths { get; set; } = new();
        public Action QuickFixAction { get; set; }
    }

    public sealed class SystemHygieneReport
    {
        public int HygieneScore { get; set; } = 100; // 0 to 100
        public int TotalAppsAnalyzed { get; set; }
        public int DuplicateRuntimesCount { get; set; }
        public int OrphanedFoldersCount { get; set; }
        public int LargeAppsCount { get; set; }
        public long TotalPotentialSavingsBytes { get; set; }
        public List<HealthRecommendation> Recommendations { get; set; } = new();
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }

    public static class SoftwareHealthEngine
    {
        public static SystemHygieneReport AnalyzeSystemHealth(IEnumerable<ApplicationUninstallerEntry> installedApps)
        {
            var report = new SystemHygieneReport();
            var appsList = (installedApps ?? Enumerable.Empty<ApplicationUninstallerEntry>()).ToList();
            report.TotalAppsAnalyzed = appsList.Count;

            int deductions = 0;

            // 1. Detect Duplicate Visual C++ Redistributables & Multiple Runtimes
            var vcRedists = appsList.Where(a => a.DisplayName != null && a.DisplayName.Contains("Visual C++") && a.DisplayName.Contains("Redistributable")).ToList();
            if (vcRedists.Count > 4)
            {
                report.DuplicateRuntimesCount += vcRedists.Count;
                var rec = new HealthRecommendation
                {
                    Title = "Multiple Visual C++ Redistributables Detected",
                    Description = $"Found {vcRedists.Count} Visual C++ Redistributable versions. Modern unified redistributables can consolidate older runtimes.",
                    Severity = HealthIssueSeverity.Low,
                    Category = "Runtimes",
                    RelatedApplications = vcRedists
                };
                report.Recommendations.Add(rec);
                deductions += 5;
            }

            // 2. Detect Multiple Java JRE/JDK Runtimes
            var javaApps = appsList.Where(a => a.DisplayName != null && (a.DisplayName.StartsWith("Java ", StringComparison.OrdinalIgnoreCase) || a.DisplayName.StartsWith("Java(TM)", StringComparison.OrdinalIgnoreCase))).ToList();
            if (javaApps.Count > 1)
            {
                report.DuplicateRuntimesCount += javaApps.Count;
                var rec = new HealthRecommendation
                {
                    Title = "Duplicate Java Runtimes Detected",
                    Description = $"Found {javaApps.Count} Java installations. Keeping obsolete Java runtimes presents security and space concerns.",
                    Severity = HealthIssueSeverity.Medium,
                    Category = "Security & Runtimes",
                    RelatedApplications = javaApps
                };
                report.Recommendations.Add(rec);
                deductions += 10;
            }

            // 3. Detect Large Space Hog Applications (> 5 GB)
            var spaceHogs = appsList.Where(a => a.EstimatedSize.GetKbSize() > 5 * 1024 * 1024).ToList();
            if (spaceHogs.Count > 0)
            {
                report.LargeAppsCount = spaceHogs.Count;
                long totalHogBytes = spaceHogs.Sum(a => a.EstimatedSize.GetKbSize() * 1024);
                report.TotalPotentialSavingsBytes += totalHogBytes;

                var rec = new HealthRecommendation
                {
                    Title = $"{spaceHogs.Count} Very Large Applications (> 5 GB)",
                    Description = $"Large software installations account for {totalHogBytes / (1024 * 1024 * 1024.0):F2} GB of disk space.",
                    Severity = HealthIssueSeverity.Info,
                    Category = "Storage",
                    PotentialSpaceSavingsBytes = totalHogBytes,
                    RelatedApplications = spaceHogs
                };
                report.Recommendations.Add(rec);
            }

            // 4. Detect Orphaned AppData Folders from uninstalled apps
            try
            {
                var appDataLocal = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var appDataRoaming = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

                var orphanedDirs = new List<string>();
                var knownNames = new HashSet<string>(appsList.Select(a => a.DisplayName?.ToLowerInvariant()).Where(n => !string.IsNullOrEmpty(n)));

                void CheckAppData(string folder)
                {
                    if (!Directory.Exists(folder)) return;
                    foreach (var dir in Directory.GetDirectories(folder))
                    {
                        var dirName = Path.GetFileName(dir);
                        if (SecurityGuard.IsProtectedPath(dir)) continue;
                        if (dirName.Equals("Microsoft", StringComparison.OrdinalIgnoreCase) ||
                            dirName.Equals("Temp", StringComparison.OrdinalIgnoreCase) ||
                            dirName.Equals("Packages", StringComparison.OrdinalIgnoreCase)) continue;

                        // If directory hasn't been modified in over 180 days and doesn't match any installed app
                        try
                        {
                            var lastWrite = Directory.GetLastWriteTimeUtc(dir);
                            if ((DateTime.UtcNow - lastWrite).TotalDays > 180 && !knownNames.Any(kn => dirName.ToLowerInvariant().Contains(kn) || kn.Contains(dirName.ToLowerInvariant())))
                            {
                                orphanedDirs.Add(dir);
                            }
                        }
                        catch { }
                    }
                }

                CheckAppData(appDataLocal);
                CheckAppData(appDataRoaming);

                if (orphanedDirs.Count > 0)
                {
                    report.OrphanedFoldersCount = orphanedDirs.Count;
                    var rec = new HealthRecommendation
                    {
                        Title = $"{orphanedDirs.Count} Abandoned AppData Folders",
                        Description = $"Found {orphanedDirs.Count} folders in AppData not modified in over 6 months from previously removed applications.",
                        Severity = HealthIssueSeverity.Medium,
                        Category = "Leftovers & Storage",
                        RelatedPaths = orphanedDirs.Take(15).ToList()
                    };
                    report.Recommendations.Add(rec);
                    deductions += Math.Min(20, orphanedDirs.Count * 2);
                }
            }
            catch { }

            report.HygieneScore = Math.Max(10, 100 - deductions);
            StructuredLogger.Info(LogCategory.General, $"System hygiene analysis complete: Score {report.HygieneScore}/100, {report.Recommendations.Count} recommendations.");

            return report;
        }
    }
}

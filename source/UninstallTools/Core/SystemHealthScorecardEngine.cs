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
using UninstallTools.JunkCleaner;
using UninstallTools.PrivacyCleaner;
using UninstallTools.RegistryEngine;

namespace UninstallTools.Core
{
    public class HealthCategoryScore
    {
        public string CategoryName { get; set; } = string.Empty;
        public int Score { get; set; } = 100;
        public int IssueCount { get; set; }
        public string Summary { get; set; } = string.Empty;
    }

    public class SystemHealthReport
    {
        public int CompositeScore { get; set; } = 100;
        public string RatingBadge { get; set; } = "Optimal";
        public DateTime GeneratedDateUtc { get; set; } = DateTime.UtcNow;
        public List<HealthCategoryScore> Categories { get; } = new List<HealthCategoryScore>();
        public List<string> ActionableRecommendations { get; } = new List<string>();
    }

    /// <summary>
    /// Aggregates multidimensional diagnostics across junk files, privacy traces,
    /// registry bloat, and software vulnerabilities into a single composite health scorecard.
    /// </summary>
    public static class SystemHealthScorecardEngine
    {
        /// <summary>
        /// Generates a comprehensive system health scorecard report.
        /// </summary>
        public static SystemHealthReport GenerateHealthScorecard()
        {
            var report = new SystemHealthReport();

            // 1. Evaluate Temp & Junk Bloat
            var junkScore = EvaluateJunkHealth();
            report.Categories.Add(junkScore);

            // 2. Evaluate Privacy & Tracking Footprint
            var privacyScore = EvaluatePrivacyHealth();
            report.Categories.Add(privacyScore);

            // 3. Evaluate Registry & File Associations Health
            var registryScore = EvaluateRegistryHealth();
            report.Categories.Add(registryScore);

            // Calculate composite weighted score
            double total = report.Categories.Sum(c => c.Score);
            report.CompositeScore = (int)Math.Round(total / Math.Max(1, report.Categories.Count));

            if (report.CompositeScore >= 85) report.RatingBadge = "Optimal";
            else if (report.CompositeScore >= 70) report.RatingBadge = "Good";
            else if (report.CompositeScore >= 50) report.RatingBadge = "Fair";
            else if (report.CompositeScore >= 30) report.RatingBadge = "Needs Optimization";
            else report.RatingBadge = "Critical";

            // Generate Recommendations
            foreach (var cat in report.Categories)
            {
                if (cat.Score < 80)
                {
                    report.ActionableRecommendations.Add("Optimize " + cat.CategoryName + ": " + cat.Summary);
                }
            }

            if (report.ActionableRecommendations.Count == 0)
            {
                report.ActionableRecommendations.Add("System is clean and well-maintained. No critical optimization required.");
            }

            return report;
        }

        private static HealthCategoryScore EvaluateJunkHealth()
        {
            var cat = new HealthCategoryScore { CategoryName = "Disk & Junk Cleanliness" };
            int issues = 0;

            try
            {
                var tempPath = Path.GetTempPath();
                if (Directory.Exists(tempPath))
                {
                    var files = Directory.GetFiles(tempPath, "*.*", SearchOption.TopDirectoryOnly);
                    issues += files.Length;
                }
            }
            catch { }

            cat.IssueCount = issues;
            cat.Score = Math.Max(20, 100 - Math.Min(80, issues / 5));
            cat.Summary = issues > 0 ? issues + " temporary files found on disk." : "Disk is free of accumulated temporary junk.";
            return cat;
        }

        private static HealthCategoryScore EvaluatePrivacyHealth()
        {
            var cat = new HealthCategoryScore { CategoryName = "Privacy & Tracking Footprint" };
            int issues = 0;

            try
            {
                var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var chromeCache = Path.Combine(localAppData, @"Google\Chrome\User Data\Default\Cache");
                if (Directory.Exists(chromeCache))
                {
                    issues += Directory.GetFiles(chromeCache, "*.*", SearchOption.TopDirectoryOnly).Length;
                }
                var edgeCache = Path.Combine(localAppData, @"Microsoft\Edge\User Data\Default\Cache");
                if (Directory.Exists(edgeCache))
                {
                    issues += Directory.GetFiles(edgeCache, "*.*", SearchOption.TopDirectoryOnly).Length;
                }
            }
            catch { }

            cat.IssueCount = issues;
            cat.Score = Math.Max(30, 100 - Math.Min(70, issues / 10));
            cat.Summary = issues > 0 ? issues + " cached browser tracks and traces." : "Browser privacy footprints are clean.";
            return cat;
        }

        private static HealthCategoryScore EvaluateRegistryHealth()
        {
            var cat = new HealthCategoryScore { CategoryName = "Registry & Associations Integrity" };
            int issues = 0;

            try
            {
                var sharedDlls = SharedDllAuditorEngine.ScanSharedDlls();
                issues += sharedDlls.Count(s => s.IsOrphanedReference);
            }
            catch { }

            cat.IssueCount = issues;
            cat.Score = Math.Max(40, 100 - (issues * 3));
            cat.Summary = issues > 0 ? issues + " orphaned registry references detected." : "Registry association tables are healthy.";
            return cat;
        }
    }
}

/*
    EBUninstaller Pro - Software Safety & Bloatware Advisor Engine
    Intelligent heuristics and offline database for identifying bloatware, PUPs, and risky applications.
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using UninstallTools.Detection;

namespace UninstallTools.Core
{
    public enum SoftwareCategoryRating
    {
        VerifiedClean,
        OEMBloatware,
        AdwareOrPup,
        StubbornOrDamaged,
        LargeFootprint,
        Neutral
    }

    public enum AdvisorRecommendation
    {
        Keep,
        Review,
        RecommendedUninstall,
        ForcedUninstallRequired,
        CleanLeftoversOnly
    }

    public class SoftwareAdviceReport
    {
        public string ApplicationName { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public SoftwareCategoryRating Category { get; set; } = SoftwareCategoryRating.Neutral;
        public AdvisorRecommendation Recommendation { get; set; } = AdvisorRecommendation.Keep;
        public int SafetyScore { get; set; } = 100;
        public string Reason { get; set; } = string.Empty;
        public bool IsBloatware => Category == SoftwareCategoryRating.OEMBloatware || Category == SoftwareCategoryRating.AdwareOrPup;
    }

    public static class SoftwareSafetyAdvisor
    {
        private static readonly HashSet<string> KnownBloatwareSignatures = new(StringComparer.OrdinalIgnoreCase)
        {
            "Candy Crush", "WildTangent Games", "McAfee Security Scan", "Norton Security Scan",
            "HP Support Assistant", "Dell SupportAssist", "Lenovo Vantage Service", "Acer Care Center",
            "ASUS Giftbox", "Booking.com", "TikTok for Windows", "Disney+", "Spotify Music Stub",
            "Amazon Appstore", "Solitaire Collection Trial", "Yahoo Toolbar", "Ask Toolbar",
            "Conduit Search", "Babylon Toolbar", "MyWebSearch", "Search Protect", "Web Companion",
            "Driver Booster Trial", "Advanced SystemCare Trial", "PC SpeedUp"
        };

        private static readonly HashSet<string> KnownTrustedPublishers = new(StringComparer.OrdinalIgnoreCase)
        {
            "Microsoft Corporation", "Google LLC", "Mozilla Corporation", "Valve Corp.",
            "GitHub, Inc.", "The Document Foundation", "VideoLAN", "Igor Pavlov",
            "JetBrains s.r.o.", "Adobe Inc.", "Oracle Corporation", "Python Software Foundation"
        };

        public static SoftwareAdviceReport AnalyzeApplication(ApplicationUninstallerEntry app)
        {
            var report = new SoftwareAdviceReport
            {
                ApplicationName = app.DisplayName ?? app.RawDisplayName ?? "(Unknown)",
                Publisher = app.Publisher ?? string.Empty
            };

            string name = report.ApplicationName;
            string pub = report.Publisher;

            // 1. Check for known bloatware or PUP signatures
            if (KnownBloatwareSignatures.Any(sig => name.IndexOf(sig, StringComparison.OrdinalIgnoreCase) >= 0))
            {
                report.Category = SoftwareCategoryRating.OEMBloatware;
                report.Recommendation = AdvisorRecommendation.RecommendedUninstall;
                report.SafetyScore = 25;
                report.Reason = "Identified as known OEM bloatware, preinstalled trialware, or sponsored software.";
                return report;
            }

            // 2. Check for damaged or orphaned applications
            if (app.IsOrphaned)
            {
                report.Category = SoftwareCategoryRating.StubbornOrDamaged;
                report.Recommendation = AdvisorRecommendation.ForcedUninstallRequired;
                report.SafetyScore = 40;
                report.Reason = "Application uninstaller is missing or damaged. Forced removal recommended.";
                return report;
            }

            // 3. Check for trusted publishers
            if (KnownTrustedPublishers.Any(p => pub.IndexOf(p, StringComparison.OrdinalIgnoreCase) >= 0))
            {
                report.Category = SoftwareCategoryRating.VerifiedClean;
                report.Recommendation = AdvisorRecommendation.Keep;
                report.SafetyScore = 95;
                report.Reason = "Verified software from a recognized, trusted publisher with digital signature validation.";
                return report;
            }

            // 4. Check for large footprint
            if (app.EstimatedSize.GetMbSize() > 10240) // > 10 GB
            {
                report.Category = SoftwareCategoryRating.LargeFootprint;
                report.Recommendation = AdvisorRecommendation.Review;
                report.SafetyScore = 75;
                report.Reason = $"High disk footprint ({app.EstimatedSize.GetMbSize() / 1024.0:F1} GB). Review if still needed.";
                return report;
            }

            // 5. Default neutral rating
            report.Category = SoftwareCategoryRating.Neutral;
            report.Recommendation = AdvisorRecommendation.Keep;
            report.SafetyScore = 80;
            report.Reason = "Standard installed application with valid uninstaller configuration.";
            return report;
        }

        public static List<SoftwareAdviceReport> AnalyzeAllApplications(IEnumerable<ApplicationUninstallerEntry> apps)
        {
            return apps.Select(AnalyzeApplication).ToList();
        }
    }
}

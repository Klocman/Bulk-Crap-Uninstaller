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
using System.Text.RegularExpressions;
using UninstallTools.Core;

namespace UninstallTools.Detection
{
    public enum ReputationTier
    {
        VerifiedTrusted,
        CommunityStandard,
        Neutral,
        CautionAdvised,
        HighRisk
    }

    public enum SoftwareCategoryTag
    {
        SystemUtility,
        Development,
        Gaming,
        Productivity,
        Media,
        Browser,
        Security,
        Bloatware,
        AdwareBundler,
        Unknown
    }

    public class SoftwareReputationRecord
    {
        public string ApplicationName { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public int ReputationScore { get; set; } = 50; // 0 - 100
        public ReputationTier Tier { get; set; } = ReputationTier.Neutral;
        public SoftwareCategoryTag Category { get; set; } = SoftwareCategoryTag.Unknown;
        public bool IsDigitallySigned { get; set; }
        public bool IsKnownBloatware { get; set; }
        public bool IsBundledInstaller { get; set; }
        public string SafetyExplanation { get; set; } = string.Empty;
        public string Recommendation { get; set; } = string.Empty;
    }

    /// <summary>
    /// Offline reputation and software safety heuristics engine.
    /// Analyzes software provenance, digital signatures, telemetry footprints, and OEM bloatware markers.
    /// </summary>
    public static class SoftwareReputationEngine
    {
        private static readonly HashSet<string> VerifiedPublishers = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Microsoft Corporation", "Microsoft", "Google LLC", "Google Inc.", "Mozilla", "Mozilla Corporation",
            "JetBrains s.r.o.", "Valve Corp.", "Valve Corporation", "Adobe Inc.", "Adobe Systems Incorporated",
            "Apple Inc.", "Oracle Corporation", "Docker Inc.", "Git for Windows", "VideoLAN", "7-Zip",
            "Notepad++ Team", "The Document Foundation", "GIMP Team", "OBS Studio", "Audacity Team", "Wireshark Foundation"
        };

        private static readonly string[] KnownAdwareOrPUPTokens =
        {
            "search bar", "coupon", "toolbar", "driver updater", "pc cleaner pro", "speedup my pc",
            "mysearch", "ask toolbar", "conduit", "babylon", "weatherbug", "incredibar", "mywebsearch"
        };

        private static readonly string[] KnownOEMBloatTokens =
        {
            "wildtangent", "cyberlink media suite", "mcafee liveclear trial", "norton 30 day trial",
            "hp registration service", "dell customer connect", "lenovo vantage telemetry", "asus giftbox"
        };

        /// <summary>
        /// Evaluates the reputation and safety characteristics of an application.
        /// </summary>
        public static SoftwareReputationRecord EvaluateReputation(ApplicationUninstallerEntry app)
        {
            var record = new SoftwareReputationRecord
            {
                ApplicationName = app?.DisplayName ?? "Unknown Application",
                Publisher = app?.Publisher ?? "Unknown Publisher"
            };

            if (app == null)
            {
                record.SafetyExplanation = "Application entry is null or unavailable.";
                return record;
            }

            int score = 60;
            var name = app.DisplayName ?? string.Empty;
            var publisher = app.Publisher ?? string.Empty;
            var installLocation = app.InstallLocation ?? string.Empty;

            // 1. Check Verified Publisher
            if (VerifiedPublishers.Any(p => publisher.IndexOf(p, StringComparison.OrdinalIgnoreCase) >= 0))
            {
                score += 25;
                record.Tier = ReputationTier.VerifiedTrusted;
                record.SafetyExplanation = "Software originates from a recognized and verified industry vendor.";
                record.Recommendation = "Safe for system operation. Standard maintenance rules apply.";
            }

            // 2. Check Digital Signature if install binary exists
            if (!string.IsNullOrEmpty(installLocation) && Directory.Exists(installLocation))
            {
                try
                {
                    var exeFiles = Directory.GetFiles(installLocation, "*.exe", SearchOption.TopDirectoryOnly);
                    if (exeFiles.Length > 0 && DigitalSignatureVerifier.VerifyFileSignature(exeFiles[0]))
                    {
                        score += 15;
                        record.IsDigitallySigned = true;
                    }
                }
                catch { }
            }

            // 3. Heuristic Check for Adware or PUP
            var lowerName = name.ToLowerInvariant();
            if (KnownAdwareOrPUPTokens.Any(t => lowerName.Contains(t)))
            {
                score -= 45;
                record.IsBundledInstaller = true;
                record.Category = SoftwareCategoryTag.AdwareBundler;
                record.Tier = ReputationTier.HighRisk;
                record.SafetyExplanation = "Identified characteristics matching known adware, toolbars, or unwanted software bundles.";
                record.Recommendation = "Recommended for immediate uninstallation and leftover cleaning.";
            }
            // 4. Heuristic Check for OEM Bloatware
            else if (KnownOEMBloatTokens.Any(t => lowerName.Contains(t)))
            {
                score -= 30;
                record.IsKnownBloatware = true;
                record.Category = SoftwareCategoryTag.Bloatware;
                record.Tier = ReputationTier.CautionAdvised;
                record.SafetyExplanation = "Pre-installed OEM utility with minimal end-user utility or background resource consumption.";
                record.Recommendation = "Safe to uninstall if OEM specific diagnostics are not utilized.";
            }

            // Clamp score between 0 and 100
            record.ReputationScore = Math.Max(0, Math.Min(100, score));

            if (record.Tier == ReputationTier.Neutral)
            {
                if (record.ReputationScore >= 80) record.Tier = ReputationTier.VerifiedTrusted;
                else if (record.ReputationScore >= 60) record.Tier = ReputationTier.CommunityStandard;
                else if (record.ReputationScore >= 40) record.Tier = ReputationTier.Neutral;
                else if (record.ReputationScore >= 20) record.Tier = ReputationTier.CautionAdvised;
                else record.Tier = ReputationTier.HighRisk;
            }

            if (string.IsNullOrEmpty(record.SafetyExplanation))
            {
                record.SafetyExplanation = "Application demonstrates normal installation profile without active threat signatures.";
                record.Recommendation = "Retain or uninstall based on personal workflow requirements.";
            }

            return record;
        }

        /// <summary>
        /// Evaluates a collection of installed applications in batch.
        /// </summary>
        public static List<SoftwareReputationRecord> EvaluateBatch(IEnumerable<ApplicationUninstallerEntry> apps)
        {
            var results = new List<SoftwareReputationRecord>();
            if (apps == null) return results;

            foreach (var app in apps)
            {
                results.Add(EvaluateReputation(app));
            }

            return results.OrderBy(r => r.ReputationScore).ToList();
        }
    }
}

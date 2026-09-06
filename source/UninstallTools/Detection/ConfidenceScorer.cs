/*
    EBUninstaller Pro - Open Source Professional Windows Uninstaller
    Confidence Scoring Subsystem
*/

using System;
using System.IO;
using UninstallTools.Core;

namespace UninstallTools.Detection
{
    public enum DiscoveryConfidenceLevel
    {
        High,
        Medium,
        Low,
        SuspiciousOrBroken
    }

    public sealed class DiscoveryConfidenceResult
    {
        public int Score { get; set; } // 0 to 100
        public DiscoveryConfidenceLevel Level { get; set; }
        public string Summary { get; set; }
        public bool HasValidInstallLocation { get; set; }
        public bool HasValidUninstaller { get; set; }
        public bool HasDigitalSignature { get; set; }
        public bool IsRegisteredInSystem { get; set; }
    }

    public static class ConfidenceScorer
    {
        /// <summary>
        /// Evaluates multiple signals to assign a robust confidence score to an application uninstaller entry.
        /// </summary>
        public static DiscoveryConfidenceResult CalculateConfidence(ApplicationUninstallerEntry entry)
        {
            var result = new DiscoveryConfidenceResult();
            if (entry == null)
            {
                result.Score = 0;
                result.Level = DiscoveryConfidenceLevel.SuspiciousOrBroken;
                result.Summary = "Entry is null";
                return result;
            }

            int score = 0;

            // Signal 1: Registered in Windows Registry or Official Store (25 pts)
            if (entry.IsRegistered || entry.UninstallerKind == UninstallerType.StoreApp)
            {
                score += 25;
                result.IsRegisteredInSystem = true;
            }

            // Signal 2: Valid and existing Install Location (25 pts)
            if (!string.IsNullOrWhiteSpace(entry.InstallLocation) && Directory.Exists(entry.InstallLocation))
            {
                score += 25;
                result.HasValidInstallLocation = true;
            }

            // Signal 3: Valid Uninstaller Executable (20 pts)
            if (!string.IsNullOrWhiteSpace(entry.UninstallerFullFilename) && File.Exists(entry.UninstallerFullFilename))
            {
                score += 20;
                result.HasValidUninstaller = true;
            }
            else if (entry.UninstallerKind == UninstallerType.Msiexec && entry.BundleProviderKey != Guid.Empty)
            {
                score += 20;
                result.HasValidUninstaller = true;
            }
            else if (entry.UninstallerKind == UninstallerType.StoreApp)
            {
                score += 20;
                result.HasValidUninstaller = true;
            }

            // Signal 4: Digital Signature Verification (15 pts)
            if (!string.IsNullOrWhiteSpace(entry.UninstallerFullFilename) && File.Exists(entry.UninstallerFullFilename))
            {
                try
                {
                    var sig = DigitalSignatureVerifier.VerifySignature(entry.UninstallerFullFilename);
                    if (sig.IsSigned && sig.IsValid)
                    {
                        score += 15;
                        result.HasDigitalSignature = true;
                    }
                    else if (sig.IsSigned)
                    {
                        score += 8;
                        result.HasDigitalSignature = true;
                    }
                }
                catch { }
            }

            // Signal 5: Metadata completeness - Publisher, Version, InstallDate (15 pts)
            int metaScore = 0;
            if (!string.IsNullOrWhiteSpace(entry.Publisher)) metaScore += 5;
            if (!string.IsNullOrWhiteSpace(entry.DisplayVersion)) metaScore += 5;
            if (entry.InstallDate != default) metaScore += 5;
            score += metaScore;

            result.Score = Math.Min(100, Math.Max(0, score));

            if (result.Score >= 75)
                result.Level = DiscoveryConfidenceLevel.High;
            else if (result.Score >= 45)
                result.Level = DiscoveryConfidenceLevel.Medium;
            else if (result.Score >= 20)
                result.Level = DiscoveryConfidenceLevel.Low;
            else
                result.Level = DiscoveryConfidenceLevel.SuspiciousOrBroken;

            result.Summary = $"Score: {result.Score}/100 [{result.Level}]";
            return result;
        }
    }
}

/*
    EBUninstaller Pro - Software Safety Advisor Tests
    Unit tests for bloatware detection, PUP recognition, and safety score evaluation.
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools;
using UninstallTools.Core;

namespace BulkCrapUninstallerTests
{
    [TestClass]
    public class SoftwareSafetyAdvisorTests
    {
        [TestMethod]
        public void TestBloatwareDetection()
        {
            var bloatApp = new ApplicationUninstallerEntry
            {
                RawDisplayName = "Candy Crush Soda Saga Preinstalled",
                Publisher = "King.com"
            };

            var report = SoftwareSafetyAdvisor.AnalyzeApplication(bloatApp);
            Assert.IsNotNull(report);
            Assert.IsTrue(report.IsBloatware);
            Assert.AreEqual(SoftwareCategoryRating.OEMBloatware, report.Category);
            Assert.AreEqual(AdvisorRecommendation.RecommendedUninstall, report.Recommendation);
            Assert.IsTrue(report.SafetyScore < 50);
        }

        [TestMethod]
        public void TestTrustedPublisherClassification()
        {
            var cleanApp = new ApplicationUninstallerEntry
            {
                RawDisplayName = "Visual Studio Code",
                Publisher = "Microsoft Corporation"
            };

            var report = SoftwareSafetyAdvisor.AnalyzeApplication(cleanApp);
            Assert.IsNotNull(report);
            Assert.IsFalse(report.IsBloatware);
            Assert.AreEqual(SoftwareCategoryRating.VerifiedClean, report.Category);
            Assert.AreEqual(AdvisorRecommendation.Keep, report.Recommendation);
            Assert.IsTrue(report.SafetyScore >= 90);
        }

        [TestMethod]
        public void TestOrphanedApplicationRecommendation()
        {
            var orphanedApp = new ApplicationUninstallerEntry
            {
                RawDisplayName = "Damaged Tool",
                IsOrphaned = true
            };

            var report = SoftwareSafetyAdvisor.AnalyzeApplication(orphanedApp);
            Assert.IsNotNull(report);
            Assert.AreEqual(SoftwareCategoryRating.StubbornOrDamaged, report.Category);
            Assert.AreEqual(AdvisorRecommendation.ForcedUninstallRequired, report.Recommendation);
        }
    }
}

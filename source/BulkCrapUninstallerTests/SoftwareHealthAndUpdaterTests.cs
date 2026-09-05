/*
    EBUninstaller Pro - Software Health & Updater Tests
    Unit tests for software health metrics, hygiene score, and update check engine.
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools;
using UninstallTools.Core;
using UninstallTools.Detection;

namespace BulkCrapUninstallerTests
{
    [TestClass]
    public class SoftwareHealthAndUpdaterTests
    {
        [TestMethod]
        public void TestSoftwareHealthMetricsCalculation()
        {
            var apps = new List<ApplicationUninstallerEntry>
            {
                new() { RawDisplayName = "Good Application", Is64Bit = Klocman.Tools.YesNoMaybe.Yes, IsOrphaned = false, EstimatedSize = Klocman.IO.FileSize.FromMegabytes(200) },
                new() { RawDisplayName = "Damaged App", IsOrphaned = true, Is64Bit = Klocman.Tools.YesNoMaybe.No },
                new() { RawDisplayName = "Windows Component", IsProtected = true }
            };

            var report = SoftwareHealthEngine.AnalyzeHealth(apps);
            Assert.IsNotNull(report);
            Assert.AreEqual(3, report.TotalApplications);
            Assert.AreEqual(1, report.OrphanedApplications);
            Assert.IsTrue(report.HygieneScorePercentage >= 0 && report.HygieneScorePercentage <= 100);
            Assert.IsNotNull(report.Rating);
        }

        [TestMethod]
        public void TestUpdateManagerConfiguration()
        {
            Assert.IsNotNull(UpdateManager.CurrentVersion);
            Assert.AreEqual("7.0.0", UpdateManager.CurrentVersion);
            Assert.IsNotNull(UpdateManager.ReleaseApiEndpoint);
        }
    }
}

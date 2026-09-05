/*
    EBUninstaller Pro - Software Health, Update Manager & Registry Optimizer Test Suite
*/

using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UninstallTools;
using UninstallTools.Core;
using UninstallTools.Detection;
using UninstallTools.RegistryEngine;
using UninstallTools.WindowsIntegration;

namespace BulkCrapUninstallerTests
{
    [TestFixture]
    public class SoftwareHealthAndUpdaterTests
    {
        [Test]
        public void TestSoftwareHealthAnalysisWithDuplicates()
        {
            var testApps = new List<ApplicationUninstallerEntry>
            {
                new() { DisplayName = "Microsoft Visual C++ 2015-2022 Redistributable (x64)", EstimatedSize = new FileSize(50 * 1024 * 1024) },
                new() { DisplayName = "Microsoft Visual C++ 2013 Redistributable (x86)", EstimatedSize = new FileSize(40 * 1024 * 1024) },
                new() { DisplayName = "Microsoft Visual C++ 2012 Redistributable (x64)", EstimatedSize = new FileSize(30 * 1024 * 1024) },
                new() { DisplayName = "Microsoft Visual C++ 2010 Redistributable (x86)", EstimatedSize = new FileSize(20 * 1024 * 1024) },
                new() { DisplayName = "Microsoft Visual C++ 2008 Redistributable (x64)", EstimatedSize = new FileSize(15 * 1024 * 1024) },
                new() { DisplayName = "Java 8 Update 351", EstimatedSize = new FileSize(150 * 1024 * 1024) },
                new() { DisplayName = "Java 8 Update 281", EstimatedSize = new FileSize(150 * 1024 * 1024) },
                new() { DisplayName = "Massive Game Studio Pro", EstimatedSize = new FileSize(10L * 1024 * 1024 * 1024) } // 10 GB
            };

            var report = SoftwareHealthEngine.AnalyzeSystemHealth(testApps);

            Assert.IsNotNull(report);
            Assert.AreEqual(8, report.TotalAppsAnalyzed);
            Assert.IsTrue(report.DuplicateRuntimesCount > 0);
            Assert.AreEqual(1, report.LargeAppsCount);
            Assert.IsTrue(report.HygieneScore < 100);
            Assert.IsTrue(report.Recommendations.Count >= 2);
        }

        [Test]
        public void TestUpdateManagerVersionCheckAndChecksum()
        {
            Assert.AreEqual(new Version(7, 0, 0), UpdateManager.CurrentAssemblyVersion);
            Assert.AreEqual(UpdateChannel.Stable, UpdateManager.SelectedChannel);

            // Create temporary test file for checksum verification
            var tempFile = Path.Combine(Path.GetTempPath(), $"EBUpdate_{Guid.NewGuid():N}.bin");
            File.WriteAllBytes(tempFile, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 });

            var expectedHash = CryptoHasher.ComputeFileSha256(tempFile);
            Assert.IsTrue(UpdateManager.ValidateDownloadChecksum(tempFile, expectedHash));
            Assert.IsFalse(UpdateManager.ValidateDownloadChecksum(tempFile, "invalid_hash_value"));

            if (File.Exists(tempFile)) File.Delete(tempFile);
        }

        [Test]
        public void TestRegistryOptimizerSafeScanning()
        {
            var scan = RegistryOptimizerEngine.ScanRegistryIssues();
            Assert.IsNotNull(scan);
            Assert.IsNotNull(scan.Issues);

            // Test fix with empty list returns 0 safely
            var fixedCount = RegistryOptimizerEngine.FixRegistryIssues(new List<RegistryIssue>(), false);
            Assert.AreEqual(0, fixedCount);
        }

        [Test]
        public void TestShellIntegrationState()
        {
            // Context menu querying should not throw
            bool isInstalled = ShellIntegrationManager.IsContextMenuInstalled();
            Assert.IsNotNull(isInstalled);
        }
    }
}

/*
    EBUninstaller Pro - App Filter Engine and System Residuals Cleaner Test Suite
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UninstallTools;
using UninstallTools.Detection;
using UninstallTools.JunkCleaner;

namespace BulkCrapUninstallerTests
{
    [TestFixture]
    public class FilterAndResidualsTests
    {
        [Test]
        public void TestAppFilterEngineParsing()
        {
            var query = AppFilterEngine.ParseQuery("pub:Microsoft size:>100MB signed:true vlc");
            Assert.IsNotNull(query);
            Assert.AreEqual("Microsoft", query.PublisherFilter);
            Assert.AreEqual(100 * 1024 * 1024, query.MinSizeBytes);
            Assert.AreEqual(true, query.SignedOnly);
            Assert.Contains("vlc", query.Keywords);
        }

        [Test]
        public void TestAppFilterEngineMatching()
        {
            var apps = new List<ApplicationUninstallerEntry>
            {
                new() { DisplayName = "Microsoft Visual Studio", Publisher = "Microsoft Corporation", EstimatedSize = new FileSize(500 * 1024 * 1024) },
                new() { DisplayName = "VLC media player", Publisher = "VideoLAN", EstimatedSize = new FileSize(120 * 1024 * 1024) },
                new() { DisplayName = "7-Zip 22.01", Publisher = "Igor Pavlov", EstimatedSize = new FileSize(5 * 1024 * 1024) }
            };

            // Test publisher filter
            var msApps = AppFilterEngine.Filter(apps, "pub:Microsoft").ToList();
            Assert.AreEqual(1, msApps.Count);
            Assert.AreEqual("Microsoft Visual Studio", msApps[0].DisplayName);

            // Test size filter (>50MB)
            var largeApps = AppFilterEngine.Filter(apps, "size:>50MB").ToList();
            Assert.AreEqual(2, largeApps.Count);

            // Test keyword search
            var zipApps = AppFilterEngine.Filter(apps, "7-Zip").ToList();
            Assert.AreEqual(1, zipApps.Count);
        }

        [Test]
        public void TestSystemResidualsScanningSafety()
        {
            var residuals = DriverAndSystemResidualsCleaner.ScanSystemResiduals();
            Assert.IsNotNull(residuals);

            // Clean with empty list should return 0 safely
            var (count, freed) = DriverAndSystemResidualsCleaner.CleanResiduals(new List<SystemResidualItem>());
            Assert.AreEqual(0, count);
            Assert.AreEqual(0, freed);
        }
    }
}

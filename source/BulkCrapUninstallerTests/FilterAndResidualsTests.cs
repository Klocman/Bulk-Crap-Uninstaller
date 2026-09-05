/*
    EBUninstaller Pro - App Filters & Residuals Tests
    Unit tests for 1-click filter categorizer and driver residuals cleaner.
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools;
using UninstallTools.Detection;
using UninstallTools.JunkCleaner;

namespace BulkCrapUninstallerTests
{
    [TestClass]
    public class FilterAndResidualsTests
    {
        [TestMethod]
        public void TestAppFilterCategorization()
        {
            var apps = new List<ApplicationUninstallerEntry>
            {
                new() { RawDisplayName = "Standard Win32 App", UninstallerKind = UninstallerType.Nsis },
                new() { RawDisplayName = "Microsoft Store App", UninstallerKind = UninstallerType.StoreApp },
                new() { RawDisplayName = "Steam Game Title", UninstallerKind = UninstallerType.Steam },
                new() { RawDisplayName = "Portable Tool", IsOrphaned = true },
                new() { RawDisplayName = "Windows Update KB123456", IsUpdate = true },
                new() { RawDisplayName = "Huge Program", EstimatedSize = Klocman.IO.FileSize.FromMegabytes(1024) }
            };

            var win32Apps = AppFilterEngine.FilterApplications(apps, AppFilterCategory.Win32).ToList();
            Assert.IsTrue(win32Apps.Count >= 1);

            var storeApps = AppFilterEngine.FilterApplications(apps, AppFilterCategory.StoreApps).ToList();
            Assert.AreEqual(1, storeApps.Count);

            var games = AppFilterEngine.FilterApplications(apps, AppFilterCategory.Games).ToList();
            Assert.AreEqual(1, games.Count);

            var updates = AppFilterEngine.FilterApplications(apps, AppFilterCategory.Updates).ToList();
            Assert.AreEqual(1, updates.Count);

            var largeApps = AppFilterEngine.FilterApplications(apps, AppFilterCategory.LargeApps).ToList();
            Assert.AreEqual(1, largeApps.Count);
        }

        [TestMethod]
        public void TestDriverAndSystemResidualsScanner()
        {
            var results = DriverAndSystemResidualsCleaner.ScanResiduals();
            Assert.IsNotNull(results);

            foreach (var item in results)
            {
                Assert.IsNotNull(item.ResidualName);
                Assert.IsNotNull(item.LocationPath);
                Assert.IsNotNull(item.Kind);
            }
        }
    }
}

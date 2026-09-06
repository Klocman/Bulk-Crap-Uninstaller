/*
    EBUninstaller Pro - Package Managers Tests
    Unit tests for WinGet, Chocolatey, and Scoop package management and cache cleaner.
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.Detection;

namespace BulkCrapUninstallerTests
{
    [TestClass]
    public class PackageManagerTests
    {
        [TestMethod]
        public void TestManagedPackageItemProperties()
        {
            var item = new ManagedPackageItem
            {
                PackageId = "Git.Git",
                Name = "Git",
                InstalledVersion = "2.40.0",
                AvailableVersion = "2.44.0",
                Manager = PackageManagerType.WinGet,
                Source = "winget"
            };

            Assert.AreEqual("Git.Git", item.PackageId);
            Assert.AreEqual("Git", item.Name);
            Assert.AreEqual(PackageManagerType.WinGet, item.Manager);
            Assert.IsTrue(item.HasUpdate);
        }

        [TestMethod]
        public void TestPackageItemNoUpdate()
        {
            var item = new ManagedPackageItem
            {
                PackageId = "7zip.7zip",
                Name = "7-Zip",
                InstalledVersion = "23.01",
                AvailableVersion = "23.01",
                Manager = PackageManagerType.WinGet
            };

            Assert.IsFalse(item.HasUpdate);
        }

        [TestMethod]
        public void TestCleanPackageCachesReturnsValidStats()
        {
            var (freed, count) = PackageManagerUpdateEngine.CleanPackageCaches();
            Assert.IsTrue(freed >= 0);
            Assert.IsTrue(count >= 0);
        }
    }
}

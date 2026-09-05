/*
    EBUninstaller Pro - Developer Cache Cleaner Tests
    Unit tests for developer cache location models, sizing, and ecosystem mappings.
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.JunkCleaner;

namespace BulkCrapUninstallerTests
{
    [TestClass]
    public class DeveloperCacheCleanerTests
    {
        [TestMethod]
        public void TestDevCacheLocationItemProperties()
        {
            var item = new DevCacheLocationItem
            {
                EcosystemName = ".NET Global NuGet Packages",
                Ecosystem = DevToolEcosystem.DotNetNuGet,
                Description = "Global cached NuGet packages",
                DirectoryPath = @"C:\Users\Test\.nuget\packages",
                SizeBytes = 5368709120, // 5 GB
                FilesCount = 12450,
                Exists = true,
                IsSelected = true
            };

            Assert.AreEqual(".NET Global NuGet Packages", item.EcosystemName);
            Assert.AreEqual(DevToolEcosystem.DotNetNuGet, item.Ecosystem);
            Assert.AreEqual(5368709120, item.SizeBytes);
            Assert.AreEqual(12450, item.FilesCount);
            Assert.IsTrue(item.Exists);
            Assert.IsTrue(item.IsSelected);
        }

        [TestMethod]
        public void TestDevToolEcosystemValues()
        {
            Assert.IsTrue(Enum.IsDefined(typeof(DevToolEcosystem), DevToolEcosystem.DotNetNuGet));
            Assert.IsTrue(Enum.IsDefined(typeof(DevToolEcosystem), DevToolEcosystem.NodeNpmYarnPnpm));
            Assert.IsTrue(Enum.IsDefined(typeof(DevToolEcosystem), DevToolEcosystem.PythonPipConda));
            Assert.IsTrue(Enum.IsDefined(typeof(DevToolEcosystem), DevToolEcosystem.RustCargo));
            Assert.IsTrue(Enum.IsDefined(typeof(DevToolEcosystem), DevToolEcosystem.JavaGradleMaven));
            Assert.IsTrue(Enum.IsDefined(typeof(DevToolEcosystem), DevToolEcosystem.Golang));
        }
    }
}

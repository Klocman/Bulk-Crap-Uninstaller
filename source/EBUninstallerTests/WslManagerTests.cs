/*
    EBUninstaller Pro - WSL & Virtual Disk Manager Tests
    Unit tests for WSL distribution models, versioning, and virtual disk properties.
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.SystemTools;

namespace BulkCrapUninstallerTests
{
    [TestClass]
    public class WslManagerTests
    {
        [TestMethod]
        public void TestWslDistroItemProperties()
        {
            var item = new WslDistroItem
            {
                DistroGuid = "{12345678-ABCD-EF01-2345-6789ABCDEF01}",
                DistributionName = "Ubuntu-22.04",
                BasePath = @"C:\Users\Test\AppData\Local\Packages\CanonicalGroupLimited.Ubuntu22.04LTS_79rhkp1fndgsc\LocalState",
                VhdxPath = @"C:\Users\Test\AppData\Local\Packages\CanonicalGroupLimited.Ubuntu22.04LTS_79rhkp1fndgsc\LocalState\ext4.vhdx",
                WslVersion = 2,
                DiskSizeBytes = 21474836480, // 20 GB
                IsDefault = true,
                IsOrphanedDisk = false
            };

            Assert.AreEqual("Ubuntu-22.04", item.DistributionName);
            Assert.AreEqual(2, item.WslVersion);
            Assert.AreEqual(21474836480, item.DiskSizeBytes);
            Assert.IsTrue(item.IsDefault);
            Assert.IsFalse(item.IsOrphanedDisk);
        }
    }
}

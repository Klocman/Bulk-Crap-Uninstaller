/*
    EBUninstaller Pro - Windows Driver Manager Tests
    Unit tests for driver data structures, startup types, and properties.
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.SystemTools;

namespace BulkCrapUninstallerTests
{
    [TestClass]
    public class WindowsDriverManagerTests
    {
        [TestMethod]
        public void TestDriverInfoItemDefaults()
        {
            var driver = new DriverInfoItem
            {
                DriverName = "testdriver",
                DisplayName = "Test Kernel Driver",
                Description = "A test driver for verification",
                DriverPath = @"C:\Windows\System32\drivers\testdriver.sys",
                StartupType = DriverStartupType.Manual,
                State = DriverState.Stopped,
                Provider = "Test Vendor Corp.",
                IsMicrosoftDriver = false,
                IsOrphaned = false,
                FileSizeBytes = 102400
            };

            Assert.AreEqual("testdriver", driver.DriverName);
            Assert.AreEqual("Test Kernel Driver", driver.DisplayName);
            Assert.AreEqual("Test Vendor Corp.", driver.Provider);
            Assert.AreEqual(DriverStartupType.Manual, driver.StartupType);
            Assert.AreEqual(DriverState.Stopped, driver.State);
            Assert.IsFalse(driver.IsMicrosoftDriver);
            Assert.IsFalse(driver.IsOrphaned);
            Assert.AreEqual(102400, driver.FileSizeBytes);
        }

        [TestMethod]
        public void TestDriverStartupTypeValues()
        {
            Assert.AreEqual(0, (int)DriverStartupType.Boot);
            Assert.AreEqual(1, (int)DriverStartupType.System);
            Assert.AreEqual(2, (int)DriverStartupType.Automatic);
            Assert.AreEqual(3, (int)DriverStartupType.Manual);
            Assert.AreEqual(4, (int)DriverStartupType.Disabled);
        }
    }
}

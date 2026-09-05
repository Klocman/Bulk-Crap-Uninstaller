/*
    EBUninstaller Pro - Driver Residuals & Memory Trimmer Tests
    Unit tests for device driver residuals detection and working-set memory trimming.
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.JunkCleaner;
using UninstallTools.SystemTools;

namespace BulkCrapUninstallerTests
{
    [TestClass]
    public class DriverAndMemoryTests
    {
        [TestMethod]
        public void TestDriverResidualsScannerReturnsItems()
        {
            var residuals = DeviceDriverResidualsCleaner.ScanDriverResiduals();
            Assert.IsNotNull(residuals);

            foreach (var item in residuals)
            {
                Assert.IsNotNull(item.DriverName);
                Assert.IsNotNull(item.RegistryPath);
                Assert.IsNotNull(item.DeviceClass);
            }
        }

        [TestMethod]
        public void TestDriverResidualItemProperties()
        {
            var item = new DriverResidualItem
            {
                DriverName = "oem123.inf",
                RegistryPath = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\OldDriver",
                DeviceClass = "Net",
                Status = "Disconnected",
                HardwareId = @"PCI\VEN_8086&DEV_1234",
                EstimatedSize = 512000,
                IsSelected = true
            };

            Assert.AreEqual("oem123.inf", item.DriverName);
            Assert.AreEqual("Net", item.DeviceClass);
            Assert.IsTrue(item.IsSelected);
            Assert.AreEqual(512000, item.EstimatedSize);
        }

        [TestMethod]
        public void TestMemoryTrimmerExecution()
        {
            var result = MemoryTrimmerEngine.TrimProcessWorkingSets();
            Assert.IsNotNull(result);
            Assert.IsTrue(result.OptimizedProcesses >= 0);
            Assert.IsTrue(result.ReclaimedBytes >= 0);
        }
    }
}

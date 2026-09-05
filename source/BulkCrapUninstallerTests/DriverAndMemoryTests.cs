/*
    EBUninstaller Pro - Device Driver Residuals & Memory Trimmer Test Suite
*/

using System;
using System.Collections.Generic;
using NUnit.Framework;
using UninstallTools.JunkCleaner;
using UninstallTools.SystemTools;

namespace BulkCrapUninstallerTests
{
    [TestFixture]
    public class DriverAndMemoryTests
    {
        [Test]
        public void TestMemoryTrimmerExecution()
        {
            var res = MemoryTrimmerEngine.TrimSystemWorkingSet();
            Assert.IsNotNull(res);
            Assert.IsTrue(res.TotalProcessesInspected >= 0);
        }

        [Test]
        public void TestDriverResidualsScannerDoesNotThrow()
        {
            var drivers = DeviceDriverResidualsCleaner.ScanDriverResiduals();
            Assert.IsNotNull(drivers);
        }
    }
}

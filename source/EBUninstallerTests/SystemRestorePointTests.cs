/*
    EBUninstaller Pro - System Restore Point Tests
    Unit tests for System Restore data models, enum mappings, and restore point structures.
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.Backup;

namespace BulkCrapUninstallerTests
{
    [TestClass]
    public class SystemRestorePointTests
    {
        [TestMethod]
        public void TestSystemRestorePointItemProperties()
        {
            var item = new SystemRestorePointItem
            {
                SequenceNumber = 42,
                Description = "Pre-Uninstall Backup - Test Application",
                Type = RestorePointType.ApplicationUninstall,
                CreationTime = new DateTime(2026, 9, 5, 12, 0, 0, DateTimeKind.Utc),
                EventType = 100
            };

            Assert.AreEqual((uint)42, item.SequenceNumber);
            Assert.AreEqual("Pre-Uninstall Backup - Test Application", item.Description);
            Assert.AreEqual(RestorePointType.ApplicationUninstall, item.Type);
            Assert.AreEqual((uint)100, item.EventType);
        }

        [TestMethod]
        public void TestRestorePointTypeEnumValues()
        {
            Assert.AreEqual(0, (int)RestorePointType.ApplicationInstall);
            Assert.AreEqual(1, (int)RestorePointType.ApplicationUninstall);
            Assert.AreEqual(10, (int)RestorePointType.DeviceDriverInstall);
            Assert.AreEqual(16, (int)RestorePointType.ManualCheckpoint);
        }
    }
}

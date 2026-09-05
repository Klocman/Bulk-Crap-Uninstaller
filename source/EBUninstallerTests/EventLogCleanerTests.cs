/*
    EBUninstaller Pro - Event Log Residuals Cleaner Tests
    Unit tests for Windows Event Log enumeration, category detection, and safety protections.
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.JunkCleaner;

namespace BulkCrapUninstallerTests
{
    [TestClass]
    public class EventLogCleanerTests
    {
        [TestMethod]
        public void TestEventLogItemProperties()
        {
            var item = new EventLogItem
            {
                LogName = "Application",
                DisplayName = "Application Logs",
                Category = EventLogCategory.Application,
                RecordCount = 500,
                IsCriticalProtected = false,
                IsSelected = true
            };

            Assert.AreEqual("Application", item.LogName);
            Assert.AreEqual(EventLogCategory.Application, item.Category);
            Assert.AreEqual(500, item.RecordCount);
            Assert.IsFalse(item.IsCriticalProtected);
            Assert.IsTrue(item.IsSelected);
        }

        [TestMethod]
        public void TestSecurityLogIsProtected()
        {
            var secItem = new EventLogItem
            {
                LogName = "Security",
                DisplayName = "Security Auditing",
                Category = EventLogCategory.System,
                RecordCount = 10000,
                IsCriticalProtected = true,
                IsSelected = false
            };

            // Attempting to clear protected Security log must be skipped by safety filter
            var (cleared, records) = EventLogResidualsCleaner.ClearEventLogs(new[] { secItem });
            Assert.AreEqual(0, cleared, "Security event logs must never be purged automatically.");
            Assert.AreEqual(0, records);
        }
    }
}

/*
    EBUninstaller Pro - Environment Variables Tests
    Unit tests for PATH variable parsing, duplicate detection, and invalid directory checks.
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.SystemTools;

namespace BulkCrapUninstallerTests
{
    [TestClass]
    public class EnvironmentVariablesTests
    {
        [TestMethod]
        public void TestParsePathString()
        {
            string winDir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
            string fakePath = @"C:\NonExistentTestFolder123456789";
            string raw = $"{winDir};{fakePath};{winDir}"; // Has valid, invalid, and duplicate

            var entries = EnvironmentVariablesManager.ParsePathString(raw, false);

            Assert.AreEqual(3, entries.Count);
            Assert.IsTrue(entries[0].ExistsOnDisk);
            Assert.IsFalse(entries[0].IsDuplicate);

            Assert.IsFalse(entries[1].ExistsOnDisk);
            Assert.IsFalse(entries[1].IsDuplicate);

            Assert.IsTrue(entries[2].ExistsOnDisk);
            Assert.IsTrue(entries[2].IsDuplicate); // Duplicate of entries[0]
        }

        [TestMethod]
        public void TestEnvVarReportCalculations()
        {
            var report = new EnvVarReport();
            report.SystemPathEntries.Add(new PathEntryItem { ExistsOnDisk = true, IsDuplicate = false });
            report.SystemPathEntries.Add(new PathEntryItem { ExistsOnDisk = false, IsDuplicate = false });
            report.UserPathEntries.Add(new PathEntryItem { ExistsOnDisk = true, IsDuplicate = true });

            Assert.AreEqual(1, report.TotalInvalidEntries);
            Assert.AreEqual(1, report.TotalDuplicates);
        }
    }
}

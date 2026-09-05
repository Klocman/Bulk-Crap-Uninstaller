/*
    EBUninstaller Pro - Crash Dump Cleaner Tests
    Unit tests for memory dump identification and crash log purging.
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.JunkCleaner;

namespace BulkCrapUninstallerTests
{
    [TestClass]
    public class CrashDumpCleanerTests
    {
        private string _testDir = string.Empty;

        [TestInitialize]
        public void Setup()
        {
            _testDir = Path.Combine(Path.GetTempPath(), "EBUninstaller_DumpTest_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_testDir);
        }

        [TestCleanup]
        public void TearDown()
        {
            try
            {
                if (Directory.Exists(_testDir))
                    Directory.Delete(_testDir, true);
            }
            catch { }
        }

        [TestMethod]
        public void TestCrashDumpItemProperties()
        {
            var item = new CrashDumpItem
            {
                FileName = "chrome.exe.12345.dmp",
                FilePath = Path.Combine(_testDir, "chrome.exe.12345.dmp"),
                Kind = CrashDumpKind.UserModeCrashDump,
                SizeBytes = 204800,
                TargetProcess = "chrome.exe",
                IsSelected = true
            };

            Assert.AreEqual("chrome.exe.12345.dmp", item.FileName);
            Assert.AreEqual("chrome.exe", item.TargetProcess);
            Assert.AreEqual(CrashDumpKind.UserModeCrashDump, item.Kind);
            Assert.AreEqual(204800, item.SizeBytes);
            Assert.IsTrue(item.IsSelected);
        }

        [TestMethod]
        public void TestDeleteCrashDumpsOperation()
        {
            string fakeDump = Path.Combine(_testDir, "app.exe.dmp");
            File.WriteAllBytes(fakeDump, new byte[] { 0x4D, 0x44, 0x4D, 0x50, 0x00, 0x01 }); // MDMP mini-dump header

            var item = new CrashDumpItem
            {
                FileName = "app.exe.dmp",
                FilePath = fakeDump,
                Kind = CrashDumpKind.UserModeCrashDump,
                SizeBytes = 6,
                IsSelected = true
            };

            var (deletedCount, freedBytes) = CrashDumpCleaner.DeleteCrashDumps(new[] { item });
            Assert.AreEqual(1, deletedCount);
            Assert.AreEqual(6, freedBytes);
            Assert.IsFalse(File.Exists(fakeDump));
        }
    }
}

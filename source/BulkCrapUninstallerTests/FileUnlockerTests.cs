/*
    EBUninstaller Pro - File Unlocker Tests
    Unit tests for Restart Manager file lock inspection and process models.
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.FileSystemEngine;

namespace BulkCrapUninstallerTests
{
    [TestClass]
    public class FileUnlockerTests
    {
        private string _testFile = string.Empty;

        [TestInitialize]
        public void Setup()
        {
            _testFile = Path.Combine(Path.GetTempPath(), "EBUninstaller_UnlockTest_" + Guid.NewGuid().ToString("N") + ".tmp");
            File.WriteAllText(_testFile, "Lock test sample data");
        }

        [TestCleanup]
        public void TearDown()
        {
            try
            {
                if (File.Exists(_testFile))
                    File.Delete(_testFile);
            }
            catch { }
        }

        [TestMethod]
        public void TestLockProcessInfoModel()
        {
            var info = new LockProcessInfo
            {
                ProcessId = 1234,
                ProcessName = "notepad.exe",
                ApplicationName = "Notepad",
                MainModulePath = @"C:\Windows\notepad.exe",
                IsSystemProcess = false
            };

            Assert.AreEqual(1234, info.ProcessId);
            Assert.AreEqual("notepad.exe", info.ProcessName);
            Assert.AreEqual("Notepad", info.ApplicationName);
            Assert.IsFalse(info.IsSystemProcess);
        }

        [TestMethod]
        public void TestFindLockingProcessesOnUnlockedFile()
        {
            var locks = FileUnlockerManager.FindLockingProcesses(_testFile);
            Assert.IsNotNull(locks);
            Assert.AreEqual(0, locks.Count, "Newly created temporary file should have no locks.");
        }

        [TestMethod]
        public void TestProtectedProcessTerminationSafety()
        {
            // Terminating critical system process (like PID 0 or PID 4 or invalid) must fail safely
            bool result = FileUnlockerManager.TerminateLockingProcess(0);
            Assert.IsFalse(result);
        }
    }
}

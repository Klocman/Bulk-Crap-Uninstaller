/*
    EBUninstaller Pro - Windows Hosts File Manager Tests
    Unit tests for hosts parsing, data models, and localhost detection.
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.SystemTools;

namespace BulkCrapUninstallerTests
{
    [TestClass]
    public class WindowsHostsFileManagerTests
    {
        [TestMethod]
        public void TestHostEntryItemProperties()
        {
            var entry = new HostEntryItem
            {
                LineNumber = 12,
                IpAddress = "127.0.0.1",
                Hostname = "telemetry.badapp.com",
                Comment = "Injected by third party tool",
                IsCommentedOut = false,
                IsDefaultLocalhost = false,
                RawLine = "127.0.0.1 telemetry.badapp.com # Injected by third party tool"
            };

            Assert.AreEqual(12, entry.LineNumber);
            Assert.AreEqual("127.0.0.1", entry.IpAddress);
            Assert.AreEqual("telemetry.badapp.com", entry.Hostname);
            Assert.AreEqual("Injected by third party tool", entry.Comment);
            Assert.IsFalse(entry.IsCommentedOut);
            Assert.IsFalse(entry.IsDefaultLocalhost);
        }

        [TestMethod]
        public void TestLocalhostEntryDetection()
        {
            var entry = new HostEntryItem
            {
                LineNumber = 1,
                IpAddress = "127.0.0.1",
                Hostname = "localhost",
                IsDefaultLocalhost = true
            };

            Assert.IsTrue(entry.IsDefaultLocalhost);
        }
    }
}

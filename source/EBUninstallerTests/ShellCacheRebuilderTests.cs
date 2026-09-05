/*
    EBUninstaller Pro - Shell Cache Rebuilder Tests
    Unit tests for icon and thumbnail cache model properties and paths.
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.SystemTools;

namespace BulkCrapUninstallerTests
{
    [TestClass]
    public class ShellCacheRebuilderTests
    {
        [TestMethod]
        public void TestShellCacheItemProperties()
        {
            var item = new ShellCacheItem
            {
                CacheName = "Legacy Windows Icon Cache",
                Description = "Primary icon cache database",
                FilePath = @"C:\Users\Test\AppData\Local\IconCache.db",
                SizeBytes = 10485760, // 10 MB
                Exists = true
            };

            Assert.AreEqual("Legacy Windows Icon Cache", item.CacheName);
            Assert.AreEqual("Primary icon cache database", item.Description);
            Assert.AreEqual(10485760, item.SizeBytes);
            Assert.IsTrue(item.Exists);
        }
    }
}

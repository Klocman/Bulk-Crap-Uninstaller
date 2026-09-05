/*
    EBUninstaller Pro - Context Menu Manager Tests
    Unit tests for context menu shell handlers discovery and safety filters.
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.WindowsIntegration;

namespace BulkCrapUninstallerTests
{
    [TestClass]
    public class ContextMenuManagerTests
    {
        [TestMethod]
        public void TestGetContextMenuItemsStructure()
        {
            var items = ContextMenuManager.GetContextMenuItems();
            Assert.IsNotNull(items);

            foreach (var item in items)
            {
                Assert.IsNotNull(item.Name);
                Assert.IsNotNull(item.RegistryPath);
                Assert.IsNotNull(item.LocationType);
            }
        }

        [TestMethod]
        public void TestProtectedClsidSafetyFilter()
        {
            var testItem = new ContextMenuItem
            {
                Name = "Windows Defender Scan",
                Clsid = "{a2a9545d-a0c2-42b4-9708-a0b2badd77c8}",
                IsSystemCritical = true,
                IsEnabled = true
            };

            // Attempting to toggle or delete protected system-critical context menu should return false
            bool toggleResult = ContextMenuManager.ToggleItemStatus(testItem, false);
            Assert.IsFalse(toggleResult, "System-critical shell extensions must not be toggled.");

            bool deleteResult = ContextMenuManager.DeleteItem(testItem);
            Assert.IsFalse(deleteResult, "System-critical shell extensions must not be deleted.");
        }

        [TestMethod]
        public void TestClsidResolutionWithEmptyOrInvalidGuid()
        {
            var (path, publisher) = ContextMenuManager.ResolveClsid("");
            Assert.IsNull(path);
            Assert.IsNull(publisher);

            var (invalidPath, invalidPub) = ContextMenuManager.ResolveClsid("{00000000-0000-0000-0000-000000000000}");
            Assert.IsNull(invalidPath);
            Assert.IsNull(invalidPub);
        }
    }
}

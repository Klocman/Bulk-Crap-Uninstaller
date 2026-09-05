/*
    EBUninstaller Pro - Context Menu Manager Tests
    Unit tests for context menu shell handlers discovery and safety filters.
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Linq;
using NUnit.Framework;
using UninstallTools.WindowsIntegration;

namespace BulkCrapUninstallerTests
{
    [TestFixture]
    public class ContextMenuManagerTests
    {
        [Test]
        public void TestGetContextMenuItemsStructure()
        {
            var items = ContextMenuManager.GetContextMenuItems();
            Assert.That(items, Is.Not.Null);

            foreach (var item in items)
            {
                Assert.That(item.Name, Is.Not.Null);
                Assert.That(item.RegistryPath, Is.Not.Null);
                Assert.That(item.LocationType, Is.Not.Null);
            }
        }

        [Test]
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
            Assert.That(toggleResult, Is.False, "System-critical shell extensions must not be toggled.");

            bool deleteResult = ContextMenuManager.DeleteItem(testItem);
            Assert.That(deleteResult, Is.False, "System-critical shell extensions must not be deleted.");
        }

        [Test]
        public void TestClsidResolutionWithEmptyOrInvalidGuid()
        {
            var (path, publisher) = ContextMenuManager.ResolveClsid("");
            Assert.That(path, Is.Null);
            Assert.That(publisher, Is.Null);

            var (invalidPath, invalidPub) = ContextMenuManager.ResolveClsid("{00000000-0000-0000-0000-000000000000}");
            Assert.That(invalidPath, Is.Null);
            Assert.That(invalidPub, Is.Null);
        }
    }
}

/*
    EBUninstaller Pro - Wizard & Optimization Tests
    Unit tests for Quick Optimization Wizard steps and execution state.
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.JunkCleaner;

namespace BulkCrapUninstallerTests
{
    [TestClass]
    public class WizardAndOptimizationTests
    {
        [TestMethod]
        public void TestOptimizationTaskConfiguration()
        {
            var task = new JunkCleanupTask
            {
                Name = "Windows Temp Cleaner",
                Category = JunkCategory.WindowsTemp,
                EstimatedBytes = 1048576,
                IsSelected = true
            };

            Assert.AreEqual("Windows Temp Cleaner", task.Name);
            Assert.AreEqual(JunkCategory.WindowsTemp, task.Category);
            Assert.IsTrue(task.IsSelected);
            Assert.AreEqual(1048576, task.EstimatedBytes);
        }
    }
}

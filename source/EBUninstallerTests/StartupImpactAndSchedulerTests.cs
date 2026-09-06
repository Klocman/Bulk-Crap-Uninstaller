/*
    EBUninstaller Pro - Startup Impact Analyzer & Scheduler Tests
    Unit tests for boot impact calculation and maintenance task automation.
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.Startup;
using UninstallTools.SystemTools;

namespace BulkCrapUninstallerTests
{
    [TestClass]
    public class StartupImpactAndSchedulerTests
    {
        [TestMethod]
        public void TestStartupImpactCalculation()
        {
            var highImpact = StartupImpactAnalyzer.CalculateImpact("C:\\Windows\\System32\\HeavyApp.exe", true);
            Assert.IsNotNull(highImpact.Rating);
            Assert.IsTrue(highImpact.ImpactScore > 0);

            var lowImpact = StartupImpactAnalyzer.CalculateImpact("", false);
            Assert.AreEqual(StartupImpactRating.Low, lowImpact.Rating);
        }

        [TestMethod]
        public void TestMaintenanceSchedulerTaskName()
        {
            Assert.IsNotNull(AutoMaintenanceScheduler.TaskName);
            Assert.AreEqual("EBUninstaller_WeeklyMaintenance", AutoMaintenanceScheduler.TaskName);
        }
    }
}

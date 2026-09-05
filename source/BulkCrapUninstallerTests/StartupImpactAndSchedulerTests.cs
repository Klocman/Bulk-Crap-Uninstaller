/*
    EBUninstaller Pro - Startup Impact Analyzer & Task Scheduler Test Suite
*/

using System;
using System.Collections.Generic;
using NUnit.Framework;
using UninstallTools.Startup;
using UninstallTools.SystemTools;

namespace BulkCrapUninstallerTests
{
    [TestFixture]
    public class StartupImpactAndSchedulerTests
    {
        [Test]
        public void TestStartupImpactAnalysis()
        {
            var entries = new List<StartupEntry>
            {
                new() { ProgramName = "Discord", Command = @"C:\Users\Test\AppData\Local\Discord\app-1.0.9001\Discord.exe", Disabled = false },
                new() { ProgramName = "Spotify", Command = @"C:\Users\Test\AppData\Roaming\Spotify\Spotify.exe", Disabled = false },
                new() { ProgramName = "OneDrive", Command = @"C:\Users\Test\AppData\Local\Microsoft\OneDrive\OneDrive.exe /background", Disabled = false },
                new() { ProgramName = "Realtek HD Audio", Command = @"C:\Program Files\Realtek\Audio\HDA\RtkNGUI64.exe -s", Disabled = false },
                new() { ProgramName = "Disabled Old Tool", Command = @"C:\Tools\Old.exe", Disabled = true }
            };

            var report = StartupImpactAnalyzer.AnalyzeStartupItems(entries);

            Assert.IsNotNull(report);
            Assert.AreEqual(5, report.TotalStartupEntries);
            Assert.AreEqual(1, report.DisabledCount);
            Assert.IsTrue(report.HighImpactCount >= 2); // Discord + Spotify
            Assert.AreEqual(5, report.Recommendations.Count);
        }

        [Test]
        public void TestAutoMaintenanceSchedulerQueryDoesNotThrow()
        {
            // Querying should return boolean without uncaught exception
            var isScheduled = AutoMaintenanceScheduler.IsMaintenanceScheduled();
            Assert.IsNotNull(isScheduled);
        }
    }
}

/*
 * EBUninstaller Pro - Application Uninstaller & System Optimization Suite
 * Copyright (C) 2026 EBUninstaller Development Team & Contributors
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.Startup;

namespace EBUninstallerTests
{
    [TestClass]
    public class BootPerformanceAnalyzerTests
    {
        [TestMethod]
        public void TestQueryBootPerformanceReturnsReport()
        {
            var report = BootPerformanceAnalyzer.QueryBootPerformance();
            Assert.IsNotNull(report);
            Assert.IsNotNull(report.DegradedItems);
            Assert.IsNotNull(report.OptimizationTips);
        }

        [TestMethod]
        public void TestBootDegradationItemImpact()
        {
            var item = new BootDegradationItem
            {
                ApplicationName = "HeavyService.exe",
                DelayDurationMs = 6500,
                EventId = 101
            };

            Assert.AreEqual("High", item.ImpactLevel);
        }
    }
}

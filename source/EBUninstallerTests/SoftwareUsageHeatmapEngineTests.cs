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

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools;
using UninstallTools.Detection;

namespace EBUninstallerTests
{
    [TestClass]
    public class SoftwareUsageHeatmapEngineTests
    {
        [TestMethod]
        public void AnalyzeUsageHeatmap_NullInput_ReturnsEmptyList()
        {
            var results = SoftwareUsageHeatmapEngine.AnalyzeUsageHeatmap(null);
            Assert.IsNotNull(results);
            Assert.AreEqual(0, results.Count);
        }

        [TestMethod]
        public void AnalyzeUsageHeatmap_MultipleApps_CalculatesScores()
        {
            var apps = new List<ApplicationUninstallerEntry>
            {
                new ApplicationUninstallerEntry
                {
                    DisplayName = "Active Utility App",
                    Publisher = "Vendor Corp"
                },
                new ApplicationUninstallerEntry
                {
                    DisplayName = "Dormant Game App",
                    Publisher = "Game Studio"
                }
            };

            var results = SoftwareUsageHeatmapEngine.AnalyzeUsageHeatmap(apps);
            Assert.AreEqual(2, results.Count);
            Assert.IsTrue(results[0].ReclaimPriorityScore >= 0 && results[0].ReclaimPriorityScore <= 100);
        }
    }
}

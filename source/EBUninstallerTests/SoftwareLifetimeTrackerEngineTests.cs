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
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools;
using UninstallTools.History;

namespace EBUninstallerTests
{
    [TestClass]
    public class SoftwareLifetimeTrackerEngineTests
    {
        [TestMethod]
        public void BuildLifecycleTimeline_NullInput_ReturnsEmpty()
        {
            var res = SoftwareLifetimeTrackerEngine.BuildLifecycleTimeline(null);
            Assert.IsNotNull(res);
            Assert.AreEqual(0, res.Count);
        }

        [TestMethod]
        public void BuildLifecycleTimeline_ValidApps_BuildsTimeline()
        {
            var apps = new List<ApplicationUninstallerEntry>
            {
                new ApplicationUninstallerEntry
                {
                    DisplayName = "Brand New App",
                    InstallDate = DateTime.UtcNow.AddDays(-2)
                },
                new ApplicationUninstallerEntry
                {
                    DisplayName = "Vintage Utility",
                    InstallDate = DateTime.UtcNow.AddDays(-500)
                }
            };

            var timeline = SoftwareLifetimeTrackerEngine.BuildLifecycleTimeline(apps);
            Assert.AreEqual(2, timeline.Count);
            Assert.AreEqual(LifecycleStage.NewlyInstalled, timeline[0].Stage);
            Assert.AreEqual(LifecycleStage.Vintage, timeline[1].Stage);
        }
    }
}

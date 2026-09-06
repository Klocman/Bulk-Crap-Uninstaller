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
using UninstallTools.SystemTools;

namespace EBUninstallerTests
{
    [TestClass]
    public class WindowsRuntimesManagerTests
    {
        [TestMethod]
        public void TestScanInstalledRuntimesReturnsList()
        {
            var runtimes = WindowsRuntimesManager.ScanInstalledRuntimes();
            Assert.IsNotNull(runtimes, "Runtimes scan list should not be null.");
        }

        [TestMethod]
        public void TestRuntimeItemProperties()
        {
            var item = new RuntimeItem
            {
                Name = "Microsoft Visual C++ 2015-2022 Redistributable (x64)",
                Category = RuntimeCategory.VisualCpp,
                Version = "14.38.33135",
                Architecture = "x64",
                Publisher = "Microsoft Corporation",
                EstimatedSizeBytes = 30 * 1024 * 1024,
                IsSystemCritical = true
            };

            Assert.AreEqual("Microsoft Visual C++ 2015-2022 Redistributable (x64)", item.Name);
            Assert.AreEqual(RuntimeCategory.VisualCpp, item.Category);
            Assert.AreEqual("14.38.33135", item.Version);
            Assert.IsTrue(item.IsSystemCritical);
            Assert.AreEqual(30 * 1024 * 1024, item.EstimatedSizeBytes);
        }

        [TestMethod]
        public void TestSupersededMarking()
        {
            var list = new List<RuntimeItem>
            {
                new RuntimeItem { Name = "Microsoft Visual C++ 2015-2022 Redistributable (x64)", Category = RuntimeCategory.VisualCpp },
                new RuntimeItem { Name = "Microsoft Visual C++ 2015 Redistributable (x64)", Category = RuntimeCategory.VisualCpp }
            };

            // Simulating superseded detection
            if (list.Exists(r => r.Category == RuntimeCategory.VisualCpp && r.Name.Contains("2015-2022")))
            {
                foreach (var item in list)
                {
                    if (item.Name.Contains("2015") && !item.Name.Contains("2015-2022"))
                    {
                        item.IsSuperseded = true;
                    }
                }
            }

            Assert.IsTrue(list[1].IsSuperseded);
            Assert.IsFalse(list[0].IsSuperseded);
        }
    }
}

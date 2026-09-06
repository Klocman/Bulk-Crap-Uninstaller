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
using UninstallTools.Detection;

namespace EBUninstallerTests
{
    [TestClass]
    public class PackageManagerSyncEngineTests
    {
        [TestMethod]
        public void TestParseWingetUpgradeOutput()
        {
            var sampleOutput = @"
Name                   Id                   Version       Available     Source
-------------------------------------------------------------------------------
Mozilla Firefox        Mozilla.Firefox      120.0         121.0         winget
Google Chrome          Google.Chrome        119.0.6045    120.0.6099    winget
";

            var items = PackageManagerSyncEngine.ParseWingetUpgradeOutput(sampleOutput);
            Assert.AreEqual(2, items.Count);
            Assert.AreEqual("Mozilla Firefox", items[0].DisplayName);
            Assert.AreEqual("Mozilla.Firefox", items[0].PackageId);
            Assert.AreEqual("120.0", items[0].InstalledVersion);
            Assert.AreEqual("121.0", items[0].AvailableVersion);
            Assert.IsTrue(items[0].CanUpgrade);
        }

        [TestMethod]
        public void TestGenerateWingetExportJson()
        {
            var apps = new List<SyncAppItem>
            {
                new SyncAppItem
                {
                    DisplayName = "Mozilla Firefox",
                    PackageId = "Mozilla.Firefox",
                    ManagerType = SupportedPackageManager.Winget
                }
            };

            var json = PackageManagerSyncEngine.GenerateWingetExportJson(apps);
            Assert.IsTrue(json.Contains("Mozilla.Firefox"));
            Assert.IsTrue(json.Contains("WinGetVersion"));
        }

        [TestMethod]
        public void TestGeneratePowerShellReinstallScript()
        {
            var apps = new List<SyncAppItem>
            {
                new SyncAppItem
                {
                    DisplayName = "Mozilla Firefox",
                    PackageId = "Mozilla.Firefox",
                    ManagerType = SupportedPackageManager.Winget
                },
                new SyncAppItem
                {
                    DisplayName = "7-Zip",
                    PackageId = "7zip",
                    ManagerType = SupportedPackageManager.Chocolatey
                }
            };

            var ps1 = PackageManagerSyncEngine.GeneratePowerShellReinstallScript(apps);
            Assert.IsTrue(ps1.Contains("winget install --id \"Mozilla.Firefox\""));
            Assert.IsTrue(ps1.Contains("choco install \"7zip\""));
        }
    }
}

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
using UninstallTools.RegistryEngine;

namespace EBUninstallerTests
{
    [TestClass]
    public class RegistryBloatAnalyzerTests
    {
        [TestMethod]
        public void TestScanAllBloatReturnsValidResult()
        {
            var res = RegistryBloatAnalyzer.ScanAllBloat();
            Assert.IsNotNull(res, "Registry bloat result should not be null.");
            Assert.IsNotNull(res.Items, "Items list should not be null.");
            Assert.IsTrue(res.Duration.TotalMilliseconds >= 0);
        }

        [TestMethod]
        public void TestRegistryBloatItemProperties()
        {
            var item = new RegistryBloatItem
            {
                Category = RegistryBloatCategory.StaleAppPath,
                RootKeyName = "HKEY_LOCAL_MACHINE",
                SubKeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\missing.exe",
                TargetPath = @"C:\NonExistent\missing.exe",
                Reason = "Target executable file no longer exists.",
                IsSelected = true
            };

            Assert.AreEqual(RegistryBloatCategory.StaleAppPath, item.Category);
            Assert.AreEqual(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\missing.exe", item.FullRegistryPath);
            Assert.AreEqual("Target executable file no longer exists.", item.Reason);
            Assert.IsTrue(item.IsSelected);
        }

        [TestMethod]
        public void TestRegistryBloatScanResultCounts()
        {
            var res = new RegistryBloatScanResult();
            res.Items.Add(new RegistryBloatItem { Category = RegistryBloatCategory.OrphanedClsid });
            res.Items.Add(new RegistryBloatItem { Category = RegistryBloatCategory.StaleAppPath });
            res.Items.Add(new RegistryBloatItem { Category = RegistryBloatCategory.InvalidSharedDll });

            Assert.AreEqual(3, res.TotalCount);
            Assert.AreEqual(1, res.OrphanedClsidsCount);
            Assert.AreEqual(1, res.StaleAppPathsCount);
            Assert.AreEqual(1, res.InvalidSharedDllsCount);
        }
    }
}

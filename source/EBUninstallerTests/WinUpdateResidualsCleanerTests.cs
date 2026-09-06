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
using UninstallTools.JunkCleaner;

namespace EBUninstallerTests
{
    [TestClass]
    public class WinUpdateResidualsCleanerTests
    {
        [TestMethod]
        public void TestScanResidualsReturnsList()
        {
            var residuals = WinUpdateResidualsCleaner.ScanResiduals();
            Assert.IsNotNull(residuals, "ScanResiduals must not return null.");
        }

        [TestMethod]
        public void TestWinUpdateResidualItemProperties()
        {
            var item = new WinUpdateResidualItem
            {
                Title = "SoftwareDistribution Download Cache",
                Description = "Staged update files",
                TargetDirectoryPath = @"C:\Windows\SoftwareDistribution\Download",
                FileCount = 15,
                TotalSizeBytes = 100 * 1024 * 1024,
                IsSelected = true,
                RequiresAdmin = true
            };

            Assert.AreEqual("SoftwareDistribution Download Cache", item.Title);
            Assert.AreEqual(15, item.FileCount);
            Assert.AreEqual(100 * 1024 * 1024, item.TotalSizeBytes);
            Assert.IsTrue(item.IsSelected);
            Assert.IsTrue(item.RequiresAdmin);
        }

        [TestMethod]
        public void TestGetDismComponentCleanupStartInfo()
        {
            var psi = WinUpdateResidualsCleaner.GetDismComponentCleanupStartInfo(false);
            Assert.IsNotNull(psi);
            Assert.IsTrue(psi.Arguments.Contains("/StartComponentCleanup"));
            Assert.AreEqual("runas", psi.Verb);

            var psiResetBase = WinUpdateResidualsCleaner.GetDismComponentCleanupStartInfo(true);
            Assert.IsTrue(psiResetBase.Arguments.Contains("/ResetBase"));
        }
    }
}

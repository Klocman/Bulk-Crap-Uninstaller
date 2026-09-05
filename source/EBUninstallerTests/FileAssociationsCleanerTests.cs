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
    public class FileAssociationsCleanerTests
    {
        [TestMethod]
        public void TestScanFileAssociationsReturnsList()
        {
            var list = FileAssociationsCleaner.ScanFileAssociations();
            Assert.IsNotNull(list);
        }

        [TestMethod]
        public void TestFileAssociationItemProperties()
        {
            var item = new FileAssociationItem
            {
                Extension = ".testext",
                ProgId = "TestApp.Document",
                TargetExecutablePath = @"C:\Program Files\TestApp\app.exe",
                IsOrphaned = true,
                IsSelected = true
            };

            Assert.AreEqual(".testext", item.Extension);
            Assert.AreEqual("TestApp.Document", item.ProgId);
            Assert.IsTrue(item.IsOrphaned);
            Assert.IsTrue(item.IsSelected);
        }
    }
}

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
using UninstallTools.SystemTools;

namespace EBUninstallerTests
{
    [TestClass]
    public class InstalledFontsCleanerTests
    {
        [TestMethod]
        public void TestScanInstalledFontsReturnsList()
        {
            var list = InstalledFontsCleaner.ScanInstalledFonts();
            Assert.IsNotNull(list);
        }

        [TestMethod]
        public void TestInstalledFontItemProperties()
        {
            var font = new InstalledFontItem
            {
                FontName = "Custom Test Font (TrueType)",
                FileName = "custom_test.ttf",
                FullFontPath = @"C:\Windows\Fonts\custom_test.ttf",
                RegistryRoot = "HKLM",
                IsOrphaned = true,
                IsSystemDefault = false
            };

            Assert.AreEqual("Custom Test Font (TrueType)", font.FontName);
            Assert.AreEqual("custom_test.ttf", font.FileName);
            Assert.IsTrue(font.IsOrphaned);
            Assert.IsFalse(font.IsSystemDefault);
        }
    }
}

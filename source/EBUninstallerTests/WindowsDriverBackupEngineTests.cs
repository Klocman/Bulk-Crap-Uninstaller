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
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.SystemTools;

namespace EBUninstallerTests
{
    [TestClass]
    public class WindowsDriverBackupEngineTests
    {
        [TestMethod]
        public void TestParsePnpUtilOutput()
        {
            var sampleOutput = @"
Published Name:     oem10.inf
Original Name:      nv_dispi.inf
Provider Name:      NVIDIA
Class Name:         Display
Class GUID:         {4d36e968-e325-11ce-bfc1-08002be10318}
Driver Version:     31.0.15.4633
Driver Date:        12/06/2023
Signer Name:        Microsoft Windows Hardware Compatibility Publisher

Published Name:     oem22.inf
Original Name:      rt640x64.inf
Provider Name:      Realtek
Class Name:         Net
Class GUID:         {4d36e972-e325-11ce-bfc1-08002be10318}
Driver Version:     10.68.815.2023
Driver Date:        08/15/2023
Signer Name:        Microsoft Windows Hardware Compatibility Publisher
";

            var drivers = WindowsDriverBackupEngine.ParsePnpUtilOutput(sampleOutput);

            Assert.AreEqual(2, drivers.Count, "Should parse exactly 2 driver entries.");
            Assert.AreEqual("oem10.inf", drivers[0].PublishedName);
            Assert.AreEqual("nv_dispi.inf", drivers[0].OriginalFileName);
            Assert.AreEqual("NVIDIA", drivers[0].ProviderName);
            Assert.AreEqual("Display", drivers[0].ClassName);
            Assert.AreEqual("31.0.15.4633", drivers[0].DriverVersion);

            Assert.AreEqual("oem22.inf", drivers[1].PublishedName);
            Assert.AreEqual("Realtek", drivers[1].ProviderName);
            Assert.AreEqual("Net", drivers[1].ClassName);
        }

        [TestMethod]
        public void TestParsePnpUtilOutputEmpty()
        {
            var drivers = WindowsDriverBackupEngine.ParsePnpUtilOutput(string.Empty);
            Assert.AreEqual(0, drivers.Count);
        }

        [TestMethod]
        public void TestDriverExportSecurityDenylist()
        {
            // Should refuse to export drivers directly into System32
            var sys32 = Environment.SystemDirectory;
            var res = WindowsDriverBackupEngine.ExportDrivers(sys32);
            Assert.IsFalse(res.Success);
            Assert.IsTrue(res.ErrorMessage.Contains("protected", StringComparison.OrdinalIgnoreCase));
        }
    }
}

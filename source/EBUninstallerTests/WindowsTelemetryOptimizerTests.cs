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
using UninstallTools.PrivacyCleaner;

namespace EBUninstallerTests
{
    [TestClass]
    public class WindowsTelemetryOptimizerTests
    {
        [TestMethod]
        public void TestScanTelemetrySettingsReturnsSettings()
        {
            var settings = WindowsTelemetryOptimizer.ScanTelemetrySettings();
            Assert.IsNotNull(settings);
            Assert.IsTrue(settings.Count > 0, "Should have predefined privacy & telemetry templates.");
        }

        [TestMethod]
        public void TestTelemetrySettingItemProperties()
        {
            var item = new TelemetrySettingItem
            {
                Name = "Windows Diagnostic Telemetry Level",
                Category = TelemetryCategory.DiagnosticData,
                Description = "Restricts telemetry to basic tier.",
                RegistryRoot = "HKLM",
                SubKeyPath = @"SOFTWARE\Policies\Microsoft\Windows\DataCollection",
                ValueName = "AllowTelemetry",
                OptimizedValue = 0,
                DefaultValue = 3,
                IsOptimized = false,
                IsSelected = true
            };

            Assert.AreEqual("Windows Diagnostic Telemetry Level", item.Name);
            Assert.AreEqual(TelemetryCategory.DiagnosticData, item.Category);
            Assert.AreEqual(0, item.OptimizedValue);
            Assert.IsFalse(item.IsOptimized);
            Assert.IsTrue(item.IsSelected);
        }
    }
}

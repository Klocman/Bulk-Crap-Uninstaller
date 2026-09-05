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
using UninstallTools.Reporting;

namespace EBUninstallerTests
{
    [TestClass]
    public class SoftwareInventoryReportGeneratorTests
    {
        private List<ReportSoftwareItem> GetSampleItems()
        {
            return new List<ReportSoftwareItem>
            {
                new ReportSoftwareItem
                {
                    DisplayName = "Mozilla Firefox (x64 en-US)",
                    DisplayVersion = "120.0",
                    Publisher = "Mozilla",
                    InstallDate = "2024-01-15",
                    EstimatedSizeBytes = 250 * 1024 * 1024,
                    Architecture = "x64",
                    IsValidSigned = true,
                    SafetyScore = "Safe"
                },
                new ReportSoftwareItem
                {
                    DisplayName = "Visual Studio Code",
                    DisplayVersion = "1.85.1",
                    Publisher = "Microsoft Corporation",
                    InstallDate = "2024-02-01",
                    EstimatedSizeBytes = 350 * 1024 * 1024,
                    Architecture = "x64",
                    IsValidSigned = true,
                    SafetyScore = "Safe"
                }
            };
        }

        [TestMethod]
        public void TestGenerateHtmlReport()
        {
            var items = GetSampleItems();
            var html = SoftwareInventoryReportGenerator.GenerateHtmlReport(items);

            Assert.IsNotNull(html);
            Assert.IsTrue(html.Contains("<!DOCTYPE html>"));
            Assert.IsTrue(html.Contains("Mozilla Firefox"));
            Assert.IsTrue(html.Contains("Visual Studio Code"));
            Assert.IsTrue(html.Contains("EBUninstaller Pro"));
        }

        [TestMethod]
        public void TestGenerateMarkdownReport()
        {
            var items = GetSampleItems();
            var md = SoftwareInventoryReportGenerator.GenerateMarkdownReport(items);

            Assert.IsNotNull(md);
            Assert.IsTrue(md.Contains("# EBUninstaller Pro - Software Inventory Audit Report"));
            Assert.IsTrue(md.Contains("| Mozilla Firefox (x64 en-US) |"));
        }

        [TestMethod]
        public void TestGenerateCsvReport()
        {
            var items = GetSampleItems();
            var csv = SoftwareInventoryReportGenerator.GenerateCsvReport(items);

            Assert.IsNotNull(csv);
            Assert.IsTrue(csv.Contains("DisplayName,DisplayVersion,Publisher"));
            Assert.IsTrue(csv.Contains("\"Mozilla Firefox (x64 en-US)\""));
        }

        [TestMethod]
        public void TestGenerateJsonReport()
        {
            var items = GetSampleItems();
            var json = SoftwareInventoryReportGenerator.GenerateJsonReport(items);

            Assert.IsNotNull(json);
            Assert.IsTrue(json.Contains("\"Application\": \"EBUninstaller Pro\""));
            Assert.IsTrue(json.Contains("\"Mozilla Firefox (x64 en-US)\""));
        }
    }
}

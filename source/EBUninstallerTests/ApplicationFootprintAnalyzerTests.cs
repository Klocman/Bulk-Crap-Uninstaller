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
using UninstallTools.FileSystemEngine;

namespace EBUninstallerTests
{
    [TestClass]
    public class ApplicationFootprintAnalyzerTests
    {
        [TestMethod]
        public void TestAnalyzeFootprintReturnsReport()
        {
            var report = ApplicationFootprintAnalyzer.AnalyzeFootprint("TestApplication", @"C:\NonExistent\TestApplication", "TestPublisher");
            Assert.IsNotNull(report);
            Assert.AreEqual("TestApplication", report.ApplicationName);
            Assert.IsNotNull(report.Locations);
            Assert.IsNotNull(report.TopLargestFiles);
        }

        [TestMethod]
        public void TestFootprintLocationProperties()
        {
            var loc = new FootprintLocation
            {
                LocationType = "Main Installation Folder",
                PathOrKey = @"C:\Program Files\App",
                SizeBytes = 1024 * 1024 * 50,
                ItemCount = 120,
                Exists = true
            };

            Assert.AreEqual("Main Installation Folder", loc.LocationType);
            Assert.AreEqual(1024 * 1024 * 50, loc.SizeBytes);
            Assert.AreEqual(120, loc.ItemCount);
            Assert.IsTrue(loc.Exists);
        }
    }
}

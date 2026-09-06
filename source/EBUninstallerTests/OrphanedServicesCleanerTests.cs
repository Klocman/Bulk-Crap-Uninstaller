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
    public class OrphanedServicesCleanerTests
    {
        [TestMethod]
        public void TestExtractExecutablePathQuoted()
        {
            var raw = "\"C:\\Program Files\\My Company\\App\\service.exe\" /run -arg";
            var exe = OrphanedServicesCleaner.ExtractExecutablePath(raw);
            Assert.AreEqual(@"C:\Program Files\My Company\App\service.exe", exe);
        }

        [TestMethod]
        public void TestExtractExecutablePathUnquoted()
        {
            var raw = @"C:\Program Files\TestApp\daemon.exe -k start";
            var exe = OrphanedServicesCleaner.ExtractExecutablePath(raw);
            Assert.AreEqual(@"C:\Program Files\TestApp\daemon.exe", exe);
        }

        [TestMethod]
        public void TestExtractExecutablePathSvchost()
        {
            var raw = @"%SystemRoot%\system32\svchost.exe -k LocalServiceNetworkRestricted -p";
            var exe = OrphanedServicesCleaner.ExtractExecutablePath(raw);
            Assert.IsTrue(exe.EndsWith("svchost.exe", StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        public void TestScanOrphanedServicesNotNull()
        {
            var list = OrphanedServicesCleaner.ScanOrphanedServices();
            Assert.IsNotNull(list, "Orphaned services list should not be null.");
        }

        [TestMethod]
        public void TestOrphanedServiceItemModel()
        {
            var item = new OrphanedServiceItem
            {
                ServiceName = "OldAbandonedService",
                DisplayName = "Old Abandoned Service",
                ImagePath = @"C:\DeletedApp\service.exe",
                ParsedExecutablePath = @"C:\DeletedApp\service.exe",
                StartType = 2,
                IsOrphaned = true,
                IsProtected = false
            };

            Assert.AreEqual("OldAbandonedService", item.ServiceName);
            Assert.AreEqual("Automatic", item.StartTypeName);
            Assert.IsTrue(item.IsOrphaned);
            Assert.IsFalse(item.IsProtected);
        }
    }
}

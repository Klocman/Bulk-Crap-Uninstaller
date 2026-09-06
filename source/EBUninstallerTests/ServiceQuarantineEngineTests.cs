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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.Startup;

namespace EBUninstallerTests
{
    [TestClass]
    public class ServiceQuarantineEngineTests
    {
        [TestMethod]
        public void QuarantineService_NullOrEmpty_ReturnsFalse()
        {
            bool res1 = ServiceQuarantineEngine.QuarantineService(null);
            Assert.IsFalse(res1);

            bool res2 = ServiceQuarantineEngine.QuarantineService("");
            Assert.IsFalse(res2);
        }

        [TestMethod]
        public void ListQuarantinedServices_ReturnsNonNullList()
        {
            var list = ServiceQuarantineEngine.ListQuarantinedServices();
            Assert.IsNotNull(list);
        }

        [TestMethod]
        public void RestoreService_NonExistent_ReturnsFalse()
        {
            bool res = ServiceQuarantineEngine.RestoreService("NonExistentTestService12345");
            Assert.IsFalse(res);
        }

        [TestMethod]
        public void DeleteQuarantineRecord_NonExistent_ReturnsFalse()
        {
            bool res = ServiceQuarantineEngine.DeleteQuarantineRecord("NonExistentTestService12345");
            Assert.IsFalse(res);
        }
    }
}

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

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.RegistryEngine;

namespace EBUninstallerTests
{
    [TestClass]
    public class SharedDllAuditorEngineTests
    {
        [TestMethod]
        public void ScanSharedDlls_ReturnsNonNullList()
        {
            var list = SharedDllAuditorEngine.ScanSharedDlls();
            Assert.IsNotNull(list);
        }

        [TestMethod]
        public void CleanOrphanedSharedDlls_NullOrEmptyList_ReturnsZero()
        {
            int cleaned1 = SharedDllAuditorEngine.CleanOrphanedSharedDlls(null);
            Assert.AreEqual(0, cleaned1);

            int cleaned2 = SharedDllAuditorEngine.CleanOrphanedSharedDlls(new List<SharedDllRecord>());
            Assert.AreEqual(0, cleaned2);
        }
    }
}

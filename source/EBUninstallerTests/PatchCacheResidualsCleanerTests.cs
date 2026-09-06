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
using UninstallTools.JunkCleaner;

namespace EBUninstallerTests
{
    [TestClass]
    public class PatchCacheResidualsCleanerTests
    {
        [TestMethod]
        public void ScanPatchCache_ReturnsNonNullList()
        {
            var list = PatchCacheResidualsCleaner.ScanPatchCache();
            Assert.IsNotNull(list);
        }

        [TestMethod]
        public void CleanOrphanedPatches_NullOrEmptyList_ReturnsZero()
        {
            int res1 = PatchCacheResidualsCleaner.CleanOrphanedPatches(null);
            Assert.AreEqual(0, res1);

            int res2 = PatchCacheResidualsCleaner.CleanOrphanedPatches(new List<PatchCacheItem>());
            Assert.AreEqual(0, res2);
        }
    }
}

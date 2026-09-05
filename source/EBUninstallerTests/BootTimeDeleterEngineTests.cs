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
using UninstallTools.FileSystemEngine;

namespace EBUninstallerTests
{
    [TestClass]
    public class BootTimeDeleterEngineTests
    {
        [TestMethod]
        public void ScheduleFileForBootDeletion_NullOrEmpty_ReturnsFalse()
        {
            bool result = BootTimeDeleterEngine.ScheduleFileForBootDeletion(null);
            Assert.IsFalse(result);

            bool result2 = BootTimeDeleterEngine.ScheduleFileForBootDeletion("");
            Assert.IsFalse(result2);
        }

        [TestMethod]
        public void ScheduleFileForBootDeletion_ProtectedPath_ReturnsFalse()
        {
            bool result = BootTimeDeleterEngine.ScheduleFileForBootDeletion(@"C:\Windows\System32\ntoskrnl.exe");
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void GetPendingBootDeletions_ReturnsNonNullList()
        {
            var list = BootTimeDeleterEngine.GetPendingBootDeletions();
            Assert.IsNotNull(list);
        }

        [TestMethod]
        public void CancelBootDeletion_NullPath_ReturnsFalse()
        {
            bool result = BootTimeDeleterEngine.CancelBootDeletion(null);
            Assert.IsFalse(result);
        }
    }
}

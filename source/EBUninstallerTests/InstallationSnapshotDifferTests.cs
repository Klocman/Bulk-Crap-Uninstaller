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
using UninstallTools.InstallationMonitor;

namespace EBUninstallerTests
{
    [TestClass]
    public class InstallationSnapshotDifferTests
    {
        [TestMethod]
        public void TestCompareSnapshotsCalculatesDelta()
        {
            var before = new SystemSnapshot
            {
                SnapshotName = "Before"
            };
            before.Files[@"C:\Program Files\Existing\file.dll"] = new SnapshotFileEntry
            {
                Path = @"C:\Program Files\Existing\file.dll",
                Size = 1000,
                LastModifiedUtc = DateTime.UtcNow.AddDays(-5)
            };
            before.RegistryKeys.Add(@"HKEY_LOCAL_MACHINE\SOFTWARE\ExistingApp");

            var after = new SystemSnapshot
            {
                SnapshotName = "After"
            };
            after.Files[@"C:\Program Files\Existing\file.dll"] = new SnapshotFileEntry
            {
                Path = @"C:\Program Files\Existing\file.dll",
                Size = 1000,
                LastModifiedUtc = DateTime.UtcNow.AddDays(-5)
            };
            after.Files[@"C:\Program Files\NewApp\new.exe"] = new SnapshotFileEntry
            {
                Path = @"C:\Program Files\NewApp\new.exe",
                Size = 5000,
                LastModifiedUtc = DateTime.UtcNow
            };
            after.RegistryKeys.Add(@"HKEY_LOCAL_MACHINE\SOFTWARE\ExistingApp");
            after.RegistryKeys.Add(@"HKEY_LOCAL_MACHINE\SOFTWARE\NewApp");

            var diff = InstallationSnapshotDiffer.CompareSnapshots(before, after);

            Assert.AreEqual(1, diff.AddedFiles.Count);
            Assert.AreEqual(@"C:\Program Files\NewApp\new.exe", diff.AddedFiles[0]);
            Assert.AreEqual(1, diff.AddedRegistryKeys.Count);
            Assert.AreEqual(@"HKEY_LOCAL_MACHINE\SOFTWARE\NewApp", diff.AddedRegistryKeys[0]);
            Assert.AreEqual(0, diff.RemovedFiles.Count);
            Assert.AreEqual(2, diff.TotalChangesCount);
        }
    }
}

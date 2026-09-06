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
using UninstallTools.Exclusions;

namespace EBUninstallerTests
{
    [TestClass]
    public class SettingsTransferEngineTests
    {
        private string _tempFile;

        [TestInitialize]
        public void Setup()
        {
            _tempFile = Path.Combine(Path.GetTempPath(), $"ebu_profile_{Guid.NewGuid():N}.json");
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (File.Exists(_tempFile))
            {
                try { File.Delete(_tempFile); } catch { }
            }
        }

        [TestMethod]
        public void ExportProfile_ValidPath_CreatesFile()
        {
            ExclusionManager.AddExclusion(@"C:\TestApp\Exclusion");

            bool exported = SettingsTransferEngine.ExportProfile(_tempFile, true);
            Assert.IsTrue(exported);
            Assert.IsTrue(File.Exists(_tempFile));

            string content = File.ReadAllText(_tempFile);
            Assert.IsTrue(content.Contains("EBUninstaller Pro Profile Package"));
        }

        [TestMethod]
        public void ValidateProfile_ValidFile_ReturnsTrue()
        {
            SettingsTransferEngine.ExportProfile(_tempFile, false);

            bool isValid = SettingsTransferEngine.ValidateProfile(_tempFile, out var summary);
            Assert.IsTrue(isValid);
            Assert.IsFalse(string.IsNullOrEmpty(summary));
        }

        [TestMethod]
        public void ImportProfile_ExistingPackage_ImportsSuccessfully()
        {
            ExclusionManager.AddExclusion(@"C:\PathToKeep");
            SettingsTransferEngine.ExportProfile(_tempFile, false);

            bool imported = SettingsTransferEngine.ImportProfile(_tempFile, false);
            Assert.IsTrue(imported);
        }
    }
}

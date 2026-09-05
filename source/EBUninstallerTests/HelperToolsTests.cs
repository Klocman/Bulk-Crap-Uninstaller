/*
 * EBUninstaller Pro - Application Uninstaller & System Optimization Suite
 * HelperTools Shared Utilities Subsystem Tests
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
using System.IO;
using Klocman;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32.Interop;

namespace EBUninstallerTests
{
    [TestClass]
    public class HelperToolsTests
    {
        [TestMethod]
        public void TestExtractHrefCodeFromString()
        {
            var msg = "An error occurred during uninstallation: 0x80070005 (Access Denied)";
            var code = HelperTools.ExtractHrefCode(msg);
            Assert.AreNotEqual(ResultWin32.INVALID_ERROR_CODE, code);
        }

        [TestMethod]
        public void TestExtractHrefCodeFromException()
        {
            var ex = new InvalidOperationException("Failed with code 0x00000002");
            var code = HelperTools.ExtractHrefCode(ex);
            Assert.AreEqual(ResultWin32.ERROR_FILE_NOT_FOUND, code);
        }

        [TestMethod]
        public void TestObjectToJsonOutput()
        {
            var dummy = new { Name = "EBUninstaller Pro", Version = "7.0.0", IsPro = true };
            var json = HelperTools.ObjectToJsonOutput(dummy, false);
            Assert.IsNotNull(json);
            Assert.IsTrue(json.Contains("EBUninstaller Pro"));
            Assert.IsTrue(json.Contains("7.0.0"));
        }

        [TestMethod]
        public void TestFormatBytes()
        {
            Assert.AreEqual("0 B", HelperTools.FormatBytes(0));
            Assert.AreEqual("1 KB", HelperTools.FormatBytes(1024));
            Assert.AreEqual("1 MB", HelperTools.FormatBytes(1024 * 1024));
            Assert.AreEqual("1 GB", HelperTools.FormatBytes(1024L * 1024 * 1024));
        }

        [TestMethod]
        public void TestNormalizeLongPath()
        {
            var normalPath = @"C:\Program Files\EBUninstaller";
            var normalized = HelperTools.NormalizeLongPath(normalPath);
            Assert.IsNotNull(normalized);
            Assert.IsTrue(normalized.EndsWith("EBUninstaller"));
        }

        [TestMethod]
        public void TestSafeDeleteFileAndDirectory()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), "ebu_test_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);
            var testFile = Path.Combine(tempDir, "test.txt");
            File.WriteAllText(testFile, "test data");

            Assert.IsTrue(File.Exists(testFile));
            var delFile = HelperTools.SafeDeleteFile(testFile);
            Assert.IsTrue(delFile);
            Assert.IsFalse(File.Exists(testFile));

            var delDir = HelperTools.SafeDeleteDirectory(tempDir);
            Assert.IsTrue(delDir);
            Assert.IsFalse(Directory.Exists(tempDir));
        }

        [TestMethod]
        public void TestProcessExecutionResultModel()
        {
            var res = new ProcessExecutionResult
            {
                ExitCode = 0,
                StandardOutput = "Success",
                StandardError = string.Empty,
                TimedOut = false
            };

            Assert.IsTrue(res.Success);
            Assert.AreEqual(0, res.ExitCode);
            Assert.AreEqual("Success", res.StandardOutput);
        }

        [TestMethod]
        public void TestSystemEnvironmentInfoMemory()
        {
            var total = SystemEnvironmentInfo.GetTotalPhysicalMemoryBytes();
            var avail = SystemEnvironmentInfo.GetAvailablePhysicalMemoryBytes();
            var currentProc = SystemEnvironmentInfo.GetCurrentProcessMemoryUsageBytes();

            // On non-Windows/Wine platforms these Win32 APIs return 0 gracefully without crashing
            Assert.IsTrue(total >= 0);
            Assert.IsTrue(avail >= 0);
            Assert.IsTrue(currentProc >= 0);
        }
    }
}

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
    public class FreeSpaceWiperTests
    {
        [TestMethod]
        public void TestWipeProgressEventArgs()
        {
            var args = new WipeProgressEventArgs
            {
                BytesWiped = 500 * 1024 * 1024,
                TotalFreeBytes = 1000 * 1024 * 1024,
                StatusMessage = "Testing..."
            };

            Assert.AreEqual(50, args.Percentage);
            Assert.AreEqual("Testing...", args.StatusMessage);
        }

        [TestMethod]
        public void TestFreeSpaceWipePatternEnum()
        {
            Assert.AreEqual(FreeSpaceWipePattern.ZeroFill, (FreeSpaceWipePattern)0);
            Assert.AreEqual(FreeSpaceWipePattern.RandomFill, (FreeSpaceWipePattern)1);
            Assert.AreEqual(FreeSpaceWipePattern.TrimOnly, (FreeSpaceWipePattern)2);
        }
    }
}

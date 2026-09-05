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
    public class ShortcutResidualsCleanerTests
    {
        [TestMethod]
        public void TestScanBrokenShortcutsReturnsList()
        {
            var list = ShortcutResidualsCleaner.ScanBrokenShortcuts();
            Assert.IsNotNull(list);
        }

        [TestMethod]
        public void TestBrokenShortcutItemProperties()
        {
            var item = new BrokenShortcutItem
            {
                ShortcutName = "Uninstalled Game",
                ShortcutPath = @"C:\Users\User\Desktop\Uninstalled Game.lnk",
                TargetPath = @"C:\Games\Uninstalled Game\game.exe",
                LocationCategory = "Desktop (User)",
                IsBroken = true,
                IsSelected = true
            };

            Assert.AreEqual("Uninstalled Game", item.ShortcutName);
            Assert.AreEqual(@"C:\Games\Uninstalled Game\game.exe", item.TargetPath);
            Assert.IsTrue(item.IsBroken);
            Assert.IsTrue(item.IsSelected);
        }
    }
}

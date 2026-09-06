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
using UninstallTools.RegistryEngine;

namespace EBUninstallerTests
{
    [TestClass]
    public class ShellHandlersCleanerTests
    {
        [TestMethod]
        public void TestScanShellHandlersReturnsList()
        {
            var handlers = ShellHandlersCleaner.ScanShellHandlers();
            Assert.IsNotNull(handlers, "ScanShellHandlers should return a valid list.");
        }

        [TestMethod]
        public void TestShellHandlerItemProperties()
        {
            var item = new ShellHandlerItem
            {
                HandlerName = "7-Zip",
                TargetClass = "Directory",
                Clsid = "{23170F69-40C1-278A-1000-000100020000}",
                ModulePath = @"C:\Program Files\7-Zip\7-zip.dll",
                RegistryKeyPath = @"Directory\shellex\ContextMenuHandlers\7-Zip",
                IsOrphaned = false,
                IsSelected = true
            };

            Assert.AreEqual("7-Zip", item.HandlerName);
            Assert.AreEqual("Directory", item.TargetClass);
            Assert.AreEqual("{23170F69-40C1-278A-1000-000100020000}", item.Clsid);
            Assert.IsFalse(item.IsOrphaned);
            Assert.IsTrue(item.IsSelected);
        }
    }
}

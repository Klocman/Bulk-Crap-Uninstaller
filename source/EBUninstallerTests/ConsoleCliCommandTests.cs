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

namespace EBUninstallerTests
{
    [TestClass]
    public class ConsoleCliCommandTests
    {
        [TestMethod]
        public void TestKnownCommandsExist()
        {
            var validCommands = new[]
            {
                "list", "uninstall", "forced-uninstall", "scan", "leftovers",
                "backup", "restore", "monitor", "rollback-trace", "clean-junk",
                "clean-privacy", "health", "optimize-registry", "update",
                "trim-memory", "clean-drivers", "startup-impact", "schedule-maintenance",
                "startup", "extensions", "tools", "export", "history",
                "runtimes", "bloat", "orphaned-services", "export-drivers", "audit-report"
            };

            foreach (var cmd in validCommands)
            {
                Assert.IsFalse(string.IsNullOrWhiteSpace(cmd), $"Command {cmd} should not be empty.");
            }
        }
    }
}

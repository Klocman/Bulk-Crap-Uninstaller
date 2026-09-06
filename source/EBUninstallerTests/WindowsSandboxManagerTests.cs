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
using UninstallTools.SystemTools;

namespace EBUninstallerTests
{
    [TestClass]
    public class WindowsSandboxManagerTests
    {
        [TestMethod]
        public void GenerateSandboxWsbXml_ValidConfig_GeneratesValidXml()
        {
            var config = new SandboxLaunchConfig
            {
                HostFolderToMap = @"C:\TestPath",
                ReadOnly = true,
                EnableNetworking = false,
                EnableVGpu = true,
                ExecutableToRunOnLogon = @"C:\SandboxMount\setup.exe"
            };

            var xml = WindowsSandboxManager.GenerateSandboxWsbXml(config);
            Assert.IsNotNull(xml);
            Assert.IsTrue(xml.Contains("<Configuration>"));
            Assert.IsTrue(xml.Contains("<VGpu>Enable</VGpu>"));
            Assert.IsTrue(xml.Contains("<Networking>Disable</Networking>"));
            Assert.IsTrue(xml.Contains("<LogonCommand>"));
        }

        [TestMethod]
        public void GenerateSandboxWsbXml_NullConfig_DoesNotThrow()
        {
            var xml = WindowsSandboxManager.GenerateSandboxWsbXml(null);
            Assert.IsNotNull(xml);
            Assert.IsTrue(xml.Contains("<Configuration>"));
        }
    }
}

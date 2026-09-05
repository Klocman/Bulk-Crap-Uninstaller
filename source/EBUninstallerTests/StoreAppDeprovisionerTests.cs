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
using UninstallTools.StoreApps;

namespace EBUninstallerTests
{
    [TestClass]
    public class StoreAppDeprovisionerTests
    {
        [TestMethod]
        public void TestParseDismAppxOutput()
        {
            var sampleDismOutput = @"
Deployment Image Servicing and Management tool
Version: 10.0.22621.1

Image Version: 10.0.22621.2861

DisplayName : Microsoft.BingNews
Version : 4.53.51341.0
Architecture : neutral
ResourceId : ~
PackageName : Microsoft.BingNews_4.53.51341.0_neutral_~_8wekyb3d8bbwe

DisplayName : Microsoft.Windows.ShellExperienceHost
Version : 10.0.22621.1
Architecture : x64
ResourceId : ~
PackageName : Microsoft.Windows.ShellExperienceHost_10.0.22621.1_neutral_~_cw5n1h2txyewy
";

            var packages = StoreAppDeprovisioner.ParseDismAppxOutput(sampleDismOutput);
            Assert.AreEqual(2, packages.Count);

            Assert.AreEqual("Microsoft.BingNews", packages[0].DisplayName);
            Assert.AreEqual("4.53.51341.0", packages[0].Version);
            Assert.IsFalse(packages[0].IsSystemCritical);

            Assert.AreEqual("Microsoft.Windows.ShellExperienceHost", packages[1].DisplayName);
            Assert.IsTrue(packages[1].IsSystemCritical);
        }

        [TestMethod]
        public void TestProtectedDeprovisioningSafety()
        {
            var protectedPkg = "Microsoft.Windows.ShellExperienceHost_10.0.22621.1_neutral_~_cw5n1h2txyewy";
            var result = StoreAppDeprovisioner.DeprovisionPackage(protectedPkg);
            Assert.IsFalse(result, "Should refuse to deprovision critical Windows shell packages.");
        }
    }
}

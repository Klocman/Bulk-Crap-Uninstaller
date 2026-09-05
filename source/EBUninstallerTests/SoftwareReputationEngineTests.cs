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

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools;
using UninstallTools.Detection;

namespace EBUninstallerTests
{
    [TestClass]
    public class SoftwareReputationEngineTests
    {
        [TestMethod]
        public void EvaluateReputation_NullEntry_ReturnsNeutral()
        {
            var result = SoftwareReputationEngine.EvaluateReputation(null);
            Assert.IsNotNull(result);
            Assert.AreEqual("Unknown Application", result.ApplicationName);
        }

        [TestMethod]
        public void EvaluateReputation_VerifiedPublisher_ReturnsVerifiedTrusted()
        {
            var app = new ApplicationUninstallerEntry
            {
                DisplayName = "Visual Studio Code",
                Publisher = "Microsoft Corporation"
            };

            var record = SoftwareReputationEngine.EvaluateReputation(app);
            Assert.AreEqual(ReputationTier.VerifiedTrusted, record.Tier);
            Assert.IsTrue(record.ReputationScore >= 80);
        }

        [TestMethod]
        public void EvaluateReputation_KnownAdware_ReturnsHighRisk()
        {
            var app = new ApplicationUninstallerEntry
            {
                DisplayName = "MySearch Toolbar Pro",
                Publisher = "Unknown Adware Corp"
            };

            var record = SoftwareReputationEngine.EvaluateReputation(app);
            Assert.AreEqual(ReputationTier.HighRisk, record.Tier);
            Assert.IsTrue(record.IsBundledInstaller);
            Assert.AreEqual(SoftwareCategoryTag.AdwareBundler, record.Category);
        }

        [TestMethod]
        public void EvaluateBatch_MultipleApps_EvaluatesAll()
        {
            var list = new List<ApplicationUninstallerEntry>
            {
                new ApplicationUninstallerEntry { DisplayName = "Git for Windows", Publisher = "Git for Windows" },
                new ApplicationUninstallerEntry { DisplayName = "7-Zip 23.01", Publisher = "Igor Pavlov" },
                new ApplicationUninstallerEntry { DisplayName = "Search Bar Coupon Helper", Publisher = "Bad Actor" }
            };

            var batch = SoftwareReputationEngine.EvaluateBatch(list);
            Assert.AreEqual(3, batch.Count);
        }
    }
}

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
using UninstallTools.Core;

namespace EBUninstallerTests
{
    [TestClass]
    public class SystemHealthScorecardEngineTests
    {
        [TestMethod]
        public void GenerateHealthScorecard_GeneratesValidScorecard()
        {
            var report = SystemHealthScorecardEngine.GenerateHealthScorecard();
            Assert.IsNotNull(report);
            Assert.IsTrue(report.CompositeScore >= 0 && report.CompositeScore <= 100);
            Assert.IsFalse(string.IsNullOrEmpty(report.RatingBadge));
            Assert.IsTrue(report.Categories.Count >= 3);
            Assert.IsTrue(report.ActionableRecommendations.Count > 0);
        }
    }
}

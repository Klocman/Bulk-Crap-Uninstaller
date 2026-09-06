/*
    EBUninstaller Pro - Windows Optional Features Tests
    Unit tests for DISM optional feature parsing and safety guards.
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.Detection;

namespace BulkCrapUninstallerTests
{
    [TestClass]
    public class WindowsOptionalFeaturesTests
    {
        [TestMethod]
        public void TestGetOptionalFeaturesStructure()
        {
            var features = WindowsOptionalFeaturesManager.GetOptionalFeatures();
            Assert.IsNotNull(features);

            foreach (var f in features)
            {
                Assert.IsNotNull(f.FeatureName);
                Assert.IsNotNull(f.DisplayName);
            }
        }

        [TestMethod]
        public void TestCriticalFeatureProtection()
        {
            // Attempting to disable critical features like NetFx4Extended or kernel must be rejected
            bool result = WindowsOptionalFeaturesManager.SetFeatureState("NetFx4Extended-ASPNET45", false);
            Assert.IsFalse(result, "Critical .NET Framework feature must be protected.");

            bool kernelResult = WindowsOptionalFeaturesManager.SetFeatureState("Microsoft-Windows-Kernel", false);
            Assert.IsFalse(kernelResult, "Critical Windows kernel feature must be protected.");
        }

        [TestMethod]
        public void TestOptionalFeatureItemProperties()
        {
            var item = new WindowsOptionalFeatureItem
            {
                FeatureName = "Microsoft-Windows-Subsystem-Linux",
                DisplayName = "Subsystem Linux",
                Description = "Windows Subsystem for Linux",
                State = FeatureState.Enabled,
                IsCapability = false,
                RestartRequired = true,
                IsCritical = false
            };

            Assert.AreEqual("Microsoft-Windows-Subsystem-Linux", item.FeatureName);
            Assert.AreEqual("Subsystem Linux", item.DisplayName);
            Assert.AreEqual(FeatureState.Enabled, item.State);
            Assert.IsFalse(item.IsCapability);
            Assert.IsTrue(item.RestartRequired);
        }
    }
}

/*
    EBUninstaller Pro - Windows Optional Features Tests
    Unit tests for DISM optional feature parsing and safety guards.
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Linq;
using NUnit.Framework;
using UninstallTools.Detection;

namespace BulkCrapUninstallerTests
{
    [TestFixture]
    public class WindowsOptionalFeaturesTests
    {
        [Test]
        public void TestGetOptionalFeaturesStructure()
        {
            var features = WindowsOptionalFeaturesManager.GetOptionalFeatures();
            Assert.That(features, Is.Not.Null);

            foreach (var f in features)
            {
                Assert.That(f.FeatureName, Is.Not.Null);
                Assert.That(f.DisplayName, Is.Not.Null);
            }
        }

        [Test]
        public void TestCriticalFeatureProtection()
        {
            // Attempting to disable critical features like NetFx4Extended or kernel must be rejected
            bool result = WindowsOptionalFeaturesManager.SetFeatureState("NetFx4Extended-ASPNET45", false);
            Assert.That(result, Is.False, "Critical .NET Framework feature must be protected.");

            bool kernelResult = WindowsOptionalFeaturesManager.SetFeatureState("Microsoft-Windows-Kernel", false);
            Assert.That(kernelResult, Is.False, "Critical Windows kernel feature must be protected.");
        }

        [Test]
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

            Assert.That(item.FeatureName, Is.EqualTo("Microsoft-Windows-Subsystem-Linux"));
            Assert.That(item.DisplayName, Is.EqualTo("Subsystem Linux"));
            Assert.That(item.State, Is.EqualTo(FeatureState.Enabled));
            Assert.That(item.IsCapability, Is.False);
            Assert.That(item.RestartRequired, Is.True);
        }
    }
}

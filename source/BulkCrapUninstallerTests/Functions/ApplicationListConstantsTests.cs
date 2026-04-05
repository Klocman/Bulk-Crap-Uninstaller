using BulkCrapUninstaller.Functions.ApplicationList;
using BulkCrapUninstaller.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools;

namespace BulkCrapUninstallerTests.Functions
{
    [TestClass]
    public class ApplicationListConstantsTests
    {
        private bool _advancedHighlightSpecial;
        private bool _advancedTestInvalid;
        private bool _advancedTestCertificates;

        [TestInitialize]
        public void TestInitialize()
        {
            _advancedHighlightSpecial = Settings.Default.AdvancedHighlightSpecial;
            _advancedTestInvalid = Settings.Default.AdvancedTestInvalid;
            _advancedTestCertificates = Settings.Default.AdvancedTestCertificates;

            Settings.Default.AdvancedHighlightSpecial = true;
            Settings.Default.AdvancedTestInvalid = true;
            Settings.Default.AdvancedTestCertificates = true;
        }

        [TestCleanup]
        public void TestCleanup()
        {
            Settings.Default.AdvancedHighlightSpecial = _advancedHighlightSpecial;
            Settings.Default.AdvancedTestInvalid = _advancedTestInvalid;
            Settings.Default.AdvancedTestCertificates = _advancedTestCertificates;
        }

        [TestMethod]
        public void GetApplicationStatusText_ReturnsWindowsFeatureStatus()
        {
            var entry = new ApplicationUninstallerEntry
            {
                UninstallerKind = UninstallerType.WindowsFeature,
                IsValid = true
            };

            var result = ApplicationListConstants.GetApplicationStatusText(entry);

            Assert.AreEqual(entry.UninstallerKind.GetLocalisedName(), result);
        }

        [TestMethod]
        public void GetApplicationStatusText_ReturnsStoreAppStatus()
        {
            var entry = new ApplicationUninstallerEntry
            {
                UninstallerKind = UninstallerType.StoreApp,
                IsValid = true
            };

            var result = ApplicationListConstants.GetApplicationStatusText(entry);

            Assert.AreEqual(entry.UninstallerKind.GetLocalisedName(), result);
        }

        [TestMethod]
        public void GetApplicationStatusText_ReturnsOrphanedStatus()
        {
            var entry = new ApplicationUninstallerEntry
            {
                IsOrphaned = true,
                IsValid = true
            };

            var result = ApplicationListConstants.GetApplicationStatusText(entry);

            Assert.AreEqual(UninstallTools.Properties.Localisation.IsOrphaned, result);
        }

        [TestMethod]
        public void GetApplicationStatusText_ReturnsInvalidStatus()
        {
            var entry = new ApplicationUninstallerEntry
            {
                IsValid = false
            };

            var result = ApplicationListConstants.GetApplicationStatusText(entry);

            Assert.AreEqual(UninstallTools.Properties.Localisation.UninstallStatus_Invalid, result);
        }

        [TestMethod]
        public void GetApplicationStatusText_ReturnsVerifiedCertificateStatus()
        {
            var entry = new ApplicationUninstallerEntry
            {
                IsValid = true
            };
            entry.SetCertificate(null, true);

            var result = ApplicationListConstants.GetApplicationStatusText(entry);

            Assert.AreEqual("Verified certificate", result);
        }

        [TestMethod]
        public void GetApplicationStatusText_ReturnsUnverifiedCertificateStatus()
        {
            var entry = new ApplicationUninstallerEntry
            {
                IsValid = true
            };
            entry.SetCertificate(null, false);

            var result = ApplicationListConstants.GetApplicationStatusText(entry);

            Assert.AreEqual("Unverified certificate", result);
        }

        [TestMethod]
        public void GetApplicationStatusText_ReturnsEmptyWhenNoSpecialStateApplies()
        {
            var entry = new ApplicationUninstallerEntry
            {
                IsValid = true
            };

            var result = ApplicationListConstants.GetApplicationStatusText(entry);

            Assert.AreEqual(Localisable.Empty, result);
        }
    }
}

/*
    EBUninstaller Pro - Unit Tests for Junk and Privacy Cleaners, and Browser Extensions
*/

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.BrowserExtensions;
using UninstallTools.JunkCleaner;
using UninstallTools.PrivacyCleaner;
using UninstallTools.SystemTools;

namespace BulkCrapUninstallerTests
{
    [TestClass]
    public class JunkAndPrivacyCleanerTests
    {
        [TestMethod]
        public async Task JunkCleaner_ScanTemp_DetectsCreatedMockFiles()
        {
            var tempDir = Path.GetTempPath();
            var mockFile = Path.Combine(tempDir, "openuninstall_junk_test_" + Guid.NewGuid().ToString("N") + ".tmp");
            File.WriteAllText(mockFile, "MOCK_JUNK_DATA_FOR_CLEANER");

            try
            {
                var categories = await JunkCleanerEngine.ScanJunkAsync();
                Assert.IsNotNull(categories);
                Assert.IsTrue(categories.Count > 0);

                var userTemp = categories.FirstOrDefault(c => c.CategoryType == JunkCategoryType.UserTemp);
                Assert.IsNotNull(userTemp);
                Assert.IsTrue(userTemp.Items.Any(i => i.FilePath.Equals(mockFile, StringComparison.OrdinalIgnoreCase)));
            }
            finally
            {
                if (File.Exists(mockFile))
                    File.Delete(mockFile);
            }
        }

        [TestMethod]
        public async Task PrivacyCleaner_InitializesAllExpectedCategories()
        {
            var categories = await PrivacyCleanerEngine.ScanPrivacyTracksAsync();
            Assert.IsNotNull(categories);
            Assert.IsTrue(categories.Count >= 10);

            var chromeHistory = categories.FirstOrDefault(c => c.TargetType == PrivacyTargetType.BrowserChromeHistory);
            Assert.IsNotNull(chromeHistory);

            var cookies = categories.FirstOrDefault(c => c.TargetType == PrivacyTargetType.BrowserChromeCookies);
            Assert.IsNotNull(cookies);
            Assert.IsNotNull(cookies.Warning, "Cookies category must include a logout warning disclosure.");
        }

        [TestMethod]
        public void WindowsToolsLauncher_ContainsCoreSystemTools()
        {
            var tools = WindowsToolsLauncher.GetAvailableTools();
            Assert.IsNotNull(tools);
            Assert.IsTrue(tools.Count >= 10);

            Assert.IsTrue(tools.Any(t => t.Name == "Task Manager"));
            Assert.IsTrue(tools.Any(t => t.Name == "Services Manager"));
            Assert.IsTrue(tools.Any(t => t.Name == "Registry Editor"));
            Assert.IsTrue(tools.Any(t => t.Name == "Device Manager"));
            Assert.IsTrue(tools.Any(t => t.Name == "Event Viewer"));
            Assert.IsTrue(tools.Any(t => t.Name == "System Restore"));
        }
    }

    [TestClass]
    public class BrowserExtensionsTests
    {
        [TestMethod]
        public async Task GetInstalledExtensionsAsync_ReturnsCollectionWithoutThrowing()
        {
            var extensions = await BrowserExtensionManager.GetInstalledExtensionsAsync();
            Assert.IsNotNull(extensions);
        }
    }
}

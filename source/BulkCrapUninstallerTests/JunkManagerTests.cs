using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools;
using UninstallTools.Junk;
using UninstallTools.Junk.Containers;
using UninstallTools.Junk.Finders.Misc;

namespace BulkCrapUninstallerTests
{
    [TestClass]
    public class JunkManagerTests
    {
        [TestMethod]
        public void IsCurrentApplicationInstallation_ReturnsTrueForParentInstallLocation()
        {
            var parentDirectory = Directory.GetParent(UninstallToolsGlobalConfig.AppLocation);
            var target = new ApplicationUninstallerEntry
            {
                InstallLocation = parentDirectory?.FullName ?? UninstallToolsGlobalConfig.AppLocation
            };

            Assert.IsTrue(JunkManager.IsCurrentApplicationInstallation(target));
        }

        [TestMethod]
        public void ShouldKeepSelfJunkResult_ReturnsFalseForDifferentApplication()
        {
            var foreignInstallLocation = CreateTempDirectory();

            try
            {
                var foreignApplication = new ApplicationUninstallerEntry
                {
                    RawDisplayName = "Other app",
                    InstallLocation = foreignInstallLocation
                };

                var junk = new FileSystemJunk(new DirectoryInfo(UninstallToolsGlobalConfig.AppLocation), foreignApplication, DummyJunkCreator.Instance);

                Assert.IsFalse(JunkManager.ShouldKeepSelfJunkResult(junk));
            }
            finally
            {
                CleanupDirectory(foreignInstallLocation);
            }
        }

        [TestMethod]
        public void ShouldKeepSelfJunkResult_ReturnsTrueForCurrentApplication()
        {
            var currentApplication = new ApplicationUninstallerEntry
            {
                RawDisplayName = "Bulk Crap Uninstaller",
                InstallLocation = UninstallToolsGlobalConfig.AppLocation
            };

            var junk = new FileSystemJunk(new DirectoryInfo(UninstallToolsGlobalConfig.AppLocation), currentApplication, DummyJunkCreator.Instance);

            Assert.IsTrue(JunkManager.ShouldKeepSelfJunkResult(junk));
        }

        [TestMethod]
        public void ShouldKeepShortcutTarget_AllowsCurrentApplicationShortcuts()
        {
            var shortcutTarget = Path.Combine(UninstallToolsGlobalConfig.AppLocation, "BCUninstaller.exe");
            var currentApplication = new ApplicationUninstallerEntry
            {
                RawDisplayName = "Bulk Crap Uninstaller",
                InstallLocation = UninstallToolsGlobalConfig.AppLocation
            };

            Assert.IsTrue(ShortcutJunk.ShouldKeepShortcutTarget(shortcutTarget, currentApplication));
        }

        [TestMethod]
        public void ShouldKeepShortcutTarget_FiltersCurrentApplicationShortcutsForOtherApps()
        {
            var shortcutTarget = Path.Combine(UninstallToolsGlobalConfig.AppLocation, "BCUninstaller.exe");
            var foreignApplication = new ApplicationUninstallerEntry
            {
                RawDisplayName = "Other app",
                InstallLocation = CreateTempDirectory()
            };

            try
            {
                Assert.IsFalse(ShortcutJunk.ShouldKeepShortcutTarget(shortcutTarget, foreignApplication));
            }
            finally
            {
                CleanupDirectory(foreignApplication.InstallLocation);
            }
        }

        private static string CreateTempDirectory()
        {
            var path = Path.Combine(Path.GetTempPath(), "BCU_JunkManagerTests_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(path);
            return path;
        }

        private static void CleanupDirectory(string path)
        {
            if (Directory.Exists(path))
                Directory.Delete(path, true);
        }

        private sealed class DummyJunkCreator : IJunkCreator
        {
            public static DummyJunkCreator Instance { get; } = new();

            public void Setup(ICollection<ApplicationUninstallerEntry> allUninstallers)
            {
            }

            public IEnumerable<IJunkResult> FindJunk(ApplicationUninstallerEntry target)
            {
                yield break;
            }

            public string CategoryName => "Test";
        }
    }
}

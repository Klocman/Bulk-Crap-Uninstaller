using System.Collections.Generic;
using BulkCrapUninstaller.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools;

namespace BulkCrapUninstallerTests
{
    [TestClass]
    public class ShortcutUninstallMatcherTests
    {
        [TestMethod]
        public void MatchExecutablePathReturnsUniqueExactUninstallerMatch()
        {
            var expected = CreateEntry("C:\\Tools\\Widget\\uninstall.exe", "C:\\Tools\\Widget");
            var entries = new List<ApplicationUninstallerEntry>
            {
                expected,
                CreateEntry("C:\\Tools\\Other\\uninstall.exe", "C:\\Tools\\Other")
            };

            var result = ShortcutUninstallMatcher.MatchExecutablePath(entries, "c:\\tools\\widget\\UNINSTALL.EXE",
                out var ambiguous);

            Assert.IsFalse(ambiguous);
            Assert.AreSame(expected, result);
        }

        [TestMethod]
        public void MatchExecutablePathReturnsUniqueSortedExecutableMatch()
        {
            var expected = CreateEntry("C:\\Apps\\Widget\\uninstall.exe", "C:\\Apps\\Widget");
            expected.SortedExecutables = new[] {"C:\\Launchers\\Widget.exe"};

            var result = ShortcutUninstallMatcher.MatchExecutablePath(
                new List<ApplicationUninstallerEntry> {expected}, "c:\\launchers\\WIDGET.EXE", out var ambiguous);

            Assert.IsFalse(ambiguous);
            Assert.AreSame(expected, result);
        }

        [TestMethod]
        public void MatchExecutablePathReturnsUniqueEntryForExecutableInsideInstallLocation()
        {
            var expected = CreateEntry("C:\\Apps\\Widget\\uninstall.exe", "C:\\Apps\\Widget");

            var result = ShortcutUninstallMatcher.MatchExecutablePath(
                new List<ApplicationUninstallerEntry> {expected}, "C:\\Apps\\Widget\\bin\\Widget.exe", out var ambiguous);

            Assert.IsFalse(ambiguous);
            Assert.AreSame(expected, result);
        }

        [TestMethod]
        public void MatchExecutablePathPrefersExactUninstallerMatchOverInstallLocationMatch()
        {
            var exact = CreateEntry("C:\\Shared\\Tool.exe", "C:\\Apps\\Exact");
            var contained = CreateEntry("C:\\Apps\\Contained\\uninstall.exe", "C:\\Shared");

            var result = ShortcutUninstallMatcher.MatchExecutablePath(
                new List<ApplicationUninstallerEntry> {exact, contained}, "C:\\Shared\\Tool.exe", out var ambiguous);

            Assert.IsFalse(ambiguous);
            Assert.AreSame(exact, result);
        }

        [TestMethod]
        public void MatchExecutablePathReturnsAmbiguousForSharedInstallLocation()
        {
            var entries = new List<ApplicationUninstallerEntry>
            {
                CreateEntry("C:\\Apps\\One\\uninstall.exe", "C:\\Shared"),
                CreateEntry("C:\\Apps\\Two\\uninstall.exe", "C:\\Shared")
            };

            var result = ShortcutUninstallMatcher.MatchExecutablePath(entries, "C:\\Shared\\Tool.exe",
                out var ambiguous);

            Assert.IsTrue(ambiguous);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void MatchExecutablePathReturnsNotFoundWhenNoPathEvidenceExists()
        {
            var entries = new List<ApplicationUninstallerEntry>
            {
                CreateEntry("C:\\Apps\\One\\uninstall.exe", "C:\\Apps\\One"),
                new ApplicationUninstallerEntry {DisplayName = "Incomplete entry"}
            };

            var result = ShortcutUninstallMatcher.MatchExecutablePath(entries, "C:\\Apps\\Unknown\\Unknown.exe",
                out var ambiguous);

            Assert.IsFalse(ambiguous);
            Assert.IsNull(result);
        }

        private static ApplicationUninstallerEntry CreateEntry(string uninstallerPath, string installLocation)
        {
            return new ApplicationUninstallerEntry
            {
                UninstallerFullFilename = uninstallerPath,
                InstallLocation = installLocation
            };
        }
    }
}

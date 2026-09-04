/*
    OpenUninstall Pro - Unit Tests for Security Guard & Protection Subsystem
*/

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.Core;

namespace BulkCrapUninstallerTests
{
    [TestClass]
    public class SecurityGuardTests
    {
        [TestMethod]
        public void IsPathProtected_WindowsSystemDirectories_ReturnsTrue()
        {
            var winDir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
            if (!string.IsNullOrEmpty(winDir))
            {
                Assert.IsTrue(SecurityGuard.IsPathProtected(winDir));
                Assert.IsTrue(SecurityGuard.IsPathProtected(System.IO.Path.Combine(winDir, "System32")));
                Assert.IsTrue(SecurityGuard.IsPathProtected(System.IO.Path.Combine(winDir, "SysWOW64")));
                Assert.IsTrue(SecurityGuard.IsPathProtected(System.IO.Path.Combine(winDir, "WinSxS")));
            }
        }

        [TestMethod]
        public void IsPathProtected_NullOrEmptyPath_ReturnsTrue()
        {
            Assert.IsTrue(SecurityGuard.IsPathProtected(null));
            Assert.IsTrue(SecurityGuard.IsPathProtected(""));
            Assert.IsTrue(SecurityGuard.IsPathProtected("   "));
        }

        [TestMethod]
        public void IsRegistryKeyProtected_CriticalHives_ReturnsTrue()
        {
            Assert.IsTrue(SecurityGuard.IsRegistryKeyProtected(@"HKEY_LOCAL_MACHINE\SAM"));
            Assert.IsTrue(SecurityGuard.IsRegistryKeyProtected(@"HKEY_LOCAL_MACHINE\SECURITY"));
            Assert.IsTrue(SecurityGuard.IsRegistryKeyProtected(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control"));
            Assert.IsTrue(SecurityGuard.IsRegistryKeyProtected(@"HKLM\SYSTEM"));
            Assert.IsTrue(SecurityGuard.IsRegistryKeyProtected(@"HKCU\Software"));
            Assert.IsTrue(SecurityGuard.IsRegistryKeyProtected(@"HKEY_CLASSES_ROOT"));
        }

        [TestMethod]
        public void NormalizeRegistryPath_CanonicalizesShortHives()
        {
            Assert.AreEqual(@"HKEY_LOCAL_MACHINE\Software\Test", SecurityGuard.NormalizeRegistryPath(@"HKLM\Software\Test"));
            Assert.AreEqual(@"HKEY_CURRENT_USER\Software\Test", SecurityGuard.NormalizeRegistryPath(@"HKCU\Software\Test"));
            Assert.AreEqual(@"HKEY_CLASSES_ROOT\CLSID", SecurityGuard.NormalizeRegistryPath(@"HKCR\CLSID"));
            Assert.AreEqual(@"HKEY_LOCAL_MACHINE\Software\Test", SecurityGuard.NormalizeRegistryPath(@"[HKLM\Software\Test]"));
        }

        [TestMethod]
        public void ContainsMetacharacters_DetectsDangerousMetasymbols()
        {
            Assert.IsTrue(SecurityGuard.ContainsMetacharacters("calc.exe & notepad.exe"));
            Assert.IsTrue(SecurityGuard.ContainsMetacharacters("cmd.exe | whoami"));
            Assert.IsTrue(SecurityGuard.ContainsMetacharacters("app.exe; rm -rf /"));
            Assert.IsTrue(SecurityGuard.ContainsMetacharacters("app.exe > output.txt"));
            Assert.IsTrue(SecurityGuard.ContainsMetacharacters("app.exe `command`"));
            Assert.IsFalse(SecurityGuard.ContainsMetacharacters("C:\\Program Files\\App\\uninstall.exe /S /silent"));
        }

        [TestMethod]
        public void SanitizeCommandLineArgument_WrapsAndEscapesCorrectly()
        {
            var clean = SecurityGuard.SanitizeCommandLineArgument("C:\\Program Files\\App\\uninstall.exe");
            Assert.IsTrue(clean.StartsWith("\"") && clean.EndsWith("\""));

            var quotedWithInner = SecurityGuard.SanitizeCommandLineArgument("hello \"world\"");
            Assert.IsTrue(quotedWithInner.Contains("\\\""));
        }
    }
}

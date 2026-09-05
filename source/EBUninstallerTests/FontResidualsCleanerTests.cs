/*
    EBUninstaller Pro - Windows Font Residuals Cleaner Tests
    Unit tests for font residual models and orphan detection logic.
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.JunkCleaner;

namespace BulkCrapUninstallerTests
{
    [TestClass]
    public class FontResidualsCleanerTests
    {
        [TestMethod]
        public void TestFontResidualItemProperties()
        {
            var font = new FontResidualItem
            {
                FontName = "Custom App Font (TrueType)",
                FontFileName = "custom_app_font.ttf",
                ResolvedPath = @"C:\Windows\Fonts\custom_app_font.ttf",
                RegistryKeyPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts",
                IsCurrentUser = false,
                IsOrphaned = true
            };

            Assert.AreEqual("Custom App Font (TrueType)", font.FontName);
            Assert.AreEqual("custom_app_font.ttf", font.FontFileName);
            Assert.AreEqual(@"C:\Windows\Fonts\custom_app_font.ttf", font.ResolvedPath);
            Assert.IsFalse(font.IsCurrentUser);
            Assert.IsTrue(font.IsOrphaned);
        }
    }
}

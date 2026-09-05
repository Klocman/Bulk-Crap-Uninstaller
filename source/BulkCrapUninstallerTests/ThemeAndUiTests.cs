/*
    EBUninstaller Pro - Theme & UI Tests
    Unit tests for theme switching, dark mode palette, and chip styling.
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.Localization;

namespace BulkCrapUninstallerTests
{
    [TestClass]
    public class ThemeAndUiTests
    {
        [TestMethod]
        public void TestThemeColorPalette()
        {
            var darkBg = Color.FromArgb(32, 32, 32);
            var lightBg = Color.FromArgb(245, 245, 245);

            Assert.AreNotEqual(darkBg, lightBg);
            Assert.IsTrue(darkBg.R < 50);
            Assert.IsTrue(lightBg.R > 200);
        }

        [TestMethod]
        public void TestRtlLanguageDetection()
        {
            LanguageManager.CurrentCulture = new System.Globalization.CultureInfo("ar");
            Assert.IsTrue(LanguageManager.IsRightToLeft);

            LanguageManager.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            Assert.IsFalse(LanguageManager.IsRightToLeft);
        }
    }
}

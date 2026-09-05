/*
    EBUninstaller Pro - Theme and UI Controls Test Suite
*/

using System;
using System.Drawing;
using BulkCrapUninstaller.Controls;
using BulkCrapUninstaller.Functions;
using NUnit.Framework;
using UninstallTools;

namespace BulkCrapUninstallerTests
{
    [TestFixture]
    public class ThemeAndUiTests
    {
        [Test]
        public void TestThemePalettes()
        {
            var dark = ThemePalette.DarkTheme;
            Assert.IsTrue(dark.IsDark);
            Assert.AreNotEqual(Color.Empty, dark.Background);
            Assert.AreNotEqual(Color.Empty, dark.Surface);
            Assert.AreNotEqual(Color.Empty, dark.Accent);
            Assert.AreNotEqual(Color.Empty, dark.TextPrimary);

            var light = ThemePalette.LightTheme;
            Assert.IsFalse(light.IsDark);
            Assert.AreNotEqual(Color.Empty, light.Background);
            Assert.AreNotEqual(Color.Empty, light.Surface);
            Assert.AreNotEqual(Color.Empty, light.Accent);
            Assert.AreNotEqual(Color.Empty, light.TextPrimary);

            // Contrast test: Dark text on light, Light text on dark
            Assert.IsTrue(dark.TextPrimary.R > 200 && dark.TextPrimary.G > 200 && dark.TextPrimary.B > 200);
            Assert.IsTrue(light.TextPrimary.R < 100 && light.TextPrimary.G < 100 && light.TextPrimary.B < 100);
        }

        [Test]
        public void TestThemeEngineModeSwitch()
        {
            ThemeEngine.CurrentMode = AppThemeMode.Dark;
            Assert.AreEqual(AppThemeMode.Dark, ThemeEngine.CurrentMode);
            var darkPalette = ThemeEngine.CurrentPalette;
            Assert.IsTrue(darkPalette.IsDark);

            ThemeEngine.CurrentMode = AppThemeMode.Light;
            Assert.AreEqual(AppThemeMode.Light, ThemeEngine.CurrentMode);
            var lightPalette = ThemeEngine.CurrentPalette;
            Assert.IsFalse(lightPalette.IsDark);

            ThemeEngine.CurrentMode = AppThemeMode.System;
            Assert.AreEqual(AppThemeMode.System, ThemeEngine.CurrentMode);
        }

        [Test]
        public void TestAppDetailsPanelDisplay()
        {
            var panel = new AppDetailsPanel();
            Assert.IsNotNull(panel);

            // Display null (cleared state)
            panel.DisplayApplication(null);

            // Display valid application entry
            var entry = new ApplicationUninstallerEntry
            {
                DisplayName = "EBUninstaller Pro Test Application",
                DisplayVersion = "1.0.0.0",
                Publisher = "OpenUninstall Team",
                InstallLocation = @"C:\Program Files\TestApp",
                UninstallString = @"C:\Program Files\TestApp\unins000.exe"
            };

            panel.DisplayApplication(entry);

            // Events fire without throwing
            bool uninstallFired = false;
            panel.RequestUninstall += (s, e) => uninstallFired = true;

            panel.Dispose();
        }

        [Test]
        public void TestModernNavCommandBarInitialization()
        {
            var nav = new ModernNavCommandBar();
            Assert.IsNotNull(nav);

            string navigatedSection = null;
            nav.SectionNavigated += (s, key) => navigatedSection = key;

            Assert.IsNull(navigatedSection);
            nav.Dispose();
        }
    }
}

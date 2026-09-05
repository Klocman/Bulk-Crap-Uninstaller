/*
    EBUninstaller Pro - Unit Tests for Localization and Target Mode Subsystems
*/

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools;
using UninstallTools.HunterMode;
using UninstallTools.Localization;

namespace BulkCrapUninstallerTests
{
    [TestClass]
    public class LocalizationManagerTests
    {
        [TestMethod]
        public void GetSupportedLanguages_ReturnsEnglishGermanAndArabic()
        {
            var langs = LanguageManager.GetSupportedLanguages();
            Assert.IsNotNull(langs);
            Assert.AreEqual(3, langs.Count);

            var ar = langs[2];
            Assert.AreEqual(SupportedLanguage.Arabic, ar.Language);
            Assert.IsTrue(ar.IsRightToLeft, "Arabic language must indicate Right-To-Left layout.");
        }

        [TestMethod]
        public void GetString_ReturnsAppropriateTranslations()
        {
            LanguageManager.SetLanguage(SupportedLanguage.English);
            Assert.AreEqual("System Junk Cleaner", LanguageManager.GetString("JunkCleaner_Title"));
            Assert.AreEqual("EBUninstaller Pro", LanguageManager.GetString("App_Title"));

            LanguageManager.SetLanguage(SupportedLanguage.German);
            Assert.AreEqual("Systemmüll-Bereinigung", LanguageManager.GetString("JunkCleaner_Title"));
            Assert.AreEqual("EBUninstaller Pro", LanguageManager.GetString("App_Title"));

            LanguageManager.SetLanguage(SupportedLanguage.Arabic);
            Assert.AreEqual("تنظيف ملفات النظام غير المرغوب فيها", LanguageManager.GetString("JunkCleaner_Title"));
            Assert.AreEqual("EBUninstaller Pro - أداة إلغاء التثبيت المتقدمة", LanguageManager.GetString("App_Title"));
            Assert.AreEqual("صحة النظام", LanguageManager.GetString("Nav_Health"));
            Assert.AreEqual("إلغاء التثبيت", LanguageManager.GetString("Btn_Uninstall"));

            // Reset back to English
            LanguageManager.SetLanguage(SupportedLanguage.English);
        }
    }

    [TestClass]
    public class TargetModeControllerTests
    {
        [TestMethod]
        public void InspectFile_MatchesKnownInstalledApp()
        {
            var app = new ApplicationUninstallerEntry
            {
                DisplayName = "Target Test Suite App",
                InstallLocation = "C:\\Program Files\\TargetApp",
                UninstallerFullFilename = "C:\\Program Files\\TargetApp\\uninstall.exe"
            };

            var result = TargetModeController.InspectFile("C:\\Program Files\\TargetApp\\app.exe", new[] { app });

            Assert.IsNotNull(result);
            Assert.AreEqual("C:\\Program Files\\TargetApp\\app.exe", result.ExecutablePath);
            Assert.IsNotNull(result.MatchedApplication);
            Assert.AreEqual("Target Test Suite App", result.MatchedApplication.DisplayName);
        }

        [TestMethod]
        public void InspectFile_NonExistentFile_ReturnsAppropriateStatus()
        {
            var result = TargetModeController.InspectFile("C:\\NonExistentPath\\random.exe", null);
            Assert.IsNotNull(result);
            Assert.AreEqual("File does not exist", result.StatusMessage);
        }
    }
}

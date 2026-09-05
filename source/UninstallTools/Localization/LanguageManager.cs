/*
    OpenUninstall Pro - Open Source Professional Windows Uninstaller
    Language & RTL Localization Manager Subsystem
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using UninstallTools.Core;

namespace UninstallTools.Localization
{
    public enum SupportedLanguage
    {
        English,
        German,
        Arabic
    }

    public sealed class LanguageInfo
    {
        public SupportedLanguage Language { get; set; }
        public string CultureCode { get; set; }
        public string DisplayName { get; set; }
        public string NativeName { get; set; }
        public bool IsRightToLeft { get; set; }

        public override string ToString() => $"{DisplayName} ({NativeName})";
    }

    public static class LanguageManager
    {
        private static readonly Dictionary<SupportedLanguage, LanguageInfo> Languages = new()
        {
            {
                SupportedLanguage.English,
                new LanguageInfo
                {
                    Language = SupportedLanguage.English,
                    CultureCode = "en",
                    DisplayName = "English",
                    NativeName = "English",
                    IsRightToLeft = false
                }
            },
            {
                SupportedLanguage.German,
                new LanguageInfo
                {
                    Language = SupportedLanguage.German,
                    CultureCode = "de",
                    DisplayName = "German",
                    NativeName = "Deutsch",
                    IsRightToLeft = false
                }
            },
            {
                SupportedLanguage.Arabic,
                new LanguageInfo
                {
                    Language = SupportedLanguage.Arabic,
                    CultureCode = "ar",
                    DisplayName = "Arabic",
                    NativeName = "العربية",
                    IsRightToLeft = true
                }
            }
        };

        private static SupportedLanguage _currentLanguage = SupportedLanguage.English;

        public static event EventHandler<LanguageInfo> LanguageChanged;

        public static SupportedLanguage CurrentLanguage => _currentLanguage;

        public static IReadOnlyList<LanguageInfo> GetSupportedLanguages()
        {
            return new List<LanguageInfo>(Languages.Values);
        }

        public static void SetLanguage(SupportedLanguage language, Form mainForm = null)
        {
            if (!Languages.TryGetValue(language, out var info)) return;

            _currentLanguage = language;
            var culture = new CultureInfo(info.CultureCode);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            if (mainForm != null)
            {
                ApplyLayoutDirection(mainForm, info.IsRightToLeft);
            }

            StructuredLogger.Info(LogCategory.General, $"Applied language: {info.DisplayName} ({info.CultureCode}) [RTL: {info.IsRightToLeft}]");

            try
            {
                LanguageChanged?.Invoke(null, info);
            }
            catch { }
        }

        public static void ApplyLayoutDirection(Control control, bool isRtl)
        {
            if (control == null) return;

            control.RightToLeft = isRtl ? RightToLeft.Yes : RightToLeft.No;
            if (control is Form form)
            {
                form.RightToLeftLayout = isRtl;
            }

            foreach (Control child in control.Controls)
            {
                ApplyLayoutDirection(child, isRtl);
            }
        }

        public static string GetString(string key, string fallback = null)
        {
            // Provides direct lookup for Pro tool titles and actions
            return _currentLanguage switch
            {
                SupportedLanguage.German => GetGermanString(key) ?? fallback ?? key,
                SupportedLanguage.Arabic => GetArabicString(key) ?? fallback ?? key,
                _ => GetEnglishString(key) ?? fallback ?? key
            };
        }

        private static string GetEnglishString(string key) => key switch
        {
            "App_Title" => "EBUninstaller Pro",
            "ProTools_Menu" => "Pro Tools & Cleanup",
            "ForcedUninstall_Title" => "Forced Application Removal",
            "BackupManager_Title" => "Backup & Recovery Center",
            "InstallationMonitor_Title" => "Installation Monitor & Snapshots",
            "JunkCleaner_Title" => "System Junk Cleaner",
            "PrivacyCleaner_Title" => "Browser & Privacy Cleaner",
            "BrowserExtensions_Title" => "Browser Extensions Manager",
            "WindowsTools_Title" => "Windows Administrative Tools",
            "OperationHistory_Title" => "Operation History & Audit Log",
            "SecureDelete_Title" => "Secure File & Folder Shredder",
            _ => null
        };

        private static string GetGermanString(string key) => key switch
        {
            "App_Title" => "EBUninstaller Pro",
            "ProTools_Menu" => "Pro-Bereinigung & Werkzeuge",
            "ForcedUninstall_Title" => "Erzwungene Programmdeinstallation",
            "BackupManager_Title" => "Sicherungs- & Wiederherstellungscenter",
            "InstallationMonitor_Title" => "Installationswächter & Momentaufnahmen",
            "JunkCleaner_Title" => "Systemmüll-Bereinigung",
            "PrivacyCleaner_Title" => "Browser- & Datenschutz-Bereinigung",
            "BrowserExtensions_Title" => "Browser-Erweiterungs-Manager",
            "WindowsTools_Title" => "Windows-Verwaltungstools",
            "OperationHistory_Title" => "Vorgangsverlauf & Überwachungsprotokoll",
            "SecureDelete_Title" => "Sicherer Datei- & Ordnerschredder",
            _ => null
        };

        private static string GetArabicString(string key) => key switch
        {
            "App_Title" => "EBUninstaller Pro - أداة إلغاء التثبيت المتقدمة",
            "ProTools_Menu" => "أدوات متقدمة وتنظيف",
            "ForcedUninstall_Title" => "إزالة البرامج بالقوة",
            "BackupManager_Title" => "مركز النسخ الاحتياطي والاستعادة",
            "InstallationMonitor_Title" => "مراقب التثبيت واللقطات",
            "JunkCleaner_Title" => "تنظيف ملفات النظام غير المرغوب فيها",
            "PrivacyCleaner_Title" => "تنظيف المتصفح والخصوصية",
            "BrowserExtensions_Title" => "مدير إضافات المتصفح",
            "WindowsTools_Title" => "أدوات ويندوز الإدارية",
            "OperationHistory_Title" => "سجل العمليات والتدقيق",
            "SecureDelete_Title" => "حذف الملفات والمجلدات الآمن",
            _ => null
        };
    }
}

/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.Drawing;
using BulkCrapUninstaller.Properties;
using UninstallTools;

namespace BulkCrapUninstaller.Functions.ApplicationList
{
    internal static class ApplicationListConstants
    {
        public static Color VerifiedColor = Color.FromArgb(unchecked((int)0xffccffcc));
        public static Color UnverifiedColor = Color.FromArgb(unchecked((int)0xffbbddff));
        public static Color InvalidColor = Color.FromArgb(unchecked((int)0xffE0E0E0));
        public static Color UnregisteredColor = Color.FromArgb(unchecked((int)0xffffcccc));
        public static Color WindowsFeatureColor = Color.FromArgb(unchecked((int)0xffddbbff));
        public static Color WindowsStoreAppColor = Color.FromArgb(unchecked((int)0xffa3ffff));

        public static Color GetApplicationBackColor(ApplicationUninstallerEntry entry)
        {
            if (Settings.Default.AdvancedHighlightSpecial)
            {
                if (entry.UninstallerKind == UninstallerType.WindowsFeature)
                    return WindowsFeatureColor;

                if (entry.UninstallerKind == UninstallerType.StoreApp)
                    return WindowsStoreAppColor;

                if (entry.IsOrphaned)
                    return UnregisteredColor;
            }

            if (!entry.IsValid && Settings.Default.AdvancedTestInvalid)
                return InvalidColor;

            if (Settings.Default.AdvancedTestCertificates)
            {
                var result = entry.IsCertificateValid(true);
                if (result.HasValue)
                    return result.Value
                        ? VerifiedColor
                        : UnverifiedColor;
            }

            return Color.Empty;
        }

        public static Color GetApplicationTreemapColor(ApplicationUninstallerEntry entry)
        {
            if (entry.UninstallerKind == UninstallerType.WindowsFeature)
                return WindowsFeatureColor;

            if (entry.UninstallerKind == UninstallerType.StoreApp)
                return WindowsStoreAppColor;

            if (entry.IsOrphaned)
                return UnregisteredColor;

            if (!entry.IsValid)
                return InvalidColor;
            
            if (Settings.Default.AdvancedTestCertificates)
            {
                var result = entry.IsCertificateValid(true);
                if (result.HasValue)
                    return result.Value
                        ? VerifiedColor
                        : UnverifiedColor;
            }

            return Color.White;
        }
    }
}
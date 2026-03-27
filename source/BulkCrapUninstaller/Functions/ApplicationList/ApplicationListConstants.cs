/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.ComponentModel;
using System.Drawing;
using BulkCrapUninstaller.Controls;
using BulkCrapUninstaller.Properties;
using UninstallTools;
using UninstallToolsLocalisation = UninstallTools.Properties.Localisation;

namespace BulkCrapUninstaller.Functions.ApplicationList
{
    internal static class ApplicationListConstants
    {
        private static readonly ComponentResourceManager ListLegendResources = new(typeof(ListLegend));

        public static ApplicationListColors Colors => Settings.Default.MiscColorblind ? ApplicationListColors.ColorBlind : ApplicationListColors.Normal;

        public static string GetApplicationStatusText(ApplicationUninstallerEntry entry)
        {
            if (entry == null) return Localisable.Empty;

            if (Settings.Default.AdvancedHighlightSpecial)
            {
                if (entry.UninstallerKind == UninstallerType.WindowsFeature)
                    return entry.UninstallerKind.GetLocalisedName();

                if (entry.UninstallerKind == UninstallerType.StoreApp)
                    return entry.UninstallerKind.GetLocalisedName();

                if (entry.IsOrphaned)
                    return UninstallToolsLocalisation.IsOrphaned;
            }

            if (!entry.IsValid && Settings.Default.AdvancedTestInvalid)
                return UninstallToolsLocalisation.UninstallStatus_Invalid;

            if (Settings.Default.AdvancedTestCertificates)
            {
                var result = entry.IsCertificateValid(true);
                if (result.HasValue)
                    return result.Value ? GetListLegendText("labelVerified.Text", "Verified certificate") : GetListLegendText("labelUnverified.Text", "Unverified certificate");
            }

            return Localisable.Empty;
        }

        public static Color GetApplicationBackColor(ApplicationUninstallerEntry entry)
        {
            if (Settings.Default.AdvancedHighlightSpecial)
            {
                if (entry.UninstallerKind == UninstallerType.WindowsFeature)
                    return Colors.WindowsFeatureColor;

                if (entry.UninstallerKind == UninstallerType.StoreApp)
                    return Colors.WindowsStoreAppColor;

                if (entry.IsOrphaned)
                    return Colors.UnregisteredColor;
            }

            if (!entry.IsValid && Settings.Default.AdvancedTestInvalid)
                return Colors.InvalidColor;

            if (Settings.Default.AdvancedTestCertificates)
            {
                var result = entry.IsCertificateValid(true);
                if (result.HasValue)
                    return result.Value
                        ? Colors.VerifiedColor
                        : Colors.UnverifiedColor;
            }

            return Color.Empty;
        }

        public static Color GetApplicationTreemapColor(ApplicationUninstallerEntry entry)
        {
            if (entry.UninstallerKind == UninstallerType.WindowsFeature)
                return Colors.WindowsFeatureColor;

            if (entry.UninstallerKind == UninstallerType.StoreApp)
                return Colors.WindowsStoreAppColor;

            if (entry.IsOrphaned)
                return Colors.UnregisteredColor;

            if (!entry.IsValid)
                return Colors.InvalidColor;

            if (Settings.Default.AdvancedTestCertificates)
            {
                var result = entry.IsCertificateValid(true);
                if (result.HasValue)
                    return result.Value
                        ? Colors.VerifiedColor
                        : Colors.UnverifiedColor;
            }

            return Color.White;
        }

        private static string GetListLegendText(string resourceName, string fallback)
        {
            return ListLegendResources.GetString(resourceName) ?? fallback;
        }
    }
}

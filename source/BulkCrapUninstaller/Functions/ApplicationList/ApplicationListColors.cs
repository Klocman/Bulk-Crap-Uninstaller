/*
    Copyright (c) 2018 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.Drawing;

namespace BulkCrapUninstaller.Functions.ApplicationList
{
    internal class ApplicationListColors
    {
        public static ApplicationListColors Normal = new(
            Color.FromArgb(unchecked((int)0xffE3F5E3)), // Verified (Soft green)
            Color.FromArgb(unchecked((int)0xffDDE9F5)), // Unverified (Soft blue)
            Color.FromArgb(unchecked((int)0xffECECEC)), // Invalid (Soft gray)
            Color.FromArgb(unchecked((int)0xffF5EDE3)), // Unregistered (Soft peach)
            Color.FromArgb(unchecked((int)0xffEDE3F5)), // WindowsFeature (Soft purple)
            Color.FromArgb(unchecked((int)0xffDDF5F5))); // StoreApp (Soft cyan)

        public static ApplicationListColors ColorBlind = new(
            Color.FromArgb(unchecked((int) 0xfff6382d)), Color.FromArgb(unchecked((int)0xfffc8d59)),
            Color.FromArgb(unchecked((int) 0xff5189d3)), Color.FromArgb(unchecked((int)0xff91bfdb)),
            Color.FromArgb(unchecked((int)0xfffee090)), Color.FromArgb(unchecked((int) 0xffc9dade)));

        public static ApplicationListColors Dark = new(
            Color.FromArgb(unchecked((int)0xff1A2F1D)), // Verified (Greenish) - Darker and desaturated
            Color.FromArgb(unchecked((int)0xff1A242F)), // Unverified (Blueish) - Darker and desaturated
            Color.FromArgb(unchecked((int)0xff242424)), // Invalid (Dark Gray) - Darker and desaturated
            Color.FromArgb(unchecked((int)0xff2F241A)), // Unregistered (Brownish) - Darker and desaturated
            Color.FromArgb(unchecked((int)0xff241A2F)), // WindowsFeature (Purplish) - Darker and desaturated
            Color.FromArgb(unchecked((int)0xff1A2F2F))); // StoreApp (Cyanish) - Darker and desaturated

        public static ApplicationListColors DarkColorBlind = new(
            Color.FromArgb(unchecked((int)0xff241616)), // Verified (Muted Red)
            Color.FromArgb(unchecked((int)0xff241C16)), // Unverified (Muted Orange)
            Color.FromArgb(unchecked((int)0xff161C24)), // Invalid (Muted Blue)
            Color.FromArgb(unchecked((int)0xff162424)), // Unregistered (Muted Cyan)
            Color.FromArgb(unchecked((int)0xff242416)), // WindowsFeature (Muted Yellow)
            Color.FromArgb(unchecked((int)0xff1E1624))); // StoreApp (Muted Purple)

        public ApplicationListColors(Color verifiedColor, Color unverifiedColor, Color invalidColor,
            Color unregisteredColor, Color windowsFeatureColor, Color windowsStoreAppColor)
        {
            VerifiedColor = verifiedColor;
            UnverifiedColor = unverifiedColor;
            InvalidColor = invalidColor;
            UnregisteredColor = unregisteredColor;
            WindowsFeatureColor = windowsFeatureColor;
            WindowsStoreAppColor = windowsStoreAppColor;
        }

        public Color VerifiedColor { get; }
        public Color UnverifiedColor { get; }
        public Color InvalidColor { get; }
        public Color UnregisteredColor { get; }
        public Color WindowsFeatureColor { get; }
        public Color WindowsStoreAppColor { get; }
    }
}
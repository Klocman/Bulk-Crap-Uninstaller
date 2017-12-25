/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.Drawing;

namespace BulkCrapUninstaller.Functions
{
    internal static class ApplicationListConstants
    {
        public static Color VerifiedColor = Color.FromArgb(unchecked((int)0xffccffcc));
        public static Color UnverifiedColor = Color.FromArgb(unchecked((int)0xffbbddff));
        public static Color InvalidColor = Color.FromArgb(unchecked((int)0xffE0E0E0));
        public static Color UnregisteredColor = Color.FromArgb(unchecked((int)0xffffcccc));
        public static Color WindowsFeatureColor = Color.FromArgb(unchecked((int)0xffddbbff));
        public static Color WindowsStoreAppColor = Color.FromArgb(unchecked((int)0xffa3ffff));
    }
}
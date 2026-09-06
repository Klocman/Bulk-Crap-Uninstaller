/*
    EBUninstaller Pro - Modern Windows 11 Theme Engine
    Supports Dark Mode, Light Mode, and Windows System Theme Sync
*/

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using BrightIdeasSoftware;
using Microsoft.Win32;
using UninstallTools.Core;

namespace BulkCrapUninstaller.Functions
{
    public enum AppThemeMode
    {
        System,
        Light,
        Dark
    }

    public sealed class ThemePalette
    {
        public bool IsDark { get; set; }
        public Color Background { get; set; }
        public Color Surface { get; set; }
        public Color SurfaceHighlight { get; set; }
        public Color Border { get; set; }
        public Color TextPrimary { get; set; }
        public Color TextSecondary { get; set; }
        public Color Accent { get; set; }
        public Color AccentHover { get; set; }
        public Color Danger { get; set; }
        public Color Success { get; set; }
        public Color Warning { get; set; }

        public static ThemePalette DarkTheme => new()
        {
            IsDark = true,
            Background = Color.FromArgb(32, 32, 32),
            Surface = Color.FromArgb(45, 45, 48),
            SurfaceHighlight = Color.FromArgb(60, 60, 65),
            Border = Color.FromArgb(70, 70, 75),
            TextPrimary = Color.FromArgb(240, 240, 240),
            TextSecondary = Color.FromArgb(170, 170, 170),
            Accent = Color.FromArgb(96, 205, 255),
            AccentHover = Color.FromArgb(115, 215, 255),
            Danger = Color.FromArgb(255, 100, 100),
            Success = Color.FromArgb(108, 203, 95),
            Warning = Color.FromArgb(255, 185, 0)
        };

        public static ThemePalette LightTheme => new()
        {
            IsDark = false,
            Background = Color.FromArgb(243, 243, 243),
            Surface = Color.FromArgb(255, 255, 255),
            SurfaceHighlight = Color.FromArgb(235, 235, 235),
            Border = Color.FromArgb(215, 215, 215),
            TextPrimary = Color.FromArgb(25, 25, 25),
            TextSecondary = Color.FromArgb(90, 90, 90),
            Accent = Color.FromArgb(0, 103, 192),
            AccentHover = Color.FromArgb(0, 120, 215),
            Danger = Color.FromArgb(196, 43, 28),
            Success = Color.FromArgb(16, 124, 65),
            Warning = Color.FromArgb(157, 93, 0)
        };
    }

    public static class ThemeEngine
    {
        [DllImport("dwmapi.dll", PreserveSig = true)]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        private const int DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 = 19;
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

        private static AppThemeMode _currentMode = AppThemeMode.System;

        public static AppThemeMode CurrentMode
        {
            get => _currentMode;
            set => _currentMode = value;
        }

        public static ThemePalette CurrentPalette => IsSystemInDarkMode() || _currentMode == AppThemeMode.Dark
            ? (_currentMode == AppThemeMode.Light ? ThemePalette.LightTheme : ThemePalette.DarkTheme)
            : (_currentMode == AppThemeMode.Dark ? ThemePalette.DarkTheme : ThemePalette.LightTheme);

        public static bool IsDarkModeActive => CurrentPalette.IsDark;

        public static bool IsSystemInDarkMode()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
                if (key != null)
                {
                    var val = key.GetValue("AppsUseLightTheme");
                    if (val is int intVal)
                        return intVal == 0;
                }
            }
            catch { }
            return false;
        }

        public static void ApplyThemeToForm(Form form)
        {
            if (form == null || form.IsDisposed) return;

            var palette = CurrentPalette;
            form.BackColor = palette.Background;
            form.ForeColor = palette.TextPrimary;

            // Apply immersive dark title bar on Windows 10/11
            ApplyImmersiveDarkMode(form.Handle, palette.IsDark);

            ApplyThemeRecursive(form, palette);
        }

        private static void ApplyThemeRecursive(Control parent, ThemePalette palette)
        {
            if (parent == null) return;

            foreach (Control ctrl in parent.Controls)
            {
                if (ctrl is MenuStrip || ctrl is ToolStrip || ctrl is StatusStrip)
                {
                    ctrl.BackColor = palette.Surface;
                    ctrl.ForeColor = palette.TextPrimary;
                }
                else if (ctrl is ObjectListView olv)
                {
                    olv.BackColor = palette.Surface;
                    olv.ForeColor = palette.TextPrimary;
                    olv.HeaderFormatStyle ??= new HeaderFormatStyle();
                    olv.HeaderFormatStyle.Normal.BackColor = palette.SurfaceHighlight;
                    olv.HeaderFormatStyle.Normal.ForeColor = palette.TextPrimary;
                    olv.SelectedBackColor = palette.Accent;
                    olv.SelectedForeColor = Color.White;
                }
                else if (ctrl is Button btn)
                {
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderColor = palette.Border;
                    btn.BackColor = palette.Surface;
                    btn.ForeColor = palette.TextPrimary;
                }
                else if (ctrl is TextBox tb)
                {
                    tb.BackColor = palette.Surface;
                    tb.ForeColor = palette.TextPrimary;
                }
                else if (ctrl is GroupBox gb)
                {
                    gb.ForeColor = palette.TextPrimary;
                }
                else if (ctrl is Label lbl)
                {
                    if (lbl.ForeColor != palette.Danger && lbl.ForeColor != palette.Success && lbl.ForeColor != palette.Warning)
                        lbl.ForeColor = palette.TextPrimary;
                }
                else if (ctrl is Panel || ctrl is SplitContainer || ctrl is TableLayoutPanel || ctrl is FlowLayoutPanel)
                {
                    ctrl.BackColor = palette.Background;
                    ctrl.ForeColor = palette.TextPrimary;
                }

                if (ctrl.HasChildren)
                {
                    ApplyThemeRecursive(ctrl, palette);
                }
            }
        }

        public static void ApplyImmersiveDarkMode(IntPtr hwnd, bool enableDark)
        {
            if (hwnd == IntPtr.Zero || !RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return;

            try
            {
                int val = enableDark ? 1 : 0;
                if (DwmSetWindowAttribute(hwnd, DWMWA_USE_IMMERSIVE_DARK_MODE, ref val, sizeof(int)) != 0)
                {
                    DwmSetWindowAttribute(hwnd, DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1, ref val, sizeof(int));
                }
            }
            catch { }
        }
    }

    public static class ThemeManager
    {
        public static bool IsDarkModeEnabled => ThemeEngine.IsDarkModeActive;
        public static ThemePalette Palette => ThemeEngine.CurrentPalette;
        public static void ApplyTheme(Form form) => ThemeEngine.ApplyThemeToForm(form);
    }
}

namespace BulkCrapUninstaller.Forms.Windows
{
    internal static class ThemeManager
    {
        public static bool IsDarkModeEnabled => BulkCrapUninstaller.Functions.ThemeEngine.IsDarkModeActive;
        public static BulkCrapUninstaller.Functions.ThemePalette Palette => BulkCrapUninstaller.Functions.ThemeEngine.CurrentPalette;
        public static void ApplyTheme(Form form) => BulkCrapUninstaller.Functions.ThemeEngine.ApplyThemeToForm(form);
    }
}

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;
using BrightIdeasSoftware;
using Klocman.Forms;

namespace BulkCrapUninstaller.Themes
{
    public enum AppTheme
    {
        Default,
        Light,
        Dark
    }

    public static class ThemeManager
    {
        public static ThemePalette CurrentPalette { get; private set; }

        private static readonly System.Collections.Generic.HashSet<Form> ThemedForms = new System.Collections.Generic.HashSet<Form>();
        private static bool _globalThemingHooked;

        public static void ApplyTheme(string themeName)
        {
            if (Enum.TryParse(themeName, out AppTheme theme))
            {
                ApplyTheme(theme);
            }
            else
            {
                ApplyTheme(AppTheme.Default);
            }
        }

        public static void ApplyTheme(AppTheme theme)
        {
            // Respect OS High Contrast: don't override the accessibility theme with custom painting.
            if (SystemInformation.HighContrast)
            {
                CurrentPalette = new SystemThemePalette();

                try
                {
                    CustomMessageBox.ApplyThemeCallback = d => d.ThemeDialog(
                        CurrentPalette.WindowBackground,
                        CurrentPalette.ControlBackground,
                        CurrentPalette.TextPrimary,
                        CurrentPalette.TextSecondary,
                        CurrentPalette.Border,
                        CurrentPalette.Accent,
                        CurrentPalette.ButtonBackground,
                        CurrentPalette.ButtonForeground,
                        CurrentPalette.DisabledText);
                }
                catch
                {
                    // KlocTools types not available in every build configuration
                }

                ToolStripManager.Renderer = new ThemedToolStripRenderer(CurrentPalette);
                ThemedForms.Clear();
                if (!_globalThemingHooked)
                {
                    Application.Idle += ThemeManager_Idle;
                    _globalThemingHooked = true;
                }

                foreach (Form form in Application.OpenForms)
                {
                    UpdateForm(form);
                    ThemedForms.Add(form);
                    form.FormClosed -= Form_FormClosed;
                    form.FormClosed += Form_FormClosed;
                }

                return;
            }

            bool isDark = false;
            
            if (theme == AppTheme.Dark)
            {
                isDark = true;
            }
            else if (theme == AppTheme.Light)
            {
                isDark = false;
            }
            else
            {
                // Detect System
                isDark = IsSystemDark();
            }

            // Set Color Mode (NET 9+)
            try
            {
                Application.SetColorMode(isDark ? SystemColorMode.Dark : SystemColorMode.Classic);
            }
            catch
            {
                // Fallback or ignore if API missing
            }

            CurrentPalette = isDark ? new DarkThemePalette() : new LightThemePalette();

            // Theme the shared KlocTools dialogs (CustomMessageBox) using the active palette
            try
            {
                CustomMessageBox.ApplyThemeCallback = d => d.ThemeDialog(
                    CurrentPalette.WindowBackground,
                    CurrentPalette.ControlBackground,
                    CurrentPalette.TextPrimary,
                    CurrentPalette.TextSecondary,
                    CurrentPalette.Border,
                    CurrentPalette.Accent,
                    CurrentPalette.ButtonBackground,
                    CurrentPalette.ButtonForeground,
                    CurrentPalette.DisabledText);
            }
            catch
            {
                // KlocTools types not available in every build configuration
            }

            // Update ToolStrip Renderer
            ToolStripManager.Renderer = new ThemedToolStripRenderer(CurrentPalette);

            // Clear themed forms cache to force re-theming
            ThemedForms.Clear();

            // Hook global theming if not already done
            if (!_globalThemingHooked)
            {
                Application.Idle += ThemeManager_Idle;
                _globalThemingHooked = true;
            }

            // Update Open Forms (Title Bar and Controls)
            foreach (Form form in Application.OpenForms)
            {
                UpdateForm(form);
                ThemedForms.Add(form);
                form.FormClosed -= Form_FormClosed;
                form.FormClosed += Form_FormClosed;
            }
        }

        private static void ThemeManager_Idle(object sender, EventArgs e)
        {
            if (CurrentPalette == null) return;

            int count = Application.OpenForms.Count;
            if (count == 0) return;

            var forms = new Form[count];
            for (int i = 0; i < count; i++)
            {
                forms[i] = Application.OpenForms[i];
            }

            foreach (Form form in forms)
            {
                if (form != null && !ThemedForms.Contains(form))
                {
                    UpdateForm(form);
                    ThemedForms.Add(form);
                    form.FormClosed -= Form_FormClosed;
                    form.FormClosed += Form_FormClosed;
                }
            }
        }

        private static void Form_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (sender is Form form)
            {
                ThemedForms.Remove(form);
            }
        }
        
        public static void UpdateForm(Form form)
        {
            if (CurrentPalette == null) return;

            // Apply DWM Dark Mode to Title Bar
            if (Environment.OSVersion.Version.Major >= 10)
            {
                int useImmersiveDarkMode = CurrentPalette is DarkThemePalette ? 1 : 0;
                DwmSetWindowAttribute(form.Handle, DWMWA_USE_IMMERSIVE_DARK_MODE, ref useImmersiveDarkMode, sizeof(int));
                
                // Set Caption Color (Windows 11 Build 22000+)
                int captionColor = ColorTranslator.ToWin32(CurrentPalette.WindowBackground);
                DwmSetWindowAttribute(form.Handle, DWMWA_CAPTION_COLOR, ref captionColor, sizeof(int));
                
                int textColor = ColorTranslator.ToWin32(CurrentPalette.TextPrimary);
                DwmSetWindowAttribute(form.Handle, DWMWA_TEXT_COLOR, ref textColor, sizeof(int));

                // Set Border Color
                int borderColor = ColorTranslator.ToWin32(CurrentPalette.Border);
                DwmSetWindowAttribute(form.Handle, DWMWA_BORDER_COLOR, ref borderColor, sizeof(int));
            }

            // Apply Theme to Controls recursively
            ApplyThemeToControl(form);

            // Force repaint of all controls to apply new colors
            form.Invalidate(true);
        }

        private static void ApplyThemeToControl(Control c)
        {
            if (CurrentPalette == null) return;

            // Klocman SearchBox Custom Control
            if (c is Klocman.Controls.SearchBox sb)
            {
                sb.BackColor = CurrentPalette.WindowBackground;
                sb.ForeColor = CurrentPalette.TextPrimary;
                sb.NormalSearchColor = CurrentPalette.TextPrimary;
                sb.InactiveSearchColor = CurrentPalette.TextSecondary;
                
                sb.Paint -= SearchBox_Paint;
                sb.Paint += SearchBox_Paint;
            }
            // Form specific
            else if (c is Form f)
            {
                f.BackColor = CurrentPalette.WindowBackground;
                f.ForeColor = CurrentPalette.TextPrimary;
            }

            // UserControl (Sidebars, Filters, etc.) - Use ControlBackground to distinguish from Window
            else if (c is UserControl uc)
            {
                uc.BackColor = CurrentPalette.ControlBackground;
                uc.ForeColor = CurrentPalette.TextPrimary;
            }

            // Container types - apply background if not transparent
            // TabPage - Match WindowBackground to blend with parent container and active tab header
            else if (c is TabPage tp)
            {
                tp.BackColor = CurrentPalette.WindowBackground;
                tp.ForeColor = CurrentPalette.TextPrimary;
                tp.UseVisualStyleBackColor = false;
            }
            // Container types - apply background if not transparent
            else if (c is Panel || c is FlowLayoutPanel || c is TableLayoutPanel || c is SplitContainer)
            {
                if (c.BackColor != Color.Transparent)
                {
                    c.BackColor = CurrentPalette.ControlBackground; // Changed from WindowBackground to blend with parent controls
                }
                c.ForeColor = CurrentPalette.TextPrimary;

                // SplitContainer's splitter bar and embedded panels need explicit colours
                if (c is SplitContainer split)
                {
                    split.Panel1.BackColor = CurrentPalette.ControlBackground;
                    split.Panel2.BackColor = CurrentPalette.ControlBackground;
                }
            }

            // ObjectListView
            if (c is ObjectListView olv)
            {
                olv.HeaderUsesThemes = false;
                var style = new HeaderFormatStyle();
                style.SetBackColor(CurrentPalette.HeaderBackground);
                style.SetForeColor(CurrentPalette.TextPrimary); // Use brighter text for header
                olv.HeaderFormatStyle = style;
                
                olv.BackColor = CurrentPalette.ListBackground;
                olv.ForeColor = CurrentPalette.ListForeground;
                olv.AlternateRowBackColor = CurrentPalette.ListBackground; // No zebra striping to match user expectation (flat look)
                
                // Selection colors for dark/light mode readability
                olv.SelectedBackColor = CurrentPalette.SelectionBackground;
                olv.SelectedForeColor = CurrentPalette.SelectionForeground;
                olv.UnfocusedSelectedBackColor = CurrentPalette.ControlBackground;
                olv.UnfocusedSelectedForeColor = CurrentPalette.TextSecondary;

                olv.UseTranslucentSelection = true;
                olv.UseExplorerTheme = true; // Enable Explorer theme to allow native dark mode groups

                // Show grid lines only in light mode (too jarring in dark mode)
                olv.GridLines = CurrentPalette is LightThemePalette;
            }
            // TabControl
            else if (c is TabControl tab)
            {
                tab.DrawMode = TabDrawMode.OwnerDrawFixed;
                tab.DrawItem -= TabControl_DrawItem;
                tab.DrawItem += TabControl_DrawItem;
                // tab.BackColor = CurrentPalette.TabBackground; // Often doesn't work well, but let's try
            }
            // Button
            else if (c is Button btn)
            {
                // Only style standard buttons, avoid messing with custom ones if possible
                if (btn.FlatStyle == FlatStyle.Standard || btn.FlatStyle == FlatStyle.System || btn.FlatStyle == FlatStyle.Flat)
                {
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderColor = CurrentPalette.Border;
                    btn.FlatAppearance.MouseDownBackColor = CurrentPalette.Accent;
                    btn.FlatAppearance.MouseOverBackColor = ControlPaint.Light(CurrentPalette.Accent);
                    btn.BackColor = CurrentPalette.ButtonBackground;
                    btn.ForeColor = CurrentPalette.ButtonForeground;
                }
            }
            // Text Inputs
            else if (c is TextBox tb)
            {
                tb.BackColor = CurrentPalette.ControlBackground;
                tb.ForeColor = CurrentPalette.TextPrimary;
                tb.BorderStyle = BorderStyle.FixedSingle;
            }
            else if (c is NumericUpDown nud)
            {
                nud.BackColor = CurrentPalette.ControlBackground;
                nud.ForeColor = CurrentPalette.TextPrimary;
                nud.BorderStyle = BorderStyle.FixedSingle;
            }
            // ComboBox (Needs OwnerDraw for Dropdown) - must come before ListControl
            // because ComboBox derives from ListControl and would otherwise be consumed
            // by the generic ListControl branch below.
            else if (c is ComboBox combo)
            {
                combo.BackColor = CurrentPalette.ControlBackground;
                combo.ForeColor = CurrentPalette.TextPrimary;
                combo.FlatStyle = FlatStyle.Flat;
                
                combo.DrawMode = DrawMode.OwnerDrawFixed;
                combo.DrawItem -= ComboBox_DrawItem;
                combo.DrawItem += ComboBox_DrawItem;
            }
            else if (c is ListBox || c is ListControl)
            {
                c.BackColor = CurrentPalette.ControlBackground;
                c.ForeColor = CurrentPalette.TextPrimary;
            }
            // LinkLabel - default blue links are unreadable on dark backgrounds
            else if (c is LinkLabel link)
            {
                link.ForeColor = CurrentPalette.TextPrimary;
                link.LinkColor = CurrentPalette.LinkColor;
                link.VisitedLinkColor = CurrentPalette.LinkVisitedColor;
                link.ActiveLinkColor = CurrentPalette.LinkColor;
            }
            // Labels and Checkboxes
            else if (c is Label || c is CheckBox || c is RadioButton)
            {
                c.ForeColor = CurrentPalette.TextPrimary;
                // Transparent background is usually best for these controls
            }
            // Splitter - inherits system 3D colour, stays light in dark mode
            else if (c is Splitter splitter)
            {
                splitter.BackColor = CurrentPalette.ControlBackground;
            }
            // ProgressBar - track stays light in dark mode
            else if (c is ProgressBar progress)
            {
                progress.BackColor = CurrentPalette.ControlBackground;
                progress.ForeColor = CurrentPalette.Accent;
            }
            // GroupBox - Set border color to the subtle palette border.
            // Text is custom-painted to prevent fuzzy/double drawing and to ensure readability.
            else if (c is GroupBox gb)
            {
                gb.FlatStyle = FlatStyle.Flat;
                gb.ForeColor = CurrentPalette.Border;
                
                gb.Paint -= GroupBox_Paint;
                gb.Paint += GroupBox_Paint;
            }

            // Apply native dark scrollbars on Windows 10+
            if (Environment.OSVersion.Version.Major >= 10 && CurrentPalette != null)
            {
                if (c is TextBoxBase || c is ListBox || c is ListView || c is TreeView)
                {
                    try
                    {
                        string theme = CurrentPalette is DarkThemePalette ? "DarkMode_Explorer" : "explorer";
                        SetWindowTheme(c.Handle, theme, null);
                    }
                    catch { }
                }
            }

            // Recurse
            foreach (Control child in c.Controls)
            {
                ApplyThemeToControl(child);
            }
        }

        private static void ComboBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (CurrentPalette == null || !(sender is ComboBox combo) || e.Index < 0) return;

            string text = combo.GetItemText(combo.Items[e.Index]);
            bool isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

            var backColor = isSelected ? CurrentPalette.SelectionBackground : CurrentPalette.ControlBackground;
            var foreColor = isSelected ? CurrentPalette.SelectionForeground : CurrentPalette.TextPrimary;

            using (var brush = new SolidBrush(backColor))
            {
                e.Graphics.FillRectangle(brush, e.Bounds);
            }

            TextRenderer.DrawText(e.Graphics, text, combo.Font, e.Bounds, foreColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
        }

        private static void SearchBox_Paint(object sender, PaintEventArgs e)
        {
            if (CurrentPalette == null || !(sender is Control c)) return;
            using (var pen = new Pen(CurrentPalette.Border))
            {
                e.Graphics.DrawRectangle(pen, 0, 0, c.Width - 1, c.Height - 1);
            }
        }

        private static void TabControl_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (CurrentPalette == null || !(sender is TabControl tab) || e.Index < 0) return;

            var page = tab.TabPages[e.Index];
            var bounds = e.Bounds;
            
            bool isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            
            var backColor = isSelected ? CurrentPalette.TabSelectedBackground : CurrentPalette.TabUnselectedBackground;
            var foreColor = isSelected ? CurrentPalette.TabSelectedForeground : CurrentPalette.TabUnselectedForeground;
            
            using (var brush = new SolidBrush(backColor))
            {
                e.Graphics.FillRectangle(brush, bounds);
            }
            
            if (isSelected)
            {
                // Draw a 3px accent line at the top of the active tab
                var accentRect = new Rectangle(bounds.X, bounds.Y, bounds.Width, 3);
                using (var accentBrush = new SolidBrush(CurrentPalette.Accent))
                {
                    e.Graphics.FillRectangle(accentBrush, accentRect);
                }
            }
            else
            {
                // Draw a subtle border around inactive tabs to keep tab structure defined
                using (var pen = new Pen(CurrentPalette.Border))
                {
                    e.Graphics.DrawRectangle(pen, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
                }
            }
            
            TextRenderer.DrawText(e.Graphics, page.Text, e.Font, bounds, foreColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        private static void GroupBox_Paint(object sender, PaintEventArgs e)
        {
            if (CurrentPalette == null || !(sender is GroupBox gb)) return;

            string text = gb.Text;
            if (string.IsNullOrEmpty(text)) return;

            // Measure the text size
            Size textSize = TextRenderer.MeasureText(e.Graphics, text, gb.Font);

            // Erase the default text area (drawn at X=9 in gb.ForeColor) to avoid double/fuzzy text
            var rect = new Rectangle(8, 0, textSize.Width + 4, textSize.Height);
            using (var brush = new SolidBrush(gb.BackColor))
            {
                e.Graphics.FillRectangle(brush, rect);
            }

            // Draw the text cleanly in TextPrimary color
            TextRenderer.DrawText(e.Graphics, text, gb.Font, new Point(10, 0), CurrentPalette.TextPrimary);
        }

        private static bool IsSystemDark()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize"))
                {
                    if (key != null)
                    {
                        var val = key.GetValue("AppsUseLightTheme");
                        if (val is int i)
                        {
                            return i == 0;
                        }
                    }
                }
            }
            catch { }
            return false; // Default to Light
        }

        // P/Invoke
        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        [DllImport("uxtheme.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string pszSubIdList);

        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
        private const int DWMWA_BORDER_COLOR = 34;
        private const int DWMWA_CAPTION_COLOR = 35;
        private const int DWMWA_TEXT_COLOR = 36;
    }
}

using System.Drawing;

namespace BulkCrapUninstaller.Themes
{
    public class DarkThemePalette : ThemePalette
    {
        // Human Factors: Soft Black background to reduce eye strain (not pure black #000000)
        public override Color WindowBackground => ColorTranslator.FromHtml("#121212");
        
        // Control Background: Slightly lighter for visual hierarchy (Sidebars, Panels)
        public override Color ControlBackground => ColorTranslator.FromHtml("#1F1F1F");
        
        // Text Primary: Off-white to avoid halation/astigmatism strain (was #D4D4D4)
        public override Color TextPrimary => ColorTranslator.FromHtml("#E0E0E0");
        
        // Text Secondary: Muted for less important info
        public override Color TextSecondary => ColorTranslator.FromHtml("#A0A0A0");
        
        // Accent: Desaturated Blue to prevent chromatic vibration (was #007ACC)
        public override Color Accent => ColorTranslator.FromHtml("#4D90FE");
        
        // Border: Subtle dark gray
        public override Color Border => ColorTranslator.FromHtml("#333333");

        // Selection Background: Dark Blue for readable white text
        public override Color SelectionBackground => ColorTranslator.FromHtml("#264F78");
        
        // Status Strip: Matches Control Background for seamless look
        public override Color StatusStripBackground => ControlBackground;

        // Button: Matches Control Background
        public override Color ButtonBackground => ControlBackground;
        public override Color ButtonForeground => TextPrimary;

        // List / Grid Colors
        public override Color ListBackground => WindowBackground;
        public override Color ListForeground => TextPrimary;
        public override Color HeaderBackground => ControlBackground;
        public override Color HeaderForeground => TextSecondary;

        // TabControl Colors
        public override Color TabBackground => ControlBackground;
        public override Color TabSelectedBackground => WindowBackground;
        public override Color TabUnselectedBackground => ColorTranslator.FromHtml("#2D2D2D");
        public override Color TabSelectedForeground => Color.White; // High contrast for active tab
        public override Color TabUnselectedForeground => TextSecondary;

        // Link colors — desaturated for dark mode, WCAG AA on #121212
        public override Color LinkColor => ColorTranslator.FromHtml("#6DB3F2"); // 5.2:1 contrast
        public override Color LinkVisitedColor => ColorTranslator.FromHtml("#B39DDB"); // 4.8:1 contrast

        // State colors
        public override Color DisabledText => ColorTranslator.FromHtml("#666666");

        // Structural colors
        public override Color GroupBoxBorder => ColorTranslator.FromHtml("#3A3A3A");
        public override Color SeparatorColor => ColorTranslator.FromHtml("#2A2A2A");
        public override Color ButtonHoverBackground => ColorTranslator.FromHtml("#2D4A6F");
    }
}

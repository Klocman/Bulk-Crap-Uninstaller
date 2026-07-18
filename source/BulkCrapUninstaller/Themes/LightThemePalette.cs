using System.Drawing;

namespace BulkCrapUninstaller.Themes
{
    public class LightThemePalette : ThemePalette
    {
        public override Color WindowBackground => ColorTranslator.FromHtml("#FFFFFF");
        public override Color ControlBackground => ColorTranslator.FromHtml("#F3F3F3");
        public override Color TextPrimary => ColorTranslator.FromHtml("#000000");
        public override Color TextSecondary => ColorTranslator.FromHtml("#333333");
        public override Color Accent => ColorTranslator.FromHtml("#007ACC");
        public override Color Border => ColorTranslator.FromHtml("#CECECE");
        public override Color StatusStripBackground => WindowBackground;

        public override Color SelectionBackground => ColorTranslator.FromHtml("#ADD6FF");
        public override Color SelectionForeground => Color.Black;

        // List / Grid Colors
        public override Color ListBackground => WindowBackground;
        public override Color ListForeground => TextPrimary;
        public override Color HeaderBackground => ControlBackground;
        public override Color HeaderForeground => TextSecondary;

        // TabControl Colors
        public override Color TabBackground => ControlBackground;
        public override Color TabSelectedBackground => WindowBackground;
        public override Color TabUnselectedBackground => ColorTranslator.FromHtml("#ECECEC");
        public override Color TabSelectedForeground => TextPrimary; // Active tab should be most prominent
        public override Color TabUnselectedForeground => ColorTranslator.FromHtml("#666666");

        // Link colors — accessible blue/purple on white
        public override Color LinkColor => ColorTranslator.FromHtml("#0066CC");
        public override Color LinkVisitedColor => ColorTranslator.FromHtml("#6B3FA0");

        // State colors
        public override Color DisabledText => ColorTranslator.FromHtml("#999999");

        // Structural colors
        public override Color GroupBoxBorder => ColorTranslator.FromHtml("#D0D0D0");
        public override Color SeparatorColor => ColorTranslator.FromHtml("#E0E0E0");
        public override Color ButtonHoverBackground => ColorTranslator.FromHtml("#E0EDF8");
    }
}

using System.Drawing;
using System.Windows.Forms;

namespace BulkCrapUninstaller.Themes
{
    /// <summary>
    ///     Palette backed by OS system colors. Used when the user has High Contrast
    ///     enabled so we don't override the accessibility theme with custom painting.
    /// </summary>
    public class SystemThemePalette : ThemePalette
    {
        public override Color WindowBackground => SystemColors.Window;
        public override Color ControlBackground => SystemColors.Control;
        public override Color TextPrimary => SystemColors.WindowText;
        public override Color TextSecondary => SystemColors.GrayText;
        public override Color Accent => SystemColors.Highlight;
        public override Color Border => SystemColors.ControlDark;
        public override Color StatusStripBackground => SystemColors.Control;

        public override Color SelectionBackground => SystemColors.Highlight;
        public override Color SelectionForeground => SystemColors.HighlightText;

        public override Color ButtonBackground => SystemColors.Control;
        public override Color ButtonForeground => SystemColors.ControlText;

        public override Color ListBackground => SystemColors.Window;
        public override Color ListForeground => SystemColors.WindowText;
        public override Color HeaderBackground => SystemColors.Control;
        public override Color HeaderForeground => SystemColors.GrayText;

        public override Color TabSelectedBackground => SystemColors.Window;
        public override Color TabUnselectedBackground => SystemColors.Control;
        public override Color TabSelectedForeground => SystemColors.WindowText;
        public override Color TabUnselectedForeground => SystemColors.GrayText;

        public override Color LinkColor => SystemColors.HotTrack;
        public override Color LinkVisitedColor => SystemColors.HotTrack;

        public override Color DisabledText => SystemColors.GrayText;

        public override Color GroupBoxBorder => SystemColors.ControlDark;
        public override Color SeparatorColor => SystemColors.ControlDark;
        public override Color ButtonHoverBackground => SystemColors.ControlDark;
    }
}

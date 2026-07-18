using System.Drawing;
using System.Windows.Forms;

namespace BulkCrapUninstaller.Themes
{
    public abstract class ThemePalette
    {
        public abstract Color WindowBackground { get; }
        public abstract Color ControlBackground { get; }
        public abstract Color TextPrimary { get; }
        public abstract Color TextSecondary { get; }
        public abstract Color Accent { get; }
        public abstract Color Border { get; }
        public abstract Color StatusStripBackground { get; }
        
        public virtual Color SelectionBackground => Accent;
        public virtual Color SelectionForeground => Color.White;
        
        public virtual Color ButtonBackground => ControlBackground;
        public virtual Color ButtonForeground => TextPrimary;

        // List / Grid Colors
        public virtual Color ListBackground => WindowBackground;
        public virtual Color ListForeground => TextPrimary;
        public virtual Color HeaderBackground => ControlBackground;
        public virtual Color HeaderForeground => TextSecondary;

        // TabControl Colors
        public virtual Color TabBackground => ControlBackground;
        public virtual Color TabSelectedBackground => WindowBackground;
        public virtual Color TabUnselectedBackground => ControlBackground;
        public virtual Color TabSelectedForeground => TextPrimary;
        public virtual Color TabUnselectedForeground => TextSecondary;

        // Link colors
        public virtual Color LinkColor => Accent;
        public virtual Color LinkVisitedColor => Accent;

        // State colors
        public virtual Color DisabledText => TextSecondary;
        public virtual Color ErrorText => Color.FromArgb(0xCF, 0x66, 0x79); // Desaturated red

        // Structural colors
        public virtual Color GroupBoxBorder => Border;
        public virtual Color SeparatorColor => Border;
        public virtual Color ButtonHoverBackground => ControlPaint.Light(Accent);
    }
}

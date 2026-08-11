using System.Drawing;
using System.Windows.Forms;

namespace BulkCrapUninstaller.Themes
{
    public class ThemedToolStripRenderer : ToolStripProfessionalRenderer
    {
        private readonly ThemePalette _palette;

        public ThemedToolStripRenderer(ThemePalette palette) : base(new ThemedColorTable(palette))
        {
            _palette = palette;
            RoundedEdges = false;
        }

        // Force all ToolStrip item text to use palette TextPrimary, or DisabledText when greyed out
        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            e.TextColor = e.Item.Enabled ? _palette.TextPrimary : _palette.DisabledText;
            base.OnRenderItemText(e);
        }

        // Draw separators with palette SeparatorColor instead of OS default (fixes M4)
        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            var bounds = new Rectangle(Point.Empty, e.Item.Size);

            if (e.Vertical)
            {
                int x = bounds.Width / 2;
                using var pen = new Pen(_palette.SeparatorColor);
                e.Graphics.DrawLine(pen, x, bounds.Top + 3, x, bounds.Bottom - 3);
            }
            else
            {
                int y = bounds.Height / 2;
                using var pen = new Pen(_palette.SeparatorColor);
                e.Graphics.DrawLine(pen, bounds.Left + 4, y, bounds.Right - 4, y);
            }
        }

        // Invert ToolStrip and Menu icons in dark mode to make them visible (light gray/white)
        protected override void OnRenderItemImage(ToolStripItemImageRenderEventArgs e)
        {
            if (e.Image == null) return;

            if (_palette is DarkThemePalette)
            {
                float alpha = e.Item.Enabled ? 1.0f : 0.35f;
                using (var imageAttributes = new System.Drawing.Imaging.ImageAttributes())
                {
                    float[][] colorMatrixElements = {
                        new float[] {-1,  0,  0,  0,  0},
                        new float[] { 0, -1,  0,  0,  0},
                        new float[] { 0,  0, -1,  0,  0},
                        new float[] { 0,  0,  0,  alpha,  0},
                        new float[] { 1,  1,  1,  0,  1}
                    };
                    var colorMatrix = new System.Drawing.Imaging.ColorMatrix(colorMatrixElements);
                    imageAttributes.SetColorMatrix(colorMatrix, System.Drawing.Imaging.ColorMatrixFlag.Default, System.Drawing.Imaging.ColorAdjustType.Bitmap);

                    e.Graphics.DrawImage(
                        e.Image,
                        e.ImageRectangle,
                        0, 0, e.Image.Width, e.Image.Height,
                        GraphicsUnit.Pixel,
                        imageAttributes);
                }
            }
            else
            {
                base.OnRenderItemImage(e);
            }
        }

        private class ThemedColorTable : ProfessionalColorTable
        {
            private readonly ThemePalette _palette;

            public ThemedColorTable(ThemePalette palette)
            {
                _palette = palette;
                UseSystemColors = false;
            }

            public override Color ToolStripDropDownBackground => _palette.ControlBackground;
            public override Color ImageMarginGradientBegin => _palette.ControlBackground;
            public override Color ImageMarginGradientMiddle => _palette.ControlBackground;
            public override Color ImageMarginGradientEnd => _palette.ControlBackground;
            public override Color MenuBorder => _palette.Border;
            public override Color MenuItemBorder => _palette.Border;
            public override Color MenuItemSelected => _palette.SelectionBackground;
            public override Color MenuItemSelectedGradientBegin => _palette.SelectionBackground;
            public override Color MenuItemSelectedGradientEnd => _palette.SelectionBackground;
            public override Color MenuItemPressedGradientBegin => _palette.SelectionBackground;
            public override Color MenuItemPressedGradientMiddle => _palette.SelectionBackground;
            public override Color MenuItemPressedGradientEnd => _palette.SelectionBackground;
            
            public override Color StatusStripGradientBegin => _palette.StatusStripBackground;
            public override Color StatusStripGradientEnd => _palette.StatusStripBackground;
            
            public override Color ToolStripGradientBegin => _palette.WindowBackground;
            public override Color ToolStripGradientMiddle => _palette.WindowBackground;
            public override Color ToolStripGradientEnd => _palette.WindowBackground;
            public override Color ToolStripBorder => _palette.Border;

            public override Color SeparatorDark => _palette.SeparatorColor;
            public override Color SeparatorLight => _palette.SeparatorColor;
        }
    }
}

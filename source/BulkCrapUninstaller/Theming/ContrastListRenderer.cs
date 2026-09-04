using System;
using System.Drawing;
using System.Windows.Forms;
using BrightIdeasSoftware;

namespace BulkCrapUninstaller.Theming;

// Keep ObjectListView's filter highlighting and primary-checkbox geometry.
// Its normal CheckBoxRenderer path can retain dark themed glyphs in contrast mode.
internal sealed class ContrastListRenderer : HighlightTextRenderer
{
    protected override int DrawCheckBox(Graphics graphics, Rectangle bounds)
    {
        if (!SystemInformation.HighContrast || IsPrinting || UseCustomCheckboxImages
            || ListView.View != View.Details)
            return base.DrawCheckBox(graphics, bounds);

        var size = CalculateCheckBoxSize(graphics);
        var box = new Rectangle(bounds.X, AlignVertically(bounds, size.Height), size.Width, size.Height);
        var disabled = !ListView.Enabled || IsCheckBoxDisabled;
        var ink = disabled ? SystemColors.GrayText : SystemColors.WindowText;
        var border = !disabled && IsCheckboxHot ? SystemColors.HotTrack : ink;

        // Read the palette on each paint: no theme handle or colored bitmap cache.
        using (var fill = new SolidBrush(SystemColors.Window)) graphics.FillRectangle(fill, box);
        if (ListItem.CheckState == CheckState.Checked)
            ControlPaint.DrawMenuGlyph(graphics, box, MenuGlyph.Checkmark, ink, SystemColors.Window);
        else if (ListItem.CheckState == CheckState.Indeterminate)
        {
            var inset = Math.Max(2, size.Width / 4);
            using var fill = new SolidBrush(ink);
            graphics.FillRectangle(fill, Rectangle.Inflate(box, -inset, -inset));
        }

        var stroke = Math.Max(1, (int)Math.Round(graphics.DpiX / 96f));
        ControlPaint.DrawBorder(graphics, box,
            border, stroke, ButtonBorderStyle.Solid, border, stroke, ButtonBorderStyle.Solid,
            border, stroke, ButtonBorderStyle.Solid, border, stroke, ButtonBorderStyle.Solid);
        return size.Width;
    }
}

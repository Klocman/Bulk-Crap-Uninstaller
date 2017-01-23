/*
 * Overlays - Images, text or other things that can be rendered over the top of a ListView
 *
 * Author: Phillip Piper
 * Date: 14/04/2009 4:36 PM
 *
 * Change log:
 * v2.3
 * 2009-08-17   JPP  - Overlays now use Adornments
 *                   - Added ITransparentOverlay interface. Overlays can now have separate transparency levels
 * 2009-08-10   JPP  - Moved decoration related code to new file
 * v2.2.1
 * 200-07-24    JPP  - TintedColumnDecoration now works when last item is a member of a collapsed
 *                     group (well, it no longer crashes).
 * v2.2
 * 2009-06-01   JPP  - Make sure that TintedColumnDecoration reaches to the last item in group view
 * 2009-05-05   JPP  - Unified BillboardOverlay text rendering with that of TextOverlay
 * 2009-04-30   JPP  - Added TintedColumnDecoration
 * 2009-04-14   JPP  - Initial version
 *
 * To do:
 *
 * Copyright (C) 2009-2014 Phillip Piper
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 * If you wish to use this code in a closed source application, please contact phillip.piper@gmail.com.
 */

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace BrightIdeasSoftware
{
    /// <summary>
    /// The interface for an object which can draw itself over the top of
    /// an ObjectListView.
    /// </summary>
    public interface IOverlay
    {
        /// <summary>
        /// Draw this overlay
        /// </summary>
        /// <param name="olv">The ObjectListView that is being overlaid</param>
        /// <param name="g">The Graphics onto the given OLV</param>
        /// <param name="r">The content area of the OLV</param>
        void Draw(ObjectListView olv, Graphics g, Rectangle r);
    }

    /// <summary>
    /// An interface for an overlay that supports variable levels of transparency
    /// </summary>
    public interface ITransparentOverlay : IOverlay
    {
        /// <summary>
        /// Gets or sets the transparency of the overlay. 
        /// 0 is completely transparent, 255 is completely opaque.
        /// </summary>
        int Transparency { get; set; }
    }

    /// <summary>
    /// A null implementation of the IOverlay interface
    /// </summary>
    public class AbstractOverlay : ITransparentOverlay
    {
        #region IOverlay Members

        /// <summary>
        /// Draw this overlay
        /// </summary>
        /// <param name="olv">The ObjectListView that is being overlaid</param>
        /// <param name="g">The Graphics onto the given OLV</param>
        /// <param name="r">The content area of the OLV</param>
        public virtual void Draw(ObjectListView olv, Graphics g, Rectangle r) {
        }

        #endregion

        #region ITransparentOverlay Members

        /// <summary>
        /// How transparent should this overlay be?
        /// </summary>
        [Category("ObjectListView"),
         Description("How transparent should this overlay be"),
         DefaultValue(128),
         NotifyParentProperty(true)]
        public int Transparency {
            get { return this.transparency; }
            set { this.transparency = Math.Min(255, Math.Max(0, value)); }
        }
        private int transparency = 128;

        #endregion
    }
    
    /// <summary>
    /// An overlay that will draw an image over the top of the ObjectListView
    /// </summary>
    [TypeConverter("BrightIdeasSoftware.Design.OverlayConverter")]
    public class ImageOverlay : ImageAdornment, ITransparentOverlay
    {
        /// <summary>
        /// Create an ImageOverlay
        /// </summary>
        public ImageOverlay() {
            this.Alignment = System.Drawing.ContentAlignment.BottomRight;
        }

        #region Public properties

        /// <summary>
        /// Gets or sets the horizontal inset by which the position of the overlay will be adjusted
        /// </summary>
        [Category("ObjectListView"),
         Description("The horizontal inset by which the position of the overlay will be adjusted"),
         DefaultValue(20),
         NotifyParentProperty(true)]
        public int InsetX {
            get { return this.insetX; }
            set { this.insetX = Math.Max(0, value); }
        }
        private int insetX = 20;

        /// <summary>
        /// Gets or sets the vertical inset by which the position of the overlay will be adjusted
        /// </summary>
        [Category("ObjectListView"),
         Description("Gets or sets the vertical inset by which the position of the overlay will be adjusted"),
         DefaultValue(20),
         NotifyParentProperty(true)]
        public int InsetY {
            get { return this.insetY; }
            set { this.insetY = Math.Max(0, value); }
        }
        private int insetY = 20;

        #endregion

        #region Commands

        /// <summary>
        /// Draw this overlay
        /// </summary>
        /// <param name="olv">The ObjectListView being decorated</param>
        /// <param name="g">The Graphics used for drawing</param>
        /// <param name="r">The bounds of the rendering</param>
        public virtual void Draw(ObjectListView olv, Graphics g, Rectangle r) {
            Rectangle insetRect = r;
            insetRect.Inflate(-this.InsetX, -this.InsetY);

            // We hard code a transparency of 255 here since transparency is handled by the glass panel
            this.DrawImage(g, insetRect, this.Image, 255);
        }

        #endregion
    }

    /// <summary>
    /// An overlay that will draw text over the top of the ObjectListView
    /// </summary>
    [TypeConverter("BrightIdeasSoftware.Design.OverlayConverter")]
    public class TextOverlay : TextAdornment, ITransparentOverlay
    {
        /// <summary>
        /// Create a TextOverlay
        /// </summary>
        public TextOverlay() {
            this.Alignment = System.Drawing.ContentAlignment.BottomRight;
        }

        #region Public properties

        /// <summary>
        /// Gets or sets the horizontal inset by which the position of the overlay will be adjusted
        /// </summary>
        [Category("ObjectListView"),
         Description("The horizontal inset by which the position of the overlay will be adjusted"),
         DefaultValue(20),
         NotifyParentProperty(true)]
        public int InsetX {
            get { return this.insetX; }
            set { this.insetX = Math.Max(0, value); }
        }
        private int insetX = 20;

        /// <summary>
        /// Gets or sets the vertical inset by which the position of the overlay will be adjusted
        /// </summary>
        [Category("ObjectListView"),
         Description("Gets or sets the vertical inset by which the position of the overlay will be adjusted"),
         DefaultValue(20),
         NotifyParentProperty(true)]
        public int InsetY {
            get { return this.insetY; }
            set { this.insetY = Math.Max(0, value); }
        }
        private int insetY = 20;

        /// <summary>
        /// Gets or sets whether the border will be drawn with rounded corners
        /// </summary>
        [Browsable(false),
         Obsolete("Use CornerRounding instead", false),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool RoundCorneredBorder {
            get { return this.CornerRounding > 0; }
            set {
                if (value)
                    this.CornerRounding = 16.0f;
                else
                    this.CornerRounding = 0.0f;
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// Draw this overlay
        /// </summary>
        /// <param name="olv">The ObjectListView being decorated</param>
        /// <param name="g">The Graphics used for drawing</param>
        /// <param name="r">The bounds of the rendering</param>
        public virtual void Draw(ObjectListView olv, Graphics g, Rectangle r) {
            if (String.IsNullOrEmpty(this.Text))
                return;

            Rectangle insetRect = r;
            insetRect.Inflate(-this.InsetX, -this.InsetY);
            // We hard code a transparency of 255 here since transparency is handled by the glass panel
            this.DrawText(g, insetRect, this.Text, 255);
        }

        #endregion
    }

    /// <summary>
    /// A Billboard overlay is a TextOverlay positioned at an absolute point
    /// </summary>
    public class BillboardOverlay : TextOverlay
    {
        /// <summary>
        /// Create a BillboardOverlay
        /// </summary>
        public BillboardOverlay() {
            this.Transparency = 255;
            this.BackColor = Color.PeachPuff;
            this.TextColor = Color.Black;
            this.BorderColor = Color.Empty;
            this.Font = new Font("Tahoma", 10);
        }

        /// <summary>
        /// Gets or sets where should the top left of the billboard be placed
        /// </summary>
        public Point Location {
            get { return this.location; }
            set { this.location = value; }
        }
        private Point location;

        /// <summary>
        /// Draw this overlay
        /// </summary>
        /// <param name="olv">The ObjectListView being decorated</param>
        /// <param name="g">The Graphics used for drawing</param>
        /// <param name="r">The bounds of the rendering</param>
        public override void Draw(ObjectListView olv, Graphics g, Rectangle r) {
            if (String.IsNullOrEmpty(this.Text))
                return;

            // Calculate the bounds of the text, and then move it to where it should be
            Rectangle textRect = this.CalculateTextBounds(g, r, this.Text);
            textRect.Location = this.Location;

            // Make sure the billboard is within the bounds of the List, as far as is possible
            if (textRect.Right > r.Width)
                textRect.X = Math.Max(r.Left, r.Width - textRect.Width);
            if (textRect.Bottom > r.Height)
                textRect.Y = Math.Max(r.Top, r.Height - textRect.Height);

            this.DrawBorderedText(g, textRect, this.Text, 255);
        }
    }
}

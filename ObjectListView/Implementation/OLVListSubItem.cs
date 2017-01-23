/*
 * OLVListSubItem - A single cell in an ObjectListView
 *
 * Author: Phillip Piper
 * Date: 31-March-2011 5:53 pm
 *
 * Change log:
 * 2011-03-31  JPP  - Split into its own file
 * 
 * Copyright (C) 2011-2014 Phillip Piper
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
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;

namespace BrightIdeasSoftware {

    /// <summary>
    /// A ListViewSubItem that knows which image should be drawn against it.
    /// </summary>
    [Browsable(false)]
    public class OLVListSubItem : ListViewItem.ListViewSubItem {
        #region Constructors

        /// <summary>
        /// Create a OLVListSubItem
        /// </summary>
        public OLVListSubItem() {
        }

        /// <summary>
        /// Create a OLVListSubItem that shows the given string and image
        /// </summary>
        public OLVListSubItem(object modelValue, string text, Object image) {
            this.ModelValue = modelValue;
            this.Text = text;
            this.ImageSelector = image;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets how many pixels will be left blank around this cell
        /// </summary>
        /// <remarks>This setting only takes effect when the control is owner drawn.</remarks>
        public Rectangle? CellPadding {
            get { return this.cellPadding; }
            set { this.cellPadding = value; }
        }
        private Rectangle? cellPadding;

        /// <summary>
        /// Gets or sets how this cell will be vertically aligned
        /// </summary>
        /// <remarks>This setting only takes effect when the control is owner drawn.</remarks>
        public StringAlignment? CellVerticalAlignment {
            get { return this.cellVerticalAlignment; }
            set { this.cellVerticalAlignment = value; }
        }
        private StringAlignment? cellVerticalAlignment;

        /// <summary>
        /// Gets or sets the model value is being displayed by this subitem.
        /// </summary>
        public object ModelValue
        {
            get { return modelValue; }
            private set { modelValue = value; }
        }
        private object modelValue;

        /// <summary>
        /// Gets if this subitem has any decorations set for it.
        /// </summary>
        public bool HasDecoration {
            get {
                return this.decorations != null && this.decorations.Count > 0;
            }
        }

        /// <summary>
        /// Gets or sets the decoration that will be drawn over this item
        /// </summary>
        /// <remarks>Setting this replaces all other decorations</remarks>
        public IDecoration Decoration {
            get {
                return this.HasDecoration ? this.Decorations[0] : null;
            }
            set {
                this.Decorations.Clear();
                if (value != null)
                    this.Decorations.Add(value);
            }
        }

        /// <summary>
        /// Gets the collection of decorations that will be drawn over this item
        /// </summary>
        public IList<IDecoration> Decorations {
            get {
                if (this.decorations == null)
                    this.decorations = new List<IDecoration>();
                return this.decorations;
            }
        }
        private IList<IDecoration> decorations;

        /// <summary>
        /// Get or set the image that should be shown against this item
        /// </summary>
        /// <remarks><para>This can be an Image, a string or an int. A string or an int will
        /// be used as an index into the small image list.</para></remarks>
        public Object ImageSelector {
            get { return imageSelector; }
            set { imageSelector = value; }
        }
        private Object imageSelector;

        /// <summary>
        /// Gets or sets the url that should be invoked when this subitem is clicked
        /// </summary>
        public string Url
        {
            get { return this.url; }
            set { this.url = value; }
        }
        private string url;

        /// <summary>
        /// Gets or sets whether this cell is selected
        /// </summary>
        public bool Selected
        {
            get { return this.selected; }
            set { this.selected = value; }
        }
        private bool selected;

        #endregion

        #region Implementation Properties

        /// <summary>
        /// Return the state of the animatation of the image on this subitem.
        /// Null means there is either no image, or it is not an animation
        /// </summary>
        internal ImageRenderer.AnimationState AnimationState;

        #endregion
    }

}

/*
 * GlassPanelForm - A transparent form that is placed over an ObjectListView
 * to allow flicker-free overlay images during scrolling.
 *
 * Author: Phillip Piper
 * Date: 14/04/2009 4:36 PM
 *
 * Change log:
 * 2010-08-18   JPP  - Added WS_EX_TOOLWINDOW style so that the form won't appear in Alt-Tab list.
 * v2.4
 * 2010-03-11   JPP  - Work correctly in MDI applications -- more or less. Actually, less than more.
 *                     They don't crash but they don't correctly handle overlapping MDI children.
 *                     Overlays from one control are shown on top of other other windows.
 * 2010-03-09   JPP  - Correctly Unbind() when the related ObjectListView is disposed.
 * 2009-10-28   JPP  - Use FindForm() rather than TopMostControl, since the latter doesn't work
 *                     as I expected when the OLV is part of an MDI child window. Thanks to
 *                     wvd_vegt who tracked this down.
 * v2.3
 * 2009-08-19   JPP  - Only hide the glass pane on resize, not on move
 *                   - Each glass panel now only draws one overlays
 * v2.2
 * 2009-06-05   JPP  - Handle when owning window is a topmost window
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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace BrightIdeasSoftware
{
    /// <summary>
    /// A GlassPanelForm sits transparently over an ObjectListView to show overlays.
    /// </summary>
    internal partial class GlassPanelForm : Form
    {
        public GlassPanelForm() {
            this.Name = "GlassPanelForm";
            this.Text = "GlassPanelForm";

            ClientSize = new System.Drawing.Size(0, 0);
            ControlBox = false;
            FormBorderStyle = FormBorderStyle.None;
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.Manual;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowIcon = false;
            ShowInTaskbar = false;
            FormBorderStyle = FormBorderStyle.None;

            SetStyle(ControlStyles.Selectable, false);
            
            this.Opacity = 0.5f;
            this.BackColor = Color.FromArgb(255, 254, 254, 254);
            this.TransparencyKey = this.BackColor;
            this.HideGlass();
            NativeMethods.ShowWithoutActivate(this);
        }

        protected override void Dispose(bool disposing) {
            if (disposing)
                this.Unbind();

            base.Dispose(disposing);
        }

        #region Properties

        /// <summary>
        /// Get the low-level windows flag that will be given to CreateWindow.
        /// </summary>
        protected override CreateParams CreateParams {
            get {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x20; // WS_EX_TRANSPARENT
                cp.ExStyle |= 0x80; // WS_EX_TOOLWINDOW 
                return cp;
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// Attach this form to the given ObjectListView
        /// </summary>        
        public void Bind(ObjectListView olv, IOverlay overlay) {
            if (this.objectListView != null)
                this.Unbind();

            this.objectListView = olv;
            this.Overlay = overlay;
            this.mdiClient = null;
            this.mdiOwner = null;

            if (this.objectListView == null)
                return;

            // NOTE: If you listen to any events here, you *must* stop listening in Unbind()

            this.objectListView.Disposed += new EventHandler(objectListView_Disposed);
            this.objectListView.LocationChanged += new EventHandler(objectListView_LocationChanged);
            this.objectListView.SizeChanged += new EventHandler(objectListView_SizeChanged);
            this.objectListView.VisibleChanged += new EventHandler(objectListView_VisibleChanged);
            this.objectListView.ParentChanged += new EventHandler(objectListView_ParentChanged);

            // Collect our ancestors in the widget hierachy
            if (this.ancestors == null)
                this.ancestors = new List<Control>();
            Control parent = this.objectListView.Parent;
            while (parent != null) {
                this.ancestors.Add(parent);
                parent = parent.Parent;
            } 

            // Listen for changes in the hierachy
            foreach (Control ancestor in this.ancestors) {
                ancestor.ParentChanged += new EventHandler(objectListView_ParentChanged);
                TabControl tabControl = ancestor as TabControl;
                if (tabControl != null) {
                    tabControl.Selected += new TabControlEventHandler(tabControl_Selected);
                }
            }

            // Listen for changes in our owning form
            this.Owner = this.objectListView.FindForm();
            this.myOwner = this.Owner;
            if (this.Owner != null) {
                this.Owner.LocationChanged += new EventHandler(Owner_LocationChanged);
                this.Owner.SizeChanged += new EventHandler(Owner_SizeChanged);
                this.Owner.ResizeBegin += new EventHandler(Owner_ResizeBegin);
                this.Owner.ResizeEnd += new EventHandler(Owner_ResizeEnd);
                if (this.Owner.TopMost) {
                    // We can't do this.TopMost = true; since that will activate the panel,
                    // taking focus away from the owner of the listview
                    NativeMethods.MakeTopMost(this);
                }

                // We need special code to handle MDI
                this.mdiOwner = this.Owner.MdiParent;
                if (this.mdiOwner != null) {
                    this.mdiOwner.LocationChanged += new EventHandler(Owner_LocationChanged);
                    this.mdiOwner.SizeChanged += new EventHandler(Owner_SizeChanged);
                    this.mdiOwner.ResizeBegin += new EventHandler(Owner_ResizeBegin);
                    this.mdiOwner.ResizeEnd += new EventHandler(Owner_ResizeEnd);

                    // Find the MDIClient control, which houses all MDI children
                    foreach (Control c in this.mdiOwner.Controls) {
                        this.mdiClient = c as MdiClient;
                        if (this.mdiClient != null) {
                            break;
                        }
                    }
                    if (this.mdiClient != null) {
                        this.mdiClient.ClientSizeChanged += new EventHandler(myMdiClient_ClientSizeChanged);
                    }
                }
            }

            this.UpdateTransparency();
        }

        void myMdiClient_ClientSizeChanged(object sender, EventArgs e) {
            this.RecalculateBounds();
            this.Invalidate();
        }

        /// <summary>
        /// Made the overlay panel invisible
        /// </summary>
        public void HideGlass() {
            if (!this.isGlassShown)
                return;
            this.isGlassShown = false;
            this.Bounds = new Rectangle(-10000, -10000, 1, 1);
        }

        /// <summary>
        /// Show the overlay panel in its correctly location
        /// </summary>
        /// <remarks>
        /// If the panel is always shown, this method does nothing.
        /// If the panel is being resized, this method also does nothing.
        /// </remarks>
        public void ShowGlass() {
            if (this.isGlassShown || this.isDuringResizeSequence)
                return;

            this.isGlassShown = true;
            this.RecalculateBounds();
        }

        /// <summary>
        /// Detach this glass panel from its previous ObjectListView
        /// </summary>        
        /// <remarks>
        /// You should unbind the overlay panel before making any changes to the 
        /// widget hierarchy.
        /// </remarks>
        public void Unbind() {
            if (this.objectListView != null) {
                this.objectListView.Disposed -= new EventHandler(objectListView_Disposed);
                this.objectListView.LocationChanged -= new EventHandler(objectListView_LocationChanged);
                this.objectListView.SizeChanged -= new EventHandler(objectListView_SizeChanged);
                this.objectListView.VisibleChanged -= new EventHandler(objectListView_VisibleChanged);
                this.objectListView.ParentChanged -= new EventHandler(objectListView_ParentChanged);
                this.objectListView = null;
            }

            if (this.ancestors != null) {
                foreach (Control parent in this.ancestors) {
                    parent.ParentChanged -= new EventHandler(objectListView_ParentChanged);
                    TabControl tabControl = parent as TabControl;
                    if (tabControl != null) {
                        tabControl.Selected -= new TabControlEventHandler(tabControl_Selected);
                    }
                }
                this.ancestors = null;
            }

            if (this.myOwner != null) {
                this.myOwner.LocationChanged -= new EventHandler(Owner_LocationChanged);
                this.myOwner.SizeChanged -= new EventHandler(Owner_SizeChanged);
                this.myOwner.ResizeBegin -= new EventHandler(Owner_ResizeBegin);
                this.myOwner.ResizeEnd -= new EventHandler(Owner_ResizeEnd);
                this.myOwner = null;
            }

            if (this.mdiOwner != null) {
                this.mdiOwner.LocationChanged -= new EventHandler(Owner_LocationChanged);
                this.mdiOwner.SizeChanged -= new EventHandler(Owner_SizeChanged);
                this.mdiOwner.ResizeBegin -= new EventHandler(Owner_ResizeBegin);
                this.mdiOwner.ResizeEnd -= new EventHandler(Owner_ResizeEnd);
                this.mdiOwner = null;
            }

            if (this.mdiClient != null) {
                this.mdiClient.ClientSizeChanged -= new EventHandler(myMdiClient_ClientSizeChanged);
                this.mdiClient = null;
            }
        }

        #endregion

        #region Event Handlers

        void objectListView_Disposed(object sender, EventArgs e) {
            this.Unbind();
        }

        /// <summary>
        /// Handle when the form that owns the ObjectListView begins to be resized
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Owner_ResizeBegin(object sender, EventArgs e) {
            // When the top level window is being resized, we just want to hide
            // the overlay window. When the resizing finishes, we want to show
            // the overlay window, if it was shown before the resize started.
            this.isDuringResizeSequence = true;
            this.wasGlassShownBeforeResize = this.isGlassShown;
        }

        /// <summary>
        /// Handle when the form that owns the ObjectListView finished to be resized
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Owner_ResizeEnd(object sender, EventArgs e) {
            this.isDuringResizeSequence = false;
            if (this.wasGlassShownBeforeResize)
                this.ShowGlass();
        }

        /// <summary>
        /// The owning form has moved. Move the overlay panel too.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Owner_LocationChanged(object sender, EventArgs e) {
            if (this.mdiOwner != null)
                this.HideGlass();
            else
                this.RecalculateBounds();
        }

        /// <summary>
        /// The owning form is resizing. Hide our overlay panel until the resizing stops
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Owner_SizeChanged(object sender, EventArgs e) {
            this.HideGlass();
        }


        /// <summary>
        /// Handle when the bound OLV changes its location. The overlay panel must 
        /// be moved too, IFF it is currently visible.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void objectListView_LocationChanged(object sender, EventArgs e) {
            if (this.isGlassShown) {
                this.RecalculateBounds();
            }
        }

        /// <summary>
        /// Handle when the bound OLV changes size. The overlay panel must 
        /// resize too, IFF it is currently visible.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void objectListView_SizeChanged(object sender, EventArgs e) {
            // This event is triggered in all sorts of places, and not always when the size changes.
            //if (this.isGlassShown) {
            //    this.Size = this.objectListView.ClientSize;
            //}
        }

        /// <summary>
        /// Handle when the bound OLV is part of a TabControl and that
        /// TabControl changes tabs. The overlay panel is hidden. The
        /// first time the bound OLV is redrawn, the overlay panel will
        /// be shown again.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tabControl_Selected(object sender, TabControlEventArgs e) {
            this.HideGlass();
        }

        /// <summary>
        /// Somewhere the parent of the bound OLV has changed. Update
        /// our events.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void objectListView_ParentChanged(object sender, EventArgs e) {
            ObjectListView olv = this.objectListView;
            IOverlay overlay = this.Overlay;
            this.Unbind();
            this.Bind(olv, overlay);
        }

        /// <summary>
        /// Handle when the bound OLV changes its visibility.
        /// The overlay panel should match the OLV's visibility.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void objectListView_VisibleChanged(object sender, EventArgs e) {
            if (this.objectListView.Visible)
                this.ShowGlass();
            else
                this.HideGlass();
        }

        #endregion

        #region Implementation

        protected override void OnPaint(PaintEventArgs e) {
            if (this.objectListView == null || this.Overlay == null)
                return;

            Graphics g = e.Graphics;
            g.TextRenderingHint = ObjectListView.TextRenderingHint;
            g.SmoothingMode = ObjectListView.SmoothingMode;
            //g.DrawRectangle(new Pen(Color.Green, 4.0f), this.ClientRectangle);

            // If we are part of an MDI app, make sure we don't draw outside the bounds
            if (this.mdiClient != null) {
                Rectangle r = mdiClient.RectangleToScreen(mdiClient.ClientRectangle);
                Rectangle r2 = this.objectListView.RectangleToClient(r);
                g.SetClip(r2, System.Drawing.Drawing2D.CombineMode.Intersect);
            }

            this.Overlay.Draw(this.objectListView, g, this.objectListView.ClientRectangle);
        }

        protected void RecalculateBounds() {
            if (!this.isGlassShown)
                return;

            Rectangle rect = this.objectListView.ClientRectangle;
            rect.X = 0;
            rect.Y = 0;
            this.Bounds = this.objectListView.RectangleToScreen(rect);
        }

        internal void UpdateTransparency() {
            ITransparentOverlay transparentOverlay = this.Overlay as ITransparentOverlay;
            if (transparentOverlay == null)
                this.Opacity = this.objectListView.OverlayTransparency / 255.0f;
            else
                this.Opacity = transparentOverlay.Transparency / 255.0f;
        }

        protected override void WndProc(ref Message m) {
            const int WM_NCHITTEST = 132;
            const int HTTRANSPARENT = -1;
            switch (m.Msg) {
                // Ignore all mouse interactions
                case WM_NCHITTEST:
                    m.Result = (IntPtr)HTTRANSPARENT;
                    break;
            }
            base.WndProc(ref m);
        }

        #endregion
        
        #region Implementation variables

        internal IOverlay Overlay;

        #endregion

        #region Private variables

        private ObjectListView objectListView;
        private bool isDuringResizeSequence;
        private bool isGlassShown;
        private bool wasGlassShownBeforeResize;

        // Cache these so we can unsubscribe from events even when the OLV has been disposed.
        private Form myOwner;
        private Form mdiOwner;
        private List<Control> ancestors;
        MdiClient mdiClient;

        #endregion

    }
}

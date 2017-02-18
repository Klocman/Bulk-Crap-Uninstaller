/*
 * DropSink.cs - Add drop sink ability to an ObjectListView
 *
 * Author: Phillip Piper
 * Date: 2009-03-17 5:15 PM
 *
 * Change log:
 * v2.9
 * 2015-07-08   JPP  - Added SimpleDropSink.EnableFeedback to allow all the pretty and helpful
 *                     user feedback during drags to be turned off
 * v2.7
 * 2011-04-20   JPP  - Rewrote how ModelDropEventArgs.RefreshObjects() works on TreeListViews
 * v2.4.1
 * 2010-08-24   JPP  - Moved AcceptExternal property up to SimpleDragSource.
 * v2.3
 * 2009-09-01   JPP  - Correctly handle case where RefreshObjects() is called for
 *                     objects that were children but are now roots.
 * 2009-08-27   JPP  - Added ModelDropEventArgs.RefreshObjects() to simplify updating after
 *                     a drag-drop operation
 * 2009-08-19   JPP  - Changed to use OlvHitTest()
 * v2.2.1
 * 2007-07-06   JPP  - Added StandardDropActionFromKeys property to OlvDropEventArgs
 * v2.2
 * 2009-05-17   JPP  - Added a Handled flag to OlvDropEventArgs
 *                   - Tweaked the appearance of the drop-on-background feedback
 * 2009-04-15   JPP  - Separated DragDrop.cs into DropSink.cs
 * 2009-03-17   JPP  - Initial version
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
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace BrightIdeasSoftware
{
    /// <summary>
    /// Objects that implement this interface can acts as the receiver for drop
    /// operation for an ObjectListView.
    /// </summary>
    public interface IDropSink
    {
        /// <summary>
        /// Gets or sets the ObjectListView that is the drop sink
        /// </summary>
        ObjectListView ListView { get; set; }

        /// <summary>
        /// Draw any feedback that is appropriate to the current drop state.
        /// </summary>
        /// <remarks>
        /// Any drawing is done over the top of the ListView. This operation should disturb
        /// the Graphic as little as possible. Specifically, do not erase the area into which
        /// you draw. 
        /// </remarks>
        /// <param name="g">A Graphic for drawing</param>
        /// <param name="bounds">The contents bounds of the ListView (not including any header)</param>
        void DrawFeedback(Graphics g, Rectangle bounds);

        /// <summary>
        /// The user has released the drop over this control
        /// </summary>
        /// <remarks>
        /// Implementators should set args.Effect to the appropriate DragDropEffects. This value is returned
        /// to the originator of the drag.
        /// </remarks>
        /// <param name="args"></param>
        void Drop(DragEventArgs args);

        /// <summary>
        /// A drag has entered this control.
        /// </summary>
        /// <remarks>Implementators should set args.Effect to the appropriate DragDropEffects.</remarks>
        /// <param name="args"></param>
        void Enter(DragEventArgs args);

        /// <summary>
        /// Change the cursor to reflect the current drag operation.
        /// </summary>
        /// <param name="args"></param>
        void GiveFeedback(GiveFeedbackEventArgs args);

        /// <summary>
        /// The drag has left the bounds of this control
        /// </summary>
        void Leave();

        /// <summary>
        /// The drag is moving over this control.
        /// </summary>
        /// <remarks>This is where any drop target should be calculated.
        /// Implementators should set args.Effect to the appropriate DragDropEffects.
        /// </remarks>
        /// <param name="args"></param>
        void Over(DragEventArgs args);

        /// <summary>
        /// Should the drag be allowed to continue?
        /// </summary>
        /// <param name="args"></param>
        void QueryContinue(QueryContinueDragEventArgs args);
    }

    /// <summary>
    /// This is a do-nothing implementation of IDropSink that is a useful
    /// base class for more sophisticated implementations.
    /// </summary>
    public class AbstractDropSink : IDropSink
    {
        #region IDropSink Members

        /// <summary>
        /// Gets or sets the ObjectListView that is the drop sink
        /// </summary>
        public virtual ObjectListView ListView {
            get { return listView; }
            set { this.listView = value; }
        }
        private ObjectListView listView;

        /// <summary>
        /// Draw any feedback that is appropriate to the current drop state.
        /// </summary>
        /// <remarks>
        /// Any drawing is done over the top of the ListView. This operation should disturb
        /// the Graphic as little as possible. Specifically, do not erase the area into which
        /// you draw. 
        /// </remarks>
        /// <param name="g">A Graphic for drawing</param>
        /// <param name="bounds">The contents bounds of the ListView (not including any header)</param>
        public virtual void DrawFeedback(Graphics g, Rectangle bounds) {
        }

        /// <summary>
        /// The user has released the drop over this control
        /// </summary>
        /// <remarks>
        /// Implementators should set args.Effect to the appropriate DragDropEffects. This value is returned
        /// to the originator of the drag.
        /// </remarks>
        /// <param name="args"></param>
        public virtual void Drop(DragEventArgs args) {
            this.Cleanup();
        }

        /// <summary>
        /// A drag has entered this control.
        /// </summary>
        /// <remarks>Implementators should set args.Effect to the appropriate DragDropEffects.</remarks>
        /// <param name="args"></param>
        public virtual void Enter(DragEventArgs args) {
        }

        /// <summary>
        /// The drag has left the bounds of this control
        /// </summary>
        public virtual void Leave() {
            this.Cleanup();
        }

        /// <summary>
        /// The drag is moving over this control.
        /// </summary>
        /// <remarks>This is where any drop target should be calculated.
        /// Implementators should set args.Effect to the appropriate DragDropEffects.
        /// </remarks>
        /// <param name="args"></param>
        public virtual void Over(DragEventArgs args) {
        }

        /// <summary>
        /// Change the cursor to reflect the current drag operation.
        /// </summary>
        /// <remarks>You only need to override this if you want non-standard cursors.
        /// The standard cursors are supplied automatically.</remarks>
        /// <param name="args"></param>
        public virtual void GiveFeedback(GiveFeedbackEventArgs args) {
            args.UseDefaultCursors = true;
        }

        /// <summary>
        /// Should the drag be allowed to continue?
        /// </summary>
        /// <remarks>
        /// You only need to override this if you want the user to be able
        /// to end the drop in some non-standard way, e.g. dragging to a
        /// certain point even without releasing the mouse, or going outside
        /// the bounds of the application. 
        /// </remarks>
        /// <param name="args"></param>
        public virtual void QueryContinue(QueryContinueDragEventArgs args) {
        }


        #endregion

        #region Commands

        /// <summary>
        /// This is called when the mouse leaves the drop region and after the
        /// drop has completed.
        /// </summary>
        protected virtual void Cleanup() {
        }

        #endregion
    }

    /// <summary>
    /// The enum indicates which target has been found for a drop operation
    /// </summary>
    [Flags]
    public enum DropTargetLocation
    {
        /// <summary>
        /// No applicable target has been found
        /// </summary>
        None = 0,

        /// <summary>
        /// The list itself is the target of the drop
        /// </summary>
        Background = 0x01,

        /// <summary>
        /// An item is the target
        /// </summary>
        Item = 0x02,

        /// <summary>
        /// Between two items (or above the top item or below the bottom item)
        /// can be the target. This is not actually ever a target, only a value indicate
        /// that it is valid to drop between items
        /// </summary>
        BetweenItems = 0x04,

        /// <summary>
        /// Above an item is the target
        /// </summary>
        AboveItem = 0x08,

        /// <summary>
        /// Below an item is the target
        /// </summary>
        BelowItem = 0x10,

        /// <summary>
        /// A subitem is the target of the drop
        /// </summary>
        SubItem = 0x20,

        /// <summary>
        /// On the right of an item is the target (not currently used)
        /// </summary>
        RightOfItem = 0x40,

        /// <summary>
        /// On the left of an item is the target (not currently used)
        /// </summary>
        LeftOfItem = 0x80
    }

    /// <summary>
    /// This class represents a simple implementation of a drop sink.
    /// </summary>
    /// <remarks>
    /// Actually, it should be called CleverDropSink -- it's far from simple and can do quite a lot in its own right.
    /// </remarks>
    public class SimpleDropSink : AbstractDropSink
    {
        #region Life and death

        /// <summary>
        /// Make a new drop sink
        /// </summary>
        public SimpleDropSink() {
            this.timer = new Timer();
            this.timer.Interval = 250;
            this.timer.Tick += new EventHandler(this.timer_Tick);

            this.CanDropOnItem = true;
            //this.CanDropOnSubItem = true;
            //this.CanDropOnBackground = true;
            //this.CanDropBetween = true;

            this.FeedbackColor = Color.FromArgb(180, Color.MediumBlue);
            this.billboard = new BillboardOverlay();
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Get or set the locations where a drop is allowed to occur (OR-ed together)
        /// </summary>
        public DropTargetLocation AcceptableLocations {
            get { return this.acceptableLocations; }
            set { this.acceptableLocations = value; }
        }
        private DropTargetLocation acceptableLocations;
        
        /// <summary>
        /// Gets or sets whether this sink allows model objects to be dragged from other lists. Defaults to true.
        /// </summary>
        public bool AcceptExternal {
            get { return this.acceptExternal; }
            set { this.acceptExternal = value; }
        }
        private bool acceptExternal = true;

        /// <summary>
        /// Gets or sets whether the ObjectListView should scroll when the user drags
        /// something near to the top or bottom rows. Defaults to true.
        /// </summary>
        /// <remarks>AutoScroll does not scroll horizontally.</remarks>
        public bool AutoScroll {
            get { return this.autoScroll; }
            set { this.autoScroll = value; }
        }
        private bool autoScroll = true;

        /// <summary>
        /// Gets the billboard overlay that will be used to display feedback
        /// messages during a drag operation. 
        /// </summary>
        /// <remarks>Set this to null to stop the feedback.</remarks>
        public BillboardOverlay Billboard {
            get { return this.billboard; }
            set { this.billboard = value; }
        }
        private BillboardOverlay billboard;

        /// <summary>
        /// Get or set whether a drop can occur between items of the list
        /// </summary>
        public bool CanDropBetween {
            get { return (this.AcceptableLocations & DropTargetLocation.BetweenItems) == DropTargetLocation.BetweenItems; }
            set {
                if (value)
                    this.AcceptableLocations |= DropTargetLocation.BetweenItems;
                else 
                    this.AcceptableLocations &= ~DropTargetLocation.BetweenItems;
            }
        }

        /// <summary>
        /// Get or set whether a drop can occur on the listview itself
        /// </summary>
        public bool CanDropOnBackground {
            get { return (this.AcceptableLocations & DropTargetLocation.Background) == DropTargetLocation.Background; }
            set {
                if (value)
                    this.AcceptableLocations |= DropTargetLocation.Background;
                else
                    this.AcceptableLocations &= ~DropTargetLocation.Background;
            }
        }

        /// <summary>
        /// Get or set whether a drop can occur on items in the list
        /// </summary>
        public bool CanDropOnItem {
            get { return (this.AcceptableLocations & DropTargetLocation.Item) == DropTargetLocation.Item; }
            set {
                if (value)
                    this.AcceptableLocations |= DropTargetLocation.Item;
                else
                    this.AcceptableLocations &= ~DropTargetLocation.Item;
            }
        }

        /// <summary>
        /// Get or set whether a drop can occur on a subitem in the list
        /// </summary>
        public bool CanDropOnSubItem {
            get { return (this.AcceptableLocations & DropTargetLocation.SubItem) == DropTargetLocation.SubItem; }
            set {
                if (value)
                    this.AcceptableLocations |= DropTargetLocation.SubItem;
                else
                    this.AcceptableLocations &= ~DropTargetLocation.SubItem;
            }
        }

        /// <summary>
        /// Gets or sets whether the drop sink should draw feedback onto the given list
        /// during the drag operation. Defaults to true.
        /// </summary>
        /// <remarks>If this is false, you will have to give the user feedback in some
        /// other fashion, like cursor changes</remarks>
        public bool EnableFeedback {
            get { return enableFeedback; }
            set { enableFeedback = value; }
        }
        private bool enableFeedback = true;

        /// <summary>
        /// Get or set the index of the item that is the target of the drop
        /// </summary>
        public int DropTargetIndex {
            get { return dropTargetIndex; }
            set {
                if (this.dropTargetIndex != value) {
                    this.dropTargetIndex = value;
                    this.ListView.Invalidate();
                }
            }
        }
        private int dropTargetIndex = -1;

        /// <summary>
        /// Get the item that is the target of the drop
        /// </summary>
        public OLVListItem DropTargetItem {
            get {
                return this.ListView.GetItem(this.DropTargetIndex);
            }
        }

        /// <summary>
        /// Get or set the location of the target of the drop
        /// </summary>
        public DropTargetLocation DropTargetLocation {
            get { return dropTargetLocation; }
            set {
                if (this.dropTargetLocation != value) {
                    this.dropTargetLocation = value;
                    this.ListView.Invalidate();
                }
            }
        }
        private DropTargetLocation dropTargetLocation;

        /// <summary>
        /// Get or set the index of the subitem that is the target of the drop
        /// </summary>
        public int DropTargetSubItemIndex {
            get { return dropTargetSubItemIndex; }
            set {
                if (this.dropTargetSubItemIndex != value) {
                    this.dropTargetSubItemIndex = value;
                    this.ListView.Invalidate();
                }
            }
        }
        private int dropTargetSubItemIndex = -1;

        /// <summary>
        /// Get or set the color that will be used to provide drop feedback
        /// </summary>
        public Color FeedbackColor {
            get { return this.feedbackColor; }
            set { this.feedbackColor = value; }
        }
        private Color feedbackColor;

        /// <summary>
        /// Get whether the alt key was down during this drop event
        /// </summary>
        public bool IsAltDown {
            get { return (this.KeyState & 32) == 32; }
        }

        /// <summary>
        /// Get whether any modifier key was down during this drop event
        /// </summary>
        public bool IsAnyModifierDown {
            get { return (this.KeyState & (4 + 8 + 32)) != 0; }
        }

        /// <summary>
        /// Get whether the control key was down during this drop event
        /// </summary>
        public bool IsControlDown {
            get { return (this.KeyState & 8) == 8; }
        }

        /// <summary>
        /// Get whether the left mouse button was down during this drop event
        /// </summary>
        public bool IsLeftMouseButtonDown {
            get { return (this.KeyState & 1) == 1; }
        }

        /// <summary>
        /// Get whether the right mouse button was down during this drop event
        /// </summary>
        public bool IsMiddleMouseButtonDown {
            get { return (this.KeyState & 16) == 16; }
        }

        /// <summary>
        /// Get whether the right mouse button was down during this drop event
        /// </summary>
        public bool IsRightMouseButtonDown {
            get { return (this.KeyState & 2) == 2; }
        }

        /// <summary>
        /// Get whether the shift key was down during this drop event
        /// </summary>
        public bool IsShiftDown {
            get { return (this.KeyState & 4) == 4; }
        }

        /// <summary>
        /// Get or set the state of the keys during this drop event
        /// </summary>
        public int KeyState {
            get { return this.keyState; }
            set { this.keyState = value; }
        }
        private int keyState;

        /// <summary>
        /// Gets or sets whether the drop sink will automatically use cursors
        /// based on the drop effect. By default, this is true. If this is
        /// set to false, you must set the Cursor yourself.
        /// </summary>
        public bool UseDefaultCursors {
            get { return useDefaultCursors; }
            set { useDefaultCursors = value; }
        }
        private bool useDefaultCursors = true;

        #endregion

        #region Events

        /// <summary>
        /// Triggered when the sink needs to know if a drop can occur.
        /// </summary>
        /// <remarks>
        /// Handlers should set Effect to indicate what is possible.
        /// Handlers can change any of the DropTarget* setttings to change
        /// the target of the drop.
        /// </remarks>
        public event EventHandler<OlvDropEventArgs> CanDrop;

        /// <summary>
        /// Triggered when the drop is made.
        /// </summary>
        public event EventHandler<OlvDropEventArgs> Dropped;

        /// <summary>
        /// Triggered when the sink needs to know if a drop can occur
        /// AND the source is an ObjectListView
        /// </summary>
        /// <remarks>
        /// Handlers should set Effect to indicate what is possible.
        /// Handlers can change any of the DropTarget* setttings to change
        /// the target of the drop.
        /// </remarks>
        public event EventHandler<ModelDropEventArgs> ModelCanDrop;

        /// <summary>
        /// Triggered when the drop is made.
        /// AND the source is an ObjectListView
        /// </summary>
        public event EventHandler<ModelDropEventArgs> ModelDropped;

        #endregion

        #region DropSink Interface

        /// <summary>
        /// Cleanup the drop sink when the mouse has left the control or 
        /// the drag has finished.
        /// </summary>
        protected override void Cleanup() {
            this.DropTargetLocation = DropTargetLocation.None;
            this.ListView.FullRowSelect = this.originalFullRowSelect;
            this.Billboard.Text = null;
        }

        /// <summary>
        /// Draw any feedback that is appropriate to the current drop state.
        /// </summary>
        /// <remarks>
        /// Any drawing is done over the top of the ListView. This operation should disturb
        /// the Graphic as little as possible. Specifically, do not erase the area into which
        /// you draw. 
        /// </remarks>
        /// <param name="g">A Graphic for drawing</param>
        /// <param name="bounds">The contents bounds of the ListView (not including any header)</param>
        public override void DrawFeedback(Graphics g, Rectangle bounds) {
            g.SmoothingMode = ObjectListView.SmoothingMode;

            if (this.EnableFeedback) {
                switch (this.DropTargetLocation) {
                    case DropTargetLocation.Background:
                        this.DrawFeedbackBackgroundTarget(g, bounds);
                        break;
                    case DropTargetLocation.Item:
                        this.DrawFeedbackItemTarget(g, bounds);
                        break;
                    case DropTargetLocation.AboveItem:
                        this.DrawFeedbackAboveItemTarget(g, bounds);
                        break;
                    case DropTargetLocation.BelowItem:
                        this.DrawFeedbackBelowItemTarget(g, bounds);
                        break;
                }
            }

            if (this.Billboard != null) {
                this.Billboard.Draw(this.ListView, g, bounds);
            }
        }

        /// <summary>
        /// The user has released the drop over this control
        /// </summary>
        /// <param name="args"></param>
        public override void Drop(DragEventArgs args) {
            this.dropEventArgs.DragEventArgs = args;
            this.TriggerDroppedEvent(args);
            this.timer.Stop();
            this.Cleanup();
        }

        /// <summary>
        /// A drag has entered this control.
        /// </summary>
        /// <remarks>Implementators should set args.Effect to the appropriate DragDropEffects.</remarks>
        /// <param name="args"></param>
        public override void Enter(DragEventArgs args) {
            //System.Diagnostics.Debug.WriteLine("Enter");

            /* 
             * When FullRowSelect is true, we have two problems:
             * 1) GetItemRect(ItemOnly) returns the whole row rather than just the icon/text, which messes
             *    up our calculation of the drop rectangle.
             * 2) during the drag, the Timer events will not fire! This is the major problem, since without
             *    those events we can't autoscroll. 
             * 
             * The first problem we can solve through coding, but the second is more difficult. 
             * We avoid both problems by turning off FullRowSelect during the drop operation.
             */    
            this.originalFullRowSelect = this.ListView.FullRowSelect;
            this.ListView.FullRowSelect = false;

            // Setup our drop event args block
            this.dropEventArgs = new ModelDropEventArgs();
            this.dropEventArgs.DropSink = this;
            this.dropEventArgs.ListView = this.ListView;
            this.dropEventArgs.DragEventArgs = args;
            this.dropEventArgs.DataObject = args.Data;
            OLVDataObject olvData = args.Data as OLVDataObject;
            if (olvData != null) {
                this.dropEventArgs.SourceListView = olvData.ListView;
                this.dropEventArgs.SourceModels = olvData.ModelObjects;
            }

            this.Over(args);
        }

        /// <summary>
        /// Change the cursor to reflect the current drag operation.
        /// </summary>
        /// <param name="args"></param>
        public override void GiveFeedback(GiveFeedbackEventArgs args) {
            args.UseDefaultCursors = this.UseDefaultCursors;
        }

        /// <summary>
        /// The drag is moving over this control.
        /// </summary>
        /// <param name="args"></param>
        public override void Over(DragEventArgs args) {
            //System.Diagnostics.Debug.WriteLine("Over");
            this.dropEventArgs.DragEventArgs = args;
            this.KeyState = args.KeyState;
            Point pt = this.ListView.PointToClient(new Point(args.X, args.Y));
            args.Effect = this.CalculateDropAction(args, pt);
            this.CheckScrolling(pt);
        }

        #endregion
        
        #region Events

        /// <summary>
        /// Trigger the Dropped events
        /// </summary>
        /// <param name="args"></param>
        protected virtual void TriggerDroppedEvent(DragEventArgs args) {
            this.dropEventArgs.Handled = false;

            // If the source is an ObjectListView, trigger the ModelDropped event
            if (this.dropEventArgs.SourceListView != null) 
                this.OnModelDropped(this.dropEventArgs);

            if (!this.dropEventArgs.Handled)
                this.OnDropped(this.dropEventArgs);
        }

        /// <summary>
        /// Trigger CanDrop
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnCanDrop(OlvDropEventArgs args) {
            if (this.CanDrop != null)
                this.CanDrop(this, args);
        }

        /// <summary>
        /// Trigger Dropped
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnDropped(OlvDropEventArgs args) {
            if (this.Dropped != null)
                this.Dropped(this, args);
        }

        /// <summary>
        /// Trigger ModelCanDrop
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnModelCanDrop(ModelDropEventArgs args) {

            // Don't allow drops from other list, if that's what's configured
            if (!this.AcceptExternal && args.SourceListView != null && args.SourceListView != this.ListView) {
                args.Effect = DragDropEffects.None;
                args.DropTargetLocation = DropTargetLocation.None;
                args.InfoMessage = "This list doesn't accept drops from other lists";
                return;
            }

            if (this.ModelCanDrop != null)
                this.ModelCanDrop(this, args);
        }

        /// <summary>
        /// Trigger ModelDropped
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnModelDropped(ModelDropEventArgs args) {
            if (this.ModelDropped != null)
                this.ModelDropped(this, args);
        }

        #endregion

        #region Implementation

        private void timer_Tick(object sender, EventArgs e) {
            this.HandleTimerTick();
        }

        /// <summary>
        /// Handle the timer tick event, which is sent when the listview should
        /// scroll
        /// </summary>
        protected virtual void HandleTimerTick() {

            // If the mouse has been released, stop scrolling.
            // This is only necessary if the mouse is released outside of the control. 
            // If the mouse is released inside the control, we would receive a Drop event.
            if ((this.IsLeftMouseButtonDown && (Control.MouseButtons & MouseButtons.Left) != MouseButtons.Left) ||
                (this.IsMiddleMouseButtonDown && (Control.MouseButtons & MouseButtons.Middle) != MouseButtons.Middle) ||
                (this.IsRightMouseButtonDown && (Control.MouseButtons & MouseButtons.Right) != MouseButtons.Right)) {
                this.timer.Stop();
                this.Cleanup();
                return;
            }

            // Auto scrolling will continune while the mouse is close to the ListView
            const int GRACE_PERIMETER = 30;

            Point pt = this.ListView.PointToClient(Cursor.Position);
            Rectangle r2 = this.ListView.ClientRectangle;
            r2.Inflate(GRACE_PERIMETER, GRACE_PERIMETER);
            if (r2.Contains(pt)) {
                this.ListView.LowLevelScroll(0, this.scrollAmount);
            }
        }

        /// <summary>
        /// When the mouse is at the given point, what should the target of the drop be?
        /// </summary>
        /// <remarks>This method should update the DropTarget* members of the given arg block</remarks>
        /// <param name="args"></param>
        /// <param name="pt">The mouse point, in client co-ordinates</param>
        protected virtual void CalculateDropTarget(OlvDropEventArgs args, Point pt) {
            const int SMALL_VALUE = 3;
            DropTargetLocation location = DropTargetLocation.None;
            int targetIndex = -1;
            int targetSubIndex = 0;

            if (this.CanDropOnBackground)
                location = DropTargetLocation.Background;

            // Which item is the mouse over?
            // If it is not over any item, it's over the background.
            //ListViewHitTestInfo info = this.ListView.HitTest(pt.X, pt.Y);
            OlvListViewHitTestInfo info = this.ListView.OlvHitTest(pt.X, pt.Y);
            if (info.Item != null && this.CanDropOnItem) {
                location = DropTargetLocation.Item;
                targetIndex = info.Item.Index;
                if (info.SubItem != null && this.CanDropOnSubItem)
                    targetSubIndex = info.Item.SubItems.IndexOf(info.SubItem);
            }

            // Check to see if the mouse is "between" rows.
            // ("between" is somewhat loosely defined)
            if (this.CanDropBetween && this.ListView.GetItemCount() > 0) {

                // If the mouse is over an item, check to see if it is near the top or bottom
                if (location == DropTargetLocation.Item) {
                    if (pt.Y - SMALL_VALUE <= info.Item.Bounds.Top)
                        location = DropTargetLocation.AboveItem;
                    if (pt.Y + SMALL_VALUE >= info.Item.Bounds.Bottom)
                        location = DropTargetLocation.BelowItem;
                } else {
                    // Is there an item a little below the mouse?
                    // If so, we say the drop point is above that row
                    info = this.ListView.OlvHitTest(pt.X, pt.Y + SMALL_VALUE);
                    if (info.Item != null) {
                        targetIndex = info.Item.Index;
                        location = DropTargetLocation.AboveItem;
                    } else {
                        // Is there an item a little above the mouse?
                        info = this.ListView.OlvHitTest(pt.X, pt.Y - SMALL_VALUE);
                        if (info.Item != null) {
                            targetIndex = info.Item.Index;
                            location = DropTargetLocation.BelowItem;
                        }
                    }
                }
            }

            args.DropTargetLocation = location;
            args.DropTargetIndex = targetIndex;
            args.DropTargetSubItemIndex = targetSubIndex;
        }

        /// <summary>
        /// What sort of action is possible when the mouse is at the given point?
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="args"></param>
        /// <param name="pt"></param>
        /// <returns></returns>
        public virtual DragDropEffects CalculateDropAction(DragEventArgs args, Point pt) {

            this.CalculateDropTarget(this.dropEventArgs, pt);

            this.dropEventArgs.MouseLocation = pt;
            this.dropEventArgs.InfoMessage = null;
            this.dropEventArgs.Handled = false;

            if (this.dropEventArgs.SourceListView != null) {
                this.dropEventArgs.TargetModel = this.ListView.GetModelObject(this.dropEventArgs.DropTargetIndex);
                this.OnModelCanDrop(this.dropEventArgs);
            }

            if (!this.dropEventArgs.Handled)
                this.OnCanDrop(this.dropEventArgs);

            this.UpdateAfterCanDropEvent(this.dropEventArgs);

            return this.dropEventArgs.Effect;
        }

        /// <summary>
        /// Based solely on the state of the modifier keys, what drop operation should
        /// be used?
        /// </summary>
        /// <returns>The drop operation that matches the state of the keys</returns>
        public DragDropEffects CalculateStandardDropActionFromKeys() {
            if (this.IsControlDown) {
                if (this.IsShiftDown)
                    return DragDropEffects.Link;
                else
                    return DragDropEffects.Copy;
            } else {
                return DragDropEffects.Move;
            }
        }

        /// <summary>
        /// Should the listview be made to scroll when the mouse is at the given point?
        /// </summary>
        /// <param name="pt"></param>
        protected virtual void CheckScrolling(Point pt) {
            if (!this.AutoScroll)
                return;

            Rectangle r = this.ListView.ContentRectangle;
            int rowHeight = this.ListView.RowHeightEffective;
            int close = rowHeight;

            // In Tile view, using the whole row height is too much
            if (this.ListView.View == View.Tile)
                close /= 2;

            if (pt.Y <= (r.Top + close)) {
                // Scroll faster if the mouse is closer to the top
                this.timer.Interval = ((pt.Y <= (r.Top + close / 2)) ? 100 : 350);
                this.timer.Start();
                this.scrollAmount = -rowHeight;
            } else {
                if (pt.Y >= (r.Bottom - close)) {
                    this.timer.Interval = ((pt.Y >= (r.Bottom - close / 2)) ? 100 : 350);
                    this.timer.Start();
                    this.scrollAmount = rowHeight;
                } else {
                    this.timer.Stop();
                }
            }
        }

        /// <summary>
        /// Update the state of our sink to reflect the information that 
        /// may have been written into the drop event args.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void UpdateAfterCanDropEvent(OlvDropEventArgs args) {
            this.DropTargetIndex = args.DropTargetIndex;
            this.DropTargetLocation = args.DropTargetLocation;
            this.DropTargetSubItemIndex = args.DropTargetSubItemIndex;

            if (this.Billboard != null) {
                Point pt = args.MouseLocation;
                pt.Offset(5, 5);
                if (this.Billboard.Text != this.dropEventArgs.InfoMessage || this.Billboard.Location != pt) {
                    this.Billboard.Text = this.dropEventArgs.InfoMessage;
                    this.Billboard.Location = pt;
                    this.ListView.Invalidate();
                }
            }
        }

        #endregion

        #region Rendering

        /// <summary>
        /// Draw the feedback that shows that the background is the target
        /// </summary>
        /// <param name="g"></param>
        /// <param name="bounds"></param>
        protected virtual void DrawFeedbackBackgroundTarget(Graphics g, Rectangle bounds) {
            float penWidth = 12.0f;
            Rectangle r = bounds;
            r.Inflate((int)-penWidth / 2, (int)-penWidth / 2);
            using (Pen p = new Pen(Color.FromArgb(128, this.FeedbackColor), penWidth)) {
                using (GraphicsPath path = this.GetRoundedRect(r, 30.0f)) {
                    g.DrawPath(p, path);
                }
            }
        }

        /// <summary>
        /// Draw the feedback that shows that an item (or a subitem) is the target
        /// </summary>
        /// <param name="g"></param>
        /// <param name="bounds"></param>
        /// <remarks>
        /// DropTargetItem and DropTargetSubItemIndex tells what is the target
        /// </remarks>
        protected virtual void DrawFeedbackItemTarget(Graphics g, Rectangle bounds) {
            if (this.DropTargetItem == null)
                return;
            Rectangle r = this.CalculateDropTargetRectangle(this.DropTargetItem, this.DropTargetSubItemIndex);
            r.Inflate(1, 1);
            float diameter = r.Height / 3;
            using (GraphicsPath path = this.GetRoundedRect(r, diameter)) {
                using (SolidBrush b = new SolidBrush(Color.FromArgb(48, this.FeedbackColor))) {
                    g.FillPath(b, path);
                }
                using (Pen p = new Pen(this.FeedbackColor, 3.0f)) {
                    g.DrawPath(p, path);
                }
            }
        }

        /// <summary>
        /// Draw the feedback that shows the drop will occur before target
        /// </summary>
        /// <param name="g"></param>
        /// <param name="bounds"></param>
        protected virtual void DrawFeedbackAboveItemTarget(Graphics g, Rectangle bounds) {
            if (this.DropTargetItem == null)
                return;

            Rectangle r = this.CalculateDropTargetRectangle(this.DropTargetItem, this.DropTargetSubItemIndex);
            this.DrawBetweenLine(g, r.Left, r.Top, r.Right, r.Top);
        }

        /// <summary>
        /// Draw the feedback that shows the drop will occur after target
        /// </summary>
        /// <param name="g"></param>
        /// <param name="bounds"></param>
        protected virtual void DrawFeedbackBelowItemTarget(Graphics g, Rectangle bounds) {
            if (this.DropTargetItem == null)
                return;

            Rectangle r = this.CalculateDropTargetRectangle(this.DropTargetItem, this.DropTargetSubItemIndex);
            this.DrawBetweenLine(g, r.Left, r.Bottom, r.Right, r.Bottom);
        }

        /// <summary>
        /// Return a GraphicPath that is round corner rectangle.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="diameter"></param>
        /// <returns></returns>
        protected GraphicsPath GetRoundedRect(Rectangle rect, float diameter) {
            GraphicsPath path = new GraphicsPath();

            RectangleF arc = new RectangleF(rect.X, rect.Y, diameter, diameter);
            path.AddArc(arc, 180, 90);
            arc.X = rect.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = rect.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = rect.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();

            return path;
        }

        /// <summary>
        /// Calculate the target rectangle when the given item (and possible subitem)
        /// is the target of the drop.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="subItem"></param>
        /// <returns></returns>
        protected virtual Rectangle CalculateDropTargetRectangle(OLVListItem item, int subItem) {
            if (subItem > 0)
                return item.SubItems[subItem].Bounds;
            
            Rectangle r = this.ListView.CalculateCellTextBounds(item, subItem);

            // Allow for indent
            if (item.IndentCount > 0) {
                int indentWidth = this.ListView.SmallImageSize.Width;
                r.X += (indentWidth * item.IndentCount);
                r.Width -= (indentWidth * item.IndentCount);
            }

            return r;
        }

        /// <summary>
        /// Draw a "between items" line at the given co-ordinates
        /// </summary>
        /// <param name="g"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        protected virtual void DrawBetweenLine(Graphics g, int x1, int y1, int x2, int y2) {
            using (Brush b = new SolidBrush(this.FeedbackColor)) {
                int x = x1;
                int y = y1;
                using (GraphicsPath gp = new GraphicsPath()) {
                    gp.AddLine(
                        x, y + 5,
                        x, y - 5);
                    gp.AddBezier(
                        x, y - 6,
                        x + 3, y - 2,
                        x + 6, y - 1,
                        x + 11, y);
                    gp.AddBezier(
                        x + 11, y,
                        x + 6, y + 1,
                        x + 3, y + 2,
                        x, y + 6);
                    gp.CloseFigure();
                    g.FillPath(b, gp);
                }
                x = x2;
                y = y2;
                using (GraphicsPath gp = new GraphicsPath()) {
                    gp.AddLine(
                        x, y + 6,
                        x, y - 6);
                    gp.AddBezier(
                        x, y - 7,
                        x - 3, y - 2,
                        x - 6, y - 1,
                        x - 11, y);
                    gp.AddBezier(
                        x - 11, y,
                        x - 6, y + 1,
                        x - 3, y + 2,
                        x, y + 7);
                    gp.CloseFigure();
                    g.FillPath(b, gp);
                }
            }
            using (Pen p = new Pen(this.FeedbackColor, 3.0f)) {
                g.DrawLine(p, x1, y1, x2, y2);
            }
        }

        #endregion

        private Timer timer;
        private int scrollAmount;
        private bool originalFullRowSelect;
        private ModelDropEventArgs dropEventArgs;
    }

    /// <summary>
    /// This drop sink allows items within the same list to be rearranged,
    /// as well as allowing items to be dropped from other lists.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class can only be used on plain ObjectListViews and FastObjectListViews.
    /// The other flavours have no way to implement the insert operation that is required.
    /// </para>
    /// <para>
    /// This class does not work with grouping.
    /// </para>
    /// <para>
    /// This class works when the OLV is sorted, but it is up to the programmer
    /// to decide what rearranging such lists "means". Example: if the control is sorting
    /// students by academic grade, and the user drags a "Fail" grade student up amonst the "A+"
    /// students, it is the responsibility of the programmer to makes the appropriate changes
    /// to the model and redraw/rebuild the control so that the users action makes sense.
    /// </para>
    /// <para>
    /// Users of this class should listen for the CanDrop event to decide
    /// if models from another OLV can be moved to OLV under this sink.
    /// </para>
    /// </remarks>
    public class RearrangingDropSink : SimpleDropSink
    {
        /// <summary>
        /// Create a RearrangingDropSink
        /// </summary>
        public RearrangingDropSink() {
            this.CanDropBetween = true;
            this.CanDropOnBackground = true;
            this.CanDropOnItem = false;
        }

        /// <summary>
        /// Create a RearrangingDropSink
        /// </summary>
        /// <param name="acceptDropsFromOtherLists"></param>
        public RearrangingDropSink(bool acceptDropsFromOtherLists)
            : this() {
            this.AcceptExternal = acceptDropsFromOtherLists;
        }

        /// <summary>
        /// Trigger OnModelCanDrop
        /// </summary>
        /// <param name="args"></param>
        protected override void OnModelCanDrop(ModelDropEventArgs args) {
            base.OnModelCanDrop(args);

            if (args.Handled)
                return;

            args.Effect = DragDropEffects.Move;

            // Don't allow drops from other list, if that's what's configured
            if (!this.AcceptExternal && args.SourceListView != this.ListView) {
                args.Effect = DragDropEffects.None;
                args.DropTargetLocation = DropTargetLocation.None;
                args.InfoMessage = "This list doesn't accept drops from other lists";
            }

            // If we are rearranging a list, don't allow drops on the background
            if (args.DropTargetLocation == DropTargetLocation.Background && args.SourceListView == this.ListView) {
                args.Effect = DragDropEffects.None;
                args.DropTargetLocation = DropTargetLocation.None;
            }
        }

        /// <summary>
        /// Trigger OnModelDropped
        /// </summary>
        /// <param name="args"></param>
        protected override void OnModelDropped(ModelDropEventArgs args) {
            base.OnModelDropped(args);

            if (!args.Handled)
                this.RearrangeModels(args);
        }

        /// <summary>
        /// Do the work of processing the dropped items
        /// </summary>
        /// <param name="args"></param>
        public virtual void RearrangeModels(ModelDropEventArgs args) {
            switch (args.DropTargetLocation) {
                case DropTargetLocation.AboveItem:
                    this.ListView.MoveObjects(args.DropTargetIndex, args.SourceModels);
                    break;
                case DropTargetLocation.BelowItem:
                    this.ListView.MoveObjects(args.DropTargetIndex + 1, args.SourceModels);
                    break;
                case DropTargetLocation.Background:
                    this.ListView.AddObjects(args.SourceModels);
                    break;
                default:
                    return;
            }

            if (args.SourceListView != this.ListView) {
                args.SourceListView.RemoveObjects(args.SourceModels);
            }
        }
    }

    /// <summary>
    /// When a drop sink needs to know if something can be dropped, or
    /// to notify that a drop has occured, it uses an instance of this class.
    /// </summary>
    public class OlvDropEventArgs : EventArgs
    {
        /// <summary>
        /// Create a OlvDropEventArgs
        /// </summary>
        public OlvDropEventArgs() {
        }

        #region Data Properties

        /// <summary>
        /// Get the original drag-drop event args
        /// </summary>
        public DragEventArgs DragEventArgs
        {
            get { return this.dragEventArgs; }
            internal set { this.dragEventArgs = value; }
        }
        private DragEventArgs dragEventArgs;

        /// <summary>
        /// Get the data object that is being dragged
        /// </summary>
        public object DataObject
        {
            get { return this.dataObject; }
            internal set { this.dataObject = value; }
        }
        private object dataObject;

        /// <summary>
        /// Get the drop sink that originated this event
        /// </summary>
        public SimpleDropSink DropSink {
            get { return this.dropSink; }
            internal set { this.dropSink = value; }
        }
        private SimpleDropSink dropSink;

        /// <summary>
        /// Get or set the index of the item that is the target of the drop
        /// </summary>
        public int DropTargetIndex {
            get { return dropTargetIndex; }
            set { this.dropTargetIndex = value; }
        }
        private int dropTargetIndex = -1;

        /// <summary>
        /// Get or set the location of the target of the drop
        /// </summary>
        public DropTargetLocation DropTargetLocation {
            get { return dropTargetLocation; }
            set { this.dropTargetLocation = value; }
        }
        private DropTargetLocation dropTargetLocation;

        /// <summary>
        /// Get or set the index of the subitem that is the target of the drop
        /// </summary>
        public int DropTargetSubItemIndex {
            get { return dropTargetSubItemIndex; }
            set { this.dropTargetSubItemIndex = value; }
        }
        private int dropTargetSubItemIndex = -1;

        /// <summary>
        /// Get the item that is the target of the drop
        /// </summary>
        public OLVListItem DropTargetItem {
            get {
                return this.ListView.GetItem(this.DropTargetIndex);
            }
            set {
                if (value == null)
                    this.DropTargetIndex = -1;
                else
                    this.DropTargetIndex = value.Index;
            }
        }

        /// <summary>
        /// Get or set the drag effect that should be used for this operation
        /// </summary>
        public DragDropEffects Effect {
            get { return this.effect; }
            set { this.effect = value; }
        }
        private DragDropEffects effect;

        /// <summary>
        /// Get or set if this event was handled. No further processing will be done for a handled event.
        /// </summary>
        public bool Handled {
            get { return this.handled; }
            set { this.handled = value; }
        }
        private bool handled;

        /// <summary>
        /// Get or set the feedback message for this operation
        /// </summary>
        /// <remarks>
        /// If this is not null, it will be displayed as a feedback message
        /// during the drag.
        /// </remarks>
        public string InfoMessage {
            get { return this.infoMessage; }
            set { this.infoMessage = value; }
        }
        private string infoMessage;

        /// <summary>
        /// Get the ObjectListView that is being dropped on
        /// </summary>
        public ObjectListView ListView {
            get { return this.listView; }
            internal set { this.listView = value; }
        }
        private ObjectListView listView;

        /// <summary>
        /// Get the location of the mouse (in target ListView co-ords)
        /// </summary>
        public Point MouseLocation {
            get { return this.mouseLocation; }
            internal set { this.mouseLocation = value; }
        }
        private Point mouseLocation;

        /// <summary>
        /// Get the drop action indicated solely by the state of the modifier keys
        /// </summary>
        public DragDropEffects StandardDropActionFromKeys {
            get {
                return this.DropSink.CalculateStandardDropActionFromKeys();
            }
        }

        #endregion
    }

    /// <summary>
    /// These events are triggered when the drag source is an ObjectListView.
    /// </summary>
    public class ModelDropEventArgs : OlvDropEventArgs
    {
        /// <summary>
        /// Create a ModelDropEventArgs
        /// </summary>
        public ModelDropEventArgs()
        {
        }

        /// <summary>
        /// Gets the model objects that are being dragged.
        /// </summary>
        public IList SourceModels {
            get { return this.dragModels; }
            internal set { 
                this.dragModels = value;
                TreeListView tlv = this.SourceListView as TreeListView;
                if (tlv != null) {
                    foreach (object model in this.SourceModels) {
                        object parent = tlv.GetParent(model);
                        if (!toBeRefreshed.Contains(parent))
                            toBeRefreshed.Add(parent);
                    }
                }
            }
        }
        private IList dragModels;
        private ArrayList toBeRefreshed = new ArrayList();

        /// <summary>
        /// Gets the ObjectListView that is the source of the dragged objects.
        /// </summary>
        public ObjectListView SourceListView {
            get { return this.sourceListView; }
            internal set { this.sourceListView = value; }
        }
        private ObjectListView sourceListView;

        /// <summary>
        /// Get the model object that is being dropped upon.
        /// </summary>
        /// <remarks>This is only value for TargetLocation == Item</remarks>
        public object TargetModel {
            get { return this.targetModel; }
            internal set { this.targetModel = value; }
        }
        private object targetModel;

        /// <summary>
        /// Refresh all the objects involved in the operation
        /// </summary>
        public void RefreshObjects() {

            toBeRefreshed.AddRange(this.SourceModels);
            TreeListView tlv = this.SourceListView as TreeListView;
            if (tlv == null)
                this.SourceListView.RefreshObjects(toBeRefreshed);
            else
                tlv.RebuildAll(true);

            TreeListView tlv2 = this.ListView as TreeListView;
            if (tlv2 == null)
                this.ListView.RefreshObject(this.TargetModel);
            else
                tlv2.RebuildAll(true);
        }
    }
}

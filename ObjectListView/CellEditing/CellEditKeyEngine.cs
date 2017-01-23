/*
 * CellEditKeyEngine - A engine that allows the behaviour of arbitrary keys to be configured
 *
 * Author: Phillip Piper
 * Date: 3-March-2011 10:53 pm
 *
 * Change log:
 * v2.8
 * 2014-05-30  JPP  - When a row is disabled, skip over it when looking for another cell to edit
 * v2.5
 * 2012-04-14  JPP  - Fixed bug where, on a OLV with only a single editable column, tabbing
 *                    to change rows would edit the cell above rather than the cell below
 *                    the cell being edited.
 * 2.5
 * 2011-03-03  JPP  - First version
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
using System.Text;
using System.Windows.Forms;
using BrightIdeasSoftware;

namespace BrightIdeasSoftware {
    /// <summary>
    /// Indicates the behavior of a key when a cell "on the edge" is being edited.
    /// and the normal behavior of that key would exceed the edge. For example,
    /// for a key that normally moves one column to the left, the "edge" would be 
    /// the left most column, since the normal action of the key cannot be taken
    /// (since there are no more columns to the left).
    /// </summary>
    public enum CellEditAtEdgeBehaviour {
        /// <summary>
        /// The key press will be ignored
        /// </summary>
        Ignore,

        /// <summary>
        /// The key press will result in the cell editing wrapping to the 
        /// cell on the opposite edge.
        /// </summary>
        Wrap,

        /// <summary>
        /// The key press will wrap, but the column will be changed to the 
        /// appropiate adjacent column. This only makes sense for keys where
        /// the normal action is ChangeRow.
        /// </summary>
        ChangeColumn,

        /// <summary>
        /// The key press will wrap, but the row will be changed to the 
        /// appropiate adjacent row. This only makes sense for keys where
        /// the normal action is ChangeColumn.
        /// </summary>        
        ChangeRow,

        /// <summary>
        /// The key will result in the current edit operation being ended.
        /// </summary>
        EndEdit
    };

    /// <summary>
    /// Indicates the normal behaviour of a key when used during a cell edit
    /// operation.
    /// </summary>
    public enum CellEditCharacterBehaviour {
        /// <summary>
        /// The key press will be ignored
        /// </summary>
        Ignore,

        /// <summary>
        /// The key press will end the current edit and begin an edit
        /// operation on the next editable cell to the left.
        /// </summary>
        ChangeColumnLeft,

        /// <summary>
        /// The key press will end the current edit and begin an edit
        /// operation on the next editable cell to the right.
        /// </summary>
        ChangeColumnRight,

        /// <summary>
        /// The key press will end the current edit and begin an edit
        /// operation on the row above.
        /// </summary>
        ChangeRowUp,

        /// <summary>
        /// The key press will end the current edit and begin an edit
        /// operation on the row below
        /// </summary>
        ChangeRowDown,

        /// <summary>
        /// The key press will cancel the current edit
        /// </summary>
        CancelEdit,

        /// <summary>
        /// The key press will finish the current edit operation
        /// </summary>
        EndEdit,

        /// <summary>
        /// Custom verb that can be used for specialized actions.
        /// </summary>
        CustomVerb1,

        /// <summary>
        /// Custom verb that can be used for specialized actions.
        /// </summary>
        CustomVerb2,

        /// <summary>
        /// Custom verb that can be used for specialized actions.
        /// </summary>
        CustomVerb3,

        /// <summary>
        /// Custom verb that can be used for specialized actions.
        /// </summary>
        CustomVerb4,

        /// <summary>
        /// Custom verb that can be used for specialized actions.
        /// </summary>
        CustomVerb5,

        /// <summary>
        /// Custom verb that can be used for specialized actions.
        /// </summary>
        CustomVerb6,

        /// <summary>
        /// Custom verb that can be used for specialized actions.
        /// </summary>
        CustomVerb7,

        /// <summary>
        /// Custom verb that can be used for specialized actions.
        /// </summary>
        CustomVerb8,

        /// <summary>
        /// Custom verb that can be used for specialized actions.
        /// </summary>
        CustomVerb9,

        /// <summary>
        /// Custom verb that can be used for specialized actions.
        /// </summary>
        CustomVerb10,
    };

    /// <summary>
    /// Instances of this class handle key presses during a cell edit operation.
    /// </summary>
    public class CellEditKeyEngine {

        #region Public interface

        /// <summary>
        /// Sets the behaviour of a given key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="normalBehaviour"></param>
        /// <param name="atEdgeBehaviour"></param>
        public virtual void SetKeyBehaviour(Keys key, CellEditCharacterBehaviour normalBehaviour, CellEditAtEdgeBehaviour atEdgeBehaviour) {
            this.CellEditKeyMap[key] = normalBehaviour;
            this.CellEditKeyAtEdgeBehaviourMap[key] = atEdgeBehaviour;
        }

        /// <summary>
        /// Handle a key press
        /// </summary>
        /// <param name="olv"></param>
        /// <param name="keyData"></param>
        /// <returns>True if the key was completely handled.</returns>
        public virtual bool HandleKey(ObjectListView olv, Keys keyData) {
            if (olv == null) throw new ArgumentNullException("olv");

            CellEditCharacterBehaviour behaviour;
            if (!CellEditKeyMap.TryGetValue(keyData, out behaviour))
                return false;

            this.ListView = olv;

            switch (behaviour) {
            case CellEditCharacterBehaviour.Ignore:
                break;
            case CellEditCharacterBehaviour.CancelEdit:
                this.HandleCancelEdit();
                break;
            case CellEditCharacterBehaviour.EndEdit:
                this.HandleEndEdit();
                break;
            case CellEditCharacterBehaviour.ChangeColumnLeft:
            case CellEditCharacterBehaviour.ChangeColumnRight:
                this.HandleColumnChange(keyData, behaviour);
                break;
            case CellEditCharacterBehaviour.ChangeRowDown:
            case CellEditCharacterBehaviour.ChangeRowUp:
                this.HandleRowChange(keyData, behaviour);
                break;
            default:
                return this.HandleCustomVerb(keyData, behaviour);
            };

            return true;
        }
        
        #endregion

        #region Implementation properties

        /// <summary>
        /// Gets or sets the ObjectListView on which the current key is being handled.
        /// This cannot be null.
        /// </summary>
        protected ObjectListView ListView {
            get { return listView; }
            set { listView = value; }
        }
        private ObjectListView listView;

        /// <summary>
        /// Gets the row of the cell that is currently being edited
        /// </summary>
        protected OLVListItem ItemBeingEdited {
            get {
                return (this.ListView == null || this.ListView.CellEditEventArgs == null) ? null : this.ListView.CellEditEventArgs.ListViewItem;
            }
        }

        /// <summary>
        /// Gets the index of the column of the cell that is being edited
        /// </summary>
        protected int SubItemIndexBeingEdited {
            get {
                return (this.ListView == null || this.ListView.CellEditEventArgs == null) ? -1 : this.ListView.CellEditEventArgs.SubItemIndex;
            }
        }

        /// <summary>
        /// Gets or sets the map that remembers the normal behaviour of keys
        /// </summary>
        protected IDictionary<Keys, CellEditCharacterBehaviour> CellEditKeyMap {
            get {
                if (cellEditKeyMap == null)
                    this.InitializeCellEditKeyMaps();
                return cellEditKeyMap;
            }
            set {
                cellEditKeyMap = value;
            }
        }
        private IDictionary<Keys, CellEditCharacterBehaviour> cellEditKeyMap;

        /// <summary>
        /// Gets or sets the map that remembers the desired behaviour of keys 
        /// on edge cases.
        /// </summary>
        protected IDictionary<Keys, CellEditAtEdgeBehaviour> CellEditKeyAtEdgeBehaviourMap {
            get {
                if (cellEditKeyAtEdgeBehaviourMap == null)
                    this.InitializeCellEditKeyMaps();
                return cellEditKeyAtEdgeBehaviourMap;
            }
            set {
                cellEditKeyAtEdgeBehaviourMap = value;
            }
        }
        private IDictionary<Keys, CellEditAtEdgeBehaviour> cellEditKeyAtEdgeBehaviourMap;

        #endregion

        #region Initialization

        /// <summary>
        /// Setup the default key mapping
        /// </summary>
        protected virtual void InitializeCellEditKeyMaps() {
            this.cellEditKeyMap = new Dictionary<Keys, CellEditCharacterBehaviour>();
            this.cellEditKeyMap[Keys.Escape] = CellEditCharacterBehaviour.CancelEdit;
            this.cellEditKeyMap[Keys.Return] = CellEditCharacterBehaviour.EndEdit;
            this.cellEditKeyMap[Keys.Enter] = CellEditCharacterBehaviour.EndEdit;
            this.cellEditKeyMap[Keys.Tab] = CellEditCharacterBehaviour.ChangeColumnRight;
            this.cellEditKeyMap[Keys.Tab | Keys.Shift] = CellEditCharacterBehaviour.ChangeColumnLeft;
            this.cellEditKeyMap[Keys.Left | Keys.Alt] = CellEditCharacterBehaviour.ChangeColumnLeft;
            this.cellEditKeyMap[Keys.Right | Keys.Alt] = CellEditCharacterBehaviour.ChangeColumnRight;
            this.cellEditKeyMap[Keys.Up | Keys.Alt] = CellEditCharacterBehaviour.ChangeRowUp;
            this.cellEditKeyMap[Keys.Down | Keys.Alt] = CellEditCharacterBehaviour.ChangeRowDown;

            this.cellEditKeyAtEdgeBehaviourMap = new Dictionary<Keys, CellEditAtEdgeBehaviour>();
            this.cellEditKeyAtEdgeBehaviourMap[Keys.Tab] = CellEditAtEdgeBehaviour.Wrap;
            this.cellEditKeyAtEdgeBehaviourMap[Keys.Tab | Keys.Shift] = CellEditAtEdgeBehaviour.Wrap;
            this.cellEditKeyAtEdgeBehaviourMap[Keys.Left | Keys.Alt] = CellEditAtEdgeBehaviour.Wrap;
            this.cellEditKeyAtEdgeBehaviourMap[Keys.Right | Keys.Alt] = CellEditAtEdgeBehaviour.Wrap;
            this.cellEditKeyAtEdgeBehaviourMap[Keys.Up | Keys.Alt] = CellEditAtEdgeBehaviour.ChangeColumn;
            this.cellEditKeyAtEdgeBehaviourMap[Keys.Down | Keys.Alt] = CellEditAtEdgeBehaviour.ChangeColumn;
        }

        #endregion

        #region Command handling

        /// <summary>
        /// Handle the end edit command
        /// </summary>
        protected virtual void HandleEndEdit() {
            this.ListView.PossibleFinishCellEditing();
        }

        /// <summary>
        /// Handle the cancel edit command
        /// </summary>
        protected virtual void HandleCancelEdit() {
            this.ListView.CancelCellEdit();
        }

        /// <summary>
        /// Placeholder that subclasses can override to handle any custom verbs
        /// </summary>
        /// <param name="keyData"></param>
        /// <param name="behaviour"></param>
        /// <returns></returns>
        protected virtual bool HandleCustomVerb(Keys keyData, CellEditCharacterBehaviour behaviour) {
            return false;
        }

        /// <summary>
        /// Handle a change row command
        /// </summary>
        /// <param name="keyData"></param>
        /// <param name="behaviour"></param>
        protected virtual void HandleRowChange(Keys keyData, CellEditCharacterBehaviour behaviour) {
            // If we couldn't finish editing the current cell, don't try to move it
            if (!this.ListView.PossibleFinishCellEditing())
                return;

            OLVListItem olvi = this.ItemBeingEdited;
            int subItemIndex = this.SubItemIndexBeingEdited;
            bool isGoingUp = behaviour == CellEditCharacterBehaviour.ChangeRowUp;

            // Try to find a row above (or below) the currently edited cell
            // If we find one, start editing it and we're done.
            OLVListItem adjacentOlvi = this.GetAdjacentItemOrNull(olvi, isGoingUp);
            if (adjacentOlvi != null) {
                this.StartCellEditIfDifferent(adjacentOlvi, subItemIndex);
                return;
            }

            // There is no adjacent row in the direction we want, so we must be on an edge.
            CellEditAtEdgeBehaviour atEdgeBehaviour;
            if (!this.CellEditKeyAtEdgeBehaviourMap.TryGetValue(keyData, out atEdgeBehaviour))
                atEdgeBehaviour = CellEditAtEdgeBehaviour.Wrap;
            switch (atEdgeBehaviour) {
            case CellEditAtEdgeBehaviour.Ignore:
                break;
            case CellEditAtEdgeBehaviour.EndEdit:
                this.ListView.PossibleFinishCellEditing();
                break;
            case CellEditAtEdgeBehaviour.Wrap:
                adjacentOlvi = this.GetAdjacentItemOrNull(null, isGoingUp);
                this.StartCellEditIfDifferent(adjacentOlvi, subItemIndex);
                break;
            case CellEditAtEdgeBehaviour.ChangeColumn:
                // Figure out the next editable column
                List<OLVColumn> editableColumnsInDisplayOrder = this.EditableColumnsInDisplayOrder;
                int displayIndex = Math.Max(0, editableColumnsInDisplayOrder.IndexOf(this.ListView.GetColumn(subItemIndex)));
                if (isGoingUp)
                    displayIndex = (editableColumnsInDisplayOrder.Count + displayIndex - 1) % editableColumnsInDisplayOrder.Count;
                else
                    displayIndex = (displayIndex + 1) % editableColumnsInDisplayOrder.Count;
                subItemIndex = editableColumnsInDisplayOrder[displayIndex].Index;

                // Wrap to the next row and start the cell edit
                adjacentOlvi = this.GetAdjacentItemOrNull(null, isGoingUp);
                this.StartCellEditIfDifferent(adjacentOlvi, subItemIndex);
                break;
            }
        }

        /// <summary>
        /// Handle a change column command
        /// </summary>
        /// <param name="keyData"></param>
        /// <param name="behaviour"></param>
        protected virtual void HandleColumnChange(Keys keyData, CellEditCharacterBehaviour behaviour)
        {
            // If we couldn't finish editing the current cell, don't try to move it
            if (!this.ListView.PossibleFinishCellEditing())
                return;

            // Changing columns only works in details mode
            if (this.ListView.View != View.Details)
                return;

            List<OLVColumn> editableColumns = this.EditableColumnsInDisplayOrder;
            OLVListItem olvi = this.ItemBeingEdited;
            int displayIndex = Math.Max(0,
                editableColumns.IndexOf(this.ListView.GetColumn(this.SubItemIndexBeingEdited)));
            bool isGoingLeft = behaviour == CellEditCharacterBehaviour.ChangeColumnLeft;

            // Are we trying to continue past one of the edges?
            if ((isGoingLeft && displayIndex == 0) ||
                (!isGoingLeft && displayIndex == editableColumns.Count - 1))
            {
                // Yes, so figure out our at edge behaviour
                CellEditAtEdgeBehaviour atEdgeBehaviour;
                if (!this.CellEditKeyAtEdgeBehaviourMap.TryGetValue(keyData, out atEdgeBehaviour))
                    atEdgeBehaviour = CellEditAtEdgeBehaviour.Wrap;
                switch (atEdgeBehaviour)
                {
                    case CellEditAtEdgeBehaviour.Ignore:
                        return;
                    case CellEditAtEdgeBehaviour.EndEdit:
                        this.HandleEndEdit();
                        return;
                    case CellEditAtEdgeBehaviour.ChangeRow:
                    case CellEditAtEdgeBehaviour.Wrap:
                        if (atEdgeBehaviour == CellEditAtEdgeBehaviour.ChangeRow)
                            olvi = GetAdjacentItem(olvi, isGoingLeft && displayIndex == 0);
                        if (isGoingLeft)
                            displayIndex = editableColumns.Count - 1;
                        else
                            displayIndex = 0;
                        break;
                }
            }
            else
            {
                if (isGoingLeft)
                    displayIndex -= 1;
                else
                    displayIndex += 1;
            }

            int subItemIndex = editableColumns[displayIndex].Index;
            this.StartCellEditIfDifferent(olvi, subItemIndex);
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Start editing the indicated cell if that cell is not already being edited
        /// </summary>
        /// <param name="olvi">The row to edit</param>
        /// <param name="subItemIndex">The cell within that row to edit</param>
        protected void StartCellEditIfDifferent(OLVListItem olvi, int subItemIndex) {
            if (this.ItemBeingEdited == olvi && this.SubItemIndexBeingEdited == subItemIndex)
                return;

            this.ListView.EnsureVisible(olvi.Index);
            this.ListView.StartCellEdit(olvi, subItemIndex);
        }

        /// <summary>
        /// Gets the adjacent item to the given item in the given direction.
        /// If that item is disabled, continue in that direction until an enabled item is found.
        /// </summary>
        /// <param name="olvi">The row whose neighbour is sought</param>
        /// <param name="up">The direction of the adjacentness</param>
        /// <returns>An OLVListView adjacent to the given item, or null if there are no more enabled items in that direction.</returns>
        protected OLVListItem GetAdjacentItemOrNull(OLVListItem olvi, bool up) {
            OLVListItem item = up ? this.ListView.GetPreviousItem(olvi) : this.ListView.GetNextItem(olvi);
            while (item != null && !item.Enabled)
                item = up ? this.ListView.GetPreviousItem(item) : this.ListView.GetNextItem(item);
            return item;
        }

        /// <summary>
        /// Gets the adjacent item to the given item in the given direction, wrapping if needed.
        /// </summary>
        /// <param name="olvi">The row whose neighbour is sought</param>
        /// <param name="up">The direction of the adjacentness</param>
        /// <returns>An OLVListView adjacent to the given item, or null if there are no more items in that direction.</returns>
        protected OLVListItem GetAdjacentItem(OLVListItem olvi, bool up) {
            return this.GetAdjacentItemOrNull(olvi, up) ?? this.GetAdjacentItemOrNull(null, up);
        }

        /// <summary>
        /// Gets a collection of columns that are editable in the order they are shown to the user
        /// </summary>
        protected List<OLVColumn> EditableColumnsInDisplayOrder {
            get {
                List<OLVColumn> editableColumnsInDisplayOrder = new List<OLVColumn>();
                foreach (OLVColumn x in this.ListView.ColumnsInDisplayOrder)
                    if (x.IsEditable)
                        editableColumnsInDisplayOrder.Add(x);
                return editableColumnsInDisplayOrder;
            }
        }

        #endregion
    }
}

/*
 * ColumnSelectionForm - A utility form that allows columns to be rearranged and/or hidden
 *
 * Author: Phillip Piper
 * Date: 1/04/2011 11:15 AM
 *
 * Change log:
 * 2013-04-21  JPP  - Fixed obscure bug in column re-ordered. Thanks to Edwin Chen.
 */

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace BrightIdeasSoftware
{
    /// <summary>
    /// This form is an example of how an application could allows the user to select which columns 
    /// an ObjectListView will display, as well as select which order the columns are displayed in.
    /// </summary>
    /// <remarks>
    /// <para>In Tile view, ColumnHeader.DisplayIndex does nothing. To reorder the columns you have
    /// to change the order of objects in the Columns property.</para>
    /// <para>Remember that the first column is special!
    /// It has to remain the first column.</para>
    /// </remarks>
    public partial class ColumnSelectionForm : Form
    {
    	/// <summary>
    	/// Make a new ColumnSelectionForm
    	/// </summary>
        public ColumnSelectionForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Open this form so it will edit the columns that are available in the listview's current view
        /// </summary>
        /// <param name="olv">The ObjectListView whose columns are to be altered</param>
        public void OpenOn(ObjectListView olv)
        {
            OpenOn(olv, olv.View);
        }

        /// <summary>
        /// Open this form so it will edit the columns that are available in the given listview
        /// when the listview is showing the given type of view.
        /// </summary>
        /// <param name="olv">The ObjectListView whose columns are to be altered</param>
        /// <param name="view">The view that is to be altered. Must be View.Details or View.Tile</param>
        public void OpenOn(ObjectListView olv, View view)
        {
            if (view != View.Details && view != View.Tile)
                return;

            InitializeForm(olv, view);
            if (ShowDialog() == DialogResult.OK) 
                Apply(olv, view);
        }

        /// <summary>
        /// Initialize the form to show the columns of the given view
        /// </summary>
        /// <param name="olv"></param>
        /// <param name="view"></param>
        protected void InitializeForm(ObjectListView olv, View view)
        {
            AllColumns = olv.AllColumns;
            RearrangableColumns = new List<OLVColumn>(AllColumns);
            foreach (OLVColumn col in RearrangableColumns) {
                if (view == View.Details)
                    MapColumnToVisible[col] = col.IsVisible;
                else
                    MapColumnToVisible[col] = col.IsTileViewColumn;
            }
            RearrangableColumns.Sort(new SortByDisplayOrder(this));

            objectListView1.BooleanCheckStateGetter = delegate(Object rowObject) {
                return MapColumnToVisible[(OLVColumn)rowObject];
            };

            objectListView1.BooleanCheckStatePutter = delegate(Object rowObject, bool newValue) {
                // Some columns should always be shown, so ignore attempts to hide them
                OLVColumn column = (OLVColumn)rowObject;
                if (!column.CanBeHidden) 
                    return true;

                MapColumnToVisible[column] = newValue;
                EnableControls();
                return newValue;
            };

            objectListView1.SetObjects(RearrangableColumns);
            EnableControls();
        }
        private List<OLVColumn> AllColumns = null;
        private List<OLVColumn> RearrangableColumns = new();
        private Dictionary<OLVColumn, bool> MapColumnToVisible = new();

        /// <summary>
        /// The user has pressed OK. Do what's requied.
        /// </summary>
        /// <param name="olv"></param>
        /// <param name="view"></param>
        protected void Apply(ObjectListView olv, View view)
        {
            olv.Freeze();

            // Update the column definitions to reflect whether they have been hidden
            if (view == View.Details) {
                foreach (OLVColumn col in olv.AllColumns)
                    col.IsVisible = MapColumnToVisible[col];
            } else {
                foreach (OLVColumn col in olv.AllColumns)
                    col.IsTileViewColumn = MapColumnToVisible[col];
            }

            // Collect the columns are still visible
            List<OLVColumn> visibleColumns = RearrangableColumns.FindAll(
                delegate(OLVColumn x) { return MapColumnToVisible[x]; });

            // Detail view and Tile view have to be handled in different ways.
            if (view == View.Details) {
                // Of the still visible columns, change DisplayIndex to reflect their position in the rearranged list
                olv.ChangeToFilteredColumns(view);
                foreach (OLVColumn col in visibleColumns) {
                    col.DisplayIndex = visibleColumns.IndexOf((OLVColumn)col);
                    col.LastDisplayIndex = col.DisplayIndex;
                }
            } else {
                // In Tile view, DisplayOrder does nothing. So to change the display order, we have to change the 
                // order of the columns in the Columns property.
                // Remember, the primary column is special and has to remain first!
                OLVColumn primaryColumn = AllColumns[0];
                visibleColumns.Remove(primaryColumn);

                olv.Columns.Clear();
                olv.Columns.Add(primaryColumn);
                olv.Columns.AddRange(visibleColumns.ToArray());
                olv.CalculateReasonableTileSize();
            }

            olv.Unfreeze();
        }

        #region Event handlers

        private void buttonMoveUp_Click(object sender, EventArgs e)
        {
            int selectedIndex = objectListView1.SelectedIndices[0];
            OLVColumn col = RearrangableColumns[selectedIndex];
            RearrangableColumns.RemoveAt(selectedIndex);
            RearrangableColumns.Insert(selectedIndex-1, col);

            objectListView1.BuildList();

            EnableControls();
        }

        private void buttonMoveDown_Click(object sender, EventArgs e)
        {
            int selectedIndex = objectListView1.SelectedIndices[0];
            OLVColumn col = RearrangableColumns[selectedIndex];
            RearrangableColumns.RemoveAt(selectedIndex);
            RearrangableColumns.Insert(selectedIndex + 1, col);

            objectListView1.BuildList();

            EnableControls();
        }

        private void buttonShow_Click(object sender, EventArgs e)
        {
            objectListView1.SelectedItem.Checked = true;
        }

        private void buttonHide_Click(object sender, EventArgs e)
        {
            objectListView1.SelectedItem.Checked = false;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void objectListView1_SelectionChanged(object sender, EventArgs e)
        {
            EnableControls();
        }

        #endregion

        #region Control enabling

        /// <summary>
        /// Enable the controls on the dialog to match the current state
        /// </summary>
        protected void EnableControls()
        {
            if (objectListView1.SelectedIndices.Count == 0) {
                buttonMoveUp.Enabled = false;
                buttonMoveDown.Enabled = false;
                buttonShow.Enabled = false;
                buttonHide.Enabled = false;
            } else {
                // Can't move the first row up or the last row down
                buttonMoveUp.Enabled = (objectListView1.SelectedIndices[0] != 0);
                buttonMoveDown.Enabled = (objectListView1.SelectedIndices[0] < (objectListView1.GetItemCount() - 1));

                OLVColumn selectedColumn = (OLVColumn)objectListView1.SelectedObject;

                // Some columns cannot be hidden (and hence cannot be Shown)
                buttonShow.Enabled = !MapColumnToVisible[selectedColumn] && selectedColumn.CanBeHidden;
                buttonHide.Enabled = MapColumnToVisible[selectedColumn] && selectedColumn.CanBeHidden;
            }
        }
        #endregion

        /// <summary>
        /// A Comparer that will sort a list of columns so that visible ones come before hidden ones,
        /// and that are ordered by their display order.
        /// </summary>
        private class SortByDisplayOrder : IComparer<OLVColumn>
        {
            public SortByDisplayOrder(ColumnSelectionForm form)
            {
                Form = form;
            }
            private ColumnSelectionForm Form;

            #region IComparer<OLVColumn> Members

            int IComparer<OLVColumn>.Compare(OLVColumn x, OLVColumn y)
            {
                if (Form.MapColumnToVisible[x] && !Form.MapColumnToVisible[y])
                    return -1;

                if (!Form.MapColumnToVisible[x] && Form.MapColumnToVisible[y])
                    return 1;

                if (x.DisplayIndex == y.DisplayIndex)
                    return string.Compare(x.Text, y.Text, StringComparison.CurrentCultureIgnoreCase);
                else
                    return x.DisplayIndex - y.DisplayIndex;
            }

            #endregion
        }
    }
}

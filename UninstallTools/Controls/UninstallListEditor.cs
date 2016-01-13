using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Klocman.Forms.Tools;
using UninstallTools.Lists;

namespace UninstallTools.Controls
{
    public partial class UninstallListEditor : UserControl
    {
        private UninstallList _currentList;

        public UninstallListEditor()
        {
            InitializeComponent();

            filterEditor.ComparisonMethodChanged += OnFiltersChanged;
            filterEditor.LostFocus += EditorFocusLost;
        }

        [ReadOnly(true)]
        [Browsable(false)]
        public UninstallList CurrentList
        {
            get
            {
                if (_currentList == null)
                    throw new InvalidOperationException("CurrentList has to be set before use of this control");
                return _currentList;
            }
            set
            {
                _currentList = value;
                PopulateList();
            }
        }

        [ReadOnly(true)]
        [Browsable(false)]
        private UninstallListItem CurrentlySelected
        {
            get
            {
                if (listView1.SelectedItems.Count <= 0)
                    return null;
                return listView1.SelectedItems[0].Tag as UninstallListItem;
            }
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            //var item = GetNewItem();
            //if (item == null)
            {
                var newItem = new UninstallListItem();
                CurrentList.Add(newItem);
                PopulateList();
                //item = GetNewItem();
            }

            /*if (item != null)
            {
                item.EnsureVisible();
                item.Selected = true;
            }*/
        }

        private void buttonImport_Click(object sender, EventArgs e)
        {
            openFileDialog.ShowDialog();
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            var item = CurrentlySelected;
            if (item == null)
                return;

            CurrentList.Remove(item);
            PopulateList();
        }

        private void EditorFocusLost(object sender, EventArgs e)
        {
            RefreshSelectedFilter();
        }

        /*private ListViewItem GetNewItem()
        {
            return listView1.Items.Cast<ListViewItem>().FirstOrDefault(x =>
            {
                var tag = x.Tag as UninstallListItem;
                return tag != null && tag.FilterText.Equals(Localisation.UninstallListEditor_NewFilter);
            });
        }*/

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshConditionList();
        }

        private void RefreshConditionList()
        {
            listBox1.SelectedItem = null;
            listBox1.Items.Clear();

            var selection = CurrentlySelected;
            if (selection?.ComparisonEntries != null)
            {
                groupBoxConditions.Enabled = true;
                listBox1.Items.AddRange(selection.ComparisonEntries.AsEnumerable().Reverse().ToArray());
            }
            else
            {
                groupBoxConditions.Enabled = false;
            }
        }

        private void openFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            try
            {
                CurrentList.AddItems(UninstallList.FromFiles(openFileDialog.FileNames).Items);
                PopulateList();
            }
            catch (Exception ex)
            {
                PremadeDialogs.GenericError(ex);
                e.Cancel = true;
            }
        }

        private void PopulateList()
        {
            groupBoxConditions.Enabled = false;

            listView1.Items.Clear();
            listView1.Items.AddRange(CurrentList.Items.Select(x => new ListViewItem(
                new[] { x.ToString() }) //TODO rest of the columns
            { Tag = x }).ToArray());
        }

        private void OnFiltersChanged(object sender, EventArgs e)
        {
            FiltersChanged?.Invoke(sender, e);
        }

        public event EventHandler FiltersChanged;

        private void RefreshSelectedFilter()
        {
            if (listView1.SelectedItems.Count <= 0)
                return;

            var item = listView1.SelectedItems[0];
            var tag = CurrentlySelected;
            item.SubItems[0].Text = tag.ToString();
            //item.SubItems[1].Text = tag.ComparisonMethod.GetLocalisedName(); TODO
            item.EnsureVisible();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item = listBox1.SelectedItem as ComparisonEntry;
            if (item != null && !ReferenceEquals(filterEditor.TargetComparisonEntry, item))
                filterEditor.TargetComparisonEntry = item;
        }

        private void toolStripButtonAddCondition_Click(object sender, EventArgs e)
        {
            CurrentlySelected.ComparisonEntries.Add(new ComparisonEntry());

            RefreshConditionList();
        }

        private void toolStripButtonRemoveCondition_Click(object sender, EventArgs e)
        {
            var item = listBox1.SelectedItem as ComparisonEntry;
            if (item == null) return;
            CurrentlySelected.ComparisonEntries.Remove(item);
            RefreshConditionList();
        }
    }
}
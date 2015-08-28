using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Klocman.Events;
using Klocman.Forms.Tools;
using Klocman.Localising;
using UninstallTools.Lists;
using UninstallTools.Properties;

namespace UninstallTools.Controls
{
    public partial class UninstallListEditor : UserControl
    {
        private UninstallList _currentList;

        public UninstallListEditor()
        {
            InitializeComponent();

            filterEditor.FilterTextChanged += FilterEditor_FilterTextChanged;
            filterEditor.ComparisonMethodChanged += FilterEditor_ComparisonMethodChanged;

            // Initialize the preview box
            textBoxPreview_TextChanged(this, EventArgs.Empty);
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
        public UninstallListItem CurrentlySelected
        {
            get
            {
                if (listView1.SelectedItems.Count <= 0)
                    return null;
                return listView1.SelectedItems[0].Tag as UninstallListItem;
            }
        }

        [ReadOnly(true)]
        [Browsable(false)]
        public IEnumerable<string> TestItems { get; set; }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            var item = GetNewItem();
            if (item == null)
            {
                CurrentList.AddItems(new[] {Localisation.UninstallListEditor_NewFilter});
                PopulateList();
                item = GetNewItem();
            }

            if (item != null)
            {
                item.EnsureVisible();
                item.Selected = true;
            }
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
            RefreshSelectedItem();
        }

        private void FilterEditor_ComparisonMethodChanged(object sender,
            PropertyChangedEventArgs<FilterComparisonMethod> e)
        {
            var selection = CurrentlySelected;

            if (selection == null)
                return;

            selection.ComparisonMethod = e.NewValue;

            RefreshPreview();
        }

        private void FilterEditor_FilterTextChanged(object sender, PropertyChangedEventArgs<string> e)
        {
            var selection = CurrentlySelected;
            selection.FilterText = e.NewValue;

            RefreshPreview();
        }

        private ListViewItem GetNewItem()
        {
            return listView1.Items.Cast<ListViewItem>().FirstOrDefault(x =>
            {
                var tag = x.Tag as UninstallListItem;
                return tag != null && tag.FilterText.Equals(Localisation.UninstallListEditor_NewFilter);
            });
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selection = CurrentlySelected;

            if (selection == null)
            {
                groupBoxEditor.Enabled = false;
                return;
            }
            groupBoxEditor.Enabled = true;

            filterEditor.FilterText = selection.FilterText;
            filterEditor.ComparisonMethod = selection.ComparisonMethod;
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
            groupBoxEditor.Enabled = false;

            listView1.Items.Clear();
            listView1.Items.AddRange(CurrentList.Items.Select(x => new ListViewItem(
                new[] {x.FilterText, x.ComparisonMethod.GetLocalisedName()})
            {Tag = x}).ToArray());
        }

        private void RefreshPreview()
        {
            try
            {
                textBoxPreview.Text = string.Join(Environment.NewLine,
                    TestItems.Where(x => CurrentlySelected.TestString(x)).ToArray());
            }
            catch
            {
                textBoxPreview.Text = Localisation.UninstallListEditor_InvalidFilter;
            }
        }

        private void RefreshSelectedItem()
        {
            if (listView1.SelectedItems.Count <= 0)
                return;

            var item = listView1.SelectedItems[0];
            var tag = CurrentlySelected;
            item.SubItems[0].Text = tag.FilterText;
            item.SubItems[1].Text = tag.ComparisonMethod.GetLocalisedName();
            item.EnsureVisible();
        }

        private void textBoxPreview_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxPreview.Text))
                textBoxPreview.Text = Localisation.UninstallListEditor_NothingMatched;
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UniversalUninstaller
{
    public partial class TargetList : UserControl
    {
        public TargetList()
        {
            InitializeComponent();

            treeListView1.CanExpandGetter = model => ((TreeEntry)model).IsDirectory;
            treeListView1.ChildrenGetter = ChildrenGetter;

            treeListView1.HierarchicalCheckboxes = false;
            treeListView1.UseWaitCursorWhenExpanding = true;
            treeListView1.PersistentCheckBoxes = false;

            treeListView1.CellToolTipGetter = (column, modelObject) => (modelObject as TreeEntry)?.FileSystemInfo.FullName;

            treeListView1.BooleanCheckStateGetter = BooleanCheckStateGetter;
            treeListView1.BooleanCheckStatePutter = BooleanCheckStatePutter;

            olvColumn1.AspectGetter = rowObject => (rowObject as TreeEntry)?.FileSystemInfo.Name;
        }

        private IEnumerable ChildrenGetter(object model)
        {
            return (((TreeEntry) model).FileSystemInfo as DirectoryInfo)?.GetFileSystemInfos()
                .Select(x => new TreeEntry(x));
        }

        private bool BooleanCheckStatePutter(object rowObject, bool value)
        {
            if (value)
            {
                treeListView1.CheckObjects(treeListView1.GetChildren(rowObject));
            }
            else
            {
                var parent = treeListView1.GetParent(rowObject) as TreeEntry;
                if (parent != null && parent.Checked)
                {
                    treeListView1.UncheckObject(parent);
                }
            }

            return ((TreeEntry) rowObject).Checked = value;
        }

        private bool BooleanCheckStateGetter(object rowObject)
        {
            return ((TreeEntry) rowObject).Checked;
        }

        public void Populate(DirectoryInfo rootDirectory)
        {
            treeListView1.ClearObjects();
            var root = new TreeEntry(rootDirectory);
            treeListView1.AddObject(root);
            treeListView1.Expand(root);

            // If there is no executable in the root, expand the subdirectories to show more info
            var subs = treeListView1.GetChildren(rootDirectory).Cast<TreeEntry>().ToList();
            if (subs.All(x => x.IsDirectory))
            {
                foreach (var dir in subs)
                    treeListView1.Expand(dir);
            }

            treeListView1.CheckAll();
        }

        public IEnumerable<FileSystemInfo> GetItemsToDelete()
        {
            return treeListView1.Roots.Cast<TreeEntry>().SelectMany(GetSelectedItems);
        }


        public IEnumerable<FileSystemInfo> GetSelectedItems(object modelItem)
        {
            var treeEntry = modelItem as TreeEntry;

            if (treeEntry == null)
                return Enumerable.Empty<FileSystemInfo>();

            if (treeEntry.Checked)
                return Enumerable.Repeat(treeEntry.FileSystemInfo, 1);

            return GetSelectedItems(treeEntry);
        }

        private void toolStripButtonSelAll_Click(object sender, EventArgs e)
        {
            treeListView1.CheckAll();
        }
    }

    public class TreeEntry
    {
        public TreeEntry(FileSystemInfo fileSystemInfo)
        {
            if (fileSystemInfo == null) throw new ArgumentNullException(nameof(fileSystemInfo));
            FileSystemInfo = fileSystemInfo;
            IsDirectory = fileSystemInfo is DirectoryInfo;
            Checked = true;
        }

        public bool IsDirectory { get; }
        public FileSystemInfo FileSystemInfo { get; }
        public bool Checked { get; set; }
    }

}

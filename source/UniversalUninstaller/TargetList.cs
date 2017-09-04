using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Klocman.IO;
using Klocman.Tools;
using UniversalUninstaller.Properties;

namespace UniversalUninstaller
{
    public partial class TargetList : UserControl
    {
        public TargetList()
        {
            InitializeComponent();

            treeListView1.CanExpandGetter = model => ((TreeEntry) model).IsDirectory;
            treeListView1.ChildrenGetter = ChildrenGetter;

            treeListView1.HierarchicalCheckboxes = false;
            treeListView1.UseWaitCursorWhenExpanding = true;
            treeListView1.PersistentCheckBoxes = false;

            treeListView1.CellToolTipGetter =
                (column, modelObject) => (modelObject as TreeEntry)?.FileSystemInfo.FullName;

            treeListView1.BooleanCheckStateGetter = BooleanCheckStateGetter;
            treeListView1.BooleanCheckStatePutter = BooleanCheckStatePutter;

            olvColumn1.AspectGetter = rowObject => (rowObject as TreeEntry)?.FileSystemInfo.Name;
            olvColumn1.ImageGetter = ImageGetter;

            olvColumnSize.AspectGetter = rowObject =>
            {
                var treeEntry = rowObject as TreeEntry;
                return treeEntry?.IsDirectory == false
                    ? FileSize.FromBytes(((FileInfo) treeEntry.FileSystemInfo).Length)
                    : FileSize.Empty;
            };

            var il = new ImageList {ColorDepth = ColorDepth.Depth24Bit, ImageSize = new Size(20, 20)};
            il.Images.Add(Resources.folder);
            il.Images.Add(Resources.page_text);
            il.Images.Add(Resources.app);
            treeListView1.SmallImageList = il;
        }

        private object ImageGetter(object rowObject)
        {
            var treeEntry = rowObject as TreeEntry;

            if (treeEntry != null)
            {
                if (treeEntry.IsDirectory)
                    return 0;

                if (WindowsTools.IsExectuable(treeEntry.FileSystemInfo.FullName, false))
                    return 2;
            }

            return 1;
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

            return treeListView1.GetChildren(modelItem).Cast<object>().SelectMany(GetSelectedItems);
        }

        private void toolStripButtonSelAll_Click(object sender, EventArgs e)
        {
            treeListView1.CheckAll();
        }

        private void expand_Click(object sender, EventArgs e)
        {
            treeListView1.ExpandAll();
        }

        private void collapse_Click(object sender, EventArgs e)
        {
            treeListView1.CollapseAll();
        }

        private void treeListView1_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            treeListView1.Refresh();
        }
    }
}
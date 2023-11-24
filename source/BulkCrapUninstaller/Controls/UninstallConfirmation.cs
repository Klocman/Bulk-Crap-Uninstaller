using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using BrightIdeasSoftware;
using BulkCrapUninstaller.Functions;
using UninstallTools.Uninstaller;

namespace BulkCrapUninstaller.Forms
{
    public partial class UninstallConfirmation : UserControl
    {
        public UninstallConfirmation()
        {
            InitializeComponent();

            olvColumnEnabled.AspectGetter = rowObject => ((ConfirmationEntry) rowObject).Enabled;
            olvColumnEnabled.AspectPutter = (rowObject, value) => ((ConfirmationEntry) rowObject).Enabled = (bool) value;

            olvColumnQuiet.AspectGetter = rowObject => ((ConfirmationEntry) rowObject).Entry.IsSilentPossible;
            olvColumnQuiet.AspectPutter = (rowObject, value) =>
            {
                var entry = ((ConfirmationEntry) rowObject).Entry;
                entry.IsSilentPossible = (bool) value && entry.UninstallerEntry.QuietUninstallPossible;
            };

            olvColumnInstallLocation.AspectGetter =
                rowObject => ((ConfirmationEntry) rowObject).Entry.UninstallerEntry.InstallLocation;
            olvColumnName.AspectGetter = rowObject => ((ConfirmationEntry) rowObject).Entry.UninstallerEntry.DisplayName;

            uninstallerObjectListView.DragSource = new SimpleDragSource();
            var rearrangingDropSink = new RearrangingDropSink(false);
            uninstallerObjectListView.DropSink = rearrangingDropSink;
            rearrangingDropSink.Dropped += (sender, args) =>
            {
                uninstallerObjectListView.PrimarySortColumn = null;
                uninstallerObjectListView.Sort();
            };

            // Bug - sorting by column doesn't change the actual order, and disabling sorting doesn't do anything 
            uninstallerObjectListView.HeaderStyle = ColumnHeaderStyle.Nonclickable;
        }

        private IEnumerable<ConfirmationEntry> Entries => (uninstallerObjectListView.Objects ?? Enumerable.Empty<ConfirmationEntry>()).Cast<ConfirmationEntry>();

        private void buttonSort_Click(object sender, EventArgs e)
        {
            uninstallerObjectListView.PrimarySortColumn = null;
            var results = AppUninstaller.SortIntelligently(Entries, entry => entry.Entry);
            uninstallerObjectListView.SetObjects(results.ToList());
        }

        public IEnumerable<BulkUninstallEntry> GetResults()
        {
            return Entries.Where(x => x.Enabled).Select(x => x.Entry);
        }

        public void SetRelatedApps(IEnumerable<BulkUninstallEntry> items)
        {
            var entries = items.Select(x => new ConfirmationEntry(x));

            uninstallerObjectListView.SetObjects(entries.ToList());
        }

        private sealed class ConfirmationEntry
        {
            public ConfirmationEntry(BulkUninstallEntry entry)
            {
                Entry = entry;
                Enabled = true;
            }

            public bool Enabled { get; set; }

            public BulkUninstallEntry Entry { get; }
        }
    }
}
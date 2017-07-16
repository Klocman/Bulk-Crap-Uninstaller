using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using UninstallTools.Uninstaller;

namespace BulkCrapUninstaller.Forms
{
    public partial class UninstallConfirmationWindow : Form
    {
        private List<BulkUninstallEntry> _results;

        public UninstallConfirmationWindow()
        {
            InitializeComponent();

            olvColumnEnabled.AspectGetter = rowObject => ((ConfirmationEntry) rowObject).Enabled;
            olvColumnEnabled.AspectPutter = (rowObject, value) => ((ConfirmationEntry) rowObject).Enabled = (bool) value;

            olvColumnQuiet.AspectGetter = rowObject => ((ConfirmationEntry) rowObject).Entry.IsSilent;
            olvColumnQuiet.AspectPutter = (rowObject, value) =>
            {
                var entry = ((ConfirmationEntry)rowObject).Entry;
                entry.IsSilent = (bool) value && entry.UninstallerEntry.QuietUninstallPossible;
            };

            olvColumnInstallLocation.AspectGetter = rowObject => ((ConfirmationEntry) rowObject).Entry.UninstallerEntry.InstallLocation;
            olvColumnName.AspectGetter = rowObject => ((ConfirmationEntry) rowObject).Entry.UninstallerEntry.DisplayName;
        }

        private void buttonAccept_Click(object sender, EventArgs e)
        {
            _results = objectListView1.Objects
                .Cast<ConfirmationEntry>()
                .Where(x => x.Enabled)
                .Select(x => x.Entry)
                .ToList();

            Close();
        }

        public static List<BulkUninstallEntry> ShowConfirmationDialog(Form owner, List<BulkUninstallEntry> targets)
        {
            using (var window = new UninstallConfirmationWindow())
            {
                window.Icon = owner.Icon;
                window.Parent = owner;
                window.StartPosition = FormStartPosition.CenterParent;

                var entries = targets.Select(x => new ConfirmationEntry(x));

                window.objectListView1.SetObjects(entries.ToList());

                if (window.ShowDialog(owner) != DialogResult.OK)
                    return null;

                return window._results;
            }
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
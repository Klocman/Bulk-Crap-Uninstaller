/*
    EBUninstaller Pro - Context Menu Manager Window
    Modern GUI for managing, auditing, and cleaning Windows Explorer context menu extensions.
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using UninstallTools.Localization;
using UninstallTools.WindowsIntegration;

namespace BulkCrapUninstaller.Forms.Windows
{
    public class ContextMenuManagerWindow : Form
    {
        private ListView _listView = null!;
        private ToolStrip _toolStrip = null!;
        private ToolStripButton _refreshBtn = null!;
        private ToolStripButton _toggleStatusBtn = null!;
        private ToolStripButton _cleanOrphanedBtn = null!;
        private ToolStripComboBox _filterBox = null!;
        private StatusStrip _statusStrip = null!;
        private ToolStripStatusLabel _statusLabel = null!;

        private List<ContextMenuItem> _items = new();

        public ContextMenuManagerWindow()
        {
            InitializeComponent();
            ApplyTheme();
            Load += async (s, e) => await RefreshItemsAsync();
        }

        private void InitializeComponent()
        {
            Text = LanguageManager.GetString("ContextMenuManager_Title") ?? "Explorer Context Menu Manager - EBUninstaller Pro";
            Size = new Size(1000, 600);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(800, 450);

            _toolStrip = new ToolStrip { ImageList = null, RenderMode = ToolStripRenderMode.System, GripStyle = ToolStripGripStyle.Hidden };
            _refreshBtn = new ToolStripButton("🔄 Refresh", null, async (s, e) => await RefreshItemsAsync());
            _toggleStatusBtn = new ToolStripButton("⚡ Enable / Disable", null, (s, e) => ToggleSelectedStatus());
            _cleanOrphanedBtn = new ToolStripButton("🧹 Clean Orphaned", null, (s, e) => CleanOrphanedItems());

            _filterBox = new ToolStripComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            _filterBox.Items.AddRange(new object[] { "All Locations", "Files (*)", "Directories", "Directory Background", "Folder", "Drive" });
            _filterBox.SelectedIndex = 0;
            _filterBox.SelectedIndexChanged += (s, e) => ApplyFilter();

            _toolStrip.Items.Add(_refreshBtn);
            _toolStrip.Items.Add(new ToolStripSeparator());
            _toolStrip.Items.Add(_toggleStatusBtn);
            _toolStrip.Items.Add(_cleanOrphanedBtn);
            _toolStrip.Items.Add(new ToolStripSeparator());
            _toolStrip.Items.Add(new ToolStripLabel("Filter: "));
            _toolStrip.Items.Add(_filterBox);

            _listView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                MultiSelect = true
            };
            _listView.Columns.Add("Extension Name", 220);
            _listView.Columns.Add("Location", 140);
            _listView.Columns.Add("Status", 100);
            _listView.Columns.Add("Publisher / Company", 180);
            _listView.Columns.Add("CLSID", 240);
            _listView.Columns.Add("Target Module Path", 350);

            _statusStrip = new StatusStrip();
            _statusLabel = new ToolStripStatusLabel { Spring = true, TextAlign = ContentAlignment.MiddleLeft };
            _statusStrip.Items.Add(_statusLabel);

            Controls.Add(_listView);
            Controls.Add(_toolStrip);
            Controls.Add(_statusStrip);
        }

        private async Task RefreshItemsAsync()
        {
            _statusLabel.Text = "Scanning Windows Explorer context menu handlers...";
            _refreshBtn.Enabled = false;

            _items = await Task.Run(() => ContextMenuManager.GetContextMenuItems());

            ApplyFilter();
            _refreshBtn.Enabled = true;
            _statusLabel.Text = $"Found {_items.Count} context menu shell handlers ({_items.Count(i => i.IsOrphaned)} orphaned).";
        }

        private void ApplyFilter()
        {
            _listView.Items.Clear();
            string selectedFilter = _filterBox.SelectedItem?.ToString() ?? "All Locations";

            var filtered = _items.Where(item =>
            {
                if (selectedFilter == "Files (*)") return item.LocationType == "*";
                if (selectedFilter == "Directories") return item.LocationType.Equals("Directory", StringComparison.OrdinalIgnoreCase);
                if (selectedFilter == "Directory Background") return item.LocationType.IndexOf("Background", StringComparison.OrdinalIgnoreCase) >= 0;
                if (selectedFilter == "Folder") return item.LocationType.Equals("Folder", StringComparison.OrdinalIgnoreCase);
                if (selectedFilter == "Drive") return item.LocationType.Equals("Drive", StringComparison.OrdinalIgnoreCase);
                return true;
            }).ToList();

            foreach (var item in filtered)
            {
                var lvi = new ListViewItem(item.Name) { Tag = item };
                lvi.SubItems.Add(item.LocationType);

                string status = item.IsOrphaned ? "⚠️ Orphaned" : (item.IsEnabled ? "✓ Enabled" : "⊘ Disabled");
                lvi.SubItems.Add(status);
                lvi.SubItems.Add(string.IsNullOrEmpty(item.Publisher) ? "(Unknown)" : item.Publisher);
                lvi.SubItems.Add(item.Clsid);
                lvi.SubItems.Add(item.TargetModulePath);

                if (item.IsOrphaned)
                {
                    lvi.ForeColor = Color.FromArgb(180, 50, 50);
                }
                else if (!item.IsEnabled)
                {
                    lvi.ForeColor = Color.Gray;
                }
                else if (item.IsSystemCritical)
                {
                    lvi.ForeColor = Color.FromArgb(0, 100, 180);
                }

                _listView.Items.Add(lvi);
            }
        }

        private void ToggleSelectedStatus()
        {
            if (_listView.SelectedItems.Count == 0) return;

            foreach (ListViewItem lvi in _listView.SelectedItems)
            {
                if (lvi.Tag is ContextMenuItem item)
                {
                    if (item.IsSystemCritical)
                    {
                        MessageBox.Show($"'{item.Name}' is a protected Windows core shell component and cannot be disabled.", "Protected Component", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        continue;
                    }

                    bool newStatus = !item.IsEnabled;
                    if (ContextMenuManager.ToggleItemStatus(item, newStatus))
                    {
                        lvi.SubItems[2].Text = newStatus ? "✓ Enabled" : "⊘ Disabled";
                        lvi.ForeColor = newStatus ? Color.Black : Color.Gray;
                    }
                }
            }
        }

        private void CleanOrphanedItems()
        {
            var orphaned = _items.Where(i => i.IsOrphaned && !i.IsSystemCritical).ToList();
            if (orphaned.Count == 0)
            {
                MessageBox.Show("No orphaned context menu handlers detected.", "EBUninstaller Pro", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show($"Found {orphaned.Count} orphaned context menu entries whose target DLLs no longer exist. Clean them now?", "Confirm Cleanup", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            int cleaned = 0;
            foreach (var item in orphaned)
            {
                if (ContextMenuManager.DeleteItem(item))
                    cleaned++;
            }

            MessageBox.Show($"Successfully cleaned {cleaned} orphaned context menu handlers.", "Cleanup Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            _ = RefreshItemsAsync();
        }

        private void ApplyTheme()
        {
            bool isDark = ThemeManager.IsDarkModeEnabled;
            BackColor = isDark ? Color.FromArgb(32, 32, 32) : Color.FromArgb(245, 245, 245);
            ForeColor = isDark ? Color.White : Color.Black;

            if (LanguageManager.IsRightToLeft)
            {
                RightToLeft = RightToLeft.Yes;
                RightToLeftLayout = true;
            }
        }
    }
}

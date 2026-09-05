/*
    EBUninstaller Pro - WSL & Virtual Hard Disk Manager Window
    Modern GUI for auditing, shrinking, and unregistering WSL distributions and .vhdx files.
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using UninstallTools.Localization;
using UninstallTools.SystemTools;

namespace BulkCrapUninstaller.Forms.Windows
{
    public class WslManagerWindow : Form
    {
        private ListView _listView = null!;
        private CheckBox _chkOrphanedOnly = null!;
        private TextBox _searchBox = null!;
        private Button _refreshBtn = null!;
        private Button _compactBtn = null!;
        private Button _unregisterBtn = null!;
        private Button _showInExplorerBtn = null!;
        private Label _statsLabel = null!;
        private StatusStrip _statusStrip = null!;
        private ToolStripStatusLabel _statusLabel = null!;

        private List<WslDistroItem> _allDistros = new();

        public WslManagerWindow()
        {
            InitializeComponent();
            ApplyTheme();
            _ = LoadDistrosAsync();
        }

        private void InitializeComponent()
        {
            Text = LanguageManager.GetString("WslManager_Title") ?? "WSL & Virtual Hard Disk Manager - EBUninstaller Pro";
            Size = new Size(1050, 620);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(820, 450);

            // Top Panel
            var topPanel = new Panel { Dock = DockStyle.Top, Height = 78, Padding = new Padding(12, 8, 12, 8) };

            var searchLabel = new Label { Text = "Search:", Location = new Point(12, 12), AutoSize = true, Font = new Font("Segoe UI", 9f, FontStyle.Bold) };
            _searchBox = new TextBox { Location = new Point(70, 10), Width = 180, Font = new Font("Segoe UI", 9f) };
            _searchBox.TextChanged += (s, e) => ApplyFilter();

            _chkOrphanedOnly = new CheckBox { Text = "Orphaned Disks Only", Location = new Point(270, 11), AutoSize = true, Font = new Font("Segoe UI", 9f, FontStyle.Bold) };
            _chkOrphanedOnly.CheckedChanged += (s, e) => ApplyFilter();

            _refreshBtn = new Button { Text = "🔄 Refresh", Location = new Point(480, 8), Size = new Size(85, 28), Font = new Font("Segoe UI", 9f) };
            _refreshBtn.Click += async (s, e) => await LoadDistrosAsync();

            _showInExplorerBtn = new Button { Text = "📁 Show File", Location = new Point(570, 8), Size = new Size(90, 28), Enabled = false, Font = new Font("Segoe UI", 9f) };
            _showInExplorerBtn.Click += (s, e) => ShowSelectedInExplorer();

            _compactBtn = new Button { Text = "⚡ Compact / Shrink Disk", Location = new Point(665, 8), Size = new Size(175, 28), Enabled = false, Font = new Font("Segoe UI", 9f, FontStyle.Bold), ForeColor = Color.FromArgb(0, 120, 40) };
            _compactBtn.Click += async (s, e) => await CompactSelectedAsync();

            _unregisterBtn = new Button { Text = "🗑️ Unregister Distro", Location = new Point(845, 8), Size = new Size(155, 28), Enabled = false, Font = new Font("Segoe UI", 9f, FontStyle.Bold), ForeColor = Color.FromArgb(180, 40, 40) };
            _unregisterBtn.Click += async (s, e) => await UnregisterSelectedAsync();

            _statsLabel = new Label
            {
                Text = "Enumerating WSL distributions...",
                Location = new Point(12, 46),
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 100, 180)
            };

            topPanel.Controls.Add(searchLabel);
            topPanel.Controls.Add(_searchBox);
            topPanel.Controls.Add(_chkOrphanedOnly);
            topPanel.Controls.Add(_refreshBtn);
            topPanel.Controls.Add(_showInExplorerBtn);
            topPanel.Controls.Add(_compactBtn);
            topPanel.Controls.Add(_unregisterBtn);
            topPanel.Controls.Add(_statsLabel);

            // ListView
            _listView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                MultiSelect = false
            };
            _listView.Columns.Add("Distribution / Disk Name", 260);
            _listView.Columns.Add("WSL Version", 100);
            _listView.Columns.Add("Disk Size", 110);
            _listView.Columns.Add("Status", 130);
            _listView.Columns.Add("Virtual Disk Path (VHDX)", 420);

            _listView.SelectedIndexChanged += (s, e) =>
            {
                bool hasSel = _listView.SelectedItems.Count > 0;
                var item = hasSel ? _listView.SelectedItems[0].Tag as WslDistroItem : null;
                _showInExplorerBtn.Enabled = hasSel && !string.IsNullOrEmpty(item?.VhdxPath) && File.Exists(item.VhdxPath);
                _compactBtn.Enabled = hasSel && !string.IsNullOrEmpty(item?.VhdxPath) && File.Exists(item.VhdxPath);
                _unregisterBtn.Enabled = hasSel && item != null && !item.IsOrphanedDisk;
            };

            // StatusStrip
            _statusStrip = new StatusStrip();
            _statusLabel = new ToolStripStatusLabel { Spring = true, TextAlign = ContentAlignment.MiddleLeft, Text = "Ready." };
            _statusStrip.Items.Add(_statusLabel);

            Controls.Add(_listView);
            Controls.Add(topPanel);
            Controls.Add(_statusStrip);
        }

        private async Task LoadDistrosAsync()
        {
            _refreshBtn.Enabled = false;
            _statusLabel.Text = "Querying Windows Subsystem for Linux and virtual disks...";

            _allDistros = await Task.Run(() => WslAndVirtualDiskManager.GetWslDistros());

            ApplyFilter();
            _refreshBtn.Enabled = true;
            _statusLabel.Text = $"Found {_allDistros.Count} WSL distributions and virtual disks.";
        }

        private void ApplyFilter()
        {
            _listView.Items.Clear();
            string search = _searchBox.Text.Trim().ToLowerInvariant();
            bool orphanedOnly = _chkOrphanedOnly.Checked;

            var filtered = _allDistros.AsEnumerable();

            if (orphanedOnly)
                filtered = filtered.Where(d => d.IsOrphanedDisk);

            if (!string.IsNullOrEmpty(search))
            {
                filtered = filtered.Where(d =>
                    d.DistributionName.ToLowerInvariant().Contains(search) ||
                    d.VhdxPath.ToLowerInvariant().Contains(search));
            }

            var list = filtered.ToList();
            long totalBytes = _allDistros.Sum(d => d.DiskSizeBytes);
            int distrosCount = _allDistros.Count(d => !d.IsOrphanedDisk);
            int orphanCount = _allDistros.Count(d => d.IsOrphanedDisk);

            _statsLabel.Text = $"Active Distros: {distrosCount} | Orphaned Disks: {orphanCount} | Total Storage: {FormatSize(totalBytes)} | Showing: {list.Count}";

            foreach (var d in list)
            {
                var lvi = new ListViewItem(d.DistributionName) { Tag = d };
                lvi.SubItems.Add($"WSL {d.WslVersion}");
                lvi.SubItems.Add(FormatSize(d.DiskSizeBytes));

                string status = d.IsOrphanedDisk ? "⚠️ Orphaned Disk" : (d.IsDefault ? "✓ Default Distro" : "Installed");
                lvi.SubItems.Add(status);
                lvi.SubItems.Add(d.VhdxPath);

                if (d.IsOrphanedDisk)
                    lvi.ForeColor = Color.FromArgb(190, 40, 40);
                else if (d.DiskSizeBytes > 20L * 1024 * 1024 * 1024) // > 20 GB
                    lvi.ForeColor = Color.FromArgb(180, 90, 0);

                _listView.Items.Add(lvi);
            }
        }

        private void ShowSelectedInExplorer()
        {
            if (_listView.SelectedItems.Count == 0) return;
            if (_listView.SelectedItems[0].Tag is WslDistroItem item)
            {
                if (File.Exists(item.VhdxPath))
                {
                    try
                    {
                        Process.Start(new ProcessStartInfo("explorer.exe", $"/select,\"{item.VhdxPath}\"") { UseShellExecute = true });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Could not open Explorer: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private async Task CompactSelectedAsync()
        {
            if (_listView.SelectedItems.Count == 0) return;
            if (_listView.SelectedItems[0].Tag is WslDistroItem item)
            {
                if (!File.Exists(item.VhdxPath))
                {
                    MessageBox.Show("VHDX virtual disk file does not exist.", "File Missing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (MessageBox.Show($"Compact and shrink virtual disk for '{item.DistributionName}' ({FormatSize(item.DiskSizeBytes)})?\n\nMake sure the distribution is stopped before compacting.", "Confirm Disk Compaction", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;

                _compactBtn.Enabled = false;
                _statusLabel.Text = $"Compacting {item.VhdxPath}...";

                bool ok = await Task.Run(() => WslAndVirtualDiskManager.CompactVhdx(item.VhdxPath));
                if (ok)
                {
                    MessageBox.Show("Virtual disk compaction completed successfully.", "Compaction Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadDistrosAsync();
                }
                else
                {
                    MessageBox.Show("Failed to compact virtual disk. Administrative privileges required and the WSL instance must not be in use.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _compactBtn.Enabled = true;
                }
            }
        }

        private async Task UnregisterSelectedAsync()
        {
            if (_listView.SelectedItems.Count == 0) return;
            if (_listView.SelectedItems[0].Tag is WslDistroItem item)
            {
                if (MessageBox.Show($"Are you sure you want to completely unregister and delete the WSL distribution '{item.DistributionName}'?\n\nALL files and data inside this distribution will be permanently deleted.", "Confirm Unregister Distro", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                    return;

                _unregisterBtn.Enabled = false;
                _statusLabel.Text = $"Unregistering {item.DistributionName}...";

                bool ok = await Task.Run(() => WslAndVirtualDiskManager.UnregisterDistro(item.DistributionName));
                if (ok)
                {
                    MessageBox.Show($"Distribution '{item.DistributionName}' unregistered successfully.", "Unregistered", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadDistrosAsync();
                }
                else
                {
                    MessageBox.Show("Failed to unregister distribution.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _unregisterBtn.Enabled = true;
                }
            }
        }

        private static string FormatSize(long bytes)
        {
            if (bytes < 1024) return $"{bytes} B";
            if (bytes < 1024 * 1024) return $"{bytes / 1024.0:F1} KB";
            if (bytes < 1024 * 1024 * 1024) return $"{bytes / (1024.0 * 1024):F1} MB";
            return $"{bytes / (1024.0 * 1024 * 1024):F2} GB";
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

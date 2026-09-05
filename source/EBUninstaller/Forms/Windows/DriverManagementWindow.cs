/*
    EBUninstaller Pro - Windows Driver Management Window
    Inspect, disable, and clean orphaned or 3rd-party kernel drivers.
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
    public class DriverManagementWindow : Form
    {
        private ListView _listView = null!;
        private CheckBox _chkThirdPartyOnly = null!;
        private CheckBox _chkOrphanedOnly = null!;
        private TextBox _searchBox = null!;
        private Button _refreshBtn = null!;
        private Button _disableBtn = null!;
        private Button _setStartupBtn = null!;
        private Button _removeOrphanBtn = null!;
        private Button _showInExplorerBtn = null!;
        private Label _statsLabel = null!;
        private StatusStrip _statusStrip = null!;
        private ToolStripStatusLabel _statusLabel = null!;

        private List<DriverInfoItem> _allDrivers = new();

        public DriverManagementWindow()
        {
            InitializeComponent();
            ApplyTheme();
            _ = LoadDriversAsync();
        }

        private void InitializeComponent()
        {
            Text = LanguageManager.GetString("DriverManagement_Title") ?? "Windows Drivers & Kernel Modules Manager - EBUninstaller Pro";
            Size = new Size(1100, 680);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(850, 500);

            // Top Toolbar Panel
            var topPanel = new Panel { Dock = DockStyle.Top, Height = 80, Padding = new Padding(12, 8, 12, 8) };

            var searchLabel = new Label { Text = "Filter Drivers:", Location = new Point(12, 12), AutoSize = true, Font = new Font("Segoe UI", 9f, FontStyle.Bold) };
            _searchBox = new TextBox { Location = new Point(105, 10), Width = 220, Font = new Font("Segoe UI", 9f) };
            _searchBox.TextChanged += (s, e) => ApplyFilter();

            _chkThirdPartyOnly = new CheckBox { Text = "3rd-Party Only", Location = new Point(340, 11), AutoSize = true, Checked = true, Font = new Font("Segoe UI", 9f) };
            _chkThirdPartyOnly.CheckedChanged += (s, e) => ApplyFilter();

            _chkOrphanedOnly = new CheckBox { Text = "Orphaned Only (Missing .sys)", Location = new Point(460, 11), AutoSize = true, Font = new Font("Segoe UI", 9f) };
            _chkOrphanedOnly.CheckedChanged += (s, e) => ApplyFilter();

            _refreshBtn = new Button { Text = "🔄 Refresh", Location = new Point(660, 8), Size = new Size(95, 28), Font = new Font("Segoe UI", 9f) };
            _refreshBtn.Click += async (s, e) => await LoadDriversAsync();

            _showInExplorerBtn = new Button { Text = "📁 Show File", Location = new Point(765, 8), Size = new Size(100, 28), Enabled = false, Font = new Font("Segoe UI", 9f) };
            _showInExplorerBtn.Click += (s, e) => ShowSelectedFileInExplorer();

            _disableBtn = new Button { Text = "⛔ Disable", Location = new Point(875, 8), Size = new Size(90, 28), Enabled = false, Font = new Font("Segoe UI", 9f) };
            _disableBtn.Click += (s, e) => SetSelectedStartup(DriverStartupType.Disabled);

            _setStartupBtn = new Button { Text = "⚙️ Startup...", Location = new Point(975, 8), Size = new Size(95, 28), Enabled = false, Font = new Font("Segoe UI", 9f) };
            _setStartupBtn.Click += (s, e) => PromptChangeStartup();

            // Stats row in top panel
            _statsLabel = new Label
            {
                Text = "Loading drivers...",
                Location = new Point(12, 48),
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 100, 180)
            };

            _removeOrphanBtn = new Button
            {
                Text = "🗑️ Remove Orphaned Entry",
                Location = new Point(875, 44),
                Size = new Size(195, 28),
                Enabled = false,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(180, 40, 40)
            };
            _removeOrphanBtn.Click += async (s, e) => await RemoveSelectedOrphanAsync();

            topPanel.Controls.Add(searchLabel);
            topPanel.Controls.Add(_searchBox);
            topPanel.Controls.Add(_chkThirdPartyOnly);
            topPanel.Controls.Add(_chkOrphanedOnly);
            topPanel.Controls.Add(_refreshBtn);
            topPanel.Controls.Add(_showInExplorerBtn);
            topPanel.Controls.Add(_disableBtn);
            topPanel.Controls.Add(_setStartupBtn);
            topPanel.Controls.Add(_statsLabel);
            topPanel.Controls.Add(_removeOrphanBtn);

            // ListView
            _listView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                MultiSelect = false
            };
            _listView.Columns.Add("Driver Name", 140);
            _listView.Columns.Add("Display Name", 240);
            _listView.Columns.Add("Provider / Vendor", 180);
            _listView.Columns.Add("Startup Type", 110);
            _listView.Columns.Add("Status", 100);
            _listView.Columns.Add("Size", 80);
            _listView.Columns.Add("Driver Path", 350);

            _listView.SelectedIndexChanged += (s, e) =>
            {
                bool hasSel = _listView.SelectedItems.Count > 0;
                _showInExplorerBtn.Enabled = hasSel;
                _disableBtn.Enabled = hasSel;
                _setStartupBtn.Enabled = hasSel;

                if (hasSel && _listView.SelectedItems[0].Tag is DriverInfoItem item)
                {
                    _removeOrphanBtn.Enabled = item.IsOrphaned;
                }
                else
                {
                    _removeOrphanBtn.Enabled = false;
                }
            };

            // StatusStrip
            _statusStrip = new StatusStrip();
            _statusLabel = new ToolStripStatusLabel { Spring = true, TextAlign = ContentAlignment.MiddleLeft, Text = "Ready." };
            _statusStrip.Items.Add(_statusLabel);

            Controls.Add(_listView);
            Controls.Add(topPanel);
            Controls.Add(_statusStrip);
        }

        private async Task LoadDriversAsync()
        {
            _refreshBtn.Enabled = false;
            _statusLabel.Text = "Scanning Windows kernel and hardware drivers...";

            _allDrivers = await Task.Run(() => WindowsDriverManager.GetInstalledDrivers(false));

            ApplyFilter();
            _refreshBtn.Enabled = true;
            _statusLabel.Text = $"Found {_allDrivers.Count} total installed drivers.";
        }

        private void ApplyFilter()
        {
            _listView.Items.Clear();
            string search = _searchBox.Text.Trim().ToLowerInvariant();
            bool thirdPartyOnly = _chkThirdPartyOnly.Checked;
            bool orphanedOnly = _chkOrphanedOnly.Checked;

            var filtered = _allDrivers.AsEnumerable();

            if (thirdPartyOnly)
                filtered = filtered.Where(d => !d.IsMicrosoftDriver);

            if (orphanedOnly)
                filtered = filtered.Where(d => d.IsOrphaned);

            if (!string.IsNullOrEmpty(search))
            {
                filtered = filtered.Where(d =>
                    d.DriverName.ToLowerInvariant().Contains(search) ||
                    d.DisplayName.ToLowerInvariant().Contains(search) ||
                    d.Provider.ToLowerInvariant().Contains(search) ||
                    d.DriverPath.ToLowerInvariant().Contains(search));
            }

            var list = filtered.ToList();
            int totalCount = _allDrivers.Count;
            int thirdPartyCount = _allDrivers.Count(d => !d.IsMicrosoftDriver);
            int orphanedCount = _allDrivers.Count(d => d.IsOrphaned);

            _statsLabel.Text = $"Total Drivers: {totalCount} | 3rd-Party: {thirdPartyCount} | Orphaned: {orphanedCount} | Showing: {list.Count}";

            foreach (var d in list)
            {
                var lvi = new ListViewItem(d.DriverName) { Tag = d };
                lvi.SubItems.Add(d.DisplayName);
                lvi.SubItems.Add(d.Provider);
                lvi.SubItems.Add(d.StartupType.ToString());
                lvi.SubItems.Add(d.IsOrphaned ? "⚠️ Orphaned" : "Active");
                lvi.SubItems.Add(d.FileSizeBytes > 0 ? (d.FileSizeBytes / 1024) + " KB" : "-");
                lvi.SubItems.Add(d.DriverPath);

                if (d.IsOrphaned)
                    lvi.ForeColor = Color.FromArgb(190, 40, 40);
                else if (d.StartupType == DriverStartupType.Disabled)
                    lvi.ForeColor = Color.Gray;

                _listView.Items.Add(lvi);
            }
        }

        private void ShowSelectedFileInExplorer()
        {
            if (_listView.SelectedItems.Count == 0) return;
            if (_listView.SelectedItems[0].Tag is DriverInfoItem item)
            {
                if (File.Exists(item.DriverPath))
                {
                    try
                    {
                        Process.Start(new ProcessStartInfo("explorer.exe", $"/select,\"{item.DriverPath}\"") { UseShellExecute = true });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Could not open Explorer: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show($"Driver binary '{item.DriverPath}' does not exist on disk (orphaned service).", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void SetSelectedStartup(DriverStartupType type)
        {
            if (_listView.SelectedItems.Count == 0) return;
            if (_listView.SelectedItems[0].Tag is DriverInfoItem item)
            {
                if (MessageBox.Show($"Change startup type of '{item.DisplayName}' ({item.DriverName}) to {type}?", "Confirm Driver Modification", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;

                if (WindowsDriverManager.SetDriverStartupType(item.DriverName, type))
                {
                    item.StartupType = type;
                    ApplyFilter();
                    _statusLabel.Text = $"Driver '{item.DriverName}' startup set to {type}.";
                }
                else
                {
                    MessageBox.Show("Failed to change driver startup type. Administrative privileges required.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void PromptChangeStartup()
        {
            if (_listView.SelectedItems.Count == 0) return;
            if (_listView.SelectedItems[0].Tag is DriverInfoItem item)
            {
                using var dlg = new Form { Text = "Set Driver Startup Type", Size = new Size(350, 180), StartPosition = FormStartPosition.CenterParent, FormBorderStyle = FormBorderStyle.FixedDialog, MaximizeBox = false, MinimizeBox = false };
                var lbl = new Label { Text = $"Select startup type for {item.DriverName}:", Location = new Point(15, 15), AutoSize = true };
                var cb = new ComboBox { Location = new Point(15, 45), Width = 300, DropDownStyle = ComboBoxStyle.DropDownList };
                cb.Items.AddRange(new object[] { DriverStartupType.Boot, DriverStartupType.System, DriverStartupType.Automatic, DriverStartupType.Manual, DriverStartupType.Disabled });
                cb.SelectedItem = item.StartupType != DriverStartupType.Unknown ? item.StartupType : DriverStartupType.Manual;

                var okBtn = new Button { Text = "Apply", DialogResult = DialogResult.OK, Location = new Point(150, 95), Size = new Size(80, 28) };
                var cancelBtn = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Location = new Point(235, 95), Size = new Size(80, 28) };

                dlg.Controls.Add(lbl);
                dlg.Controls.Add(cb);
                dlg.Controls.Add(okBtn);
                dlg.Controls.Add(cancelBtn);
                dlg.AcceptButton = okBtn;
                dlg.CancelButton = cancelBtn;

                if (dlg.ShowDialog() == DialogResult.OK && cb.SelectedItem is DriverStartupType selectedType)
                {
                    SetSelectedStartup(selectedType);
                }
            }
        }

        private async Task RemoveSelectedOrphanAsync()
        {
            if (_listView.SelectedItems.Count == 0) return;
            if (_listView.SelectedItems[0].Tag is DriverInfoItem item)
            {
                if (!item.IsOrphaned)
                {
                    MessageBox.Show("Only orphaned driver entries (where the .sys binary no longer exists) can be removed.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (MessageBox.Show($"Are you sure you want to delete the orphaned driver registry entry '{item.DriverName}'?", "Confirm Orphan Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                    return;

                bool removed = await Task.Run(() => WindowsDriverManager.RemoveOrphanedDriver(item.DriverName));
                if (removed)
                {
                    _allDrivers.Remove(item);
                    ApplyFilter();
                    _statusLabel.Text = $"Removed orphaned driver '{item.DriverName}'.";
                }
                else
                {
                    MessageBox.Show("Failed to remove orphaned driver registry entry.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
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

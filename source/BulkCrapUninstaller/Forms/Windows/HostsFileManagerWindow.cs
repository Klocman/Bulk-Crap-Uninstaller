/*
    EBUninstaller Pro - Windows Hosts File Manager Window
    Modern GUI for auditing, cleaning, and restoring the Windows hosts file.
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
    public class HostsFileManagerWindow : Form
    {
        private ListView _listView = null!;
        private CheckBox _chkCustomOnly = null!;
        private TextBox _searchBox = null!;
        private Button _refreshBtn = null!;
        private Button _deleteBtn = null!;
        private Button _resetBtn = null!;
        private Button _backupBtn = null!;
        private Button _openNotepadBtn = null!;
        private Label _statsLabel = null!;
        private StatusStrip _statusStrip = null!;
        private ToolStripStatusLabel _statusLabel = null!;

        private List<HostEntryItem> _allEntries = new();

        public HostsFileManagerWindow()
        {
            InitializeComponent();
            ApplyTheme();
            _ = LoadHostsAsync();
        }

        private void InitializeComponent()
        {
            Text = LanguageManager.GetString("HostsFileManager_Title") ?? "Windows Hosts File & Network Redirection Manager - EBUninstaller Pro";
            Size = new Size(1000, 600);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(800, 450);

            // Top Panel
            var topPanel = new Panel { Dock = DockStyle.Top, Height = 78, Padding = new Padding(12, 8, 12, 8) };

            var searchLabel = new Label { Text = "Search Hosts:", Location = new Point(12, 12), AutoSize = true, Font = new Font("Segoe UI", 9f, FontStyle.Bold) };
            _searchBox = new TextBox { Location = new Point(105, 10), Width = 190, Font = new Font("Segoe UI", 9f) };
            _searchBox.TextChanged += (s, e) => ApplyFilter();

            _chkCustomOnly = new CheckBox { Text = "Custom Redirections Only (Hide Localhost)", Location = new Point(305, 11), AutoSize = true, Checked = true, Font = new Font("Segoe UI", 9f, FontStyle.Bold) };
            _chkCustomOnly.CheckedChanged += (s, e) => ApplyFilter();

            _refreshBtn = new Button { Text = "🔄 Refresh", Location = new Point(595, 8), Size = new Size(85, 28), Font = new Font("Segoe UI", 9f) };
            _refreshBtn.Click += async (s, e) => await LoadHostsAsync();

            _openNotepadBtn = new Button { Text = "📝 Notepad", Location = new Point(685, 8), Size = new Size(85, 28), Font = new Font("Segoe UI", 9f) };
            _openNotepadBtn.Click += (s, e) => OpenInNotepad();

            _backupBtn = new Button { Text = "💾 Backup", Location = new Point(775, 8), Size = new Size(80, 28), Font = new Font("Segoe UI", 9f) };
            _backupBtn.Click += (s, e) => BackupHosts();

            _deleteBtn = new Button { Text = "🗑️ Delete", Location = new Point(860, 8), Size = new Size(115, 28), Enabled = false, Font = new Font("Segoe UI", 9f, FontStyle.Bold), ForeColor = Color.FromArgb(180, 40, 40) };
            _deleteBtn.Click += async (s, e) => await DeleteSelectedAsync();

            _resetBtn = new Button { Text = "⚡ Reset Hosts to Default", Location = new Point(790, 42), Size = new Size(185, 28), Font = new Font("Segoe UI", 9f, FontStyle.Bold), ForeColor = Color.FromArgb(0, 120, 40) };
            _resetBtn.Click += async (s, e) => await ResetHostsAsync();

            _statsLabel = new Label
            {
                Text = "Reading hosts file...",
                Location = new Point(12, 46),
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 100, 180)
            };

            topPanel.Controls.Add(searchLabel);
            topPanel.Controls.Add(_searchBox);
            topPanel.Controls.Add(_chkCustomOnly);
            topPanel.Controls.Add(_refreshBtn);
            topPanel.Controls.Add(_openNotepadBtn);
            topPanel.Controls.Add(_backupBtn);
            topPanel.Controls.Add(_deleteBtn);
            topPanel.Controls.Add(_resetBtn);
            topPanel.Controls.Add(_statsLabel);

            // ListView
            _listView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                MultiSelect = true
            };
            _listView.Columns.Add("Line #", 65);
            _listView.Columns.Add("IP Address", 140);
            _listView.Columns.Add("Domain / Hostname", 280);
            _listView.Columns.Add("Status", 110);
            _listView.Columns.Add("Type", 120);
            _listView.Columns.Add("Comment", 250);

            _listView.SelectedIndexChanged += (s, e) =>
            {
                _deleteBtn.Enabled = _listView.SelectedItems.Count > 0;
            };

            // StatusStrip
            _statusStrip = new StatusStrip();
            _statusLabel = new ToolStripStatusLabel { Spring = true, TextAlign = ContentAlignment.MiddleLeft, Text = "Ready." };
            _statusStrip.Items.Add(_statusLabel);

            Controls.Add(_listView);
            Controls.Add(topPanel);
            Controls.Add(_statusStrip);
        }

        private async Task LoadHostsAsync()
        {
            _refreshBtn.Enabled = false;
            _statusLabel.Text = "Reading %WINDIR%\\System32\\drivers\\etc\\hosts...";

            _allEntries = await Task.Run(() => WindowsHostsFileManager.ReadHostsFile());

            ApplyFilter();
            _refreshBtn.Enabled = true;
            _statusLabel.Text = $"Found {_allEntries.Count} hosts file lines ({_allEntries.Count(e => !e.IsDefaultLocalhost)} custom).";
        }

        private void ApplyFilter()
        {
            _listView.Items.Clear();
            string search = _searchBox.Text.Trim().ToLowerInvariant();
            bool customOnly = _chkCustomOnly.Checked;

            var filtered = _allEntries.AsEnumerable();

            if (customOnly)
                filtered = filtered.Where(e => !e.IsDefaultLocalhost);

            if (!string.IsNullOrEmpty(search))
            {
                filtered = filtered.Where(e =>
                    e.Hostname.ToLowerInvariant().Contains(search) ||
                    e.IpAddress.ToLowerInvariant().Contains(search) ||
                    e.Comment.ToLowerInvariant().Contains(search));
            }

            var list = filtered.ToList();
            int totalCount = _allEntries.Count;
            int customCount = _allEntries.Count(e => !e.IsDefaultLocalhost);

            _statsLabel.Text = $"Total Entries: {totalCount} | Custom Redirections: {customCount} | Showing: {list.Count}";

            foreach (var e in list)
            {
                var lvi = new ListViewItem(e.LineNumber.ToString()) { Tag = e };
                lvi.SubItems.Add(e.IpAddress);
                lvi.SubItems.Add(e.Hostname);
                lvi.SubItems.Add(e.IsCommentedOut ? "# Disabled" : "✓ Active");
                lvi.SubItems.Add(e.IsDefaultLocalhost ? "Standard Localhost" : "⚠️ Custom Redirection");
                lvi.SubItems.Add(e.Comment);

                if (!e.IsDefaultLocalhost && !e.IsCommentedOut)
                    lvi.ForeColor = Color.FromArgb(190, 40, 40);
                else if (e.IsCommentedOut)
                    lvi.ForeColor = Color.Gray;

                _listView.Items.Add(lvi);
            }
        }

        private void OpenInNotepad()
        {
            try
            {
                Process.Start(new ProcessStartInfo("notepad.exe", WindowsHostsFileManager.HostsFilePath) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not open Notepad: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BackupHosts()
        {
            string backupPath = WindowsHostsFileManager.BackupHostsFile();
            if (!string.IsNullOrEmpty(backupPath))
            {
                MessageBox.Show($"Hosts file backup saved to:\n{backupPath}", "Backup Created", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Failed to create hosts backup. Check permissions.", "Backup Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task DeleteSelectedAsync()
        {
            var selected = _listView.SelectedItems.Cast<ListViewItem>()
                .Select(l => l.Tag as HostEntryItem)
                .Where(e => e != null)
                .Cast<HostEntryItem>()
                .ToList();

            if (selected.Count == 0) return;

            if (MessageBox.Show($"Are you sure you want to remove {selected.Count} host redirection entry/entries?", "Confirm Removal", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            int deleted = 0;
            await Task.Run(() =>
            {
                foreach (var item in selected)
                {
                    if (WindowsHostsFileManager.RemoveHostEntry(item))
                    {
                        deleted++;
                        _allEntries.Remove(item);
                    }
                }
            });

            ApplyFilter();
            _statusLabel.Text = $"Removed {deleted} hosts file entries.";
            MessageBox.Show($"Successfully removed {deleted} hosts redirection entries.", "Cleanup Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async Task ResetHostsAsync()
        {
            if (MessageBox.Show("This will backup your existing hosts file and reset it to the clean Microsoft Windows default. Proceed?", "Confirm Reset Hosts", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            bool reset = await Task.Run(() => WindowsHostsFileManager.ResetHostsToDefault());
            if (reset)
            {
                MessageBox.Show("Hosts file successfully reset to default.", "Hosts Reset Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await LoadHostsAsync();
            }
            else
            {
                MessageBox.Show("Failed to reset hosts file. Administrative privileges required.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

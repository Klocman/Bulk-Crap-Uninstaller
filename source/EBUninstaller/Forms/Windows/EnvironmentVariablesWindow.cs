/*
    EBUninstaller Pro - Windows Environment Variables & PATH Cleaner Window
    Modern GUI for auditing and cleaning invalid directories from PATH environment variables.
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
    public class EnvironmentVariablesWindow : Form
    {
        private ListView _listView = null!;
        private CheckBox _chkInvalidOnly = null!;
        private CheckBox _chkDuplicatesOnly = null!;
        private ComboBox _scopeBox = null!;
        private TextBox _searchBox = null!;
        private Button _refreshBtn = null!;
        private Button _cleanBtn = null!;
        private Button _backupBtn = null!;
        private Button _showInExplorerBtn = null!;
        private Label _statsLabel = null!;
        private StatusStrip _statusStrip = null!;
        private ToolStripStatusLabel _statusLabel = null!;

        private EnvVarReport _currentReport = new();

        public EnvironmentVariablesWindow()
        {
            InitializeComponent();
            ApplyTheme();
            _ = LoadPathDataAsync();
        }

        private void InitializeComponent()
        {
            Text = LanguageManager.GetString("EnvVars_Title") ?? "Environment Variables & PATH Orphan Cleaner - EBUninstaller Pro";
            Size = new Size(1050, 650);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(850, 480);

            // Top Panel
            var topPanel = new Panel { Dock = DockStyle.Top, Height = 78, Padding = new Padding(12, 8, 12, 8) };

            var searchLabel = new Label { Text = "Search:", Location = new Point(12, 12), AutoSize = true, Font = new Font("Segoe UI", 9f, FontStyle.Bold) };
            _searchBox = new TextBox { Location = new Point(70, 10), Width = 160, Font = new Font("Segoe UI", 9f) };
            _searchBox.TextChanged += (s, e) => ApplyFilter();

            _scopeBox = new ComboBox { Location = new Point(240, 9), Width = 130, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9f) };
            _scopeBox.Items.AddRange(new object[] { "All Scopes", "System PATH", "User PATH" });
            _scopeBox.SelectedIndex = 0;
            _scopeBox.SelectedIndexChanged += (s, e) => ApplyFilter();

            _chkInvalidOnly = new CheckBox { Text = "Invalid / Missing Only", Location = new Point(380, 11), AutoSize = true, Checked = true, Font = new Font("Segoe UI", 9f, FontStyle.Bold) };
            _chkInvalidOnly.CheckedChanged += (s, e) => ApplyFilter();

            _chkDuplicatesOnly = new CheckBox { Text = "Duplicates Only", Location = new Point(540, 11), AutoSize = true, Font = new Font("Segoe UI", 9f) };
            _chkDuplicatesOnly.CheckedChanged += (s, e) => ApplyFilter();

            _refreshBtn = new Button { Text = "🔄 Refresh", Location = new Point(660, 8), Size = new Size(85, 28), Font = new Font("Segoe UI", 9f) };
            _refreshBtn.Click += async (s, e) => await LoadPathDataAsync();

            _showInExplorerBtn = new Button { Text = "📁 Show", Location = new Point(750, 8), Size = new Size(70, 28), Enabled = false, Font = new Font("Segoe UI", 9f) };
            _showInExplorerBtn.Click += (s, e) => ShowSelectedInExplorer();

            _backupBtn = new Button { Text = "💾 Backup .REG", Location = new Point(825, 8), Size = new Size(105, 28), Font = new Font("Segoe UI", 9f) };
            _backupBtn.Click += (s, e) => BackupReg();

            _cleanBtn = new Button { Text = "⚡ Clean Invalid Entries", Location = new Point(775, 42), Size = new Size(205, 28), Font = new Font("Segoe UI", 9f, FontStyle.Bold), ForeColor = Color.FromArgb(0, 120, 40) };
            _cleanBtn.Click += async (s, e) => await CleanInvalidAsync();

            _statsLabel = new Label
            {
                Text = "Analyzing PATH variables...",
                Location = new Point(12, 46),
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 100, 180)
            };

            topPanel.Controls.Add(searchLabel);
            topPanel.Controls.Add(_searchBox);
            topPanel.Controls.Add(_scopeBox);
            topPanel.Controls.Add(_chkInvalidOnly);
            topPanel.Controls.Add(_chkDuplicatesOnly);
            topPanel.Controls.Add(_refreshBtn);
            topPanel.Controls.Add(_showInExplorerBtn);
            topPanel.Controls.Add(_backupBtn);
            topPanel.Controls.Add(_cleanBtn);
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
            _listView.Columns.Add("Status", 120);
            _listView.Columns.Add("Scope", 90);
            _listView.Columns.Add("Path Entry", 380);
            _listView.Columns.Add("Resolved Full Path", 400);

            _listView.SelectedIndexChanged += (s, e) =>
            {
                bool hasSel = _listView.SelectedItems.Count > 0;
                _showInExplorerBtn.Enabled = hasSel && (_listView.SelectedItems[0].Tag as PathEntryItem)?.ExistsOnDisk == true;
            };

            // StatusStrip
            _statusStrip = new StatusStrip();
            _statusLabel = new ToolStripStatusLabel { Spring = true, TextAlign = ContentAlignment.MiddleLeft, Text = "Ready." };
            _statusStrip.Items.Add(_statusLabel);

            Controls.Add(_listView);
            Controls.Add(topPanel);
            Controls.Add(_statusStrip);
        }

        private async Task LoadPathDataAsync()
        {
            _refreshBtn.Enabled = false;
            _statusLabel.Text = "Analyzing System and User PATH environment variables...";

            _currentReport = await Task.Run(() => EnvironmentVariablesManager.AnalyzePathVariables());

            ApplyFilter();
            _refreshBtn.Enabled = true;
            _statusLabel.Text = $"Analysis complete. Found {_currentReport.TotalInvalidEntries} invalid and {_currentReport.TotalDuplicates} duplicate PATH entries.";
        }

        private void ApplyFilter()
        {
            _listView.Items.Clear();
            string search = _searchBox.Text.Trim().ToLowerInvariant();
            string scope = _scopeBox.SelectedItem?.ToString() ?? "All Scopes";
            bool invalidOnly = _chkInvalidOnly.Checked;
            bool duplicatesOnly = _chkDuplicatesOnly.Checked;

            var all = new List<PathEntryItem>();
            if (scope != "User PATH")
                all.AddRange(_currentReport.SystemPathEntries);
            if (scope != "System PATH")
                all.AddRange(_currentReport.UserPathEntries);

            var filtered = all.AsEnumerable();

            if (invalidOnly)
                filtered = filtered.Where(p => !p.ExistsOnDisk);

            if (duplicatesOnly)
                filtered = filtered.Where(p => p.IsDuplicate);

            if (!string.IsNullOrEmpty(search))
            {
                filtered = filtered.Where(p =>
                    p.RawPath.ToLowerInvariant().Contains(search) ||
                    p.ExpandedPath.ToLowerInvariant().Contains(search));
            }

            var list = filtered.ToList();
            int sysCount = _currentReport.SystemPathEntries.Count;
            int userCount = _currentReport.UserPathEntries.Count;
            int invalidCount = _currentReport.TotalInvalidEntries;

            _statsLabel.Text = $"System PATH: {sysCount} | User PATH: {userCount} | Invalid / Missing: {invalidCount} | Duplicates: {_currentReport.TotalDuplicates} | Showing: {list.Count}";
            _cleanBtn.Enabled = invalidCount > 0 || _currentReport.TotalDuplicates > 0;

            foreach (var p in list)
            {
                string status = !p.ExistsOnDisk ? "⚠️ Missing / Dead" : (p.IsDuplicate ? "⚠️ Duplicate" : "✓ Valid");
                var lvi = new ListViewItem(status) { Tag = p };
                lvi.SubItems.Add(p.IsUserLevel ? "User" : "System");
                lvi.SubItems.Add(p.RawPath);
                lvi.SubItems.Add(p.ExpandedPath);

                if (!p.ExistsOnDisk)
                    lvi.ForeColor = Color.FromArgb(190, 40, 40);
                else if (p.IsDuplicate)
                    lvi.ForeColor = Color.FromArgb(180, 90, 0);

                _listView.Items.Add(lvi);
            }
        }

        private void ShowSelectedInExplorer()
        {
            if (_listView.SelectedItems.Count == 0) return;
            if (_listView.SelectedItems[0].Tag is PathEntryItem item)
            {
                if (Directory.Exists(item.ExpandedPath) || File.Exists(item.ExpandedPath))
                {
                    try
                    {
                        Process.Start(new ProcessStartInfo("explorer.exe", $"/select,\"{item.ExpandedPath}\"") { UseShellExecute = true });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Could not open Explorer: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Directory does not exist on disk.", "Missing Directory", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void BackupReg()
        {
            string backupPath = EnvironmentVariablesManager.BackupEnvironmentVariables();
            if (!string.IsNullOrEmpty(backupPath))
            {
                MessageBox.Show($"Environment variables backup saved to:\n{backupPath}", "Backup Created", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Failed to create backup.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task CleanInvalidAsync()
        {
            int invalid = _currentReport.TotalInvalidEntries;
            int dupes = _currentReport.TotalDuplicates;

            if (invalid == 0 && dupes == 0)
            {
                MessageBox.Show("No invalid or duplicate PATH entries detected.", "PATH is Healthy", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show($"Clean {invalid} invalid (non-existent) directories and {dupes} duplicate entries from System & User PATH?\n\nA .REG backup will be automatically saved.", "Confirm PATH Cleanup", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            _cleanBtn.Enabled = false;
            _statusLabel.Text = "Cleaning PATH variables...";

            bool success = await Task.Run(() => EnvironmentVariablesManager.CleanInvalidPathEntries(true, true));
            if (success)
            {
                MessageBox.Show("PATH environment variables cleaned successfully.", "Cleanup Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await LoadPathDataAsync();
            }
            else
            {
                MessageBox.Show("Failed to clean PATH variables. Administrative privileges required.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _cleanBtn.Enabled = true;
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

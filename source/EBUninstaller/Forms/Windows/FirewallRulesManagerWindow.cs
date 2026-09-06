/*
    EBUninstaller Pro - Windows Firewall Rules & Orphan Cleaner Window
    Modern GUI for auditing and cleaning stale firewall rules left by uninstalled software.
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
    public class FirewallRulesManagerWindow : Form
    {
        private ListView _listView = null!;
        private CheckBox _chkOrphanedOnly = null!;
        private CheckBox _chkHideSystem = null!;
        private TextBox _searchBox = null!;
        private Button _refreshBtn = null!;
        private Button _deleteSelectedBtn = null!;
        private Button _cleanAllOrphansBtn = null!;
        private Button _showInExplorerBtn = null!;
        private Label _statsLabel = null!;
        private StatusStrip _statusStrip = null!;
        private ToolStripStatusLabel _statusLabel = null!;

        private List<FirewallRuleItem> _allRules = new();

        public FirewallRulesManagerWindow()
        {
            InitializeComponent();
            ApplyTheme();
            _ = LoadRulesAsync();
        }

        private void InitializeComponent()
        {
            Text = LanguageManager.GetString("FirewallManager_Title") ?? "Windows Firewall Rules & Orphan Cleaner - EBUninstaller Pro";
            Size = new Size(1100, 650);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(850, 480);

            // Top Panel
            var topPanel = new Panel { Dock = DockStyle.Top, Height = 78, Padding = new Padding(12, 8, 12, 8) };

            var searchLabel = new Label { Text = "Search Rules:", Location = new Point(12, 12), AutoSize = true, Font = new Font("Segoe UI", 9f, FontStyle.Bold) };
            _searchBox = new TextBox { Location = new Point(105, 10), Width = 200, Font = new Font("Segoe UI", 9f) };
            _searchBox.TextChanged += (s, e) => ApplyFilter();

            _chkOrphanedOnly = new CheckBox { Text = "Orphaned Only (Missing .exe)", Location = new Point(320, 11), AutoSize = true, Checked = true, Font = new Font("Segoe UI", 9f, FontStyle.Bold) };
            _chkOrphanedOnly.CheckedChanged += (s, e) => ApplyFilter();

            _chkHideSystem = new CheckBox { Text = "Hide Windows System Rules", Location = new Point(530, 11), AutoSize = true, Checked = true, Font = new Font("Segoe UI", 9f) };
            _chkHideSystem.CheckedChanged += (s, e) => ApplyFilter();

            _refreshBtn = new Button { Text = "🔄 Refresh", Location = new Point(730, 8), Size = new Size(85, 28), Font = new Font("Segoe UI", 9f) };
            _refreshBtn.Click += async (s, e) => await LoadRulesAsync();

            _showInExplorerBtn = new Button { Text = "📁 Show File", Location = new Point(820, 8), Size = new Size(90, 28), Enabled = false, Font = new Font("Segoe UI", 9f) };
            _showInExplorerBtn.Click += (s, e) => ShowSelectedInExplorer();

            _deleteSelectedBtn = new Button { Text = "🗑️ Delete Selected", Location = new Point(915, 8), Size = new Size(130, 28), Enabled = false, Font = new Font("Segoe UI", 9f, FontStyle.Bold), ForeColor = Color.FromArgb(180, 40, 40) };
            _deleteSelectedBtn.Click += async (s, e) => await DeleteSelectedAsync();

            _cleanAllOrphansBtn = new Button { Text = "⚡ Clean All Orphaned Rules", Location = new Point(860, 42), Size = new Size(185, 28), Enabled = false, Font = new Font("Segoe UI", 9f, FontStyle.Bold), ForeColor = Color.FromArgb(0, 120, 40) };
            _cleanAllOrphansBtn.Click += async (s, e) => await CleanAllOrphansAsync();

            _statsLabel = new Label
            {
                Text = "Loading firewall policy...",
                Location = new Point(12, 46),
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 100, 180)
            };

            topPanel.Controls.Add(searchLabel);
            topPanel.Controls.Add(_searchBox);
            topPanel.Controls.Add(_chkOrphanedOnly);
            topPanel.Controls.Add(_chkHideSystem);
            topPanel.Controls.Add(_refreshBtn);
            topPanel.Controls.Add(_showInExplorerBtn);
            topPanel.Controls.Add(_deleteSelectedBtn);
            topPanel.Controls.Add(_cleanAllOrphansBtn);
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
            _listView.Columns.Add("Rule Name", 240);
            _listView.Columns.Add("Direction", 80);
            _listView.Columns.Add("Action", 70);
            _listView.Columns.Add("Protocol / Ports", 120);
            _listView.Columns.Add("Status", 110);
            _listView.Columns.Add("Application Executable Path", 400);

            _listView.SelectedIndexChanged += (s, e) =>
            {
                bool hasSel = _listView.SelectedItems.Count > 0;
                _deleteSelectedBtn.Enabled = hasSel && _listView.SelectedItems.Cast<ListViewItem>().All(l => !(l.Tag as FirewallRuleItem)?.IsSystemRule == true);
                _showInExplorerBtn.Enabled = hasSel && _listView.SelectedItems.Count == 1 && !string.IsNullOrEmpty((_listView.SelectedItems[0].Tag as FirewallRuleItem)?.ApplicationPath);
            };

            // StatusStrip
            _statusStrip = new StatusStrip();
            _statusLabel = new ToolStripStatusLabel { Spring = true, TextAlign = ContentAlignment.MiddleLeft, Text = "Ready." };
            _statusStrip.Items.Add(_statusLabel);

            Controls.Add(_listView);
            Controls.Add(topPanel);
            Controls.Add(_statusStrip);
        }

        private async Task LoadRulesAsync()
        {
            _refreshBtn.Enabled = false;
            _statusLabel.Text = "Scanning Windows Defender Firewall rules...";

            _allRules = await Task.Run(() => WindowsFirewallManager.GetFirewallRules(false));

            ApplyFilter();
            _refreshBtn.Enabled = true;
            _statusLabel.Text = $"Found {_allRules.Count} total firewall rules.";
        }

        private void ApplyFilter()
        {
            _listView.Items.Clear();
            string search = _searchBox.Text.Trim().ToLowerInvariant();
            bool orphanedOnly = _chkOrphanedOnly.Checked;
            bool hideSystem = _chkHideSystem.Checked;

            var filtered = _allRules.AsEnumerable();

            if (hideSystem)
                filtered = filtered.Where(r => !r.IsSystemRule);

            if (orphanedOnly)
                filtered = filtered.Where(r => r.IsOrphaned);

            if (!string.IsNullOrEmpty(search))
            {
                filtered = filtered.Where(r =>
                    r.Name.ToLowerInvariant().Contains(search) ||
                    r.ApplicationPath.ToLowerInvariant().Contains(search) ||
                    r.Description.ToLowerInvariant().Contains(search));
            }

            var list = filtered.ToList();
            int totalCount = _allRules.Count;
            int orphanCount = _allRules.Count(r => r.IsOrphaned);

            _statsLabel.Text = $"Total Rules: {totalCount} | Orphaned (Dead Executables): {orphanCount} | Showing: {list.Count}";
            _cleanAllOrphansBtn.Enabled = orphanCount > 0;

            foreach (var r in list)
            {
                var lvi = new ListViewItem(r.Name) { Tag = r };
                lvi.SubItems.Add(r.Direction.ToString());
                lvi.SubItems.Add(r.Action.ToString());
                lvi.SubItems.Add($"{r.Protocol}:{r.Ports}");
                lvi.SubItems.Add(r.IsOrphaned ? "⚠️ Orphaned" : (r.IsEnabled ? "Active" : "Disabled"));
                lvi.SubItems.Add(r.ApplicationPath);

                if (r.IsOrphaned)
                    lvi.ForeColor = Color.FromArgb(190, 40, 40);
                else if (r.IsSystemRule)
                    lvi.ForeColor = Color.FromArgb(0, 100, 180);
                else if (!r.IsEnabled)
                    lvi.ForeColor = Color.Gray;

                _listView.Items.Add(lvi);
            }
        }

        private void ShowSelectedInExplorer()
        {
            if (_listView.SelectedItems.Count == 0) return;
            if (_listView.SelectedItems[0].Tag is FirewallRuleItem rule)
            {
                if (File.Exists(rule.ApplicationPath))
                {
                    try
                    {
                        Process.Start(new ProcessStartInfo("explorer.exe", $"/select,\"{rule.ApplicationPath}\"") { UseShellExecute = true });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Could not open Explorer: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show($"Executable file '{rule.ApplicationPath}' no longer exists on disk.", "Orphaned Rule", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private async Task DeleteSelectedAsync()
        {
            var selected = _listView.SelectedItems.Cast<ListViewItem>()
                .Select(l => l.Tag as FirewallRuleItem)
                .Where(r => r != null && !r.IsSystemRule)
                .Cast<FirewallRuleItem>()
                .ToList();

            if (selected.Count == 0) return;

            if (MessageBox.Show($"Are you sure you want to delete {selected.Count} selected firewall rule(s)?", "Confirm Rule Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            int deleted = 0;
            await Task.Run(() =>
            {
                foreach (var rule in selected)
                {
                    if (WindowsFirewallManager.DeleteFirewallRule(rule))
                    {
                        deleted++;
                        _allRules.Remove(rule);
                    }
                }
            });

            ApplyFilter();
            _statusLabel.Text = $"Deleted {deleted} firewall rules.";
            MessageBox.Show($"Successfully deleted {deleted} firewall rules.", "Cleanup Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async Task CleanAllOrphansAsync()
        {
            var orphans = _allRules.Where(r => r.IsOrphaned && !r.IsSystemRule).ToList();
            if (orphans.Count == 0) return;

            if (MessageBox.Show($"Are you sure you want to delete all {orphans.Count} orphaned firewall rules?", "Confirm Cleanup All", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            int deleted = 0;
            await Task.Run(() =>
            {
                foreach (var rule in orphans)
                {
                    if (WindowsFirewallManager.DeleteFirewallRule(rule))
                    {
                        deleted++;
                        _allRules.Remove(rule);
                    }
                }
            });

            ApplyFilter();
            _statusLabel.Text = $"Deleted all {deleted} orphaned firewall rules.";
            MessageBox.Show($"Successfully cleaned {deleted} orphaned firewall rules.", "Cleanup Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

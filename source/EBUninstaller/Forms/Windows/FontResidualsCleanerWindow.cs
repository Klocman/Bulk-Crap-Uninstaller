/*
    EBUninstaller Pro - Windows Font Residuals Cleaner Window
    Modern GUI for scanning and cleaning orphaned font registry entries.
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using UninstallTools.JunkCleaner;
using UninstallTools.Localization;

namespace BulkCrapUninstaller.Forms.Windows
{
    public class FontResidualsCleanerWindow : Form
    {
        private ListView _listView = null!;
        private CheckBox _chkOrphanedOnly = null!;
        private TextBox _searchBox = null!;
        private Button _refreshBtn = null!;
        private Button _cleanSelectedBtn = null!;
        private Button _cleanAllOrphansBtn = null!;
        private Label _statsLabel = null!;
        private StatusStrip _statusStrip = null!;
        private ToolStripStatusLabel _statusLabel = null!;

        private List<FontResidualItem> _allFonts = new();

        public FontResidualsCleanerWindow()
        {
            InitializeComponent();
            ApplyTheme();
            _ = LoadFontsAsync();
        }

        private void InitializeComponent()
        {
            Text = LanguageManager.GetString("FontCleaner_Title") ?? "Font Registry Residuals Cleaner - EBUninstaller Pro";
            Size = new Size(1000, 600);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(800, 450);

            // Top Panel
            var topPanel = new Panel { Dock = DockStyle.Top, Height = 75, Padding = new Padding(12, 8, 12, 8) };

            var searchLabel = new Label { Text = "Search Fonts:", Location = new Point(12, 12), AutoSize = true, Font = new Font("Segoe UI", 9f, FontStyle.Bold) };
            _searchBox = new TextBox { Location = new Point(105, 10), Width = 200, Font = new Font("Segoe UI", 9f) };
            _searchBox.TextChanged += (s, e) => ApplyFilter();

            _chkOrphanedOnly = new CheckBox { Text = "Orphaned Only (Missing Font Files)", Location = new Point(320, 11), AutoSize = true, Checked = true, Font = new Font("Segoe UI", 9f, FontStyle.Bold) };
            _chkOrphanedOnly.CheckedChanged += (s, e) => ApplyFilter();

            _refreshBtn = new Button { Text = "🔄 Refresh", Location = new Point(590, 8), Size = new Size(90, 28), Font = new Font("Segoe UI", 9f) };
            _refreshBtn.Click += async (s, e) => await LoadFontsAsync();

            _cleanSelectedBtn = new Button { Text = "🗑️ Clean Selected", Location = new Point(690, 8), Size = new Size(130, 28), Enabled = false, Font = new Font("Segoe UI", 9f, FontStyle.Bold), ForeColor = Color.FromArgb(180, 40, 40) };
            _cleanSelectedBtn.Click += async (s, e) => await CleanSelectedAsync();

            _cleanAllOrphansBtn = new Button { Text = "⚡ Clean All Orphans", Location = new Point(830, 8), Size = new Size(150, 28), Enabled = false, Font = new Font("Segoe UI", 9f, FontStyle.Bold), ForeColor = Color.FromArgb(0, 120, 40) };
            _cleanAllOrphansBtn.Click += async (s, e) => await CleanAllOrphansAsync();

            _statsLabel = new Label
            {
                Text = "Loading font registrations...",
                Location = new Point(12, 45),
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 100, 180)
            };

            topPanel.Controls.Add(searchLabel);
            topPanel.Controls.Add(_searchBox);
            topPanel.Controls.Add(_chkOrphanedOnly);
            topPanel.Controls.Add(_refreshBtn);
            topPanel.Controls.Add(_cleanSelectedBtn);
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
            _listView.Columns.Add("Font Name", 260);
            _listView.Columns.Add("Font File Name", 180);
            _listView.Columns.Add("Status", 110);
            _listView.Columns.Add("Scope", 80);
            _listView.Columns.Add("Expected File Path", 320);

            _listView.SelectedIndexChanged += (s, e) =>
            {
                _cleanSelectedBtn.Enabled = _listView.SelectedItems.Count > 0 &&
                                           _listView.SelectedItems.Cast<ListViewItem>().Any(l => (l.Tag as FontResidualItem)?.IsOrphaned == true);
            };

            // StatusStrip
            _statusStrip = new StatusStrip();
            _statusLabel = new ToolStripStatusLabel { Spring = true, TextAlign = ContentAlignment.MiddleLeft, Text = "Ready." };
            _statusStrip.Items.Add(_statusLabel);

            Controls.Add(_listView);
            Controls.Add(topPanel);
            Controls.Add(_statusStrip);
        }

        private async Task LoadFontsAsync()
        {
            _refreshBtn.Enabled = false;
            _statusLabel.Text = "Scanning Windows font registry and filesystem...";

            _allFonts = await Task.Run(() => FontResidualsCleaner.ScanFontResiduals(false));

            ApplyFilter();
            _refreshBtn.Enabled = true;
            _statusLabel.Text = $"Found {_allFonts.Count} total font registrations.";
        }

        private void ApplyFilter()
        {
            _listView.Items.Clear();
            string search = _searchBox.Text.Trim().ToLowerInvariant();
            bool orphanedOnly = _chkOrphanedOnly.Checked;

            var filtered = _allFonts.AsEnumerable();

            if (orphanedOnly)
                filtered = filtered.Where(f => f.IsOrphaned);

            if (!string.IsNullOrEmpty(search))
            {
                filtered = filtered.Where(f =>
                    f.FontName.ToLowerInvariant().Contains(search) ||
                    f.FontFileName.ToLowerInvariant().Contains(search) ||
                    f.ResolvedPath.ToLowerInvariant().Contains(search));
            }

            var list = filtered.ToList();
            int totalCount = _allFonts.Count;
            int orphanCount = _allFonts.Count(f => f.IsOrphaned);

            _statsLabel.Text = $"Total Registered Fonts: {totalCount} | Orphaned Font Residuals: {orphanCount} | Showing: {list.Count}";
            _cleanAllOrphansBtn.Enabled = orphanCount > 0;

            foreach (var f in list)
            {
                var lvi = new ListViewItem(f.FontName) { Tag = f };
                lvi.SubItems.Add(f.FontFileName);
                lvi.SubItems.Add(f.IsOrphaned ? "⚠️ Orphaned" : "Active");
                lvi.SubItems.Add(f.IsCurrentUser ? "User" : "System");
                lvi.SubItems.Add(f.ResolvedPath);

                if (f.IsOrphaned)
                    lvi.ForeColor = Color.FromArgb(190, 40, 40);

                _listView.Items.Add(lvi);
            }
        }

        private async Task CleanSelectedAsync()
        {
            var selectedOrphans = _listView.SelectedItems.Cast<ListViewItem>()
                .Select(l => l.Tag as FontResidualItem)
                .Where(f => f != null && f.IsOrphaned)
                .Cast<FontResidualItem>()
                .ToList();

            if (selectedOrphans.Count == 0) return;

            if (MessageBox.Show($"Are you sure you want to clean {selectedOrphans.Count} orphaned font registry entries?", "Confirm Cleanup", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            int cleaned = 0;
            await Task.Run(() =>
            {
                foreach (var item in selectedOrphans)
                {
                    if (FontResidualsCleaner.RemoveFontResidual(item))
                    {
                        cleaned++;
                        _allFonts.Remove(item);
                    }
                }
            });

            ApplyFilter();
            _statusLabel.Text = $"Cleaned {cleaned} orphaned font entries.";
            MessageBox.Show($"Successfully cleaned {cleaned} orphaned font registry entries.", "Cleanup Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async Task CleanAllOrphansAsync()
        {
            var orphans = _allFonts.Where(f => f.IsOrphaned).ToList();
            if (orphans.Count == 0) return;

            if (MessageBox.Show($"Are you sure you want to clean all {orphans.Count} orphaned font registry entries?", "Confirm Cleanup All", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            int cleaned = 0;
            await Task.Run(() =>
            {
                foreach (var item in orphans)
                {
                    if (FontResidualsCleaner.RemoveFontResidual(item))
                    {
                        cleaned++;
                        _allFonts.Remove(item);
                    }
                }
            });

            ApplyFilter();
            _statusLabel.Text = $"Cleaned all {cleaned} orphaned font entries.";
            MessageBox.Show($"Successfully cleaned all {cleaned} orphaned font registry entries.", "Cleanup Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

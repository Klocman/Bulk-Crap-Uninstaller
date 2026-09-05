/*
    EBUninstaller Pro - Windows Shell Icon & Thumbnail Cache Rebuilder Window
    Modern GUI for fixing broken desktop icons and purging thumbnail databases.
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using UninstallTools.Localization;
using UninstallTools.SystemTools;

namespace BulkCrapUninstaller.Forms.Windows
{
    public class ShellCacheRebuilderWindow : Form
    {
        private ListView _listView = null!;
        private CheckBox _chkRestartExplorer = null!;
        private Button _refreshBtn = null!;
        private Button _rebuildBtn = null!;
        private Label _statsLabel = null!;
        private StatusStrip _statusStrip = null!;
        private ToolStripStatusLabel _statusLabel = null!;

        private List<ShellCacheItem> _allCaches = new();

        public ShellCacheRebuilderWindow()
        {
            InitializeComponent();
            ApplyTheme();
            _ = LoadCachesAsync();
        }

        private void InitializeComponent()
        {
            Text = LanguageManager.GetString("ShellCache_Title") ?? "Shell Icon & Thumbnail Cache Rebuilder - EBUninstaller Pro";
            Size = new Size(1000, 580);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(800, 420);

            // Top Panel
            var topPanel = new Panel { Dock = DockStyle.Top, Height = 75, Padding = new Padding(12, 8, 12, 8) };

            _chkRestartExplorer = new CheckBox
            {
                Text = "Automatically restart Windows Explorer (Recommended for instant icon refresh)",
                Location = new Point(12, 12),
                AutoSize = true,
                Checked = true,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold)
            };

            _refreshBtn = new Button { Text = "🔄 Refresh", Location = new Point(620, 8), Size = new Size(90, 28), Font = new Font("Segoe UI", 9f) };
            _refreshBtn.Click += async (s, e) => await LoadCachesAsync();

            _rebuildBtn = new Button
            {
                Text = "⚡ Rebuild & Fix Broken Icons",
                Location = new Point(720, 8),
                Size = new Size(245, 28),
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 120, 40)
            };
            _rebuildBtn.Click += async (s, e) => await RebuildCachesAsync();

            _statsLabel = new Label
            {
                Text = "Scanning shell caches...",
                Location = new Point(12, 45),
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 100, 180)
            };

            topPanel.Controls.Add(_chkRestartExplorer);
            topPanel.Controls.Add(_refreshBtn);
            topPanel.Controls.Add(_rebuildBtn);
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
            _listView.Columns.Add("Cache Database", 260);
            _listView.Columns.Add("Status", 100);
            _listView.Columns.Add("Size", 100);
            _listView.Columns.Add("Description", 260);
            _listView.Columns.Add("File Path", 380);

            // StatusStrip
            _statusStrip = new StatusStrip();
            _statusLabel = new ToolStripStatusLabel { Spring = true, TextAlign = ContentAlignment.MiddleLeft, Text = "Ready." };
            _statusStrip.Items.Add(_statusLabel);

            Controls.Add(_listView);
            Controls.Add(topPanel);
            Controls.Add(_statusStrip);
        }

        private async Task LoadCachesAsync()
        {
            _refreshBtn.Enabled = false;
            _statusLabel.Text = "Scanning icon and thumbnail databases...";

            _allCaches = await Task.Run(() => ShellCacheRebuilder.ScanShellCaches());

            _listView.Items.Clear();
            long totalBytes = _allCaches.Sum(c => c.SizeBytes);
            int existingCount = _allCaches.Count(c => c.Exists);

            _statsLabel.Text = $"Active Databases: {existingCount} | Total Cache Size: {FormatSize(totalBytes)}";

            foreach (var c in _allCaches)
            {
                var lvi = new ListViewItem(c.CacheName) { Tag = c };
                lvi.SubItems.Add(c.Exists ? "Active" : "Not Found");
                lvi.SubItems.Add(FormatSize(c.SizeBytes));
                lvi.SubItems.Add(c.Description);
                lvi.SubItems.Add(c.FilePath);

                if (c.SizeBytes > 100L * 1024 * 1024) // > 100 MB
                    lvi.ForeColor = Color.FromArgb(190, 40, 40);

                _listView.Items.Add(lvi);
            }

            _refreshBtn.Enabled = true;
            _rebuildBtn.Enabled = existingCount > 0;
            _statusLabel.Text = $"Identified {existingCount} active shell cache databases.";
        }

        private async Task RebuildCachesAsync()
        {
            if (MessageBox.Show("This will clear corrupted icon and thumbnail databases and restart Windows Explorer to refresh all desktop and taskbar icons. Proceed?", "Confirm Rebuild", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            _rebuildBtn.Enabled = false;
            _refreshBtn.Enabled = false;
            _statusLabel.Text = "Rebuilding shell caches...";

            bool restart = _chkRestartExplorer.Checked;
            var (cleaned, freed, restarted) = await Task.Run(() => ShellCacheRebuilder.RebuildShellCaches(restart));

            MessageBox.Show($"Successfully purged {cleaned} shell cache files ({FormatSize(freed)} freed).\n\nWindows Explorer has regenerated pristine icons.", "Rebuild Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            await LoadCachesAsync();
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

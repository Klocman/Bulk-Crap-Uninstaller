/*
    EBUninstaller Pro - Package Managers Manager Window
    Modern GUI for managing WinGet, Chocolatey, and Scoop packages and purging download caches.
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using UninstallTools.Detection;
using UninstallTools.Localization;

namespace BulkCrapUninstaller.Forms.Windows
{
    public class PackageManagerWindow : Form
    {
        private ListView _listView = null!;
        private ToolStrip _toolStrip = null!;
        private ToolStripButton _refreshBtn = null!;
        private ToolStripButton _cleanCacheBtn = null!;
        private ToolStripComboBox _filterBox = null!;
        private Panel _summaryPanel = null!;
        private Label _packagesCountLabel = null!;
        private Label _updatesCountLabel = null!;
        private StatusStrip _statusStrip = null!;
        private ToolStripStatusLabel _statusLabel = null!;

        private List<ManagedPackageItem> _packages = new();

        public PackageManagerWindow()
        {
            InitializeComponent();
            ApplyTheme();
            Load += async (s, e) => await RefreshPackagesAsync();
        }

        private void InitializeComponent()
        {
            Text = LanguageManager.GetString("PackageManager_Title") ?? "Windows Package Managers (WinGet / Choco / Scoop) - EBUninstaller Pro";
            Size = new Size(1020, 620);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(800, 450);

            // Summary Panel
            _summaryPanel = new Panel { Dock = DockStyle.Top, Height = 60, Padding = new Padding(15, 8, 15, 8) };
            _packagesCountLabel = new Label
            {
                Text = "Managed Packages: 0",
                Font = new Font("Segoe UI", 11.5f, FontStyle.Bold),
                Location = new Point(15, 10),
                AutoSize = true
            };
            _updatesCountLabel = new Label
            {
                Text = "Available Updates: 0",
                Font = new Font("Segoe UI", 10.5f),
                Location = new Point(15, 34),
                AutoSize = true,
                ForeColor = Color.FromArgb(0, 120, 60)
            };
            _summaryPanel.Controls.Add(_packagesCountLabel);
            _summaryPanel.Controls.Add(_updatesCountLabel);

            // ToolStrip
            _toolStrip = new ToolStrip { ImageList = null, RenderMode = ToolStripRenderMode.System, GripStyle = ToolStripGripStyle.Hidden };
            _refreshBtn = new ToolStripButton("🔄 Refresh", null, async (s, e) => await RefreshPackagesAsync());
            _cleanCacheBtn = new ToolStripButton("🧹 Clean Package Caches", null, async (s, e) => await CleanCacheAsync());

            _filterBox = new ToolStripComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            _filterBox.Items.AddRange(new object[] { "All Managers", "WinGet Packages", "Chocolatey Packages", "Scoop Packages", "Updates Available Only" });
            _filterBox.SelectedIndex = 0;
            _filterBox.SelectedIndexChanged += (s, e) => ApplyFilter();

            _toolStrip.Items.Add(_refreshBtn);
            _toolStrip.Items.Add(new ToolStripSeparator());
            _toolStrip.Items.Add(_cleanCacheBtn);
            _toolStrip.Items.Add(new ToolStripSeparator());
            _toolStrip.Items.Add(new ToolStripLabel("View: "));
            _toolStrip.Items.Add(_filterBox);

            // ListView
            _listView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true
            };
            _listView.Columns.Add("Package Name", 260);
            _listView.Columns.Add("Package ID", 240);
            _listView.Columns.Add("Manager", 120);
            _listView.Columns.Add("Installed Version", 140);
            _listView.Columns.Add("Latest Available", 140);

            _statusStrip = new StatusStrip();
            _statusLabel = new ToolStripStatusLabel { Spring = true, TextAlign = ContentAlignment.MiddleLeft };
            _statusStrip.Items.Add(_statusLabel);

            Controls.Add(_listView);
            Controls.Add(_toolStrip);
            Controls.Add(_summaryPanel);
            Controls.Add(_statusStrip);
        }

        private async Task RefreshPackagesAsync()
        {
            _statusLabel.Text = "Scanning WinGet, Chocolatey, and Scoop packages...";
            _refreshBtn.Enabled = false;

            _packages = await Task.Run(() => PackageManagerUpdateEngine.ScanPackages(msg =>
            {
                BeginInvoke(new Action(() => _statusLabel.Text = msg));
            }));

            int updates = _packages.Count(p => p.HasUpdate);
            _packagesCountLabel.Text = $"Managed Packages: {_packages.Count}";
            _updatesCountLabel.Text = $"Available Package Updates: {updates}";

            ApplyFilter();
            _refreshBtn.Enabled = true;
            _statusLabel.Text = $"Scan complete. Found {_packages.Count} packages ({updates} updates).";
        }

        private void ApplyFilter()
        {
            _listView.Items.Clear();
            string filter = _filterBox.SelectedItem?.ToString() ?? "All Managers";

            var filtered = _packages.Where(p =>
            {
                if (filter == "WinGet Packages") return p.Manager == PackageManagerType.WinGet;
                if (filter == "Chocolatey Packages") return p.Manager == PackageManagerType.Chocolatey;
                if (filter == "Scoop Packages") return p.Manager == PackageManagerType.Scoop;
                if (filter == "Updates Available Only") return p.HasUpdate;
                return true;
            }).ToList();

            foreach (var p in filtered)
            {
                var lvi = new ListViewItem(p.Name) { Tag = p };
                lvi.SubItems.Add(p.PackageId);
                lvi.SubItems.Add(p.Manager.ToString());
                lvi.SubItems.Add(p.InstalledVersion);
                lvi.SubItems.Add(p.AvailableVersion);

                if (p.HasUpdate)
                {
                    lvi.ForeColor = Color.FromArgb(0, 120, 60);
                }

                _listView.Items.Add(lvi);
            }
        }

        private async Task CleanCacheAsync()
        {
            _cleanCacheBtn.Enabled = false;
            _statusLabel.Text = "Purging installer cache files from WinGet, Chocolatey, and Scoop...";

            var (freed, count) = await Task.Run(() => PackageManagerUpdateEngine.CleanPackageCaches(msg =>
            {
                BeginInvoke(new Action(() => _statusLabel.Text = msg));
            }));

            _cleanCacheBtn.Enabled = true;
            string freedStr = FormatSize(freed);
            MessageBox.Show($"Cleaned {count} cached installer packages, freeing {freedStr}.", "Cache Cleanup Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            _statusLabel.Text = $"Cleaned {count} cached packages ({freedStr} freed).";
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
            _summaryPanel.BackColor = isDark ? Color.FromArgb(40, 40, 40) : Color.FromArgb(235, 238, 245);

            if (LanguageManager.IsRightToLeft)
            {
                RightToLeft = RightToLeft.Yes;
                RightToLeftLayout = true;
            }
        }
    }
}

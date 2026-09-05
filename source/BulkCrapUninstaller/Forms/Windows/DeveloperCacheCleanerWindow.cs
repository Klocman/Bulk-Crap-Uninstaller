/*
    EBUninstaller Pro - Developer & Build Artifact Cache Cleaner Window
    Modern GUI for scanning and reclaiming gigabytes of developer package caches.
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
using UninstallTools.JunkCleaner;
using UninstallTools.Localization;

namespace BulkCrapUninstaller.Forms.Windows
{
    public class DeveloperCacheCleanerWindow : Form
    {
        private ListView _listView = null!;
        private ToolStrip _toolStrip = null!;
        private ToolStripButton _refreshBtn = null!;
        private ToolStripButton _selectAllBtn = null!;
        private ToolStripButton _purgeBtn = null!;
        private ToolStripButton _openFolderBtn = null!;
        private ToolStripComboBox _filterBox = null!;
        private Panel _summaryPanel = null!;
        private Label _totalSpaceLabel = null!;
        private Label _cacheCountLabel = null!;
        private StatusStrip _statusStrip = null!;
        private ToolStripStatusLabel _statusLabel = null!;

        private List<DevCacheLocationItem> _items = new();

        public DeveloperCacheCleanerWindow()
        {
            InitializeComponent();
            ApplyTheme();
            Load += async (s, e) => await RefreshCachesAsync();
        }

        private void InitializeComponent()
        {
            Text = LanguageManager.GetString("DevCache_Title") ?? "Developer & Build Package Cache Cleaner - EBUninstaller Pro";
            Size = new Size(1080, 640);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(850, 480);

            // Summary Panel
            _summaryPanel = new Panel { Dock = DockStyle.Top, Height = 60, Padding = new Padding(15, 8, 15, 8) };
            _cacheCountLabel = new Label
            {
                Text = "Developer Caches: 0",
                Font = new Font("Segoe UI", 11.5f, FontStyle.Bold),
                Location = new Point(15, 10),
                AutoSize = true
            };
            _totalSpaceLabel = new Label
            {
                Text = "Reclaimable Disk Space: 0 MB",
                Font = new Font("Segoe UI", 10.5f),
                Location = new Point(15, 34),
                AutoSize = true,
                ForeColor = Color.FromArgb(0, 120, 40)
            };
            _summaryPanel.Controls.Add(_cacheCountLabel);
            _summaryPanel.Controls.Add(_totalSpaceLabel);

            // ToolStrip
            _toolStrip = new ToolStrip { ImageList = null, RenderMode = ToolStripRenderMode.System, GripStyle = ToolStripGripStyle.Hidden };
            _refreshBtn = new ToolStripButton("🔄 Refresh", null, async (s, e) => await RefreshCachesAsync());
            _selectAllBtn = new ToolStripButton("☑ Select All", null, (s, e) => SelectAll(true));
            _purgeBtn = new ToolStripButton("🗑️ Purge Selected Caches", null, async (s, e) => await PurgeSelectedAsync()) { Enabled = false, ForeColor = Color.FromArgb(180, 40, 40) };
            _openFolderBtn = new ToolStripButton("📁 Show Folder", null, (s, e) => ShowSelectedFolder()) { Enabled = false };

            _filterBox = new ToolStripComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            _filterBox.Items.AddRange(new object[] { "All Ecosystems", ".NET / NuGet", "Node / npm / Yarn / pnpm", "Python / pip / Conda", "Rust / Cargo", "Java / Gradle / Maven", "Golang", "Visual Studio / C++" });
            _filterBox.SelectedIndex = 0;
            _filterBox.SelectedIndexChanged += (s, e) => ApplyFilter();

            _toolStrip.Items.Add(_refreshBtn);
            _toolStrip.Items.Add(new ToolStripSeparator());
            _toolStrip.Items.Add(_selectAllBtn);
            _toolStrip.Items.Add(_purgeBtn);
            _toolStrip.Items.Add(_openFolderBtn);
            _toolStrip.Items.Add(new ToolStripSeparator());
            _toolStrip.Items.Add(new ToolStripLabel("Ecosystem: "));
            _toolStrip.Items.Add(_filterBox);

            // ListView
            _listView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                CheckBoxes = true,
                FullRowSelect = true,
                GridLines = true
            };
            _listView.Columns.Add("Cache Target", 240);
            _listView.Columns.Add("Ecosystem", 140);
            _listView.Columns.Add("Space Occupied", 110);
            _listView.Columns.Add("Files Count", 90);
            _listView.Columns.Add("Description", 240);
            _listView.Columns.Add("Directory Path", 350);

            _listView.SelectedIndexChanged += (s, e) =>
            {
                _openFolderBtn.Enabled = _listView.SelectedItems.Count == 1 &&
                                        Directory.Exists((_listView.SelectedItems[0].Tag as DevCacheLocationItem)?.DirectoryPath);
            };

            _listView.ItemChecked += (s, e) =>
            {
                _purgeBtn.Enabled = _listView.CheckedItems.Count > 0;
            };

            // StatusStrip
            _statusStrip = new StatusStrip();
            _statusLabel = new ToolStripStatusLabel { Spring = true, TextAlign = ContentAlignment.MiddleLeft };
            _statusStrip.Items.Add(_statusLabel);

            Controls.Add(_listView);
            Controls.Add(_toolStrip);
            Controls.Add(_summaryPanel);
            Controls.Add(_statusStrip);
        }

        private async Task RefreshCachesAsync()
        {
            _statusLabel.Text = "Scanning developer and build package caches...";
            _refreshBtn.Enabled = false;
            _purgeBtn.Enabled = false;

            _items = await Task.Run(() => DeveloperCacheCleaner.ScanDeveloperCaches(msg =>
            {
                BeginInvoke(new Action(() => _statusLabel.Text = msg));
            }));

            long totalBytes = _items.Sum(i => i.SizeBytes);
            int withData = _items.Count(i => i.SizeBytes > 0);

            _cacheCountLabel.Text = $"Developer Caches: {_items.Count} ({withData} active)";
            _totalSpaceLabel.Text = $"Reclaimable Disk Space: {FormatSize(totalBytes)}";

            ApplyFilter();
            _refreshBtn.Enabled = true;
            _statusLabel.Text = $"Scan complete. Identified {FormatSize(totalBytes)} of reclaimable developer caches.";
        }

        private void ApplyFilter()
        {
            _listView.Items.Clear();
            string filter = _filterBox.SelectedItem?.ToString() ?? "All Ecosystems";

            var filtered = _items.Where(i =>
            {
                if (filter == ".NET / NuGet") return i.Ecosystem == DevToolEcosystem.DotNetNuGet;
                if (filter == "Node / npm / Yarn / pnpm") return i.Ecosystem == DevToolEcosystem.NodeNpmYarnPnpm;
                if (filter == "Python / pip / Conda") return i.Ecosystem == DevToolEcosystem.PythonPipConda;
                if (filter == "Rust / Cargo") return i.Ecosystem == DevToolEcosystem.RustCargo;
                if (filter == "Java / Gradle / Maven") return i.Ecosystem == DevToolEcosystem.JavaGradleMaven;
                if (filter == "Golang") return i.Ecosystem == DevToolEcosystem.Golang;
                if (filter == "Visual Studio / C++") return i.Ecosystem == DevToolEcosystem.VisualStudioAndCpp;
                return true;
            }).ToList();

            foreach (var item in filtered)
            {
                var lvi = new ListViewItem(item.EcosystemName) { Tag = item, Checked = item.IsSelected };
                lvi.SubItems.Add(FormatEcosystem(item.Ecosystem));
                lvi.SubItems.Add(FormatSize(item.SizeBytes));
                lvi.SubItems.Add(item.FilesCount.ToString("N0"));
                lvi.SubItems.Add(item.Description);
                lvi.SubItems.Add(item.DirectoryPath);

                if (item.SizeBytes > 1024L * 1024 * 1024) // > 1 GB
                {
                    lvi.ForeColor = Color.FromArgb(190, 40, 40);
                }
                else if (item.SizeBytes == 0)
                {
                    lvi.ForeColor = Color.Gray;
                }

                _listView.Items.Add(lvi);
            }
        }

        private void SelectAll(bool select)
        {
            foreach (ListViewItem lvi in _listView.Items)
            {
                lvi.Checked = select;
            }
        }

        private void ShowSelectedFolder()
        {
            if (_listView.SelectedItems.Count == 0) return;
            if (_listView.SelectedItems[0].Tag is DevCacheLocationItem item)
            {
                if (Directory.Exists(item.DirectoryPath))
                {
                    try
                    {
                        Process.Start(new ProcessStartInfo("explorer.exe", item.DirectoryPath) { UseShellExecute = true });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Could not open Explorer: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private async Task PurgeSelectedAsync()
        {
            var selected = _listView.CheckedItems.Cast<ListViewItem>()
                .Select(l => l.Tag as DevCacheLocationItem)
                .Where(i => i != null && i.SizeBytes > 0)
                .Cast<DevCacheLocationItem>()
                .ToList();

            if (selected.Count == 0)
            {
                MessageBox.Show("Please select at least one cache with stored data to purge.", "EBUninstaller Pro", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            long totalBytes = selected.Sum(s => s.SizeBytes);
            if (MessageBox.Show($"Purge {selected.Count} developer cache locations?\n\nThis will safely free {FormatSize(totalBytes)} of disk space. Any needed packages will be re-downloaded on demand during future builds.", "Confirm Cache Purge", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            _purgeBtn.Enabled = false;
            _refreshBtn.Enabled = false;

            var (count, freed) = await Task.Run(() => DeveloperCacheCleaner.PurgeDeveloperCaches(selected, msg =>
            {
                BeginInvoke(new Action(() => _statusLabel.Text = msg));
            }));

            MessageBox.Show($"Successfully purged {count} cached files, freeing {FormatSize(freed)} of storage.", "Purge Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            await RefreshCachesAsync();
        }

        private static string FormatEcosystem(DevToolEcosystem eco)
        {
            return eco switch
            {
                DevToolEcosystem.DotNetNuGet => ".NET / NuGet",
                DevToolEcosystem.NodeNpmYarnPnpm => "Node.js / JavaScript",
                DevToolEcosystem.PythonPipConda => "Python / pip",
                DevToolEcosystem.RustCargo => "Rust / Cargo",
                DevToolEcosystem.JavaGradleMaven => "Java / Gradle / Maven",
                DevToolEcosystem.Golang => "Go Language",
                DevToolEcosystem.VisualStudioAndCpp => "Visual Studio / C++",
                _ => "Other"
            };
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

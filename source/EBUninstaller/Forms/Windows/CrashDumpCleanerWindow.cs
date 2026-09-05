/*
    EBUninstaller Pro - Crash Dump & WER Cleaner Window
    Modern GUI for analyzing and purging kernel memory dumps, minidumps, and WER crash reports.
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using UninstallTools.JunkCleaner;
using UninstallTools.Localization;

namespace BulkCrapUninstaller.Forms.Windows
{
    public class CrashDumpCleanerWindow : Form
    {
        private ListView _listView = null!;
        private ToolStrip _toolStrip = null!;
        private ToolStripButton _refreshBtn = null!;
        private ToolStripButton _cleanBtn = null!;
        private ToolStripButton _selectAllBtn = null!;
        private ToolStripComboBox _filterBox = null!;
        private Panel _summaryPanel = null!;
        private Label _totalSpaceLabel = null!;
        private Label _dumpCountLabel = null!;
        private StatusStrip _statusStrip = null!;
        private ToolStripStatusLabel _statusLabel = null!;

        private List<CrashDumpItem> _items = new();

        public CrashDumpCleanerWindow()
        {
            InitializeComponent();
            ApplyTheme();
            Load += async (s, e) => await RefreshDumpsAsync();
        }

        private void InitializeComponent()
        {
            Text = LanguageManager.GetString("CrashDumpCleaner_Title") ?? "Crash Dump & Memory Dump Cleaner - EBUninstaller Pro";
            Size = new Size(1020, 620);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(820, 460);

            // Summary Panel
            _summaryPanel = new Panel { Dock = DockStyle.Top, Height = 60, Padding = new Padding(15, 8, 15, 8) };
            _dumpCountLabel = new Label
            {
                Text = "Crash Dumps & Error Reports: 0",
                Font = new Font("Segoe UI", 11.5f, FontStyle.Bold),
                Location = new Point(15, 10),
                AutoSize = true
            };
            _totalSpaceLabel = new Label
            {
                Text = "Space Occupied: 0 MB",
                Font = new Font("Segoe UI", 10.5f),
                Location = new Point(15, 34),
                AutoSize = true,
                ForeColor = Color.FromArgb(180, 50, 50)
            };
            _summaryPanel.Controls.Add(_dumpCountLabel);
            _summaryPanel.Controls.Add(_totalSpaceLabel);

            // ToolStrip
            _toolStrip = new ToolStrip { ImageList = null, RenderMode = ToolStripRenderMode.System, GripStyle = ToolStripGripStyle.Hidden };
            _refreshBtn = new ToolStripButton("🔄 Refresh", null, async (s, e) => await RefreshDumpsAsync());
            _selectAllBtn = new ToolStripButton("☑ Select All", null, (s, e) => SelectAll(true));
            _cleanBtn = new ToolStripButton("🗑️ Clean Selected Dumps", null, async (s, e) => await CleanSelectedDumpsAsync()) { Enabled = false };

            _filterBox = new ToolStripComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            _filterBox.Items.AddRange(new object[] { "All Dumps & Reports", "Kernel BSOD Dumps", "User-Mode Crash Dumps", "Windows Error Reporting (WER)" });
            _filterBox.SelectedIndex = 0;
            _filterBox.SelectedIndexChanged += (s, e) => ApplyFilter();

            _toolStrip.Items.Add(_refreshBtn);
            _toolStrip.Items.Add(new ToolStripSeparator());
            _toolStrip.Items.Add(_selectAllBtn);
            _toolStrip.Items.Add(_cleanBtn);
            _toolStrip.Items.Add(new ToolStripSeparator());
            _toolStrip.Items.Add(new ToolStripLabel("Filter: "));
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
            _listView.Columns.Add("Dump / Report File", 260);
            _listView.Columns.Add("Faulting Application", 180);
            _listView.Columns.Add("Dump Type", 150);
            _listView.Columns.Add("File Size", 100);
            _listView.Columns.Add("Crash Date", 130);
            _listView.Columns.Add("Path", 380);

            _statusStrip = new StatusStrip();
            _statusLabel = new ToolStripStatusLabel { Spring = true, TextAlign = ContentAlignment.MiddleLeft };
            _statusStrip.Items.Add(_statusLabel);

            Controls.Add(_listView);
            Controls.Add(_toolStrip);
            Controls.Add(_summaryPanel);
            Controls.Add(_statusStrip);
        }

        private async Task RefreshDumpsAsync()
        {
            _statusLabel.Text = "Scanning Windows directories for crash dumps and memory logs...";
            _refreshBtn.Enabled = false;
            _cleanBtn.Enabled = false;

            _items = await Task.Run(() => CrashDumpCleaner.ScanCrashDumps(msg =>
            {
                BeginInvoke(new Action(() => _statusLabel.Text = msg));
            }));

            long totalBytes = _items.Sum(i => i.SizeBytes);
            _dumpCountLabel.Text = $"Crash Dumps & Error Reports: {_items.Count}";
            _totalSpaceLabel.Text = $"Total Space Occupied: {FormatSize(totalBytes)}";

            ApplyFilter();
            _refreshBtn.Enabled = true;
            _cleanBtn.Enabled = _items.Count > 0;
            _statusLabel.Text = $"Scan complete. Found {_items.Count} crash dump artifacts ({FormatSize(totalBytes)}).";
        }

        private void ApplyFilter()
        {
            _listView.Items.Clear();
            string filter = _filterBox.SelectedItem?.ToString() ?? "All Dumps & Reports";

            var filtered = _items.Where(i =>
            {
                if (filter == "Kernel BSOD Dumps") return i.Kind == CrashDumpKind.KernelMemoryDump || i.Kind == CrashDumpKind.Minidump || i.Kind == CrashDumpKind.LiveKernelReport;
                if (filter == "User-Mode Crash Dumps") return i.Kind == CrashDumpKind.UserModeCrashDump;
                if (filter == "Windows Error Reporting (WER)") return i.Kind == CrashDumpKind.WindowsErrorReporting;
                return true;
            }).ToList();

            foreach (var item in filtered)
            {
                var lvi = new ListViewItem(item.FileName) { Tag = item, Checked = item.IsSelected };
                lvi.SubItems.Add(item.TargetProcess);
                lvi.SubItems.Add(FormatKind(item.Kind));
                lvi.SubItems.Add(FormatSize(item.SizeBytes));
                lvi.SubItems.Add(item.CreatedDate.ToLocalTime().ToString("yyyy-MM-dd HH:mm"));
                lvi.SubItems.Add(item.FilePath);

                if (item.Kind == CrashDumpKind.KernelMemoryDump)
                {
                    lvi.ForeColor = Color.FromArgb(190, 40, 40);
                }
                else if (item.Kind == CrashDumpKind.UserModeCrashDump)
                {
                    lvi.ForeColor = Color.FromArgb(160, 90, 0);
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

        private async Task CleanSelectedDumpsAsync()
        {
            var selectedItems = _listView.CheckedItems.Cast<ListViewItem>()
                .Select(l => l.Tag as CrashDumpItem)
                .Where(i => i != null)
                .Cast<CrashDumpItem>()
                .ToList();

            if (selectedItems.Count == 0)
            {
                MessageBox.Show("Please select at least one crash dump to delete.", "EBUninstaller Pro", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show($"Are you sure you want to permanently delete {selectedItems.Count} crash dump file(s)?", "Confirm Dump Cleanup", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            _cleanBtn.Enabled = false;
            _refreshBtn.Enabled = false;

            var (count, freed) = await Task.Run(() => CrashDumpCleaner.DeleteCrashDumps(selectedItems, msg =>
            {
                BeginInvoke(new Action(() => _statusLabel.Text = msg));
            }));

            MessageBox.Show($"Successfully cleaned {count} crash dumps, freeing {FormatSize(freed)} of storage.", "Cleanup Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            await RefreshDumpsAsync();
        }

        private static string FormatKind(CrashDumpKind kind)
        {
            return kind switch
            {
                CrashDumpKind.KernelMemoryDump => "Full Kernel Memory Dump",
                CrashDumpKind.Minidump => "Windows Minidump",
                CrashDumpKind.UserModeCrashDump => "User-Mode Crash Dump",
                CrashDumpKind.WindowsErrorReporting => "WER Crash Archive",
                CrashDumpKind.LiveKernelReport => "Live Kernel Report",
                _ => "Crash Log"
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

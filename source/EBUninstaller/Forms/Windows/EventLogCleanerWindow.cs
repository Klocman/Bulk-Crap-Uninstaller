/*
    EBUninstaller Pro - Windows Event Log Cleaner Window
    Modern GUI for reviewing, auditing, and purging cluttered Windows event logs.
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
    public class EventLogCleanerWindow : Form
    {
        private ListView _listView = null!;
        private ToolStrip _toolStrip = null!;
        private ToolStripButton _refreshBtn = null!;
        private ToolStripButton _clearBtn = null!;
        private ToolStripButton _selectAllBtn = null!;
        private ToolStripComboBox _filterBox = null!;
        private Panel _summaryPanel = null!;
        private Label _logCountLabel = null!;
        private Label _recordsCountLabel = null!;
        private StatusStrip _statusStrip = null!;
        private ToolStripStatusLabel _statusLabel = null!;

        private List<EventLogItem> _items = new();

        public EventLogCleanerWindow()
        {
            InitializeComponent();
            ApplyTheme();
            Load += async (s, e) => await RefreshLogsAsync();
        }

        private void InitializeComponent()
        {
            Text = LanguageManager.GetString("EventLogCleaner_Title") ?? "Windows Event Log & Diagnostic Trace Cleaner - EBUninstaller Pro";
            Size = new Size(1000, 600);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(800, 450);

            // Summary Panel
            _summaryPanel = new Panel { Dock = DockStyle.Top, Height = 60, Padding = new Padding(15, 8, 15, 8) };
            _logCountLabel = new Label
            {
                Text = "Event Logs Registered: 0",
                Font = new Font("Segoe UI", 11.5f, FontStyle.Bold),
                Location = new Point(15, 10),
                AutoSize = true
            };
            _recordsCountLabel = new Label
            {
                Text = "Total Recorded Events: 0",
                Font = new Font("Segoe UI", 10.5f),
                Location = new Point(15, 34),
                AutoSize = true,
                ForeColor = Color.FromArgb(0, 100, 180)
            };
            _summaryPanel.Controls.Add(_logCountLabel);
            _summaryPanel.Controls.Add(_recordsCountLabel);

            // ToolStrip
            _toolStrip = new ToolStrip { ImageList = null, RenderMode = ToolStripRenderMode.System, GripStyle = ToolStripGripStyle.Hidden };
            _refreshBtn = new ToolStripButton("🔄 Refresh", null, async (s, e) => await RefreshLogsAsync());
            _selectAllBtn = new ToolStripButton("☑ Select All", null, (s, e) => SelectAll(true));
            _clearBtn = new ToolStripButton("🧹 Clear Selected Logs", null, async (s, e) => await ClearSelectedLogsAsync()) { Enabled = false };

            _filterBox = new ToolStripComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            _filterBox.Items.AddRange(new object[] { "All Event Logs", "Application Logs", "Setup Logs", "System Logs", "Diagnostic Traces" });
            _filterBox.SelectedIndex = 0;
            _filterBox.SelectedIndexChanged += (s, e) => ApplyFilter();

            _toolStrip.Items.Add(_refreshBtn);
            _toolStrip.Items.Add(new ToolStripSeparator());
            _toolStrip.Items.Add(_selectAllBtn);
            _toolStrip.Items.Add(_clearBtn);
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
            _listView.Columns.Add("Event Log Name", 260);
            _listView.Columns.Add("Category", 130);
            _listView.Columns.Add("Record Count", 120);
            _listView.Columns.Add("Internal Identifier", 350);

            _statusStrip = new StatusStrip();
            _statusLabel = new ToolStripStatusLabel { Spring = true, TextAlign = ContentAlignment.MiddleLeft };
            _statusStrip.Items.Add(_statusLabel);

            Controls.Add(_listView);
            Controls.Add(_toolStrip);
            Controls.Add(_summaryPanel);
            Controls.Add(_statusStrip);
        }

        private async Task RefreshLogsAsync()
        {
            _statusLabel.Text = "Scanning Windows Event Logs and diagnostic traces...";
            _refreshBtn.Enabled = false;
            _clearBtn.Enabled = false;

            _items = await Task.Run(() => EventLogResidualsCleaner.ScanEventLogs(msg =>
            {
                BeginInvoke(new Action(() => _statusLabel.Text = msg));
            }));

            long totalRecords = _items.Sum(i => i.RecordCount);
            _logCountLabel.Text = $"Event Logs Registered: {_items.Count}";
            _recordsCountLabel.Text = $"Total Recorded Events: {totalRecords:N0}";

            ApplyFilter();
            _refreshBtn.Enabled = true;
            _clearBtn.Enabled = _items.Any(i => !i.IsCriticalProtected);
            _statusLabel.Text = $"Found {_items.Count} event logs ({totalRecords:N0} records).";
        }

        private void ApplyFilter()
        {
            _listView.Items.Clear();
            string filter = _filterBox.SelectedItem?.ToString() ?? "All Event Logs";

            var filtered = _items.Where(i =>
            {
                if (filter == "Application Logs") return i.Category == EventLogCategory.Application;
                if (filter == "Setup Logs") return i.Category == EventLogCategory.Setup;
                if (filter == "System Logs") return i.Category == EventLogCategory.System;
                if (filter == "Diagnostic Traces") return i.Category == EventLogCategory.DiagnosticTrace;
                return true;
            }).ToList();

            foreach (var item in filtered)
            {
                var lvi = new ListViewItem(item.DisplayName) { Tag = item, Checked = item.IsSelected };
                lvi.SubItems.Add(item.Category.ToString());
                lvi.SubItems.Add(item.RecordCount > 0 ? $"{item.RecordCount:N0}" : "-");
                lvi.SubItems.Add(item.LogName);

                if (item.IsCriticalProtected)
                {
                    lvi.ForeColor = Color.FromArgb(0, 100, 180);
                }
                else if (item.RecordCount > 1000)
                {
                    lvi.ForeColor = Color.FromArgb(180, 50, 50);
                }

                _listView.Items.Add(lvi);
            }
        }

        private void SelectAll(bool select)
        {
            foreach (ListViewItem lvi in _listView.Items)
            {
                if (lvi.Tag is EventLogItem item && !item.IsCriticalProtected)
                {
                    lvi.Checked = select;
                }
            }
        }

        private async Task ClearSelectedLogsAsync()
        {
            var selectedLogs = _listView.CheckedItems.Cast<ListViewItem>()
                .Select(l => l.Tag as EventLogItem)
                .Where(i => i != null && !i.IsCriticalProtected)
                .Cast<EventLogItem>()
                .ToList();

            if (selectedLogs.Count == 0)
            {
                MessageBox.Show("Please select at least one non-protected event log to clear.", "EBUninstaller Pro", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show($"Are you sure you want to clear {selectedLogs.Count} selected event log(s)?", "Confirm Event Log Purge", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            _clearBtn.Enabled = false;
            _refreshBtn.Enabled = false;

            var (cleared, records) = await Task.Run(() => EventLogResidualsCleaner.ClearEventLogs(selectedLogs, msg =>
            {
                BeginInvoke(new Action(() => _statusLabel.Text = msg));
            }));

            MessageBox.Show($"Successfully cleared {cleared} event logs ({records:N0} records removed).", "Logs Cleared", MessageBoxButtons.OK, MessageBoxIcon.Information);
            await RefreshLogsAsync();
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

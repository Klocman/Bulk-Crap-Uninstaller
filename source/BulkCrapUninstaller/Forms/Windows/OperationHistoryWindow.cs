/*
    OpenUninstall Pro - Professional Next-Generation Windows Uninstaller
    Operation History & Audit Log Window
*/

using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using BrightIdeasSoftware;
using UninstallTools.History;

namespace BulkCrapUninstaller.Forms.Windows
{
    public sealed class OperationHistoryWindow : Form
    {
        private FastObjectListView _folvHistory;
        private TextBox _txtSearch;
        private TextBox _txtDetails;
        private Button _btnExportCsv;
        private Button _btnExportJson;
        private Button _btnClear;
        private Button _btnClose;
        private Label _lblStatus;

        public OperationHistoryWindow()
        {
            InitializeComponent();
            RefreshHistory();
        }

        private void InitializeComponent()
        {
            Text = "OpenUninstall Pro - Operation History & Audit Log";
            Size = new Size(950, 600);
            MinimumSize = new Size(700, 480);
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Padding = new Padding(12)
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Search bar
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // Split
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Status
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Buttons

            // Search bar
            var searchLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, Margin = new Padding(0, 0, 0, 8) };
            searchLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            searchLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            var lblSearch = new Label { Text = "Filter History:", AutoSize = true, Margin = new Padding(0, 5, 8, 0) };
            _txtSearch = new TextBox { Dock = DockStyle.Fill };
            _txtSearch.TextChanged += (s, e) => RefreshHistory();

            searchLayout.Controls.Add(lblSearch, 0, 0);
            searchLayout.Controls.Add(_txtSearch, 1, 0);
            mainLayout.Controls.Add(searchLayout, 0, 0);

            // Split Container
            var split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 300
            };

            _folvHistory = new FastObjectListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                MultiSelect = false,
                ShowGroups = false,
                GridLines = true
            };

            var colTime = new OLVColumn("Timestamp", nameof(OperationHistoryEntry.Timestamp)) { Width = 150, AspectToStringConverter = v => ((DateTime)v).ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss") };
            var colOp = new OLVColumn("Operation", nameof(OperationHistoryEntry.OperationType)) { Width = 140 };
            var colStatus = new OLVColumn("Status", nameof(OperationHistoryEntry.Status)) { Width = 90 };
            var colApp = new OLVColumn("Application", nameof(OperationHistoryEntry.ApplicationName)) { Width = 260, FillsFreeSpace = true };
            var colDel = new OLVColumn("Deleted Items", nameof(OperationHistoryEntry.DeletedItemsCount)) { Width = 90 };
            var colBackup = new OLVColumn("Backup ID", nameof(OperationHistoryEntry.BackupId)) { Width = 130 };

            _folvHistory.AllColumns.AddRange(new[] { colTime, colOp, colStatus, colApp, colDel, colBackup });
            _folvHistory.RebuildColumns();
            _folvHistory.SelectionChanged += (s, e) => OnHistorySelected();
            split.Panel1.Controls.Add(_folvHistory);

            _txtDetails = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Consolas", 8.5F, FontStyle.Regular, GraphicsUnit.Point)
            };
            split.Panel2.Controls.Add(_txtDetails);

            mainLayout.Controls.Add(split, 0, 1);

            // Status label
            _lblStatus = new Label { Text = "Showing history records.", AutoSize = true, Margin = new Padding(0, 4, 0, 4) };
            mainLayout.Controls.Add(_lblStatus, 0, 2);

            // Buttons
            var btnPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft, AutoSize = true };
            _btnClose = new Button { Text = "Close", DialogResult = DialogResult.Cancel, AutoSize = true };
            _btnClear = new Button { Text = "Clear History", AutoSize = true, ForeColor = Color.DarkRed };
            _btnClear.Click += (s, e) => ClearHistory();

            _btnExportJson = new Button { Text = "Export JSON...", AutoSize = true };
            _btnExportJson.Click += (s, e) => ExportJson();

            _btnExportCsv = new Button { Text = "Export CSV...", AutoSize = true };
            _btnExportCsv.Click += (s, e) => ExportCsv();

            btnPanel.Controls.Add(_btnClose);
            btnPanel.Controls.Add(_btnClear);
            btnPanel.Controls.Add(_btnExportJson);
            btnPanel.Controls.Add(_btnExportCsv);
            mainLayout.Controls.Add(btnPanel, 0, 3);

            Controls.Add(mainLayout);
        }

        private void RefreshHistory()
        {
            var search = _txtSearch.Text?.Trim();
            var items = OperationHistoryManager.GetHistory(search);
            _folvHistory.SetObjects(items);
            _lblStatus.Text = $"Found {items.Count} operation history entries.";
            _txtDetails.Text = string.Empty;
        }

        private void OnHistorySelected()
        {
            var selected = _folvHistory.SelectedObject as OperationHistoryEntry;
            if (selected == null)
            {
                _txtDetails.Text = string.Empty;
                return;
            }

            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"OPERATION AUDIT LOG: {selected.OperationType}");
            sb.AppendLine($"History ID: {selected.HistoryId}");
            sb.AppendLine($"Timestamp: {selected.Timestamp.ToLocalTime():yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"Target Application: {selected.ApplicationName}");
            sb.AppendLine($"Publisher: {selected.Publisher}");
            sb.AppendLine($"Status: {selected.Status}");
            if (!string.IsNullOrEmpty(selected.BackupId))
                sb.AppendLine($"Associated Backup ID: {selected.BackupId}");
            sb.AppendLine($"Items Detected: {selected.DetectedItemsCount}, Deleted: {selected.DeletedItemsCount}, Failed: {selected.FailedItemsCount}");
            sb.AppendLine();
            if (selected.RemovedItems.Count > 0)
            {
                sb.AppendLine("REMOVED ITEMS:");
                foreach (var item in selected.RemovedItems)
                    sb.AppendLine($" - {item}");
            }
            if (selected.Warnings.Count > 0)
            {
                sb.AppendLine("WARNINGS:");
                foreach (var w in selected.Warnings)
                    sb.AppendLine($" [!] {w}");
            }
            if (selected.Errors.Count > 0)
            {
                sb.AppendLine("ERRORS:");
                foreach (var err in selected.Errors)
                    sb.AppendLine($" [X] {err}");
            }

            _txtDetails.Text = sb.ToString();
        }

        private void ClearHistory()
        {
            if (MessageBox.Show("Are you sure you want to clear all operation history records?", "Confirm Clear", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                OperationHistoryManager.ClearHistory();
                RefreshHistory();
            }
        }

        private void ExportCsv()
        {
            using var sfd = new SaveFileDialog { FileName = "OpenUninstall_History.csv", Filter = "CSV File (*.csv)|*.csv" };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                var csv = OperationHistoryManager.ExportHistoryToCsv();
                File.WriteAllText(sfd.FileName, csv, System.Text.Encoding.UTF8);
                MessageBox.Show("History exported to CSV successfully.", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ExportJson()
        {
            using var sfd = new SaveFileDialog { FileName = "OpenUninstall_History.json", Filter = "JSON File (*.json)|*.json" };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                var json = OperationHistoryManager.ExportHistoryToJson();
                File.WriteAllText(sfd.FileName, json, System.Text.Encoding.UTF8);
                MessageBox.Show("History exported to JSON successfully.", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}

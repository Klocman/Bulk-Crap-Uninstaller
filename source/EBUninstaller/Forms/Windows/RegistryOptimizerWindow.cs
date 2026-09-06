/*
    EBUninstaller Pro - Registry Optimizer & Integrity Repair Window
*/

using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BulkCrapUninstaller.Functions;
using UninstallTools.Core;
using UninstallTools.RegistryEngine;

namespace BulkCrapUninstaller.Forms.Windows
{
    public sealed class RegistryOptimizerWindow : Form
    {
        private ListView _lvIssues;
        private Label _lblStatus;
        private CheckBox _chkBackup;
        private Button _btnScan;
        private Button _btnFix;
        private Button _btnClose;
        private RegistryOptimizationScanResult _lastResult;

        public RegistryOptimizerWindow()
        {
            InitializeComponent();
            ThemeEngine.ApplyThemeToForm(this);
            StartScan();
        }

        private void InitializeComponent()
        {
            Text = "EBUninstaller Pro - Registry Optimizer & Integrity Repair";
            Size = new Size(840, 520);
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            MinimumSize = new Size(680, 420);

            var mainTable = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(12)
            };
            mainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 40)); // Status/Header
            mainTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // List View
            mainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 45)); // Action Buttons

            _lblStatus = new Label
            {
                Text = "Scanning Windows Registry for invalid references and obsolete keys...",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };
            mainTable.Controls.Add(_lblStatus, 0, 0);

            _lvIssues = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                CheckBoxes = true,
                FullRowSelect = true,
                GridLines = true
            };
            _lvIssues.Columns.Add("Type", 140);
            _lvIssues.Columns.Add("Registry Key", 280);
            _lvIssues.Columns.Add("Value / Target", 180);
            _lvIssues.Columns.Add("Description", 280);
            mainTable.Controls.Add(_lvIssues, 0, 1);

            var bottomTable = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1
            };
            bottomTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            bottomTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

            _chkBackup = new CheckBox
            {
                Text = "Create cryptographic .reg backup before fixing (Recommended)",
                Checked = true,
                AutoSize = true,
                Anchor = AnchorStyles.Left
            };
            bottomTable.Controls.Add(_chkBackup, 0, 0);

            var btnFlow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft
            };
            _btnClose = new Button { Text = "Close", Size = new Size(95, 32), DialogResult = DialogResult.OK };
            _btnFix = new Button { Text = "Fix Selected Issues", Size = new Size(150, 32), Font = new Font(Font, FontStyle.Bold) };
            _btnFix.Click += BtnFix_Click;
            _btnScan = new Button { Text = "Scan Again", Size = new Size(105, 32) };
            _btnScan.Click += (s, e) => StartScan();

            btnFlow.Controls.Add(_btnClose);
            btnFlow.Controls.Add(_btnFix);
            btnFlow.Controls.Add(_btnScan);
            bottomTable.Controls.Add(btnFlow, 1, 0);

            mainTable.Controls.Add(bottomTable, 0, 2);
            Controls.Add(mainTable);
        }

        private void StartScan()
        {
            _lvIssues.Items.Clear();
            _btnFix.Enabled = false;
            _lblStatus.Text = "Scanning Windows Registry...";

            _lastResult = RegistryOptimizerEngine.ScanRegistryIssues();

            _lblStatus.Text = $"Found {_lastResult.Issues.Count} registry issue(s) across {_lastResult.TotalKeysScanned} inspected keys.";

            foreach (var iss in _lastResult.Issues)
            {
                var lvi = new ListViewItem(iss.IssueType.ToString()) { Checked = true };
                lvi.SubItems.Add(iss.KeyPath);
                lvi.SubItems.Add(iss.ValueName);
                lvi.SubItems.Add(iss.Description);
                lvi.Tag = iss;
                _lvIssues.Items.Add(lvi);
            }

            _btnFix.Enabled = _lastResult.Issues.Count > 0;
        }

        private void BtnFix_Click(object sender, EventArgs e)
        {
            var selectedIssues = _lvIssues.Items.Cast<ListViewItem>().Where(i => i.Checked).Select(i => i.Tag as RegistryIssue).Where(iss => iss != null).ToList();
            if (selectedIssues.Count == 0)
            {
                MessageBox.Show("Please select at least one registry issue to fix.", "None Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show($"Are you sure you want to fix {selectedIssues.Count} registry issues?", "Confirm Fix", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            int fixedCount = RegistryOptimizerEngine.FixRegistryIssues(selectedIssues, _chkBackup.Checked);
            MessageBox.Show($"Successfully repaired {fixedCount} registry issues!", "Repair Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            StartScan();
        }
    }
}

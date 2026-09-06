/*
    EBUninstaller Pro - Modern Statistics & Quick Actions Dashboard Bar
*/

using System;
using System.Drawing;
using System.Windows.Forms;
using BulkCrapUninstaller.Functions;

namespace BulkCrapUninstaller.Controls
{
    public sealed class ModernStatsDashboard : UserControl
    {
        private Label _lblTotalApps;
        private Label _lblTotalSize;
        private Label _lblSelectedInfo;
        private Label _lblHygieneBadge;

        private Button _btnQuickUninstall;
        private Button _btnScanLeftovers;
        private Button _btnCleanJunk;

        public event EventHandler RequestBatchUninstall;
        public event EventHandler RequestScanLeftovers;
        public event EventHandler RequestCleanJunk;
        public event EventHandler RequestHygieneAdvisor;

        public ModernStatsDashboard()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Dock = DockStyle.Top;
            Height = 44;
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            Padding = new Padding(8, 4, 8, 4);

            var mainTable = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 5,
                RowCount = 1
            };
            mainTable.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // Total Apps / Size
            mainTable.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // Selected Info
            mainTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100)); // Spacer
            mainTable.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // Hygiene Badge
            mainTable.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // Quick Action Buttons

            // 1. Total Apps & Disk Space
            _lblTotalApps = new Label
            {
                Text = "Discovered: 0 apps (0.0 GB)",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                Margin = new Padding(0, 0, 15, 0)
            };
            mainTable.Controls.Add(_lblTotalApps, 0, 0);

            // 2. Selected Items Info
            _lblSelectedInfo = new Label
            {
                Text = "Selected: 0 apps (0 MB)",
                ForeColor = Color.DarkSlateBlue,
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                Margin = new Padding(0, 0, 15, 0)
            };
            mainTable.Controls.Add(_lblSelectedInfo, 1, 0);

            // 3. Hygiene Score Badge Button
            _lblHygieneBadge = new Label
            {
                Text = "Hygiene: 100/100 (Optimal)",
                ForeColor = Color.ForestGreen,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                AutoSize = true,
                Anchor = AnchorStyles.Right,
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 15, 0)
            };
            _lblHygieneBadge.Click += (s, e) => RequestHygieneAdvisor?.Invoke(this, EventArgs.Empty);
            mainTable.Controls.Add(_lblHygieneBadge, 3, 0);

            // 4. Quick Action Buttons
            var btnFlow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                AutoSize = true,
                WrapContents = false
            };

            _btnCleanJunk = new Button { Text = "🧹 Junk Cleaner", AutoSize = true, Height = 28, FlatStyle = FlatStyle.Flat, Margin = new Padding(2, 0, 2, 0) };
            _btnCleanJunk.Click += (s, e) => RequestCleanJunk?.Invoke(this, EventArgs.Empty);

            _btnScanLeftovers = new Button { Text = "🔍 Scan Leftovers", AutoSize = true, Height = 28, FlatStyle = FlatStyle.Flat, Margin = new Padding(2, 0, 2, 0) };
            _btnScanLeftovers.Click += (s, e) => RequestScanLeftovers?.Invoke(this, EventArgs.Empty);

            _btnQuickUninstall = new Button { Text = "⚡ Uninstall Selected", AutoSize = true, Height = 28, FlatStyle = FlatStyle.Flat, Font = new Font(Font, FontStyle.Bold), Margin = new Padding(2, 0, 2, 0) };
            _btnQuickUninstall.Click += (s, e) => RequestBatchUninstall?.Invoke(this, EventArgs.Empty);

            btnFlow.Controls.Add(_btnCleanJunk);
            btnFlow.Controls.Add(_btnScanLeftovers);
            btnFlow.Controls.Add(_btnQuickUninstall);
            mainTable.Controls.Add(btnFlow, 4, 0);

            Controls.Add(mainTable);
            UpdateDashboard(0, 0, 0, 0, 100);
        }

        public void UpdateDashboard(int totalApps, long totalBytes, int selectedApps, long selectedBytes, int hygieneScore)
        {
            _lblTotalApps.Text = $"Discovered: {totalApps} applications ({(totalBytes / (1024 * 1024 * 1024.0)):F1} GB)";
            _lblSelectedInfo.Text = $"Selected: {selectedApps} ({(selectedBytes / (1024 * 1024.0)):F1} MB)";

            _lblHygieneBadge.Text = $"Hygiene: {hygieneScore}/100 ({(hygieneScore >= 85 ? "Optimal" : (hygieneScore >= 65 ? "Good" : "Needs Review"))})";
            _lblHygieneBadge.ForeColor = hygieneScore >= 85 ? Color.ForestGreen : (hygieneScore >= 65 ? Color.DarkOrange : Color.Crimson);

            _btnQuickUninstall.Enabled = selectedApps > 0;
            _btnScanLeftovers.Enabled = selectedApps > 0;
        }
    }
}

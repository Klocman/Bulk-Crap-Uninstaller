/*
    EBUninstaller Pro - Professional Next-Generation Windows Uninstaller
    Installation Monitor Window
*/

using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using BrightIdeasSoftware;
using UninstallTools.InstallationMonitor;

namespace BulkCrapUninstaller.Forms.Windows
{
    public sealed class InstallationMonitorWindow : Form
    {
        private TextBox _txtInstaller;
        private Button _btnBrowse;
        private Button _btnStartMonitor;
        private FastObjectListView _folvTraces;
        private FastObjectListView _folvTraceItems;
        private Button _btnRollback;
        private Button _btnDeleteTrace;
        private Button _btnExportTrace;
        private Button _btnClose;
        private Label _lblStatus;
        private ProgressBar _progressBar;

        public InstallationMonitorWindow()
        {
            InitializeComponent();
            RefreshTraces();
        }

        private void InitializeComponent()
        {
            Text = "EBUninstaller Pro - Installation Monitor & Snapshot Trace Center";
            Size = new Size(950, 620);
            MinimumSize = new Size(720, 480);
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 5,
                Padding = new Padding(12)
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Top Monitor Box
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Progress
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // Split Traces / Items
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Status
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Buttons

            // Top Installer Monitoring Box
            var monBox = new GroupBox
            {
                Text = "Live Installation Monitor",
                Dock = DockStyle.Fill,
                Padding = new Padding(8),
                Height = 65
            };
            var monLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3 };
            monLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            monLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            monLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            _txtInstaller = new TextBox { Dock = DockStyle.Fill, Margin = new Padding(3, 4, 3, 3) };
            _btnBrowse = new Button { Text = "Browse Setup...", AutoSize = true };
            _btnBrowse.Click += (s, e) =>
            {
                using var ofd = new OpenFileDialog { Filter = "Installers & Executables (*.exe;*.msi)|*.exe;*.msi|All Files (*.*)|*.*" };
                if (ofd.ShowDialog() == DialogResult.OK)
                    _txtInstaller.Text = ofd.FileName;
            };

            _btnStartMonitor = new Button
            {
                Text = "Start Monitored Install",
                AutoSize = true,
                Font = new Font(Font, FontStyle.Bold),
                ForeColor = Color.DarkGreen
            };
            _btnStartMonitor.Click += (s, e) => StartMonitoring();

            monLayout.Controls.Add(_txtInstaller, 0, 0);
            monLayout.Controls.Add(_btnBrowse, 1, 0);
            monLayout.Controls.Add(_btnStartMonitor, 2, 0);
            monBox.Controls.Add(monLayout);
            mainLayout.Controls.Add(monBox, 0, 0);

            // Progress bar
            _progressBar = new ProgressBar { Dock = DockStyle.Fill, Height = 10, Style = ProgressBarStyle.Marquee, Visible = false, Margin = new Padding(0, 4, 0, 4) };
            mainLayout.Controls.Add(_progressBar, 0, 1);

            // Split Container for Traces and Items
            var split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 220
            };

            _folvTraces = new FastObjectListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                MultiSelect = false,
                ShowGroups = false,
                GridLines = true
            };
            var colDate = new OLVColumn("Recorded Date", nameof(InstallationTrace.MonitoringStartedAt)) { Width = 150, AspectToStringConverter = v => ((DateTime)v).ToLocalTime().ToString("yyyy-MM-dd HH:mm") };
            var colName = new OLVColumn("Application Name", nameof(InstallationTrace.ApplicationName)) { Width = 250 };
            var colExe = new OLVColumn("Installer Path", nameof(InstallationTrace.InstallerExecutablePath)) { Width = 300, FillsFreeSpace = true };
            var colChanges = new OLVColumn("Total Changes", nameof(InstallationTrace.TotalChangesCount)) { Width = 110 };

            _folvTraces.AllColumns.AddRange(new[] { colDate, colName, colExe, colChanges });
            _folvTraces.RebuildColumns();
            _folvTraces.SelectionChanged += (s, e) => OnTraceSelected();
            split.Panel1.Controls.Add(_folvTraces);

            _folvTraceItems = new FastObjectListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                ShowGroups = true,
                GridLines = true
            };
            var colType = new OLVColumn("Category", nameof(TraceItem.Category)) { Width = 100 };
            var colChange = new OLVColumn("Action", nameof(TraceItem.ChangeType)) { Width = 80 };
            var colPath = new OLVColumn("Path / Registry Identifier", nameof(TraceItem.PathOrIdentifier)) { Width = 400, FillsFreeSpace = true };
            var colVal = new OLVColumn("Value Name", nameof(TraceItem.ValueName)) { Width = 140 };

            _folvTraceItems.AllColumns.AddRange(new[] { colType, colChange, colPath, colVal });
            _folvTraceItems.RebuildColumns();
            split.Panel2.Controls.Add(_folvTraceItems);

            mainLayout.Controls.Add(split, 0, 2);

            // Status label
            _lblStatus = new Label { Text = "Select a recorded trace or start monitoring a new installation.", AutoSize = true, Margin = new Padding(0, 4, 0, 4) };
            mainLayout.Controls.Add(_lblStatus, 0, 3);

            // Action Buttons
            var btnPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                AutoSize = true
            };

            _btnClose = new Button { Text = "Close", DialogResult = DialogResult.Cancel, AutoSize = true };
            _btnDeleteTrace = new Button { Text = "Delete Trace", AutoSize = true, Enabled = false, ForeColor = Color.DarkRed };
            _btnDeleteTrace.Click += (s, e) => DeleteSelectedTrace();

            _btnExportTrace = new Button { Text = "Export Trace...", AutoSize = true, Enabled = false };
            _btnExportTrace.Click += (s, e) => ExportSelectedTrace();

            _btnRollback = new Button { Text = "Replay Trace (Clean Removal)", AutoSize = true, Enabled = false, Font = new Font(Font, FontStyle.Bold) };
            _btnRollback.Click += (s, e) => RollbackSelectedTrace();

            btnPanel.Controls.Add(_btnClose);
            btnPanel.Controls.Add(_btnDeleteTrace);
            btnPanel.Controls.Add(_btnExportTrace);
            btnPanel.Controls.Add(_btnRollback);
            mainLayout.Controls.Add(btnPanel, 0, 4);

            Controls.Add(mainLayout);
        }

        private void RefreshTraces()
        {
            var traces = InstallationMonitorEngine.ListTraces();
            _folvTraces.SetObjects(traces);
            _folvTraceItems.SetObjects(Array.Empty<TraceItem>());
            _lblStatus.Text = $"Found {traces.Count} recorded installation traces.";
            UpdateButtons(false);
        }

        private async void StartMonitoring()
        {
            var exe = _txtInstaller.Text?.Trim();
            if (string.IsNullOrWhiteSpace(exe) || !File.Exists(exe))
            {
                MessageBox.Show("Please select a valid installer executable (.exe or .msi).", "Invalid Installer", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _btnStartMonitor.Enabled = false;
            _progressBar.Visible = true;
            _lblStatus.Text = "Taking pre-installation system snapshot and launching setup...";

            try
            {
                var trace = await InstallationMonitorEngine.MonitorInstallerAsync(exe);
                MessageBox.Show($"Installation monitoring completed!\n\nRecorded {trace.Items.Count} total system changes.",
                    "Monitoring Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshTraces();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Monitoring failed: {ex.Message}", "Monitoring Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _progressBar.Visible = false;
                _btnStartMonitor.Enabled = true;
            }
        }

        private void OnTraceSelected()
        {
            var selected = _folvTraces.SelectedObject as InstallationTrace;
            if (selected == null)
            {
                _folvTraceItems.SetObjects(Array.Empty<TraceItem>());
                UpdateButtons(false);
                return;
            }

            UpdateButtons(true);
            _folvTraceItems.SetObjects(selected.Items);
            _lblStatus.Text = $"Trace '{selected.ApplicationName}': {selected.Items.Count} recorded changes.";
        }

        private void UpdateButtons(bool hasSelection)
        {
            _btnRollback.Enabled = hasSelection;
            _btnDeleteTrace.Enabled = hasSelection;
            _btnExportTrace.Enabled = hasSelection;
        }

        private void RollbackSelectedTrace()
        {
            var selected = _folvTraces.SelectedObject as InstallationTrace;
            if (selected == null) return;

            if (MessageBox.Show($"Rollback and remove all {selected.Items.Count} items recorded during installation of '{selected.ApplicationName}'?",
                "Confirm Rollback", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            var success = InstallationMonitorEngine.RollbackTrace(selected, out var removed, out var errors);
            if (success)
            {
                MessageBox.Show($"Trace rollback completed successfully! ({removed.Count} items removed)",
                    "Rollback Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show($"Rollback completed with errors:\n{string.Join("\n", errors)}",
                    "Rollback Notice", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void DeleteSelectedTrace()
        {
            var selected = _folvTraces.SelectedObject as InstallationTrace;
            if (selected == null) return;

            if (MessageBox.Show($"Delete trace file for '{selected.ApplicationName}'?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            InstallationMonitorEngine.DeleteTrace(selected.TraceId);
            RefreshTraces();
        }

        private void ExportSelectedTrace()
        {
            var selected = _folvTraces.SelectedObject as InstallationTrace;
            if (selected == null) return;

            using var sfd = new SaveFileDialog
            {
                FileName = $"Trace_{selected.ApplicationName}_{selected.TraceId.Substring(0, 8)}.trace",
                Filter = "Trace File (*.trace)|*.trace|JSON File (*.json)|*.json"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                InstallationMonitorEngine.SaveTrace(selected, Path.GetDirectoryName(sfd.FileName));
                MessageBox.Show("Trace exported successfully.", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}

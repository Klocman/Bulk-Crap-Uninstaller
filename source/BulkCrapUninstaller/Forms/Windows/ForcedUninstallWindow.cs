/*
    OpenUninstall Pro - Professional Next-Generation Windows Uninstaller
    Forced Removal Window
*/

using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using BrightIdeasSoftware;
using UninstallTools.ForcedRemoval;

namespace BulkCrapUninstaller.Forms.Windows
{
    public sealed class ForcedUninstallWindow : Form
    {
        private TextBox _txtTarget;
        private Button _btnBrowseFolder;
        private Button _btnBrowseFile;
        private Button _btnScan;
        private FastObjectListView _folvResults;
        private CheckBox _chkCreateBackup;
        private Button _btnExecute;
        private Button _btnClose;
        private Label _lblSummary;
        private ProgressBar _progressBar;
        private ForcedRemovalPlan _currentPlan;

        public ForcedUninstallWindow(string initialPath = null)
        {
            InitializeComponent();
            if (!string.IsNullOrWhiteSpace(initialPath))
            {
                _txtTarget.Text = initialPath;
                StartScan();
            }
        }

        private void InitializeComponent()
        {
            Text = "OpenUninstall Pro - Forced Application Removal";
            Size = new Size(850, 580);
            MinimumSize = new Size(650, 450);
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 5,
                Padding = new Padding(12)
            };
            mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Top input
            mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Progress
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // List
            mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Summary & Options
            mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Buttons

            // 1. Target Selector Panel
            var targetBox = new GroupBox
            {
                Text = "Target Corrupted / Broken Application or Folder",
                Dock = DockStyle.Fill,
                Padding = new Padding(8),
                Height = 65
            };
            var targetLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4
            };
            targetLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            targetLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            targetLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            targetLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            _txtTarget = new TextBox { Dock = DockStyle.Fill, Margin = new Padding(3, 4, 3, 3) };
            _btnBrowseFolder = new Button { Text = "Browse Folder...", AutoSize = true };
            _btnBrowseFolder.Click += (s, e) =>
            {
                using var fbd = new FolderBrowserDialog { Description = "Select installation folder to remove" };
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    _txtTarget.Text = fbd.SelectedPath;
                    StartScan();
                }
            };

            _btnBrowseFile = new Button { Text = "Browse Exe...", AutoSize = true };
            _btnBrowseFile.Click += (s, e) =>
            {
                using var ofd = new OpenFileDialog { Filter = "Executable Files (*.exe)|*.exe|All Files (*.*)|*.*" };
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    _txtTarget.Text = ofd.FileName;
                    StartScan();
                }
            };

            _btnScan = new Button { Text = "Deep Scan Leftovers", AutoSize = true, Font = new Font(Font, FontStyle.Bold) };
            _btnScan.Click += (s, e) => StartScan();

            targetLayout.Controls.Add(_txtTarget, 0, 0);
            targetLayout.Controls.Add(_btnBrowseFolder, 1, 0);
            targetLayout.Controls.Add(_btnBrowseFile, 2, 0);
            targetLayout.Controls.Add(_btnScan, 3, 0);
            targetBox.Controls.Add(targetLayout);
            mainPanel.Controls.Add(targetBox, 0, 0);

            // 2. Progress Bar
            _progressBar = new ProgressBar { Dock = DockStyle.Fill, Height = 12, Style = ProgressBarStyle.Marquee, Visible = false, Margin = new Padding(0, 4, 0, 4) };
            mainPanel.Controls.Add(_progressBar, 0, 1);

            // 3. Results ObjectListView
            _folvResults = new FastObjectListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                CheckBoxes = true,
                ShowGroups = true,
                GridLines = true
            };

            var colConfidence = new OLVColumn("Confidence", nameof(ForcedRemovalItem.Confidence)) { Width = 90 };
            var colType = new OLVColumn("Type", nameof(ForcedRemovalItem.ItemType)) { Width = 100 };
            var colPath = new OLVColumn("Path / Registry Key", nameof(ForcedRemovalItem.PathOrKey)) { Width = 380, FillsFreeSpace = true };
            var colReason = new OLVColumn("Match Reason", nameof(ForcedRemovalItem.MatchReason)) { Width = 200 };

            _folvResults.AllColumns.AddRange(new[] { colConfidence, colType, colPath, colReason });
            _folvResults.RebuildColumns();
            _folvResults.CheckStateGetter = row => ((ForcedRemovalItem)row).IsSelected ? CheckState.Checked : CheckState.Unchecked;
            _folvResults.CheckStatePutter = (row, state) =>
            {
                ((ForcedRemovalItem)row).IsSelected = state == CheckState.Checked;
                UpdateSummary();
                return state;
            };

            mainPanel.Controls.Add(_folvResults, 0, 2);

            // 4. Summary & Options
            var summaryLayout = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true };
            _lblSummary = new Label { Text = "Enter an application name or directory to scan.", AutoSize = true, Margin = new Padding(0, 4, 16, 4) };
            _chkCreateBackup = new CheckBox { Text = "Create pre-removal backup & restore point", Checked = true, AutoSize = true, Margin = new Padding(0, 4, 0, 4) };
            summaryLayout.Controls.Add(_lblSummary);
            summaryLayout.Controls.Add(_chkCreateBackup);
            mainPanel.Controls.Add(summaryLayout, 0, 3);

            // 5. Action Buttons
            var btnPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                AutoSize = true
            };
            _btnClose = new Button { Text = "Close", DialogResult = DialogResult.Cancel, AutoSize = true };
            _btnExecute = new Button
            {
                Text = "Permanently Remove Selected",
                Enabled = false,
                AutoSize = true,
                ForeColor = Color.DarkRed,
                Font = new Font(Font, FontStyle.Bold)
            };
            _btnExecute.Click += (s, e) => ExecuteRemoval();

            btnPanel.Controls.Add(_btnClose);
            btnPanel.Controls.Add(_btnExecute);
            mainPanel.Controls.Add(btnPanel, 0, 4);

            Controls.Add(mainPanel);
        }

        private async void StartScan()
        {
            var target = _txtTarget.Text?.Trim();
            if (string.IsNullOrWhiteSpace(target))
            {
                MessageBox.Show("Please enter an application name or choose a folder to scan.", "Target Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _btnScan.Enabled = false;
            _btnExecute.Enabled = false;
            _progressBar.Visible = true;
            _lblSummary.Text = "Scanning registry and file system for leftovers...";

            try
            {
                _currentPlan = await Task.Run(() => ForcedUninstallManager.BuildPlan(target));
                _folvResults.SetObjects(_currentPlan.Items);
                UpdateSummary();
                _btnExecute.Enabled = _currentPlan.Items.Count > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Scan failed: {ex.Message}", "Scan Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _progressBar.Visible = false;
                _btnScan.Enabled = true;
            }
        }

        private void UpdateSummary()
        {
            if (_currentPlan == null) return;
            var selectedCount = _currentPlan.Items.Count(i => i.IsSelected);
            _lblSummary.Text = $"Found {_currentPlan.Items.Count} items ({selectedCount} selected for removal). High Confidence: {_currentPlan.HighConfidenceCount}, Medium: {_currentPlan.MediumConfidenceCount}, Low: {_currentPlan.LowConfidenceCount}";
        }

        private async void ExecuteRemoval()
        {
            if (_currentPlan == null) return;
            var selectedItems = _currentPlan.Items.Where(i => i.IsSelected).ToList();
            if (selectedItems.Count == 0)
            {
                MessageBox.Show("No items selected for removal.", "Selection Empty", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirmMsg = $"Are you sure you want to permanently remove {selectedItems.Count} items?\n\n" +
                             "This will delete associated files, folders, and registry keys.";
            if (MessageBox.Show(confirmMsg, "Confirm Forced Removal", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            _btnExecute.Enabled = false;
            _progressBar.Visible = true;
            _lblSummary.Text = "Executing removal plan and creating backup...";

            var result = await Task.Run(() => ForcedUninstallManager.ExecutePlan(_currentPlan, _chkCreateBackup.Checked));

            _progressBar.Visible = false;
            if (result.Success)
            {
                MessageBox.Show($"Forced removal completed successfully!\n\nRemoved: {result.RemovedItemsCount} items" +
                                (result.BackupId != null ? $"\nBackup ID: {result.BackupId}" : ""),
                    "Removal Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show($"Removal finished with errors.\nRemoved: {result.RemovedItemsCount}\nFailed: {result.FailedItemsCount}",
                    "Partial Removal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                StartScan(); // Refresh
            }
        }
    }
}

/*
    EBUninstaller Pro - Professional Next-Generation Windows Uninstaller
    Junk Cleaner Window
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using BrightIdeasSoftware;
using UninstallTools.JunkCleaner;

namespace BulkCrapUninstaller.Forms.Windows
{
    public sealed class JunkCleanerWindow : Form
    {
        private FastObjectListView _folvCategories;
        private FastObjectListView _folvItems;
        private Button _btnScan;
        private Button _btnClean;
        private Button _btnClose;
        private Label _lblStatus;
        private ProgressBar _progressBar;
        private List<JunkCategory> _categories = new();

        public JunkCleanerWindow()
        {
            InitializeComponent();
            StartScan();
        }

        private void InitializeComponent()
        {
            Text = "EBUninstaller Pro - System Junk Cleaner";
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
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // Split
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Progress
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Status
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Buttons

            // Split Container for Categories and File Items
            var split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                SplitterDistance = 380
            };

            _folvCategories = new FastObjectListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                CheckBoxes = true,
                ShowGroups = false,
                GridLines = true
            };
            var colCatName = new OLVColumn("Category", nameof(JunkCategory.Name)) { Width = 200, FillsFreeSpace = true };
            var colCount = new OLVColumn("Items", nameof(JunkCategory.ItemCount)) { Width = 70 };
            var colSize = new OLVColumn("Size", nameof(JunkCategory.TotalSizeBytes)) { Width = 80, AspectToStringConverter = v => $"{(long)v / (1024.0 * 1024.0):F1} MB" };

            _folvCategories.AllColumns.AddRange(new[] { colCatName, colCount, colSize });
            _folvCategories.RebuildColumns();
            _folvCategories.CheckStateGetter = row => ((JunkCategory)row).IsEnabled ? CheckState.Checked : CheckState.Unchecked;
            _folvCategories.CheckStatePutter = (row, state) =>
            {
                ((JunkCategory)row).IsEnabled = state == CheckState.Checked;
                UpdateSummary();
                return state;
            };
            _folvCategories.SelectionChanged += (s, e) => OnCategorySelected();
            split.Panel1.Controls.Add(_folvCategories);

            _folvItems = new FastObjectListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                ShowGroups = false,
                GridLines = true
            };
            var colFilePath = new OLVColumn("File Path", nameof(JunkFileItem.FilePath)) { Width = 380, FillsFreeSpace = true };
            var colFileSize = new OLVColumn("Size", nameof(JunkFileItem.Size)) { Width = 80, AspectToStringConverter = v => $"{(long)v / 1024.0:F1} KB" };
            var colModified = new OLVColumn("Modified", nameof(JunkFileItem.LastModified)) { Width = 130, AspectToStringConverter = v => ((DateTime)v).ToLocalTime().ToString("yyyy-MM-dd HH:mm") };

            _folvItems.AllColumns.AddRange(new[] { colFilePath, colFileSize, colModified });
            _folvItems.RebuildColumns();
            split.Panel2.Controls.Add(_folvItems);

            mainLayout.Controls.Add(split, 0, 0);

            // Progress bar
            _progressBar = new ProgressBar { Dock = DockStyle.Fill, Height = 10, Style = ProgressBarStyle.Marquee, Visible = false, Margin = new Padding(0, 4, 0, 4) };
            mainLayout.Controls.Add(_progressBar, 0, 1);

            // Status label
            _lblStatus = new Label { Text = "Ready to scan for junk files.", AutoSize = true, Margin = new Padding(0, 4, 0, 4) };
            mainLayout.Controls.Add(_lblStatus, 0, 2);

            // Action Buttons
            var btnPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                AutoSize = true
            };

            _btnClose = new Button { Text = "Close", DialogResult = DialogResult.Cancel, AutoSize = true };
            _btnClean = new Button { Text = "Clean Selected Junk", AutoSize = true, Enabled = false, Font = new Font(Font, FontStyle.Bold), ForeColor = Color.DarkGreen };
            _btnClean.Click += (s, e) => CleanJunk();

            _btnScan = new Button { Text = "Rescan", AutoSize = true };
            _btnScan.Click += (s, e) => StartScan();

            btnPanel.Controls.Add(_btnClose);
            btnPanel.Controls.Add(_btnClean);
            btnPanel.Controls.Add(_btnScan);
            mainLayout.Controls.Add(btnPanel, 0, 3);

            Controls.Add(mainLayout);
        }

        private async void StartScan()
        {
            _btnScan.Enabled = false;
            _btnClean.Enabled = false;
            _progressBar.Visible = true;
            _lblStatus.Text = "Scanning temporary folders, logs, caches, and recycle bin...";

            try
            {
                _categories = await JunkCleanerEngine.ScanJunkAsync(null, msg =>
                {
                    if (InvokeRequired) Invoke(new Action(() => _lblStatus.Text = msg));
                    else _lblStatus.Text = msg;
                });

                _folvCategories.SetObjects(_categories);
                UpdateSummary();
                _btnClean.Enabled = _categories.Any(c => c.ItemCount > 0);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Junk scan failed: {ex.Message}", "Scan Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _progressBar.Visible = false;
                _btnScan.Enabled = true;
            }
        }

        private void OnCategorySelected()
        {
            var selected = _folvCategories.SelectedObject as JunkCategory;
            if (selected != null)
            {
                _folvItems.SetObjects(selected.Items);
            }
            else
            {
                _folvItems.SetObjects(Array.Empty<JunkFileItem>());
            }
        }

        private void UpdateSummary()
        {
            var enabledCats = _categories.Where(c => c.IsEnabled).ToList();
            var totalFiles = enabledCats.Sum(c => c.ItemCount);
            var totalMB = enabledCats.Sum(c => c.TotalSizeBytes) / (1024.0 * 1024.0);
            _lblStatus.Text = $"Total Selected Junk: {totalFiles} items ({totalMB:F2} MB to free).";
        }

        private async void CleanJunk()
        {
            var enabledCats = _categories.Where(c => c.IsEnabled && c.ItemCount > 0).ToList();
            if (enabledCats.Count == 0) return;

            var totalMB = enabledCats.Sum(c => c.TotalSizeBytes) / (1024.0 * 1024.0);
            if (MessageBox.Show($"Clean selected junk files ({totalMB:F2} MB)?\n\nThis will remove temporary caches, log files, and empty the recycle bin.",
                "Confirm Junk Cleanup", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            _btnClean.Enabled = false;
            _btnScan.Enabled = false;
            _progressBar.Visible = true;
            _lblStatus.Text = "Cleaning junk files...";

            var result = await JunkCleanerEngine.CleanJunkAsync(enabledCats);

            _progressBar.Visible = false;
            _btnScan.Enabled = true;

            var skippedMsg = result.FailedCount > 0 ? "\nLocked/In-use files skipped: " + result.FailedCount : "";
            MessageBox.Show($"Junk cleanup finished!\n\nDeleted: {result.DeletedFilesCount} files\nFreed: {result.BytesFreed / (1024.0 * 1024.0):F2} MB" + skippedMsg,
                "Cleanup Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);

            StartScan();
        }
    }
}

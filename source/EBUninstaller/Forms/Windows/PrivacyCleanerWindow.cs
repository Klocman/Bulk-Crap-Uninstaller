/*
    EBUninstaller Pro - Professional Next-Generation Windows Uninstaller
    Privacy Cleaner Window
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using BrightIdeasSoftware;
using UninstallTools.PrivacyCleaner;

namespace BulkCrapUninstaller.Forms.Windows
{
    public sealed class PrivacyCleanerWindow : Form
    {
        private FastObjectListView _folvCategories;
        private FastObjectListView _folvItems;
        private Button _btnScan;
        private Button _btnClean;
        private Button _btnClose;
        private Label _lblStatus;
        private Label _lblWarning;
        private ProgressBar _progressBar;
        private List<PrivacyCategory> _categories = new();

        public PrivacyCleanerWindow()
        {
            InitializeComponent();
            StartScan();
        }

        private void InitializeComponent()
        {
            Text = "EBUninstaller Pro - Privacy & Browser Cleaner";
            Size = new Size(950, 600);
            MinimumSize = new Size(700, 480);
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 5,
                Padding = new Padding(12)
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // Split
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Warning banner
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Progress
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Status
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Buttons

            // Split Container
            var split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                SplitterDistance = 420
            };

            _folvCategories = new FastObjectListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                CheckBoxes = true,
                ShowGroups = true,
                GridLines = true
            };
            var colGroup = new OLVColumn("Group", nameof(PrivacyCategory.GroupName)) { Width = 140 };
            var colItem = new OLVColumn("Item", nameof(PrivacyCategory.ItemName)) { Width = 180, FillsFreeSpace = true };
            var colCount = new OLVColumn("Records", nameof(PrivacyCategory.ItemCount)) { Width = 70 };

            _folvCategories.AllColumns.AddRange(new[] { colGroup, colItem, colCount });
            _folvCategories.RebuildColumns();
            _folvCategories.CheckStateGetter = row => ((PrivacyCategory)row).IsSelected ? CheckState.Checked : CheckState.Unchecked;
            _folvCategories.CheckStatePutter = (row, state) =>
            {
                ((PrivacyCategory)row).IsSelected = state == CheckState.Checked;
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
            var colTarget = new OLVColumn("Target Location / Key", nameof(PrivacyItem.TargetPathOrKey)) { Width = 340, FillsFreeSpace = true };
            var colDesc = new OLVColumn("Description", nameof(PrivacyItem.Description)) { Width = 180 };

            _folvItems.AllColumns.AddRange(new[] { colTarget, colDesc });
            _folvItems.RebuildColumns();
            split.Panel2.Controls.Add(_folvItems);

            mainLayout.Controls.Add(split, 0, 0);

            // Warning label
            _lblWarning = new Label
            {
                Dock = DockStyle.Fill,
                ForeColor = Color.DarkOrange,
                Font = new Font(Font, FontStyle.Bold),
                Text = "Note: Cleaning browser cookies will sign you out of active websites.",
                Margin = new Padding(0, 4, 0, 4)
            };
            mainLayout.Controls.Add(_lblWarning, 0, 1);

            // Progress bar
            _progressBar = new ProgressBar { Dock = DockStyle.Fill, Height = 10, Style = ProgressBarStyle.Marquee, Visible = false, Margin = new Padding(0, 4, 0, 4) };
            mainLayout.Controls.Add(_progressBar, 0, 2);

            // Status label
            _lblStatus = new Label { Text = "Ready to scan privacy tracks.", AutoSize = true, Margin = new Padding(0, 4, 0, 4) };
            mainLayout.Controls.Add(_lblStatus, 0, 3);

            // Action Buttons
            var btnPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                AutoSize = true
            };

            _btnClose = new Button { Text = "Close", DialogResult = DialogResult.Cancel, AutoSize = true };
            _btnClean = new Button { Text = "Clean Selected Tracks", AutoSize = true, Enabled = false, Font = new Font(Font, FontStyle.Bold), ForeColor = Color.DarkBlue };
            _btnClean.Click += (s, e) => CleanPrivacy();

            _btnScan = new Button { Text = "Rescan", AutoSize = true };
            _btnScan.Click += (s, e) => StartScan();

            btnPanel.Controls.Add(_btnClose);
            btnPanel.Controls.Add(_btnClean);
            btnPanel.Controls.Add(_btnScan);
            mainLayout.Controls.Add(btnPanel, 0, 4);

            Controls.Add(mainLayout);
        }

        private async void StartScan()
        {
            _btnScan.Enabled = false;
            _btnClean.Enabled = false;
            _progressBar.Visible = true;
            _lblStatus.Text = "Scanning browser histories, cookies, and Windows privacy items...";

            try
            {
                _categories = await PrivacyCleanerEngine.ScanPrivacyTracksAsync(msg =>
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
                MessageBox.Show($"Privacy scan failed: {ex.Message}", "Scan Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _progressBar.Visible = false;
                _btnScan.Enabled = true;
            }
        }

        private void OnCategorySelected()
        {
            var selected = _folvCategories.SelectedObject as PrivacyCategory;
            if (selected != null)
            {
                _folvItems.SetObjects(selected.Items);
                if (!string.IsNullOrEmpty(selected.Warning))
                    _lblWarning.Text = $"Warning: {selected.Warning}";
                else
                    _lblWarning.Text = selected.Description ?? "";
            }
            else
            {
                _folvItems.SetObjects(Array.Empty<PrivacyItem>());
                _lblWarning.Text = string.Empty;
            }
        }

        private void UpdateSummary()
        {
            var selectedCats = _categories.Where(c => c.IsSelected).ToList();
            var totalItems = selectedCats.Sum(c => c.ItemCount);
            _lblStatus.Text = $"Total Selected Tracks: {totalItems} items.";
        }

        private async void CleanPrivacy()
        {
            var selectedCats = _categories.Where(c => c.IsSelected && c.ItemCount > 0).ToList();
            if (selectedCats.Count == 0) return;

            var hasCookies = selectedCats.Any(c => c.ItemName.Contains("Cookie", StringComparison.OrdinalIgnoreCase));
            var warnMsg = hasCookies ? "\n\nWARNING: Cleaning cookies will log you out of your active website sessions." : "";

            if (MessageBox.Show($"Clean selected privacy tracks ({selectedCats.Sum(c => c.ItemCount)} items)?{warnMsg}",
                "Confirm Privacy Cleanup", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            _btnClean.Enabled = false;
            _btnScan.Enabled = false;
            _progressBar.Visible = true;
            _lblStatus.Text = "Cleaning privacy tracks...";

            var result = await PrivacyCleanerEngine.CleanPrivacyTracksAsync(selectedCats);

            _progressBar.Visible = false;
            _btnScan.Enabled = true;

            var skippedMsg = result.FailedCount > 0 ? "\nLocked/In-use tracks skipped: " + result.FailedCount : "";
            MessageBox.Show($"Privacy cleanup finished!\n\nCleaned: {result.CleanedItemsCount} items" + skippedMsg,
                "Cleanup Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);

            StartScan();
        }
    }
}

/*
    EBUninstaller Pro - Professional Next-Generation Windows Uninstaller
    Secure File & Folder Deletion Window
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using BrightIdeasSoftware;
using UninstallTools.FileSystemEngine;

namespace BulkCrapUninstaller.Forms.Windows
{
    public sealed class SecureDeleteWindow : Form
    {
        private FastObjectListView _folvTargets;
        private ComboBox _cmbMethod;
        private Button _btnAddFiles;
        private Button _btnAddFolder;
        private Button _btnClearList;
        private Button _btnDelete;
        private Button _btnClose;
        private Label _lblDisclaimer;
        private Label _lblStatus;
        private ProgressBar _progressBar;
        private readonly List<string> _targetPaths = new();

        public SecureDeleteWindow(string initialPath = null)
        {
            InitializeComponent();
            if (!string.IsNullOrWhiteSpace(initialPath))
            {
                AddTarget(initialPath);
            }
        }

        private void InitializeComponent()
        {
            Text = "EBUninstaller Pro - Secure File & Folder Shredder";
            Size = new Size(850, 560);
            MinimumSize = new Size(650, 420);
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 6,
                Padding = new Padding(12)
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Top Action Buttons
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // Target List
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Method selector
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // SSD Disclaimer
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Progress
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Buttons

            // Top Buttons
            var topPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, Margin = new Padding(0, 0, 0, 8) };
            _btnAddFiles = new Button { Text = "Add Files...", AutoSize = true };
            _btnAddFiles.Click += (s, e) =>
            {
                using var ofd = new OpenFileDialog { Multiselect = true, Filter = "All Files (*.*)|*.*" };
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    foreach (var f in ofd.FileNames) AddTarget(f);
                }
            };

            _btnAddFolder = new Button { Text = "Add Folder...", AutoSize = true };
            _btnAddFolder.Click += (s, e) =>
            {
                using var fbd = new FolderBrowserDialog { Description = "Select folder to securely shred" };
                if (fbd.ShowDialog() == DialogResult.OK) AddTarget(fbd.SelectedPath);
            };

            _btnClearList = new Button { Text = "Clear List", AutoSize = true };
            _btnClearList.Click += (s, e) =>
            {
                _targetPaths.Clear();
                RefreshList();
            };

            topPanel.Controls.Add(_btnAddFiles);
            topPanel.Controls.Add(_btnAddFolder);
            topPanel.Controls.Add(_btnClearList);
            mainLayout.Controls.Add(topPanel, 0, 0);

            // Targets ObjectListView
            _folvTargets = new FastObjectListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                ShowGroups = false,
                GridLines = true
            };
            var colPath = new OLVColumn("Target Path", "ToString") { Width = 550, FillsFreeSpace = true };
            var colType = new OLVColumn("Type", "") { Width = 100, AspectGetter = row => Directory.Exists((string)row) ? "Folder" : "File" };
            var colSize = new OLVColumn("Size", "")
            {
                Width = 100,
                AspectGetter = row =>
                {
                    var p = (string)row;
                    if (File.Exists(p)) return $"{new FileInfo(p).Length / 1024.0:F1} KB";
                    if (Directory.Exists(p)) return $"{SafeFileSystemEngine.GetDirectorySize(p, out _, out _) / (1024.0 * 1024.0):F2} MB";
                    return "N/A";
                }
            };

            _folvTargets.AllColumns.AddRange(new[] { colPath, colType, colSize });
            _folvTargets.RebuildColumns();
            mainLayout.Controls.Add(_folvTargets, 0, 1);

            // Method Selector Layout
            var methodLayout = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, Margin = new Padding(0, 4, 0, 4) };
            var lblMethod = new Label { Text = "Sanitization Method:", AutoSize = true, Margin = new Padding(0, 6, 8, 0) };
            _cmbMethod = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 300 };
            _cmbMethod.Items.AddRange(new object[]
            {
                "Send to Recycle Bin (Recoverable)",
                "Permanent Delete (Standard filesystem unlinking)",
                "Zero-Fill Shred (1-Pass Zero overwrite + Unlink)",
                "DoD 5220.22-M Multi-Pass Shred (3-Pass Zero/Random + Unlink)"
            });
            _cmbMethod.SelectedIndex = 2; // Default to Zero-Fill
            methodLayout.Controls.Add(lblMethod);
            methodLayout.Controls.Add(_cmbMethod);
            mainLayout.Controls.Add(methodLayout, 0, 2);

            // Transparent SSD Limitation Banner
            _lblDisclaimer = new Label
            {
                Dock = DockStyle.Fill,
                ForeColor = Color.DarkSlateGray,
                Font = new Font("Segoe UI", 8.25F, FontStyle.Italic),
                Text = "SSD & Flash Disclaimer: On modern Solid-State Drives with wear-leveling and TRIM, hardware flash controllers manage sector reallocation. Data blocks cannot be guaranteed 100% physically erased until TRIM and garbage collection cycle.",
                Margin = new Padding(0, 4, 0, 4)
            };
            mainLayout.Controls.Add(_lblDisclaimer, 0, 3);

            // Progress bar
            _progressBar = new ProgressBar { Dock = DockStyle.Fill, Height = 10, Style = ProgressBarStyle.Marquee, Visible = false, Margin = new Padding(0, 4, 0, 4) };
            mainLayout.Controls.Add(_progressBar, 0, 4);

            // Status & Buttons
            var bottomLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, AutoSize = true };
            bottomLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            bottomLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            _lblStatus = new Label { Text = "Add files or folders to shred.", AutoSize = true, Margin = new Padding(0, 8, 0, 0) };

            var btnPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft, AutoSize = true };
            _btnClose = new Button { Text = "Close", DialogResult = DialogResult.Cancel, AutoSize = true };
            _btnDelete = new Button { Text = "Permanently Shred", AutoSize = true, Enabled = false, ForeColor = Color.DarkRed, Font = new Font(Font, FontStyle.Bold) };
            _btnDelete.Click += (s, e) => ExecuteShred();

            btnPanel.Controls.Add(_btnClose);
            btnPanel.Controls.Add(_btnDelete);

            bottomLayout.Controls.Add(_lblStatus, 0, 0);
            bottomLayout.Controls.Add(btnPanel, 1, 0);
            mainLayout.Controls.Add(bottomLayout, 0, 5);

            Controls.Add(mainLayout);
        }

        private void AddTarget(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return;
            if (!_targetPaths.Contains(path, StringComparer.OrdinalIgnoreCase))
            {
                _targetPaths.Add(path);
                RefreshList();
            }
        }

        private void RefreshList()
        {
            _folvTargets.SetObjects(_targetPaths);
            _lblStatus.Text = $"{_targetPaths.Count} items queued for deletion.";
            _btnDelete.Enabled = _targetPaths.Count > 0;
        }

        private async void ExecuteShred()
        {
            if (_targetPaths.Count == 0) return;

            var mode = _cmbMethod.SelectedIndex switch
            {
                0 => DeletionMode.SendToRecycleBin,
                1 => DeletionMode.PermanentNormal,
                2 => DeletionMode.SecureZeroFill,
                3 => DeletionMode.SecureMultiPassDod,
                _ => DeletionMode.SecureZeroFill
            };

            if (MessageBox.Show($"Are you sure you want to permanently shred {_targetPaths.Count} items using {mode}?\n\nThis action cannot be undone.",
                "Confirm Permanent Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            _btnDelete.Enabled = false;
            _progressBar.Visible = true;
            _lblStatus.Text = "Shredding files and directories...";

            var targets = _targetPaths.ToList();
            var deleted = 0;
            var failed = 0;

            await Task.Run(() =>
            {
                foreach (var path in targets)
                {
                    var success = false;
                    if (File.Exists(path))
                        success = SafeFileSystemEngine.DeleteFileSafe(path, mode);
                    else if (Directory.Exists(path))
                        success = SafeFileSystemEngine.DeleteDirectorySafe(path, mode);

                    if (success) deleted++;
                    else failed++;
                }
            });

            _progressBar.Visible = false;
            MessageBox.Show($"Shredding complete!\n\nDeleted: {deleted}\nFailed: {failed}", "Shredding Report", MessageBoxButtons.OK, MessageBoxIcon.Information);

            _targetPaths.Clear();
            RefreshList();
        }
    }
}

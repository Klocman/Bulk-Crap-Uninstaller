/*
    EBUninstaller Pro - Duplicate & Empty Folder Cleaner Window
    Modern GUI for scanning and safely removing empty directories and duplicate files.
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using UninstallTools.FileSystemEngine;
using UninstallTools.Localization;

namespace BulkCrapUninstaller.Forms.Windows
{
    public class DuplicateAndEmptyFolderWindow : Form
    {
        private TabControl _tabControl = null!;
        private TabPage _emptyFoldersTab = null!;
        private TabPage _duplicateFilesTab = null!;

        // Empty Folders UI
        private ListView _emptyFolderListView = null!;
        private Button _scanEmptyBtn = null!;
        private Button _cleanEmptyBtn = null!;
        private Label _emptyStatusLabel = null!;
        private ProgressBar _emptyProgressBar = null!;
        private List<EmptyDirectoryItem> _emptyFolderItems = new();

        // Duplicate Files UI
        private ListView _duplicateListView = null!;
        private Button _scanDuplicatesBtn = null!;
        private Button _cleanDuplicatesBtn = null!;
        private Label _dupStatusLabel = null!;
        private ProgressBar _dupProgressBar = null!;
        private List<DuplicateFileGroup> _duplicateGroups = new();

        public DuplicateAndEmptyFolderWindow()
        {
            InitializeComponent();
            ApplyTheme();
        }

        private void InitializeComponent()
        {
            Text = LanguageManager.GetString("DuplicateAndEmptyCleaner_Title") ?? "Duplicate & Empty Folder Cleaner - EBUninstaller Pro";
            Size = new Size(950, 650);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(800, 500);

            _tabControl = new TabControl { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 9.5f) };
            _emptyFoldersTab = new TabPage { Text = "Empty Folders Cleaner", Padding = new Padding(10) };
            _duplicateFilesTab = new TabPage { Text = "Duplicate Files Scanner", Padding = new Padding(10) };

            SetupEmptyFoldersTab();
            SetupDuplicateFilesTab();

            _tabControl.TabPages.Add(_emptyFoldersTab);
            _tabControl.TabPages.Add(_duplicateFilesTab);

            Controls.Add(_tabControl);
        }

        private void SetupEmptyFoldersTab()
        {
            var topPanel = new Panel { Dock = DockStyle.Top, Height = 50 };
            _scanEmptyBtn = new Button
            {
                Text = "🔍 Scan Empty Folders",
                Size = new Size(180, 36),
                Location = new Point(5, 7),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold)
            };
            _scanEmptyBtn.Click += async (s, e) => await ScanEmptyFoldersAsync();

            _cleanEmptyBtn = new Button
            {
                Text = "🗑️ Clean Selected Folders",
                Size = new Size(190, 36),
                Location = new Point(195, 7),
                Enabled = false,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold)
            };
            _cleanEmptyBtn.Click += async (s, e) => await CleanEmptyFoldersAsync();

            _emptyStatusLabel = new Label
            {
                Text = "Click 'Scan Empty Folders' to discover orphaned empty directories across AppData and ProgramData.",
                AutoSize = true,
                Location = new Point(400, 16),
                Font = new Font("Segoe UI", 9.5f)
            };

            topPanel.Controls.Add(_scanEmptyBtn);
            topPanel.Controls.Add(_cleanEmptyBtn);
            topPanel.Controls.Add(_emptyStatusLabel);

            _emptyProgressBar = new ProgressBar { Dock = DockStyle.Bottom, Height = 14, Visible = false };

            _emptyFolderListView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                CheckBoxes = true,
                FullRowSelect = true,
                GridLines = true
            };
            _emptyFolderListView.Columns.Add("Directory Path", 550);
            _emptyFolderListView.Columns.Add("Parent Location", 220);
            _emptyFolderListView.Columns.Add("Last Modified", 130);

            _emptyFoldersTab.Controls.Add(_emptyFolderListView);
            _emptyFoldersTab.Controls.Add(_emptyProgressBar);
            _emptyFoldersTab.Controls.Add(topPanel);
        }

        private void SetupDuplicateFilesTab()
        {
            var topPanel = new Panel { Dock = DockStyle.Top, Height = 50 };
            _scanDuplicatesBtn = new Button
            {
                Text = "🔍 Scan Duplicate Files",
                Size = new Size(180, 36),
                Location = new Point(5, 7),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold)
            };
            _scanDuplicatesBtn.Click += async (s, e) => await ScanDuplicatesAsync();

            _cleanDuplicatesBtn = new Button
            {
                Text = "🗑️ Remove Selected Duplicates",
                Size = new Size(220, 36),
                Location = new Point(195, 7),
                Enabled = false,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold)
            };
            _cleanDuplicatesBtn.Click += async (s, e) => await CleanDuplicatesAsync();

            _dupStatusLabel = new Label
            {
                Text = "Scans Downloads and Documents for identical duplicate files via SHA-256 digests.",
                AutoSize = true,
                Location = new Point(430, 16),
                Font = new Font("Segoe UI", 9.5f)
            };

            topPanel.Controls.Add(_scanDuplicatesBtn);
            topPanel.Controls.Add(_cleanDuplicatesBtn);
            topPanel.Controls.Add(_dupStatusLabel);

            _dupProgressBar = new ProgressBar { Dock = DockStyle.Bottom, Height = 14, Visible = false };

            _duplicateListView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                CheckBoxes = true,
                FullRowSelect = true,
                GridLines = true
            };
            _duplicateListView.Columns.Add("File Name", 260);
            _duplicateListView.Columns.Add("Size", 100);
            _duplicateListView.Columns.Add("Status", 120);
            _duplicateListView.Columns.Add("Full Path", 420);

            _duplicateFilesTab.Controls.Add(_duplicateListView);
            _duplicateFilesTab.Controls.Add(_dupProgressBar);
            _duplicateFilesTab.Controls.Add(topPanel);
        }

        private async Task ScanEmptyFoldersAsync()
        {
            _scanEmptyBtn.Enabled = false;
            _cleanEmptyBtn.Enabled = false;
            _emptyProgressBar.Visible = true;
            _emptyProgressBar.Style = ProgressBarStyle.Marquee;
            _emptyFolderListView.Items.Clear();

            _emptyStatusLabel.Text = "Scanning for empty directories...";

            var items = await Task.Run(() => EmptyDirectoryCleaner.ScanForEmptyDirectories(null, msg =>
            {
                BeginInvoke(new Action(() => _emptyStatusLabel.Text = msg));
            }));

            _emptyFolderItems = items;
            foreach (var item in _emptyFolderItems)
            {
                var lvi = new ListViewItem(item.Path) { Checked = true, Tag = item };
                lvi.SubItems.Add(item.ParentFolder);
                lvi.SubItems.Add(item.LastModified.ToLocalTime().ToString("yyyy-MM-dd HH:mm"));
                _emptyFolderListView.Items.Add(lvi);
            }

            _emptyProgressBar.Visible = false;
            _scanEmptyBtn.Enabled = true;
            _cleanEmptyBtn.Enabled = _emptyFolderItems.Count > 0;
            _emptyStatusLabel.Text = $"Scan complete. Found {_emptyFolderItems.Count} empty directories.";
        }

        private async Task CleanEmptyFoldersAsync()
        {
            var selectedItems = _emptyFolderListView.CheckedItems.Cast<ListViewItem>()
                .Select(l => l.Tag as EmptyDirectoryItem)
                .Where(i => i != null)
                .Cast<EmptyDirectoryItem>()
                .ToList();

            if (selectedItems.Count == 0)
            {
                MessageBox.Show("Please select at least one empty folder to remove.", "EBUninstaller Pro", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show($"Are you sure you want to delete {selectedItems.Count} empty folder(s)?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            _cleanEmptyBtn.Enabled = false;
            _emptyProgressBar.Visible = true;
            _emptyProgressBar.Style = ProgressBarStyle.Marquee;

            int deleted = await Task.Run(() => EmptyDirectoryCleaner.DeleteEmptyDirectories(selectedItems, msg =>
            {
                BeginInvoke(new Action(() => _emptyStatusLabel.Text = msg));
            }));

            _emptyProgressBar.Visible = false;
            _emptyStatusLabel.Text = $"Successfully removed {deleted} empty directories.";
            await ScanEmptyFoldersAsync();
        }

        private async Task ScanDuplicatesAsync()
        {
            _scanDuplicatesBtn.Enabled = false;
            _cleanDuplicatesBtn.Enabled = false;
            _dupProgressBar.Visible = true;
            _dupProgressBar.Style = ProgressBarStyle.Marquee;
            _duplicateListView.Items.Clear();

            var scanPaths = new[]
            {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Documents")
            }.Where(Directory.Exists);

            _dupStatusLabel.Text = "Scanning duplicate files...";

            var groups = await Task.Run(() => DuplicateFileScanner.ScanForDuplicates(scanPaths, 1024, msg =>
            {
                BeginInvoke(new Action(() => _dupStatusLabel.Text = msg));
            }));

            _duplicateGroups = groups;
            long totalWasted = 0;

            foreach (var group in _duplicateGroups)
            {
                totalWasted += group.WastedSpaceBytes;
                foreach (var file in group.Files)
                {
                    var lvi = new ListViewItem(file.FileName)
                    {
                        Checked = file.IsSelectedForRemoval,
                        Tag = file
                    };
                    lvi.SubItems.Add(FormatSize(file.FileSizeBytes));
                    lvi.SubItems.Add(file.IsOriginal ? "Original" : "Duplicate Copy");
                    lvi.SubItems.Add(file.FilePath);

                    if (file.IsOriginal)
                    {
                        lvi.ForeColor = Color.FromArgb(0, 120, 60);
                    }
                    else
                    {
                        lvi.ForeColor = Color.FromArgb(160, 40, 40);
                    }

                    _duplicateListView.Items.Add(lvi);
                }
            }

            _dupProgressBar.Visible = false;
            _scanDuplicatesBtn.Enabled = true;
            _cleanDuplicatesBtn.Enabled = _duplicateListView.Items.Count > 0;
            _dupStatusLabel.Text = $"Found {_duplicateGroups.Count} duplicate groups ({FormatSize(totalWasted)} wasted space).";
        }

        private async Task CleanDuplicatesAsync()
        {
            var selectedFiles = _duplicateListView.CheckedItems.Cast<ListViewItem>()
                .Select(l => l.Tag as DuplicateFileItem)
                .Where(f => f != null && !f.IsOriginal)
                .Cast<DuplicateFileItem>()
                .ToList();

            if (selectedFiles.Count == 0)
            {
                MessageBox.Show("Please select at least one duplicate copy to delete.", "EBUninstaller Pro", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show($"Are you sure you want to permanently delete {selectedFiles.Count} duplicate file(s)?", "Confirm Duplicate Cleanup", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            _cleanDuplicatesBtn.Enabled = false;
            _dupProgressBar.Visible = true;
            _dupProgressBar.Style = ProgressBarStyle.Marquee;

            var (deleted, freed) = await Task.Run(() => DuplicateFileScanner.DeleteDuplicates(selectedFiles, msg =>
            {
                BeginInvoke(new Action(() => _dupStatusLabel.Text = msg));
            }));

            _dupProgressBar.Visible = false;
            _dupStatusLabel.Text = $"Removed {deleted} duplicates, freeing {FormatSize(freed)} space.";
            await ScanDuplicatesAsync();
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

            if (LanguageManager.IsRightToLeft)
            {
                RightToLeft = RightToLeft.Yes;
                RightToLeftLayout = true;
            }
        }
    }
}

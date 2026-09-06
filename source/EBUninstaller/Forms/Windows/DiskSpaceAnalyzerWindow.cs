/*
    EBUninstaller Pro - Disk Space & Large File Analyzer Window
    Modern GUI for analyzing storage usage, category distributions, and top largest files.
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using UninstallTools.FileSystemEngine;
using UninstallTools.Localization;

namespace BulkCrapUninstaller.Forms.Windows
{
    public class DiskSpaceAnalyzerWindow : Form
    {
        private ComboBox _driveComboBox = null!;
        private Button _browseBtn = null!;
        private Button _scanBtn = null!;
        private Button _openExplorerBtn = null!;
        private Button _deleteFileBtn = null!;
        private ListView _listView = null!;
        private Panel _summaryPanel = null!;
        private Label _totalSizeLabel = null!;
        private Label _breakdownLabel = null!;
        private StatusStrip _statusStrip = null!;
        private ToolStripStatusLabel _statusLabel = null!;

        private DiskSpaceReport _currentReport = new();

        public DiskSpaceAnalyzerWindow()
        {
            InitializeComponent();
            ApplyTheme();
            LoadDrives();
        }

        private void InitializeComponent()
        {
            Text = LanguageManager.GetString("DiskSpaceAnalyzer_Title") ?? "Disk Space & Large File Visualizer - EBUninstaller Pro";
            Size = new Size(1050, 650);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(850, 500);

            // Top Path / Drive Selector Panel
            var topPanel = new Panel { Dock = DockStyle.Top, Height = 55, Padding = new Padding(12, 10, 12, 10) };
            var driveLabel = new Label { Text = "Target Drive / Folder:", Location = new Point(12, 16), AutoSize = true, Font = new Font("Segoe UI", 9.5f, FontStyle.Bold) };
            _driveComboBox = new ComboBox { Location = new Point(165, 14), Width = 280, Font = new Font("Segoe UI", 9.5f), DropDownStyle = ComboBoxStyle.DropDown };
            _browseBtn = new Button { Text = "Browse...", Location = new Point(452, 12), Size = new Size(100, 30), Font = new Font("Segoe UI", 9f) };
            _scanBtn = new Button { Text = "🔍 Scan Storage", Location = new Point(558, 12), Size = new Size(140, 30), Font = new Font("Segoe UI", 9f, FontStyle.Bold) };
            _openExplorerBtn = new Button { Text = "📁 Show in Explorer", Location = new Point(704, 12), Size = new Size(140, 30), Enabled = false, Font = new Font("Segoe UI", 9f) };
            _deleteFileBtn = new Button { Text = "🗑️ Delete File", Location = new Point(850, 12), Size = new Size(120, 30), Enabled = false, Font = new Font("Segoe UI", 9f, FontStyle.Bold), ForeColor = Color.FromArgb(180, 40, 40) };

            _browseBtn.Click += (s, e) =>
            {
                using var fbd = new FolderBrowserDialog { Description = "Select Folder to Analyze" };
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    _driveComboBox.Text = fbd.SelectedPath;
                    _ = ScanStorageAsync();
                }
            };

            _scanBtn.Click += async (s, e) => await ScanStorageAsync();
            _openExplorerBtn.Click += (s, e) => OpenSelectedInExplorer();
            _deleteFileBtn.Click += async (s, e) => await DeleteSelectedFileAsync();

            topPanel.Controls.Add(driveLabel);
            topPanel.Controls.Add(_driveComboBox);
            topPanel.Controls.Add(_browseBtn);
            topPanel.Controls.Add(_scanBtn);
            topPanel.Controls.Add(_openExplorerBtn);
            topPanel.Controls.Add(_deleteFileBtn);

            // Summary Panel
            _summaryPanel = new Panel { Dock = DockStyle.Top, Height = 65, Padding = new Padding(15, 8, 15, 8) };
            _totalSizeLabel = new Label
            {
                Text = "Scanned: 0 GB across 0 files",
                Font = new Font("Segoe UI", 11.5f, FontStyle.Bold),
                Location = new Point(15, 10),
                AutoSize = true
            };
            _breakdownLabel = new Label
            {
                Text = "Category Breakdown: Apps: 0 MB | Videos/Media: 0 MB | Archives: 0 MB | Disk Images: 0 MB",
                Font = new Font("Segoe UI", 9.5f),
                Location = new Point(15, 36),
                AutoSize = true,
                ForeColor = Color.FromArgb(0, 100, 180)
            };
            _summaryPanel.Controls.Add(_totalSizeLabel);
            _summaryPanel.Controls.Add(_breakdownLabel);

            // ListView
            _listView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                MultiSelect = false
            };
            _listView.Columns.Add("Rank", 60);
            _listView.Columns.Add("File Name", 260);
            _listView.Columns.Add("Size", 110);
            _listView.Columns.Add("Category", 180);
            _listView.Columns.Add("Last Modified", 130);
            _listView.Columns.Add("Full Path", 400);

            _listView.SelectedIndexChanged += (s, e) =>
            {
                bool hasSel = _listView.SelectedItems.Count > 0;
                _openExplorerBtn.Enabled = hasSel;
                _deleteFileBtn.Enabled = hasSel;
            };

            // StatusStrip
            _statusStrip = new StatusStrip();
            _statusLabel = new ToolStripStatusLabel { Spring = true, TextAlign = ContentAlignment.MiddleLeft, Text = "Ready. Select drive or folder and click 'Scan Storage'." };
            _statusStrip.Items.Add(_statusLabel);

            Controls.Add(_listView);
            Controls.Add(_summaryPanel);
            Controls.Add(topPanel);
            Controls.Add(_statusStrip);
        }

        private void LoadDrives()
        {
            try
            {
                foreach (var drive in DriveInfo.GetDrives().Where(d => d.IsReady))
                {
                    _driveComboBox.Items.Add($"{drive.Name} ({drive.VolumeLabel}) - {FormatSize(drive.TotalSize)}");
                }
                if (_driveComboBox.Items.Count > 0)
                    _driveComboBox.SelectedIndex = 0;
            }
            catch
            {
                _driveComboBox.Text = @"C:\";
            }
        }

        private string GetSelectedPath()
        {
            string txt = _driveComboBox.Text.Trim();
            if (txt.Contains(" ("))
                return txt.Substring(0, txt.IndexOf(" (")).Trim();

            return txt;
        }

        private async Task ScanStorageAsync()
        {
            string path = GetSelectedPath();
            if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
            {
                MessageBox.Show("Please select a valid existing drive or directory.", "Invalid Path", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _scanBtn.Enabled = false;
            _statusLabel.Text = $"Scanning disk space on {path}...";
            _listView.Items.Clear();

            _currentReport = await Task.Run(() => DiskSpaceAnalyzer.AnalyzeDirectory(path, 100, msg =>
            {
                BeginInvoke(new Action(() => _statusLabel.Text = msg));
            }));

            _totalSizeLabel.Text = $"Total Scanned: {FormatSize(_currentReport.TotalScannedBytes)} across {_currentReport.TotalFilesCount:N0} files";

            long apps = _currentReport.CategorySizes.GetValueOrDefault(FileTypeCategory.ApplicationsAndExecutables, 0);
            long media = _currentReport.CategorySizes.GetValueOrDefault(FileTypeCategory.MediaAndVideos, 0);
            long archives = _currentReport.CategorySizes.GetValueOrDefault(FileTypeCategory.ArchivesAndZips, 0);
            long images = _currentReport.CategorySizes.GetValueOrDefault(FileTypeCategory.DiskImagesAndIsos, 0);

            _breakdownLabel.Text = $"Breakdown: Apps: {FormatSize(apps)} | Media: {FormatSize(media)} | Archives: {FormatSize(archives)} | Disk Images: {FormatSize(images)}";

            int rank = 1;
            foreach (var f in _currentReport.TopLargestFiles)
            {
                var lvi = new ListViewItem($"#{rank++}") { Tag = f };
                lvi.SubItems.Add(f.FileName);
                lvi.SubItems.Add(FormatSize(f.SizeBytes));
                lvi.SubItems.Add(FormatCategory(f.Category));
                lvi.SubItems.Add(f.LastModified.ToLocalTime().ToString("yyyy-MM-dd HH:mm"));
                lvi.SubItems.Add(f.FilePath);

                if (f.SizeBytes > 1024L * 1024 * 1024) // > 1 GB
                {
                    lvi.ForeColor = Color.FromArgb(190, 40, 40);
                }

                _listView.Items.Add(lvi);
            }

            _scanBtn.Enabled = true;
            _statusLabel.Text = $"Scan complete. Identified {_currentReport.TopLargestFiles.Count} largest files.";
        }

        private void OpenSelectedInExplorer()
        {
            if (_listView.SelectedItems.Count == 0) return;
            if (_listView.SelectedItems[0].Tag is LargeFileItem item)
            {
                try
                {
                    Process.Start(new ProcessStartInfo("explorer.exe", $"/select,\"{item.FilePath}\"") { UseShellExecute = true });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Could not open Explorer: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async Task DeleteSelectedFileAsync()
        {
            if (_listView.SelectedItems.Count == 0) return;
            if (_listView.SelectedItems[0].Tag is LargeFileItem item)
            {
                if (item.IsProtected)
                {
                    MessageBox.Show($"'{item.FileName}' is located in a protected Windows system directory and cannot be deleted.", "Protected System File", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (MessageBox.Show($"Are you sure you want to permanently delete '{item.FileName}' ({FormatSize(item.SizeBytes)})?", "Confirm File Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                    return;

                bool deleted = await Task.Run(() => DiskSpaceAnalyzer.DeleteLargeFile(item));
                if (deleted)
                {
                    MessageBox.Show("File deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await ScanStorageAsync();
                }
                else
                {
                    MessageBox.Show("Failed to delete file. It may be locked by another process.", "Deletion Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private static string FormatCategory(FileTypeCategory cat)
        {
            return cat switch
            {
                FileTypeCategory.ApplicationsAndExecutables => "Apps & Executables",
                FileTypeCategory.DiskImagesAndIsos => "Disk Images / ISO / VMs",
                FileTypeCategory.ArchivesAndZips => "Compressed Archives",
                FileTypeCategory.MediaAndVideos => "Media & Videos",
                FileTypeCategory.Documents => "Documents",
                FileTypeCategory.LogsAndDumps => "Logs & Memory Dumps",
                _ => "Other Files"
            };
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
            _summaryPanel.BackColor = isDark ? Color.FromArgb(40, 40, 40) : Color.FromArgb(235, 238, 245);

            if (LanguageManager.IsRightToLeft)
            {
                RightToLeft = RightToLeft.Yes;
                RightToLeftLayout = true;
            }
        }
    }
}

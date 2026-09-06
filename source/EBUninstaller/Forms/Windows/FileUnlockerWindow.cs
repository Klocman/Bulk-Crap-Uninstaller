/*
    EBUninstaller Pro - File & Folder Unlocker Window
    Modern GUI for detecting locking processes and unlocking stubborn files or folders.
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
    public class FileUnlockerWindow : Form
    {
        private TextBox _pathTextBox = null!;
        private Button _browseFileBtn = null!;
        private Button _browseFolderBtn = null!;
        private Button _scanBtn = null!;
        private Button _killProcessBtn = null!;
        private Button _unlockDeleteBtn = null!;
        private ListView _listView = null!;
        private StatusStrip _statusStrip = null!;
        private ToolStripStatusLabel _statusLabel = null!;

        private List<LockProcessInfo> _lockingProcesses = new();

        public FileUnlockerWindow(string? initialPath = null)
        {
            InitializeComponent();
            ApplyTheme();
            if (!string.IsNullOrEmpty(initialPath))
            {
                _pathTextBox.Text = initialPath;
                _ = ScanLocksAsync();
            }
        }

        private void InitializeComponent()
        {
            Text = LanguageManager.GetString("FileUnlocker_Title") ?? "File & Process Unlocker - EBUninstaller Pro";
            Size = new Size(880, 520);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(720, 400);
            AllowDrop = true;

            DragEnter += (s, e) =>
            {
                if (e.Data?.GetDataPresent(DataFormats.FileDrop) == true)
                    e.Effect = DragDropEffects.Copy;
            };

            DragDrop += (s, e) =>
            {
                if (e.Data?.GetDataPresent(DataFormats.FileDrop) == true)
                {
                    string[] files = (string[])e.Data.GetData(DataFormats.FileDrop)!;
                    if (files.Length > 0)
                    {
                        _pathTextBox.Text = files[0];
                        _ = ScanLocksAsync();
                    }
                }
            };

            // Top Path Selection Panel
            var topPanel = new Panel { Dock = DockStyle.Top, Height = 95, Padding = new Padding(12) };
            var pathLabel = new Label { Text = "Select or Drag & Drop a locked file or folder:", Location = new Point(12, 10), AutoSize = true, Font = new Font("Segoe UI", 9.5f, FontStyle.Bold) };

            _pathTextBox = new TextBox { Location = new Point(12, 34), Width = 520, Font = new Font("Segoe UI", 10f) };
            _browseFileBtn = new Button { Text = "Browse File...", Location = new Point(540, 32), Size = new Size(110, 30), Font = new Font("Segoe UI", 9f) };
            _browseFolderBtn = new Button { Text = "Browse Folder...", Location = new Point(658, 32), Size = new Size(120, 30), Font = new Font("Segoe UI", 9f) };
            _scanBtn = new Button { Text = "🔍 Find Locks", Location = new Point(12, 66), Size = new Size(130, 26), Font = new Font("Segoe UI", 9f, FontStyle.Bold) };

            _killProcessBtn = new Button { Text = "⚡ Terminate Process", Location = new Point(150, 66), Size = new Size(160, 26), Enabled = false, Font = new Font("Segoe UI", 9f) };
            _unlockDeleteBtn = new Button { Text = "🗑️ Unlock & Force Delete", Location = new Point(318, 66), Size = new Size(180, 26), Enabled = false, Font = new Font("Segoe UI", 9f, FontStyle.Bold), ForeColor = Color.FromArgb(180, 30, 30) };

            _browseFileBtn.Click += (s, e) =>
            {
                using var ofd = new OpenFileDialog { Title = "Select Locked File", CheckFileExists = true };
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    _pathTextBox.Text = ofd.FileName;
                    _ = ScanLocksAsync();
                }
            };

            _browseFolderBtn.Click += (s, e) =>
            {
                using var fbd = new FolderBrowserDialog { Description = "Select Locked Folder" };
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    _pathTextBox.Text = fbd.SelectedPath;
                    _ = ScanLocksAsync();
                }
            };

            _scanBtn.Click += async (s, e) => await ScanLocksAsync();
            _killProcessBtn.Click += (s, e) => TerminateSelectedProcess();
            _unlockDeleteBtn.Click += async (s, e) => await UnlockAndDeleteAsync();

            topPanel.Controls.Add(pathLabel);
            topPanel.Controls.Add(_pathTextBox);
            topPanel.Controls.Add(_browseFileBtn);
            topPanel.Controls.Add(_browseFolderBtn);
            topPanel.Controls.Add(_scanBtn);
            topPanel.Controls.Add(_killProcessBtn);
            topPanel.Controls.Add(_unlockDeleteBtn);

            // ListView
            _listView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                MultiSelect = false
            };
            _listView.Columns.Add("Process Name", 180);
            _listView.Columns.Add("Process ID (PID)", 110);
            _listView.Columns.Add("Application Title", 200);
            _listView.Columns.Add("System Process", 120);
            _listView.Columns.Add("Executable Path", 340);

            _listView.SelectedIndexChanged += (s, e) =>
            {
                _killProcessBtn.Enabled = _listView.SelectedItems.Count > 0;
            };

            // StatusStrip
            _statusStrip = new StatusStrip();
            _statusLabel = new ToolStripStatusLabel { Spring = true, TextAlign = ContentAlignment.MiddleLeft, Text = "Ready. Enter path or drag and drop a file." };
            _statusStrip.Items.Add(_statusLabel);

            Controls.Add(_listView);
            Controls.Add(topPanel);
            Controls.Add(_statusStrip);
        }

        private async Task ScanLocksAsync()
        {
            string path = _pathTextBox.Text.Trim();
            if (string.IsNullOrEmpty(path) || (!File.Exists(path) && !Directory.Exists(path)))
            {
                MessageBox.Show("The specified path does not exist on disk.", "Invalid Path", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _statusLabel.Text = $"Analyzing locks on: {path}...";
            _scanBtn.Enabled = false;
            _listView.Items.Clear();

            _lockingProcesses = await Task.Run(() => FileUnlockerManager.FindLockingProcesses(path));

            foreach (var proc in _lockingProcesses)
            {
                var lvi = new ListViewItem(proc.ProcessName) { Tag = proc };
                lvi.SubItems.Add(proc.ProcessId.ToString());
                lvi.SubItems.Add(proc.ApplicationName);
                lvi.SubItems.Add(proc.IsSystemProcess ? "Yes (Protected)" : "No (Safe to close)");
                lvi.SubItems.Add(proc.MainModulePath);

                if (proc.IsSystemProcess)
                {
                    lvi.ForeColor = Color.FromArgb(0, 100, 180);
                }
                else
                {
                    lvi.ForeColor = Color.FromArgb(190, 40, 40);
                }

                _listView.Items.Add(lvi);
            }

            _scanBtn.Enabled = true;
            _unlockDeleteBtn.Enabled = true;
            _killProcessBtn.Enabled = _listView.SelectedItems.Count > 0;

            if (_lockingProcesses.Count == 0)
            {
                _statusLabel.Text = "No active process locks detected. File is accessible.";
            }
            else
            {
                _statusLabel.Text = $"Detected {_lockingProcesses.Count} locking process(es).";
            }
        }

        private void TerminateSelectedProcess()
        {
            if (_listView.SelectedItems.Count == 0) return;

            if (_listView.SelectedItems[0].Tag is LockProcessInfo info)
            {
                if (info.IsSystemProcess)
                {
                    MessageBox.Show($"'{info.ProcessName}' is a core Windows system process and cannot be terminated.", "Protected Process", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (MessageBox.Show($"Are you sure you want to terminate '{info.ProcessName}' (PID {info.ProcessId})?", "Confirm Process Termination", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (FileUnlockerManager.TerminateLockingProcess(info.ProcessId))
                    {
                        MessageBox.Show($"Terminated {info.ProcessName}.", "Process Terminated", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        _ = ScanLocksAsync();
                    }
                }
            }
        }

        private async Task UnlockAndDeleteAsync()
        {
            string path = _pathTextBox.Text.Trim();
            if (string.IsNullOrEmpty(path)) return;

            if (MessageBox.Show($"Are you sure you want to unlock and permanently delete '{Path.GetFileName(path)}'?", "Confirm Force Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            _statusLabel.Text = "Unlocking and deleting...";
            bool success = await Task.Run(() => FileUnlockerManager.UnlockAndDelete(path));

            if (success)
            {
                MessageBox.Show("File / Folder successfully unlocked and removed.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                _pathTextBox.Text = string.Empty;
                _listView.Items.Clear();
                _statusLabel.Text = "Target unlocked and removed successfully.";
            }
            else
            {
                MessageBox.Show("Could not delete target path. Check permissions or protected status.", "Deletion Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                await ScanLocksAsync();
            }
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

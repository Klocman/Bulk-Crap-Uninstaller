/*
    OpenUninstall Pro - Professional Next-Generation Windows Uninstaller
    Backup Manager Window
*/

using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using BrightIdeasSoftware;
using UninstallTools.Backup;

namespace BulkCrapUninstaller.Forms.Windows
{
    public sealed class BackupManagerWindow : Form
    {
        private FastObjectListView _folvBackups;
        private TextBox _txtDetails;
        private Button _btnRestore;
        private Button _btnVerify;
        private Button _btnDelete;
        private Button _btnExport;
        private Button _btnImport;
        private Button _btnClose;
        private Label _lblStatus;

        public BackupManagerWindow()
        {
            InitializeComponent();
            RefreshBackups();
        }

        private void InitializeComponent()
        {
            Text = "OpenUninstall Pro - Backup & Recovery Center";
            Size = new Size(900, 600);
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
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Top bar
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // Split list & details
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Status
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Buttons

            // Top action bar
            var topPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, Margin = new Padding(0, 0, 0, 8) };
            _btnImport = new Button { Text = "Import Backup Archive...", AutoSize = true };
            _btnImport.Click += (s, e) => ImportBackup();

            _btnExport = new Button { Text = "Export Selected Backup...", AutoSize = true, Enabled = false };
            _btnExport.Click += (s, e) => ExportBackup();

            topPanel.Controls.Add(_btnImport);
            topPanel.Controls.Add(_btnExport);
            mainLayout.Controls.Add(topPanel, 0, 0);

            // Split Container for Backups list and Details
            var split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 280
            };

            _folvBackups = new FastObjectListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                MultiSelect = false,
                ShowGroups = false,
                GridLines = true
            };

            var colDate = new OLVColumn("Created Date", nameof(BackupSummary.CreatedAt)) { Width = 150, AspectToStringConverter = v => ((DateTime)v).ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss") };
            var colApp = new OLVColumn("Application", nameof(BackupSummary.ApplicationName)) { Width = 260, FillsFreeSpace = true };
            var colReg = new OLVColumn("Registry Keys", nameof(BackupSummary.RegistryEntriesCount)) { Width = 100 };
            var colFiles = new OLVColumn("Files", nameof(BackupSummary.FileEntriesCount)) { Width = 90 };
            var colSize = new OLVColumn("Size", nameof(BackupSummary.TotalSizeBytes)) { Width = 100, AspectToStringConverter = v => $"{(long)v / 1024.0:F1} KB" };
            var colRp = new OLVColumn("Restore Point", nameof(BackupSummary.HasRestorePoint)) { Width = 100, AspectToStringConverter = v => (bool)v ? "Yes" : "No" };

            _folvBackups.AllColumns.AddRange(new[] { colDate, colApp, colReg, colFiles, colSize, colRp });
            _folvBackups.RebuildColumns();
            _folvBackups.SelectionChanged += (s, e) => OnBackupSelected();

            split.Panel1.Controls.Add(_folvBackups);

            _txtDetails = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Consolas", 8.5F, FontStyle.Regular, GraphicsUnit.Point)
            };
            split.Panel2.Controls.Add(_txtDetails);

            mainLayout.Controls.Add(split, 0, 1);

            // Status label
            _lblStatus = new Label { Text = "Select a backup to view manifest details or restore.", AutoSize = true, Margin = new Padding(0, 4, 0, 4) };
            mainLayout.Controls.Add(_lblStatus, 0, 2);

            // Action Buttons
            var btnPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                AutoSize = true
            };

            _btnClose = new Button { Text = "Close", DialogResult = DialogResult.Cancel, AutoSize = true };
            _btnDelete = new Button { Text = "Delete Backup", AutoSize = true, Enabled = false, ForeColor = Color.DarkRed };
            _btnDelete.Click += (s, e) => DeleteSelectedBackup();

            _btnVerify = new Button { Text = "Verify Integrity (SHA-256)", AutoSize = true, Enabled = false };
            _btnVerify.Click += (s, e) => VerifySelectedBackup();

            _btnRestore = new Button { Text = "Restore Application", AutoSize = true, Enabled = false, Font = new Font(Font, FontStyle.Bold) };
            _btnRestore.Click += (s, e) => RestoreSelectedBackup();

            btnPanel.Controls.Add(_btnClose);
            btnPanel.Controls.Add(_btnDelete);
            btnPanel.Controls.Add(_btnVerify);
            btnPanel.Controls.Add(_btnRestore);
            mainLayout.Controls.Add(btnPanel, 0, 3);

            Controls.Add(mainLayout);
        }

        private void RefreshBackups()
        {
            var backups = BackupManager.ListBackups();
            _folvBackups.SetObjects(backups);
            _lblStatus.Text = $"Found {backups.Count} backups in storage.";
            _txtDetails.Text = string.Empty;
            UpdateButtons(false);
        }

        private void OnBackupSelected()
        {
            var selected = _folvBackups.SelectedObject as BackupSummary;
            if (selected == null)
            {
                _txtDetails.Text = string.Empty;
                UpdateButtons(false);
                return;
            }

            UpdateButtons(true);
            var manifest = BackupManager.GetBackup(selected.BackupId);
            if (manifest != null)
            {
                var sb = new System.Text.StringBuilder();
                sb.AppendLine($"BACKUP MANIFEST: {manifest.ApplicationName}");
                sb.AppendLine($"Backup ID: {manifest.BackupId}");
                sb.AppendLine($"Created: {manifest.CreatedAt.ToLocalTime():yyyy-MM-dd HH:mm:ss}");
                sb.AppendLine($"Publisher: {manifest.ApplicationPublisher}");
                sb.AppendLine($"Operation: {manifest.OperationType}");
                if (!string.IsNullOrEmpty(manifest.SystemRestorePointName))
                    sb.AppendLine($"Restore Point: {manifest.SystemRestorePointName}");
                sb.AppendLine();
                sb.AppendLine($"REGISTRY ENTRIES ({manifest.RegistryEntries.Count}):");
                foreach (var r in manifest.RegistryEntries)
                    sb.AppendLine($" - {r.KeyPath} (SHA-256: {r.Sha256Hash?.Substring(0, 16)}...)");
                sb.AppendLine();
                sb.AppendLine($"FILE ARCHIVE ENTRIES ({manifest.FileEntries.Count}):");
                foreach (var f in manifest.FileEntries)
                    sb.AppendLine($" - {f.OriginalPath} [{f.Size / 1024.0:F1} KB] (SHA-256: {f.Sha256Hash?.Substring(0, 16)}...)");

                _txtDetails.Text = sb.ToString();
            }
        }

        private void UpdateButtons(bool hasSelection)
        {
            _btnRestore.Enabled = hasSelection;
            _btnVerify.Enabled = hasSelection;
            _btnDelete.Enabled = hasSelection;
            _btnExport.Enabled = hasSelection;
        }

        private void VerifySelectedBackup()
        {
            var selected = _folvBackups.SelectedObject as BackupSummary;
            if (selected == null) return;

            var result = BackupManager.VerifyBackup(selected.BackupId);
            if (result.IsValid)
            {
                MessageBox.Show($"All {result.TotalItemsChecked} backup items verified successfully against cryptographic SHA-256 checksums.",
                    "Integrity Verified", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                var errStr = string.Join("\n", result.Errors);
                MessageBox.Show($"Verification failed!\n\nMissing: {result.MissingItemsCount}\nCorrupted: {result.CorruptedItemsCount}\n\nDetails:\n{errStr}",
                    "Integrity Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RestoreSelectedBackup()
        {
            var selected = _folvBackups.SelectedObject as BackupSummary;
            if (selected == null) return;

            if (MessageBox.Show($"Restore backup for '{selected.ApplicationName}'?\n\nThis will restore backed-up registry keys and extract application files.",
                "Confirm Restore", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            var success = BackupManager.RestoreBackup(selected.BackupId, out var restored, out var errors);
            if (success)
            {
                MessageBox.Show($"Backup restored successfully! ({restored.Count} items restored)", "Restore Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                var errStr = string.Join("\n", errors);
                MessageBox.Show($"Restore completed with errors:\n{errStr}", "Restore Incomplete", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void DeleteSelectedBackup()
        {
            var selected = _folvBackups.SelectedObject as BackupSummary;
            if (selected == null) return;

            if (MessageBox.Show($"Permanently delete backup for '{selected.ApplicationName}'?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            BackupManager.DeleteBackup(selected.BackupId);
            RefreshBackups();
        }

        private void ExportBackup()
        {
            var selected = _folvBackups.SelectedObject as BackupSummary;
            if (selected == null) return;

            using var sfd = new SaveFileDialog
            {
                FileName = $"Backup_{selected.ApplicationName}_{selected.BackupId.Substring(0, 8)}.zip",
                Filter = "ZIP Archive (*.zip)|*.zip"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                if (BackupManager.ExportBackup(selected.BackupId, sfd.FileName))
                {
                    MessageBox.Show("Backup exported successfully.", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void ImportBackup()
        {
            using var ofd = new OpenFileDialog { Filter = "ZIP Archive (*.zip)|*.zip" };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                var backupId = BackupManager.ImportBackup(ofd.FileName);
                if (!string.IsNullOrEmpty(backupId))
                {
                    MessageBox.Show("Backup imported successfully.", "Import Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    RefreshBackups();
                }
                else
                {
                    MessageBox.Show("Failed to import backup archive.", "Import Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}

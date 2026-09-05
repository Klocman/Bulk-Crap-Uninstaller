/*
    OpenUninstall Pro - Application Details & Inspection Panel
    Modern Windows 11 Styled Inspector
*/

using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using BulkCrapUninstaller.Forms.Windows;
using BulkCrapUninstaller.Functions;
using UninstallTools;
using UninstallTools.Core;
using UninstallTools.Detection;

namespace BulkCrapUninstaller.Controls
{
    public sealed class AppDetailsPanel : UserControl
    {
        private PictureBox _picIcon;
        private Label _lblName;
        private Label _lblMeta;
        private Label _lblSignature;
        private Label _lblLocation;
        private Label _lblUninstallString;
        private Label _lblConfidence;

        private Button _btnUninstall;
        private Button _btnForcedRemoval;
        private Button _btnScanLeftovers;
        private Button _btnOpenFolder;
        private Button _btnOpenRegistry;
        private Button _btnBackup;

        private ApplicationUninstallerEntry _currentEntry;

        public event EventHandler<ApplicationUninstallerEntry> RequestUninstall;
        public event EventHandler<ApplicationUninstallerEntry> RequestForcedRemoval;
        public event EventHandler<ApplicationUninstallerEntry> RequestScanLeftovers;

        public AppDetailsPanel()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Dock = DockStyle.Bottom;
            Height = 150;
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            Padding = new Padding(10);

            var mainTable = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1
            };
            mainTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 55)); // Icon
            mainTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100)); // Metadata info
            mainTable.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // Action Buttons

            // 1. Icon Box
            _picIcon = new PictureBox
            {
                Size = new Size(48, 48),
                SizeMode = PictureBoxSizeMode.CenterImage,
                Margin = new Padding(0, 4, 8, 0)
            };
            mainTable.Controls.Add(_picIcon, 0, 0);

            // 2. Metadata Information Layout
            var metaLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 5
            };
            metaLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Title
            metaLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Version / Publisher / Size
            metaLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Signature & Confidence
            metaLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Install Location
            metaLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Command

            _lblName = new Label
            {
                Text = "Select an application to view details",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                AutoSize = true
            };

            _lblMeta = new Label
            {
                Text = "Publisher: - | Version: - | Size: - | Date: -",
                ForeColor = Color.Gray,
                AutoSize = true
            };

            _lblSignature = new Label
            {
                Text = "Digital Signature: -",
                AutoSize = true
            };

            _lblLocation = new Label
            {
                Text = "Location: -",
                ForeColor = Color.DimGray,
                AutoSize = true,
                AutoEllipsis = true
            };

            _lblUninstallString = new Label
            {
                Text = "Uninstall Command: -",
                ForeColor = Color.DimGray,
                AutoSize = true,
                AutoEllipsis = true
            };

            metaLayout.Controls.Add(_lblName, 0, 0);
            metaLayout.Controls.Add(_lblMeta, 0, 1);
            metaLayout.Controls.Add(_lblSignature, 0, 2);
            metaLayout.Controls.Add(_lblLocation, 0, 3);
            metaLayout.Controls.Add(_lblUninstallString, 0, 4);
            mainTable.Controls.Add(metaLayout, 1, 0);

            // 3. Action Buttons Grid
            var buttonGrid = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 3,
                AutoSize = true
            };
            buttonGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));
            buttonGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));

            _btnUninstall = new Button { Text = "Uninstall", Dock = DockStyle.Fill, Font = new Font(Font, FontStyle.Bold), Height = 30 };
            _btnUninstall.Click += (s, e) => { if (_currentEntry != null) RequestUninstall?.Invoke(this, _currentEntry); };

            _btnForcedRemoval = new Button { Text = "Forced Removal", Dock = DockStyle.Fill, ForeColor = Color.DarkRed, Height = 30 };
            _btnForcedRemoval.Click += (s, e) => { if (_currentEntry != null) RequestForcedRemoval?.Invoke(this, _currentEntry); };

            _btnScanLeftovers = new Button { Text = "Scan Leftovers", Dock = DockStyle.Fill, Height = 30 };
            _btnScanLeftovers.Click += (s, e) => { if (_currentEntry != null) RequestScanLeftovers?.Invoke(this, _currentEntry); };

            _btnBackup = new Button { Text = "Backup App", Dock = DockStyle.Fill, Height = 30 };
            _btnBackup.Click += (s, e) => BackupCurrentApp();

            _btnOpenFolder = new Button { Text = "Open Folder", Dock = DockStyle.Fill, Height = 30 };
            _btnOpenFolder.Click += (s, e) => OpenAppFolder();

            _btnOpenRegistry = new Button { Text = "Open Registry", Dock = DockStyle.Fill, Height = 30 };
            _btnOpenRegistry.Click += (s, e) => OpenAppRegistry();

            buttonGrid.Controls.Add(_btnUninstall, 0, 0);
            buttonGrid.Controls.Add(_btnForcedRemoval, 1, 0);
            buttonGrid.Controls.Add(_btnScanLeftovers, 0, 1);
            buttonGrid.Controls.Add(_btnBackup, 1, 1);
            buttonGrid.Controls.Add(_btnOpenFolder, 0, 2);
            buttonGrid.Controls.Add(_btnOpenRegistry, 1, 2);

            mainTable.Controls.Add(buttonGrid, 2, 0);
            Controls.Add(mainTable);

            SetButtonsEnabled(false);
        }

        public void DisplayApplication(ApplicationUninstallerEntry entry)
        {
            _currentEntry = entry;
            if (entry == null)
            {
                _lblName.Text = "Select an application to view details";
                _lblMeta.Text = "Publisher: - | Version: - | Size: - | Date: -";
                _lblSignature.Text = "Digital Signature: -";
                _lblSignature.ForeColor = Color.Gray;
                _lblLocation.Text = "Location: -";
                _lblUninstallString.Text = "Uninstall Command: -";
                _picIcon.Image = null;
                SetButtonsEnabled(false);
                return;
            }

            SetButtonsEnabled(true);
            _lblName.Text = entry.DisplayName;
            var dateStr = entry.InstallDate != default ? entry.InstallDate.ToString("yyyy-MM-dd") : "Unknown";
            _lblMeta.Text = $"Publisher: {entry.Publisher ?? "Unknown"}  |  Version: {entry.DisplayVersion ?? "Unknown"}  |  Size: {entry.EstimatedSize}  |  Installed: {dateStr}";
            _lblLocation.Text = $"Location: {entry.InstallLocation ?? "N/A"}";
            _lblUninstallString.Text = $"Type: {entry.UninstallerKind}  |  Command: {entry.UninstallString ?? "None"}";

            // Digital Signature verification
            if (!string.IsNullOrEmpty(entry.UninstallerFullFilename) && File.Exists(entry.UninstallerFullFilename))
            {
                var sig = DigitalSignatureVerifier.VerifySignature(entry.UninstallerFullFilename);
                if (sig.IsSigned && sig.IsValid)
                {
                    _lblSignature.Text = $"Digital Signature: Verified ({sig.SignerName ?? sig.Subject})";
                    _lblSignature.ForeColor = Color.ForestGreen;
                }
                else if (sig.IsSigned)
                {
                    _lblSignature.Text = $"Digital Signature: {sig.Status} ({sig.StatusMessage})";
                    _lblSignature.ForeColor = Color.DarkOrange;
                }
                else
                {
                    _lblSignature.Text = "Digital Signature: Unsigned binary";
                    _lblSignature.ForeColor = Color.DimGray;
                }
            }
            else
            {
                var confidence = ConfidenceScorer.CalculateConfidence(entry);
                _lblSignature.Text = $"Confidence: {confidence.Score}/100 [{confidence.Level}]  |  Registered: {(entry.IsRegistered ? "Yes" : "No")}";
                _lblSignature.ForeColor = confidence.Score >= 70 ? Color.ForestGreen : Color.DimGray;
            }

            _btnOpenFolder.Enabled = !string.IsNullOrEmpty(entry.InstallLocation) && Directory.Exists(entry.InstallLocation);
            _btnOpenRegistry.Enabled = !string.IsNullOrEmpty(entry.RegistryPath);
        }

        private void SetButtonsEnabled(bool enabled)
        {
            _btnUninstall.Enabled = enabled;
            _btnForcedRemoval.Enabled = enabled;
            _btnScanLeftovers.Enabled = enabled;
            _btnBackup.Enabled = enabled;
            _btnOpenFolder.Enabled = enabled;
            _btnOpenRegistry.Enabled = enabled;
        }

        private void OpenAppFolder()
        {
            if (_currentEntry == null || string.IsNullOrEmpty(_currentEntry.InstallLocation)) return;
            try
            {
                if (Directory.Exists(_currentEntry.InstallLocation))
                    Process.Start(new ProcessStartInfo { FileName = "explorer.exe", Arguments = $"\"{_currentEntry.InstallLocation}\"", UseShellExecute = true });
            }
            catch { }
        }

        private void OpenAppRegistry()
        {
            if (_currentEntry == null || string.IsNullOrEmpty(_currentEntry.RegistryPath)) return;
            try
            {
                Process.Start(new ProcessStartInfo { FileName = "regedit.exe", UseShellExecute = true });
            }
            catch { }
        }

        private void BackupCurrentApp()
        {
            if (_currentEntry == null) return;
            var reg = !string.IsNullOrEmpty(_currentEntry.RegistryPath) ? new[] { _currentEntry.RegistryPath } : null;
            var files = (!string.IsNullOrEmpty(_currentEntry.InstallLocation) && Directory.Exists(_currentEntry.InstallLocation)) ? new[] { _currentEntry.InstallLocation } : null;

            var manifest = UninstallTools.Backup.BackupManager.CreateBackup(_currentEntry.DisplayName, _currentEntry.DisplayVersion, _currentEntry.Publisher, reg, files, true);
            MessageBox.Show($"Backup created successfully for '{_currentEntry.DisplayName}'!\n\nBackup ID: {manifest.BackupId}", "Backup Created", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}

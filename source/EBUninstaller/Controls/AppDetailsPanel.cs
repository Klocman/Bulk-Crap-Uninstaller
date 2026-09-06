/*
    EBUninstaller Pro - Application Details & Inspection Panel
    Modern Windows 11 Tabbed Inspector with Deep Security, Registry & Leftover Details
*/

using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using BulkCrapUninstaller.Functions;
using UninstallTools;
using UninstallTools.Backup;
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

        private TabControl _tabDetails;
        private TabPage _tabOverview;
        private TabPage _tabSecurity;
        private TabPage _tabRegistry;

        private TextBox _txtSecurityDetails;
        private TextBox _txtRegistryDetails;

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
            Height = 165;
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            Padding = new Padding(8, 4, 8, 4);

            var mainTable = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1
            };
            mainTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 50)); // Icon
            mainTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100)); // Tabbed details
            mainTable.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // Action Buttons

            // 1. Icon Box
            _picIcon = new PictureBox
            {
                Size = new Size(42, 42),
                SizeMode = PictureBoxSizeMode.CenterImage,
                Margin = new Padding(0, 6, 8, 0)
            };
            mainTable.Controls.Add(_picIcon, 0, 0);

            // 2. Tabbed Details Inspector
            _tabDetails = new TabControl { Dock = DockStyle.Fill };

            // Tab 1: Overview
            _tabOverview = new TabPage("Application Overview");
            var metaLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Padding = new Padding(4)
            };
            _lblName = new Label { Text = "Select an application to inspect details", Font = new Font("Segoe UI", 10.5F, FontStyle.Bold), AutoSize = true };
            _lblMeta = new Label { Text = "Publisher: - | Version: - | Size: - | Date: -", ForeColor = Color.Gray, AutoSize = true };
            _lblLocation = new Label { Text = "Install Path: -", ForeColor = Color.DimGray, AutoSize = true, AutoEllipsis = true };
            _lblUninstallString = new Label { Text = "Command: -", ForeColor = Color.DimGray, AutoSize = true, AutoEllipsis = true };

            metaLayout.Controls.Add(_lblName, 0, 0);
            metaLayout.Controls.Add(_lblMeta, 0, 1);
            metaLayout.Controls.Add(_lblLocation, 0, 2);
            metaLayout.Controls.Add(_lblUninstallString, 0, 3);
            _tabOverview.Controls.Add(metaLayout);

            // Tab 2: Security & Signatures
            _tabSecurity = new TabPage("Digital Signature & Authenticode");
            _txtSecurityDetails = new TextBox { Dock = DockStyle.Fill, Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Vertical, Font = new Font("Consolas", 8.5F) };
            _tabSecurity.Controls.Add(_txtSecurityDetails);

            // Tab 3: Registry & Identifiers
            _tabRegistry = new TabPage("Registry & Product GUID");
            _txtRegistryDetails = new TextBox { Dock = DockStyle.Fill, Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Vertical, Font = new Font("Consolas", 8.5F) };
            _tabRegistry.Controls.Add(_txtRegistryDetails);

            _tabDetails.TabPages.Add(_tabOverview);
            _tabDetails.TabPages.Add(_tabSecurity);
            _tabDetails.TabPages.Add(_tabRegistry);
            mainTable.Controls.Add(_tabDetails, 1, 0);

            // 3. Action Buttons Grid
            var buttonGrid = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 3,
                AutoSize = true
            };
            buttonGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 125));
            buttonGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 125));

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
                _lblName.Text = "Select an application to inspect details";
                _lblMeta.Text = "Publisher: - | Version: - | Size: - | Date: -";
                _lblLocation.Text = "Install Path: -";
                _lblUninstallString.Text = "Command: -";
                _txtSecurityDetails.Text = "No application selected.";
                _txtRegistryDetails.Text = "No application selected.";
                _picIcon.Image = null;
                SetButtonsEnabled(false);
                return;
            }

            SetButtonsEnabled(true);
            _lblName.Text = entry.DisplayName;
            var dateStr = entry.InstallDate != default ? entry.InstallDate.ToString("yyyy-MM-dd") : "Unknown";
            _lblMeta.Text = $"Publisher: {entry.Publisher ?? "Unknown"}  |  Version: {entry.DisplayVersion ?? "Unknown"}  |  Size: {entry.EstimatedSize}  |  Installed: {dateStr}";
            _lblLocation.Text = $"Location: {entry.InstallLocation ?? "N/A"}";
            _lblUninstallString.Text = $"Kind: {entry.UninstallerKind}  |  Command: {entry.UninstallString ?? "None"}";

            // Digital Signature Verification & Tab Details
            if (!string.IsNullOrEmpty(entry.UninstallerFullFilename) && File.Exists(entry.UninstallerFullFilename))
            {
                var sig = DigitalSignatureVerifier.VerifySignature(entry.UninstallerFullFilename);
                var sha = CryptoHasher.ComputeFileSha256(entry.UninstallerFullFilename);
                _txtSecurityDetails.Text = $"Status: {sig.Status}\nSubject: {sig.Subject}\nIssuer: {sig.Issuer}\nSigner: {sig.SignerName}\nSerial: {sig.SerialNumber}\nValid: {sig.ValidFrom} to {sig.ValidTo}\nSHA-256: {sha}\nPath: {entry.UninstallerFullFilename}";
            }
            else
            {
                var conf = ConfidenceScorer.CalculateConfidence(entry);
                _txtSecurityDetails.Text = $"Authenticode: Unsigned binary or uninstaller path unavailable.\nConfidence Score: {conf.Score}/100 [{conf.Level}]\nRegistered: {(entry.IsRegistered ? "Yes" : "No")}\nOrphaned: {(entry.IsOrphaned ? "Yes" : "No")}";
            }

            // Registry Details
            _txtRegistryDetails.Text = $"Registry Path: {entry.RegistryPath ?? "N/A"}\nProduct GUID: {entry.BundleProviderKey ?? entry.RatingId ?? "N/A"}\nQuiet Uninstall: {(entry.QuietUninstallPossible ? entry.QuietUninstallString : "Not available")}\nRegistry Key Valid: {(entry.RegistryKeyActuallyExists() ? "Yes" : "No")}";

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

            var manifest = BackupManager.CreateBackup(_currentEntry.DisplayName, _currentEntry.DisplayVersion, _currentEntry.Publisher, reg, files, true);
            MessageBox.Show($"Backup created successfully for '{_currentEntry.DisplayName}'!\n\nBackup ID: {manifest.BackupId}", "Backup Created", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}

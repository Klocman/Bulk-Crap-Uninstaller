/*
    EBUninstaller Pro - System Restore Point Manager Window
    Modern GUI for viewing, creating, and launching Windows System Restore points.
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using UninstallTools.Backup;
using UninstallTools.Localization;

namespace BulkCrapUninstaller.Forms.Windows
{
    public class SystemRestorePointWindow : Form
    {
        private ListView _listView = null!;
        private ToolStrip _toolStrip = null!;
        private ToolStripButton _refreshBtn = null!;
        private ToolStripButton _launchRstruiBtn = null!;
        private Panel _createPanel = null!;
        private TextBox _descTextBox = null!;
        private Button _createBtn = null!;
        private StatusStrip _statusStrip = null!;
        private ToolStripStatusLabel _statusLabel = null!;

        private List<SystemRestorePointItem> _restorePoints = new();

        public SystemRestorePointWindow()
        {
            InitializeComponent();
            ApplyTheme();
            Load += async (s, e) => await RefreshRestorePointsAsync();
        }

        private void InitializeComponent()
        {
            Text = LanguageManager.GetString("RestorePointManager_Title") ?? "Windows System Restore Points - EBUninstaller Pro";
            Size = new Size(950, 580);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(780, 420);

            // Create Restore Point Top Panel
            _createPanel = new Panel { Dock = DockStyle.Top, Height = 65, Padding = new Padding(12) };
            var label = new Label { Text = "Create New System Restore Point:", Location = new Point(12, 10), AutoSize = true, Font = new Font("Segoe UI", 9.5f, FontStyle.Bold) };
            _descTextBox = new TextBox { Location = new Point(12, 32), Width = 560, Font = new Font("Segoe UI", 10f), Text = "EBUninstaller Pro Manual Backup Checkpoint" };
            _createBtn = new Button { Text = "🛡️ Create Restore Point", Location = new Point(580, 30), Size = new Size(200, 28), Font = new Font("Segoe UI", 9f, FontStyle.Bold) };
            _createBtn.Click += async (s, e) => await CreatePointAsync();

            _createPanel.Controls.Add(label);
            _createPanel.Controls.Add(_descTextBox);
            _createPanel.Controls.Add(_createBtn);

            // ToolStrip
            _toolStrip = new ToolStrip { ImageList = null, RenderMode = ToolStripRenderMode.System, GripStyle = ToolStripGripStyle.Hidden };
            _refreshBtn = new ToolStripButton("🔄 Refresh", null, async (s, e) => await RefreshRestorePointsAsync());
            _launchRstruiBtn = new ToolStripButton("🚀 Launch System Restore Wizard (rstrui.exe)", null, (s, e) => LaunchSystemRestore());

            _toolStrip.Items.Add(_refreshBtn);
            _toolStrip.Items.Add(new ToolStripSeparator());
            _toolStrip.Items.Add(_launchRstruiBtn);

            // ListView
            _listView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true
            };
            _listView.Columns.Add("Sequence #", 100);
            _listView.Columns.Add("Restore Point Description", 380);
            _listView.Columns.Add("Type", 180);
            _listView.Columns.Add("Creation Date & Time", 200);

            _statusStrip = new StatusStrip();
            _statusLabel = new ToolStripStatusLabel { Spring = true, TextAlign = ContentAlignment.MiddleLeft };
            _statusStrip.Items.Add(_statusLabel);

            Controls.Add(_listView);
            Controls.Add(_toolStrip);
            Controls.Add(_createPanel);
            Controls.Add(_statusStrip);
        }

        private async Task RefreshRestorePointsAsync()
        {
            _statusLabel.Text = "Querying Windows Volume Shadow Copy & System Restore points...";
            _refreshBtn.Enabled = false;
            _listView.Items.Clear();

            _restorePoints = await Task.Run(() => SystemRestorePointManager.GetRestorePoints());

            foreach (var rp in _restorePoints.OrderByDescending(r => r.CreationTime))
            {
                var lvi = new ListViewItem(rp.SequenceNumber.ToString()) { Tag = rp };
                lvi.SubItems.Add(rp.Description);
                lvi.SubItems.Add(FormatType(rp.Type));
                lvi.SubItems.Add(rp.CreationTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"));

                if (rp.Type == RestorePointType.ApplicationUninstall)
                {
                    lvi.ForeColor = Color.FromArgb(0, 100, 180);
                }

                _listView.Items.Add(lvi);
            }

            _refreshBtn.Enabled = true;
            _statusLabel.Text = $"Found {_restorePoints.Count} active system restore points.";
        }

        private async Task CreatePointAsync()
        {
            string desc = _descTextBox.Text.Trim();
            if (string.IsNullOrEmpty(desc))
            {
                MessageBox.Show("Please enter a description for the restore point.", "EBUninstaller Pro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _createBtn.Enabled = false;
            _statusLabel.Text = "Creating Windows System Restore point via VSS...";

            bool success = await Task.Run(() => SystemRestorePointManager.CreateRestorePoint(desc, RestorePointType.ManualCheckpoint));

            _createBtn.Enabled = true;
            if (success)
            {
                MessageBox.Show("System Restore point created successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await RefreshRestorePointsAsync();
            }
            else
            {
                MessageBox.Show("Could not create System Restore point. Ensure System Protection is enabled in Windows and EBUninstaller Pro has administrative privileges.", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _statusLabel.Text = "Restore point creation failed.";
            }
        }

        private void LaunchSystemRestore()
        {
            try
            {
                Process.Start(new ProcessStartInfo("rstrui.exe") { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to launch rstrui.exe: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static string FormatType(RestorePointType type)
        {
            return type switch
            {
                RestorePointType.ApplicationInstall => "App Install",
                RestorePointType.ApplicationUninstall => "App Uninstall (Safe Backup)",
                RestorePointType.DeviceDriverInstall => "Driver Install",
                RestorePointType.ModifySettings => "Settings Change",
                RestorePointType.ManualCheckpoint => "Manual Checkpoint",
                _ => "System Snapshot"
            };
        }

        private void ApplyTheme()
        {
            bool isDark = ThemeManager.IsDarkModeEnabled;
            BackColor = isDark ? Color.FromArgb(32, 32, 32) : Color.FromArgb(245, 245, 245);
            ForeColor = isDark ? Color.White : Color.Black;
            _createPanel.BackColor = isDark ? Color.FromArgb(40, 40, 40) : Color.FromArgb(235, 238, 245);

            if (LanguageManager.IsRightToLeft)
            {
                RightToLeft = RightToLeft.Yes;
                RightToLeftLayout = true;
            }
        }
    }
}

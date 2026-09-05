/*
    EBUninstaller Pro - Windows Services Optimizer Window
    Modern GUI for auditing, configuring, and cleaning background Windows services.
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using UninstallTools.Localization;
using UninstallTools.Startup;

namespace BulkCrapUninstaller.Forms.Windows
{
    public class ServicesOptimizerWindow : Form
    {
        private ListView _listView = null!;
        private ToolStrip _toolStrip = null!;
        private ToolStripButton _refreshBtn = null!;
        private ToolStripDropDownButton _startupModeBtn = null!;
        private ToolStripButton _cleanOrphanedBtn = null!;
        private ToolStripComboBox _filterBox = null!;
        private StatusStrip _statusStrip = null!;
        private ToolStripStatusLabel _statusLabel = null!;

        private List<WindowsServiceItem> _services = new();

        public ServicesOptimizerWindow()
        {
            InitializeComponent();
            ApplyTheme();
            Load += async (s, e) => await RefreshServicesAsync();
        }

        private void InitializeComponent()
        {
            Text = LanguageManager.GetString("ServicesOptimizer_Title") ?? "Windows Services Optimizer - EBUninstaller Pro";
            Size = new Size(1050, 620);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(850, 480);

            _toolStrip = new ToolStrip { ImageList = null, RenderMode = ToolStripRenderMode.System, GripStyle = ToolStripGripStyle.Hidden };
            _refreshBtn = new ToolStripButton("🔄 Refresh", null, async (s, e) => await RefreshServicesAsync());

            _startupModeBtn = new ToolStripDropDownButton("⚡ Change Startup");
            _startupModeBtn.DropDownItems.Add("Automatic (Delayed)", null, (s, e) => ChangeSelectedStartup(ServiceStartupMode.AutomaticDelayed));
            _startupModeBtn.DropDownItems.Add("Automatic", null, (s, e) => ChangeSelectedStartup(ServiceStartupMode.Automatic));
            _startupModeBtn.DropDownItems.Add("Manual", null, (s, e) => ChangeSelectedStartup(ServiceStartupMode.Manual));
            _startupModeBtn.DropDownItems.Add("Disabled", null, (s, e) => ChangeSelectedStartup(ServiceStartupMode.Disabled));

            _cleanOrphanedBtn = new ToolStripButton("🧹 Clean Orphaned", null, (s, e) => CleanOrphanedServices());

            _filterBox = new ToolStripComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            _filterBox.Items.AddRange(new object[] { "Third-Party Only", "All Services", "Orphaned Only", "Running Only", "Automatic Services" });
            _filterBox.SelectedIndex = 0;
            _filterBox.SelectedIndexChanged += (s, e) => ApplyFilter();

            _toolStrip.Items.Add(_refreshBtn);
            _toolStrip.Items.Add(new ToolStripSeparator());
            _toolStrip.Items.Add(_startupModeBtn);
            _toolStrip.Items.Add(_cleanOrphanedBtn);
            _toolStrip.Items.Add(new ToolStripSeparator());
            _toolStrip.Items.Add(new ToolStripLabel("View: "));
            _toolStrip.Items.Add(_filterBox);

            _listView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                MultiSelect = true
            };
            _listView.Columns.Add("Service Display Name", 240);
            _listView.Columns.Add("Service Name", 140);
            _listView.Columns.Add("Startup Type", 130);
            _listView.Columns.Add("Status", 90);
            _listView.Columns.Add("Publisher / Vendor", 170);
            _listView.Columns.Add("Executable Image Path", 320);

            _statusStrip = new StatusStrip();
            _statusLabel = new ToolStripStatusLabel { Spring = true, TextAlign = ContentAlignment.MiddleLeft };
            _statusStrip.Items.Add(_statusLabel);

            Controls.Add(_listView);
            Controls.Add(_toolStrip);
            Controls.Add(_statusStrip);
        }

        private async Task RefreshServicesAsync()
        {
            _statusLabel.Text = "Scanning Windows services and executable dependencies...";
            _refreshBtn.Enabled = false;

            _services = await Task.Run(() => WindowsServicesOptimizer.GetServices());

            ApplyFilter();
            _refreshBtn.Enabled = true;
            _statusLabel.Text = $"Found {_services.Count} services ({_services.Count(s => !s.IsMicrosoftService)} third-party, {_services.Count(s => s.IsOrphaned)} orphaned).";
        }

        private void ApplyFilter()
        {
            _listView.Items.Clear();
            string selectedFilter = _filterBox.SelectedItem?.ToString() ?? "Third-Party Only";

            var filtered = _services.Where(s =>
            {
                if (selectedFilter == "Third-Party Only") return !s.IsMicrosoftService;
                if (selectedFilter == "Orphaned Only") return s.IsOrphaned;
                if (selectedFilter == "Running Only") return s.Status == System.ServiceProcess.ServiceControllerStatus.Running;
                if (selectedFilter == "Automatic Services") return s.StartupMode == ServiceStartupMode.Automatic || s.StartupMode == ServiceStartupMode.AutomaticDelayed;
                return true;
            }).ToList();

            foreach (var svc in filtered)
            {
                var lvi = new ListViewItem(svc.DisplayName) { Tag = svc };
                lvi.SubItems.Add(svc.ServiceName);
                lvi.SubItems.Add(svc.StartupMode.ToString());
                lvi.SubItems.Add(svc.Status.ToString());
                lvi.SubItems.Add(svc.Publisher);
                lvi.SubItems.Add(svc.ImagePath);

                if (svc.IsOrphaned)
                {
                    lvi.ForeColor = Color.FromArgb(190, 40, 40);
                }
                else if (svc.StartupMode == ServiceStartupMode.Disabled)
                {
                    lvi.ForeColor = Color.Gray;
                }
                else if (svc.IsCriticalSystem)
                {
                    lvi.ForeColor = Color.FromArgb(0, 100, 180);
                }

                _listView.Items.Add(lvi);
            }
        }

        private void ChangeSelectedStartup(ServiceStartupMode mode)
        {
            if (_listView.SelectedItems.Count == 0) return;

            foreach (ListViewItem lvi in _listView.SelectedItems)
            {
                if (lvi.Tag is WindowsServiceItem item)
                {
                    if (item.IsCriticalSystem)
                    {
                        MessageBox.Show($"'{item.DisplayName}' is a critical Windows system service and its startup mode cannot be modified.", "Protected Service", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        continue;
                    }

                    if (WindowsServicesOptimizer.ChangeStartupMode(item.ServiceName, mode))
                    {
                        item.StartupMode = mode;
                        lvi.SubItems[2].Text = mode.ToString();
                    }
                }
            }
        }

        private void CleanOrphanedServices()
        {
            var orphaned = _services.Where(s => s.IsOrphaned && !s.IsCriticalSystem).ToList();
            if (orphaned.Count == 0)
            {
                MessageBox.Show("No orphaned Windows services detected.", "EBUninstaller Pro", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show($"Found {orphaned.Count} orphaned services whose executable binaries no longer exist. Remove these service definitions from the Windows Registry?", "Confirm Service Cleanup", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            int deleted = 0;
            foreach (var svc in orphaned)
            {
                if (WindowsServicesOptimizer.DeleteOrphanedService(svc.ServiceName))
                    deleted++;
            }

            MessageBox.Show($"Successfully cleaned {deleted} orphaned service definitions.", "Cleanup Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            _ = RefreshServicesAsync();
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

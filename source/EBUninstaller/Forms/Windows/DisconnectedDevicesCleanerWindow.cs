/*
    EBUninstaller Pro - Disconnected & Ghost Devices Cleaner Window
    Modern GUI for auditing and removing stale/disconnected USB, Bluetooth, audio, and printer registry device nodes.
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using UninstallTools.JunkCleaner;
using UninstallTools.Localization;

namespace BulkCrapUninstaller.Forms.Windows
{
    public class DisconnectedDevicesCleanerWindow : Form
    {
        private ListView _listView = null!;
        private CheckBox _chkHideSystem = null!;
        private ComboBox _categoryBox = null!;
        private TextBox _searchBox = null!;
        private Button _refreshBtn = null!;
        private Button _deleteSelectedBtn = null!;
        private Button _cleanAllBtn = null!;
        private Label _statsLabel = null!;
        private StatusStrip _statusStrip = null!;
        private ToolStripStatusLabel _statusLabel = null!;

        private List<DisconnectedDeviceItem> _allDevices = new();

        public DisconnectedDevicesCleanerWindow()
        {
            InitializeComponent();
            ApplyTheme();
            _ = LoadDevicesAsync();
        }

        private void InitializeComponent()
        {
            Text = LanguageManager.GetString("DeviceCleaner_Title") ?? "Disconnected & Ghost Devices Cleaner - EBUninstaller Pro";
            Size = new Size(1080, 640);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(850, 480);

            // Top Panel
            var topPanel = new Panel { Dock = DockStyle.Top, Height = 78, Padding = new Padding(12, 8, 12, 8) };

            var searchLabel = new Label { Text = "Search:", Location = new Point(12, 12), AutoSize = true, Font = new Font("Segoe UI", 9f, FontStyle.Bold) };
            _searchBox = new TextBox { Location = new Point(70, 10), Width = 160, Font = new Font("Segoe UI", 9f) };
            _searchBox.TextChanged += (s, e) => ApplyFilter();

            _categoryBox = new ComboBox { Location = new Point(240, 9), Width = 170, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9f) };
            _categoryBox.Items.AddRange(new object[] { "All Categories", "USB Storage (Flash Drives)", "Bluetooth Devices", "Audio Endpoints", "Printers", "HID Input Devices" });
            _categoryBox.SelectedIndex = 0;
            _categoryBox.SelectedIndexChanged += (s, e) => ApplyFilter();

            _chkHideSystem = new CheckBox { Text = "Hide System Devices", Location = new Point(420, 11), AutoSize = true, Checked = true, Font = new Font("Segoe UI", 9f, FontStyle.Bold) };
            _chkHideSystem.CheckedChanged += (s, e) => ApplyFilter();

            _refreshBtn = new Button { Text = "🔄 Refresh", Location = new Point(620, 8), Size = new Size(85, 28), Font = new Font("Segoe UI", 9f) };
            _refreshBtn.Click += async (s, e) => await LoadDevicesAsync();

            _deleteSelectedBtn = new Button { Text = "🗑️ Remove Selected", Location = new Point(710, 8), Size = new Size(145, 28), Enabled = false, Font = new Font("Segoe UI", 9f, FontStyle.Bold), ForeColor = Color.FromArgb(180, 40, 40) };
            _deleteSelectedBtn.Click += async (s, e) => await DeleteSelectedAsync();

            _cleanAllBtn = new Button { Text = "⚡ Clean All Stale Devices", Location = new Point(860, 8), Size = new Size(190, 28), Enabled = false, Font = new Font("Segoe UI", 9f, FontStyle.Bold), ForeColor = Color.FromArgb(0, 120, 40) };
            _cleanAllBtn.Click += async (s, e) => await CleanAllAsync();

            _statsLabel = new Label
            {
                Text = "Scanning Windows device nodes...",
                Location = new Point(12, 46),
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 100, 180)
            };

            topPanel.Controls.Add(searchLabel);
            topPanel.Controls.Add(_searchBox);
            topPanel.Controls.Add(_categoryBox);
            topPanel.Controls.Add(_chkHideSystem);
            topPanel.Controls.Add(_refreshBtn);
            topPanel.Controls.Add(_deleteSelectedBtn);
            topPanel.Controls.Add(_cleanAllBtn);
            topPanel.Controls.Add(_statsLabel);

            // ListView
            _listView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                MultiSelect = true
            };
            _listView.Columns.Add("Device Friendly Name", 280);
            _listView.Columns.Add("Category", 150);
            _listView.Columns.Add("Hardware ID", 220);
            _listView.Columns.Add("Status", 130);
            _listView.Columns.Add("Device Instance ID", 380);

            _listView.SelectedIndexChanged += (s, e) =>
            {
                bool hasSel = _listView.SelectedItems.Count > 0;
                _deleteSelectedBtn.Enabled = hasSel && _listView.SelectedItems.Cast<ListViewItem>().All(l => !(l.Tag as DisconnectedDeviceItem)?.IsSystemCritical == true);
            };

            // StatusStrip
            _statusStrip = new StatusStrip();
            _statusLabel = new ToolStripStatusLabel { Spring = true, TextAlign = ContentAlignment.MiddleLeft, Text = "Ready." };
            _statusStrip.Items.Add(_statusLabel);

            Controls.Add(_listView);
            Controls.Add(topPanel);
            Controls.Add(_statusStrip);
        }

        private async Task LoadDevicesAsync()
        {
            _refreshBtn.Enabled = false;
            _statusLabel.Text = "Scanning PnP device tree for disconnected devices...";

            _allDevices = await Task.Run(() => DisconnectedDevicesCleaner.ScanDisconnectedDevices());

            ApplyFilter();
            _refreshBtn.Enabled = true;
            _statusLabel.Text = $"Scan complete. Identified {_allDevices.Count} total device nodes.";
        }

        private void ApplyFilter()
        {
            _listView.Items.Clear();
            string search = _searchBox.Text.Trim().ToLowerInvariant();
            string catFilter = _categoryBox.SelectedItem?.ToString() ?? "All Categories";
            bool hideSystem = _chkHideSystem.Checked;

            var filtered = _allDevices.AsEnumerable();

            if (hideSystem)
                filtered = filtered.Where(d => !d.IsSystemCritical);

            if (catFilter == "USB Storage (Flash Drives)")
                filtered = filtered.Where(d => d.Category == DeviceCategoryClass.UsbStorage);
            else if (catFilter == "Bluetooth Devices")
                filtered = filtered.Where(d => d.Category == DeviceCategoryClass.Bluetooth);
            else if (catFilter == "Audio Endpoints")
                filtered = filtered.Where(d => d.Category == DeviceCategoryClass.AudioEndpoint);
            else if (catFilter == "Printers")
                filtered = filtered.Where(d => d.Category == DeviceCategoryClass.Printer);
            else if (catFilter == "HID Input Devices")
                filtered = filtered.Where(d => d.Category == DeviceCategoryClass.HumanInterfaceDevice);

            if (!string.IsNullOrEmpty(search))
            {
                filtered = filtered.Where(d =>
                    d.FriendlyName.ToLowerInvariant().Contains(search) ||
                    d.DeviceInstanceId.ToLowerInvariant().Contains(search) ||
                    d.HardwareId.ToLowerInvariant().Contains(search));
            }

            var list = filtered.ToList();
            int total = _allDevices.Count;
            int removable = _allDevices.Count(d => !d.IsSystemCritical);

            _statsLabel.Text = $"Total Device Nodes: {total} | Disconnected / Removable: {removable} | Showing: {list.Count}";
            _cleanAllBtn.Enabled = removable > 0;

            foreach (var d in list)
            {
                var lvi = new ListViewItem(d.FriendlyName) { Tag = d };
                lvi.SubItems.Add(FormatCategory(d.Category));
                lvi.SubItems.Add(d.HardwareId);
                lvi.SubItems.Add(d.IsSystemCritical ? "System Protected" : "⚠️ Removable Stale Node");
                lvi.SubItems.Add(d.DeviceInstanceId);

                if (d.IsSystemCritical)
                    lvi.ForeColor = Color.FromArgb(0, 100, 180);
                else
                    lvi.ForeColor = Color.FromArgb(190, 40, 40);

                _listView.Items.Add(lvi);
            }
        }

        private async Task DeleteSelectedAsync()
        {
            var selected = _listView.SelectedItems.Cast<ListViewItem>()
                .Select(l => l.Tag as DisconnectedDeviceItem)
                .Where(d => d != null && !d.IsSystemCritical)
                .Cast<DisconnectedDeviceItem>()
                .ToList();

            if (selected.Count == 0) return;

            if (MessageBox.Show($"Remove {selected.Count} stale device registration node(s)?", "Confirm Device Removal", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            int deleted = 0;
            await Task.Run(() =>
            {
                foreach (var item in selected)
                {
                    if (DisconnectedDevicesCleaner.RemoveDeviceNode(item))
                    {
                        deleted++;
                        _allDevices.Remove(item);
                    }
                }
            });

            ApplyFilter();
            _statusLabel.Text = $"Removed {deleted} disconnected device nodes.";
            MessageBox.Show($"Successfully removed {deleted} stale device nodes.", "Cleanup Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async Task CleanAllAsync()
        {
            var removable = _allDevices.Where(d => !d.IsSystemCritical).ToList();
            if (removable.Count == 0) return;

            if (MessageBox.Show($"Remove all {removable.Count} disconnected/stale device registry nodes?\n\nThis is completely safe and speeds up Windows PnP device enumeration.", "Confirm Cleanup All", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            int deleted = 0;
            await Task.Run(() =>
            {
                foreach (var item in removable)
                {
                    if (DisconnectedDevicesCleaner.RemoveDeviceNode(item))
                    {
                        deleted++;
                        _allDevices.Remove(item);
                    }
                }
            });

            ApplyFilter();
            _statusLabel.Text = $"Removed all {deleted} disconnected device nodes.";
            MessageBox.Show($"Successfully cleaned {deleted} disconnected device nodes.", "Cleanup Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private static string FormatCategory(DeviceCategoryClass cat)
        {
            return cat switch
            {
                DeviceCategoryClass.UsbStorage => "USB Flash / HDD",
                DeviceCategoryClass.Bluetooth => "Bluetooth Device",
                DeviceCategoryClass.AudioEndpoint => "Audio Endpoint",
                DeviceCategoryClass.Printer => "Printer Device",
                DeviceCategoryClass.HumanInterfaceDevice => "HID Input / Gamepad",
                _ => "Other Device"
            };
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

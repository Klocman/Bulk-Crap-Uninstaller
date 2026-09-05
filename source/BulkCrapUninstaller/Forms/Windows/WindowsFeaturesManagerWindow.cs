/*
    EBUninstaller Pro - Windows Optional Features Manager Window
    Modern GUI for auditing, enabling, disabling, and removing Windows optional features and capabilities.
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using UninstallTools.Detection;
using UninstallTools.Localization;

namespace BulkCrapUninstaller.Forms.Windows
{
    public class WindowsFeaturesManagerWindow : Form
    {
        private ListView _listView = null!;
        private ToolStrip _toolStrip = null!;
        private ToolStripButton _refreshBtn = null!;
        private ToolStripButton _enableBtn = null!;
        private ToolStripButton _disableBtn = null!;
        private ToolStripComboBox _filterBox = null!;
        private StatusStrip _statusStrip = null!;
        private ToolStripStatusLabel _statusLabel = null!;

        private List<WindowsOptionalFeatureItem> _features = new();

        public WindowsFeaturesManagerWindow()
        {
            InitializeComponent();
            ApplyTheme();
            Load += async (s, e) => await RefreshFeaturesAsync();
        }

        private void InitializeComponent()
        {
            Text = LanguageManager.GetString("WindowsFeaturesManager_Title") ?? "Windows Optional Features & Capabilities - EBUninstaller Pro";
            Size = new Size(1000, 600);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(800, 450);

            _toolStrip = new ToolStrip { ImageList = null, RenderMode = ToolStripRenderMode.System, GripStyle = ToolStripGripStyle.Hidden };
            _refreshBtn = new ToolStripButton("🔄 Refresh", null, async (s, e) => await RefreshFeaturesAsync());
            _enableBtn = new ToolStripButton("✓ Enable / Install", null, async (s, e) => await SetSelectedStateAsync(true));
            _disableBtn = new ToolStripButton("⊘ Disable / Remove", null, async (s, e) => await SetSelectedStateAsync(false));

            _filterBox = new ToolStripComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            _filterBox.Items.AddRange(new object[] { "All Items", "Installed / Enabled Only", "Disabled Only", "Optional Features", "Capabilities (On-Demand)" });
            _filterBox.SelectedIndex = 0;
            _filterBox.SelectedIndexChanged += (s, e) => ApplyFilter();

            _toolStrip.Items.Add(_refreshBtn);
            _toolStrip.Items.Add(new ToolStripSeparator());
            _toolStrip.Items.Add(_enableBtn);
            _toolStrip.Items.Add(_disableBtn);
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
            _listView.Columns.Add("Feature / Capability Name", 280);
            _listView.Columns.Add("State", 120);
            _listView.Columns.Add("Kind", 120);
            _listView.Columns.Add("Internal Identifier", 420);

            _statusStrip = new StatusStrip();
            _statusLabel = new ToolStripStatusLabel { Spring = true, TextAlign = ContentAlignment.MiddleLeft };
            _statusStrip.Items.Add(_statusLabel);

            Controls.Add(_listView);
            Controls.Add(_toolStrip);
            Controls.Add(_statusStrip);
        }

        private async Task RefreshFeaturesAsync()
        {
            _statusLabel.Text = "Scanning Windows optional features and capabilities via DISM...";
            _refreshBtn.Enabled = false;

            _features = await Task.Run(() => WindowsOptionalFeaturesManager.GetOptionalFeatures(msg =>
            {
                BeginInvoke(new Action(() => _statusLabel.Text = msg));
            }));

            ApplyFilter();
            _refreshBtn.Enabled = true;
            _statusLabel.Text = $"Found {_features.Count} items ({_features.Count(f => f.State == FeatureState.Enabled || f.State == FeatureState.Installed)} active).";
        }

        private void ApplyFilter()
        {
            _listView.Items.Clear();
            string filter = _filterBox.SelectedItem?.ToString() ?? "All Items";

            var filtered = _features.Where(f =>
            {
                if (filter == "Installed / Enabled Only") return f.State == FeatureState.Enabled || f.State == FeatureState.Installed;
                if (filter == "Disabled Only") return f.State == FeatureState.Disabled || f.State == FeatureState.NotPresent;
                if (filter == "Optional Features") return !f.IsCapability;
                if (filter == "Capabilities (On-Demand)") return f.IsCapability;
                return true;
            }).ToList();

            foreach (var item in filtered)
            {
                var lvi = new ListViewItem(item.DisplayName) { Tag = item };
                lvi.SubItems.Add(item.State.ToString());
                lvi.SubItems.Add(item.IsCapability ? "Capability" : "Feature");
                lvi.SubItems.Add(item.FeatureName);

                if (item.State == FeatureState.Enabled || item.State == FeatureState.Installed)
                {
                    lvi.ForeColor = Color.FromArgb(0, 120, 60);
                }
                else if (item.State == FeatureState.Disabled || item.State == FeatureState.NotPresent)
                {
                    lvi.ForeColor = Color.Gray;
                }
                else if (item.IsCritical)
                {
                    lvi.ForeColor = Color.FromArgb(0, 100, 180);
                }

                _listView.Items.Add(lvi);
            }
        }

        private async Task SetSelectedStateAsync(bool enable)
        {
            if (_listView.SelectedItems.Count == 0) return;

            var items = _listView.SelectedItems.Cast<ListViewItem>()
                .Select(l => l.Tag as WindowsOptionalFeatureItem)
                .Where(f => f != null)
                .Cast<WindowsOptionalFeatureItem>()
                .ToList();

            string actionName = enable ? "enable / install" : "disable / remove";
            if (MessageBox.Show($"Are you sure you want to {actionName} {items.Count} selected feature(s)?", "Confirm Feature Modification", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            _enableBtn.Enabled = false;
            _disableBtn.Enabled = false;
            _statusLabel.Text = $"Applying changes...";

            int processed = 0;
            foreach (var item in items)
            {
                if (item.IsCritical)
                {
                    MessageBox.Show($"'{item.DisplayName}' is a critical Windows dependency and cannot be disabled.", "Protected Component", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    continue;
                }

                _statusLabel.Text = $"Processing {item.DisplayName}...";
                bool ok = await Task.Run(() => WindowsOptionalFeaturesManager.SetFeatureState(item.FeatureName, enable, item.IsCapability));
                if (ok)
                {
                    processed++;
                    item.State = enable ? (item.IsCapability ? FeatureState.Installed : FeatureState.Enabled)
                                        : (item.IsCapability ? FeatureState.NotPresent : FeatureState.Disabled);
                }
            }

            _enableBtn.Enabled = true;
            _disableBtn.Enabled = true;
            _statusLabel.Text = $"Successfully processed {processed} item(s).";
            ApplyFilter();
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

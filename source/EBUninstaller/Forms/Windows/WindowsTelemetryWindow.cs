/*
 * EBUninstaller Pro - Application Uninstaller & System Optimization Suite
 * Copyright (C) 2026 EBUninstaller Development Team & Contributors
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using UninstallTools.PrivacyCleaner;

namespace BulkCrapUninstaller.Forms
{
    public class WindowsTelemetryWindow : Form
    {
        private ListView _listView;
        private Label _lblStatus;
        private Button _btnScan;
        private Button _btnOptimizeSelected;
        private Button _btnClose;
        private List<TelemetrySettingItem> _settings = new List<TelemetrySettingItem>();

        public WindowsTelemetryWindow()
        {
            InitializeComponents();
            RunScan();
        }

        private void InitializeComponents()
        {
            Text = "EBUninstaller Pro - Windows Privacy & Diagnostic Telemetry Optimizer";
            Size = new Size(950, 560);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(800, 450);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            var topPanel = new Panel { Dock = DockStyle.Top, Height = 50, Padding = new Padding(12, 10, 12, 10) };
            _btnScan = new Button { Text = "Scan Privacy Settings", Location = new Point(12, 9), Width = 150, Height = 28 };
            _btnScan.Click += (s, e) => RunScan();

            _btnOptimizeSelected = new Button { Text = "Optimize Selected...", Location = new Point(170, 9), Width = 150, Height = 28 };
            _btnOptimizeSelected.Click += BtnOptimizeSelected_Click;

            topPanel.Controls.AddRange(new Control[] { _btnScan, _btnOptimizeSelected });

            _listView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                CheckBoxes = true,
                GridLines = true
            };

            _listView.Columns.Add("Privacy / Telemetry Feature", 280);
            _listView.Columns.Add("Category", 140);
            _listView.Columns.Add("Current Status", 130);
            _listView.Columns.Add("Description", 350);

            var btmPanel = new Panel { Dock = DockStyle.Bottom, Height = 45, Padding = new Padding(12, 8, 12, 8) };
            _lblStatus = new Label { Dock = DockStyle.Left, AutoSize = true, Text = "Ready.", Location = new Point(12, 12) };
            _btnClose = new Button { Text = "Close", Dock = DockStyle.Right, Width = 90, DialogResult = DialogResult.OK };
            btmPanel.Controls.AddRange(new Control[] { _lblStatus, _btnClose });

            Controls.Add(_listView);
            Controls.Add(topPanel);
            Controls.Add(btmPanel);
        }

        private void RunScan()
        {
            _lblStatus.Text = "Scanning Windows telemetry and diagnostic tracking policies...";
            _listView.Items.Clear();

            try
            {
                _settings = WindowsTelemetryOptimizer.ScanTelemetrySettings();
                int optimizedCount = 0;

                foreach (var s in _settings)
                {
                    if (s.IsOptimized) optimizedCount++;
                    var lvi = new ListViewItem(s.Name) { Checked = !s.IsOptimized };
                    lvi.SubItems.Add(s.Category.ToString());
                    lvi.SubItems.Add(s.IsOptimized ? "Optimized (Protected)" : "Default (Active Tracking)");
                    lvi.SubItems.Add(s.Description);

                    if (!s.IsOptimized)
                    {
                        lvi.ForeColor = Color.DarkOrange;
                    }

                    _listView.Items.Add(lvi);
                }

                _lblStatus.Text = $"Found {_settings.Count} privacy settings ({optimizedCount} already optimized).";
            }
            catch (Exception ex)
            {
                _lblStatus.Text = $"Scan failed: {ex.Message}";
            }
        }

        private void BtnOptimizeSelected_Click(object sender, EventArgs e)
        {
            var selected = new List<TelemetrySettingItem>();
            for (int i = 0; i < _listView.Items.Count; i++)
            {
                if (_listView.Items[i].Checked && i < _settings.Count)
                {
                    selected.Add(_settings[i]);
                }
            }

            if (!selected.Any())
            {
                MessageBox.Show(this, "Please select at least one privacy setting to optimize.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show(this, $"Optimize {selected.Count} privacy & telemetry settings?\nA registry backup will be created automatically.", "Confirm Privacy Optimization", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var backupDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "EBUninstaller", "Backups");
                var applied = WindowsTelemetryOptimizer.ApplyOptimizations(selected, backupDir);

                MessageBox.Show(this, $"Applied {applied} privacy optimizations successfully.", "Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RunScan();
            }
        }
    }
}

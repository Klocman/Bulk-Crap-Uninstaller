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
using UninstallTools.RegistryEngine;

namespace BulkCrapUninstaller.Forms
{
    public class RegistryBloatWindow : Form
    {
        private ListView _listView;
        private Label _lblStatus;
        private Button _btnScan;
        private Button _btnCleanSelected;
        private Button _btnClose;
        private RegistryBloatScanResult _scanResult = new RegistryBloatScanResult();

        public RegistryBloatWindow()
        {
            InitializeComponents();
            RunScan();
        }

        private void InitializeComponents()
        {
            Text = "EBUninstaller Pro - Registry Bloat & Stale Associations Cleaner";
            Size = new Size(950, 560);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(800, 450);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            var topPanel = new Panel { Dock = DockStyle.Top, Height = 50, Padding = new Padding(12, 10, 12, 10) };
            _btnScan = new Button { Text = "Scan Registry", Location = new Point(12, 9), Width = 110, Height = 28 };
            _btnScan.Click += (s, e) => RunScan();

            _btnCleanSelected = new Button { Text = "Clean Selected...", Location = new Point(130, 9), Width = 140, Height = 28 };
            _btnCleanSelected.Click += BtnCleanSelected_Click;

            topPanel.Controls.AddRange(new Control[] { _btnScan, _btnCleanSelected });

            _listView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                CheckBoxes = true,
                GridLines = true
            };

            _listView.Columns.Add("Category", 140);
            _listView.Columns.Add("Registry Location", 300);
            _listView.Columns.Add("Missing Target Path", 280);
            _listView.Columns.Add("Reason", 180);

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
            _lblStatus.Text = "Scanning registry for orphaned CLSIDs, App Paths, and invalid SharedDLLs...";
            _listView.Items.Clear();

            try
            {
                _scanResult = RegistryBloatAnalyzer.ScanAllBloat();
                foreach (var item in _scanResult.Items)
                {
                    var lvi = new ListViewItem(item.Category.ToString()) { Checked = true };
                    lvi.SubItems.Add(item.FullRegistryPath);
                    lvi.SubItems.Add(item.TargetPath);
                    lvi.SubItems.Add(item.Reason);
                    _listView.Items.Add(lvi);
                }

                _lblStatus.Text = $"Detected {_scanResult.TotalCount} registry bloat entries in {_scanResult.Duration.TotalSeconds:F2}s.";
            }
            catch (Exception ex)
            {
                _lblStatus.Text = $"Scan failed: {ex.Message}";
            }
        }

        private void BtnCleanSelected_Click(object sender, EventArgs e)
        {
            var selected = new List<RegistryBloatItem>();
            for (int i = 0; i < _listView.Items.Count; i++)
            {
                if (_listView.Items[i].Checked && i < _scanResult.Items.Count)
                {
                    selected.Add(_scanResult.Items[i]);
                }
            }

            if (!selected.Any())
            {
                MessageBox.Show(this, "Please select at least one registry item to clean.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show(this, $"Are you sure you want to clean {selected.Count} registry bloat entries?\nA registry backup will be created automatically.", "Confirm Registry Cleanup", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var backupDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "EBUninstaller", "Backups");
                var cleaned = RegistryBloatAnalyzer.CleanBloatItems(selected, backupDir);

                MessageBox.Show(this, $"Successfully cleaned {cleaned} registry bloat entries.", "Cleanup Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RunScan();
            }
        }
    }
}

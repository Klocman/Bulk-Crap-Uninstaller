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
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using UninstallTools.JunkCleaner;

namespace BulkCrapUninstaller.Forms
{
    public class WinUpdateResidualsWindow : Form
    {
        private ListView _listView;
        private Label _lblStatus;
        private Button _btnScan;
        private Button _btnCleanSelected;
        private Button _btnDismCleanup;
        private Button _btnClose;
        private List<WinUpdateResidualItem> _items = new List<WinUpdateResidualItem>();

        public WinUpdateResidualsWindow()
        {
            InitializeComponents();
            RunScan();
        }

        private void InitializeComponents()
        {
            Text = "EBUninstaller Pro - Windows Update & Component Store Cleaner";
            Size = new Size(950, 560);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(800, 450);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            var topPanel = new Panel { Dock = DockStyle.Top, Height = 50, Padding = new Padding(12, 10, 12, 10) };
            _btnScan = new Button { Text = "Scan Caches", Location = new Point(12, 9), Width = 110, Height = 28 };
            _btnScan.Click += (s, e) => RunScan();

            _btnCleanSelected = new Button { Text = "Clean Selected Caches", Location = new Point(130, 9), Width = 160, Height = 28 };
            _btnCleanSelected.Click += BtnCleanSelected_Click;

            _btnDismCleanup = new Button { Text = "Run WinSxS DISM Cleanup...", Location = new Point(298, 9), Width = 190, Height = 28 };
            _btnDismCleanup.Click += BtnDismCleanup_Click;

            topPanel.Controls.AddRange(new Control[] { _btnScan, _btnCleanSelected, _btnDismCleanup });

            _listView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                CheckBoxes = true,
                GridLines = true
            };

            _listView.Columns.Add("Cache / Residual Category", 280);
            _listView.Columns.Add("Files", 70);
            _listView.Columns.Add("Size", 90);
            _listView.Columns.Add("Target Path", 320);
            _listView.Columns.Add("Description", 240);

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
            _lblStatus.Text = "Scanning Windows Update and Upgrade residual stores...";
            _listView.Items.Clear();

            try
            {
                _items = WinUpdateResidualsCleaner.ScanResiduals();
                long totalBytes = 0;

                foreach (var item in _items)
                {
                    totalBytes += item.TotalSizeBytes;
                    var lvi = new ListViewItem(item.Title) { Checked = true };
                    lvi.SubItems.Add(item.FileCount.ToString());
                    lvi.SubItems.Add(FormatSize(item.TotalSizeBytes));
                    lvi.SubItems.Add(item.TargetDirectoryPath);
                    lvi.SubItems.Add(item.Description);
                    _listView.Items.Add(lvi);
                }

                _lblStatus.Text = $"Found {_items.Count} update residual stores ({FormatSize(totalBytes)} total).";
            }
            catch (Exception ex)
            {
                _lblStatus.Text = $"Scan failed: {ex.Message}";
            }
        }

        private void BtnCleanSelected_Click(object sender, EventArgs e)
        {
            var selected = new List<WinUpdateResidualItem>();
            for (int i = 0; i < _listView.Items.Count; i++)
            {
                if (_listView.Items[i].Checked && i < _items.Count)
                {
                    selected.Add(_items[i]);
                }
            }

            if (!selected.Any())
            {
                MessageBox.Show(this, "Please select at least one cache category to clean.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show(this, $"Are you sure you want to clean {selected.Count} update cache directories?", "Confirm Cleanup", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var res = WinUpdateResidualsCleaner.CleanResiduals(selected);
                MessageBox.Show(this, $"Cleaned {res.DeletedFilesCount} files in {res.Duration.TotalSeconds:F1}s ({FormatSize(res.BytesFreed)} freed).", "Cleanup Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RunScan();
            }
        }

        private void BtnDismCleanup_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "Launch Windows DISM Component Store cleanup in an elevated console?\nThis will consolidate superseded Windows Updates in WinSxS.", "Run DISM Cleanup", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                try
                {
                    var psi = WinUpdateResidualsCleaner.GetDismComponentCleanupStartInfo(false);
                    Process.Start(psi);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, $"Failed to launch DISM: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private static string FormatSize(long bytes)
        {
            if (bytes <= 0) return "0 MB";
            if (bytes >= 1024L * 1024 * 1024)
                return $"{(bytes / (1024.0 * 1024 * 1024)):F2} GB";
            return $"{(bytes / (1024.0 * 1024)):F1} MB";
        }
    }
}

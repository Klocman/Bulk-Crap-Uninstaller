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
    public class FileAssociationsWindow : Form
    {
        private ListView _listView;
        private Label _lblStatus;
        private Button _btnScan;
        private Button _btnCleanOrphaned;
        private Button _btnClose;
        private List<FileAssociationItem> _items = new List<FileAssociationItem>();

        public FileAssociationsWindow()
        {
            InitializeComponents();
            RunScan();
        }

        private void InitializeComponents()
        {
            Text = "EBUninstaller Pro - File Associations & Default Apps Cleaner";
            Size = new Size(950, 560);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(800, 450);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            var topPanel = new Panel { Dock = DockStyle.Top, Height = 50, Padding = new Padding(12, 10, 12, 10) };
            _btnScan = new Button { Text = "Scan Associations", Location = new Point(12, 9), Width = 140, Height = 28 };
            _btnScan.Click += (s, e) => RunScan();

            _btnCleanOrphaned = new Button { Text = "Clean Selected Orphaned...", Location = new Point(160, 9), Width = 190, Height = 28 };
            _btnCleanOrphaned.Click += BtnCleanOrphaned_Click;

            topPanel.Controls.AddRange(new Control[] { _btnScan, _btnCleanOrphaned });

            _listView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                CheckBoxes = true,
                GridLines = true
            };

            _listView.Columns.Add("Extension", 90);
            _listView.Columns.Add("ProgID", 200);
            _listView.Columns.Add("Executable Target Path", 380);
            _listView.Columns.Add("Status", 120);

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
            _lblStatus.Text = "Scanning registered file associations in registry...";
            _listView.Items.Clear();

            try
            {
                _items = FileAssociationsCleaner.ScanFileAssociations();
                int orphaned = 0;

                foreach (var item in _items)
                {
                    if (item.IsOrphaned) orphaned++;
                    var lvi = new ListViewItem(item.Extension) { Checked = item.IsOrphaned };
                    lvi.SubItems.Add(item.ProgId);
                    lvi.SubItems.Add(item.TargetExecutablePath);
                    lvi.SubItems.Add(item.IsOrphaned ? "Dead Application" : "Valid");

                    if (item.IsOrphaned)
                    {
                        lvi.ForeColor = Color.OrangeRed;
                    }

                    _listView.Items.Add(lvi);
                }

                _lblStatus.Text = $"Scanned {_items.Count} file associations ({orphaned} dead associations found).";
            }
            catch (Exception ex)
            {
                _lblStatus.Text = $"Scan failed: {ex.Message}";
            }
        }

        private void BtnCleanOrphaned_Click(object sender, EventArgs e)
        {
            var selected = new List<FileAssociationItem>();
            for (int i = 0; i < _listView.Items.Count; i++)
            {
                if (_listView.Items[i].Checked && i < _items.Count)
                {
                    selected.Add(_items[i]);
                }
            }

            if (!selected.Any())
            {
                MessageBox.Show(this, "Please select at least one orphaned file association to clean.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show(this, $"Are you sure you want to clean {selected.Count} dead file associations?\nA registry backup will be created automatically.", "Confirm Cleanup", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var backupDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "EBUninstaller", "Backups");
                var cleaned = FileAssociationsCleaner.CleanOrphanedAssociations(selected, backupDir);

                MessageBox.Show(this, $"Cleaned {cleaned} dead file associations.", "Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RunScan();
            }
        }
    }
}

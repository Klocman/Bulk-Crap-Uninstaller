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
using UninstallTools.JunkCleaner;

namespace BulkCrapUninstaller.Forms
{
    public class OrphanedServicesWindow : Form
    {
        private ListView _listView;
        private Label _lblStatus;
        private Button _btnRefresh;
        private Button _btnRemoveSelected;
        private Button _btnClose;
        private List<OrphanedServiceItem> _services = new List<OrphanedServiceItem>();

        public OrphanedServicesWindow()
        {
            InitializeComponents();
            LoadServices();
        }

        private void InitializeComponents()
        {
            Text = "EBUninstaller Pro - Orphaned & Broken Services Cleaner";
            Size = new Size(950, 560);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(800, 450);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            var topPanel = new Panel { Dock = DockStyle.Top, Height = 50, Padding = new Padding(12, 10, 12, 10) };
            _btnRefresh = new Button { Text = "Scan Services", Location = new Point(12, 9), Width = 110, Height = 28 };
            _btnRefresh.Click += (s, e) => LoadServices();

            _btnRemoveSelected = new Button { Text = "Remove Orphaned...", Location = new Point(130, 9), Width = 150, Height = 28 };
            _btnRemoveSelected.Click += BtnRemoveSelected_Click;

            topPanel.Controls.AddRange(new Control[] { _btnRefresh, _btnRemoveSelected });

            _listView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                CheckBoxes = true,
                GridLines = true
            };

            _listView.Columns.Add("Service Name", 160);
            _listView.Columns.Add("Display Name", 200);
            _listView.Columns.Add("Missing Image Path", 320);
            _listView.Columns.Add("Startup Type", 100);
            _listView.Columns.Add("Status", 100);

            var btmPanel = new Panel { Dock = DockStyle.Bottom, Height = 45, Padding = new Padding(12, 8, 12, 8) };
            _lblStatus = new Label { Dock = DockStyle.Left, AutoSize = true, Text = "Ready.", Location = new Point(12, 12) };
            _btnClose = new Button { Text = "Close", Dock = DockStyle.Right, Width = 90, DialogResult = DialogResult.OK };
            btmPanel.Controls.AddRange(new Control[] { _lblStatus, _btnClose });

            Controls.Add(_listView);
            Controls.Add(topPanel);
            Controls.Add(btmPanel);
        }

        private void LoadServices()
        {
            _lblStatus.Text = "Scanning Windows services for missing binary paths...";
            _listView.Items.Clear();

            try
            {
                _services = OrphanedServicesCleaner.ScanOrphanedServices();
                foreach (var s in _services)
                {
                    var item = new ListViewItem(s.ServiceName) { Checked = !s.IsProtected };
                    item.SubItems.Add(s.DisplayName);
                    item.SubItems.Add(s.ParsedExecutablePath);
                    item.SubItems.Add(s.StartTypeName);
                    item.SubItems.Add(s.IsProtected ? "Protected System Service" : "Orphaned Binary");

                    if (s.IsProtected)
                    {
                        item.ForeColor = Color.Gray;
                    }

                    _listView.Items.Add(item);
                }

                _lblStatus.Text = $"Found {_services.Count} orphaned service definitions.";
            }
            catch (Exception ex)
            {
                _lblStatus.Text = $"Scan error: {ex.Message}";
            }
        }

        private void BtnRemoveSelected_Click(object sender, EventArgs e)
        {
            var selected = new List<OrphanedServiceItem>();
            for (int i = 0; i < _listView.Items.Count; i++)
            {
                if (_listView.Items[i].Checked && i < _services.Count)
                {
                    selected.Add(_services[i]);
                }
            }

            if (!selected.Any())
            {
                MessageBox.Show(this, "Please select at least one orphaned service to remove.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show(this, $"Are you sure you want to remove {selected.Count} orphaned service(s)?\nA registry backup will be created automatically.", "Confirm Service Cleanup", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                var backupDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "EBUninstaller", "Backups");
                var res = OrphanedServicesCleaner.RemoveOrphanedServices(selected, backupDir);

                if (res.Success)
                {
                    MessageBox.Show(this, $"Removed {res.CleanedCount} orphaned services.\nBackup saved to: {res.BackupRegPath}", "Cleanup Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadServices();
                }
                else
                {
                    MessageBox.Show(this, $"Service removal encountered errors: {string.Join("\n", res.Errors)}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}

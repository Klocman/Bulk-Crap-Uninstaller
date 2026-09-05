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
using UninstallTools.Detection;

namespace BulkCrapUninstaller.Forms
{
    public class PackageManagerSyncWindow : Form
    {
        private ListView _listView;
        private Label _lblStatus;
        private Button _btnScan;
        private Button _btnExportWinget;
        private Button _btnExportScript;
        private Button _btnClose;
        private List<SyncAppItem> _apps = new List<SyncAppItem>();

        public PackageManagerSyncWindow()
        {
            InitializeComponents();
            RunScan();
        }

        private void InitializeComponents()
        {
            Text = "EBUninstaller Pro - Windows Package Manager (Winget / Chocolatey) Sync & Bundle Exporter";
            Size = new Size(950, 560);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(800, 450);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            var topPanel = new Panel { Dock = DockStyle.Top, Height = 50, Padding = new Padding(12, 10, 12, 10) };
            _btnScan = new Button { Text = "Check Upgrades", Location = new Point(12, 9), Width = 130, Height = 28 };
            _btnScan.Click += (s, e) => RunScan();

            _btnExportWinget = new Button { Text = "Export Winget JSON...", Location = new Point(150, 9), Width = 160, Height = 28 };
            _btnExportWinget.Click += BtnExportWinget_Click;

            _btnExportScript = new Button { Text = "Export Install Script (.ps1)...", Location = new Point(318, 9), Width = 190, Height = 28 };
            _btnExportScript.Click += BtnExportScript_Click;

            topPanel.Controls.AddRange(new Control[] { _btnScan, _btnExportWinget, _btnExportScript });

            _listView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                CheckBoxes = true,
                GridLines = true
            };

            _listView.Columns.Add("Application Name", 260);
            _listView.Columns.Add("Package ID", 240);
            _listView.Columns.Add("Source", 100);
            _listView.Columns.Add("Installed Ver", 110);
            _listView.Columns.Add("Available Ver", 110);
            _listView.Columns.Add("Status", 100);

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
            _lblStatus.Text = "Scanning package managers for available package upgrades...";
            _listView.Items.Clear();

            try
            {
                _apps = PackageManagerSyncEngine.ScanUpgradablePackages();
                foreach (var a in _apps)
                {
                    var lvi = new ListViewItem(a.DisplayName) { Checked = true };
                    lvi.SubItems.Add(a.PackageId);
                    lvi.SubItems.Add(a.ManagerType.ToString());
                    lvi.SubItems.Add(a.InstalledVersion);
                    lvi.SubItems.Add(a.AvailableVersion);
                    lvi.SubItems.Add(a.CanUpgrade ? "Update Available" : "Up to date");

                    if (a.CanUpgrade)
                    {
                        lvi.ForeColor = Color.ForestGreen;
                    }

                    _listView.Items.Add(lvi);
                }

                _lblStatus.Text = $"Discovered {_apps.Count} package manager packages.";
            }
            catch (Exception ex)
            {
                _lblStatus.Text = $"Scan failed: {ex.Message}";
            }
        }

        private void BtnExportWinget_Click(object sender, EventArgs e)
        {
            using var sfd = new SaveFileDialog
            {
                Filter = "JSON Files (*.json)|*.json",
                FileName = $"winget_bundle_{DateTime.Now:yyyyMMdd}.json"
            };

            if (sfd.ShowDialog(this) == DialogResult.OK)
            {
                var json = PackageManagerSyncEngine.GenerateWingetExportJson(_apps);
                File.WriteAllText(sfd.FileName, json);
                MessageBox.Show(this, "Winget manifest exported successfully.", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnExportScript_Click(object sender, EventArgs e)
        {
            using var sfd = new SaveFileDialog
            {
                Filter = "PowerShell Scripts (*.ps1)|*.ps1",
                FileName = $"Install_Application_Bundle_{DateTime.Now:yyyyMMdd}.ps1"
            };

            if (sfd.ShowDialog(this) == DialogResult.OK)
            {
                var script = PackageManagerSyncEngine.GeneratePowerShellReinstallScript(_apps);
                File.WriteAllText(sfd.FileName, script);
                MessageBox.Show(this, "PowerShell installation bundle script exported successfully.", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}

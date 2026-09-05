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
using System.Linq;
using System.Windows.Forms;
using UninstallTools.StoreApps;

namespace BulkCrapUninstaller.Forms
{
    public class StoreAppDeprovisionerWindow : Form
    {
        private ListView _listView;
        private Label _lblStatus;
        private Button _btnScan;
        private Button _btnDeprovisionSelected;
        private Button _btnClose;
        private List<ProvisionedAppPackage> _packages = new List<ProvisionedAppPackage>();

        public StoreAppDeprovisionerWindow()
        {
            InitializeComponents();
            RunScan();
        }

        private void InitializeComponents()
        {
            Text = "EBUninstaller Pro - Windows AppX / MSIX OS Image Deprovisioner";
            Size = new Size(950, 560);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(800, 450);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            var topPanel = new Panel { Dock = DockStyle.Top, Height = 50, Padding = new Padding(12, 10, 12, 10) };
            _btnScan = new Button { Text = "Scan Provisioned Apps", Location = new Point(12, 9), Width = 160, Height = 28 };
            _btnScan.Click += (s, e) => RunScan();

            _btnDeprovisionSelected = new Button { Text = "Deprovision Selected...", Location = new Point(180, 9), Width = 170, Height = 28 };
            _btnDeprovisionSelected.Click += BtnDeprovisionSelected_Click;

            topPanel.Controls.AddRange(new Control[] { _btnScan, _btnDeprovisionSelected });

            _listView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                CheckBoxes = true,
                GridLines = true
            };

            _listView.Columns.Add("Display Name", 240);
            _listView.Columns.Add("Package Full Name", 380);
            _listView.Columns.Add("Version", 110);
            _listView.Columns.Add("Arch", 70);
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
            _lblStatus.Text = "Querying staged and provisioned AppX packages in OS image via DISM...";
            _listView.Items.Clear();

            try
            {
                _packages = StoreAppDeprovisioner.GetProvisionedPackages();
                foreach (var p in _packages)
                {
                    var lvi = new ListViewItem(p.DisplayName) { Checked = !p.IsSystemCritical };
                    lvi.SubItems.Add(p.PackageName);
                    lvi.SubItems.Add(p.Version);
                    lvi.SubItems.Add(p.Architecture);
                    lvi.SubItems.Add(p.IsSystemCritical ? "Protected System App" : "Staged Bloatware");

                    if (p.IsSystemCritical)
                    {
                        lvi.ForeColor = Color.Gray;
                    }

                    _listView.Items.Add(lvi);
                }

                _lblStatus.Text = $"Found {_packages.Count} provisioned package(s) staged in OS image.";
            }
            catch (Exception ex)
            {
                _lblStatus.Text = $"Scan failed: {ex.Message}";
            }
        }

        private void BtnDeprovisionSelected_Click(object sender, EventArgs e)
        {
            var selected = new List<ProvisionedAppPackage>();
            for (int i = 0; i < _listView.Items.Count; i++)
            {
                if (_listView.Items[i].Checked && i < _packages.Count)
                {
                    selected.Add(_packages[i]);
                }
            }

            if (!selected.Any())
            {
                MessageBox.Show(this, "Please select at least one provisioned package to deprovision.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show(this, $"Are you sure you want to deprovision {selected.Count} package(s) from the Windows OS image?\nThese packages will no longer be installed for newly created Windows user accounts.", "Confirm Deprovisioning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                int deprovisioned = 0;
                foreach (var p in selected)
                {
                    _lblStatus.Text = $"Deprovisioning {p.DisplayName}...";
                    Application.DoEvents();

                    if (StoreAppDeprovisioner.DeprovisionPackage(p.PackageName))
                    {
                        deprovisioned++;
                    }
                }

                MessageBox.Show(this, $"Deprovisioned {deprovisioned}/{selected.Count} packages from the OS image.", "Deprovisioning Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RunScan();
            }
        }
    }
}

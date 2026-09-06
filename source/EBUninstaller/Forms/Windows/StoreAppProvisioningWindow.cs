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
using System.Drawing;
using System.Windows.Forms;
using UninstallTools.StoreApps;

namespace BulkCrapUninstaller.Forms.Windows
{
    public class StoreAppProvisioningWindow : Form
    {
        private ListView _listView;
        private Button _btnRefresh;
        private Button _btnClose;
        private Label _lblSummary;

        public StoreAppProvisioningWindow()
        {
            InitializeComponent();
            RefreshList();
        }

        private void InitializeComponent()
        {
            Text = "Store App Provisioning & Staging Analyzer - EBUninstaller Pro";
            Size = new Size(920, 520);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(750, 420);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            var topPanel = new Panel { Dock = DockStyle.Top, Height = 55, Padding = new Padding(12) };
            var bottomPanel = new Panel { Dock = DockStyle.Bottom, Height = 55, Padding = new Padding(12) };

            _lblSummary = new Label
            {
                Text = "Auditing all-user staged and provisioned AppX/MSIX packages.",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Regular)
            };
            topPanel.Controls.Add(_lblSummary);

            _listView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true
            };

            _listView.Columns.Add("Package Display Name", 260);
            _listView.Columns.Add("Version", 120);
            _listView.Columns.Add("Publisher", 180);
            _listView.Columns.Add("Estimated Size", 110);
            _listView.Columns.Add("System Critical", 110);

            _btnRefresh = new Button { Text = "Refresh", Width = 100, Dock = DockStyle.Left };
            _btnRefresh.Click += (s, e) => RefreshList();

            _btnClose = new Button { Text = "Close", Width = 90, Dock = DockStyle.Right, DialogResult = DialogResult.OK };

            bottomPanel.Controls.Add(_btnClose);
            bottomPanel.Controls.Add(_btnRefresh);

            Controls.Add(_listView);
            Controls.Add(topPanel);
            Controls.Add(bottomPanel);
        }

        private void RefreshList()
        {
            _listView.Items.Clear();
            var packages = StoreAppProvisioningAnalyzer.ScanProvisionedPackages();

            foreach (var p in packages)
            {
                var lvi = new ListViewItem(p.DisplayName);
                lvi.SubItems.Add(p.Version);
                lvi.SubItems.Add(p.Publisher);
                lvi.SubItems.Add(p.EstimatedSizeBytes > 0 ? (p.EstimatedSizeBytes / (1024 * 1024)) + " MB" : "-");
                lvi.SubItems.Add(p.IsSystemCritical ? "Yes (Protected)" : "No");

                if (p.IsSystemCritical)
                {
                    lvi.ForeColor = Color.DarkSlateGray;
                }

                _listView.Items.Add(lvi);
            }

            _lblSummary.Text = "Found " + packages.Count + " provisioned AppX/MSIX packages in Windows AllUserStore.";
        }
    }
}

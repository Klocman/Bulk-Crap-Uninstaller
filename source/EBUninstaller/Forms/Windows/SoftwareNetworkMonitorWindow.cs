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
using UninstallTools.Detection;

namespace BulkCrapUninstaller.Forms.Windows
{
    public class SoftwareNetworkMonitorWindow : Form
    {
        private ListView _listView;
        private Button _btnRefresh;
        private Button _btnClose;
        private Label _lblSummary;

        public SoftwareNetworkMonitorWindow()
        {
            InitializeComponent();
            RefreshConnections();
        }

        private void InitializeComponent()
        {
            Text = "Software Network Sockets & Telemetry Monitor - EBUninstaller Pro";
            Size = new Size(920, 520);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(760, 420);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            var topPanel = new Panel { Dock = DockStyle.Top, Height = 55, Padding = new Padding(12) };
            var bottomPanel = new Panel { Dock = DockStyle.Bottom, Height = 50, Padding = new Padding(12) };

            _lblSummary = new Label
            {
                Text = "Auditing active network connections and outbound software sockets...",
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

            _listView.Columns.Add("Local Socket Endpoint", 240);
            _listView.Columns.Add("Remote Host / Endpoint", 260);
            _listView.Columns.Add("Protocol", 90);
            _listView.Columns.Add("Socket State", 130);
            _listView.Columns.Add("Associated App / Process", 160);

            _btnRefresh = new Button { Text = "Refresh Sockets", Width = 140, Dock = DockStyle.Left };
            _btnRefresh.Click += (s, e) => RefreshConnections();

            _btnClose = new Button { Text = "Close", Width = 90, Dock = DockStyle.Right, DialogResult = DialogResult.OK };

            bottomPanel.Controls.Add(_btnClose);
            bottomPanel.Controls.Add(_btnRefresh);

            Controls.Add(_listView);
            Controls.Add(topPanel);
            Controls.Add(bottomPanel);
        }

        private void RefreshConnections()
        {
            _listView.Items.Clear();
            var connections = SoftwareNetworkMonitorEngine.GetActiveConnections();

            foreach (var c in connections)
            {
                var lvi = new ListViewItem(c.LocalEndpoint);
                lvi.SubItems.Add(c.RemoteEndpoint);
                lvi.SubItems.Add(c.Protocol);
                lvi.SubItems.Add(c.State);
                lvi.SubItems.Add(c.AssociatedAppName);
                _listView.Items.Add(lvi);
            }

            _lblSummary.Text = "Monitoring " + connections.Count + " active TCP sockets across the system.";
        }
    }
}

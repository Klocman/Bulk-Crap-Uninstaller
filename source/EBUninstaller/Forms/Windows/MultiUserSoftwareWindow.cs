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
    public class MultiUserSoftwareWindow : Form
    {
        private ListView _listView;
        private Button _btnRefresh;
        private Button _btnClose;
        private Label _lblSummary;

        public MultiUserSoftwareWindow()
        {
            InitializeComponent();
            RefreshList();
        }

        private void InitializeComponent()
        {
            Text = "Multi-User Software Installation Matrix - EBUninstaller Pro";
            Size = new Size(950, 520);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(780, 420);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            var topPanel = new Panel { Dock = DockStyle.Top, Height = 55, Padding = new Padding(12) };
            var bottomPanel = new Panel { Dock = DockStyle.Bottom, Height = 50, Padding = new Padding(12) };

            _lblSummary = new Label
            {
                Text = "Auditing per-user software installations across all local Windows user accounts...",
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

            _listView.Columns.Add("Application Name", 260);
            _listView.Columns.Add("Version", 120);
            _listView.Columns.Add("Publisher", 180);
            _listView.Columns.Add("User Account SID", 220);
            _listView.Columns.Add("Install Location", 240);

            _btnRefresh = new Button { Text = "Refresh", Width = 90, Dock = DockStyle.Left };
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
            var list = MultiUserSoftwareMatrixEngine.ScanAllUserProfiles();

            foreach (var item in list)
            {
                var lvi = new ListViewItem(item.ApplicationName);
                lvi.SubItems.Add(item.Version);
                lvi.SubItems.Add(item.Publisher);
                lvi.SubItems.Add(item.UserSid);
                lvi.SubItems.Add(item.InstallLocation);
                _listView.Items.Add(lvi);
            }

            _lblSummary.Text = "Found " + list.Count + " per-user software installations across Windows user profiles.";
        }
    }
}

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
using UninstallTools.Startup;

namespace BulkCrapUninstaller.Forms.Windows
{
    public class ServiceQuarantineWindow : Form
    {
        private ListView _listView;
        private Button _btnRestore;
        private Button _btnDelete;
        private Button _btnClose;
        private Label _lblSummary;

        public ServiceQuarantineWindow()
        {
            InitializeComponent();
            RefreshList();
        }

        private void InitializeComponent()
        {
            Text = "Service Quarantine Vault - EBUninstaller Pro";
            Size = new Size(880, 520);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(750, 420);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            var topPanel = new Panel { Dock = DockStyle.Top, Height = 55, Padding = new Padding(12) };
            var bottomPanel = new Panel { Dock = DockStyle.Bottom, Height = 55, Padding = new Padding(12) };

            _lblSummary = new Label
            {
                Text = "Quarantine vault for disabled or removed background services. Restore services anytime with 1 click.",
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

            _listView.Columns.Add("Service Name", 160);
            _listView.Columns.Add("Display Name", 220);
            _listView.Columns.Add("Binary Image Path", 240);
            _listView.Columns.Add("Quarantined Date (UTC)", 150);

            _btnRestore = new Button { Text = "Restore Selected Service", Width = 180, Dock = DockStyle.Left };
            _btnRestore.Click += (s, e) => RestoreSelected();

            _btnDelete = new Button { Text = "Purge from Vault", Width = 140, Dock = DockStyle.Left };
            _btnDelete.Click += (s, e) => DeleteSelected();

            _btnClose = new Button { Text = "Close", Width = 90, Dock = DockStyle.Right, DialogResult = DialogResult.OK };

            bottomPanel.Controls.Add(_btnClose);
            bottomPanel.Controls.Add(_btnDelete);
            bottomPanel.Controls.Add(_btnRestore);

            Controls.Add(_listView);
            Controls.Add(topPanel);
            Controls.Add(bottomPanel);
        }

        private void RefreshList()
        {
            _listView.Items.Clear();
            var services = ServiceQuarantineEngine.ListQuarantinedServices();

            foreach (var svc in services)
            {
                var lvi = new ListViewItem(svc.ServiceName) { Tag = svc };
                lvi.SubItems.Add(svc.DisplayName);
                lvi.SubItems.Add(svc.ImagePath);
                lvi.SubItems.Add(svc.QuarantineDateUtc.ToString("yyyy-MM-dd HH:mm"));
                _listView.Items.Add(lvi);
            }

            _lblSummary.Text = $"Total {services.Count} service configurations safely preserved in Quarantine Vault.";
        }

        private void RestoreSelected()
        {
            if (_listView.SelectedItems.Count == 0) return;
            var svc = _listView.SelectedItems[0].Tag as QuarantinedServiceRecord;
            if (svc != null)
            {
                if (MessageBox.Show($"Restore service '{svc.DisplayName}' back to Windows Services?", "Confirm Restore", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (ServiceQuarantineEngine.RestoreService(svc.ServiceName))
                    {
                        RefreshList();
                        MessageBox.Show($"Service '{svc.DisplayName}' successfully restored!", "Restored", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        private void DeleteSelected()
        {
            if (_listView.SelectedItems.Count == 0) return;
            var svc = _listView.SelectedItems[0].Tag as QuarantinedServiceRecord;
            if (svc != null)
            {
                if (MessageBox.Show($"Permanently delete quarantine backup for '{svc.ServiceName}'?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    ServiceQuarantineEngine.DeleteQuarantineRecord(svc.ServiceName);
                    RefreshList();
                }
            }
        }
    }
}

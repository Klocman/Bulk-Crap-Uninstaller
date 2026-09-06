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
using UninstallTools.Backup;

namespace BulkCrapUninstaller.Forms.Windows
{
    public class ShadowCopyManagerWindow : Form
    {
        private ListView _listView;
        private Button _btnDeleteSelected;
        private Button _btnPurgeOldest;
        private Button _btnRefresh;
        private Button _btnClose;
        private Label _lblSummary;

        public ShadowCopyManagerWindow()
        {
            InitializeComponent();
            RefreshList();
        }

        private void InitializeComponent()
        {
            Text = "Volume Shadow Copy & VSS Storage Manager - EBUninstaller Pro";
            Size = new Size(880, 500);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(720, 400);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            var topPanel = new Panel { Dock = DockStyle.Top, Height = 55, Padding = new Padding(12) };
            var bottomPanel = new Panel { Dock = DockStyle.Bottom, Height = 55, Padding = new Padding(12) };

            _lblSummary = new Label
            {
                Text = "Manage Windows Volume Shadow Copies (VSS) and reclaim gigabytes of hidden shadow storage.",
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

            _listView.Columns.Add("Shadow Copy ID", 280);
            _listView.Columns.Add("Volume", 100);
            _listView.Columns.Add("Creation Timestamp (UTC)", 180);
            _listView.Columns.Add("Device Object", 260);

            _btnDeleteSelected = new Button { Text = "Delete Selected", Width = 130, Dock = DockStyle.Left };
            _btnDeleteSelected.Click += (s, e) => DeleteSelected();

            _btnPurgeOldest = new Button { Text = "Purge Old (Keep 3)", Width = 150, Dock = DockStyle.Left };
            _btnPurgeOldest.Click += (s, e) => PurgeOldest();

            _btnRefresh = new Button { Text = "Refresh", Width = 90, Dock = DockStyle.Left };
            _btnRefresh.Click += (s, e) => RefreshList();

            _btnClose = new Button { Text = "Close", Width = 90, Dock = DockStyle.Right, DialogResult = DialogResult.OK };

            bottomPanel.Controls.Add(_btnClose);
            bottomPanel.Controls.Add(_btnRefresh);
            bottomPanel.Controls.Add(_btnPurgeOldest);
            bottomPanel.Controls.Add(_btnDeleteSelected);

            Controls.Add(_listView);
            Controls.Add(topPanel);
            Controls.Add(bottomPanel);
        }

        private void RefreshList()
        {
            _listView.Items.Clear();
            var list = VolumeShadowCopyManager.GetShadowCopies();

            foreach (var s in list)
            {
                var lvi = new ListViewItem(s.ShadowCopyId) { Tag = s };
                lvi.SubItems.Add(s.VolumeName);
                lvi.SubItems.Add(s.CreationTimeUtc.ToString("yyyy-MM-dd HH:mm:ss"));
                lvi.SubItems.Add(s.DeviceObject);
                _listView.Items.Add(lvi);
            }

            _lblSummary.Text = "Found " + list.Count + " Volume Shadow Copies preserved on system storage.";
        }

        private void DeleteSelected()
        {
            if (_listView.SelectedItems.Count == 0) return;
            var item = _listView.SelectedItems[0].Tag as ShadowCopyRecord;
            if (item != null)
            {
                if (MessageBox.Show("Delete selected Shadow Copy snapshot (" + item.ShadowCopyId + ")?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    VolumeShadowCopyManager.DeleteShadowCopy(item.ShadowCopyId);
                    RefreshList();
                }
            }
        }

        private void PurgeOldest()
        {
            if (MessageBox.Show("Purge older shadow copies while retaining the 3 most recent snapshots?", "Confirm Purge", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                int purged = VolumeShadowCopyManager.PurgeOldestShadowCopies(3);
                RefreshList();
                MessageBox.Show("Purged " + purged + " older shadow copies.", "Purge Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}

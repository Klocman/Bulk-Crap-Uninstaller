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
using System.Linq;
using System.Windows.Forms;
using UninstallTools.Startup;

namespace BulkCrapUninstaller.Forms.Windows
{
    public class StartupDelayWindow : Form
    {
        private ListView _listView;
        private Button _btnAddDelay;
        private Button _btnRemoveDelay;
        private Button _btnClose;
        private NumericUpDown _numDelaySeconds;
        private Label _lblInfo;

        public StartupDelayWindow()
        {
            InitializeComponent();
            RefreshList();
        }

        private void InitializeComponent()
        {
            Text = "Startup Latency & Delay Optimizer - EBUninstaller Pro";
            Size = new Size(780, 480);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(650, 400);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            var topPanel = new Panel { Dock = DockStyle.Top, Height = 55, Padding = new Padding(12) };
            var bottomPanel = new Panel { Dock = DockStyle.Bottom, Height = 55, Padding = new Padding(12) };

            _lblInfo = new Label
            {
                Text = "Delay non-critical startup applications to accelerate Windows desktop login speed.",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Regular)
            };
            topPanel.Controls.Add(_lblInfo);

            _listView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true
            };

            _listView.Columns.Add("Application / Task Name", 220);
            _listView.Columns.Add("Executable Target", 260);
            _listView.Columns.Add("Configured Delay", 120);
            _listView.Columns.Add("Status", 100);

            var lblDelay = new Label { Text = "Delay (sec):", Width = 75, Dock = DockStyle.Left, TextAlign = ContentAlignment.MiddleRight };
            _numDelaySeconds = new NumericUpDown { Minimum = 5, Maximum = 300, Value = 30, Width = 60, Dock = DockStyle.Left };

            _btnAddDelay = new Button { Text = "Add Custom Delay...", Width = 150, Dock = DockStyle.Left };
            _btnAddDelay.Click += (s, e) => AddDelayDialog();

            _btnRemoveDelay = new Button { Text = "Remove Delay", Width = 120, Dock = DockStyle.Left };
            _btnRemoveDelay.Click += (s, e) => RemoveSelected();

            _btnClose = new Button { Text = "Close", Width = 90, Dock = DockStyle.Right, DialogResult = DialogResult.OK };

            bottomPanel.Controls.Add(_btnClose);
            bottomPanel.Controls.Add(_btnRemoveDelay);
            bottomPanel.Controls.Add(_btnAddDelay);
            bottomPanel.Controls.Add(_numDelaySeconds);
            bottomPanel.Controls.Add(lblDelay);

            Controls.Add(_listView);
            Controls.Add(topPanel);
            Controls.Add(bottomPanel);
        }

        private void RefreshList()
        {
            _listView.Items.Clear();
            var items = StartupDelayOptimizer.GetDelayedStartupItems();

            foreach (var item in items)
            {
                var lvi = new ListViewItem(item.EntryName) { Tag = item };
                lvi.SubItems.Add(item.ExecutablePath);
                lvi.SubItems.Add($"{item.DelaySeconds} seconds");
                lvi.SubItems.Add(item.IsEnabled ? "Active" : "Disabled");
                _listView.Items.Add(lvi);
            }
        }

        private void AddDelayDialog()
        {
            using var ofd = new OpenFileDialog
            {
                Filter = "Executable Files (*.exe)|*.exe|All Files (*.*)|*.*",
                Title = "Select Application to Delay"
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                var appName = System.IO.Path.GetFileNameWithoutExtension(ofd.FileName);
                int delay = (int)_numDelaySeconds.Value;
                if (StartupDelayOptimizer.ConfigureDelay(appName, ofd.FileName, "", delay))
                {
                    RefreshList();
                    MessageBox.Show($"Configured {delay}-second delay trigger for {appName}.", "Delay Configured", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void RemoveSelected()
        {
            if (_listView.SelectedItems.Count == 0) return;
            var item = _listView.SelectedItems[0].Tag as DelayedStartupItem;
            if (item != null)
            {
                StartupDelayOptimizer.RemoveDelay(item.EntryName);
                RefreshList();
            }
        }
    }
}

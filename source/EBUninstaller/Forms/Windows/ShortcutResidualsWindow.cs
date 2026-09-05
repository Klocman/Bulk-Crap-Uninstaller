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
using UninstallTools.JunkCleaner;

namespace BulkCrapUninstaller.Forms
{
    public class ShortcutResidualsWindow : Form
    {
        private ListView _listView;
        private Label _lblStatus;
        private Button _btnScan;
        private Button _btnCleanSelected;
        private Button _btnClose;
        private List<BrokenShortcutItem> _shortcuts = new List<BrokenShortcutItem>();

        public ShortcutResidualsWindow()
        {
            InitializeComponents();
            RunScan();
        }

        private void InitializeComponents()
        {
            Text = "EBUninstaller Pro - Broken Desktop & Start Menu Shortcuts Cleaner";
            Size = new Size(950, 560);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(800, 450);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            var topPanel = new Panel { Dock = DockStyle.Top, Height = 50, Padding = new Padding(12, 10, 12, 10) };
            _btnScan = new Button { Text = "Scan Shortcuts", Location = new Point(12, 9), Width = 130, Height = 28 };
            _btnScan.Click += (s, e) => RunScan();

            _btnCleanSelected = new Button { Text = "Delete Selected Shortcuts...", Location = new Point(150, 9), Width = 180, Height = 28 };
            _btnCleanSelected.Click += BtnCleanSelected_Click;

            topPanel.Controls.AddRange(new Control[] { _btnScan, _btnCleanSelected });

            _listView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                CheckBoxes = true,
                GridLines = true
            };

            _listView.Columns.Add("Shortcut Name", 200);
            _listView.Columns.Add("Location Tier", 160);
            _listView.Columns.Add("Missing Target Binary", 300);
            _listView.Columns.Add(".lnk File Path", 320);

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
            _lblStatus.Text = "Scanning Start Menu, Desktop, and Quick Launch for broken shortcuts...";
            _listView.Items.Clear();

            try
            {
                _shortcuts = ShortcutResidualsCleaner.ScanBrokenShortcuts();
                foreach (var s in _shortcuts)
                {
                    var lvi = new ListViewItem(s.ShortcutName) { Checked = true };
                    lvi.SubItems.Add(s.LocationCategory);
                    lvi.SubItems.Add(s.TargetPath);
                    lvi.SubItems.Add(s.ShortcutPath);
                    _listView.Items.Add(lvi);
                }

                _lblStatus.Text = $"Found {_shortcuts.Count} dead / broken shortcut(s).";
            }
            catch (Exception ex)
            {
                _lblStatus.Text = $"Scan failed: {ex.Message}";
            }
        }

        private void BtnCleanSelected_Click(object sender, EventArgs e)
        {
            var selected = new List<BrokenShortcutItem>();
            for (int i = 0; i < _listView.Items.Count; i++)
            {
                if (_listView.Items[i].Checked && i < _shortcuts.Count)
                {
                    selected.Add(_shortcuts[i]);
                }
            }

            if (!selected.Any())
            {
                MessageBox.Show(this, "Please select at least one broken shortcut to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show(this, $"Are you sure you want to permanently delete {selected.Count} broken shortcut file(s)?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var cleaned = ShortcutResidualsCleaner.CleanShortcuts(selected);
                MessageBox.Show(this, $"Deleted {cleaned} broken shortcut files.", "Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RunScan();
            }
        }
    }
}

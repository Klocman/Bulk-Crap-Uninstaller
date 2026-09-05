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
using UninstallTools.RegistryEngine;

namespace BulkCrapUninstaller.Forms
{
    public class ShellHandlersWindow : Form
    {
        private ListView _listView;
        private Label _lblStatus;
        private Button _btnScan;
        private Button _btnRemoveSelected;
        private Button _btnClose;
        private List<ShellHandlerItem> _handlers = new List<ShellHandlerItem>();

        public ShellHandlersWindow()
        {
            InitializeComponents();
            RunScan();
        }

        private void InitializeComponents()
        {
            Text = "EBUninstaller Pro - Explorer Context Menu Handlers Cleaner";
            Size = new Size(950, 560);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(800, 450);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            var topPanel = new Panel { Dock = DockStyle.Top, Height = 50, Padding = new Padding(12, 10, 12, 10) };
            _btnScan = new Button { Text = "Scan Shell Handlers", Location = new Point(12, 9), Width = 140, Height = 28 };
            _btnScan.Click += (s, e) => RunScan();

            _btnRemoveSelected = new Button { Text = "Remove Selected Orphaned...", Location = new Point(160, 9), Width = 190, Height = 28 };
            _btnRemoveSelected.Click += BtnRemoveSelected_Click;

            topPanel.Controls.AddRange(new Control[] { _btnScan, _btnRemoveSelected });

            _listView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                CheckBoxes = true,
                GridLines = true
            };

            _listView.Columns.Add("Handler Name", 200);
            _listView.Columns.Add("Target Class", 90);
            _listView.Columns.Add("CLSID", 260);
            _listView.Columns.Add("Module DLL Path", 300);
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
            _lblStatus.Text = "Scanning Explorer Context Menu Handlers...";
            _listView.Items.Clear();

            try
            {
                _handlers = ShellHandlersCleaner.ScanShellHandlers();
                int orphanCount = 0;

                foreach (var h in _handlers)
                {
                    if (h.IsOrphaned) orphanCount++;
                    var lvi = new ListViewItem(h.HandlerName) { Checked = h.IsOrphaned };
                    lvi.SubItems.Add(h.TargetClass);
                    lvi.SubItems.Add(h.Clsid);
                    lvi.SubItems.Add(h.ModulePath);
                    lvi.SubItems.Add(h.IsOrphaned ? "Orphaned" : "Active");

                    if (h.IsOrphaned)
                    {
                        lvi.ForeColor = Color.OrangeRed;
                    }

                    _listView.Items.Add(lvi);
                }

                _lblStatus.Text = $"Found {_handlers.Count} context menu handlers ({orphanCount} orphaned).";
            }
            catch (Exception ex)
            {
                _lblStatus.Text = $"Scan failed: {ex.Message}";
            }
        }

        private void BtnRemoveSelected_Click(object sender, EventArgs e)
        {
            var selected = new List<ShellHandlerItem>();
            for (int i = 0; i < _listView.Items.Count; i++)
            {
                if (_listView.Items[i].Checked && i < _handlers.Count)
                {
                    selected.Add(_handlers[i]);
                }
            }

            if (!selected.Any())
            {
                MessageBox.Show(this, "Please select at least one handler to remove.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show(this, $"Are you sure you want to remove {selected.Count} shell handler(s)?\nA registry backup will be created automatically.", "Confirm Removal", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var backupDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "EBUninstaller", "Backups");
                var cleaned = ShellHandlersCleaner.RemoveShellHandlers(selected, backupDir);

                MessageBox.Show(this, $"Removed {cleaned} context menu handler(s).", "Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RunScan();
            }
        }
    }
}

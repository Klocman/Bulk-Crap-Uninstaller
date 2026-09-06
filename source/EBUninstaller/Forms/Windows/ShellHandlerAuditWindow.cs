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
using UninstallTools.RegistryEngine;

namespace BulkCrapUninstaller.Forms.Windows
{
    public class ShellHandlerAuditWindow : Form
    {
        private ListView _listView;
        private Button _btnRefresh;
        private Button _btnClose;
        private Label _lblSummary;

        public ShellHandlerAuditWindow()
        {
            InitializeComponent();
            RefreshList();
        }

        private void InitializeComponent()
        {
            Text = "Explorer Shell Extension & Context Handlers Audit - EBUninstaller Pro";
            Size = new Size(950, 540);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(800, 440);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            var topPanel = new Panel { Dock = DockStyle.Top, Height = 55, Padding = new Padding(12) };
            var bottomPanel = new Panel { Dock = DockStyle.Bottom, Height = 50, Padding = new Padding(12) };

            _lblSummary = new Label
            {
                Text = "Auditing Windows Explorer Context Menu & Shell Extension Handlers...",
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

            _listView.Columns.Add("Handler Name", 220);
            _listView.Columns.Add("Shell Location", 220);
            _listView.Columns.Add("Binary Module Path", 280);
            _listView.Columns.Add("Status on Disk", 120);

            _btnRefresh = new Button { Text = "Refresh Handlers", Width = 150, Dock = DockStyle.Left };
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
            var handlers = ShellHandlerAuditEngine.ScanShellHandlers();

            foreach (var h in handlers)
            {
                var lvi = new ListViewItem(h.HandlerName);
                lvi.SubItems.Add(h.ShellLocation);
                lvi.SubItems.Add(string.IsNullOrEmpty(h.ModulePath) ? h.Clsid : h.ModulePath);
                lvi.SubItems.Add(h.FileExistsOnDisk ? "Present" : "Missing / Orphaned");

                if (!h.FileExistsOnDisk)
                {
                    lvi.BackColor = Color.FromArgb(255, 235, 235);
                    lvi.ForeColor = Color.DarkRed;
                }

                _listView.Items.Add(lvi);
            }

            _lblSummary.Text = "Found " + handlers.Count + " registered Explorer shell context menu handlers.";
        }
    }
}

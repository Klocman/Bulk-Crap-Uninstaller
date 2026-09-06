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
using System.Windows.Forms;
using UninstallTools.SystemTools;

namespace BulkCrapUninstaller.Forms.Windows
{
    public class ProductKeyExtractorWindow : Form
    {
        private ListView _listView;
        private Button _btnExport;
        private Button _btnRefresh;
        private Button _btnClose;
        private Label _lblSummary;
        private List<ProductKeyRecord> _currentKeys = new List<ProductKeyRecord>();

        public ProductKeyExtractorWindow()
        {
            InitializeComponent();
            RefreshKeys();
        }

        private void InitializeComponent()
        {
            Text = "Software License & Product Key Extractor - EBUninstaller Pro";
            Size = new Size(880, 500);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(720, 400);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            var topPanel = new Panel { Dock = DockStyle.Top, Height = 55, Padding = new Padding(12) };
            var bottomPanel = new Panel { Dock = DockStyle.Bottom, Height = 55, Padding = new Padding(12) };

            _lblSummary = new Label
            {
                Text = "Extract and backup software product keys and digital licenses before uninstallation.",
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

            _listView.Columns.Add("Product Name", 240);
            _listView.Columns.Add("Product Key", 230);
            _listView.Columns.Add("Product ID", 180);
            _listView.Columns.Add("Publisher", 160);

            _btnExport = new Button { Text = "Export Keys (.json)...", Width = 160, Dock = DockStyle.Left };
            _btnExport.Click += (s, e) => ExportKeys();

            _btnRefresh = new Button { Text = "Refresh", Width = 100, Dock = DockStyle.Left };
            _btnRefresh.Click += (s, e) => RefreshKeys();

            _btnClose = new Button { Text = "Close", Width = 90, Dock = DockStyle.Right, DialogResult = DialogResult.OK };

            bottomPanel.Controls.Add(_btnClose);
            bottomPanel.Controls.Add(_btnRefresh);
            bottomPanel.Controls.Add(_btnExport);

            Controls.Add(_listView);
            Controls.Add(topPanel);
            Controls.Add(bottomPanel);
        }

        private void RefreshKeys()
        {
            _listView.Items.Clear();
            _currentKeys = ProductKeyExtractorEngine.ExtractAllProductKeys();

            foreach (var k in _currentKeys)
            {
                var lvi = new ListViewItem(k.ProductName);
                lvi.SubItems.Add(k.ProductKey);
                lvi.SubItems.Add(k.ProductId);
                lvi.SubItems.Add(k.Publisher);
                _listView.Items.Add(lvi);
            }

            _lblSummary.Text = $"Found {_currentKeys.Count} registered software product keys and digital license credentials.";
        }

        private void ExportKeys()
        {
            if (_currentKeys.Count == 0)
            {
                MessageBox.Show("No product keys to export.", "Empty List", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var sfd = new SaveFileDialog
            {
                Filter = "JSON Document (*.json)|*.json|All Files (*.*)|*.*",
                FileName = $"LicenseKeys_Backup_{DateTime.UtcNow:yyyyMMdd}.json"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                if (ProductKeyExtractorEngine.ExportKeys(_currentKeys, sfd.FileName))
                {
                    MessageBox.Show($"Product keys exported successfully to:\n{sfd.FileName}", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}

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
using UninstallTools.SystemTools;
using UninstallTools.Core;

namespace BulkCrapUninstaller.Forms
{
    public class RuntimesManagerWindow : Form
    {
        private ListView _listView;
        private Label _lblStatus;
        private Button _btnRefresh;
        private Button _btnExport;
        private Button _btnClose;
        private ComboBox _cmbFilter;
        private List<RuntimeItem> _runtimes = new List<RuntimeItem>();

        public RuntimesManagerWindow()
        {
            InitializeComponents();
            LoadRuntimes();
        }

        private void InitializeComponents()
        {
            Text = "EBUninstaller Pro - Shared Runtimes & Redistributables Manager";
            Size = new Size(950, 560);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(800, 450);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            var topPanel = new Panel { Dock = DockStyle.Top, Height = 50, Padding = new Padding(12, 10, 12, 10) };
            var lblFilter = new Label { Text = "Category:", AutoSize = true, Location = new Point(12, 14) };
            _cmbFilter = new ComboBox { Location = new Point(80, 10), Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };
            _cmbFilter.Items.AddRange(new object[] { "All Runtimes", "Visual C++", ".NET Runtimes", "DirectX", "Java", "WebView2 / Other" });
            _cmbFilter.SelectedIndex = 0;
            _cmbFilter.SelectedIndexChanged += (s, e) => FilterList();

            _btnRefresh = new Button { Text = "Refresh", Location = new Point(275, 9), Width = 90, Height = 28 };
            _btnRefresh.Click += (s, e) => LoadRuntimes();

            _btnExport = new Button { Text = "Export Manifest...", Location = new Point(375, 9), Width = 130, Height = 28 };
            _btnExport.Click += BtnExport_Click;

            topPanel.Controls.AddRange(new Control[] { lblFilter, _cmbFilter, _btnRefresh, _btnExport });

            _listView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true
            };

            _listView.Columns.Add("Runtime Name", 320);
            _listView.Columns.Add("Category", 110);
            _listView.Columns.Add("Version", 110);
            _listView.Columns.Add("Arch", 70);
            _listView.Columns.Add("Publisher", 160);
            _listView.Columns.Add("Status", 130);

            var btmPanel = new Panel { Dock = DockStyle.Bottom, Height = 45, Padding = new Padding(12, 8, 12, 8) };
            _lblStatus = new Label { Dock = DockStyle.Left, AutoSize = true, Text = "Ready.", Location = new Point(12, 12) };
            _btnClose = new Button { Text = "Close", Dock = DockStyle.Right, Width = 90, DialogResult = DialogResult.OK };
            btmPanel.Controls.AddRange(new Control[] { _lblStatus, _btnClose });

            Controls.Add(_listView);
            Controls.Add(topPanel);
            Controls.Add(btmPanel);
        }

        private void LoadRuntimes()
        {
            _lblStatus.Text = "Scanning installed runtimes and redistributables...";
            _listView.Items.Clear();

            try
            {
                _runtimes = WindowsRuntimesManager.ScanInstalledRuntimes();
                FilterList();
                _lblStatus.Text = $"Detected {_runtimes.Count} shared runtime packages.";
            }
            catch (Exception ex)
            {
                _lblStatus.Text = $"Scan failed: {ex.Message}";
            }
        }

        private void FilterList()
        {
            _listView.Items.Clear();
            var filter = _cmbFilter.SelectedIndex;

            var filtered = _runtimes.Where(r =>
            {
                return filter switch
                {
                    1 => r.Category == RuntimeCategory.VisualCpp,
                    2 => r.Category == RuntimeCategory.DotNet,
                    3 => r.Category == RuntimeCategory.DirectX,
                    4 => r.Category == RuntimeCategory.Java,
                    5 => r.Category != RuntimeCategory.VisualCpp && r.Category != RuntimeCategory.DotNet && r.Category != RuntimeCategory.DirectX && r.Category != RuntimeCategory.Java,
                    _ => true
                };
            }).ToList();

            foreach (var r in filtered)
            {
                var item = new ListViewItem(r.Name);
                item.SubItems.Add(r.Category.ToString());
                item.SubItems.Add(r.Version);
                item.SubItems.Add(r.Architecture);
                item.SubItems.Add(r.Publisher);
                item.SubItems.Add(r.IsSuperseded ? $"Superseded" : "Active");

                if (r.IsSuperseded)
                {
                    item.ForeColor = Color.OrangeRed;
                }

                _listView.Items.Add(item);
            }
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            using var sfd = new SaveFileDialog
            {
                Filter = "JSON Files (*.json)|*.json|CSV Files (*.csv)|*.csv",
                FileName = $"Runtimes_Manifest_{DateTime.Now:yyyyMMdd}.json"
            };

            if (sfd.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    var json = System.Text.Json.JsonSerializer.Serialize(_runtimes, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
                    System.IO.File.WriteAllText(sfd.FileName, json);
                    MessageBox.Show(this, "Runtime manifest exported successfully.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, $"Export failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}

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

namespace BulkCrapUninstaller.Forms
{
    public class DriverBackupWindow : Form
    {
        private ListView _listView;
        private Label _lblStatus;
        private Button _btnRefresh;
        private Button _btnBackupAll;
        private Button _btnClose;
        private List<DriverBackupItem> _drivers = new List<DriverBackupItem>();

        public DriverBackupWindow()
        {
            InitializeComponents();
            LoadDrivers();
        }

        private void InitializeComponents()
        {
            Text = "EBUninstaller Pro - Hardware Driver Backup & Export Engine";
            Size = new Size(950, 560);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(800, 450);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            var topPanel = new Panel { Dock = DockStyle.Top, Height = 50, Padding = new Padding(12, 10, 12, 10) };
            _btnRefresh = new Button { Text = "Refresh", Location = new Point(12, 9), Width = 90, Height = 28 };
            _btnRefresh.Click += (s, e) => LoadDrivers();

            _btnBackupAll = new Button { Text = "Backup All Drivers...", Location = new Point(110, 9), Width = 150, Height = 28 };
            _btnBackupAll.Click += BtnBackupAll_Click;

            topPanel.Controls.AddRange(new Control[] { _btnRefresh, _btnBackupAll });

            _listView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true
            };

            _listView.Columns.Add("Published Name", 110);
            _listView.Columns.Add("Original Name", 130);
            _listView.Columns.Add("Provider", 150);
            _listView.Columns.Add("Class", 110);
            _listView.Columns.Add("Driver Version", 110);
            _listView.Columns.Add("Driver Date", 100);
            _listView.Columns.Add("Signer", 150);

            var btmPanel = new Panel { Dock = DockStyle.Bottom, Height = 45, Padding = new Padding(12, 8, 12, 8) };
            _lblStatus = new Label { Dock = DockStyle.Left, AutoSize = true, Text = "Ready.", Location = new Point(12, 12) };
            _btnClose = new Button { Text = "Close", Dock = DockStyle.Right, Width = 90, DialogResult = DialogResult.OK };
            btmPanel.Controls.AddRange(new Control[] { _lblStatus, _btnClose });

            Controls.Add(_listView);
            Controls.Add(topPanel);
            Controls.Add(btmPanel);
        }

        private void LoadDrivers()
        {
            _lblStatus.Text = "Enumerating OEM drivers from Windows Driver Store...";
            _listView.Items.Clear();

            try
            {
                _drivers = WindowsDriverBackupEngine.EnumerateOemDrivers();
                foreach (var d in _drivers)
                {
                    var item = new ListViewItem(d.PublishedName);
                    item.SubItems.Add(d.OriginalFileName);
                    item.SubItems.Add(d.ProviderName);
                    item.SubItems.Add(d.ClassName);
                    item.SubItems.Add(d.DriverVersion);
                    item.SubItems.Add(d.DriverDate);
                    item.SubItems.Add(d.SignerName);
                    _listView.Items.Add(item);
                }

                _lblStatus.Text = $"Found {_drivers.Count} OEM third-party driver packages.";
            }
            catch (Exception ex)
            {
                _lblStatus.Text = $"Failed to load drivers: {ex.Message}";
            }
        }

        private void BtnBackupAll_Click(object sender, EventArgs e)
        {
            using var fbd = new FolderBrowserDialog
            {
                Description = "Select destination folder for driver backup:"
            };

            if (fbd.ShowDialog(this) == DialogResult.OK)
            {
                _lblStatus.Text = "Exporting drivers... This may take a moment.";
                Application.DoEvents();

                var res = WindowsDriverBackupEngine.ExportDrivers(fbd.SelectedPath);
                if (res.Success)
                {
                    MessageBox.Show(this, $"Successfully exported {res.ExportedCount} drivers in {res.Duration.TotalSeconds:F1}s.\nManifest: {res.ManifestPath}", "Driver Backup Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _lblStatus.Text = $"Backed up {res.ExportedCount} drivers to {fbd.SelectedPath}";
                }
                else
                {
                    MessageBox.Show(this, $"Driver export failed: {res.ErrorMessage}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _lblStatus.Text = "Driver backup failed.";
                }
            }
        }
    }
}

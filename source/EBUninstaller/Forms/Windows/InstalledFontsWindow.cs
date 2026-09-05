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
using UninstallTools.SystemTools;

namespace BulkCrapUninstaller.Forms
{
    public class InstalledFontsWindow : Form
    {
        private ListView _listView;
        private Label _lblStatus;
        private Button _btnScan;
        private Button _btnCleanOrphaned;
        private Button _btnClose;
        private List<InstalledFontItem> _fonts = new List<InstalledFontItem>();

        public InstalledFontsWindow()
        {
            InitializeComponents();
            RunScan();
        }

        private void InitializeComponents()
        {
            Text = "EBUninstaller Pro - Installed Fonts & Font Registry Cleaner";
            Size = new Size(950, 560);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(800, 450);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            var topPanel = new Panel { Dock = DockStyle.Top, Height = 50, Padding = new Padding(12, 10, 12, 10) };
            _btnScan = new Button { Text = "Scan Fonts", Location = new Point(12, 9), Width = 110, Height = 28 };
            _btnScan.Click += (s, e) => RunScan();

            _btnCleanOrphaned = new Button { Text = "Clean Selected Orphaned...", Location = new Point(130, 9), Width = 190, Height = 28 };
            _btnCleanOrphaned.Click += BtnCleanOrphaned_Click;

            topPanel.Controls.AddRange(new Control[] { _btnScan, _btnCleanOrphaned });

            _listView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                CheckBoxes = true,
                GridLines = true
            };

            _listView.Columns.Add("Font Name", 240);
            _listView.Columns.Add("File Name", 180);
            _listView.Columns.Add("Full Path", 320);
            _listView.Columns.Add("Hive", 70);
            _listView.Columns.Add("Status", 110);

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
            _lblStatus.Text = "Scanning font registrations across HKLM and HKCU...";
            _listView.Items.Clear();

            try
            {
                _fonts = InstalledFontsCleaner.ScanInstalledFonts();
                int orphaned = 0;

                foreach (var f in _fonts)
                {
                    if (f.IsOrphaned) orphaned++;
                    var lvi = new ListViewItem(f.FontName) { Checked = f.IsOrphaned && !f.IsSystemDefault };
                    lvi.SubItems.Add(f.FileName);
                    lvi.SubItems.Add(f.FullFontPath);
                    lvi.SubItems.Add(f.RegistryRoot);
                    lvi.SubItems.Add(f.IsOrphaned ? "Orphaned File" : (f.IsSystemDefault ? "System Default" : "Installed"));

                    if (f.IsOrphaned)
                    {
                        lvi.ForeColor = Color.OrangeRed;
                    }
                    else if (f.IsSystemDefault)
                    {
                        lvi.ForeColor = Color.Gray;
                    }

                    _listView.Items.Add(lvi);
                }

                _lblStatus.Text = $"Found {_fonts.Count} registered fonts ({orphaned} orphaned font registrations).";
            }
            catch (Exception ex)
            {
                _lblStatus.Text = $"Scan failed: {ex.Message}";
            }
        }

        private void BtnCleanOrphaned_Click(object sender, EventArgs e)
        {
            var selected = new List<InstalledFontItem>();
            for (int i = 0; i < _listView.Items.Count; i++)
            {
                if (_listView.Items[i].Checked && i < _fonts.Count)
                {
                    selected.Add(_fonts[i]);
                }
            }

            if (!selected.Any())
            {
                MessageBox.Show(this, "Please select at least one orphaned font entry to clean.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show(this, $"Are you sure you want to clean {selected.Count} orphaned font registry entries?\nA backup will be created automatically.", "Confirm Cleanup", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var backupDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "EBUninstaller", "Backups");
                var cleaned = InstalledFontsCleaner.CleanOrphanedFonts(selected, backupDir);

                MessageBox.Show(this, $"Cleaned {cleaned} orphaned font registry entries.", "Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RunScan();
            }
        }
    }
}

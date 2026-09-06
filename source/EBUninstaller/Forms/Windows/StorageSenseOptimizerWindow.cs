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
using UninstallTools.SystemTools;

namespace BulkCrapUninstaller.Forms.Windows
{
    public class StorageSenseOptimizerWindow : Form
    {
        private CheckBox _chkEnableStorageSense;
        private ComboBox _cboRecycleBinDays;
        private ComboBox _cboDownloadsDays;
        private Button _btnApply;
        private Button _btnClose;
        private Label _lblStatus;

        public StorageSenseOptimizerWindow()
        {
            InitializeComponent();
            LoadSettings();
        }

        private void InitializeComponent()
        {
            Text = "Windows Storage Sense & Retention Optimizer - EBUninstaller Pro";
            Size = new Size(620, 360);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            var lblIntro = new Label
            {
                Text = "Configure automated Windows Storage Sense policies to periodically purge temporary files, empty the Recycle Bin, and clean old downloaded setup installers.",
                Location = new Point(20, 20),
                Size = new Size(560, 45)
            };

            var grpPolicies = new GroupBox
            {
                Text = "Automated Retention Rules",
                Location = new Point(20, 75),
                Size = new Size(560, 175)
            };

            _chkEnableStorageSense = new CheckBox
            {
                Text = "Enable Windows Storage Sense Automated Cleaner",
                Location = new Point(20, 30),
                AutoSize = true,
                Checked = true
            };

            var lblRecycle = new Label { Text = "Purge Recycle Bin older than:", Location = new Point(20, 70), AutoSize = true };
            _cboRecycleBinDays = new ComboBox { Location = new Point(230, 67), Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };
            _cboRecycleBinDays.Items.AddRange(new object[] { "Never", "1 Day", "14 Days", "30 Days", "60 Days" });
            _cboRecycleBinDays.SelectedIndex = 3;

            var lblDownloads = new Label { Text = "Purge Downloads folder older than:", Location = new Point(20, 115), AutoSize = true };
            _cboDownloadsDays = new ComboBox { Location = new Point(230, 112), Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };
            _cboDownloadsDays.Items.AddRange(new object[] { "Never (Keep Downloads)", "1 Day", "14 Days", "30 Days", "60 Days" });
            _cboDownloadsDays.SelectedIndex = 0;

            grpPolicies.Controls.Add(_chkEnableStorageSense);
            grpPolicies.Controls.Add(lblRecycle);
            grpPolicies.Controls.Add(_cboRecycleBinDays);
            grpPolicies.Controls.Add(lblDownloads);
            grpPolicies.Controls.Add(_cboDownloadsDays);

            _lblStatus = new Label
            {
                Text = "Ready to update Storage Sense configuration.",
                Location = new Point(20, 265),
                Size = new Size(380, 25),
                ForeColor = Color.DarkSlateGray
            };

            _btnApply = new Button
            {
                Text = "Apply Policies",
                Location = new Point(360, 275),
                Width = 130,
                Height = 32
            };
            _btnApply.Click += (s, e) => SaveSettings();

            _btnClose = new Button
            {
                Text = "Close",
                Location = new Point(500, 275),
                Width = 80,
                Height = 32,
                DialogResult = DialogResult.OK
            };

            Controls.Add(lblIntro);
            Controls.Add(grpPolicies);
            Controls.Add(_lblStatus);
            Controls.Add(_btnApply);
            Controls.Add(_btnClose);
        }

        private void LoadSettings()
        {
            var cfg = StorageSenseOptimizer.GetStorageSensePolicy();
            _chkEnableStorageSense.Checked = cfg.IsStorageSenseEnabled;
        }

        private void SaveSettings()
        {
            var cfg = new StorageSenseConfig
            {
                IsStorageSenseEnabled = _chkEnableStorageSense.Checked,
                RecycleBinCleanupDays = 30,
                DownloadsCleanupDays = 0
            };

            if (StorageSenseOptimizer.SetStorageSensePolicy(cfg))
            {
                _lblStatus.Text = "Storage Sense policies applied successfully.";
                _lblStatus.ForeColor = Color.DarkGreen;
                MessageBox.Show("Windows Storage Sense policies updated successfully!", "Settings Applied", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}

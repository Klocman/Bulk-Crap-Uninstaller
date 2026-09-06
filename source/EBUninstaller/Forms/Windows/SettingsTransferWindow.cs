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
using System.IO;
using System.Windows.Forms;
using UninstallTools.Exclusions;

namespace BulkCrapUninstaller.Forms.Windows
{
    public class SettingsTransferWindow : Form
    {
        private TextBox _txtFilePath;
        private Button _btnBrowse;
        private Button _btnExport;
        private Button _btnImport;
        private Button _btnClose;
        private CheckBox _chkIncludeHistory;
        private Label _lblStatus;

        public SettingsTransferWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Profile & Settings Transfer - EBUninstaller Pro";
            Size = new Size(620, 360);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            var lblIntro = new Label
            {
                Text = "Export or import your complete EBUninstaller Pro profile, including custom filters, exclusion lists, quiet automation configurations, and operation history.",
                Location = new Point(20, 20),
                Size = new Size(560, 50)
            };

            var grpPath = new GroupBox
            {
                Text = "Profile Package File (*.json / *.ebuprofile)",
                Location = new Point(20, 80),
                Size = new Size(560, 80)
            };

            _txtFilePath = new TextBox
            {
                Location = new Point(15, 30),
                Width = 430,
                Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "EBUninstaller_Profile.json")
            };

            _btnBrowse = new Button
            {
                Text = "Browse...",
                Location = new Point(455, 28),
                Width = 90
            };
            _btnBrowse.Click += (s, e) =>
            {
                using var sfd = new SaveFileDialog
                {
                    Filter = "EBUninstaller Profile (*.json)|*.json|All Files (*.*)|*.*",
                    FileName = "EBUninstaller_Profile.json"
                };
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    _txtFilePath.Text = sfd.FileName;
                }
            };

            grpPath.Controls.Add(_txtFilePath);
            grpPath.Controls.Add(_btnBrowse);

            _chkIncludeHistory = new CheckBox
            {
                Text = "Include Operation & Uninstallation History in Export",
                Location = new Point(20, 175),
                AutoSize = true,
                Checked = true
            };

            _lblStatus = new Label
            {
                Text = "Ready to export or import configuration package.",
                Location = new Point(20, 210),
                Size = new Size(560, 30),
                ForeColor = Color.DarkSlateGray
            };

            _btnExport = new Button
            {
                Text = "Export Profile",
                Location = new Point(160, 260),
                Width = 130,
                Height = 32
            };
            _btnExport.Click += (s, e) =>
            {
                var target = _txtFilePath.Text.Trim();
                if (string.IsNullOrEmpty(target)) return;

                bool success = SettingsTransferEngine.ExportProfile(target, _chkIncludeHistory.Checked);
                if (success)
                {
                    _lblStatus.Text = $"Export completed successfully to: {Path.GetFileName(target)}";
                    _lblStatus.ForeColor = Color.DarkGreen;
                    MessageBox.Show("Profile exported successfully!", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    _lblStatus.Text = "Export failed. Please check file path permissions.";
                    _lblStatus.ForeColor = Color.DarkRed;
                }
            };

            _btnImport = new Button
            {
                Text = "Import Profile",
                Location = new Point(300, 260),
                Width = 130,
                Height = 32
            };
            _btnImport.Click += (s, e) =>
            {
                var target = _txtFilePath.Text.Trim();
                if (!File.Exists(target))
                {
                    MessageBox.Show("Selected file does not exist.", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (SettingsTransferEngine.ValidateProfile(target, out var summary))
                {
                    if (MessageBox.Show($"Are you sure you want to import this profile?\n\n{summary}", "Confirm Import", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        if (SettingsTransferEngine.ImportProfile(target, false))
                        {
                            _lblStatus.Text = "Profile imported and applied successfully.";
                            _lblStatus.ForeColor = Color.DarkGreen;
                            MessageBox.Show("Profile imported successfully!", "Import Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                else
                {
                    MessageBox.Show($"Invalid profile package:\n{summary}", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            _btnClose = new Button
            {
                Text = "Close",
                Location = new Point(460, 260),
                Width = 90,
                Height = 32,
                DialogResult = DialogResult.OK
            };

            Controls.Add(lblIntro);
            Controls.Add(grpPath);
            Controls.Add(_chkIncludeHistory);
            Controls.Add(_lblStatus);
            Controls.Add(_btnExport);
            Controls.Add(_btnImport);
            Controls.Add(_btnClose);
        }
    }
}

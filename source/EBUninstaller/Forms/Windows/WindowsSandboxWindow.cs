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
    public class WindowsSandboxWindow : Form
    {
        private TextBox _txtHostFolder;
        private Button _btnBrowseFolder;
        private CheckBox _chkReadOnly;
        private CheckBox _chkNetworking;
        private CheckBox _chkGPU;
        private TextBox _txtLogonCommand;
        private Button _btnLaunch;
        private Button _btnClose;
        private Label _lblStatus;

        public WindowsSandboxWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Windows Sandbox Isolated Launcher - EBUninstaller Pro";
            Size = new Size(650, 420);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            var lblIntro = new Label
            {
                Text = "Safely evaluate untrusted setup installers or test uninstallation procedures inside a disposable, isolated Windows Sandbox without modifying your primary system.",
                Location = new Point(20, 20),
                Size = new Size(590, 50)
            };

            var grpConfig = new GroupBox
            {
                Text = "Sandbox Environment Configuration",
                Location = new Point(20, 80),
                Size = new Size(590, 220)
            };

            var lblHost = new Label { Text = "Host Folder to Mount:", Location = new Point(15, 30), AutoSize = true };
            _txtHostFolder = new TextBox { Location = new Point(15, 55), Width = 460 };
            _btnBrowseFolder = new Button { Text = "Browse...", Location = new Point(485, 53), Width = 90 };
            _btnBrowseFolder.Click += (s, e) =>
            {
                using var fbd = new FolderBrowserDialog { Description = "Select Host Folder to Map into Sandbox" };
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    _txtHostFolder.Text = fbd.SelectedPath;
                }
            };

            _chkReadOnly = new CheckBox { Text = "Mount as Read-Only (Protect Host Files)", Location = new Point(15, 95), AutoSize = true, Checked = true };
            _chkNetworking = new CheckBox { Text = "Enable Sandbox Internet & Network Access", Location = new Point(15, 125), AutoSize = true, Checked = true };
            _chkGPU = new CheckBox { Text = "Enable Virtual GPU Hardware Acceleration", Location = new Point(15, 155), AutoSize = true, Checked = true };

            var lblCommand = new Label { Text = "Command to Run on Logon:", Location = new Point(15, 185), AutoSize = true };
            _txtLogonCommand = new TextBox { Location = new Point(190, 182), Width = 385 };

            grpConfig.Controls.Add(lblHost);
            grpConfig.Controls.Add(_txtHostFolder);
            grpConfig.Controls.Add(_btnBrowseFolder);
            grpConfig.Controls.Add(_chkReadOnly);
            grpConfig.Controls.Add(_chkNetworking);
            grpConfig.Controls.Add(_chkGPU);
            grpConfig.Controls.Add(lblCommand);
            grpConfig.Controls.Add(_txtLogonCommand);

            _lblStatus = new Label
            {
                Text = "Ready to launch isolated sandbox.",
                Location = new Point(20, 315),
                Size = new Size(400, 25),
                ForeColor = Color.DarkSlateGray
            };

            _btnLaunch = new Button
            {
                Text = "Launch Sandbox",
                Location = new Point(360, 335),
                Width = 140,
                Height = 32
            };
            _btnLaunch.Click += (s, e) => LaunchSandbox();

            _btnClose = new Button
            {
                Text = "Close",
                Location = new Point(510, 335),
                Width = 100,
                Height = 32,
                DialogResult = DialogResult.OK
            };

            Controls.Add(lblIntro);
            Controls.Add(grpConfig);
            Controls.Add(_lblStatus);
            Controls.Add(_btnLaunch);
            Controls.Add(_btnClose);
        }

        private void LaunchSandbox()
        {
            var config = new SandboxLaunchConfig
            {
                HostFolderToMap = _txtHostFolder.Text.Trim(),
                ReadOnly = _chkReadOnly.Checked,
                EnableNetworking = _chkNetworking.Checked,
                EnableVGpu = _chkGPU.Checked,
                ExecutableToRunOnLogon = _txtLogonCommand.Text.Trim()
            };

            _lblStatus.Text = "Starting Windows Sandbox...";
            _lblStatus.ForeColor = Color.Navy;

            if (WindowsSandboxManager.LaunchInSandbox(config, out var err))
            {
                _lblStatus.Text = "Windows Sandbox launched successfully.";
                _lblStatus.ForeColor = Color.DarkGreen;
            }
            else
            {
                _lblStatus.Text = $"Failed to start: {err}";
                _lblStatus.ForeColor = Color.DarkRed;
                MessageBox.Show($"Failed to launch Windows Sandbox:\n{err}\n\nPlease verify that Windows Sandbox is enabled in Windows Features.", "Launch Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}

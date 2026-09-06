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
using System.Threading;
using System.Windows.Forms;
using UninstallTools.FileSystemEngine;

namespace BulkCrapUninstaller.Forms
{
    public class FreeSpaceWiperWindow : Form
    {
        private ComboBox _cmbDrive;
        private ComboBox _cmbPattern;
        private ProgressBar _progressBar;
        private Label _lblStatus;
        private Button _btnStart;
        private Button _btnCancel;
        private Button _btnClose;
        private CancellationTokenSource _cts;

        public FreeSpaceWiperWindow()
        {
            InitializeComponents();
            LoadDrives();
        }

        private void InitializeComponents()
        {
            Text = "EBUninstaller Pro - Free Disk Space Sanitizer";
            Size = new Size(650, 360);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            var lblDrive = new Label { Text = "Select Drive Volume:", Location = new Point(24, 20), AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
            _cmbDrive = new ComboBox { Location = new Point(24, 45), Width = 580, DropDownStyle = ComboBoxStyle.DropDownList };

            var lblPattern = new Label { Text = "Sanitization Method:", Location = new Point(24, 85), AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
            _cmbPattern = new ComboBox { Location = new Point(24, 110), Width = 580, DropDownStyle = ComboBoxStyle.DropDownList };
            _cmbPattern.Items.AddRange(new object[]
            {
                "1-Pass Zero Fill (Standard HDD / Flash)",
                "1-Pass Pseudo-Random Fill (Enhanced Privacy)",
                "SSD TRIM Re-optimization (Safe for NVMe / SSDs)"
            });
            _cmbPattern.SelectedIndex = 0;

            var lblDisclaimer = new Label
            {
                Text = "Note: Sanitizing free space permanently overwrites deleted file clusters to prevent forensic recovery. For SSDs, TRIM is recommended to avoid unnecessary flash wear.",
                Location = new Point(24, 150),
                Size = new Size(580, 40),
                ForeColor = Color.DimGray
            };

            _progressBar = new ProgressBar { Location = new Point(24, 200), Width = 580, Height = 22 };
            _lblStatus = new Label { Text = "Ready.", Location = new Point(24, 230), AutoSize = true };

            _btnStart = new Button { Text = "Start Sanitizing", Location = new Point(280, 265), Width = 120, Height = 32 };
            _btnStart.Click += BtnStart_Click;

            _btnCancel = new Button { Text = "Cancel", Location = new Point(410, 265), Width = 90, Height = 32, Enabled = false };
            _btnCancel.Click += (s, e) => _cts?.Cancel();

            _btnClose = new Button { Text = "Close", Location = new Point(510, 265), Width = 94, Height = 32, DialogResult = DialogResult.OK };

            Controls.AddRange(new Control[]
            {
                lblDrive, _cmbDrive, lblPattern, _cmbPattern, lblDisclaimer,
                _progressBar, _lblStatus, _btnStart, _btnCancel, _btnClose
            });
        }

        private void LoadDrives()
        {
            _cmbDrive.Items.Clear();
            foreach (var d in DriveInfo.GetDrives())
            {
                if (d.IsReady && d.DriveType == DriveType.Fixed)
                {
                    _cmbDrive.Items.Add($"{d.Name} ({d.VolumeLabel}) - Free: {d.AvailableFreeSpace / (1024.0 * 1024 * 1024):F1} GB / Total: {d.TotalSize / (1024.0 * 1024 * 1024):F1} GB");
                }
            }

            if (_cmbDrive.Items.Count > 0) _cmbDrive.SelectedIndex = 0;
        }

        private async void BtnStart_Click(object sender, EventArgs e)
        {
            if (_cmbDrive.SelectedIndex < 0) return;

            var driveStr = _cmbDrive.SelectedItem.ToString();
            var driveRoot = driveStr.Split(' ')[0];

            var pattern = _cmbPattern.SelectedIndex switch
            {
                1 => FreeSpaceWipePattern.RandomFill,
                2 => FreeSpaceWipePattern.TrimOnly,
                _ => FreeSpaceWipePattern.ZeroFill
            };

            if (MessageBox.Show(this, $"Start free space sanitization on {driveRoot} using {_cmbPattern.SelectedItem}?\nActive files will NOT be modified.", "Confirm Free Space Wipe", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            _btnStart.Enabled = false;
            _btnCancel.Enabled = true;
            _btnClose.Enabled = false;
            _cts = new CancellationTokenSource();

            var progress = new Progress<WipeProgressEventArgs>(p =>
            {
                _progressBar.Value = Math.Min(100, Math.Max(0, p.Percentage));
                _lblStatus.Text = p.StatusMessage;
            });

            var success = await FreeSpaceWiper.WipeFreeSpaceAsync(driveRoot, pattern, progress, _cts.Token);

            _btnStart.Enabled = true;
            _btnCancel.Enabled = false;
            _btnClose.Enabled = true;

            if (success)
            {
                _progressBar.Value = 100;
                _lblStatus.Text = "Sanitization complete!";
                MessageBox.Show(this, "Free disk space sanitized successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                _lblStatus.Text = _cts.IsCancellationRequested ? "Canceled by user." : "Sanitization encountered an error.";
            }
        }
    }
}

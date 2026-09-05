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
using UninstallTools.InstallationMonitor;

namespace BulkCrapUninstaller.Forms
{
    public class InstallationSnapshotDiffWindow : Form
    {
        private SystemSnapshot _snapshotBefore;
        private SystemSnapshot _snapshotAfter;
        private ListView _listView;
        private Label _lblStatus;
        private Button _btnCaptureBefore;
        private Button _btnCaptureAfter;
        private Button _btnCompare;
        private Button _btnClose;

        public InstallationSnapshotDiffWindow()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            Text = "EBUninstaller Pro - Installation Snapshot Comparison & Differential Inspector";
            Size = new Size(950, 560);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(800, 450);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            var topPanel = new Panel { Dock = DockStyle.Top, Height = 50, Padding = new Padding(12, 10, 12, 10) };
            _btnCaptureBefore = new Button { Text = "1. Capture Snapshot A (Before)", Location = new Point(12, 9), Width = 190, Height = 28 };
            _btnCaptureBefore.Click += (s, e) => CaptureBefore();

            _btnCaptureAfter = new Button { Text = "2. Capture Snapshot B (After)", Location = new Point(210, 9), Width = 190, Height = 28, Enabled = false };
            _btnCaptureAfter.Click += (s, e) => CaptureAfter();

            _btnCompare = new Button { Text = "3. Compare Differential", Location = new Point(410, 9), Width = 160, Height = 28, Enabled = false };
            _btnCompare.Click += (s, e) => RunCompare();

            topPanel.Controls.AddRange(new Control[] { _btnCaptureBefore, _btnCaptureAfter, _btnCompare });

            _listView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true
            };

            _listView.Columns.Add("Delta Type", 120);
            _listView.Columns.Add("Item Category", 110);
            _listView.Columns.Add("File Path / Registry Key", 600);

            var btmPanel = new Panel { Dock = DockStyle.Bottom, Height = 45, Padding = new Padding(12, 8, 12, 8) };
            _lblStatus = new Label { Dock = DockStyle.Left, AutoSize = true, Text = "Click 'Capture Snapshot A' to record system state before installing software.", Location = new Point(12, 12) };
            _btnClose = new Button { Text = "Close", Dock = DockStyle.Right, Width = 90, DialogResult = DialogResult.OK };
            btmPanel.Controls.AddRange(new Control[] { _lblStatus, _btnClose });

            Controls.Add(_listView);
            Controls.Add(topPanel);
            Controls.Add(btmPanel);
        }

        private void CaptureBefore()
        {
            _lblStatus.Text = "Capturing Snapshot A...";
            Application.DoEvents();

            _snapshotBefore = InstallationSnapshotDiffer.CaptureSnapshot("Snapshot A (Before)");
            _btnCaptureAfter.Enabled = true;
            _lblStatus.Text = $"Snapshot A captured ({_snapshotBefore.Files.Count} files, {_snapshotBefore.RegistryKeys.Count} registry keys). Now install software, then capture Snapshot B.";
        }

        private void CaptureAfter()
        {
            _lblStatus.Text = "Capturing Snapshot B...";
            Application.DoEvents();

            _snapshotAfter = InstallationSnapshotDiffer.CaptureSnapshot("Snapshot B (After)");
            _btnCompare.Enabled = true;
            _lblStatus.Text = $"Snapshot B captured ({_snapshotAfter.Files.Count} files, {_snapshotAfter.RegistryKeys.Count} registry keys). Ready to compare.";
        }

        private void RunCompare()
        {
            _lblStatus.Text = "Calculating differential changes...";
            _listView.Items.Clear();

            var diff = InstallationSnapshotDiffer.CompareSnapshots(_snapshotBefore, _snapshotAfter);

            foreach (var f in diff.AddedFiles)
            {
                var lvi = new ListViewItem("Added");
                lvi.SubItems.Add("File");
                lvi.SubItems.Add(f);
                lvi.ForeColor = Color.ForestGreen;
                _listView.Items.Add(lvi);
            }

            foreach (var f in diff.ModifiedFiles)
            {
                var lvi = new ListViewItem("Modified");
                lvi.SubItems.Add("File");
                lvi.SubItems.Add(f);
                lvi.ForeColor = Color.DarkOrange;
                _listView.Items.Add(lvi);
            }

            foreach (var r in diff.AddedRegistryKeys)
            {
                var lvi = new ListViewItem("Added");
                lvi.SubItems.Add("Registry Key");
                lvi.SubItems.Add(r);
                lvi.ForeColor = Color.DodgerBlue;
                _listView.Items.Add(lvi);
            }

            _lblStatus.Text = $"Differential analysis complete: {diff.TotalChangesCount} total change(s) detected in {diff.ComparisonDuration.TotalSeconds:F2}s.";
        }
    }
}

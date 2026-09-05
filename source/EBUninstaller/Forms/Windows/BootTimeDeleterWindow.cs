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
using UninstallTools.FileSystemEngine;

namespace BulkCrapUninstaller.Forms.Windows
{
    public class BootTimeDeleterWindow : Form
    {
        private ListView _listView;
        private Button _btnAddFile;
        private Button _btnCancelSelected;
        private Button _btnClose;
        private Label _lblSummary;

        public BootTimeDeleterWindow()
        {
            InitializeComponent();
            RefreshList();
        }

        private void InitializeComponent()
        {
            Text = "Boot-Time Locked File Deleter - EBUninstaller Pro";
            Size = new Size(840, 500);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(700, 400);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            var topPanel = new Panel { Dock = DockStyle.Top, Height = 55, Padding = new Padding(12) };
            var bottomPanel = new Panel { Dock = DockStyle.Bottom, Height = 55, Padding = new Padding(12) };

            _lblSummary = new Label
            {
                Text = "Manage stubborn files and locked DLLs scheduled for automatic deletion on next Windows reboot.",
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

            _listView.Columns.Add("Source Path", 380);
            _listView.Columns.Add("Operation", 120);
            _listView.Columns.Add("Exists on Disk", 110);
            _listView.Columns.Add("Size", 100);

            _btnAddFile = new Button { Text = "Schedule File for Boot Delete...", Width = 210, Dock = DockStyle.Left };
            _btnAddFile.Click += (s, e) => AddFileSchedule();

            _btnCancelSelected = new Button { Text = "Cancel Selected", Width = 130, Dock = DockStyle.Left };
            _btnCancelSelected.Click += (s, e) => CancelSelected();

            _btnClose = new Button { Text = "Close", Width = 90, Dock = DockStyle.Right, DialogResult = DialogResult.OK };

            bottomPanel.Controls.Add(_btnClose);
            bottomPanel.Controls.Add(_btnCancelSelected);
            bottomPanel.Controls.Add(_btnAddFile);

            Controls.Add(_listView);
            Controls.Add(topPanel);
            Controls.Add(bottomPanel);
        }

        private void RefreshList()
        {
            _listView.Items.Clear();
            var items = BootTimeDeleterEngine.GetPendingBootDeletions();

            foreach (var item in items)
            {
                var lvi = new ListViewItem(item.SourcePath) { Tag = item };
                lvi.SubItems.Add(item.IsDeleteOperation ? "Delete on Boot" : $"Rename to {item.DestinationPath}");
                lvi.SubItems.Add(item.FileExistsOnDisk ? "Yes" : "Pending / Deleted");
                lvi.SubItems.Add(item.FileSizeBytes > 0 ? $"{item.FileSizeBytes / 1024} KB" : "-");
                _listView.Items.Add(lvi);
            }

            _lblSummary.Text = $"Total {items.Count} pending boot-time file operations registered in Windows Session Manager.";
        }

        private void AddFileSchedule()
        {
            using var ofd = new OpenFileDialog
            {
                Title = "Select Stubborn or Locked File to Delete on Next Reboot",
                Filter = "All Files (*.*)|*.*"
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                if (BootTimeDeleterEngine.ScheduleFileForBootDeletion(ofd.FileName))
                {
                    RefreshList();
                    MessageBox.Show($"File successfully scheduled for boot-time deletion:\n{ofd.FileName}", "Scheduled Successfully", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void CancelSelected()
        {
            if (_listView.SelectedItems.Count == 0) return;
            var item = _listView.SelectedItems[0].Tag as PendingBootDeletionItem;
            if (item != null)
            {
                if (BootTimeDeleterEngine.CancelBootDeletion(item.SourcePath))
                {
                    RefreshList();
                }
            }
        }
    }
}

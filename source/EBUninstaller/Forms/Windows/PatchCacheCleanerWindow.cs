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
using UninstallTools.JunkCleaner;

namespace BulkCrapUninstaller.Forms.Windows
{
    public class PatchCacheCleanerWindow : Form
    {
        private ListView _listView;
        private Button _btnClean;
        private Button _btnRefresh;
        private Button _btnClose;
        private Label _lblSummary;
        private List<PatchCacheItem> _items = new List<PatchCacheItem>();

        public PatchCacheCleanerWindow()
        {
            InitializeComponent();
            RefreshList();
        }

        private void InitializeComponent()
        {
            Text = "Windows Installer Patch Cache (.msp/.msi) Cleaner - EBUninstaller Pro";
            Size = new Size(880, 520);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(720, 420);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            var topPanel = new Panel { Dock = DockStyle.Top, Height = 55, Padding = new Padding(12) };
            var bottomPanel = new Panel { Dock = DockStyle.Bottom, Height = 50, Padding = new Padding(12) };

            _lblSummary = new Label
            {
                Text = "Auditing Windows Installer Cache (C:\\Windows\\Installer)...",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Regular)
            };
            topPanel.Controls.Add(_lblSummary);

            _listView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                CheckBoxes = true,
                GridLines = true
            };

            _listView.Columns.Add("Patch File Name", 240);
            _listView.Columns.Add("Size", 100);
            _listView.Columns.Add("Type", 120);
            _listView.Columns.Add("Status", 140);
            _listView.Columns.Add("File Path", 240);

            _btnClean = new Button { Text = "Clean Orphaned Patches", Width = 190, Dock = DockStyle.Left };
            _btnClean.Click += (s, e) => CleanSelected();

            _btnRefresh = new Button { Text = "Refresh", Width = 90, Dock = DockStyle.Left };
            _btnRefresh.Click += (s, e) => RefreshList();

            _btnClose = new Button { Text = "Close", Width = 90, Dock = DockStyle.Right, DialogResult = DialogResult.OK };

            bottomPanel.Controls.Add(_btnClose);
            bottomPanel.Controls.Add(_btnRefresh);
            bottomPanel.Controls.Add(_btnClean);

            Controls.Add(_listView);
            Controls.Add(topPanel);
            Controls.Add(bottomPanel);
        }

        private void RefreshList()
        {
            _listView.Items.Clear();
            _items = PatchCacheResidualsCleaner.ScanPatchCache();

            long orphanedBytes = 0;
            int orphanedCount = 0;

            foreach (var item in _items)
            {
                var lvi = new ListViewItem(item.FileName) { Checked = item.IsOrphaned, Tag = item };
                lvi.SubItems.Add(item.FileSizeBytes > 0 ? (item.FileSizeBytes / (1024 * 1024)) + " MB" : "-");
                lvi.SubItems.Add(item.PackageType);
                lvi.SubItems.Add(item.IsOrphaned ? "Orphaned (Safe to Delete)" : "Registered (In Use)");
                lvi.SubItems.Add(item.FilePath);

                if (item.IsOrphaned)
                {
                    lvi.BackColor = Color.FromArgb(255, 245, 235);
                    orphanedBytes += item.FileSizeBytes;
                    orphanedCount++;
                }

                _listView.Items.Add(lvi);
            }

            _lblSummary.Text = "Found " + _items.Count + " patch files (" + orphanedCount + " orphaned, " + (orphanedBytes / (1024 * 1024)) + " MB reclaimable).";
        }

        private void CleanSelected()
        {
            var selected = new List<PatchCacheItem>();
            foreach (ListViewItem lvi in _listView.Items)
            {
                if (lvi.Checked && lvi.Tag is PatchCacheItem p && p.IsOrphaned)
                {
                    selected.Add(p);
                }
            }

            if (selected.Count == 0)
            {
                MessageBox.Show("No orphaned patch files selected for cleaning.", "Nothing Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show("Permanently delete " + selected.Count + " orphaned patch files?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                int deleted = PatchCacheResidualsCleaner.CleanOrphanedPatches(selected);
                RefreshList();
                MessageBox.Show("Deleted " + deleted + " orphaned patch files.", "Cleanup Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}

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
using UninstallTools.RegistryEngine;

namespace BulkCrapUninstaller.Forms.Windows
{
    public class SharedDllAuditorWindow : Form
    {
        private ListView _listView;
        private Button _btnCleanOrphaned;
        private Button _btnRefresh;
        private Button _btnClose;
        private Label _lblSummary;
        private List<SharedDllRecord> _currentRecords = new List<SharedDllRecord>();

        public SharedDllAuditorWindow()
        {
            InitializeComponent();
            RefreshList();
        }

        private void InitializeComponent()
        {
            Text = "Shared DLL Reference Auditor - EBUninstaller Pro";
            Size = new Size(920, 520);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(760, 420);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            var topPanel = new Panel { Dock = DockStyle.Top, Height = 55, Padding = new Padding(12) };
            var bottomPanel = new Panel { Dock = DockStyle.Bottom, Height = 55, Padding = new Padding(12) };

            _lblSummary = new Label
            {
                Text = "Auditing Windows SharedDLL reference counters across installed software and runtimes.",
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

            _listView.Columns.Add("Shared File Path", 440);
            _listView.Columns.Add("Reference Count", 130);
            _listView.Columns.Add("Status on Disk", 130);
            _listView.Columns.Add("Registry Hive", 110);

            _btnCleanOrphaned = new Button { Text = "Clean Orphaned SharedDLLs", Width = 210, Dock = DockStyle.Left };
            _btnCleanOrphaned.Click += (s, e) => CleanOrphaned();

            _btnRefresh = new Button { Text = "Refresh", Width = 100, Dock = DockStyle.Left };
            _btnRefresh.Click += (s, e) => RefreshList();

            _btnClose = new Button { Text = "Close", Width = 90, Dock = DockStyle.Right, DialogResult = DialogResult.OK };

            bottomPanel.Controls.Add(_btnClose);
            bottomPanel.Controls.Add(_btnRefresh);
            bottomPanel.Controls.Add(_btnCleanOrphaned);

            Controls.Add(_listView);
            Controls.Add(topPanel);
            Controls.Add(bottomPanel);
        }

        private void RefreshList()
        {
            _listView.Items.Clear();
            _currentRecords = SharedDllAuditorEngine.ScanSharedDlls();

            foreach (var r in _currentRecords)
            {
                var lvi = new ListViewItem(r.FilePath) { Checked = r.IsOrphanedReference, Tag = r };
                lvi.SubItems.Add(r.ReferenceCount.ToString());
                lvi.SubItems.Add(r.FileExistsOnDisk ? "Present" : "Orphaned / Missing");
                lvi.SubItems.Add(r.RegistryRoot);

                if (r.IsOrphanedReference)
                {
                    lvi.BackColor = Color.FromArgb(255, 240, 240);
                    lvi.ForeColor = Color.DarkRed;
                }

                _listView.Items.Add(lvi);
            }

            int orphaned = _currentRecords.Count(r => r.IsOrphanedReference);
            _lblSummary.Text = $"Found {_currentRecords.Count} shared DLL entries ({orphaned} orphaned pointing to missing files).";
        }

        private void CleanOrphaned()
        {
            var selected = new List<SharedDllRecord>();
            foreach (ListViewItem item in _listView.Items)
            {
                if (item.Checked && item.Tag is SharedDllRecord rec && rec.IsOrphanedReference)
                {
                    selected.Add(rec);
                }
            }

            if (selected.Count == 0)
            {
                MessageBox.Show("No orphaned SharedDLL entries selected for cleaning.", "Nothing Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show($"Clean {selected.Count} orphaned SharedDLL registry entries?", "Confirm Cleaning", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                int cleaned = SharedDllAuditorEngine.CleanOrphanedSharedDlls(selected);
                RefreshList();
                MessageBox.Show($"Successfully cleaned {cleaned} orphaned SharedDLL entries.", "Cleanup Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}

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

namespace BulkCrapUninstaller.Forms
{
    public class ApplicationFootprintWindow : Form
    {
        private string _appName;
        private string _installLocation;
        private string _publisher;
        private ListView _listView;
        private Label _lblSummary;
        private ListBox _lstLargestFiles;
        private Button _btnClose;

        public ApplicationFootprintWindow(string appName, string installLocation, string publisher = null)
        {
            _appName = appName;
            _installLocation = installLocation;
            _publisher = publisher;

            InitializeComponents();
            LoadFootprint();
        }

        private void InitializeComponents()
        {
            Text = $"EBUninstaller Pro - Storage & Registry Footprint: {_appName}";
            Size = new Size(950, 600);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(800, 480);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            var topPanel = new Panel { Dock = DockStyle.Top, Height = 55, Padding = new Padding(12, 10, 12, 10) };
            _lblSummary = new Label { Dock = DockStyle.Fill, Text = "Analyzing application footprint...", AutoSize = false, Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            topPanel.Controls.Add(_lblSummary);

            var split = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Horizontal, SplitterDistance = 320 };

            _listView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true
            };

            _listView.Columns.Add("Location Type", 180);
            _listView.Columns.Add("Path / Registry Key", 420);
            _listView.Columns.Add("Size", 100);
            _listView.Columns.Add("Files / Keys", 90);

            var btmSplitPanel = new Panel { Dock = DockStyle.Fill };
            var lblLargest = new Label { Text = "Largest Files in Application Root:", Dock = DockStyle.Top, Height = 25, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
            _lstLargestFiles = new ListBox { Dock = DockStyle.Fill };
            btmSplitPanel.Controls.Add(_lstLargestFiles);
            btmSplitPanel.Controls.Add(lblLargest);

            split.Panel1.Controls.Add(_listView);
            split.Panel2.Controls.Add(btmSplitPanel);

            var btmPanel = new Panel { Dock = DockStyle.Bottom, Height = 45, Padding = new Padding(12, 8, 12, 8) };
            _btnClose = new Button { Text = "Close", Dock = DockStyle.Right, Width = 90, DialogResult = DialogResult.OK };
            btmPanel.Controls.Add(_btnClose);

            Controls.Add(split);
            Controls.Add(topPanel);
            Controls.Add(btmPanel);
        }

        private void LoadFootprint()
        {
            var report = ApplicationFootprintAnalyzer.AnalyzeFootprint(_appName, _installLocation, _publisher);

            _lblSummary.Text = $"Total Disk Usage: {FormatSize(report.TotalDiskSizeBytes)} ({report.TotalFileCount} files across {report.Locations.Count} locations) | Registry Keys: {report.TotalRegistryKeysCount}";

            _listView.Items.Clear();
            foreach (var loc in report.Locations)
            {
                var lvi = new ListViewItem(loc.LocationType);
                lvi.SubItems.Add(loc.PathOrKey);
                lvi.SubItems.Add(loc.LocationType == "Registry" ? "N/A" : FormatSize(loc.SizeBytes));
                lvi.SubItems.Add(loc.ItemCount.ToString());
                _listView.Items.Add(lvi);
            }

            _lstLargestFiles.Items.Clear();
            foreach (var file in report.TopLargestFiles)
            {
                _lstLargestFiles.Items.Add(file);
            }
        }

        private static string FormatSize(long bytes)
        {
            if (bytes <= 0) return "0 MB";
            if (bytes >= 1024L * 1024 * 1024)
                return $"{(bytes / (1024.0 * 1024 * 1024)):F2} GB";
            return $"{(bytes / (1024.0 * 1024)):F1} MB";
        }
    }
}

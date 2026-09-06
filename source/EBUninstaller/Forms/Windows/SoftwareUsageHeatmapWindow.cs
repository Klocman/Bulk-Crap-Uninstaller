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
using UninstallTools;
using UninstallTools.Detection;

namespace BulkCrapUninstaller.Forms.Windows
{
    public class SoftwareUsageHeatmapWindow : Form
    {
        private readonly List<ApplicationUninstallerEntry> _installedApps;
        private ListView _listView;
        private ComboBox _cboCategory;
        private Label _lblSummary;
        private Button _btnClose;

        public SoftwareUsageHeatmapWindow(IEnumerable<ApplicationUninstallerEntry> apps = null)
        {
            _installedApps = apps?.ToList() ?? new List<ApplicationUninstallerEntry>();
            InitializeComponent();
            LoadHeatmap();
        }

        private void InitializeComponent()
        {
            Text = "Software Inactivity & Usage Heatmap - EBUninstaller Pro";
            Size = new Size(960, 560);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(800, 460);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            var topPanel = new Panel { Dock = DockStyle.Top, Height = 55, Padding = new Padding(12) };
            var bottomPanel = new Panel { Dock = DockStyle.Bottom, Height = 50, Padding = new Padding(12) };

            _lblSummary = new Label
            {
                Text = "Analyzing application launch frequency and unused disk footprints...",
                Dock = DockStyle.Left,
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
            };

            _cboCategory = new ComboBox
            {
                Dock = DockStyle.Right,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 220
            };
            _cboCategory.Items.AddRange(new object[] { "All Applications", "Zombie Installations Only", "Unused Over 90 Days", "Rarely Used" });
            _cboCategory.SelectedIndex = 0;
            _cboCategory.SelectedIndexChanged += (s, e) => LoadHeatmap();

            topPanel.Controls.Add(_lblSummary);
            topPanel.Controls.Add(_cboCategory);

            _listView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true
            };

            _listView.Columns.Add("Application Name", 240);
            _listView.Columns.Add("Publisher", 160);
            _listView.Columns.Add("Usage Activity", 160);
            _listView.Columns.Add("Days Inactive", 110);
            _listView.Columns.Add("Estimated Size", 110);
            _listView.Columns.Add("Reclaim Priority", 120);

            _btnClose = new Button { Text = "Close", Width = 90, Dock = DockStyle.Right, DialogResult = DialogResult.OK };
            bottomPanel.Controls.Add(_btnClose);

            Controls.Add(_listView);
            Controls.Add(topPanel);
            Controls.Add(bottomPanel);
        }

        private void LoadHeatmap()
        {
            _listView.Items.Clear();
            var entries = SoftwareUsageHeatmapEngine.AnalyzeUsageHeatmap(_installedApps);

            var filter = _cboCategory.SelectedItem?.ToString() ?? "All Applications";
            if (filter == "Zombie Installations Only")
                entries = entries.Where(e => e.Category == UsageFrequencyCategory.ZombieInstallation).ToList();
            else if (filter == "Unused Over 90 Days")
                entries = entries.Where(e => e.DaysSinceLastUsed > 90).ToList();
            else if (filter == "Rarely Used")
                entries = entries.Where(e => e.Category == UsageFrequencyCategory.RarelyUsed).ToList();

            foreach (var e in entries)
            {
                var lvi = new ListViewItem(e.ApplicationName);
                lvi.SubItems.Add(e.Publisher);
                lvi.SubItems.Add(e.Category.ToString());
                lvi.SubItems.Add(e.DaysSinceLastUsed >= 999 ? "Never / Unknown" : e.DaysSinceLastUsed + " days");
                lvi.SubItems.Add(e.EstimatedSizeBytes > 0 ? (e.EstimatedSizeBytes / (1024 * 1024)) + " MB" : "-");
                lvi.SubItems.Add($"{e.ReclaimPriorityScore}/100");

                if (e.Category == UsageFrequencyCategory.ZombieInstallation)
                {
                    lvi.BackColor = Color.FromArgb(255, 235, 235);
                    lvi.ForeColor = Color.DarkRed;
                }
                else if (e.Category == UsageFrequencyCategory.UnusedOver90Days)
                {
                    lvi.BackColor = Color.FromArgb(255, 248, 225);
                }

                _listView.Items.Add(lvi);
            }

            int zombieCount = entries.Count(e => e.Category == UsageFrequencyCategory.ZombieInstallation);
            _lblSummary.Text = $"Evaluated {entries.Count} applications: {zombieCount} Zombie (Long-abandoned) Installations found.";
        }
    }
}

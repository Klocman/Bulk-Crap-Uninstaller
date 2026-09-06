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
using UninstallTools.Startup;

namespace BulkCrapUninstaller.Forms
{
    public class BootPerformanceWindow : Form
    {
        private ListView _listView;
        private Label _lblSummary;
        private ListBox _lstTips;
        private Button _btnRefresh;
        private Button _btnClose;

        public BootPerformanceWindow()
        {
            InitializeComponents();
            LoadBootMetrics();
        }

        private void InitializeComponents()
        {
            Text = "EBUninstaller Pro - Windows Boot & Startup Performance Benchmark";
            Size = new Size(950, 600);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(800, 480);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            var topPanel = new Panel { Dock = DockStyle.Top, Height = 65, Padding = new Padding(12, 10, 12, 10) };
            _lblSummary = new Label { Dock = DockStyle.Left, AutoSize = false, Width = 680, Text = "Querying boot performance logs...", Font = new Font("Segoe UI", 9.5F, FontStyle.Bold) };
            _btnRefresh = new Button { Text = "Refresh Benchmark", Dock = DockStyle.Right, Width = 150, Height = 32 };
            _btnRefresh.Click += (s, e) => LoadBootMetrics();
            topPanel.Controls.Add(_lblSummary);
            topPanel.Controls.Add(_btnRefresh);

            var split = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Horizontal, SplitterDistance = 300 };

            _listView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true
            };

            _listView.Columns.Add("Impact", 80);
            _listView.Columns.Add("Application / Component", 260);
            _listView.Columns.Add("Startup Delay (ms)", 130);
            _listView.Columns.Add("Binary Path", 400);

            var btmSplitPanel = new Panel { Dock = DockStyle.Fill };
            var lblTips = new Label { Text = "Optimization Recommendations:", Dock = DockStyle.Top, Height = 25, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
            _lstTips = new ListBox { Dock = DockStyle.Fill };
            btmSplitPanel.Controls.Add(_lstTips);
            btmSplitPanel.Controls.Add(lblTips);

            split.Panel1.Controls.Add(_listView);
            split.Panel2.Controls.Add(btmSplitPanel);

            var btmPanel = new Panel { Dock = DockStyle.Bottom, Height = 45, Padding = new Padding(12, 8, 12, 8) };
            _btnClose = new Button { Text = "Close", Dock = DockStyle.Right, Width = 90, DialogResult = DialogResult.OK };
            btmPanel.Controls.Add(_btnClose);

            Controls.Add(split);
            Controls.Add(topPanel);
            Controls.Add(btmPanel);
        }

        private void LoadBootMetrics()
        {
            var report = BootPerformanceAnalyzer.QueryBootPerformance();

            _lblSummary.Text = $"Last Boot Duration: {(report.TotalBootDurationMs / 1000.0):F1}s (Main Path: {(report.MainPathBootTimeMs / 1000.0):F1}s, Post-Boot: {(report.BootPostBootTimeMs / 1000.0):F1}s)\nRecorded: {report.LastBootTimeUtc:yyyy-MM-dd HH:mm:ss} UTC";

            _listView.Items.Clear();
            foreach (var item in report.DegradedItems)
            {
                var lvi = new ListViewItem(item.ImpactLevel);
                lvi.SubItems.Add(item.ApplicationName);
                lvi.SubItems.Add($"{item.DelayDurationMs} ms");
                lvi.SubItems.Add(item.Path);

                if (item.ImpactLevel == "High") lvi.ForeColor = Color.Red;
                else if (item.ImpactLevel == "Medium") lvi.ForeColor = Color.DarkOrange;

                _listView.Items.Add(lvi);
            }

            _lstTips.Items.Clear();
            foreach (var tip in report.OptimizationTips)
            {
                _lstTips.Items.Add(tip);
            }
        }
    }
}

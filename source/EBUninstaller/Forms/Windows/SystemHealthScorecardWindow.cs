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
using UninstallTools.Core;

namespace BulkCrapUninstaller.Forms.Windows
{
    public class SystemHealthScorecardWindow : Form
    {
        private ListView _listView;
        private Label _lblOverallScore;
        private Label _lblBadge;
        private TextBox _txtRecommendations;
        private Button _btnRefresh;
        private Button _btnClose;

        public SystemHealthScorecardWindow()
        {
            InitializeComponent();
            RefreshScorecard();
        }

        private void InitializeComponent()
        {
            Text = "System Health & Optimization Scorecard - EBUninstaller Pro";
            Size = new Size(860, 560);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(720, 460);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            var topPanel = new Panel { Dock = DockStyle.Top, Height = 75, Padding = new Padding(15) };
            var bottomPanel = new Panel { Dock = DockStyle.Bottom, Height = 55, Padding = new Padding(12) };

            _lblOverallScore = new Label
            {
                Text = "Health Score: --/100",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                Location = new Point(15, 15),
                AutoSize = true
            };

            _lblBadge = new Label
            {
                Text = "Rating: Evaluating...",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Location = new Point(320, 22),
                AutoSize = true,
                ForeColor = Color.DarkSlateBlue
            };

            topPanel.Controls.Add(_lblOverallScore);
            topPanel.Controls.Add(_lblBadge);

            var split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 240
            };

            _listView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true
            };
            _listView.Columns.Add("Diagnostic Health Category", 260);
            _listView.Columns.Add("Score", 80);
            _listView.Columns.Add("Issues Found", 100);
            _listView.Columns.Add("Category Health Summary", 380);

            _txtRecommendations = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Segoe UI", 9F, FontStyle.Regular)
            };

            split.Panel1.Controls.Add(_listView);
            split.Panel2.Controls.Add(_txtRecommendations);

            _btnRefresh = new Button { Text = "Recalculate Score", Width = 150, Dock = DockStyle.Left };
            _btnRefresh.Click += (s, e) => RefreshScorecard();

            _btnClose = new Button { Text = "Close", Width = 90, Dock = DockStyle.Right, DialogResult = DialogResult.OK };

            bottomPanel.Controls.Add(_btnClose);
            bottomPanel.Controls.Add(_btnRefresh);

            Controls.Add(split);
            Controls.Add(topPanel);
            Controls.Add(bottomPanel);
        }

        private void RefreshScorecard()
        {
            _listView.Items.Clear();
            var report = SystemHealthScorecardEngine.GenerateHealthScorecard();

            _lblOverallScore.Text = "Health Score: " + report.CompositeScore + "/100";
            _lblBadge.Text = "Status: " + report.RatingBadge;

            if (report.CompositeScore >= 80) _lblOverallScore.ForeColor = Color.DarkGreen;
            else if (report.CompositeScore >= 60) _lblOverallScore.ForeColor = Color.DarkOrange;
            else _lblOverallScore.ForeColor = Color.DarkRed;

            foreach (var cat in report.Categories)
            {
                var lvi = new ListViewItem(cat.CategoryName);
                lvi.SubItems.Add(cat.Score + "/100");
                lvi.SubItems.Add(cat.IssueCount.ToString());
                lvi.SubItems.Add(cat.Summary);
                _listView.Items.Add(lvi);
            }

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("ACTIONABLE OPTIMIZATION RECOMMENDATIONS:");
            sb.AppendLine();
            foreach (var rec in report.ActionableRecommendations)
            {
                sb.AppendLine(" • " + rec);
            }

            _txtRecommendations.Text = sb.ToString();
        }
    }
}

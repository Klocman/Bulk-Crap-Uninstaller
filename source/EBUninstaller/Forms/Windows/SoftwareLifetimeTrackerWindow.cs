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
using UninstallTools.History;

namespace BulkCrapUninstaller.Forms.Windows
{
    public class SoftwareLifetimeTrackerWindow : Form
    {
        private readonly List<ApplicationUninstallerEntry> _apps;
        private ListView _listView;
        private ComboBox _cboStage;
        private Label _lblSummary;
        private Button _btnClose;

        public SoftwareLifetimeTrackerWindow(IEnumerable<ApplicationUninstallerEntry> apps)
        {
            _apps = apps?.ToList() ?? new List<ApplicationUninstallerEntry>();
            InitializeComponent();
            LoadTimeline();
        }

        private void InitializeComponent()
        {
            Text = "Software Installation & Lifetime Timeline - EBUninstaller Pro";
            Size = new Size(950, 540);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(780, 440);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            var topPanel = new Panel { Dock = DockStyle.Top, Height = 55, Padding = new Padding(12) };
            var bottomPanel = new Panel { Dock = DockStyle.Bottom, Height = 50, Padding = new Padding(12) };

            _lblSummary = new Label
            {
                Text = "Reconstructing software installation history and lifetime timeline...",
                Dock = DockStyle.Left,
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
            };

            _cboStage = new ComboBox
            {
                Dock = DockStyle.Right,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 200
            };
            _cboStage.Items.AddRange(new object[] { "All Applications", "Newly Installed (<14 days)", "Established (14-90 days)", "Long-Term (>90 days)", "Vintage (>1 year)" });
            _cboStage.SelectedIndex = 0;
            _cboStage.SelectedIndexChanged += (s, e) => LoadTimeline();

            topPanel.Controls.Add(_lblSummary);
            topPanel.Controls.Add(_cboStage);

            _listView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true
            };

            _listView.Columns.Add("Application Name", 260);
            _listView.Columns.Add("Install Date", 120);
            _listView.Columns.Add("Installation Age", 130);
            _listView.Columns.Add("Lifecycle Stage", 130);
            _listView.Columns.Add("Publisher", 180);

            _btnClose = new Button { Text = "Close", Width = 90, Dock = DockStyle.Right, DialogResult = DialogResult.OK };
            bottomPanel.Controls.Add(_btnClose);

            Controls.Add(_listView);
            Controls.Add(topPanel);
            Controls.Add(bottomPanel);
        }

        private void LoadTimeline()
        {
            _listView.Items.Clear();
            var timeline = SoftwareLifetimeTrackerEngine.BuildLifecycleTimeline(_apps);

            var filter = _cboStage.SelectedItem?.ToString() ?? "All Applications";
            if (filter == "Newly Installed (<14 days)")
                timeline = timeline.Where(t => t.Stage == LifecycleStage.NewlyInstalled).ToList();
            else if (filter == "Established (14-90 days)")
                timeline = timeline.Where(t => t.Stage == LifecycleStage.Established).ToList();
            else if (filter == "Long-Term (>90 days)")
                timeline = timeline.Where(t => t.Stage == LifecycleStage.LongTerm).ToList();
            else if (filter == "Vintage (>1 year)")
                timeline = timeline.Where(t => t.Stage == LifecycleStage.Vintage).ToList();

            foreach (var t in timeline)
            {
                var lvi = new ListViewItem(t.ApplicationName);
                lvi.SubItems.Add(t.FormattedInstallDate);
                lvi.SubItems.Add(t.AgeInDays + " days old");
                lvi.SubItems.Add(t.Stage.ToString());
                lvi.SubItems.Add(t.Publisher);

                if (t.Stage == LifecycleStage.NewlyInstalled)
                {
                    lvi.BackColor = Color.FromArgb(240, 255, 240);
                }
                else if (t.Stage == LifecycleStage.Vintage)
                {
                    lvi.BackColor = Color.FromArgb(250, 250, 250);
                }

                _listView.Items.Add(lvi);
            }

            int newCount = timeline.Count(t => t.Stage == LifecycleStage.NewlyInstalled);
            _lblSummary.Text = "Timeline contains " + timeline.Count + " installed software entries (" + newCount + " recently installed).";
        }
    }
}

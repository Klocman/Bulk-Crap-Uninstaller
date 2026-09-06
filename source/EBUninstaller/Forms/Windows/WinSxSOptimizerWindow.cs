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
using UninstallTools.SystemTools;

namespace BulkCrapUninstaller.Forms.Windows
{
    public class WinSxSOptimizerWindow : Form
    {
        private TextBox _txtOutput;
        private Button _btnAnalyze;
        private Button _btnClean;
        private Button _btnResetBase;
        private Button _btnClose;
        private Label _lblStatus;

        public WinSxSOptimizerWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Windows Component Store (WinSxS) Deep Optimizer - EBUninstaller Pro";
            Size = new Size(820, 520);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(680, 400);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            var topPanel = new Panel { Dock = DockStyle.Top, Height = 55, Padding = new Padding(12) };
            var bottomPanel = new Panel { Dock = DockStyle.Bottom, Height = 55, Padding = new Padding(12) };

            _lblStatus = new Label
            {
                Text = "Analyze and clean superseded Windows update packages from C:\\Windows\\WinSxS.",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Regular)
            };
            topPanel.Controls.Add(_lblStatus);

            _txtOutput = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Both,
                Font = new Font("Consolas", 8.5F, FontStyle.Regular)
            };

            _btnAnalyze = new Button { Text = "Analyze WinSxS Store", Width = 160, Dock = DockStyle.Left };
            _btnAnalyze.Click += (s, e) => AnalyzeStore();

            _btnClean = new Button { Text = "Start Component Cleanup", Width = 180, Dock = DockStyle.Left };
            _btnClean.Click += (s, e) => CleanStore(false);

            _btnResetBase = new Button { Text = "Deep ResetBase Cleanup", Width = 180, Dock = DockStyle.Left };
            _btnResetBase.Click += (s, e) => CleanStore(true);

            _btnClose = new Button { Text = "Close", Width = 90, Dock = DockStyle.Right, DialogResult = DialogResult.OK };

            bottomPanel.Controls.Add(_btnClose);
            bottomPanel.Controls.Add(_btnResetBase);
            bottomPanel.Controls.Add(_btnClean);
            bottomPanel.Controls.Add(_btnAnalyze);

            Controls.Add(_txtOutput);
            Controls.Add(topPanel);
            Controls.Add(bottomPanel);
        }

        private void AnalyzeStore()
        {
            _lblStatus.Text = "Running DISM /AnalyzeComponentStore... Please wait.";
            _txtOutput.Text = "Analyzing Windows Component Store (WinSxS)...\r\n";

            var report = WinSxSStoreAnalyzer.AnalyzeComponentStore();
            _txtOutput.Text = report.RawOutput;
            _lblStatus.Text = report.ComponentCleanupRecommended ? "Cleanup Recommended by Windows DISM!" : "Component Store Analysis Completed.";
        }

        private void CleanStore(bool resetBase)
        {
            if (MessageBox.Show("Start DISM Component Store Cleanup" + (resetBase ? " with /ResetBase (Warning: Superseded updates cannot be uninstalled afterwards)?" : "?"),
                "Confirm Cleanup", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _lblStatus.Text = "Executing Component Cleanup...";
                bool success = WinSxSStoreAnalyzer.RunComponentCleanup(resetBase);
                _lblStatus.Text = success ? "Component cleanup finished successfully." : "Component cleanup finished with warnings.";
                MessageBox.Show(success ? "Component store cleanup completed successfully!" : "Cleanup finished. Check DISM logs for details.", "DISM Execution", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}

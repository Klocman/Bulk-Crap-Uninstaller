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
using UninstallTools.Startup;

namespace BulkCrapUninstaller.Forms
{
    public class ServiceDependencyWindow : Form
    {
        private TreeView _treeView;
        private Label _lblStatus;
        private Button _btnRefresh;
        private Button _btnClose;
        private Dictionary<string, ServiceDependencyNode> _dependencyMap = new Dictionary<string, ServiceDependencyNode>();

        public ServiceDependencyWindow()
        {
            InitializeComponents();
            LoadDependencies();
        }

        private void InitializeComponents()
        {
            Text = "EBUninstaller Pro - Windows Services Dependency Graph";
            Size = new Size(950, 600);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(800, 480);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            var topPanel = new Panel { Dock = DockStyle.Top, Height = 50, Padding = new Padding(12, 10, 12, 10) };
            _btnRefresh = new Button { Text = "Refresh Dependency Map", Location = new Point(12, 9), Width = 180, Height = 28 };
            _btnRefresh.Click += (s, e) => LoadDependencies();
            topPanel.Controls.Add(_btnRefresh);

            _treeView = new TreeView
            {
                Dock = DockStyle.Fill,
                ShowNodeToolTips = true
            };

            var btmPanel = new Panel { Dock = DockStyle.Bottom, Height = 45, Padding = new Padding(12, 8, 12, 8) };
            _lblStatus = new Label { Dock = DockStyle.Left, AutoSize = true, Text = "Ready.", Location = new Point(12, 12) };
            _btnClose = new Button { Text = "Close", Dock = DockStyle.Right, Width = 90, DialogResult = DialogResult.OK };
            btmPanel.Controls.AddRange(new Control[] { _lblStatus, _btnClose });

            Controls.Add(_treeView);
            Controls.Add(topPanel);
            Controls.Add(btmPanel);
        }

        private void LoadDependencies()
        {
            _lblStatus.Text = "Building service dependency tree...";
            _treeView.Nodes.Clear();

            try
            {
                _dependencyMap = ServiceDependencyTree.BuildDependencyMap();

                var rootNodes = _dependencyMap.Values
                    .Where(n => n.DependsOn.Count > 0 || n.RequiredBy.Count > 0)
                    .OrderBy(n => n.DisplayName)
                    .ToList();

                foreach (var svc in rootNodes)
                {
                    var parentNode = new TreeNode($"{svc.DisplayName} ({svc.ServiceName}) - [{svc.StartTypeName}]")
                    {
                        Tag = svc
                    };

                    if (svc.DependsOn.Count > 0)
                    {
                        var depsNode = new TreeNode($"Depends On ({svc.DependsOn.Count})");
                        foreach (var d in svc.DependsOn)
                        {
                            var title = _dependencyMap.TryGetValue(d, out var depNode) ? $"{depNode.DisplayName} ({d})" : d;
                            depsNode.Nodes.Add(new TreeNode(title));
                        }
                        parentNode.Nodes.Add(depsNode);
                    }

                    if (svc.RequiredBy.Count > 0)
                    {
                        var reqNode = new TreeNode($"Required By ({svc.RequiredBy.Count})");
                        foreach (var r in svc.RequiredBy)
                        {
                            var title = _dependencyMap.TryGetValue(r, out var reqSvc) ? $"{reqSvc.DisplayName} ({r})" : r;
                            reqNode.Nodes.Add(new TreeNode(title));
                        }
                        parentNode.Nodes.Add(reqNode);
                    }

                    _treeView.Nodes.Add(parentNode);
                }

                _lblStatus.Text = $"Mapped {_dependencyMap.Count} services ({rootNodes.Count} with active dependencies).";
            }
            catch (Exception ex)
            {
                _lblStatus.Text = $"Error: {ex.Message}";
            }
        }
    }
}

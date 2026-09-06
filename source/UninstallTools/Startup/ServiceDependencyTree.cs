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
using System.Linq;
using Microsoft.Win32;
using UninstallTools.Core;

namespace UninstallTools.Startup
{
    /// <summary>
    /// Node representing a service and its hierarchical dependencies.
    /// </summary>
    public class ServiceDependencyNode
    {
        public string ServiceName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public int StartType { get; set; } // 2=Auto, 3=Manual, 4=Disabled
        public string StartTypeName => StartType switch
        {
            2 => "Automatic",
            3 => "Manual",
            4 => "Disabled",
            _ => "Unknown"
        };
        public List<string> DependsOn { get; } = new List<string>();
        public List<string> RequiredBy { get; } = new List<string>();
        public bool IsProtected { get; set; }
        public bool IsSafeToDisable => !IsProtected && RequiredBy.Count == 0;
    }

    /// <summary>
    /// Builds directed dependency graphs for Windows Services to prevent breaking dependent software when optimizing or removing services.
    /// </summary>
    public static class ServiceDependencyTree
    {
        /// <summary>
        /// Builds the complete system service dependency map from the registry.
        /// </summary>
        public static Dictionary<string, ServiceDependencyNode> BuildDependencyMap()
        {
            var map = new Dictionary<string, ServiceDependencyNode>(StringComparer.OrdinalIgnoreCase);

            try
            {
                using var servicesKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services");
                if (servicesKey == null) return map;

                // 1. First pass: Create nodes and gather direct dependencies
                foreach (var serviceName in servicesKey.GetSubKeyNames())
                {
                    try
                    {
                        using var sub = servicesKey.OpenSubKey(serviceName);
                        if (sub == null) continue;

                        var displayName = sub.GetValue("DisplayName")?.ToString() ?? serviceName;
                        var imagePath = sub.GetValue("ImagePath")?.ToString() ?? string.Empty;
                        var startType = sub.GetValue("Start") is int st ? st : 3;
                        var depsRaw = sub.GetValue("DependOnService");

                        var node = new ServiceDependencyNode
                        {
                            ServiceName = serviceName,
                            DisplayName = displayName,
                            ImagePath = imagePath,
                            StartType = startType,
                            IsProtected = SecurityGuard.IsCriticalService(serviceName)
                        };

                        if (depsRaw is string[] depArr)
                        {
                            node.DependsOn.AddRange(depArr.Where(d => !string.IsNullOrWhiteSpace(d)));
                        }
                        else if (depsRaw is string depStr && !string.IsNullOrWhiteSpace(depStr))
                        {
                            node.DependsOn.Add(depStr);
                        }

                        map[serviceName] = node;
                    }
                    catch { }
                }

                // 2. Second pass: Calculate inverse dependencies (RequiredBy)
                foreach (var kvp in map)
                {
                    var node = kvp.Value;
                    foreach (var dep in node.DependsOn)
                    {
                        if (map.TryGetValue(dep, out var parentNode))
                        {
                            if (!parentNode.RequiredBy.Contains(node.ServiceName, StringComparer.OrdinalIgnoreCase))
                            {
                                parentNode.RequiredBy.Add(node.ServiceName);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Error($"Failed to build service dependency map: {ex.Message}");
            }

            return map;
        }
    }
}

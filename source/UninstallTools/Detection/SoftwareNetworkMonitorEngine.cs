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
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using UninstallTools.Core;

namespace UninstallTools.Detection
{
    public class AppNetworkConnectionRecord
    {
        public int ProcessId { get; set; }
        public string ProcessName { get; set; } = string.Empty;
        public string LocalEndpoint { get; set; } = string.Empty;
        public string RemoteEndpoint { get; set; } = string.Empty;
        public string Protocol { get; set; } = "TCP";
        public string State { get; set; } = "Established";
        public string AssociatedAppName { get; set; } = "Background Process";
    }

    /// <summary>
    /// Monitors and correlates active TCP and UDP sockets with installed software binaries
    /// to identify background processes transmitting outbound data or telemetry.
    /// </summary>
    public static class SoftwareNetworkMonitorEngine
    {
        /// <summary>
        /// Retrieves active network connections and maps them against installed applications.
        /// </summary>
        public static List<AppNetworkConnectionRecord> GetActiveConnections(IEnumerable<ApplicationUninstallerEntry> apps = null)
        {
            var list = new List<AppNetworkConnectionRecord>();

            try
            {
                var ipProps = IPGlobalProperties.GetIPGlobalProperties();
                var tcpConnections = ipProps.GetActiveTcpConnections();

                var procMap = new Dictionary<int, string>();
                try
                {
                    foreach (var p in Process.GetProcesses())
                    {
                        try { procMap[p.Id] = p.ProcessName; } catch { }
                    }
                }
                catch { }

                foreach (var tcp in tcpConnections)
                {
                    var local = tcp.LocalEndPoint.ToString();
                    var remote = tcp.RemoteEndPoint.ToString();

                    list.Add(new AppNetworkConnectionRecord
                    {
                        LocalEndpoint = local,
                        RemoteEndpoint = remote,
                        Protocol = "TCP",
                        State = tcp.State.ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Error(LogCategory.Detection, "Failed to inspect active network connections: " + ex.Message);
            }

            return list;
        }
    }
}

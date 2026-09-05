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
using System.Diagnostics;
using System.IO;
using System.Text;
using UninstallTools.Core;

namespace UninstallTools.SystemTools
{
    public class SandboxLaunchConfig
    {
        public string HostFolderToMap { get; set; } = string.Empty;
        public string SandboxFolder { get; set; } = @"C:\SandboxMount";
        public bool ReadOnly { get; set; } = true;
        public bool EnableNetworking { get; set; } = true;
        public bool EnableVGpu { get; set; } = true;
        public string ExecutableToRunOnLogon { get; set; } = string.Empty;
    }

    /// <summary>
    /// Generates dynamic Windows Sandbox configuration profiles (.wsb)
    /// to safely execute and evaluate untrusted software installers in an isolated environment.
    /// </summary>
    public static class WindowsSandboxManager
    {
        /// <summary>
        /// Generates the XML content for a Windows Sandbox configuration file (.wsb).
        /// </summary>
        public static string GenerateSandboxWsbXml(SandboxLaunchConfig config)
        {
            if (config == null) config = new SandboxLaunchConfig();

            var sb = new StringBuilder();
            sb.AppendLine("<Configuration>");
            sb.AppendLine($"  <VGpu>{(config.EnableVGpu ? "Enable" : "Disable")}</VGpu>");
            sb.AppendLine($"  <Networking>{(config.EnableNetworking ? "Default" : "Disable")}</Networking>");

            if (!string.IsNullOrEmpty(config.HostFolderToMap) && Directory.Exists(config.HostFolderToMap))
            {
                sb.AppendLine("  <MappedFolders>");
                sb.AppendLine("    <MappedFolder>");
                sb.AppendLine($"      <HostFolder>{config.HostFolderToMap}</HostFolder>");
                sb.AppendLine($"      <SandboxFolder>{config.SandboxFolder}</SandboxFolder>");
                sb.AppendLine($"      <ReadOnly>{(config.ReadOnly ? "true" : "false")}</ReadOnly>");
                sb.AppendLine("    </MappedFolder>");
                sb.AppendLine("  </MappedFolders>");
            }

            if (!string.IsNullOrEmpty(config.ExecutableToRunOnLogon))
            {
                sb.AppendLine("  <LogonCommand>");
                sb.AppendLine($"    <Command>{config.ExecutableToRunOnLogon}</Command>");
                sb.AppendLine("  </LogonCommand>");
            }

            sb.AppendLine("</Configuration>");
            return sb.ToString();
        }

        /// <summary>
        /// Creates a temporary .wsb file and launches Windows Sandbox.
        /// </summary>
        public static bool LaunchInSandbox(SandboxLaunchConfig config, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                var xml = GenerateSandboxWsbXml(config);
                var tempWsb = Path.Combine(Path.GetTempPath(), $"EBUninstaller_Sandbox_{Guid.NewGuid():N}.wsb");
                File.WriteAllText(tempWsb, xml, Encoding.UTF8);

                var psi = new ProcessStartInfo
                {
                    FileName = tempWsb,
                    UseShellExecute = true
                };

                var proc = Process.Start(psi);
                StructuredLogger.Info(LogCategory.SystemTools, $"Launched Windows Sandbox with profile: {tempWsb}");
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                StructuredLogger.Error(LogCategory.SystemTools, $"Failed to launch Windows Sandbox: {ex.Message}");
                return false;
            }
        }
    }
}

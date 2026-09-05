/*
    EBUninstaller Pro - Windows Optional Features & Capabilities Manager
    Auditing, enabling, disabling, and cleanup of Windows optional features and on-demand capabilities.
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using UninstallTools.Core;

namespace UninstallTools.Detection
{
    public enum FeatureState
    {
        Enabled,
        Disabled,
        EnablePending,
        DisablePending,
        Installed,
        NotPresent,
        Unknown
    }

    public class WindowsOptionalFeatureItem
    {
        public string FeatureName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public FeatureState State { get; set; } = FeatureState.Unknown;
        public bool IsCapability { get; set; }
        public bool RestartRequired { get; set; }
        public bool IsCritical { get; set; }
    }

    public static class WindowsOptionalFeaturesManager
    {
        private static readonly HashSet<string> CriticalFeatures = new(StringComparer.OrdinalIgnoreCase)
        {
            "NetFx4Extended-ASPNET45",
            "NetFx3",
            "Microsoft-Windows-Kernel",
            "Windows-Defender-Default-Definitions"
        };

        public static List<WindowsOptionalFeatureItem> GetOptionalFeatures(Action<string>? onProgress = null)
        {
            var results = new List<WindowsOptionalFeatureItem>();
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return results;

            onProgress?.Invoke("Querying Windows optional features via DISM...");
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "dism.exe"),
                    Arguments = "/Online /Get-Features /Format:Table",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };

                using var proc = Process.Start(psi);
                if (proc != null)
                {
                    string output = proc.StandardOutput.ReadToEnd();
                    proc.WaitForExit();

                    var lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    bool headerPassed = false;

                    foreach (var line in lines)
                    {
                        if (line.StartsWith("---"))
                        {
                            headerPassed = true;
                            continue;
                        }

                        if (!headerPassed) continue;

                        int pipeIndex = line.LastIndexOf('|');
                        if (pipeIndex > 0)
                        {
                            string featureName = line.Substring(0, pipeIndex).Trim();
                            string stateStr = line.Substring(pipeIndex + 1).Trim();

                            if (!string.IsNullOrEmpty(featureName))
                            {
                                FeatureState state = stateStr.ToLowerInvariant() switch
                                {
                                    "enabled" => FeatureState.Enabled,
                                    "disabled" => FeatureState.Disabled,
                                    "enable pending" => FeatureState.EnablePending,
                                    "disable pending" => FeatureState.DisablePending,
                                    _ => FeatureState.Unknown
                                };

                                results.Add(new WindowsOptionalFeatureItem
                                {
                                    FeatureName = featureName,
                                    DisplayName = FormatDisplayName(featureName),
                                    State = state,
                                    IsCapability = false,
                                    IsCritical = CriticalFeatures.Contains(featureName)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Log(LogLevel.Warning, "WindowsOptionalFeaturesManager", $"DISM Features query error: {ex.Message}");
            }

            // Also query Capabilities (App-on-Demand packages)
            onProgress?.Invoke("Querying Windows Capabilities...");
            try
            {
                var psiCap = new ProcessStartInfo
                {
                    FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "dism.exe"),
                    Arguments = "/Online /Get-Capabilities /Format:Table",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };

                using var procCap = Process.Start(psiCap);
                if (procCap != null)
                {
                    string output = procCap.StandardOutput.ReadToEnd();
                    procCap.WaitForExit();

                    var lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    bool headerPassed = false;

                    foreach (var line in lines)
                    {
                        if (line.StartsWith("---"))
                        {
                            headerPassed = true;
                            continue;
                        }

                        if (!headerPassed) continue;

                        int pipeIndex = line.LastIndexOf('|');
                        if (pipeIndex > 0)
                        {
                            string capIdentity = line.Substring(0, pipeIndex).Trim();
                            string stateStr = line.Substring(pipeIndex + 1).Trim();

                            if (!string.IsNullOrEmpty(capIdentity))
                            {
                                FeatureState state = stateStr.ToLowerInvariant() switch
                                {
                                    "installed" => FeatureState.Installed,
                                    "not present" => FeatureState.NotPresent,
                                    _ => FeatureState.Unknown
                                };

                                results.Add(new WindowsOptionalFeatureItem
                                {
                                    FeatureName = capIdentity,
                                    DisplayName = FormatDisplayName(capIdentity),
                                    State = state,
                                    IsCapability = true,
                                    IsCritical = false
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Log(LogLevel.Warning, "WindowsOptionalFeaturesManager", $"DISM Capabilities query error: {ex.Message}");
            }

            return results;
        }

        private static string FormatDisplayName(string rawName)
        {
            if (rawName.StartsWith("Microsoft-Windows-", StringComparison.OrdinalIgnoreCase))
                rawName = rawName.Substring("Microsoft-Windows-".Length);

            if (rawName.Contains("~~~~"))
                rawName = rawName.Substring(0, rawName.IndexOf("~~~~"));

            return rawName.Replace("-", " ").Replace("OptionalFeature", "").Trim();
        }

        public static bool SetFeatureState(string featureName, bool enable, bool isCapability = false)
        {
            if (CriticalFeatures.Contains(featureName))
                return false;

            try
            {
                string action = isCapability
                    ? (enable ? $"/Online /Add-Capability /CapabilityName:\"{featureName}\"" : $"/Online /Remove-Capability /CapabilityName:\"{featureName}\"")
                    : (enable ? $"/Online /Enable-Feature /FeatureName:\"{featureName}\" /NoRestart" : $"/Online /Disable-Feature /FeatureName:\"{featureName}\" /NoRestart");

                var psi = new ProcessStartInfo
                {
                    FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "dism.exe"),
                    Arguments = action,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };

                using var proc = Process.Start(psi);
                if (proc != null)
                {
                    proc.WaitForExit();
                    bool success = proc.ExitCode == 0 || proc.ExitCode == 3010; // 3010 = restart required
                    StructuredLogger.Log(LogLevel.Info, "WindowsOptionalFeaturesManager", $"Set {featureName} to {(enable ? "Enabled" : "Disabled")}: ExitCode {proc.ExitCode}");
                    return success;
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Log(LogLevel.Error, "WindowsOptionalFeaturesManager", $"Failed to modify feature {featureName}: {ex.Message}");
            }

            return false;
        }
    }
}

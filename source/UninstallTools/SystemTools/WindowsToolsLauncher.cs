/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Windows Built-in Tools Launcher Subsystem
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UninstallTools.Core;

namespace UninstallTools.SystemTools
{
    public sealed class WindowsToolItem
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ExecutableOrUri { get; set; }
        public string Arguments { get; set; }
        public bool RequiresAdmin { get; set; }
        public string Category { get; set; }

        public override string ToString() => Name;
    }

    public static class WindowsToolsLauncher
    {
        public static IReadOnlyList<WindowsToolItem> GetAvailableTools()
        {
            var winDir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
            var sys32 = Path.Combine(winDir, "System32");

            return new List<WindowsToolItem>
            {
                new()
                {
                    Name = "Task Manager",
                    Description = "Monitor running processes, performance, app history, and startup impact.",
                    ExecutableOrUri = Path.Combine(sys32, "taskmgr.exe"),
                    RequiresAdmin = false,
                    Category = "Diagnostics"
                },
                new()
                {
                    Name = "Services Manager",
                    Description = "Manage background Windows services, startup types, and status.",
                    ExecutableOrUri = Path.Combine(sys32, "services.msc"),
                    RequiresAdmin = true,
                    Category = "System Management"
                },
                new()
                {
                    Name = "Device Manager",
                    Description = "View and update hardware devices, drivers, and peripheral properties.",
                    ExecutableOrUri = Path.Combine(sys32, "devmgmt.msc"),
                    RequiresAdmin = true,
                    Category = "Hardware"
                },
                new()
                {
                    Name = "Event Viewer",
                    Description = "Inspect Windows application, security, and system event logs.",
                    ExecutableOrUri = Path.Combine(sys32, "eventvwr.msc"),
                    RequiresAdmin = true,
                    Category = "Diagnostics"
                },
                new()
                {
                    Name = "Registry Editor",
                    Description = "View and edit the Windows registry hives and configuration keys.",
                    ExecutableOrUri = Path.Combine(winDir, "regedit.exe"),
                    RequiresAdmin = true,
                    Category = "System Management"
                },
                new()
                {
                    Name = "Disk Management",
                    Description = "Manage storage partitions, drive letters, formatting, and virtual disks.",
                    ExecutableOrUri = Path.Combine(sys32, "diskmgmt.msc"),
                    RequiresAdmin = true,
                    Category = "Hardware"
                },
                new()
                {
                    Name = "System Information",
                    Description = "Comprehensive summary of hardware, OS build, system drivers, and components.",
                    ExecutableOrUri = Path.Combine(sys32, "msinfo32.exe"),
                    RequiresAdmin = false,
                    Category = "Diagnostics"
                },
                new()
                {
                    Name = "Windows Optional Features",
                    Description = "Enable or disable optional Windows platform features and components.",
                    ExecutableOrUri = Path.Combine(sys32, "optionalfeatures.exe"),
                    RequiresAdmin = true,
                    Category = "System Management"
                },
                new()
                {
                    Name = "Windows Settings - Installed Apps",
                    Description = "Open Windows modern Settings app page for installed applications.",
                    ExecutableOrUri = "ms-settings:appsfeatures",
                    RequiresAdmin = false,
                    Category = "Applications"
                },
                new()
                {
                    Name = "System Restore",
                    Description = "Restore computer system files and settings to an earlier point in time.",
                    ExecutableOrUri = Path.Combine(sys32, "rstrui.exe"),
                    RequiresAdmin = true,
                    Category = "Recovery"
                },
                new()
                {
                    Name = "Command Prompt",
                    Description = "Open standard Windows command-line console.",
                    ExecutableOrUri = Path.Combine(sys32, "cmd.exe"),
                    RequiresAdmin = false,
                    Category = "Console"
                },
                new()
                {
                    Name = "PowerShell",
                    Description = "Open Windows PowerShell command environment.",
                    ExecutableOrUri = Path.Combine(sys32, "WindowsPowerShell", "v1.0", "powershell.exe"),
                    RequiresAdmin = false,
                    Category = "Console"
                },
                new()
                {
                    Name = "Windows Terminal",
                    Description = "Modern multi-tab terminal for Windows (if installed).",
                    ExecutableOrUri = "wt.exe",
                    RequiresAdmin = false,
                    Category = "Console"
                }
            };
        }

        public static bool LaunchTool(WindowsToolItem tool)
        {
            if (tool == null) return false;

            StructuredLogger.Info(LogCategory.General, $"Launching Windows Tool: {tool.Name} ({tool.ExecutableOrUri})");

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = tool.ExecutableOrUri,
                    Arguments = tool.Arguments ?? string.Empty,
                    UseShellExecute = true
                };

                if (tool.RequiresAdmin && !SecurityGuard.IsAdministrator())
                {
                    psi.Verb = "runas";
                }

                Process.Start(psi);
                return true;
            }
            catch (Exception ex)
            {
                StructuredLogger.Error(LogCategory.General, $"Failed to launch {tool.Name}", ex.Message);
                return false;
            }
        }
    }
}

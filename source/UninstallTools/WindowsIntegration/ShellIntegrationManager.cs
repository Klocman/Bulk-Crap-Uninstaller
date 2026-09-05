/*
    EBUninstaller Pro - Windows Explorer Shell Context Menu Integration
*/

using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using UninstallTools.Core;

namespace UninstallTools.WindowsIntegration
{
    public static class ShellIntegrationManager
    {
        private const string ContextMenuKeyName = "EBUninstaller.Pro";

        public static bool IsContextMenuInstalled()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return false;

            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(@"Software\Classes\Directory\shell\" + ContextMenuKeyName);
                return key != null;
            }
            catch
            {
                return false;
            }
        }

        public static bool InstallContextMenu(string exePath = null)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return false;

            try
            {
                var targetExe = exePath ?? Assembly.GetEntryAssembly()?.Location ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BCUninstaller.exe");

                // 1. Directory Context Menu
                using (var dirKey = Registry.CurrentUser.CreateSubKey($@"Software\Classes\Directory\shell\{ContextMenuKeyName}"))
                {
                    if (dirKey != null)
                    {
                        dirKey.SetValue("", "EBUninstaller - Forced Removal");
                        dirKey.SetValue("Icon", $"\"{targetExe}\",0");
                        using var cmdKey = dirKey.CreateSubKey("command");
                        cmdKey?.SetValue("", $"\"{targetExe}\" forced-uninstall \"%1\"");
                    }
                }

                // 2. Executable Files Context Menu (Monitor Installation)
                using (var exeKey = Registry.CurrentUser.CreateSubKey($@"Software\Classes\exefile\shell\EBUninstaller.Monitor"))
                {
                    if (exeKey != null)
                    {
                        exeKey.SetValue("", "EBUninstaller - Monitor Setup Installation");
                        exeKey.SetValue("Icon", $"\"{targetExe}\",0");
                        using var cmdKey = exeKey.CreateSubKey("command");
                        cmdKey?.SetValue("", $"\"{targetExe}\" monitor \"%1\"");
                    }
                }

                StructuredLogger.Info(LogCategory.General, "Windows Explorer shell context menu registered successfully.");
                return true;
            }
            catch (Exception ex)
            {
                StructuredLogger.Warning(LogCategory.General, "Failed to register shell context menu", ex.Message);
                return false;
            }
        }

        public static bool RemoveContextMenu()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return false;

            try
            {
                Registry.CurrentUser.DeleteSubKeyTree($@"Software\Classes\Directory\shell\{ContextMenuKeyName}", false);
                Registry.CurrentUser.DeleteSubKeyTree(@"Software\Classes\exefile\shell\EBUninstaller.Monitor", false);

                StructuredLogger.Info(LogCategory.General, "Windows Explorer shell context menu removed.");
                return true;
            }
            catch (Exception ex)
            {
                StructuredLogger.Warning(LogCategory.General, "Failed to remove shell context menu", ex.Message);
                return false;
            }
        }
    }
}

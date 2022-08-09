using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Klocman;
using Klocman.Extensions;
using Microsoft.Win32;

namespace ScriptHelper
{
    /*  Some scripts are based on https://github.com/W4RH4WK/Debloat-Windows-10/
     * 
     * ## Thanks To
     * 
     * - [10se1ucgo](https://github.com/10se1ucgo)
     * - [Plumebit](https://github.com/Plumebit)
     * - [aramboi](https://github.com/aramboi)
     * - [maci0](https://github.com/maci0)
     * - [narutards](https://github.com/narutards)
     * - [tumpio](https://github.com/tumpio)
     * 
     * ## License
     * 
     * "THE BEER-WARE LICENSE" (Revision 42):
     * 
     * As long as you retain this notice you can do whatever you want with this
     * stuff. If we meet some day, and you think this stuff is worth it, you can
     * buy us a beer in return.
     * 
     * This project is distributed in the hope that it will be useful, but WITHOUT
     * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
     * FITNESS FOR A PARTICULAR PURPOSE.
     */
    internal static class Tweaks
    {
        static Tweaks()
        {
            _tweaks = new List<TweakEntry>
            {
                new()
                {
                    DisplayName = "Easy Access Keyboard Shortcuts",
                    Publisher = "Microsoft Corporation",
                    SystemIcon = "Shield",
                    OnUninstall = _ =>
                    {
                        Registry.CurrentUser.OpenSubKey(@"Control Panel\Accessibility\StickyKeys")?.SetValue("Flags", "506", RegistryValueKind.String);
                        Registry.CurrentUser.OpenSubKey(@"Control Panel\Accessibility\Keyboard Response")?.SetValue("Flags", "122", RegistryValueKind.String);
                        Registry.CurrentUser.OpenSubKey(@"Control Panel\Accessibility\ToggleKeys")?.SetValue("Flags", "58", RegistryValueKind.String);
                    },
                    IsPresent = () => Registry.CurrentUser.OpenSubKey(@"Control Panel\Accessibility\StickyKeys")?.GetValue("Flags")?.ToString() != "506" ||
                                      Registry.CurrentUser.OpenSubKey(@"Control Panel\Accessibility\Keyboard Response")?.GetValue("Flags")?.ToString() != "122" ||
                                      Registry.CurrentUser.OpenSubKey(@"Control Panel\Accessibility\ToggleKeys")?.GetValue("Flags")?.ToString() != "58"
                },
                new()
                {
                    DisplayName = "Subscribed content (Auto-install suggested apps)",
                    Publisher = "Microsoft Corporation",
                    SystemIcon = "Shield",
                    OnUninstall = _ =>
                    {
                        Console.WriteLine(@"Disabling automatic downloading and installing of subscribed content");
                        var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager");
                        if (key != null)
                        {
                            key.SetValue("FeatureManagementEnabled", 0, RegistryValueKind.DWord);
                            key.SetValue("OemPreInstalledAppsEnabled", 0, RegistryValueKind.DWord);
                            key.SetValue("PreInstalledAppsEnabled", 0, RegistryValueKind.DWord);
                            key.SetValue("SilentInstalledAppsEnabled", 0, RegistryValueKind.DWord);
                            key.SetValue("ContentDeliveryAllowed", 0, RegistryValueKind.DWord);
                            key.SetValue("PreInstalledAppsEverEnabled", 0, RegistryValueKind.DWord);
                            key.SetValue("SubscribedContentEnabled", 0, RegistryValueKind.DWord);
                            key.SetValue("SubscribedContent-338388Enabled", 0, RegistryValueKind.DWord);
                            key.SetValue("SubscribedContent-338389Enabled", 0, RegistryValueKind.DWord);
                            key.SetValue("SubscribedContent-314559Enabled", 0, RegistryValueKind.DWord);
                            key.SetValue("SubscribedContent-338387Enabled", 0, RegistryValueKind.DWord);
                            key.SetValue("SubscribedContent-338393Enabled", 0, RegistryValueKind.DWord);
                            key.SetValue("SystemPaneSuggestionsEnabled", 0, RegistryValueKind.DWord);
                        }

                        Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Policies\Microsoft\WindowsStore")?.SetValue("AutoDownload", 2, RegistryValueKind.DWord);
                        Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Policies\Microsoft\Windows\CloudContent")
                            ?.SetValue("DisableWindowsConsumerFeatures", 1, RegistryValueKind.DWord);
                    },
                    IsPresent = () =>
                    {
                        // If ContentDeliveryManager doesn't exist it's likely not supported by the OS
                        // If SubscribedContentEnabled value doesn't exist assume it's turned on. It's disabled if set to 0
                        var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager");
                        return key != null && key.GetValue("SubscribedContentEnabled", 1)?.ToString() != "0";
                    }
                },
                new()
                {
                    DisplayName = "Decreased Wallpaper quality (high compression)",
                    Publisher = "Microsoft Corporation",
                    SystemIcon = "Shield",
                    OnUninstall = _ => { Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop")?.SetValue("JPEGImportQuality", 100, RegistryValueKind.DWord); },
                    IsPresent = () =>
                    {
                        var key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop");
                        // ReSharper disable once PossibleNullReferenceException
                        return key != null && (int)key.GetValue("JPEGImportQuality", 0) < 100;
                    }
                },
                new()
                {
                    DisplayName = "Mouse Acceleration",
                    Publisher = "Microsoft Corporation",
                    SystemIcon = "Shield",
                    OnUninstall = _ =>
                    {
                        var key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Mouse");
                        if (key != null)
                        {
                            Console.WriteLine(@"Disabling mouse acceleration");
                            key.SetValue("MouseSensitivity", "10", RegistryValueKind.String);
                            key.SetValue("MouseSpeed", "0", RegistryValueKind.String);
                            key.SetValue("MouseThreshold1", "0", RegistryValueKind.String);
                            key.SetValue("MouseThreshold2", "0", RegistryValueKind.String);

                            // Curves depend on display dpi and hz. They can be generated with MarkC's mouse acceleration fix
                            // They are only active with mouse acceleration turned on, so no reason to mess with them
                            //key.SetValue("SmoothMouseXCurve", new byte[]{0x00, 0x00, 0x00,
                            //    0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0xCC, 0x0C, 0x00, 0x00, 0x00, 0x00, 0x00,
                            //    0x80, 0x99, 0x19, 0x00, 0x00, 0x00, 0x00, 0x00, 0x40, 0x66, 0x26, 0x00, 0x00,
                            //    0x00, 0x00, 0x00, 0x00, 0x33, 0x33, 0x00, 0x00, 0x00, 0x00, 0x00}, RegistryValueKind.Binary);
                            //key.SetValue("SmoothMouseYCurve", new byte[]{0x00, 0x00, 0x00,
                            //    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x38, 0x00, 0x00, 0x00, 0x00, 0x00,
                            //    0x00, 0x00, 0x70, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xA8, 0x00, 0x00,
                            //    0x00, 0x00, 0x00, 0x00, 0x00, 0xE0, 0x00, 0x00, 0x00, 0x00, 0x00}, RegistryValueKind.Binary);
                        }
                    },
                    IsPresent = () => Registry.CurrentUser.OpenSubKey(@"Control Panel\Mouse")?.GetValue("MouseSpeed", "0")?.ToString() == "1"
                },
                new()
                {
                    DisplayName = "Showing Desktop under This PC",
                    Publisher = "Microsoft Corporation",
                    SystemIcon = "Shield",
                    OnUninstall = _ =>
                    {
                        Registry.LocalMachine.DeleteSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{B4BFCC3A-DB2C-424C-B029-7FE99A87C641}", false);
                        Registry.LocalMachine.DeleteSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{B4BFCC3A-DB2C-424C-B029-7FE99A87C641}", false);
                    },
                    IsPresent = () => Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{B4BFCC3A-DB2C-424C-B029-7FE99A87C641}") != null
                },
                new()
                {
                    DisplayName = "Showing Documents under This PC",
                    Publisher = "Microsoft Corporation",
                    SystemIcon = "Shield",
                    OnUninstall = _ =>
                    {
                        Registry.LocalMachine.DeleteSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{A8CDFF1C-4878-43be-B5FD-F8091C1C60D0}", false);
                        Registry.LocalMachine.DeleteSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{d3162b92-9365-467a-956b-92703aca08af}", false);
                        Registry.LocalMachine.DeleteSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{A8CDFF1C-4878-43be-B5FD-F8091C1C60D0}", false);
                        Registry.LocalMachine.DeleteSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{d3162b92-9365-467a-956b-92703aca08af}", false);
                    },
                    IsPresent = () => Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{A8CDFF1C-4878-43be-B5FD-F8091C1C60D0}") != null
                },
                new()
                {
                    DisplayName = "Showing Downloads under This PC",
                    Publisher = "Microsoft Corporation",
                    SystemIcon = "Shield",
                    OnUninstall = _ =>
                    {
                        Registry.LocalMachine.DeleteSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{374DE290-123F-4565-9164-39C4925E467B}", false);
                        Registry.LocalMachine.DeleteSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{088e3905-0323-4b02-9826-5d99428e115f}", false);
                        Registry.LocalMachine.DeleteSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{374DE290-123F-4565-9164-39C4925E467B}", false);
                        Registry.LocalMachine.DeleteSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{088e3905-0323-4b02-9826-5d99428e115f}", false);
                    },
                    IsPresent = () => Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{374DE290-123F-4565-9164-39C4925E467B}") != null
                },
                new()
                {
                    DisplayName = "Showing Music under This PC",
                    Publisher = "Microsoft Corporation",
                    SystemIcon = "Shield",
                    OnUninstall = _ =>
                    {
                        Registry.LocalMachine.DeleteSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{1CF1260C-4DD0-4ebb-811F-33C572699FDE}", false);
                        Registry.LocalMachine.DeleteSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{3dfdf296-dbec-4fb4-81d1-6a3438bcf4de}", false);
                        Registry.LocalMachine.DeleteSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{1CF1260C-4DD0-4ebb-811F-33C572699FDE}", false);
                        Registry.LocalMachine.DeleteSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{3dfdf296-dbec-4fb4-81d1-6a3438bcf4de}", false);
                    },
                    IsPresent = () => Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{1CF1260C-4DD0-4ebb-811F-33C572699FDE}") != null
                },
                new()
                {
                    DisplayName = "Showing Pictures under This PC",
                    Publisher = "Microsoft Corporation",
                    SystemIcon = "Shield",
                    OnUninstall = _ =>
                    {
                        Registry.LocalMachine.DeleteSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{3ADD1653-EB32-4cb0-BBD7-DFA0ABB5ACCA}", false);
                        Registry.LocalMachine.DeleteSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{24ad3ad4-a569-4530-98e1-ab02f9417aa8}", false);
                        Registry.LocalMachine.DeleteSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{3ADD1653-EB32-4cb0-BBD7-DFA0ABB5ACCA}", false);
                        Registry.LocalMachine.DeleteSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{24ad3ad4-a569-4530-98e1-ab02f9417aa8}", false);
                    },
                    IsPresent = () => Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{3ADD1653-EB32-4cb0-BBD7-DFA0ABB5ACCA}") != null
                },
                new()
                {
                    DisplayName = "Showing Videos under This PC",
                    Publisher = "Microsoft Corporation",
                    SystemIcon = "Shield",
                    OnUninstall = _ =>
                    {
                        Registry.LocalMachine.DeleteSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{A0953C92-50DC-43bf-BE83-3742FED03C9C}", false);
                        Registry.LocalMachine.DeleteSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{f86fa3ab-70d2-4fc7-9c99-fcbf05467f3a}", false);
                        Registry.LocalMachine.DeleteSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{A0953C92-50DC-43bf-BE83-3742FED03C9C}", false);
                        Registry.LocalMachine.DeleteSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{f86fa3ab-70d2-4fc7-9c99-fcbf05467f3a}", false);
                    },
                    IsPresent = () => Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{A0953C92-50DC-43bf-BE83-3742FED03C9C}") != null
                },
                new()
                {
                    DisplayName = "Showing 3D Objects under This PC",
                    Publisher = "Microsoft Corporation",
                    SystemIcon = "Shield",
                    OnUninstall = _ =>
                    {
                        Registry.LocalMachine.DeleteSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{0DB7E03F-FC29-4DC6-9020-FF41B59E513A}", false);
                        Registry.LocalMachine.DeleteSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{0DB7E03F-FC29-4DC6-9020-FF41B59E513A}", false);
                    },
                    IsPresent = () => Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{0DB7E03F-FC29-4DC6-9020-FF41B59E513A}") != null
                },
                /*new TweakEntry todo check if this still works and convert into a c# script
                {
                    DisplayName = "Microsoft OneDrive",
                    Publisher = "Microsoft Corporation",
                    SystemIcon = "Shield",
                    OnUninstall = _ =>
                    {
                        //# This script will remove and disable OneDrive integration.
                        // 
                        // Write-Output "Kill OneDrive process"
                        // taskkill.exe /F /IM "OneDrive.exe"
                        // taskkill.exe /F /IM "explorer.exe"
                        // 
                        // Write-Output "Remove OneDrive"
                        // if (Test-Path "$env:systemroot\System32\OneDriveSetup.exe") {
                        //     & "$env:systemroot\System32\OneDriveSetup.exe" /uninstall
                        // }
                        // if (Test-Path "$env:systemroot\SysWOW64\OneDriveSetup.exe") {
                        //     & "$env:systemroot\SysWOW64\OneDriveSetup.exe" /uninstall
                        // }
                        // 
                        // Write-Output "Removing OneDrive leftovers"
                        // Remove-Item -Recurse -Force -ErrorAction SilentlyContinue "$env:localappdata\Microsoft\OneDrive"
                        // Remove-Item -Recurse -Force -ErrorAction SilentlyContinue "$env:programdata\Microsoft OneDrive"
                        // Remove-Item -Recurse -Force -ErrorAction SilentlyContinue "$env:systemdrive\OneDriveTemp"
                        // # check if directory is empty before removing:
                        // If ((Get-ChildItem "$env:userprofile\OneDrive" -Recurse | Measure-Object).Count -eq 0) {
                        //     Remove-Item -Recurse -Force -ErrorAction SilentlyContinue "$env:userprofile\OneDrive"
                        // }
                        // 
                        // Write-Output "Disable OneDrive via Group Policies"
                        // force-mkdir "HKLM:\SOFTWARE\Wow6432Node\Policies\Microsoft\Windows\OneDrive"
                        // Set-ItemProperty "HKLM:\SOFTWARE\Wow6432Node\Policies\Microsoft\Windows\OneDrive" "DisableFileSyncNGSC" 1
                        // 
                        // Write-Output "Remove Onedrive from explorer sidebar"
                        // New-PSDrive -PSProvider "Registry" -Root "HKEY_CLASSES_ROOT" -Name "HKCR"
                        // mkdir -Force "HKCR:\CLSID\{018D5C66-4533-4307-9B53-224DE2ED1FE6}"
                        // Set-ItemProperty "HKCR:\CLSID\{018D5C66-4533-4307-9B53-224DE2ED1FE6}" "System.IsPinnedToNameSpaceTree" 0
                        // mkdir -Force "HKCR:\Wow6432Node\CLSID\{018D5C66-4533-4307-9B53-224DE2ED1FE6}"
                        // Set-ItemProperty "HKCR:\Wow6432Node\CLSID\{018D5C66-4533-4307-9B53-224DE2ED1FE6}" "System.IsPinnedToNameSpaceTree" 0
                        // Remove-PSDrive "HKCR"
                        // 
                        // # Thank you Matthew Israelsson
                        // Write-Output "Removing run hook for new users"
                        // reg load "hku\Default" "C:\Users\Default\NTUSER.DAT"
                        // reg delete "HKEY_USERS\Default\SOFTWARE\Microsoft\Windows\CurrentVersion\Run" /v "OneDriveSetup" /f
                        // reg unload "hku\Default"
                        // 
                        // Write-Output "Removing startmenu entry"
                        // Remove-Item -Force -ErrorAction SilentlyContinue "$env:userprofile\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\OneDrive.lnk"
                        // 
                        // Write-Output "Removing scheduled task"
                        // Get-ScheduledTask -TaskPath '\' -TaskName 'OneDrive*' -ea SilentlyContinue | Unregister-ScheduledTask -Confirm:$false
                        // 
                        // Write-Output "Restarting explorer"
                        // Start-Process "explorer.exe"
                        // 
                        // Write-Output "Waiting for explorer to complete loading"
                        // Start-Sleep 10
                        // 
                        // Write-Output "Removing additional OneDrive leftovers"
                        // foreach ($item in (Get-ChildItem "$env:WinDir\WinSxS\*onedrive*")) {
                        //     Takeown-Folder $item.FullName
                        //     Remove-Item -Recurse -Force $item.FullName
                        // }
                    },
                    IsPresent = () =>
                    {
                        var key = Registry.ClassesRoot.OpenSubKey(@"CLSID\{018D5C66-4533-4307-9B53-224DE2ED1FE6}");
                        if (key != null && (int)key.GetValue("System.IsPinnedToNameSpaceTree", 0) > 0) return true;

                        key = Registry.ClassesRoot.OpenSubKey(@"Wow6432Node\CLSID\{018D5C66-4533-4307-9B53-224DE2ED1FE6}");
                        return (key != null && (int)key.GetValue("System.IsPinnedToNameSpaceTree", 0) > 0);
                    }
                }*/
            };
        }

        internal record TweakEntry
        {
            public string DisplayName { get; init; }
            public string RatingId => "Tweak-" + DisplayName.ToPascalCase().RemoveSpecialCharacters();
            public string Publisher { get; init; }
            public string SystemIcon { get; init; }

            public Action<bool> OnUninstall { get; init; }
            public Func<bool> IsPresent { get; init; }
        }

        private static readonly List<TweakEntry> _tweaks;

        public static TweakEntry GetEntry(string ratingId) => _tweaks.FirstOrDefault(x => x.RatingId == ratingId);

        public static IEnumerable<Dictionary<string, string>> GetConsoleOutput()
        {
            var fileLoc = Path.Combine(Path.GetDirectoryName(typeof(Tweaks).Assembly.Location) ?? throw new InvalidOperationException("null Location"), "ScriptHelper.exe");

            foreach (var tweakEntry in _tweaks)
            {
                Dictionary<string, string> dic = null;
                try
                {
                    if (tweakEntry.IsPresent())
                    {
                        dic = new Dictionary<string, string>();
                        dic["DisplayName"] = tweakEntry.DisplayName;
                        dic["Publisher"] = tweakEntry.Publisher;
                        var ratingId = tweakEntry.RatingId;
                        dic["RatingId"] = ratingId;
                        var uninsStr = $"\"{fileLoc}\" uninstall {ratingId}";
                        dic["UninstallString"] = uninsStr;
                        dic["QuietUninstallString"] = uninsStr;
                    }
                }
                catch (Exception ex)
                {
                    LogWriter.WriteExceptionToLog(ex);
                    //Console.WriteLine(ex);
                    continue;
                }
                if (dic != null) yield return dic;
            }
        }
    }
}

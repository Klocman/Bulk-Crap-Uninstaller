/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using BulkCrapUninstaller.Properties;
using Klocman.Extensions;
using Klocman.Tools;
using Microsoft.Win32;
using UninstallTools;

namespace BulkCrapUninstaller
{
    //TODO This is a leftover class, extract the self installed detection logic and get rid of it
    public static class Program
    {
        private static string _applicationGuid;
        private static DirectoryInfo _assemblyLocation;
        private static string _installedRegistryKeyName;
        private static bool? _isInstalled;

        public static string ApplicationGuid
        {
            get
            {
                if (String.IsNullOrEmpty(_applicationGuid))
                {
                    var assembly = typeof(Program).Assembly;
                    var attribute = (GuidAttribute)assembly.GetCustomAttributes(typeof(GuidAttribute), true)[0];
                    _applicationGuid = attribute.Value;
                }
                return _applicationGuid;
            }
        }

        public static DirectoryInfo AssemblyLocation
        {
            get
            {
                if (_assemblyLocation == null)
                {
                    var location = Assembly.GetAssembly(typeof(Program)).Location;
                    if (location.Substring(location.LastIndexOf('\\')).Contains('.'))
                        location = PathTools.GetDirectory(location);
                    _assemblyLocation = new DirectoryInfo(location);
                }
                return _assemblyLocation;
            }
        }

        public static Version AssemblyVersion => Assembly.GetExecutingAssembly().GetName().Version;

        /// <summary>
        ///     Do not call before CheckForOldSettings() completes
        /// </summary>
        public static bool EnableDebug => Debugger.IsAttached || Settings.Default.Debug;

        /// <summary>
        /// Don't use settings
        /// </summary>
        public static string DbConnectionString => 
            Debugger.IsAttached ? Resources.DbDebugConnectionString : Resources.DbConnectionString;

        public static string InstalledRegistryKeyName
        {
            get
            {
                if (_installedRegistryKeyName == null)
                    GetInstalledRegKey();
                return _installedRegistryKeyName;
            }
        }

        public static bool IsAfterUpgrade { get; private set; }

        /// <summary>
        ///     Use setter to override the value
        /// </summary>
        public static bool IsInstalled
        {
            get
            {
                if (!_isInstalled.HasValue)
                    _isInstalled = InstalledRegistryKeyName.IsNotEmpty();
                return _isInstalled.Value;
            }
            internal set { _isInstalled = value; }
        }

        private static string ConfigFileFullname => Path.Combine(AssemblyLocation.FullName, @"BCUninstaller.settings");


        private static bool? _net4IsAvailable;
        public static Version PreviousVersion { get; private set; }

        public static bool Net4IsAvailable
        {
            get
            {
                if (!_net4IsAvailable.HasValue)
                {
                    try
                    {
                        using (var key = RegistryTools.OpenRegistryKey(
                            @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full"))
                        {
                            _net4IsAvailable = (int)key.GetValue("Install", 0) == 1;
                        }
                    }
                    catch
                    {
                        _net4IsAvailable = false;
                    }
                }
                return _net4IsAvailable.Value;
            }
        }

        /// <summary>
        ///     Remove old or invalid setting files and make sure settings are ready to be used.
        ///     Run before the settings are used, best at the very start of the application.
        /// </summary>
        internal static void PrepareSettings()
        {
            IsAfterUpgrade = false;
            try
            {
                var settingsXmlDocument = XDocument.Parse(File.ReadAllText(ConfigFileFullname));
                if (settingsXmlDocument.Root == null)
                    throw new FormatException();
                var result = settingsXmlDocument.Root.Element("MiscVersion");
                if (result == null || result.Value.Equals("Reset"))
                    throw new FormatException();

                PreviousVersion = new Version(result.Value);
                if (PreviousVersion < AssemblyVersion)
                    IsAfterUpgrade = true;
                //if(new Version(result) < )
            }
            catch
            {
                DeleteConfigFile();
            }

            // Initializes the settings object (unless it has been accessed before, which it shouldnt have)
            if (Settings.Default.MiscUserId == 0)
                Settings.Default.MiscUserId = WindowsTools.GetUniqueUserId();
        }

        private static void DeleteConfigFile()
        {
            File.Delete(ConfigFileFullname);
        }

        /// <summary>
        ///     TODO: A bit wonky, needs to be remade
        /// </summary>
        private static void GetInstalledRegKey()
        {
            const string regUninstallersKeyDirect = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";

            try
            {
                using (var regKey = Registry.LocalMachine.OpenSubKey(regUninstallersKeyDirect))
                {
                    if (regKey == null)
                        throw new ArgumentException("Could not open Software registry key");

                    var keyNames = regKey.GetSubKeyNames()
                        .Where(x => x.Contains(ApplicationGuid, StringComparison.InvariantCultureIgnoreCase));

                    foreach (var keyName in keyNames)
                    {
                        using (var subKey = regKey.OpenSubKey(keyName, true))
                        {
                            var installLocation = subKey?.GetValue(
                                ApplicationUninstallerEntry.RegistryNameInstallLocation) as string;
                            if (String.IsNullOrEmpty(installLocation)) continue;

                            var item1 = AssemblyLocation.FullName;
                            var item2 = installLocation.TrimEnd('\\');
                            if (item1.Equals(item2, StringComparison.InvariantCultureIgnoreCase))
                            {
                                // We are installed!
                                _installedRegistryKeyName = keyName;

                                // Update the version number
                                subKey.SetValue("DisplayVersion", AssemblyVersion.ToString(),
                                    RegistryValueKind.String);
                            }
                        }
                    }
                }
            }
            catch
            {
                _installedRegistryKeyName = String.Empty;
            }
        }

        public static void StartLogCleaner()
        {
            try
            {
                const string cleanerName = "CleanLogs.bat";
                var cleanerPath = Path.Combine(AssemblyLocation.FullName, cleanerName);

                if (!File.Exists(cleanerPath))
                {
                    Console.WriteLine(@"WARNING: CleanLogs.bat doesn't exist, can't clean logs.");
                    return;
                }
                
                var cleanerUri = PathToUri(cleanerPath);
                if (cleanerUri.IsUnc)
                {
                    // 'cmd.exe /c start' doesn't work with UNC paths, script has to run in foreground.
                    Process.Start(cleanerPath);
                }
                else
                {
                    // Run cleanup script in minimized cmd window
                    var ps = new ProcessStartInfo
                    {
                        WorkingDirectory = AssemblyLocation.FullName,
                        FileName = "cmd.exe",
                        Arguments = "/c start /min " + cleanerName,
                        UseShellExecute = true,
                        WindowStyle = ProcessWindowStyle.Minimized
                    };
                    Process.Start(ps);
                }
            }
            catch (Exception ex)
            {
                // Ignore errors, not critical
                Console.WriteLine(ex);
            }
        }

        private static Uri PathToUri(string filePath)
        {
            try
            {
                return new Uri(filePath);
            }
            catch (UriFormatException)
            {
                filePath = Path.GetFullPath(filePath);
                return new Uri(filePath);
            }
        }
    }
}
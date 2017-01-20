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
using UninstallTools.Uninstaller;

namespace BulkCrapUninstaller
{
    //TODO This is a leftover class, extract the self installed detection logic and get rid of it
    public static class Program
    {
        private static string _applicationGuid;
        private static DirectoryInfo _assemblyLocation;
        private static string _installedRegistryKeyName;
        private static bool? _isInstalled;
        private static string _dbConnectionString;

        public static string ApplicationGuid
        {
            get
            {
                if (string.IsNullOrEmpty(_applicationGuid))
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

        public static string DbConnectionString
        {
            get
            {
                return _dbConnectionString ??
                       (_dbConnectionString =
                           EnableDebug ? Resources.DbDebugConnectionString : Resources.DbConnectionString);
            }
            set { _dbConnectionString = value; }
        }

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

                if (new Version(result.Value) < AssemblyVersion)
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
            try
            {
                using (var regKey = Registry.LocalMachine.OpenSubKey(
                    ApplicationUninstallerManager.RegUninstallersKeyDirect))
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
                            if (string.IsNullOrEmpty(installLocation)) continue;

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
                _installedRegistryKeyName = string.Empty;
            }
        }
    }
}
/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using Klocman.Extensions;
using Klocman.Tools;
using Microsoft.Win32;
using UninstallTools.Junk.Confidence;
using UninstallTools.Junk.Containers;
using UninstallTools.Properties;

namespace UninstallTools.Junk.Finders.Registry
{
    public class SoftwareRegKeyScanner : JunkCreatorBase
    {
        public override string CategoryName => Localisation.Junk_Registry_GroupName;

        private const string KeynameRegisteredApps = "RegisteredApplications";

        private const string KeyVirtualStoreCu = @"HKEY_CURRENT_USER\SOFTWARE\Classes\VirtualStore\MACHINE\SOFTWARE";

        private const string KeyVirtualStoreCuWow =
            @"HKEY_CURRENT_USER\SOFTWARE\Classes\VirtualStore\MACHINE\SOFTWARE\Wow6432Node";

        private const string KeyVirtualStoreLm = @"HKEY_LOCAL_MACHINE\SOFTWARE\Classes\VirtualStore\MACHINE\SOFTWARE";

        private const string KeyVirtualStoreLmWow =
            @"HKEY_LOCAL_MACHINE\SOFTWARE\Classes\VirtualStore\MACHINE\SOFTWARE\Wow6432Node";

        /// <summary>
        /// Keys to step over when scanning
        /// </summary>
        private static readonly IEnumerable<string> KeyBlacklist = new[]
        {
            "Microsoft", "Wow6432Node", "Windows", "Classes", "Clients", KeynameRegisteredApps
        };

        /// <summary>
        /// Always points to program's directory
        /// </summary>
        private static readonly IEnumerable<string> InstallDirKeyNames = new[]
        {
            "InstallDir",
            "Install_Dir",
            "Install Directory",
            "InstDir",
            "ApplicationPath",
            "Install folder",
            "Last Stable Install Path",
            "TARGETDIR",
            "JavaHome"
        };

        /// <summary>
        /// Always points to program's main executable
        /// </summary>
        private static readonly IEnumerable<string> ExePathKeyNames = new[]
        {
            "exe64",
            "exe32",
            "Executable",
            "PathToExe",
            "ExePath"
        };

        /// <summary>
        /// Can point to programs executable or directory
        /// </summary>
        private static readonly IEnumerable<string> ExeOrDirPathKeyNames = new[]
        {
            "Path",
            "Path64",
            "pth",
            "PlayerPath",
            "AppPath"
        };

        internal static readonly string KeyCu = @"HKEY_CURRENT_USER\SOFTWARE";
        internal static readonly string KeyCuWow = @"HKEY_CURRENT_USER\SOFTWARE\Wow6432Node";
        internal static readonly string KeyLm = @"HKEY_LOCAL_MACHINE\SOFTWARE";
        internal static readonly string KeyLmWow = @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node";

        private static readonly ICollection<string> SoftwareRegKeys;

        private ApplicationUninstallerEntry _uninstaller;

        static SoftwareRegKeyScanner()
        {
            if (ProcessTools.Is64BitProcess)
            {
                SoftwareRegKeys = new[]
                {
                    KeyLm, KeyCu, KeyVirtualStoreCu, KeyVirtualStoreLm,
                    KeyLmWow, KeyCuWow, KeyVirtualStoreCuWow, KeyVirtualStoreLmWow
                };
            }
            else
            {
                SoftwareRegKeys = new[] { KeyLm, KeyCu, KeyVirtualStoreCu, KeyVirtualStoreLm };
            }
        }

        public override IEnumerable<IJunkResult> FindJunk(ApplicationUninstallerEntry target)
        {
            _uninstaller = target;
            var output = new List<RegistryKeyJunk>();

            foreach (var softwareKeyName in SoftwareRegKeys)
            {
                using (var softwareKey = RegistryTools.OpenRegistryKey(softwareKeyName))
                {
                    if (softwareKey != null)
                        output.AddRange(FindJunkRecursively(softwareKey));
                }
            }

            return output.Concat(ScanRelatedKeys(output));
        }

        private IEnumerable<RegistryKeyJunk> FindJunkRecursively(RegistryKey softwareKey, int level = -1)
        {
            var added = new List<RegistryKeyJunk>();
            IEnumerable<RegistryKeyJunk> returnList = added;

            try
            {
                // Don't try to scan root keys
                if (level > -1)
                {
                    var keyName = Path.GetFileName(softwareKey.Name);
                    var keyDir = Path.GetDirectoryName(softwareKey.Name);
                    var confidence = ConfidenceGenerators.GenerateConfidence(keyName, keyDir, level, _uninstaller).ToList();

                    // Check if application's location is explicitly mentioned in any of the values
                    if (softwareKey.TryGetValueNames().Any(valueName => TestValueForMatches(softwareKey, valueName)))
                        confidence.Add(ConfidenceRecords.ExplicitConnection);

                    if (confidence.Any())
                    {
                        // TODO Add extra confidence if the key is, or will be empty after junk removal
                        var newNode = new RegistryKeyJunk(softwareKey.Name, _uninstaller, this);
                        newNode.Confidence.AddRange(confidence);
                        added.Add(newNode);
                    }
                }

                // Limit recursion depth
                if (level <= 1)
                {
                    foreach (var subKeyName in softwareKey.GetSubKeyNames())
                    {
                        if (KeyBlacklist.Contains(subKeyName, StringComparison.InvariantCultureIgnoreCase))
                            continue;

                        using (var subKey = softwareKey.OpenSubKey(subKeyName, false))
                        {
                            if (subKey != null)
                                // ReSharper disable once PossibleMultipleEnumeration
                                returnList = returnList.Concat(FindJunkRecursively(subKey, level + 1));
                        }
                    }
                }

                ConfidenceGenerators.TestForSimilarNames(_uninstaller, AllUninstallers, added.Select(x => new KeyValuePair<JunkResultBase, string>(x, x.RegKeyName)).ToList());
            }
            // Reg key invalid
            catch (ArgumentException)
            {
            }
            catch (SecurityException)
            {
            }
            catch (ObjectDisposedException)
            {
            }

            // ReSharper disable once PossibleMultipleEnumeration
            return returnList;
        }

        private bool TestValueForMatches(RegistryKey softwareKey, string valueName)
        {
            bool hit;
            if (InstallDirKeyNames.Contains(valueName, StringComparison.InvariantCultureIgnoreCase))
            {
                hit = PathTools.SubPathIsInsideBasePath(_uninstaller.InstallLocation, softwareKey.GetStringSafe(valueName), true);
            }
            else if (ExePathKeyNames.Contains(valueName, StringComparison.InvariantCultureIgnoreCase))
            {
                hit = TestPathsMatchExe(softwareKey.GetStringSafe(valueName));
            }
            else if (ExeOrDirPathKeyNames.Contains(valueName, StringComparison.InvariantCultureIgnoreCase))
            {
                var path = softwareKey.GetStringSafe(valueName);
                hit = File.Exists(path)
                    ? TestPathsMatchExe(softwareKey.GetStringSafe(valueName))
                    : PathTools.SubPathIsInsideBasePath(_uninstaller.InstallLocation, softwareKey.GetStringSafe(valueName), true);
            }
            else
            {
                hit = PathTools.SubPathIsInsideBasePath(_uninstaller.InstallLocation, softwareKey.GetStringSafe(null), true);
            }

            return hit;
        }

        private IEnumerable<RegistryKeyJunk> ScanRelatedKeys(IEnumerable<RegistryKeyJunk> itemsToCompare)
        {
            var input = itemsToCompare.ToList();
            var output = new List<RegistryKeyJunk>();

            foreach (var registryJunkNode in input)
            {
                var nodeName = registryJunkNode.FullRegKeyPath;

                // Check Wow first because non-wow path will match wow path
                var softwareKey = new[] { KeyLmWow, KeyCuWow, KeyLm, KeyCu }.First(
                    key => nodeName.StartsWith(key, StringComparison.InvariantCultureIgnoreCase));

                nodeName = nodeName.Substring(softwareKey.Length + 1);

                foreach (var keyToTest in SoftwareRegKeys.Except(new[] { softwareKey }))
                {
                    var nodePath = Path.Combine(keyToTest, nodeName);
                    // Check if the same node exists in other root keys
                    var node = input.FirstOrDefault(x => PathTools.PathsEqual(x.FullRegKeyPath, nodePath));

                    if (node != null)
                    {
                        // Add any non-duplicate confidence to the existing node
                        node.Confidence.AddRange(registryJunkNode.Confidence.ConfidenceParts
                            .Where(x => !node.Confidence.ConfidenceParts.Any(x.Equals)));
                    }
                    else
                    {
                        try
                        {
                            // Check if the key acually exists
                            using (var nodeKey = RegistryTools.OpenRegistryKey(nodePath, false))
                            {
                                if (nodeKey != null)
                                {
                                    var newNode = new RegistryKeyJunk(nodePath, _uninstaller, this);
                                    newNode.Confidence.AddRange(registryJunkNode.Confidence.ConfidenceParts);
                                    output.Add(newNode);
                                }
                            }
                        }
                        catch
                        {
                            // Ignore keys that don't exist
                        }
                    }
                }
            }

            return output;
        }

        private bool TestPathsMatchExe(string keyValue)
        {
            return PathTools.SubPathIsInsideBasePath(_uninstaller.InstallLocation, Path.GetDirectoryName(keyValue), true);
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using Klocman.Tools;
using Microsoft.Win32;
using UninstallTools.Uninstaller;
using Klocman.Extensions;
namespace UninstallTools.Junk
{
    public class RegistryJunk : JunkBase
    {
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
            "exe64"       ,
            "exe32"       ,
            "Executable"  ,
            "PathToExe"   ,
            "ExePath"
        };

        /// <summary>
        /// Can point to programs executable or directory
        /// </summary>
        private static readonly IEnumerable<string> ExeOrDirPathKeyNames = new[]
        {
            "Path"        ,
            "Path64"      ,
            "pth"         ,
            "PlayerPath"  ,
            "AppPath"
        };

        private const string KeynameRegisteredApps = "RegisteredApplications";

        private static readonly string KeyCu = @"HKEY_CURRENT_USER\SOFTWARE";
        private static readonly string KeyCuWow = @"HKEY_CURRENT_USER\SOFTWARE\Wow6432Node";
        private static readonly string KeyLm = @"HKEY_LOCAL_MACHINE\SOFTWARE";
        private static readonly string KeyLmWow = @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node";

        public RegistryJunk(ApplicationUninstallerEntry entry,
            IEnumerable<ApplicationUninstallerEntry> otherUninstallers)
            : base(entry, otherUninstallers)
        {
        }

        public override IEnumerable<JunkNode> FindJunk()
        {
            var returnList = new List<RegistryJunkNode>();

            // Look for junk
            foreach (var softwareKeyName in GetSoftwareRegKeys())
            {
                using (var softwareKey = RegistryTools.OpenRegistryKey(softwareKeyName))
                {
                    if(softwareKey != null)
                        returnList.AddRange(FindJunkRecursively(softwareKey));
                }
            }

            // Check other root keys for junk based on what was already found
            foreach (var registryJunkNode in returnList.ToList())
            {
                var nodeName = registryJunkNode.FullName;

                // Check Wow first because non-wow path will match wow path
                var softwareKey = new[] { KeyLmWow, KeyCuWow, KeyLm, KeyCu }.First(
                    key => nodeName.StartsWith(key, StringComparison.InvariantCultureIgnoreCase));

                nodeName = nodeName.Substring(softwareKey.Length + 1);

                foreach (var keyToTest in GetSoftwareRegKeys().Except(new[] { softwareKey }))
                {
                    var nodePath = Path.Combine(keyToTest, nodeName);
                    // Check if the same node exists in other root keys
                    var node = returnList.FirstOrDefault(x => PathTools.PathsEqual(x.FullName, nodePath));

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
                                    var newNode = new RegistryJunkNode(Path.GetDirectoryName(nodePath),
                                        Path.GetFileName(nodePath), Uninstaller.DisplayName);
                                    newNode.Confidence.AddRange(registryJunkNode.Confidence.ConfidenceParts);
                                    returnList.Add(newNode);
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

            if (Uninstaller.RegKeyStillExists())
            {
                var regKeyNode = new RegistryJunkNode(PathTools.GetDirectory(Uninstaller.RegistryPath),
                    Uninstaller.RegistryKeyName, Uninstaller.DisplayName);
                regKeyNode.Confidence.Add(ConfidencePart.IsUninstallerRegistryKey);
                returnList.Add(regKeyNode);
            }

            return returnList.Cast<JunkNode>();
        }

        private static string[] GetSoftwareRegKeys()
        {
            return ProcessTools.Is64BitProcess ? new[] {KeyLm, KeyCu, KeyLmWow, KeyCuWow} : new[] { KeyLm, KeyCu };
        }

        private IEnumerable<RegistryJunkNode> FindJunkRecursively(RegistryKey softwareKey, int level = -1)
        {
            var returnList = new List<RegistryJunkNode>();

            try
            {
                // Don't try to scan root keys
                if (level > -1)
                {
                    var keyName = Path.GetFileName(softwareKey.Name);
                    var keyDir = Path.GetDirectoryName(softwareKey.Name);
                    var confidence = GenerateConfidence(keyName, keyDir, level).ToList();

                    // Check if application's location is explicitly mentioned in any of the values
                    // TODO Check default value too, but with lower confidence
                    foreach (var valueName in softwareKey.GetValueNames())
                    {
                        var hit = false;

                        if (InstallDirKeyNames.Contains(valueName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            hit = TestPathsEqual(softwareKey.GetValue(valueName) as string);
                        }
                        else if (ExePathKeyNames.Contains(valueName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            hit = TestPathsEqualExe(softwareKey.GetValue(valueName) as string);
                        }
                        else if (ExeOrDirPathKeyNames.Contains(valueName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            var path = softwareKey.GetValue(valueName) as string;
                            hit = File.Exists(path)
                                ? TestPathsEqualExe(softwareKey.GetValue(valueName) as string)
                                : TestPathsEqual(softwareKey.GetValue(valueName) as string);
                        }

                        if (hit)
                        {
                            confidence.Add(ConfidencePart.ExplicitConnection);
                            break;
                        }
                    }

                    if (confidence.Any())
                    {
                        var newNode = new RegistryJunkNode(keyDir, keyName, Uninstaller.DisplayName);
                        newNode.Confidence.AddRange(confidence);
                        returnList.Add(newNode);
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
                            if(subKey != null)
                                returnList.AddRange(FindJunkRecursively(subKey, level + 1));
                        }
                    }
                }
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

            return returnList;
        }

        private bool TestPathsEqualExe(string keyValue)
        {
            return TestPathsEqual(Path.GetDirectoryName(keyValue));
        }

        private bool TestPathsEqual(string keyValue)
        {
            return PathTools.PathsEqual(Uninstaller.InstallLocation, keyValue);
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using Klocman.Tools;
using UninstallTools.Uninstaller;

namespace UninstallTools.Junk
{
    public class RegistryJunk : JunkBase
    {
        private static readonly IEnumerable<string> KeyBlacklist = new[]
        {"Microsoft", "Wow6432Node", "Windows", "Classes", "Clients"};

        private static readonly string KeyCu = @"HKEY_CURRENT_USER\SOFTWARE";
        private static readonly string KeyCuWow = @"HKEY_CURRENT_USER\SOFTWARE\Wow6432Node";
        private static readonly string KeyLm = @"HKEY_LOCAL_MACHINE\SOFTWARE";
        private static readonly string KeyLmWow = @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node";

        public RegistryJunk(ApplicationUninstallerEntry entry, IEnumerable<ApplicationUninstallerEntry> otherUninstallers)
            : base(entry, otherUninstallers)
        {
        }

        public override IEnumerable<JunkNode> FindJunk()
        {
            var softwareKeys = GetSoftwareRegKeys(Uninstaller.Is64Bit);
            var returnList = new List<RegistryJunkNode>();
            foreach (var softwareKey in softwareKeys)
            {
                FindJunkRecursively(returnList, softwareKey, 0);
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

        private static string[] GetSoftwareRegKeys(bool get64Bit)
        {
            var returnVal = new string[2];
            if (ProcessTools.Is64BitProcess)
            {
                if (get64Bit)
                {
                    returnVal[0] = KeyLm;
                    returnVal[1] = KeyCu;
                }
                else
                {
                    returnVal[0] = KeyLmWow;
                    returnVal[1] = KeyCuWow;
                }
            }
            else
            {
                returnVal[0] = KeyLm;
                returnVal[1] = KeyCu;
            }

            return returnVal;
        }

        private void FindJunkRecursively(List<RegistryJunkNode> returnList, string softwareKey, int level)
        {
            try
            {
                string[] names;
                using (var key = RegistryTools.OpenRegistryKey(softwareKey))
                {
                    names = key.GetSubKeyNames();
                }

                foreach (var name in names)
                {
                    if (KeyBlacklist.Any(y => y.Equals(name)))
                        continue;

                    var generatedConfidence = GenerateConfidence(name, softwareKey, level, false);
                    var confidenceParts = generatedConfidence as IList<ConfidencePart> ?? generatedConfidence.ToList();

                    if (confidenceParts.Any())
                    {
                        var newNode = new RegistryJunkNode(softwareKey, name, Uninstaller.DisplayName);
                        newNode.Confidence.AddRange(confidenceParts);
                        returnList.Add(newNode);
                    }
                    else if (level <= 1)
                    {
                        FindJunkRecursively(returnList, Path.Combine(softwareKey, name), level + 1);
                    }
                }
            }
            // Reg key invalid
            catch (ArgumentException) { }
            catch (SecurityException) { }
            catch (ObjectDisposedException) { }
        }
    }
}
/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Klocman.Extensions;
using Klocman.IO;
using Klocman.Tools;
using UninstallTools.Junk.Confidence;
using UninstallTools.Junk.Containers;
using UninstallTools.Properties;

namespace UninstallTools.Junk.Finders.Registry
{
    public class UninstallerKeySearcher : IJunkCreator
    {
        private static readonly IEnumerable<string> InstallerSubkeyPaths;

        /// <summary>
        /// parent key path, upgrade code(key name)
        /// </summary>
        private IEnumerable<KeyValuePair<string, string>> _targetKeys;

        static UninstallerKeySearcher()
        {
            InstallerSubkeyPaths = new[]
            {
                @"SOFTWARE\Classes\Installer\Products",
                @"SOFTWARE\Classes\Installer\Features",
                @"SOFTWARE\Classes\Installer\Patches"
            };

            try
            {
                var currentUserId = WindowsTools.GetUserSid().Value;
                if (string.IsNullOrEmpty(currentUserId) || currentUserId.Length <= 9)
                    return;

                var currentUserInstallerDataPath = Path.Combine(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Installer\UserData",
                    currentUserId);

                InstallerSubkeyPaths = InstallerSubkeyPaths.Concat(new []
                {
                    Path.Combine(currentUserInstallerDataPath, "Products"),
                    Path.Combine(currentUserInstallerDataPath, "Patches"),
                    Path.Combine(currentUserInstallerDataPath, "Components")
                });
            }
            catch (SystemException ex)
            {
                Trace.WriteLine(ex);
            }
        }

        public void Setup(ICollection<ApplicationUninstallerEntry> allUninstallers)
        {
            _targetKeys = InstallerSubkeyPaths
                .Using(x => Microsoft.Win32.Registry.LocalMachine.OpenSubKey(x))
                .Where(k => k != null)
                .SelectMany(k =>
                {
                    var parentPath = k.Name;
                    return k.GetSubKeyNames().Select(n => new KeyValuePair<string, string>(parentPath, n));
                }).ToList();
        }

        public IEnumerable<IJunkResult> FindJunk(ApplicationUninstallerEntry target)
        {
            if (target.RegKeyStillExists())
            {
                var regKeyNode = new RegistryKeyJunk(target.RegistryPath, target, this);
                regKeyNode.Confidence.Add(ConfidenceRecords.IsUninstallerRegistryKey);
                yield return regKeyNode;
            }

            if (target.UninstallerKind == UninstallerType.Msiexec && !target.BundleProviderKey.IsEmpty())
            {
                var upgradeKey = MsiTools.ConvertBetweenUpgradeAndProductCode(target.BundleProviderKey).ToString("N");

                var matchedKeyPaths = _targetKeys
                    .Where(x => x.Value.Equals(upgradeKey, StringComparison.OrdinalIgnoreCase));

                foreach (var keyPath in matchedKeyPaths)
                {
                    var fullKeyPath = Path.Combine(keyPath.Key, keyPath.Value);
                    var result = new RegistryKeyJunk(fullKeyPath, target, this);
                    result.Confidence.Add(ConfidenceRecords.ExplicitConnection);
                    yield return result;
                }
            }
        }

        public string CategoryName => Localisation.Junk_UninstallerKey_GroupName;
    }
}
/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Klocman.Extensions;
using Klocman.Tools;
using UninstallTools.Junk.Confidence;
using UninstallTools.Junk.Containers;

namespace UninstallTools.Junk.Finders.Registry
{
    public class RegisteredApplicationsFinder : JunkCreatorBase
    {
        private struct RegAppEntry
        {
            public RegAppEntry(string valueName, string rootKeyName, string targetSubKeyPath)
            {
                ValueName = valueName;
                RootKeyName = rootKeyName;
                TargetSubKeyPath = targetSubKeyPath;

                AppName = AppKey = null;

                if (valueName.Length == 36 && valueName.StartsWith("App", StringComparison.Ordinal))
                {
                    var pathParts = targetSubKeyPath.Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = pathParts.Length - 2; i >= 8; i--)
                    {
                        if (pathParts[i] == "Packages")
                        {
                            AppName = pathParts[i + 1];
                            AppKey = string.Join("\\", pathParts, 0, i + 2);
                            break;
                        }
                    }
                }
            }

            public string ValueName { get; }
            public string TargetSubKeyPath { get; }
            public string RootKeyName { get; }
            public string TargetFullPath => Path.Combine(RootKeyName, TargetSubKeyPath);
            public string RegAppFullPath => Path.Combine(RootKeyName, RegAppsSubKeyPath);
            public string AppName { get; }
            public string AppKey { get; }
        }

        private const string RegAppsSubKeyPath = @"Software\RegisteredApplications";
        List<RegAppEntry> _regAppsValueCache;

        public override void Setup(ICollection<ApplicationUninstallerEntry> allUninstallers)
        {
            base.Setup(allUninstallers);

            // Preload all values into a new cache
            _regAppsValueCache = new List<RegAppEntry>();

            foreach (var targetRootName in TargetRoots)
            {
                using (var rootKey = RegistryTools.OpenRegistryKey(targetRootName))
                {
                    using (var regAppsKey = rootKey.OpenSubKey(RegAppsSubKeyPath))
                    {
                        if (regAppsKey == null) continue;

                        var names = regAppsKey.GetValueNames();

                        var results = names.Attempt(n => new { name = n, value = regAppsKey.GetStringSafe(n) })
                                .Where(x => !string.IsNullOrEmpty(x.value))
                                .ToList();

                        _regAppsValueCache.AddRange(results.Select(x => new RegAppEntry(x.name, targetRootName, x.value.Trim('\\', ' ', '"', '\''))));
                    }
                }
            }
        }

        private static readonly string[] TargetRoots = { @"HKEY_CURRENT_USER\", @"HKEY_LOCAL_MACHINE\" };

        public override IEnumerable<IJunkResult> FindJunk(ApplicationUninstallerEntry target)
        {
            var isStoreApp = target.UninstallerKind == UninstallerType.StoreApp;
            if (isStoreApp && string.IsNullOrEmpty(target.RatingId))
                throw new ArgumentException("StoreApp entry has no ID");

            if (isStoreApp)
            {
                foreach (var regAppEntry in _regAppsValueCache)
                {
                    if (regAppEntry.AppName == null)
                        continue;

                    if (string.Equals(regAppEntry.AppName, target.RatingId, StringComparison.OrdinalIgnoreCase))
                    {
                        // Handle the value under RegisteredApps itself
                        var regAppResult = new RegistryValueJunk(regAppEntry.RegAppFullPath, regAppEntry.ValueName, target, this);
                        regAppResult.Confidence.Add(ConfidenceRecords.ExplicitConnection);
                        yield return regAppResult;

                        // Handle the key pointed at by the value
                        var appEntryKey = new RegistryKeyJunk(regAppEntry.AppKey, target, this);
                        appEntryKey.Confidence.Add(ConfidenceRecords.ExplicitConnection);
                        appEntryKey.Confidence.Add(ConfidenceRecords.IsStoreApp);
                        yield return appEntryKey;
                    }
                }
            }
            else
            {
                foreach (var regAppEntry in _regAppsValueCache)
                {
                    if (regAppEntry.AppName != null)
                        continue;

                    var generatedConfidence = ConfidenceGenerators.GenerateConfidence(regAppEntry.ValueName, target).ToList();

                    if (generatedConfidence.Count > 0)
                    {
                        // Handle the value under RegisteredApps itself
                        var regAppResult = new RegistryValueJunk(regAppEntry.RegAppFullPath, regAppEntry.ValueName, target, this);
                        regAppResult.Confidence.AddRange(generatedConfidence);
                        yield return regAppResult;

                        // Handle the key pointed at by the value
                        const string capabilitiesSubkeyName = "\\Capabilities";
                        if (regAppEntry.TargetSubKeyPath.EndsWith(capabilitiesSubkeyName, StringComparison.Ordinal))
                        {
                            var capabilitiesKeyResult = new RegistryKeyJunk(regAppEntry.TargetFullPath, target, this);
                            capabilitiesKeyResult.Confidence.AddRange(generatedConfidence);
                            yield return capabilitiesKeyResult;

                            var ownerKey = regAppEntry.TargetFullPath.Substring(0,
                                regAppEntry.TargetFullPath.Length - capabilitiesSubkeyName.Length);

                            var subConfidence = ConfidenceGenerators.GenerateConfidence(Path.GetFileName(ownerKey),
                                target).ToList();
                            if (subConfidence.Count > 0)
                            {
                                var subResult = new RegistryKeyJunk(ownerKey, target, this);
                                subResult.Confidence.AddRange(subConfidence);
                                yield return subResult;
                            }
                        }
                    }
                }
            }
        }

        public override string CategoryName => "Registered app capabilities";
    }
}

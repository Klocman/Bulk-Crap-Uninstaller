/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.IO;
using Klocman.Native;
using Klocman.Tools;
using Microsoft.Win32;
using UninstallTools.Junk.Confidence;
using UninstallTools.Junk.Containers;
using UninstallTools.Properties;

namespace UninstallTools.Junk.Finders.Registry
{
    public class ClsidScanner : JunkCreatorBase
    {
        private static readonly string WindowsDirectory = WindowsTools.GetEnvironmentPath(CSIDL.CSIDL_WINDOWS);

        private static readonly string[] ClsidKeys =
        {
            @"HKEY_LOCAL_MACHINE\SOFTWARE\Classes\CLSID",
            @"HKEY_LOCAL_MACHINE\SOFTWARE\Classes\WOW6432Node\CLSID",
            @"HKEY_CURRENT_USER\SOFTWARE\Classes\CLSID",
            @"HKEY_CURRENT_USER\SOFTWARE\Classes\WOW6432Node\CLSID"
        };

        private ICollection<KeyValuePair<string, string>> _clsudEntries;

        public override void Setup(ICollection<ApplicationUninstallerEntry> allUninstallers)
        {
            base.Setup(allUninstallers);
            _clsudEntries = CollectClsidEnries();
        }

        private static List<KeyValuePair<string, string>> CollectClsidEnries()
        {
            var results = new List<KeyValuePair<string, string>>();

            foreach (var keyName in ClsidKeys)
            {
                using (var key = RegistryTools.OpenRegistryKey(keyName))
                {
                    if (key == null)
                        continue;

                    string[] subKeyNames;
                    try
                    {
                        subKeyNames = key.GetSubKeyNames();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        continue;
                    }

                    foreach (var x in subKeyNames)
                    {
                        var subKeyName = x.TrimEnd('\"'); // For some reason GetSubKeyNames puts quotes at end sometimes
                        RegistryKey subKey = null;

                        try
                        {
                            subKey = key.OpenSubKey(Path.Combine(subKeyName, "InprocServer32"));

                            var path = subKey?.GetValue(null) as string;
                            if (string.IsNullOrEmpty(path)) continue;

                            path = Environment.ExpandEnvironmentVariables(path).Trim('\"');
                            if (!Path.IsPathRooted(path) || SubPathIsInsideBasePath(WindowsDirectory, path))
                                continue;

                            path = Path.GetDirectoryName(path);
                            if (string.IsNullOrEmpty(path)) continue;

                            results.Add(new KeyValuePair<string, string>(Path.Combine(key.Name, subKeyName), path));
                        }
                        catch (Exception ex)
                        {
                            // TODO better handling?
                            Console.WriteLine(ex);
                        }
                        finally
                        {
                            subKey?.Close();
                        }
                    }
                }
            }

            return results;
        }

        public override IEnumerable<IJunkResult> FindJunk(ApplicationUninstallerEntry target)
        {
            var results = new List<IJunkResult>();

            foreach (var entry in _clsudEntries)
            {
                if (SubPathIsInsideBasePath(target.InstallLocation, entry.Value))
                {
                    var node = new RegistryKeyJunk(entry.Key, target, this);
                    node.Confidence.Add(ConfidenceRecords.ExplicitConnection);
                    results.Add(node);
                }
            }

            return results;
        }

        public override string CategoryName => Localisation.Junk_Clsid_GroupName;
    }
}
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

namespace UninstallTools.Junk
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

        public override IEnumerable<JunkNode> FindJunk(ApplicationUninstallerEntry target)
        {
            var results = new List<JunkNode>();

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

                            if (string.IsNullOrEmpty(path))
                                continue;

                            path = Environment.ExpandEnvironmentVariables(path).Trim('\"');

                            if (!Path.IsPathRooted(path) || SubPathIsInsideBasePath(WindowsDirectory, path))
                                continue;

                            if (SubPathIsInsideBasePath(target.InstallLocation, Path.GetDirectoryName(path)))
                            {
                                var node = new RegistryKeyJunkNode(keyName, subKeyName, target.DisplayName);
                                node.Confidence.Add(ConfidencePart.ExplicitConnection);
                                results.Add(node);
                            }
                        }
                        catch
                        {
                            // TODO better handling?
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
    }
}
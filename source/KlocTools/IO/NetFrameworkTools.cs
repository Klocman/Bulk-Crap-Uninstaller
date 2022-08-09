/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Linq;
using Klocman.Extensions;
using Microsoft.Win32;

namespace Klocman.IO
{
    public static class NetFrameworkTools
    {
        public static string[] GetInstalledFrameworkVersions()
        {
            var results = new List<string>();

            using (var ndpKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\"))
            {
                if (ndpKey != null)
                {
                    foreach (var netKeyName in ndpKey.GetSubKeyNames())
                    {
                        if (!netKeyName.StartsWith("v", StringComparison.Ordinal)) continue;

                        results.AddRange(GetNetFromKey(ndpKey, netKeyName));
                    }
                }
            }

            return results.ToArray();
        }

        private static IEnumerable<string> GetNetFromKey(RegistryKey ndpKey, string netKeyName)
        {
            using (var netKey = ndpKey.OpenSubKey(netKeyName, false))
            {
                if (netKey == null) yield break;

                var valueNames = netKey.GetValueNames();
                if (valueNames.Length == 0 || !valueNames.Contains("Install"))
                {
                    var subKeyNames = netKey.GetSubKeyNames();
                    if (subKeyNames.Contains("Full"))
                    {
                        foreach (var result in GetNetFromKey(netKey, "Full"))
                            yield return result + " Full";
                    }
                    if (subKeyNames.Contains("Client"))
                    {
                        foreach (var result in GetNetFromKey(netKey, "Client"))
                            yield return result + " Client";
                    }
                }
                else if (netKey.GetValue("Install", "")?.ToString() == "1")
                {
                    var versionStr = netKey.GetValue("Version", "")?.ToString();
                    if (string.IsNullOrEmpty(versionStr))
                        versionStr = netKey.GetKeyName();
                    yield return versionStr;
                }
            }
        }
    }
}

/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Win32;

namespace UninstallTools.Junk
{
    public class TracingScanner : IJunkCreator
    {
        private const string TracingKey = @"SOFTWARE\Microsoft\Tracing";
        private const string FullTracingKey = @"HKEY_LOCAL_MACHINE\" + TracingKey;

        public void Setup(ICollection<ApplicationUninstallerEntry> allUninstallers)
        {
        }

        public IEnumerable<JunkNode> FindJunk(ApplicationUninstallerEntry target)
        {
            var results = new List<JunkNode>();
            using (var key = Registry.LocalMachine.OpenSubKey(TracingKey))
            {
                if (key != null)
                {
                    foreach (var subKeyName in key.GetSubKeyNames())
                    {
                        var i = subKeyName.LastIndexOf('_');
                        if (i <= 0)
                            continue;

                        var str = subKeyName.Substring(0, i);

                        var conf = ConfidenceGenerators.GenerateConfidence(str, Path.Combine(FullTracingKey, subKeyName), 0, target).ToList();
                        if (conf.Any())
                        {
                            var node = new RegistryKeyJunkNode(FullTracingKey, subKeyName, target.DisplayName);
                            node.Confidence.AddRange(conf);
                            results.Add(node);
                        }
                    }
                }
            }
            return results;
        }
    }
}
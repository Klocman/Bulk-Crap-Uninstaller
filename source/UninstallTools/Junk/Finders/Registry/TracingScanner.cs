/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.Collections.Generic;
using System.IO;
using System.Linq;
using UninstallTools.Junk.Confidence;
using UninstallTools.Junk.Containers;
using UninstallTools.Properties;

namespace UninstallTools.Junk.Finders.Registry
{
    public class TracingScanner : IJunkCreator
    {
        private const string TracingKey = @"SOFTWARE\Microsoft\Tracing";
        private const string FullTracingKey = @"HKEY_LOCAL_MACHINE\" + TracingKey;

        private ICollection<ApplicationUninstallerEntry> _allEntries;

        public void Setup(ICollection<ApplicationUninstallerEntry> allUninstallers)
        {
            _allEntries = allUninstallers;
        }

        public IEnumerable<IJunkResult> FindJunk(ApplicationUninstallerEntry target)
        {
            var results = new List<RegistryKeyJunk>();
            using (var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(TracingKey))
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
                            var node = new RegistryKeyJunk(Path.Combine(FullTracingKey, subKeyName), target, this);
                            node.Confidence.AddRange(conf);
                            results.Add(node);
                        }
                    }
                }
            }

            ConfidenceGenerators.TestForSimilarNames(target, _allEntries, results.Select(x => new KeyValuePair<JunkResultBase, string>(x, x.RegKeyName)).ToList());

            return results;
        }

        public string CategoryName => Localisation.Junk_Tracing_GroupName;
    }
}
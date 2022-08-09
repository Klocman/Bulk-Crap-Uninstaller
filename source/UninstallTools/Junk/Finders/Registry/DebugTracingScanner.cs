/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using Klocman.Extensions;
using UninstallTools.Junk.Confidence;
using UninstallTools.Junk.Containers;
using UninstallTools.Properties;

namespace UninstallTools.Junk.Finders.Registry
{
    public class DebugTracingScanner : IJunkCreator
    {
        public void Setup(ICollection<ApplicationUninstallerEntry> allUninstallers)
        {
        }

        public IEnumerable<IJunkResult> FindJunk(ApplicationUninstallerEntry target)
        {
            var returnList = new List<IJunkResult>();

            if (string.IsNullOrEmpty(target.InstallLocation))
                return returnList;

            string pathRoot;

            try
            {
                pathRoot = Path.GetPathRoot(target.InstallLocation) ?? throw new ArgumentException("No path root for " + target.InstallLocation);
            }
            catch (SystemException ex)
            {
                Trace.WriteLine(ex);
                return returnList;
            }

            var unrootedLocation = pathRoot.Length >= 1
                ? target.InstallLocation.Replace(pathRoot, string.Empty)
                : target.InstallLocation;

            if (string.IsNullOrEmpty(unrootedLocation.Trim()))
                return returnList;

            try
            {
                using (var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Tracing", true))
                {
                    if (key != null && target.SortedExecutables != null)
                    {
                        var exeNames = target.SortedExecutables.Select(Path.GetFileNameWithoutExtension).ToList();

                        foreach (var keyGroup in key.GetSubKeyNames()
                            .Where(x => x.EndsWith("_RASAPI32") || x.EndsWith("_RASMANCS"))
                            .Select(name => new {name, trimmed = name.Substring(0, name.LastIndexOf('_'))})
                            .GroupBy(x => x.trimmed))
                        {
                            if (exeNames.Contains(keyGroup.Key, StringComparison.InvariantCultureIgnoreCase))
                            {
                                foreach (var keyName in keyGroup)
                                {
                                    var junk = new RegistryKeyJunk(Path.Combine(key.Name, keyName.name), target, this);
                                    junk.Confidence.Add(ConfidenceRecords.ExplicitConnection);
                                    returnList.Add(junk);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is UnauthorizedAccessException || ex is SecurityException || ex is IOException)
                    Trace.WriteLine(ex);
                else
                    throw;
            }

            return returnList;
        }

        public string CategoryName => Localisation.Junk_DebugTracing_GroupName;
    }
}
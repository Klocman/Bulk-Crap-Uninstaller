/*
    Copyright (c) 2020 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Klocman.Extensions;
using Klocman.Tools;
using UninstallTools.Junk.Confidence;
using UninstallTools.Junk.Containers;

namespace UninstallTools.Junk.Finders.Drive
{
    public class PrefetchScanner : JunkCreatorBase
    {
        private ILookup<string, string> _pfFiles;

        public override void Setup(ICollection<ApplicationUninstallerEntry> allUninstallers)
        {
            base.Setup(allUninstallers);

            try
            {
                var prefetchDir = Path.Combine(WindowsTools.GetEnvironmentPath(Klocman.Native.CSIDL.CSIDL_WINDOWS), "Prefetch");
                if (!Directory.Exists(prefetchDir)) return;

                _pfFiles = Directory.GetFiles(prefetchDir)
                    .Where(x => x.EndsWith(".pf", StringComparison.OrdinalIgnoreCase))
                    .Select(
                        fullPath =>
                        {
                            var fileName = Path.GetFileName(fullPath);
                            var i = fileName.LastIndexOf('-');
                            if (i < 0) return null;
                            var appFilename = fileName.Substring(0, i);
                            if (!appFilename.EndsWith(".exe", StringComparison.OrdinalIgnoreCase)) return null;
                            return new { fullPath, appFilename };
                        })
                    .Where(x => x != null)
                    .ToLookup(arg => arg.appFilename.ToLowerInvariant(), arg => arg.fullPath);
            }
            catch (SystemException ex)
            {
                Trace.WriteLine("Failed to gather prefetch files - " + ex);
            }
        }

        public override IEnumerable<IJunkResult> FindJunk(ApplicationUninstallerEntry target)
        {
            var results = new List<IJunkResult>();
            if (_pfFiles == null || target.SortedExecutables == null || target.SortedExecutables.Length == 0) return results;

            var targetExeNames = target.SortedExecutables
                .Attempt(Path.GetFileName)
                .Where(x => !string.IsNullOrEmpty(x))
                .Select(x => x.ToLowerInvariant())
                .ToList();

            var pfFileHits = targetExeNames
                .SelectMany(fileName => _pfFiles[fileName]
                .Select(fullPath => new { fileName, fullPath }))
                .ToList();

            var usedByOthers = AllUninstallers
                .Where(x => x != target)
                .SelectMany(x => x.SortedExecutables ?? Enumerable.Empty<string>())
                .Attempt(Path.GetFileName)
                .Where(x => !string.IsNullOrEmpty(x))
                .Select(x => x.ToLowerInvariant());
            var usedByOthersLookup = new HashSet<string>(usedByOthers);

            foreach (var pfHit in pfFileHits)
            {
                var node = new FileSystemJunk(new FileInfo(pfHit.fullPath), target, this);
                node.Confidence.Add(ConfidenceRecords.ExplicitConnection);
                if (usedByOthersLookup.Contains(pfHit.fileName))
                    node.Confidence.Add(ConfidenceRecords.UsedBySimilarNamedApp);
                results.Add(node);
            }

            return results;
        }

        public override string CategoryName => "Prefetch";
    }
}
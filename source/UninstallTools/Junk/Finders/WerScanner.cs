using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Klocman.Extensions;
using Klocman.Tools;

namespace UninstallTools.Junk
{
    public class WerScanner : JunkCreatorBase
    {
        private const string CrashLabel = "AppCrash_";
        private static readonly IEnumerable<string> Archives;

        static WerScanner()
        {
            Archives = new[]
            {
                WindowsTools.GetEnvironmentPath(Klocman.Native.CSIDL.CSIDL_COMMON_APPDATA),
                WindowsTools.GetEnvironmentPath(Klocman.Native.CSIDL.CSIDL_LOCAL_APPDATA)
            }.Select(x => Path.Combine(x, @"Microsoft\Windows\WER\ReportArchive")).Where(Directory.Exists);
        }

        public override IEnumerable<JunkNode> FindJunk(ApplicationUninstallerEntry target)
        {
            if (target.SortedExecutables == null || target.SortedExecutables.Length == 0)
                yield break;

            var appExecutables = target.SortedExecutables.Attempt(Path.GetFileName).ToList();

            foreach (var candidate in Archives.Attempt(Directory.GetDirectories).SelectMany(x => x))
            {
                var startIndex = candidate.LastIndexOf(CrashLabel, StringComparison.InvariantCultureIgnoreCase);
                if (startIndex <= 0) continue;
                startIndex = startIndex + CrashLabel.Length;

                var count = candidate.IndexOf('_', startIndex) - startIndex;
                if (count <= 1) continue;

                var filename = candidate.Substring(startIndex, count);

                if (appExecutables.Any(x => x.StartsWith(filename, StringComparison.InvariantCultureIgnoreCase)))
                {
                    var node = new DriveDirectoryJunkNode(Path.GetDirectoryName(candidate), Path.GetFileName(candidate),
                        target.DisplayName);
                    node.Confidence.Add(ConfidencePart.ExplicitConnection);
                    yield return node;
                }
            }
        }
    }
}
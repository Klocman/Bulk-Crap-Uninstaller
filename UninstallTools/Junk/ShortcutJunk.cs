using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Klocman.Native;
using Klocman.Tools;
using UninstallTools.Uninstaller;

namespace UninstallTools.Junk
{
    public class ShortcutJunk : JunkBase
    {
        private readonly IList<Shortcut> _links;

        private ShortcutJunk(ApplicationUninstallerEntry entry, IList<Shortcut> links)
            : base(entry, Enumerable.Empty<ApplicationUninstallerEntry>())
        {
            _links = links;
        }

        public static IEnumerable<JunkNode> FindAllJunk(IEnumerable<ApplicationUninstallerEntry> targets)
        {
            var results = new List<Shortcut>();
            foreach (var linkFilename in 
                Directory.GetFiles(WindowsTools.GetEnvironmentPath(CSIDL.CSIDL_PROGRAMS), "*.lnk", SearchOption.AllDirectories)
                .Concat(Directory.GetFiles(WindowsTools.GetEnvironmentPath(CSIDL.CSIDL_COMMON_PROGRAMS), "*.lnk", SearchOption.AllDirectories))
                .Concat(Directory.GetFiles(WindowsTools.GetEnvironmentPath(CSIDL.CSIDL_DESKTOPDIRECTORY), "*.lnk", SearchOption.TopDirectoryOnly))
                .Concat(Directory.GetFiles(WindowsTools.GetEnvironmentPath(CSIDL.CSIDL_COMMON_DESKTOPDIRECTORY), "*.lnk", SearchOption.TopDirectoryOnly))
                .Distinct())
            {
                try
                {
                    results.Add(new Shortcut(linkFilename));
                }
                catch
                {
                }
            }

            var output = new List<JunkNode>();
            foreach (var applicationUninstallerEntry in targets)
            {
                output.AddRange(new ShortcutJunk(applicationUninstallerEntry, results).FindJunk());
            }
            return output;
        }

        public override IEnumerable<JunkNode> FindJunk()
        {
            var results = new List<DriveJunkNode>();

            foreach (var source in _links.ToList())
            {
                if (CheckMatch(source.LinkTarget, Uninstaller.InstallLocation) ||
                    CheckMatch(source.LinkTarget, Uninstaller.UninstallerLocation))
                {
                    var driveJunkNode = new DriveJunkNode(Path.GetDirectoryName(source.LinkFilename),
                        Path.GetFileName(source.LinkFilename), Uninstaller.DisplayName);
                    driveJunkNode.Confidence.Add(ConfidencePart.ExplicitConnection);
                    results.Add(driveJunkNode);
                    _links.Remove(source);
                }
            }

            return results.Cast<JunkNode>();
        }

        private static bool CheckMatch(string linkTarget, string uninstallerTarget)
        {
            return !string.IsNullOrEmpty(uninstallerTarget)
                && linkTarget.IndexOf(uninstallerTarget, StringComparison.InvariantCultureIgnoreCase) >= 0;
        }

        private sealed class Shortcut
        {
            public Shortcut(string linkFilename)
            {
                LinkFilename = linkFilename;
                LinkTarget = WindowsTools.ResolveShortcut(linkFilename);
            }

            public string LinkFilename { get; }
            public string LinkTarget { get; }

            public override string ToString()
            {
                return LinkTarget;
            }
        }
    }
}
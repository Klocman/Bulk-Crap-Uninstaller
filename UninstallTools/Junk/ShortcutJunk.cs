/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Klocman.Native;
using Klocman.Tools;
using UninstallTools.Uninstaller;
using Klocman.Extensions;

namespace UninstallTools.Junk
{
    public class ShortcutJunk : JunkBase
    {
        private readonly IList<Shortcut> _links;

        private ShortcutJunk(ApplicationUninstallerEntry entry, IEnumerable<ApplicationUninstallerEntry> other, IList<Shortcut> links)
            : base(entry, other)
        {
            _links = links;
        }

        public static IEnumerable<JunkNode> FindAllJunk(IEnumerable<ApplicationUninstallerEntry> targets, IEnumerable<ApplicationUninstallerEntry> other)
        {
            var syspath = WindowsTools.GetEnvironmentPath(CSIDL.CSIDL_WINDOWS);

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
                    var target = WindowsTools.ResolveShortcut(linkFilename);

                    if (string.IsNullOrEmpty(target) || target.Contains(syspath, StringComparison.InvariantCultureIgnoreCase))
                        continue;

                    results.Add(new Shortcut(linkFilename, target));
                }
                catch
                {
                }
            }

            var output = new List<JunkNode>();
            var otherEntries = other.ToList();

            foreach (var applicationUninstallerEntry in targets.ToList())
                output.AddRange(new ShortcutJunk(applicationUninstallerEntry, otherEntries, results).FindJunk());

            return output;
        }

        public override IEnumerable<JunkNode> FindJunk()
        {
            var results = new List<JunkNode>();

            var installLocationIsSafe = !OtherUninstallers.Any(
                x => PathTools.PathsEqual(x.InstallLocation, Uninstaller.InstallLocation));
            var uninstallerLocationIsSafe = !OtherUninstallers.Any(
                x => PathTools.PathsEqual(x.UninstallerLocation, Uninstaller.UninstallerLocation));

            var addAction = new Action<bool, Shortcut>((isSafe, source) =>
            {
                var driveJunkNode = new DriveFileJunkNode(Path.GetDirectoryName(source.LinkFilename),
                    Path.GetFileName(source.LinkFilename), Uninstaller.DisplayName);

                driveJunkNode.Confidence.Add(ConfidencePart.ExplicitConnection);
                if (!isSafe)
                    driveJunkNode.Confidence.Add(ConfidencePart.DirectoryStillUsed);

                results.Add(driveJunkNode);
                // Remove from the shortcut list so other uninstallers won't show up with it
                //_links.Remove(source);
            });

            foreach (var source in _links.ToList())
            {
                if (CheckMatch(source.LinkTarget, Uninstaller.InstallLocation))
                    addAction(installLocationIsSafe, source);
                else if (CheckMatch(source.LinkTarget, Uninstaller.UninstallerLocation))
                    addAction(uninstallerLocationIsSafe, source);
            }

            return results;
        }

        private static bool CheckMatch(string linkTarget, string uninstallerTarget)
        {
            return !string.IsNullOrEmpty(uninstallerTarget)
                && linkTarget.Contains(uninstallerTarget, StringComparison.InvariantCultureIgnoreCase);
        }

        private sealed class Shortcut
        {
            public Shortcut(string linkFilename, string linkTarget)
            {
                LinkFilename = linkFilename;
                LinkTarget = linkTarget;
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
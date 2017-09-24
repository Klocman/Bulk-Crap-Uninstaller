/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Klocman.Native;
using Klocman.Tools;
using Klocman.Extensions;

namespace UninstallTools.Junk
{
    public class ShortcutJunk : JunkCreatorBase
    {
        private ICollection<Shortcut> _links;

        public override void Setup(ICollection<ApplicationUninstallerEntry> allUninstallers)
        {
            base.Setup(allUninstallers);

            _links = GetShortcuts();
        }

        private static IEnumerable<string> GetLnkFilesSafe(CSIDL directory, SearchOption option)
        {
            try
            {
                return Directory.GetFiles(WindowsTools.GetEnvironmentPath(directory), "*.lnk", option);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Debug.Fail(ex.ToString());
            }
            return Enumerable.Empty<string>();
        }

        private static List<Shortcut> GetShortcuts()
        {
            var syspath = WindowsTools.GetEnvironmentPath(CSIDL.CSIDL_WINDOWS);

            var results = new List<Shortcut>();
            foreach (var linkFilename in
                GetLnkFilesSafe(CSIDL.CSIDL_PROGRAMS, SearchOption.AllDirectories)
                .Concat(GetLnkFilesSafe(CSIDL.CSIDL_COMMON_PROGRAMS, SearchOption.AllDirectories))
                .Concat(GetLnkFilesSafe(CSIDL.CSIDL_DESKTOPDIRECTORY, SearchOption.TopDirectoryOnly))
                .Concat(GetLnkFilesSafe(CSIDL.CSIDL_COMMON_DESKTOPDIRECTORY, SearchOption.TopDirectoryOnly))
                .Distinct())
            {
                try
                {
                    var target = WindowsTools.ResolveShortcut(linkFilename);

                    if (string.IsNullOrEmpty(target) || target.Contains(syspath, StringComparison.InvariantCultureIgnoreCase))
                        continue;

                    results.Add(new Shortcut(linkFilename, target));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    Debug.Fail("Failed to resolve shortcut " + linkFilename);
                }
            }

            return results;
        }
        
        public override IEnumerable<JunkNode> FindJunk(ApplicationUninstallerEntry target)
        {
            var results = new List<JunkNode>();

            if (!string.IsNullOrEmpty(target.InstallLocation))
            {
                results.AddRange(CheckLocation(entry => entry.InstallLocation, target)
                    .DoForEach(x=>x.Confidence.Add(ConfidenceRecord.ExplicitConnection)));
            }

            if (!string.IsNullOrEmpty(target.UninstallerFullFilename))
            {
                results.AddRange(CheckLocation(entry => entry.UninstallerFullFilename, target)
                    .DoForEach(x => x.Confidence.Add(ConfidenceRecord.ExplicitConnection)));
            }

            if (!string.IsNullOrEmpty(target.UninstallerLocation))
            {
                var exceptUninstallerShortcut = CheckLocation(entry => entry.UninstallerLocation, target)
                    .Where(x => results.All(y => !PathTools.PathsEqual(y.FullName, x.FullName)))
                    .ToList();

                results.AddRange(exceptUninstallerShortcut);
            }

            foreach (var junkNode in results.ToList())
            {
                var name = Path.GetFileNameWithoutExtension(junkNode.FullName);
                if (name == null) continue;
                junkNode.Confidence.AddRange(ConfidenceGenerators.GenerateConfidence(name, target));

                if (junkNode.Confidence.IsEmpty)
                    results.Remove(junkNode);
            }

            return results;
        }

        private static DriveFileJunkNode CreateJunkNode(Shortcut source, ApplicationUninstallerEntry entry)
        {
            return new DriveFileJunkNode(Path.GetDirectoryName(source.LinkFilename),
                                Path.GetFileName(source.LinkFilename), entry.DisplayName);
        }

        private IEnumerable<JunkNode> CheckLocation(Func<ApplicationUninstallerEntry, string> targetSelector, ApplicationUninstallerEntry entry)
        {
            var target = targetSelector(entry);
            var targetIsSafe = !GetOtherUninstallers(entry).Any(x => PathTools.PathsEqual(targetSelector(x), target));

            foreach (var source in _links)
            {
                if (source.LinkTarget.Contains(target, StringComparison.InvariantCultureIgnoreCase))
                {
                    var result = CreateJunkNode(source, entry);
                    if(!targetIsSafe)
                        result.Confidence.Add(ConfidenceRecord.DirectoryStillUsed);
                    yield return result;
                }
            }
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
/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Klocman.Extensions;
using Klocman.Native;
using Klocman.Tools;
using UninstallTools.Junk.Confidence;
using UninstallTools.Junk.Containers;
using UninstallTools.Properties;

namespace UninstallTools.Junk.Finders.Misc
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
                Trace.WriteLine(ex);
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

                    if (string.IsNullOrEmpty(target) ||
                        PathTools.SubPathIsInsideBasePath(syspath, target, true) ||
                        PathTools.SubPathIsInsideBasePath(UninstallToolsGlobalConfig.AppLocation, target, true))
                        continue;

                    results.Add(new Shortcut(linkFilename, target));
                }
                catch (Exception ex)
                {
                    var failMessage = "Failed to resolve shortcut: " + linkFilename;
                    Trace.WriteLine(failMessage);
                    Trace.WriteLine(ex);
                    Debug.Fail(failMessage);
                }
            }

            return results;
        }

        public override IEnumerable<IJunkResult> FindJunk(ApplicationUninstallerEntry target)
        {
            var results = new List<FileSystemJunk>();

            if (!string.IsNullOrEmpty(target.InstallLocation))
            {
                results.AddRange(GetLinksPointingToLocation(entry => entry.InstallLocation, target)
                    .DoForEach(x => x.Confidence.Add(ConfidenceRecords.ExplicitConnection)));
            }

            if (target.UninstallerKind == UninstallerType.Steam)
            {
                results.AddRange(GetLinksPointingToSteamApp(target));
            }
            else
            {
                if (!string.IsNullOrEmpty(target.UninstallerFullFilename))
                {
                    results.AddRange(GetLinksPointingToLocation(entry => entry.UninstallerFullFilename, target)
                        .DoForEach(x => x.Confidence.Add(ConfidenceRecords.ExplicitConnection)));
                }

                if (!string.IsNullOrEmpty(target.UninstallerLocation))
                {
                    var exceptUninstallerShortcut = GetLinksPointingToLocation(entry => entry.UninstallerLocation, target)
                        .Where(possibleResult => results.All(result => !PathTools.PathsEqual(result.Path, possibleResult.Path)))
                        .ToList();

                    results.AddRange(exceptUninstallerShortcut);
                }
            }

            // Remove shortcuts that we aren't sure about
            foreach (var junkNode in results.ToList())
            {
                var name = Path.GetFileNameWithoutExtension(junkNode.Path.Name);
                junkNode.Confidence.AddRange(ConfidenceGenerators.GenerateConfidence(name, target));

                if (junkNode.Confidence.IsEmpty)
                    results.Remove(junkNode);
            }

            return results;
        }

        /// <summary>
        /// Avoids marking all steam application shortcuts as junk (they use same exe path as uninstall commands)
        /// </summary>
        private IEnumerable<FileSystemJunk> GetLinksPointingToSteamApp(ApplicationUninstallerEntry target)
        {
            Debug.Assert(target.UninstallerKind == UninstallerType.Steam);

            var appId = System.Text.RegularExpressions.Regex.Replace(target.RatingId ?? target.RegistryKeyName ?? string.Empty, @"[^0-9]", "");

            if (!string.IsNullOrEmpty(appId))
            {
                foreach (var source in _links)
                {
                    if (source.LinkTarget.Contains(appId, StringComparison.Ordinal)
                        && source.LinkTarget.Contains("steam", StringComparison.OrdinalIgnoreCase))
                    {
                        var result = CreateJunkNode(source, target);
                        yield return result;
                    }
                }
            }
            else
            {
                Debug.Fail("Steam app has an invalid RegistryKeyName, it should contain its ID. Actual value: " + target.RegistryKeyName);
            }
        }

        public override string CategoryName => Localisation.Junk_Shortcut_GroupName;

        private FileSystemJunk CreateJunkNode(Shortcut source, ApplicationUninstallerEntry entry)
        {
            return new FileSystemJunk(new FileInfo(source.LinkFilename), entry, this);
        }

        private IEnumerable<FileSystemJunk> GetLinksPointingToLocation(
            Func<ApplicationUninstallerEntry, string> targetSelector, ApplicationUninstallerEntry entry)
        {
            var target = targetSelector(entry);
            var targetIsSafe = !GetOtherUninstallers(entry).Any(x => PathTools.PathsEqual(targetSelector(x), target));

            foreach (var source in _links)
            {
                if (source.LinkTarget.Contains(target, StringComparison.InvariantCultureIgnoreCase))
                {
                    var result = CreateJunkNode(source, entry);
                    if (!targetIsSafe)
                        result.Confidence.Add(ConfidenceRecords.DirectoryStillUsed);
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
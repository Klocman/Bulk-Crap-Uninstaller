/*
    Copyright (c) 2026 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Linq;
using Klocman.Tools;

namespace UninstallTools
{
    internal enum ShortcutUninstallMatchStatus
    {
        NotFound,
        Unique,
        Ambiguous
    }

    internal sealed class ShortcutUninstallMatch
    {
        private ShortcutUninstallMatch(ShortcutUninstallMatchStatus status, ApplicationUninstallerEntry entry)
        {
            Status = status;
            Entry = entry;
        }

        public ShortcutUninstallMatchStatus Status { get; }
        public ApplicationUninstallerEntry Entry { get; }

        public static ShortcutUninstallMatch NotFound()
        {
            return new ShortcutUninstallMatch(ShortcutUninstallMatchStatus.NotFound, null);
        }

        public static ShortcutUninstallMatch Ambiguous()
        {
            return new ShortcutUninstallMatch(ShortcutUninstallMatchStatus.Ambiguous, null);
        }

        public static ShortcutUninstallMatch Unique(ApplicationUninstallerEntry entry)
        {
            return new ShortcutUninstallMatch(ShortcutUninstallMatchStatus.Unique, entry);
        }
    }

    internal static class ShortcutUninstallMatcher
    {
        public static ShortcutUninstallMatch MatchExecutablePath(IEnumerable<ApplicationUninstallerEntry> entries,
            string executablePath)
        {
            if (entries == null || string.IsNullOrWhiteSpace(executablePath))
                return ShortcutUninstallMatch.NotFound();

            var candidates = entries as IList<ApplicationUninstallerEntry> ?? new List<ApplicationUninstallerEntry>(entries);
            var exactMatch = FindUnique(candidates,
                entry => PathTools.PathsEqual(entry?.UninstallerFullFilename, executablePath));
            if (exactMatch.Status != ShortcutUninstallMatchStatus.NotFound)
                return exactMatch;

            var executableMatch = FindUnique(candidates,
                entry => entry?.GetSortedExecutables()
                    .Any(path => PathTools.PathsEqual(path, executablePath)) == true);
            if (executableMatch.Status != ShortcutUninstallMatchStatus.NotFound)
                return executableMatch;

            return FindUnique(candidates,
                entry => PathTools.SubPathIsInsideBasePath(entry?.InstallLocation, executablePath, true, false));
        }

        private static ShortcutUninstallMatch FindUnique(IEnumerable<ApplicationUninstallerEntry> entries,
            Func<ApplicationUninstallerEntry, bool> predicate)
        {
            ApplicationUninstallerEntry match = null;
            foreach (var entry in entries)
            {
                if (!predicate(entry))
                    continue;

                if (match != null)
                    return ShortcutUninstallMatch.Ambiguous();

                match = entry;
            }

            return match == null ? ShortcutUninstallMatch.NotFound() : ShortcutUninstallMatch.Unique(match);
        }
    }
}

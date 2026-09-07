/*
    Copyright (c) 2026 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
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

    internal static class ShortcutTargetResolver
    {
        private const uint StgmRead = 0;
        private const int MaxPathLength = 32768;

        public static bool TryGetExecutableTarget(string shortcutPath, out string executablePath)
        {
            executablePath = null;
            if (string.IsNullOrWhiteSpace(shortcutPath) ||
                !shortcutPath.EndsWith(".lnk", StringComparison.OrdinalIgnoreCase) ||
                !File.Exists(shortcutPath))
                return false;

            IShellLinkW shellLink = null;
            try
            {
                shellLink = (IShellLinkW)new ShellLink();
                var persistFile = (IPersistFile)shellLink;
                persistFile.Load(shortcutPath, StgmRead);

                var targetPath = new StringBuilder(MaxPathLength);
                Marshal.ThrowExceptionForHR(shellLink.GetPath(targetPath, targetPath.Capacity, IntPtr.Zero, 0));

                var target = targetPath.ToString();
                if (string.IsNullOrWhiteSpace(target) ||
                    !target.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) ||
                    !File.Exists(target))
                    return false;

                executablePath = target;
                return true;
            }
            catch (COMException)
            {
                return false;
            }
            catch (IOException)
            {
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (NotSupportedException)
            {
                return false;
            }
            finally
            {
                if (shellLink != null)
                    Marshal.FinalReleaseComObject(shellLink);
            }
        }

        [ComImport]
        [Guid("000214F9-0000-0000-C000-000000000046")]
        private class ShellLink
        {
        }

        [ComImport]
        [Guid("000214F9-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IShellLinkW
        {
            [PreserveSig]
            int GetPath([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder file,
                int maxPathLength, IntPtr findData, uint flags);
        }

        [ComImport]
        [Guid("0000010B-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IPersistFile
        {
            void GetClassID(out Guid classId);

            [PreserveSig]
            int IsDirty();

            void Load([MarshalAs(UnmanagedType.LPWStr)] string fileName, uint mode);
        }
    }
}

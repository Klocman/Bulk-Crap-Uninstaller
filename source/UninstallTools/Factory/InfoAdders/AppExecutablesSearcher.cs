/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Klocman.Extensions;
using Klocman.Tools;

namespace UninstallTools.Factory.InfoAdders
{
    public class AppExecutablesSearcher : IMissingInfoAdder
    {
        internal static readonly string[] BinaryDirectoryNames =
        {
            "bin", "bin32", "bin64", "binaries", "program", "client", "app", "application", "win32", "win64" //"system"
        };

        public string[] CanProduceValueNames { get; } = {
            nameof(ApplicationUninstallerEntry.SortedExecutables)
        };

        public InfoAdderPriority Priority { get; } = InfoAdderPriority.RunFirst;

        public string[] RequiredValueNames { get; } = {
            nameof(ApplicationUninstallerEntry.InstallLocation)
        };

        public bool RequiresAllValues { get; } = true;
        public bool AlwaysRun { get; } = false;

        public void AddMissingInformation(ApplicationUninstallerEntry target)
        {
            /*var trimmedDispName = target.DisplayNameTrimmed;
            if (string.IsNullOrEmpty(trimmedDispName) || trimmedDispName.Length < 5)
            {
                trimmedDispName = target.DisplayName;
                if (string.IsNullOrEmpty(trimmedDispName))
                    // Impossible to search for the executable without knowing the app name
                    return;
            }*/

            if (!Directory.Exists(target.InstallLocation))
                return;

            var trimmedDispName = target.DisplayNameTrimmed;

            try
            {
                var results = ScanDirectory(new DirectoryInfo(target.InstallLocation));

                target.SortedExecutables = SortListExecutables(results.ExecutableFiles, trimmedDispName)
                    .Select(x => x.FullName).ToArray();
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
        }

        internal static ScanDirectoryResult ScanDirectory(DirectoryInfo directory)
        {
            var results = new List<FileInfo>(directory.GetFiles("*.exe", SearchOption.TopDirectoryOnly));
            var otherSubdirs = new List<DirectoryInfo>();
            var binSubdirs = new List<DirectoryInfo>();
            foreach (var subdir in directory.GetDirectories())
            {
                try
                {
                    var subName = subdir.Name;
                    if (subName.StartsWithAny(BinaryDirectoryNames, StringComparison.OrdinalIgnoreCase))
                    {
                        binSubdirs.Add(subdir);
                        results.AddRange(subdir.GetFiles("*.exe", SearchOption.TopDirectoryOnly));
                    }
                    else
                    {
                        // Directories with very short names likely contain program files
                        if (subName.Length > 3 &&
                            // This skips ISO language codes, much faster than a more specific compare
                            (subName.Length != 5 || !subName[2].Equals('-')))
                            otherSubdirs.Add(subdir);
                    }
                }
                catch (IOException)
                {
                }
                catch (UnauthorizedAccessException)
                {
                }
            }

            return new ScanDirectoryResult(results, binSubdirs, otherSubdirs);
        }

        internal static IEnumerable<FileInfo> SortListExecutables(IEnumerable<FileInfo> targets, string targetString)
        {
            return targets.OrderBy(x => Sift4.SimplestDistance(x.Name, targetString, 3));
        }

        internal sealed class ScanDirectoryResult
        {
            public ScanDirectoryResult(ICollection<FileInfo> executableFiles,
                ICollection<DirectoryInfo> binSubdirs, ICollection<DirectoryInfo> otherSubdirs)
            {
                OtherSubdirs = otherSubdirs;
                ExecutableFiles = executableFiles;
                BinSubdirs = binSubdirs;
            }

            public ICollection<DirectoryInfo> BinSubdirs { get; }
            public ICollection<FileInfo> ExecutableFiles { get; }
            public ICollection<DirectoryInfo> OtherSubdirs { get; }
        }
    }
}
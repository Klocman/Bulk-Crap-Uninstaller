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
using UninstallTools.Factory.InfoAdders;

namespace UninstallTools.Factory
{
    using KVP = KeyValuePair<DirectoryInfo, bool?>;

    public class DirectoryFactory : IUninstallerFactory
    {
        private static readonly string[] BinaryDirectoryNames =
        {
            "bin", "program", "client", "app", "application" //"system"
        };
    
        private readonly IEnumerable<ApplicationUninstallerEntry> _existingUninstallerEntries;

        public DirectoryFactory(IEnumerable<ApplicationUninstallerEntry> existing)
        {
            _existingUninstallerEntries = existing;
        }

        public IEnumerable<ApplicationUninstallerEntry> GetUninstallerEntries()
        {
            var existingUninstallers = _existingUninstallerEntries.ToList();

            var pfDirs = UninstallToolsGlobalConfig.GetProgramFilesDirectories(true).ToList();
            var dirsToSkip = GetDirectoriesToSkip(existingUninstallers, pfDirs).ToList();

            var results = new List<ApplicationUninstallerEntry>();

            var itemsToScan = GetDirectoriesToScan(existingUninstallers, pfDirs, dirsToSkip);
            foreach (var directory in itemsToScan)
            {
                if (UninstallToolsGlobalConfig.IsSystemDirectory(directory.Key) ||
                    directory.Key.Name.StartsWith("Windows", StringComparison.InvariantCultureIgnoreCase))
                    continue;

                var detectedEntries = TryCreateFromDirectory(directory.Key, directory.Value, dirsToSkip);

                results.AddRange(detectedEntries);
            }

            return results;
        }

        /// <summary>
        /// Get directories to scan for applications
        /// </summary>
        private static IEnumerable<KVP> GetDirectoriesToScan(List<ApplicationUninstallerEntry> existingUninstallers, IEnumerable<KVP> pfDirs, IEnumerable<string> dirsToSkip)
        {
            var pfDirectories = pfDirs.ToList();

            var extraPfDirectories = FindExtraPfDirectories(existingUninstallers)
                .Where(extraDir => !extraDir.Key.FullName.Contains(@"\Common Files", StringComparison.InvariantCultureIgnoreCase))
                .Where(extraDir => !pfDirectories.Any(pfDir => pfDir.Key.FullName.Contains(extraDir.Key.FullName,
                StringComparison.InvariantCultureIgnoreCase)));

            pfDirectories.AddRange(extraPfDirectories);

            var directoriesToSkip = dirsToSkip.ToList();

            // Get sub directories which could contain user programs
            var directoriesToCheck = pfDirectories.SelectMany(x =>
            {
                try
                {
                    return x.Key.GetDirectories().Select(y => new KVP(y, x.Value));
                }
                catch (IOException)
                {
                }
                catch (UnauthorizedAccessException)
                {
                }
                return Enumerable.Empty<KVP>();
            });

            // Get directories that can be relatively safely checked
            return directoriesToCheck.Where(check => !directoriesToSkip.Any(skip =>
                check.Key.FullName.Contains(skip, StringComparison.InvariantCultureIgnoreCase)));
        }

        /// <summary>
        /// Get directories which are already used and should be skipped
        /// </summary>
        private static IEnumerable<string> GetDirectoriesToSkip(IEnumerable<ApplicationUninstallerEntry> existingUninstallers,
            IEnumerable<KVP> pfDirectories)
        {
            var dirs = new List<string>();
            foreach (var x in existingUninstallers)
            {
                dirs.Add(x.InstallLocation);
                dirs.Add(x.UninstallerLocation);

                if (string.IsNullOrEmpty(x.DisplayIcon)) continue;
                try
                {
                    var iconFilename = x.DisplayIcon.Contains('.')
                        ? ProcessTools.SeparateArgsFromCommand(x.DisplayIcon).FileName
                        : x.DisplayIcon;

                    dirs.Add(PathTools.GetDirectory(iconFilename));
                }
                catch
                {
                    // Ignore invalid DisplayIcon paths
                }
            }

            return dirs.Where(x => !string.IsNullOrEmpty(x)).Distinct()
                .Where(x => !pfDirectories.Any(pfd => pfd.Key.FullName.Contains(x, StringComparison.InvariantCultureIgnoreCase)));
        }

        private static IEnumerable<KVP> FindExtraPfDirectories(IEnumerable<ApplicationUninstallerEntry> existingUninstallers)
        {
            var extraSearchLocations = existingUninstallers
                .Select(x => x.InstallLocation)
                .Where(x => !string.IsNullOrEmpty(x))
                .Select(s =>
                {
                    try
                    {
                        return Path.GetDirectoryName(s);
                    }
                    catch (ArgumentException)
                    {
                        return null;
                    }
                }).Where(x => x != null)
                .GroupBy(x => x.ToLowerInvariant())
                // Select only groups with 2 or more hits
                .Where(g => g.Take(2).Count() == 2)
                .Select(g => g.Key);

            return extraSearchLocations.Select(x =>
            {
                try
                {
                    return new DirectoryInfo(x);
                }
                catch
                {
                    return null;
                }
            }).Where(x => x != null)
            .Select(x => new KVP(x, null));
        }

        private static void CreateFromDirectoryHelper(ICollection<ApplicationUninstallerEntry> results, DirectoryInfo directory, int level,
            ICollection<string> dirsToSkip)
        {
            if (level >= 2 || dirsToSkip.Any(x => directory.FullName.Contains(x, StringComparison.InvariantCultureIgnoreCase)))
                return;

            // Get contents of this directory
            List<string> files = null;
            DirectoryInfo[] dirs = null;
            var binDirs = new List<DirectoryInfo>();

            try
            {
                files = new List<string>(Directory.GetFiles(directory.FullName, "*.exe", SearchOption.TopDirectoryOnly));

                var rawDirs = directory.GetDirectories();
                dirs = rawDirs
                    // Directories with very short names likely contain program files
                    .Where(x => x.Name.Length > 3)
                    // This matches ISO language codes, much faster than a more specific compare
                    .Where(x => x.Name.Length != 5 || !x.Name[2].Equals('-'))
                    .ToArray();

                // Check for the bin directory and add files from it to the scan
                binDirs.AddRange(rawDirs.Where(x => x.Name.StartsWithAny(BinaryDirectoryNames,
                    StringComparison.OrdinalIgnoreCase)));
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }

            // Check if it is impossible or potentially dangerous to process this directory.
            if (files == null || dirs == null || files.Count > 40)
                return;

            if (files.Count == 0 && !binDirs.Any())
            {
                foreach (var dir in dirs)
                {
                    CreateFromDirectoryHelper(results, dir, level + 1, dirsToSkip);
                }
            }
            else
            {
                var entry = new ApplicationUninstallerEntry();

                // Parse directories into useful information
                if (level > 0 && directory.Name.StartsWithAny(BinaryDirectoryNames, StringComparison.OrdinalIgnoreCase))
                {
                    entry.InstallLocation = directory.Parent?.FullName;
                    entry.RawDisplayName = directory.Parent?.Name;
                }
                else
                {
                    entry.InstallLocation = directory.FullName;
                    entry.RawDisplayName = directory.Name;

                    if (level > 0)
                        entry.Publisher = directory.Parent?.Name;
                }

                // Add files from bin directories
                foreach (var binDir in binDirs)
                {
                    try
                    {
                        files.AddRange(Directory.GetFiles(binDir.FullName, "*.exe", SearchOption.TopDirectoryOnly));
                    }
                    catch (IOException)
                    {
                    }
                    catch (UnauthorizedAccessException)
                    {
                    }
                }

                if (files.Count == 0)
                    return;

                // Use string similarity algorithm to find out which executable is likely the main application
                // TODO merge with ApplicationUninstallerEntry.GetMainExecutableCandidates
                var compareResults = files.OrderBy(
                    x => StringTools.CompareSimilarity(Path.GetFileNameWithoutExtension(x), entry.DisplayNameTrimmed));

                // Extract info from file metadata
                var compareBestMatchFile = new FileInfo(compareResults.First());
                entry.InstallDate = compareBestMatchFile.CreationTime;
                entry.DisplayIcon = compareBestMatchFile.FullName;
                //entry.IconBitmap = Icon.ExtractAssociatedIcon(compareBestMatchFile.FullName);

                try
                {
                    entry.Is64Bit = FilesystemTools.CheckExecutableMachineType(compareBestMatchFile.FullName);
                }
                catch
                {
                    entry.Is64Bit = MachineType.Unknown;
                }

                try
                {
                    FileAttribInfoAdder.FillInformationFromFileAttribs(entry, compareBestMatchFile.FullName, false);
                }
                catch
                {
                    // Not critical
                }

                // Attempt to find an uninstaller application
                var uninstallerFilters = new[] { "unins0", "uninstall", "uninst", "uninstaller" };
                var uninstaller = files.Concat(FindExtraExecutables(directory.FullName))
                    .FirstOrDefault(file =>
                    {
                        var name = Path.GetFileNameWithoutExtension(file);
                        if (string.IsNullOrEmpty(name)) return false;
                        return uninstallerFilters.Any(filter =>
                            name.StartsWith(filter, StringComparison.InvariantCultureIgnoreCase) ||
                            name.EndsWith(filter, StringComparison.InvariantCultureIgnoreCase));
                    });

                if (uninstaller != null)
                    entry.UninstallString = uninstaller;

                results.Add(entry);
            }
        }

        private static IEnumerable<string> FindExtraExecutables(string directoryPath)
        {
            try
            {
                return Directory.GetFiles(directoryPath, "*.bat", SearchOption.TopDirectoryOnly);
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
            return Enumerable.Empty<string>();
        }

        /// <summary>
        /// Try to get the main executable from the filtered folders. If no executables are present check subfolders.
        /// </summary>
        public static IEnumerable<ApplicationUninstallerEntry> TryCreateFromDirectory(DirectoryInfo directory, bool? is64Bit,
            ICollection<string> dirsToSkip)
        {
            if (directory == null)
                throw new ArgumentNullException(nameof(directory));

            var results = new List<ApplicationUninstallerEntry>();

            CreateFromDirectoryHelper(results, directory, 0, dirsToSkip);

            foreach (var tempEntry in results)
            {
                if (is64Bit.HasValue && tempEntry.Is64Bit == MachineType.Unknown)
                    tempEntry.Is64Bit = is64Bit.Value ? MachineType.X64 : MachineType.X86;

                tempEntry.IsRegistered = false;
                tempEntry.IsOrphaned = true;

                tempEntry.UninstallerKind = tempEntry.UninstallPossible
                    ? UninstallerTypeAdder.GetUninstallerType(tempEntry.UninstallString)
                    : UninstallerType.SimpleDelete;
            }

            return results;
        }
    }
}
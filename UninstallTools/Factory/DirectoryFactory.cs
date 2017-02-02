/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Klocman.Extensions;
using Klocman.Tools;
using UninstallTools.Factory.InfoAdders;

namespace UninstallTools.Factory
{
    public class DirectoryFactory : IUninstallerFactory
    {
        private static readonly string[] BinaryDirectoryNames =
        {
            "bin", "program", "client", "app", "application" //"system"
        };

        static IEnumerable<ApplicationUninstallerEntry> TryCreateFromDirectory(DirectoryInfo directory,
            bool? is64Bit)
        {
            if (directory == null)
                throw new ArgumentNullException(nameof(directory));

            var results = new List<ApplicationUninstallerEntry>();

            CreateFromDirectoryHelper(results, directory, 0);

            foreach (var tempEntry in results)
            {
                if (is64Bit.HasValue && tempEntry.Is64Bit == MachineType.Unknown)
                    tempEntry.Is64Bit = is64Bit.Value ? MachineType.X64 : MachineType.X86;

                tempEntry.IsRegistered = false;
                tempEntry.IsOrphaned = true;

                tempEntry.UninstallerKind = tempEntry.UninstallPossible ? UninstallerTypeAdder.GetUninstallerType(tempEntry.UninstallString) : UninstallerType.SimpleDelete;
                UninstallStringAdder.GenerateUninstallStrings(tempEntry);

                tempEntry.IsValid = true;
            }

            return results;
        }

        private static void CreateFromDirectoryHelper(ICollection<ApplicationUninstallerEntry> results, DirectoryInfo directory,
            int level)
        {
            if (level >= 2)
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
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }

            // Check if it is impossible or potentially dangerous to process this directory.
            if (files == null || dirs == null || files.Count > 40)
                return;

            if (files.Count == 0 && !binDirs.Any())
            {
                foreach (var dir in dirs)
                {
                    DirectoryFactory.CreateFromDirectoryHelper(results, dir, level + 1);
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
                files.AddRange(binDirs.Aggregate(Enumerable.Empty<string>(),
                    (x, y) => x.Concat(Directory.GetFiles(y.FullName, "*.exe", SearchOption.TopDirectoryOnly))));
                if (files.Count == 0)
                    return;

                // Use string similarity algorithm to find out which executable is likely the main application
                var compareResults =
                    files.OrderBy(
                        x =>
                            StringTools.CompareSimilarity(Path.GetFileNameWithoutExtension(x), entry.DisplayNameTrimmed));

                // Extract info from file metadata
                var compareBestMatchFile = new FileInfo(compareResults.First());
                entry.InstallDate = compareBestMatchFile.CreationTime;
                entry.DisplayIcon = compareBestMatchFile.FullName;
                entry.IconBitmap = Icon.ExtractAssociatedIcon(compareBestMatchFile.FullName);

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
                var uninstaller = files.Concat(Directory.GetFiles(directory.FullName, "*.bat",
                    SearchOption.TopDirectoryOnly))
                    .FirstOrDefault(file =>
                    {
                        var name = Path.GetFileNameWithoutExtension(file);
                        if (string.IsNullOrEmpty(name)) return false;
                        return uninstallerFilters.Any(filter =>
                            name.StartsWith(filter, StringComparison.InvariantCultureIgnoreCase) ||
                            name.EndsWith(filter, StringComparison.InvariantCultureIgnoreCase));
                    });

                if (uninstaller != null)
                {
                    entry.UninstallString = uninstaller;
                    entry.UninstallerFullFilename = ApplicationUninstallerFactory.GetUninstallerFilename(entry.UninstallString,
                        UninstallerType.Unknown, Guid.Empty);
                    entry.IsValid = true;
                }
                else
                {
                    entry.IsValid = false;
                }

                results.Add(entry);
            }
        }


        public DirectoryFactory(IEnumerable<ApplicationUninstallerEntry> existing)
        {
            _existingUninstallerEntries = existing;
        }

        private readonly IEnumerable<ApplicationUninstallerEntry> _existingUninstallerEntries;
        
        public IEnumerable<ApplicationUninstallerEntry> GetUninstallerEntries()
        {
            // todo extract install paths and detect overall install locations to scan
            var existingUninstallers = _existingUninstallerEntries.ToList();

            var pfDirectories = UninstallToolsGlobalConfig.GetProgramFilesDirectories(true).ToList();

            // Get directories which are already used and should be skipped
            var directoriesToSkip = existingUninstallers.SelectMany(x =>
            {
                if (!string.IsNullOrEmpty(x.DisplayIcon))
                {
                    try
                    {
                        var iconFilename = x.DisplayIcon.Contains('.')
                            ? ProcessTools.SeparateArgsFromCommand(x.DisplayIcon).FileName
                            : x.DisplayIcon;

                        return new[] { x.InstallLocation, x.UninstallerLocation, PathTools.GetDirectory(iconFilename) };
                    }
                    catch
                    {
                        // Ignore invalid DisplayIcon paths
                    }
                }
                return new[] { x.InstallLocation, x.UninstallerLocation };
            }).Where(x => x.IsNotEmpty()).Select(PathTools.PathToNormalCase)
            .Where(x => !pfDirectories.Any(pfd => pfd.Key.FullName.Contains(x, StringComparison.InvariantCultureIgnoreCase)))
            .Distinct().ToList();

            // Get sub directories which could contain user programs
            var directoriesToCheck = pfDirectories.Aggregate(Enumerable.Empty<KeyValuePair<DirectoryInfo, bool?>>(),
                (a, b) => a.Concat(b.Key.GetDirectories().Select(x => new KeyValuePair<DirectoryInfo, bool?>(x, b.Value))));

            // Get directories that can be relatively safely checked
            var inputs = directoriesToCheck.Where(x => !directoriesToSkip.Any(y =>
                x.Key.FullName.Contains(y, StringComparison.InvariantCultureIgnoreCase)
                || y.Contains(x.Key.FullName, StringComparison.InvariantCultureIgnoreCase))).ToList();

            var results = new List<ApplicationUninstallerEntry>();
            //var itemId = 0;
            foreach (var directory in inputs)
            {
                //itemId++;
                //todo callback
                //var progress = new UninstallManager.GetUninstallerListProgress(inputs.Count) { CurrentCount = itemId };
                //callback(progress);

                if (UninstallToolsGlobalConfig.IsSystemDirectory(directory.Key) ||
                    directory.Key.Name.StartsWith("Windows", StringComparison.InvariantCultureIgnoreCase))
                    continue;

                //Try to get the main executable from the filtered folders. If no executables are present check subfolders.
                var detectedEntries = DirectoryFactory.TryCreateFromDirectory(directory.Key,
                    directory.Value);

                results.AddRange(detectedEntries.Where(detected => !existingUninstallers.Any(existing =>
                {
                    if (!string.IsNullOrEmpty(existing.DisplayName) && !string.IsNullOrEmpty(detected.DisplayNameTrimmed)
                    && existing.DisplayName.Contains(detected.DisplayNameTrimmed))
                    {
                        return !existing.IsInstallLocationValid() ||
                               detected.InstallLocation.Contains(existing.InstallLocation,
                                   StringComparison.CurrentCultureIgnoreCase);
                    }
                    return false;
                })));

                //if (result != null && !existingUninstallers.Any(x => x.DisplayName.Contains(result.DisplayNameTrimmed)))
                //    results.Add(result);
            }

            return results;
        }
    }
}
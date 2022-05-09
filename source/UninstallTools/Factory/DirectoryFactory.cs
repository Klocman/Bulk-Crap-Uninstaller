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
using Klocman.Tools;
using UninstallTools.Factory.InfoAdders;
using UninstallTools.Properties;

namespace UninstallTools.Factory
{
    public class DirectoryFactory : IUninstallerFactory
    {
        private readonly IEnumerable<ApplicationUninstallerEntry> _existingUninstallerEntries;

        public DirectoryFactory(IEnumerable<ApplicationUninstallerEntry> existing)
        {
            _existingUninstallerEntries = existing;
        }

        public IList<ApplicationUninstallerEntry> GetUninstallerEntries(ListGenerationProgress.ListGenerationCallback progressCallback)
        {
            progressCallback(new ListGenerationProgress(0, -1, Localisation.Progress_DriveScan_Gathering));

            var existingUninstallers = _existingUninstallerEntries.ToList();

            var pfDirs = UninstallToolsGlobalConfig.GetProgramFilesDirectories(true);
            var dirsToSkip = GetDirectoriesToSkip(existingUninstallers, pfDirs).ToList();

            var itemsToScan = GetDirectoriesToScan(existingUninstallers, pfDirs, dirsToSkip).ToList();
            return FactoryThreadedHelpers.DriveApplicationScan(progressCallback, dirsToSkip, itemsToScan);
        }
        
        public static IEnumerable<ApplicationUninstallerEntry> TryGetApplicationsFromDirectories(
            ICollection<DirectoryInfo> directoriesToScan, IEnumerable<ApplicationUninstallerEntry> existingUninstallers)
        {
            var pfDirs = UninstallToolsGlobalConfig.GetProgramFilesDirectories(true);
            var dirsToSkip = GetDirectoriesToSkip(existingUninstallers, pfDirs).ToList();

            var results = new List<ApplicationUninstallerEntry>();
            foreach (var directory in directoriesToScan)
            {
                if (UninstallToolsGlobalConfig.IsSystemDirectory(directory) ||
                    directory.Name.StartsWith("Windows", StringComparison.InvariantCultureIgnoreCase))
                    continue;

                var detectedEntries = TryCreateFromDirectory(directory, dirsToSkip);

                results.AddRange(detectedEntries);
            }
            return results;
        }

        /// <summary>
        /// Get directories to scan for applications
        /// </summary>
        private static IEnumerable<DirectoryInfo> GetDirectoriesToScan(IEnumerable<ApplicationUninstallerEntry> existingUninstallers,
            IEnumerable<DirectoryInfo> pfDirs, IEnumerable<string> dirsToSkip)
        {
            var pfDirectories = pfDirs.ToList();

            if (UninstallToolsGlobalConfig.AutoDetectCustomProgramFiles)
            {
                var extraPfDirectories = FindExtraPfDirectories(existingUninstallers)
                  .Where(extraDir => !extraDir.FullName.Contains(@"\Common Files", StringComparison.InvariantCultureIgnoreCase))
                  .Where(extraDir => pfDirectories.All(pfDir => !PathTools.PathsEqual(pfDir.FullName, extraDir.FullName)));

                pfDirectories.AddRange(extraPfDirectories);

                string[] goodNames =
                {
                    "Apps", "App", "Applications", "Programs", "Games", "Game",
                    "Portable", "PortableApplications", "PortablePrograms", "PortableGames",
                    "PortableApps", "LiberKey"
                };

                IEnumerable<DirectoryInfo> FindDirsOnDrive(DriveInfo d)
                {
                    return d.RootDirectory.GetDirectories().Where(x => goodNames.Contains(x.Name, StringComparison.InvariantCultureIgnoreCase));
                }

                var drives = DriveInfo.GetDrives();
                foreach (var driveInfo in drives)
                {
                    if (!driveInfo.IsReady) continue;
                    switch (driveInfo.DriveType)
                    {
                        case DriveType.Fixed:
                            try
                            {
                                pfDirectories.AddRange(FindDirsOnDrive(driveInfo));
                            }
                            catch (SystemException ex)
                            {
                                Trace.WriteLine(ex);
                            }
                            break;

                        case DriveType.Removable:
                            if (UninstallToolsGlobalConfig.AutoDetectScanRemovable)
                                goto case DriveType.Fixed;
                            break;

                        case DriveType.Network:
                            // Slow and unreliable, might also be buggy
                            break;

                        case DriveType.Unknown:
                        case DriveType.NoRootDirectory:
                        case DriveType.CDRom:
                        case DriveType.Ram:
                            // No point in scanning
                            break;
                    }
                }
            }

            var directoriesToSkip = dirsToSkip.ToList();

            // Get sub directories which could contain user programs
            var directoriesToCheck = pfDirectories.SelectMany(x =>
            {
                try
                {
                    return x.GetDirectories();
                }
                catch (SystemException ex)
                {
                    Trace.WriteLine($"Could not access a program files directory: {x?.Name} | Reason:{ex.Message}");
                }
                return Enumerable.Empty<DirectoryInfo>();
            });

            // Get directories that can be relatively safely checked
            return directoriesToCheck.Where(check => !directoriesToSkip.Any(skip =>
                check.FullName.StartsWith(skip, StringComparison.InvariantCultureIgnoreCase)))
                .Distinct((pair, otherPair) => PathTools.PathsEqual(pair.FullName, otherPair.FullName));
        }

        /// <summary>
        /// Get directories which are already used and should be skipped
        /// </summary>
        private static IEnumerable<string> GetDirectoriesToSkip(IEnumerable<ApplicationUninstallerEntry> existingUninstallers,
            IEnumerable<DirectoryInfo> pfDirectories)
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
                .Where(x => !pfDirectories.Any(pfd => pfd.FullName.Contains(x, StringComparison.InvariantCultureIgnoreCase)));
        }

        private static IEnumerable<DirectoryInfo> FindExtraPfDirectories(IEnumerable<ApplicationUninstallerEntry> existingUninstallers)
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
                    var directoryInfo = new DirectoryInfo(PathTools.PathToNormalCase(x).TrimEnd('\\'));
                    return directoryInfo.Exists ? directoryInfo : null;
                }
                catch
                {
                    return null;
                }
            }).Where(x => x != null);
        }


        private static void CreateFromDirectoryHelper(ICollection<ApplicationUninstallerEntry> results,
            DirectoryInfo directory, int level, ICollection<string> dirsToSkip)
        {
            // Level 0 is for the pf folder itself. First subfolder is level 1.
            if (level > 2 || dirsToSkip.Any(x => directory.FullName.Contains(x, StringComparison.InvariantCultureIgnoreCase)))
                return;

            // Get contents of this installDir
            AppExecutablesSearcher.ScanDirectoryResult result;

            try
            {
                result = AppExecutablesSearcher.ScanDirectory(directory);
            }
            catch (IOException)
            {
                return;
            }
            catch (UnauthorizedAccessException)
            {
                return;
            }

            // Check if it is potentially dangerous to process this installDir.
            if (result.ExecutableFiles.Count > 40)
                return;

            var anyFiles = result.ExecutableFiles.Any();
            if (!anyFiles && !result.BinSubdirs.Any())
            {
                foreach (var dir in result.OtherSubdirs)
                    CreateFromDirectoryHelper(results, dir, level + 1, dirsToSkip);
            }
            else if (anyFiles)
            {
                var entry = new ApplicationUninstallerEntry();

                // Parse directories into useful information
                if (level > 0 && directory.Name.StartsWithAny(AppExecutablesSearcher.BinaryDirectoryNames, StringComparison.OrdinalIgnoreCase))
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

                var sorted = AppExecutablesSearcher.SortListExecutables(result.ExecutableFiles, entry.DisplayNameTrimmed).ToArray();
                entry.SortedExecutables = sorted.Select(x => x.FullName).ToArray();

                entry.InstallDate = directory.CreationTime;
                //entry.IconBitmap = TryExtractAssociatedIcon(compareBestMatchFile.FullName);

                // Extract info from file metadata and overwrite old values
                var compareBestMatchFile = sorted.First();
                ExecutableAttributeExtractor.FillInformationFromFileAttribs(entry, compareBestMatchFile.FullName, false);

                results.Add(entry);
            }
        }

        /// <summary>
        /// Try to get the main executable from the filtered folders. If no executables are present check subfolders.
        /// </summary>
        public static IEnumerable<ApplicationUninstallerEntry> TryCreateFromDirectory(
            DirectoryInfo directory, ICollection<string> dirsToSkip)
        {
            if (directory == null)
                throw new ArgumentNullException(nameof(directory));

            var results = new List<ApplicationUninstallerEntry>();

            CreateFromDirectoryHelper(results, directory, 0, dirsToSkip);

            foreach (var tempEntry in results)
            {
                if (tempEntry.Is64Bit == MachineType.Unknown)
                    tempEntry.Is64Bit = UninstallToolsGlobalConfig.IsPathInsideProgramFiles(directory.FullName);

                tempEntry.IsRegistered = false;
                tempEntry.IsOrphaned = true;
            }

            return results;
        }

        public static IEnumerable<ApplicationUninstallerEntry> TryCreateFromDirectory(
            DirectoryInfo directoryToScan, IEnumerable<ApplicationUninstallerEntry> existingUninstallers)
        {
            var pfDirs = UninstallToolsGlobalConfig.GetProgramFilesDirectories(true);
            var dirsToSkip = GetDirectoriesToSkip(existingUninstallers, pfDirs).ToList();

            if (UninstallToolsGlobalConfig.IsSystemDirectory(directoryToScan) ||
                directoryToScan.Name.StartsWith("Windows", StringComparison.InvariantCultureIgnoreCase))
                return Enumerable.Empty<ApplicationUninstallerEntry>();

            return TryCreateFromDirectory(directoryToScan, dirsToSkip);
        }
    }
}

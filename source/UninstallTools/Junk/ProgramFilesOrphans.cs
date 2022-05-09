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
using UninstallTools.Junk.Confidence;
using UninstallTools.Junk.Containers;
using UninstallTools.Properties;

namespace UninstallTools.Junk
{
    public class ProgramFilesOrphans : IJunkCreator
    {
        private string[] _otherInstallLocations;
        private string[] _otherNames;
        private string[] _otherPublishers;
        private List<DirectoryInfo> _programFilesDirectories;

        public IEnumerable<IJunkResult> FindJunk(ApplicationUninstallerEntry target)
        {
            // Do nothing when called by the manager
            yield break;
        }

        public IEnumerable<IJunkResult> FindAllJunk()
        {
            var output = new List<FileSystemJunk>();

            foreach (var kvp in _programFilesDirectories)
                FindJunkRecursively(output, kvp, 0);

            return output;
        }

        private void FindJunkRecursively(ICollection<FileSystemJunk> returnList, DirectoryInfo parentDirectory, int level)
        {
            try
            {
                if ((parentDirectory.Attributes & FileAttributes.System) == FileAttributes.System)
                    return;

                var subDirectories = parentDirectory.GetDirectories();

                foreach (var subDirectory in subDirectories)
                {
                    if (UninstallToolsGlobalConfig.IsSystemDirectory(subDirectory))
                        continue;

                    if (subDirectory.FullName.ContainsAny(_otherInstallLocations, StringComparison.CurrentCultureIgnoreCase))
                        continue;

                    var questionableDirName = subDirectory.Name.ContainsAny(UninstallToolsGlobalConfig.QuestionableDirectoryNames, StringComparison.CurrentCultureIgnoreCase);

                    var nameIsUsed = subDirectory.Name.ContainsAny(_otherNames, StringComparison.CurrentCultureIgnoreCase);

                    var allFiles = subDirectory.GetFiles("*", SearchOption.AllDirectories);
                    var allFilesContainExe = allFiles.Any(x => WindowsTools.IsExectuable(x.Extension, false, true));
                    var immediateFiles = subDirectory.GetFiles("*", SearchOption.TopDirectoryOnly);

                    ConfidenceRecord resultRecord;

                    if (immediateFiles.Any())
                    {
                        // No executables, MAYBE safe to remove
                        // Executables present, bad idea to remove
                        resultRecord = allFilesContainExe ? ConfidenceRecords.ExecutablesArePresent : ConfidenceRecords.FilesArePresent;
                    }
                    else if (!allFiles.Any())
                    {
                        // Empty folder, safe to remove
                        resultRecord = ConfidenceRecords.IsEmptyFolder;
                    }
                    else
                    {
                        // This folder is empty, but insides contain stuff
                        resultRecord = allFilesContainExe ? ConfidenceRecords.ExecutablesArePresent : ConfidenceRecords.FilesArePresent;

                        if (level < 1 && !questionableDirName && !nameIsUsed)
                        {
                            FindJunkRecursively(returnList, subDirectory, level + 1);
                        }
                    }

                    if (resultRecord == null) continue;

                    var newNode = new FileSystemJunk(subDirectory, null, this);
                    newNode.Confidence.Add(resultRecord);

                    if (subDirectory.Name.ContainsAny(_otherPublishers, StringComparison.CurrentCultureIgnoreCase))
                        newNode.Confidence.Add(ConfidenceRecords.PublisherIsStillUsed);

                    if (nameIsUsed)
                        newNode.Confidence.Add(ConfidenceRecords.ProgramNameIsStillUsed);

                    if (questionableDirName)
                        newNode.Confidence.Add(ConfidenceRecords.QuestionableDirectoryName);

                    if (allFiles.Length > 100)
                        newNode.Confidence.Add(ConfidenceRecords.ManyFilesArePresent);

                    // Remove 2 points for every sublevel
                    newNode.Confidence.Add(level * -2);

                    if (!subDirectory.GetDirectories().Any())
                        newNode.Confidence.Add(ConfidenceRecords.FolderHasNoSubdirectories);

                    returnList.Add(newNode);
                }
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached) throw;
                Trace.WriteLine($"Crash while scanning for {CategoryName} junk: {ex}");
            }
        }

        public void Setup(ICollection<ApplicationUninstallerEntry> allUninstallers)
        {
            _programFilesDirectories = UninstallToolsGlobalConfig.GetProgramFilesDirectories(true);

            var applicationUninstallerEntries = allUninstallers as IList<ApplicationUninstallerEntry> ?? allUninstallers.ToList();

            _otherInstallLocations =
                applicationUninstallerEntries.SelectMany(x => new[] { x.InstallLocation, x.UninstallerLocation })
                    .Where(x => x.IsNotEmpty()).Distinct().ToArray();

            _otherPublishers =
                applicationUninstallerEntries.Select(x => x.PublisherTrimmed).Where(x => x != null && x.Length > 3)
                    .Distinct().ToArray();
            _otherNames =
                applicationUninstallerEntries.Select(x => x.DisplayNameTrimmed).Where(x => x != null && x.Length > 3)
                    .Distinct().ToArray();
        }

        public string CategoryName => Localisation.Junk_ProgramFilesOrphans_GroupName;
    }
}
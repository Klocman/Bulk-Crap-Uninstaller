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
using UninstallTools.Properties;

namespace UninstallTools.Junk
{
    public class ProgramFilesOrphans
    {
        public static readonly ConfidenceRecord ConfidenceEmptyFolder = new ConfidenceRecord(4,
            Localisation.Confidence_PF_EmptyFolder);

        public static readonly ConfidenceRecord ConfidenceExecsPresent = new ConfidenceRecord(-4,
            Localisation.Confidence_PF_ExecsPresent);

        public static readonly ConfidenceRecord ConfidenceFilesPresent = new ConfidenceRecord(0,
            Localisation.Confidence_PF_FilesPresent);

        public static readonly ConfidenceRecord ConfidenceManyFilesPresent = new ConfidenceRecord(-2,
            Localisation.Confidence_PF_ManyFilesPresent);

        public static readonly ConfidenceRecord ConfidenceNameIsUsed = new ConfidenceRecord(-4,
            Localisation.Confidence_PF_NameIsUsed);

        public static readonly ConfidenceRecord ConfidenceNoSubdirs = new ConfidenceRecord(2,
            Localisation.Confidence_PF_NoSubdirs);

        public static readonly ConfidenceRecord ConfidencePublisherIsUsed = new ConfidenceRecord(-4,
            Localisation.Confidence_PF_PublisherIsUsed);

        private readonly string[] _otherInstallLocations;
        private readonly string[] _otherNames;
        private readonly string[] _otherPublishers;

        public ProgramFilesOrphans(IEnumerable<ApplicationUninstallerEntry> allEntries)
        {
            var applicationUninstallerEntries = allEntries as IList<ApplicationUninstallerEntry> ?? allEntries.ToList();

            _otherInstallLocations =
                applicationUninstallerEntries.SelectMany(x => new[] {x.InstallLocation, x.UninstallerLocation})
                    .Where(x => x.IsNotEmpty()).Distinct().ToArray();

            _otherPublishers =
                applicationUninstallerEntries.Select(x => x.PublisherTrimmed).Where(x => x != null && x.Length > 3)
                    .Distinct().ToArray();
            _otherNames =
                applicationUninstallerEntries.Select(x => x.DisplayNameTrimmed).Where(x => x != null && x.Length > 3)
                    .Distinct().ToArray();
        }

        public IEnumerable<JunkNode> FindJunk()
        {
            var output = new List<ProgramFilesJunkNode>();

            foreach (var kvp in UninstallToolsGlobalConfig.GetProgramFilesDirectories(true))
            {
                FindJunkRecursively(output, kvp.Key, 0);
            }

            return JunkManager.RemoveDuplicates(output.Cast<DriveJunkNode>()).Cast<JunkNode>();
        }
        
        private void FindJunkRecursively(ICollection<ProgramFilesJunkNode> returnList, DirectoryInfo directory, int level)
        {
            try
            {
                if ((directory.Attributes & FileAttributes.System) == FileAttributes.System)
                    return;

                var dirs = directory.GetDirectories();

                foreach (var dir in dirs)
                {
                    if (UninstallToolsGlobalConfig.IsSystemDirectory(dir))
                        continue;

                    if (dir.FullName.ContainsAny(_otherInstallLocations, StringComparison.CurrentCultureIgnoreCase))
                        continue;

                    var questionableDirName = dir.Name.ContainsAny(
                        UninstallToolsGlobalConfig.QuestionableDirectoryNames, StringComparison.CurrentCultureIgnoreCase)
                                              ||
                                              UninstallToolsGlobalConfig.QuestionableDirectoryNames.Any(
                                                  x => x.Contains(dir.Name, StringComparison.CurrentCultureIgnoreCase));

                    var nameIsUsed = dir.Name.ContainsAny(_otherNames, StringComparison.CurrentCultureIgnoreCase);

                    var allFiles = dir.GetFiles("*", SearchOption.AllDirectories);
                    var allFilesContainExe = allFiles.Any(x => WindowsTools.IsExectuable(x.Extension, false, true));
                    var immediateFiles = dir.GetFiles("*", SearchOption.TopDirectoryOnly);

                    ConfidenceRecord resultRecord;

                    if (immediateFiles.Any())
                    {
                        // No executables, MAYBE safe to remove
                        // Executables present, bad idea to remove
                        resultRecord = allFilesContainExe ? ConfidenceExecsPresent : ConfidenceFilesPresent;
                    }
                    else if (!allFiles.Any())
                    {
                        // Empty folder, safe to remove
                        resultRecord = ConfidenceEmptyFolder;
                    }
                    else
                    {
                        // This folder is empty, but insides contain stuff
                        resultRecord = allFilesContainExe ? ConfidenceExecsPresent : ConfidenceFilesPresent;

                        if (level < 1 && !questionableDirName && !nameIsUsed)
                        {
                            FindJunkRecursively(returnList, dir, level + 1);
                        }
                    }

                    if (resultRecord == null) continue;

                    var newNode = new ProgramFilesJunkNode(directory.FullName, dir.Name,
                        Localisation.Junk_ProgramFilesOrphans_GroupName);
                    newNode.Confidence.Add(resultRecord);

                    if (dir.Name.ContainsAny(_otherPublishers, StringComparison.CurrentCultureIgnoreCase))
                        newNode.Confidence.Add(ConfidencePublisherIsUsed);

                    if (nameIsUsed)
                        newNode.Confidence.Add(ConfidenceNameIsUsed);

                    if (questionableDirName)
                        newNode.Confidence.Add(ConfidenceRecord.QuestionableDirectoryName);

                    if (allFiles.Length > 100)
                        newNode.Confidence.Add(ConfidenceManyFilesPresent);

                    // Remove 2 points for every sublevel
                    newNode.Confidence.Add(level*-2);

                    if (!dir.GetDirectories().Any())
                        newNode.Confidence.Add(ConfidenceNoSubdirs);

                    returnList.Add(newNode);
                }
            }
            catch
            {
                if (Debugger.IsAttached) throw;
            }
        }

        public class ProgramFilesJunkNode : DriveDirectoryJunkNode
        {
            public ProgramFilesJunkNode(string parentPath, string name, string uninstallerName)
                : base(parentPath, name, uninstallerName)
            {
            }

            public override string GroupName => Localisation.Junk_ProgramFilesOrphans_GroupName;
        }
    }
}
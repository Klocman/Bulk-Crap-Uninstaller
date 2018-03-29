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

namespace UninstallTools.Junk.Finders.Drive
{
    public class CommonDriveJunkScanner : JunkCreatorBase
    {
        private static IEnumerable<DirectoryInfo> _foldersToCheck;
        private ApplicationUninstallerEntry _uninstaller;

        public override void Setup(ICollection<ApplicationUninstallerEntry> allUninstallers)
        {
            base.Setup(allUninstallers);

            var allDirs = UninstallToolsGlobalConfig.JunkSearchDirs.Concat(UninstallToolsGlobalConfig.GetAllProgramFiles());
            var validDirs = allDirs.Attempt(dir =>
                            {
                                var dirinfo = new DirectoryInfo(dir);
                                return dirinfo.Exists ? dirinfo : null;
                            }).Where(x => x != null);
            _foldersToCheck = validDirs.DistinctBy(x => x.FullName.ToLowerInvariant()).ToList();
        }

        public override IEnumerable<IJunkResult> FindJunk(ApplicationUninstallerEntry target)
        {
            _uninstaller = target;

            return _foldersToCheck.SelectMany((x) => FindJunkRecursively(x)).Cast<IJunkResult>();
        }

        public override string CategoryName => Localisation.Junk_Drive_GroupName;

        private IEnumerable<FileSystemJunk> FindJunkRecursively(DirectoryInfo directory, int level = 0)
        {
            var results = new List<FileSystemJunk>();

            try
            {
                var dirs = directory.GetDirectories();

                foreach (var dir in dirs)
                {
                    if (UninstallToolsGlobalConfig.IsSystemDirectory(dir))
                        continue;

                    var generatedConfidence = GenerateConfidence(dir.Name, directory.FullName, level).ToList();

                    FileSystemJunk newNode = null;
                    if (generatedConfidence.Any())
                    {
                        newNode = new FileSystemJunk(dir, _uninstaller, this);
                        newNode.Confidence.AddRange(generatedConfidence);

                        if (CheckIfDirIsStillUsed(dir.FullName, GetOtherInstallLocations(_uninstaller)))
                            newNode.Confidence.Add(ConfidenceRecords.DirectoryStillUsed);

                        results.Add(newNode);
                    }

                    if (level > 1) continue;

                    var junkNodes = FindJunkRecursively(dir, level + 1).ToList();
                    results.AddRange(junkNodes);

                    if (newNode != null)
                    {
                        // Check if the directory will have nothing left after junk removal.
                        if (!dir.GetFiles().Any())
                        {
                            var subDirs = dir.GetDirectories();
                            if (!subDirs.Any() || subDirs.All(d => junkNodes.Any(y => PathTools.PathsEqual(d.FullName, y.Path.FullName))))
                                newNode.Confidence.Add(ConfidenceRecords.AllSubdirsMatched);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached) throw;
                Console.WriteLine(ex);
            }

            return results;
        }

        protected IEnumerable<ConfidenceRecord> GenerateConfidence(string itemName, string itemParentPath, int level)
        {
            var baseOutput = ConfidenceGenerators.GenerateConfidence(itemName, itemParentPath, level, _uninstaller).ToList();

            if (!baseOutput.Any(x => x.Change > 0))
                return Enumerable.Empty<ConfidenceRecord>();

            if (UninstallToolsGlobalConfig.QuestionableDirectoryNames.Contains(itemName, StringComparison.OrdinalIgnoreCase))
                baseOutput.Add(ConfidenceRecords.QuestionableDirectoryName);

            return baseOutput;
        }
    }
}

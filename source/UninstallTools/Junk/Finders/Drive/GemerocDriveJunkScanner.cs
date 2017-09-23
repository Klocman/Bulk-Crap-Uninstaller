/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using Klocman.Extensions;
using Klocman.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace UninstallTools.Junk
{
    public class GemerocDriveJunkScanner : JunkCreatorBase
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
                                if (dirinfo.Exists)
                                    return dirinfo;
                                return null;
                            }).Where(x => x != null);
            _foldersToCheck = validDirs.DistinctBy(x => x.FullName.ToLowerInvariant()).ToList();
        }

        public override IEnumerable<JunkNode> FindJunk(ApplicationUninstallerEntry target)
        {
            _uninstaller = target;

            return _foldersToCheck.SelectMany(FindJunkRecursively).Cast<JunkNode>();
        }

        private IEnumerable<DriveJunkNode> FindJunkRecursively(DirectoryInfo directory, int level = 0)
        {
            var results = new List<DriveJunkNode>();

            try
            {
                var dirs = directory.GetDirectories();

                foreach (var dir in dirs)
                {
                    if (UninstallToolsGlobalConfig.IsSystemDirectory(dir))
                        continue;

                    var generatedConfidence = GenerateConfidence(dir.Name, directory.FullName, level).ToList();

                    DriveJunkNode newNode = null;
                    if (generatedConfidence.Any())
                    {
                        newNode = new DriveDirectoryJunkNode(directory.FullName, dir.Name, _uninstaller.DisplayName);
                        newNode.Confidence.AddRange(generatedConfidence);

                        if (CheckIfDirIsStillUsed(dir.FullName, GetOtherInstallLocations(_uninstaller)))
                            newNode.Confidence.Add(ConfidencePart.DirectoryStillUsed);

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
                            if (!subDirs.Any() || subDirs.All(d => junkNodes.Any(y => PathTools.PathsEqual(d.FullName, y.FullName))))
                                newNode.Confidence.Add(ConfidencePart.AllSubdirsMatched);
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

        protected IEnumerable<ConfidencePart> GenerateConfidence(string itemName, string itemParentPath, int level)
        {
            var baseOutput = ConfidenceGenerators.GenerateConfidence(itemName, itemParentPath, level, _uninstaller).ToList();

            if (!baseOutput.Any(x => x.Change > 0))
                return Enumerable.Empty<ConfidencePart>();

            if (UninstallToolsGlobalConfig.QuestionableDirectoryNames.Contains(itemName, StringComparison.OrdinalIgnoreCase))
                baseOutput.Add(ConfidencePart.QuestionableDirectoryName);

            return baseOutput;
        }
    }
}

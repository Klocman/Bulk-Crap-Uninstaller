/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Linq;
using Klocman.Tools;

namespace UninstallTools.Junk
{
    public static class JunkManager
    {
        public static IEnumerable<JunkNode> RemoveDuplicates(List<JunkNode> input)
        {
            // todo add stuff to compare against to nodes
            // todo hide fullpath, parent path etc from nodes, implement displaystr, group, open instead
            foreach (var VARIABLE in input.GroupBy(x=>x.GetType()))
            {
                
            }

            foreach (var group in input.GroupBy(x => x.FullName))
            {
                FileSystemJunk node = null;
                foreach (var item in group)
                {
                    if (node == null)
                    {
                        node = item;
                    }
                    else
                    {
                        node.Confidence.AddRange(item.Confidence.ConfidenceParts);
                    }
                }

                if (node != null)
                    yield return node;
            }
        }

        public static IEnumerable<JunkNode> FindJunk(IEnumerable<ApplicationUninstallerEntry> targets,
            ICollection<ApplicationUninstallerEntry> allUninstallers, ListGenerationProgress.ListGenerationCallback progressCallback)
        {
            progressCallback(new ListGenerationProgress(-1, 0, "Setting up junk scanners..."));

            var scanners = ReflectionTools.GetTypesImplementingBase<IJunkCreator>()
                .Select(Activator.CreateInstance)
                .Cast<IJunkCreator>()
                .ToList();

            foreach (var junkCreator in scanners)
            {
                junkCreator.Setup(allUninstallers);
            }

            var results = new List<JunkNode>();
            var targetEntries = targets as IList<ApplicationUninstallerEntry> ?? targets.ToList();
            var progress = 0;
            foreach (var junkCreator in scanners)
            {
                var scannerProgress = new ListGenerationProgress(progress++, scanners.Count, junkCreator.CategoryName);

                var entryProgress = 0;
                foreach (var target in targetEntries)
                {
                    scannerProgress.Inner = new ListGenerationProgress(entryProgress++, targetEntries.Count, target.DisplayName);
                    progressCallback(scannerProgress);

                    results.AddRange(junkCreator.FindJunk(target));
                }
            }

            progressCallback(new ListGenerationProgress(-1, 0, "Finishing up..."));

            return RemoveDuplicates(results);

            /*
            var targetEntries = targets as IList<ApplicationUninstallerEntry> ?? applicationUninstallerEntries.ToList();

            var otherUninstallers = allUninstallers.Except(targetEntries).ToList();

            var result = new List<JunkNode>(targetEntries.Count);
            var progress = 0;
            foreach (var uninstaller in targetEntries)
            {
                var progressInfo = new ListGenerationProgress(progress++, targetEntries.Count, uninstaller.DisplayName);

                progressInfo.Inner = new ListGenerationProgress(0, 3, "Scanning start-ups...");
                progressCallback(progressInfo);
                var sj = new StartupJunk(uninstaller);
                result.AddRange(sj.FindJunk());

                progressInfo.Inner = new ListGenerationProgress(1, 3, "Scanning drives...");
                progressCallback(progressInfo);
                var dj = new DriveJunk(uninstaller, otherUninstallers);
                result.AddRange(dj.FindJunk());

                progressInfo.Inner = new ListGenerationProgress(2, 3, "Scanning registry...");
                progressCallback(progressInfo);
                var rj = new RegistryJunk(uninstaller, otherUninstallers);
                result.AddRange(rj.FindJunk());
            }

            result.AddRange(ShortcutJunk.FindAllJunk(targetEntries, otherUninstallers));

            return result;*/
        }

        public static IEnumerable<JunkNode> FindProgramFilesJunk(
            IEnumerable<ApplicationUninstallerEntry> allUninstallers)
        {
            return new ProgramFilesOrphans(allUninstallers).FindJunk();
        }
    }
}
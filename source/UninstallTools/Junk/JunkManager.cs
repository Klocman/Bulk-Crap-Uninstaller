/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Linq;
using Klocman.Extensions;
using Klocman.Forms.Tools;
using Klocman.Tools;
using UninstallTools.Junk.Containers;
using UninstallTools.Properties;

namespace UninstallTools.Junk
{
    public static class JunkManager
    {
        private static IEnumerable<IJunkResult> CleanUpResults(IEnumerable<IJunkResult> input)
        {
            return RemoveDuplicates(input).Where(JunkDoesNotPointToSelf);
        }

        /// <summary>
        /// Make sure that the junk result doesn't point to this application.
        /// </summary>
        private static bool JunkDoesNotPointToSelf(IJunkResult x)
        {
            if (x is FileSystemJunk fileSystemJunk)
            {
                return !fileSystemJunk.Path.FullName.StartsWith(UninstallToolsGlobalConfig.AppLocation, StringComparison.OrdinalIgnoreCase);
            }

            if (x is StartupJunkNode startupJunk)
            {
                return !startupJunk.Entry.CommandFilePath.StartsWith(UninstallToolsGlobalConfig.AppLocation, StringComparison.OrdinalIgnoreCase);
            }

            return true;
        }

        /// <summary>
        /// Merge duplicate junk entries and their confidence parts
        /// </summary>
        private static IEnumerable<IJunkResult> RemoveDuplicates(IEnumerable<IJunkResult> input)
        {
            foreach (var appGroup in input.GroupBy(x => x.Application))
            {
                foreach (var group in appGroup.GroupBy(x => PathTools.NormalizePath(x.GetDisplayName()).ToLowerInvariant()))
                {
                    IJunkResult firstJunkResult = null;
                    foreach (var junkResult in group)
                    {
                        if (firstJunkResult == null)
                            firstJunkResult = junkResult;
                        else
                            firstJunkResult.Confidence.AddRange(junkResult.Confidence.ConfidenceParts);
                    }

                    if (firstJunkResult != null)
                        yield return firstJunkResult;
                }
            }
        }

        public static IEnumerable<IJunkResult> FindJunk(IEnumerable<ApplicationUninstallerEntry> targets,
            ICollection<ApplicationUninstallerEntry> allUninstallers, ListGenerationProgress.ListGenerationCallback progressCallback)
        {
            progressCallback(new ListGenerationProgress(-1, 0, Localisation.Junk_Progress_Startup));

            var scanners = ReflectionTools.GetTypesImplementingBase<IJunkCreator>()
                .Attempt(Activator.CreateInstance)
                .Cast<IJunkCreator>()
                .ToList();

            foreach (var junkCreator in scanners)
            {
                junkCreator.Setup(allUninstallers);
            }

            var results = new List<IJunkResult>();
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

                    try { results.AddRange(junkCreator.FindJunk(target)); }
                    catch (SystemException ex) { PremadeDialogs.GenericError(ex); }
                }
            }

            progressCallback(new ListGenerationProgress(-1, 0, Localisation.Junk_Progress_Finishing));

            foreach (var target in targetEntries)
                results.AddRange(target.AdditionalJunk);

            return CleanUpResults(results);
        }

        public static IEnumerable<IJunkResult> FindProgramFilesJunk(
            ICollection<ApplicationUninstallerEntry> allUninstallers)
        {
            var pfScanner = new ProgramFilesOrphans();
            pfScanner.Setup(allUninstallers);
            return CleanUpResults(pfScanner.FindAllJunk().ToList());
        }
    }
}
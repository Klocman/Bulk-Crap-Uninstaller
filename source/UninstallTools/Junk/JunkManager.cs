/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.Collections.Generic;
using System.Linq;

namespace UninstallTools.Junk
{
    public static class JunkManager
    {

        // todo merge all for single uninstaller at end
        public static IEnumerable<DriveJunkNode> RemoveDuplicates(IEnumerable<DriveJunkNode> input)
        {
            foreach (var group in input.GroupBy(x => x.FullName))
            {
                DriveJunkNode node = null;
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

        public static IEnumerable<JunkNode> FindJunk(IEnumerable<ApplicationUninstallerEntry> uninstallers,
            IEnumerable<ApplicationUninstallerEntry> allUninstallers, ListGenerationProgress.ListGenerationCallback progressCallback)
        {
            var targetEntries = uninstallers as IList<ApplicationUninstallerEntry> ?? uninstallers.ToList();

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

            return result;
        }

        public static IEnumerable<JunkNode> FindProgramFilesJunk(
            IEnumerable<ApplicationUninstallerEntry> allUninstallers)
        {
            return new ProgramFilesOrphans(allUninstallers).FindJunk();
        }
    }
}
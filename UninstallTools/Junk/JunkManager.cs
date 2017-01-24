/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.Collections.Generic;
using System.Linq;
using UninstallTools.Uninstaller;

namespace UninstallTools.Junk
{
    public static class JunkManager
    {
        public static IEnumerable<JunkNode> FindJunk(IEnumerable<ApplicationUninstallerEntry> uninstallers,
            IEnumerable<ApplicationUninstallerEntry> allUninstallers)
        {
            var targetEntries = uninstallers as IList<ApplicationUninstallerEntry> ?? uninstallers.ToList();

            var otherUninstallers = allUninstallers.Except(targetEntries).ToList();

            var result = new List<JunkNode>(targetEntries.Count);
            foreach (var uninstaller in targetEntries)
            {
                var dj = new DriveJunk(uninstaller, otherUninstallers);
                result.AddRange(dj.FindJunk());

                var rj = new RegistryJunk(uninstaller, otherUninstallers);
                result.AddRange(rj.FindJunk());

                var sj = new StartupJunk(uninstaller);
                result.AddRange(sj.FindJunk());
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
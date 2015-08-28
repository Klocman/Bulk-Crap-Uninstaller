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
            var allEntries = allUninstallers as IList<ApplicationUninstallerEntry> ?? allUninstallers.ToList();

            var result = new List<JunkNode>(targetEntries.Count());
            foreach (var uninstaller in targetEntries)
            {
                var otherUninstallers = allEntries.Except(targetEntries).ToList();

                var dj = new DriveJunk(uninstaller, otherUninstallers);
                result.AddRange(dj.FindJunk());

                var rj = new RegistryJunk(uninstaller, otherUninstallers);
                result.AddRange(rj.FindJunk());

                var sj = new StartupJunk(uninstaller);
                result.AddRange(sj.FindJunk());
            }

            return result;
        }

        public static IEnumerable<JunkNode> FindProgramFilesJunk(
            IEnumerable<ApplicationUninstallerEntry> allUninstallers)
        {
            return new ProgramFilesOrphans(allUninstallers).FindJunk();
        }
    }
}
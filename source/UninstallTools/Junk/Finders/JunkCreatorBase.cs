using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Klocman.Tools;

namespace UninstallTools.Junk
{
    public abstract class JunkCreatorBase : IJunkCreator
    {
        public virtual void Setup(ICollection<ApplicationUninstallerEntry> allUninstallers)
        {
            AllUninstallers = allUninstallers;
        }

        protected ICollection<ApplicationUninstallerEntry> AllUninstallers { get; private set; }

        protected IEnumerable<ApplicationUninstallerEntry> GetOtherUninstallers(ApplicationUninstallerEntry exceptThis)
        {
            return AllUninstallers.Where(x => x != exceptThis);
        }
        
        protected IEnumerable<string> GetOtherInstallLocations(ApplicationUninstallerEntry target)
        {
            return GetOtherUninstallers(target).Select(x => x.InstallLocation).Where(x => !string.IsNullOrEmpty(x));
        }

        public abstract IEnumerable<JunkNode> FindJunk(ApplicationUninstallerEntry target);

        public abstract string CategoryName { get; }

        /// <summary>
        /// Returns true if the dir is still used by other apps and can't be safely deleted.
        /// </summary>
        public static bool CheckIfDirIsStillUsed(string location, IEnumerable<string> otherInstallLocations)
        {
            if (string.IsNullOrEmpty(location))
                return false;

            return otherInstallLocations.Any(x => x.TrimEnd('\\').StartsWith(location, StringComparison.InvariantCultureIgnoreCase));
        }

        private static readonly string FullWindowsDirectoryName = PathTools.GetWindowsDirectory().FullName;

        // TODO overhaul
        internal static DriveJunkNode GetJunkNodeFromLocation(IEnumerable<string> otherInstallLocations, string directory, string displayName)
        {
            try
            {
                var dirInfo = new DirectoryInfo(directory);

                if (dirInfo.FullName.Contains(FullWindowsDirectoryName) || !dirInfo.Exists || dirInfo.Parent == null)
                    return null;

                var newNode = new DriveDirectoryJunkNode(Path.GetDirectoryName(directory),
                    Path.GetFileName(directory), displayName);
                newNode.Confidence.Add(ConfidenceRecord.ExplicitConnection);

                if (CheckIfDirIsStillUsed(dirInfo.FullName, otherInstallLocations))
                    newNode.Confidence.Add(ConfidenceRecord.DirectoryStillUsed);

                return newNode;
            }
            catch
            {
                return null;
            }
        }

        protected static bool SubPathIsInsideBasePath(string basePath, string subPath)
        {
            basePath = basePath?.Normalize().Trim().Trim('\\', '/');
            if (string.IsNullOrEmpty(basePath))
                return false;

            subPath = subPath?.Normalize().Trim().Trim('\\', '/');
            if (string.IsNullOrEmpty(subPath))
                return false;

            return subPath.StartsWith(basePath, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
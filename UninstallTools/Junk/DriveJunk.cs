using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Klocman.Extensions;
using Klocman.Tools;
using UninstallTools.Uninstaller;

namespace UninstallTools.Junk
{
    public class DriveJunk : JunkBase
    {
        private static IEnumerable<DirectoryInfo> _foldersToCheck;
        private IEnumerable<string> _otherInstallLocations;

        public DriveJunk(ApplicationUninstallerEntry entry, IEnumerable<ApplicationUninstallerEntry> otherUninstallers)
            : base(entry, otherUninstallers)
        {
        }

        private static IEnumerable<DirectoryInfo> FoldersToCheck
        {
            get
            {
                if (_foldersToCheck == null)
                {
                    var result = new List<DirectoryInfo>(7);

                    foreach (
                        var item in
                            UninstallToolsGlobalConfig.JunkSearchDirs.Concat(UninstallToolsGlobalConfig.AllProgramFiles)
                        )
                    {
                        try
                        {
                            var dirinfo = new DirectoryInfo(item);
                            if (dirinfo.Exists && !result.Any(x => x.FullName.Equals(dirinfo.FullName)))
                                result.Add(dirinfo);
                        }
                        catch
                        {
                            // Not enough rights to access the folder
                        }
                    }
                    _foldersToCheck = result;
                }
                return _foldersToCheck;
            }
        }

        private IEnumerable<string> OtherInstallLocations
        {
            get
            {
                if (_otherInstallLocations == null && OtherUninstallers != null)
                {
                    var result = new List<string>();
                    foreach (var item in OtherUninstallers.Where(item => item.IsInstallLocationValid()))
                    {
                        try
                        {
                            result.Add(new DirectoryInfo(item.InstallLocation).FullName);
                        }
                        catch
                        {
                            // Invalid install location or security exception
                        }
                    }
                    _otherInstallLocations = result;
                }
                return _otherInstallLocations;
            }
        }

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

        public override IEnumerable<JunkNode> FindJunk()
        {
            var output = new List<DriveJunkNode>();

            var uninLoc = Uninstaller.UninstallerLocation;
            if (uninLoc.IsNotEmpty()
                && UninstallToolsGlobalConfig.AllProgramFiles.Any(
                    x => uninLoc.StartsWith(x, StringComparison.InvariantCultureIgnoreCase))
                && !OtherInstallLocations.Any(x => uninLoc.StartsWith(x) || x.StartsWith(uninLoc)))
            {
                var resultNode = GetJunkNodeFromLocation(uninLoc);
                if (resultNode != null)
                    output.Add(resultNode);
            }

            if (Uninstaller.IsInstallLocationValid())
            {
                var resultNode = GetJunkNodeFromLocation(Uninstaller.InstallLocation);
                if (resultNode != null)
                    output.Add(resultNode);
            }

            foreach (var folder in FoldersToCheck)
            {
                FindJunkRecursively(output, folder, 0);
            }

            if (Uninstaller.UninstallerKind == UninstallerType.StoreApp)
            {
                foreach (var driveJunkNode in output)
                {
                    driveJunkNode.Confidence.Add(ConfidencePart.IsStoreApp);
                }
            }

            return RemoveDuplicates(output).Cast<JunkNode>();
        }

        public override IEnumerable<ConfidencePart> GenerateConfidence(string itemName, string itemParentPath, int level,
            bool skipNameCheck)
        {
            var baseOutput = base.GenerateConfidence(itemName, itemParentPath, level, skipNameCheck);
            var generateConfidence = baseOutput as IList<ConfidencePart> ?? baseOutput.ToList();

            if (!generateConfidence.Any())
                return generateConfidence;

            var output = new List<ConfidencePart>();
            if (
                UninstallToolsGlobalConfig.QuestionableDirectoryNames.Any(
                    x => x.Equals(itemName, StringComparison.OrdinalIgnoreCase)))
                output.Add(ConfidencePart.QuestionableDirectoryName);

            return generateConfidence.Concat(output);
        }

        // Returns true if another installer is still using this location
        private bool CheckAgainstOtherInstallers(DirectoryInfo location)
        {
            if (location == null)
                return false;

            var fullname = location.FullName;
            return OtherInstallLocations.Any(x => x.Equals(fullname, StringComparison.OrdinalIgnoreCase));
        }

        private void FindJunkRecursively(List<DriveJunkNode> returnList, DirectoryInfo directory, int level)
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

                    var generatedConfidence = GenerateConfidence(dir.Name, directory.FullName, level, false);
                    var confidenceParts = generatedConfidence as IList<ConfidencePart> ?? generatedConfidence.ToList();

                    if (confidenceParts.Any())
                    {
                        var newNode = new DriveJunkNode(directory.FullName, dir.Name, Uninstaller.DisplayName);
                        newNode.Confidence.AddRange(confidenceParts);

                        if (CheckAgainstOtherInstallers(dir))
                            newNode.Confidence.Add(ConfidencePart.DirectoryStillUsed);

                        returnList.Add(newNode);
                    }
                    else if (level < 1)
                    {
                        FindJunkRecursively(returnList, dir, level + 1);
                    }
                }
            }
            catch
            {
                if (Debugger.IsAttached) throw;
            }
        }

        private static readonly string FullWindowsDirectoryName = PathTools.GetWindowsDirectory().FullName;

        private DriveJunkNode GetJunkNodeFromLocation(string directory)
        {
            try
            {
                var dirInfo = new DirectoryInfo(directory);

                if (dirInfo.FullName.Contains(FullWindowsDirectoryName) || !dirInfo.Exists || dirInfo.Parent == null)
                    return null;

                var newNode = new DriveJunkNode(Path.GetDirectoryName(directory),
                    Path.GetFileName(directory), Uninstaller.DisplayName);
                newNode.Confidence.Add(ConfidencePart.ExplicitConnection);

                var generatedConfidence = GenerateConfidence(dirInfo.Name, dirInfo.Parent.FullName, 1, true);
                newNode.Confidence.AddRange(generatedConfidence);

                if (CheckAgainstOtherInstallers(dirInfo))
                    newNode.Confidence.Add(ConfidencePart.DirectoryStillUsed);

                return newNode;
            }
            catch
            {
                return null;
            }
        }
    }
}
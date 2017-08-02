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
    public class DriveJunk : JunkBase
    {
        private static IEnumerable<DirectoryInfo> _foldersToCheck;
        private static readonly string FullWindowsDirectoryName = PathTools.GetWindowsDirectory().FullName;
        private IEnumerable<string> _otherInstallLocations;

        private static IEnumerable<DirectoryInfo> FoldersToCheck
        {
            get
            {
                if (_foldersToCheck == null)
                {
                    var result = new List<DirectoryInfo>(7);

                    foreach (
                        var item in
                            UninstallToolsGlobalConfig.JunkSearchDirs.Concat(UninstallToolsGlobalConfig.GetAllProgramFiles())
                        )
                    {
                        try
                        {
                            var dirinfo = new DirectoryInfo(item);
                            if (dirinfo.Exists && !result.Any(x => PathTools.PathsEqual(x.FullName, dirinfo.FullName)))
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

        public DriveJunk(ApplicationUninstallerEntry entry, IEnumerable<ApplicationUninstallerEntry> otherUninstallers)
                    : base(entry, otherUninstallers)
        {
        }

        private bool CheckAgainstOtherInstallers(FileSystemInfo location)
        {
            if (location == null)
                return false;

            var fullname = location.FullName;
            return OtherInstallLocations.Any(x => x.Equals(fullname, StringComparison.InvariantCultureIgnoreCase));
        }

        public override IEnumerable<JunkNode> FindJunk()
        {
            var output = new List<DriveJunkNode>();

            var uninLoc = Uninstaller.UninstallerLocation;
            if (uninLoc.IsNotEmpty()
                && UninstallToolsGlobalConfig.GetAllProgramFiles().Any(
                    x => uninLoc.StartsWith(x, StringComparison.InvariantCultureIgnoreCase))
                && !OtherInstallLocations.Any(x => uninLoc.StartsWith(x, StringComparison.InvariantCultureIgnoreCase) || x.StartsWith(uninLoc, StringComparison.InvariantCultureIgnoreCase)))
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

            output.AddRange(GetUninstallerJunk());

            foreach (var folder in FoldersToCheck)
            {
                output.AddRange(FindJunkRecursively(folder));
            }

            if (Uninstaller.UninstallerKind == UninstallerType.StoreApp)
            {
                foreach (var driveJunkNode in output)
                {
                    driveJunkNode.Confidence.Add(ConfidencePart.IsStoreApp);
                }
            }

            output.AddRange(SearchWerReports());

            return RemoveDuplicates(output).Cast<JunkNode>();
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
                        newNode = new DriveDirectoryJunkNode(directory.FullName, dir.Name, Uninstaller.DisplayName);
                        newNode.Confidence.AddRange(generatedConfidence);

                        if (CheckAgainstOtherInstallers(dir))
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
            catch
            {
                if (Debugger.IsAttached) throw;
            }

            return results;
        }

        public override IEnumerable<ConfidencePart> GenerateConfidence(string itemName, string itemParentPath, int level)
        {
            var baseOutput = base.GenerateConfidence(itemName, itemParentPath, level).ToList();

            if (!baseOutput.Any(x => x.Change > 0))
                return Enumerable.Empty<ConfidencePart>();

            if (UninstallToolsGlobalConfig.QuestionableDirectoryNames.Contains(itemName, StringComparison.OrdinalIgnoreCase))
                baseOutput.Add(ConfidencePart.QuestionableDirectoryName);

            return baseOutput;
        }

        // TODO overhaul
        private DriveJunkNode GetJunkNodeFromLocation(string directory)
        {
            try
            {
                var dirInfo = new DirectoryInfo(directory);

                if (dirInfo.FullName.Contains(FullWindowsDirectoryName) || !dirInfo.Exists || dirInfo.Parent == null)
                    return null;

                var newNode = new DriveDirectoryJunkNode(Path.GetDirectoryName(directory),
                    Path.GetFileName(directory), Uninstaller.DisplayName);
                newNode.Confidence.Add(ConfidencePart.ExplicitConnection);

                if (CheckAgainstOtherInstallers(dirInfo))
                    newNode.Confidence.Add(ConfidencePart.DirectoryStillUsed);

                return newNode;
            }
            catch
            {
                return null;
            }
        }

        private IEnumerable<DriveJunkNode> GetUninstallerJunk()
        {
            if (!File.Exists(Uninstaller.UninstallerFullFilename))
                return Enumerable.Empty<DriveJunkNode>();

            DriveJunkNode result;

            switch (Uninstaller.UninstallerKind)
            {
                case UninstallerType.InstallShield:
                    var target = Path.GetDirectoryName(Uninstaller.UninstallerFullFilename);
                    result = new DriveDirectoryJunkNode(Path.GetDirectoryName(target),
                        Path.GetFileName(target), Uninstaller.DisplayName);
                    break;

                case UninstallerType.InnoSetup:
                case UninstallerType.Msiexec:
                case UninstallerType.Nsis:
                    result = new DriveFileJunkNode(Path.GetDirectoryName(Uninstaller.UninstallerFullFilename),
                        Path.GetFileName(Uninstaller.UninstallerFullFilename), Uninstaller.DisplayName);
                    break;

                default:
                    return Enumerable.Empty<DriveJunkNode>();
            }

            result.Confidence.Add(ConfidencePart.ExplicitConnection);

            return new[] { result };
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

        private IEnumerable<DriveJunkNode> SearchWerReports()
        {
            var output = new List<DriveJunkNode>();

            if (!Directory.Exists(Uninstaller.InstallLocation))
                return output;

            List<string> appExecutables;
            try
            {
                appExecutables = Directory.GetFiles(Uninstaller.InstallLocation, "*.exe", SearchOption.AllDirectories)
                    .Select(Path.GetFileName).ToList();
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine(ex);
                return Enumerable.Empty<DriveJunkNode>();
            }

            var archives = new[]
            {
                WindowsTools.GetEnvironmentPath(Klocman.Native.CSIDL.CSIDL_COMMON_APPDATA),
                WindowsTools.GetEnvironmentPath(Klocman.Native.CSIDL.CSIDL_LOCAL_APPDATA)
            }.Select(x => Path.Combine(x, @"Microsoft\Windows\WER\ReportArchive")).Where(Directory.Exists);

            const string crashLabel = "AppCrash_";
            var candidates = archives.SelectMany(s =>
            {
                try
                {
                    return Directory.GetDirectories(s);
                }
                catch (IOException e)
                {
                    Debug.WriteLine(e.Message);
                }
                catch (UnauthorizedAccessException e)
                {
                    Debug.WriteLine(e.Message);
                }
                return Enumerable.Empty<string>();
            });

            foreach (var candidate in candidates)
            {
                var startIndex = candidate.IndexOf(crashLabel, StringComparison.InvariantCultureIgnoreCase);
                if (startIndex <= 0) continue;
                startIndex = startIndex + crashLabel.Length;

                var count = candidate.IndexOf('_', startIndex) - startIndex;
                if (count <= 1) continue;

                var filename = candidate.Substring(startIndex, count);

                if (appExecutables.Any(x => x.StartsWith(filename, StringComparison.InvariantCultureIgnoreCase)))
                {
                    var node = new DriveDirectoryJunkNode(Path.GetDirectoryName(candidate), Path.GetFileName(candidate),
                        Uninstaller.DisplayName);
                    node.Confidence.Add(ConfidencePart.ExplicitConnection);
                    output.Add(node);
                }
            }

            return output;
        }
    }
}

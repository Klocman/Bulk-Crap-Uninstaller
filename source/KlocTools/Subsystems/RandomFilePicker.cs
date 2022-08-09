/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Klocman.Collections;
using Klocman.Extensions;

namespace Klocman.Subsystems
{
    public class RandomFilePicker
    {
        /// <summary>
        ///     Method to use when selecting another folder/file
        /// </summary>
        public enum GotoType
        {
            Next,
            Previous,
            Random
        }

        private readonly List<string> _directoryHistory = new();
        //private readonly List<string> _fileHistory = new List<string>();
        private readonly ObservedList<string> _matchedDirectories = new();
        private readonly Random _r = new();

        /// <summary>
        ///     Currently selected directory
        /// </summary>
        public string CurrentDirectory { get; private set; } = string.Empty;

        /// <summary>
        ///     Currently selected file
        /// </summary>
        public string CurrentFile { get; private set; }

        /// <summary>
        ///     Filters applied to file extensions
        /// </summary>
        public string[] ExtensionFilters { get; set; } = {};

        /// <summary>
        ///     Filters applied to filenames (exclude if hit)
        /// </summary>
        public string[] FilenameExcludeFilters { get; set; } = {};

        /// <summary>
        ///     If false, file extensions will be stripped when filtering by FilenameFilters.
        /// </summary>
        public bool FilenameFiltersIncludeExtension { get; set; }

        /// <summary>
        ///     Filters applied to filenames (include if hit)
        /// </summary>
        public string[] FilenameFilters { get; set; } = {};

        /// <summary>
        ///     Filtered files in current directory
        /// </summary>
        public string[] FilesInCurrentDir { get; private set; } = {};

        /// <summary>
        ///     Directories that contain filtered files
        /// </summary>
        public string[] MatchedDirectories => _matchedDirectories.ToArray();

        /// <summary>
        ///     How deep to search directories for files
        /// </summary>
        public int MaximumDirectorySearchDepth { get; set; } = 4;

        /// <summary>
        ///     Move from random to pseudo-random algorighm to prevent same
        ///     directory or file being choosen multiple times in a short succession.
        /// </summary>
        public bool PreventDuplicates { get; set; } = true;

        /// <summary>
        ///     Select another file
        /// </summary>
        public void GetNextFile(GotoType direction)
        {
            if (FilesInCurrentDir.Length == 0)
            {
                CurrentFile = string.Empty;
                return;
            }

            switch (direction)
            {
                case GotoType.Random:
                    CurrentFile = FilesInCurrentDir[_r.Next(FilesInCurrentDir.Length)];
                    break;

                case GotoType.Next:
                    CurrentFile = FilesInCurrentDir[Wrap(FilesInCurrentDir.GetPositionOfElement(CurrentFile) + 1,
                        0, FilesInCurrentDir.Length - 1)];
                    break;

                case GotoType.Previous:
                    CurrentFile = FilesInCurrentDir[Wrap(FilesInCurrentDir.GetPositionOfElement(CurrentFile) - 1,
                        0, FilesInCurrentDir.Length - 1)];
                    break;
            }
        }

        /// <summary>
        ///     Select another folder
        /// </summary>
        public void GetNextFolder(GotoType direction)
        {
            if (_matchedDirectories.Count == 0)
            {
                SetCurrentFolder(string.Empty);
                return;
            }

            switch (direction)
            {
                case GotoType.Random:
                {
                    string randomDir;

                    if (PreventDuplicates)
                    {
                        while (true)
                        {
                            if (_directoryHistory.Count == _matchedDirectories.Count)
                            {
                                _directoryHistory.RemoveRange(0, (int) Math.Ceiling(_directoryHistory.Count/2.0));
                            }

                            randomDir = _matchedDirectories[_r.Next(_matchedDirectories.Count)];

                            if (!_directoryHistory.Contains(randomDir))
                            {
                                _directoryHistory.Add(randomDir);
                                break;
                            }
                        }
                    }
                    else
                    {
                        randomDir = _matchedDirectories[_r.Next(_matchedDirectories.Count)];
                    }

                    SetCurrentFolder(randomDir);
                }
                    break;

                case GotoType.Next:
                    SetCurrentFolder(
                        _matchedDirectories[Wrap(_matchedDirectories.IndexOf(CurrentDirectory) + 1,
                            0, _matchedDirectories.Count - 1)]);
                    break;

                case GotoType.Previous:
                    SetCurrentFolder(
                        _matchedDirectories[Wrap(_matchedDirectories.IndexOf(CurrentDirectory) - 1,
                            0, _matchedDirectories.Count - 1)]);
                    break;
            }
        }

        public bool SetCurrentFolder(string newPath)
        {
            try
            {
                FilesInCurrentDir = Directory.Exists(newPath)
                    ? Directory.GetFiles(newPath).Where(CheckFilenameWithFilters).ToArray()
                    : new string[] {};
                CurrentDirectory = newPath;
                return true;
            }
            catch
            {
                FilesInCurrentDir = Array.Empty<string>();
                CurrentDirectory = null;
                return false;
            }
        }

        /// <summary>
        ///     Scan for directories with matching files inside of supplied root directories
        /// </summary>
        public void ScanForDirectories(string[] rootDirectories)
        {
            ScanForDirectories(rootDirectories, null);
        }

        /// <summary>
        ///     Scan for directories with matching files inside of supplied root directories and report directory hits using the
        ///     delegate.
        /// </summary>
        public void ScanForDirectories(string[] rootDirectories, Action onDirectoryFound)
        {
            _matchedDirectories.Clear();
            _directoryHistory.Clear();

            if (onDirectoryFound != null)
                _matchedDirectories.ListChanged += onDirectoryFound;

            foreach (var directory in rootDirectories)
            {
                if (Directory.Exists(directory))
                {
                    _matchedDirectories.Add(directory);
                    _matchedDirectories.AddRange(GetSubFolders(directory));
                }
            }

            if (onDirectoryFound != null)
                _matchedDirectories.ListChanged -= onDirectoryFound;

            if (_matchedDirectories.Count == 0)
            {
                return;
            }

            _matchedDirectories.RemoveAll(obj =>
            {
                var tempList = new List<string>(Directory.GetFiles(obj));
                if (tempList.Count == 0)
                {
                    return true;
                }
                return !tempList.Any(CheckFilenameWithFilters);
            });
        }

        private static int Wrap(int kX, int kLowerBound, int kUpperBound)
        {
            var rangeSize = kUpperBound - kLowerBound + 1;

            if (kX < kLowerBound)
                kX += rangeSize*((kLowerBound - kX)/rangeSize + 1);

            return kLowerBound + (kX - kLowerBound)%rangeSize;
        }

        private bool CheckFilenameWithFilters(string filePath)
        {
            //filePath = filePath.ToLower();
            var fileName = FilenameFiltersIncludeExtension ? filePath :
                Path.GetDirectoryName(filePath) + '\\' + Path.GetFileNameWithoutExtension(filePath);

            var fileNameMatch = false;
            var fileExtMatch = false;

            if (FilenameFilters.Length == 0 ||
                FilenameFilters.Any(filter => fileName.Contains(filter, StringComparison.CurrentCultureIgnoreCase)))
                fileNameMatch = true;

            if (FilenameExcludeFilters.Length > 0 &&
                FilenameExcludeFilters.Any(x => fileName.Contains(x, StringComparison.CurrentCultureIgnoreCase)))
                fileNameMatch = false;

            if (!fileNameMatch)
                return false;

            var extension = Path.GetExtension(filePath);
            if (extension != null)
            {
                var fileExt = extension.ToLower();

                if (ExtensionFilters.Length == 0)
                {
                    fileExtMatch = true;
                }
                else
                {
                    if (ExtensionFilters.Any(filter => fileExt.Contains(filter)))
                    {
                        fileExtMatch = true;
                    }
                }
            }
            return fileExtMatch;
        }

        private IEnumerable<string> GetSubFolders(string rootFolder, int depth = 0)
        {
            var subFolders = new List<string>(Directory.GetDirectories(rootFolder));

            if (subFolders.Count != 0 && depth < MaximumDirectorySearchDepth)
            {
                var subFoldersCopy = subFolders.ToArray();
                foreach (var folder in subFoldersCopy)
                {
                    subFolders.AddRange(GetSubFolders(folder, depth + 1));
                }
            }

            return subFolders;
        }

        /// <summary>
        ///     Recheck that specified directories still exist and contain files that conform to the filters.
        ///     If any directory fails those checks it is removed from the list.
        /// </summary>
        /// <param name="directoriesToRefresh">Directories to check. If none are passed all directories are checked.</param>
        public void RefreshDirectories(params string[] directoriesToRefresh)
        {
            var checkAll = directoriesToRefresh == null || directoriesToRefresh.Length > 0;
            _matchedDirectories.RemoveAll(obj =>
            {
                if (checkAll || directoriesToRefresh!.Any(x => x.Equals(obj)))
                {
                    if (!Directory.Exists(obj))
                        return true;

                    var tempList = new List<string>(Directory.GetFiles(obj));
                    if (tempList.Count == 0)
                    {
                        return true;
                    }
                    return !tempList.Any(CheckFilenameWithFilters);
                }
                return false;
            });
        }
    }
}
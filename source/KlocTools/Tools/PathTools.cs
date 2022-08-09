/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Text.RegularExpressions;
using Klocman.Extensions;
using Microsoft.Win32;

namespace Klocman.Tools
{
    public static class PathTools
    {
        private static Dictionary<string, string> _volumeIdLookup;

        private static readonly char[] PathTrimChars = {
            '\\',
            '/',
            '"',
            // SPACE 
            '\u0020',
            // NO-BREAK SPACE 
            '\u00A0',
            // OGHAM SPACE MARK 
            '\u1680',
            // EN QUAD 
            '\u2000',
            // EM QUAD 
            '\u2001',
            // EN SPACE 
            '\u2002',
            // EM SPACE 
            '\u2003',
            // THREE-PER-EM SPACE 
            '\u2004',
            // FOUR-PER-EM SPACE 
            '\u2005',
            // SIX-PER-EM SPACE 
            '\u2006',
            // FIGURE SPACE 
            '\u2007',
            // PUNCTUATION SPACE 
            '\u2008',
            // THIN SPACE 
            '\u2009',
            // HAIR SPACE 
            '\u200A',
            // NARROW NO-BREAK SPACE 
            '\u202F',
            // MEDIUM MATHEMATICAL SPACE 
            '\u205F',
            // and IDEOGRAPHIC SPACE 
            '\u3000',

            // LINE SEPARATOR 
            '\u2028',

            // PARAGRAPH SEPARATOR  
            '\u2029',

            // CHARACTER TABULATION 
            '\u0009',
            // LINE FEED 
            '\u000A',
            // LINE TABULATION 
            '\u000B',
            // FORM FEED 
            '\u000C',
            // CARRIAGE RETURN 
            '\u000D',
            // NEXT LINE 
            '\u0085'
        };

        private static void PopulateVolumeIdLookup()
        {
            try
            {
                _volumeIdLookup = new Dictionary<string, string>();

                var searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Volume");

                foreach (var queryObj in searcher.Get().OfType<ManagementObject>())
                {
                    var id = (queryObj["DeviceID"] as string)?.TrimEnd('\\', '/');
                    var dl = queryObj["DriveLetter"] as string;

                    if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(dl))
                        continue;

                    _volumeIdLookup.Add(id, dl);
                }
            }
            catch (ManagementException e)
            {
                Console.WriteLine($@"An error occurred while querying for WMI data: {e.Message}");
            }
        }

        /// <summary>
        /// Convert path from the \\?\Volume{} form to the drive letter form.
        /// Only works for volumes with assigned drive letters.
        /// </summary>
        /// <param name="volumePath">Path to any element with volume in \\?\Volume{} form.</param>
        public static string ResolveVolumeIdToPath(string volumePath)
        {
            if (_volumeIdLookup == null)
                PopulateVolumeIdLookup();

            _volumeIdLookup.ForEach(x => volumePath = volumePath.Replace(x.Key, x.Value, StringComparison.OrdinalIgnoreCase));

            return volumePath;
        }

        /// <summary>
        /// Get full path of an application available in current environment. Same as writing it's name in CMD.
        /// </summary>
        /// <param name="filename">Name of the exectuable, including the extension</param>
        /// <returns></returns>
        public static string GetFullPathOfExecutable(string filename)
        {
            IEnumerable<string> paths = new[] { Environment.CurrentDirectory };
            var pathVariable = Environment.GetEnvironmentVariable("PATH");
            if (pathVariable != null) paths = paths.Concat(pathVariable.Split(';'));
            var combinations = paths.Select(x => Path.Combine(x, filename));
            return combinations.FirstOrDefault(File.Exists) ?? GetExecutablePathFromAppPaths(filename);
        }

        /// <param name="exename">name of the exectuable, including .exe</param>
        private static string GetExecutablePathFromAppPaths(string exename)
        {
            const string appPaths = @"Software\Microsoft\Windows\CurrentVersion\App Paths";
            var executableEntry = Path.Combine(appPaths, exename);
            using (var key = Registry.CurrentUser.OpenSubKey(executableEntry) ?? Registry.LocalMachine.OpenSubKey(executableEntry))
            {
                return key?.GetStringSafe(null);
            }
        }

        /// <summary>
        ///     Get full directory path of directory that contains the item pointed at by the path string.
        /// </summary>
        public static string GetDirectory(string fullPath)
        {
            var trimmed = fullPath.TrimEnd('"', ' ', '\\').TrimStart('"', ' ');
            if (trimmed.Contains('\\'))
            {
                var index = trimmed.LastIndexOf('\\');
                if (index < trimmed.Length)
                {
                    return trimmed.Substring(0, index);
                }
            }
            return string.Empty;
        }

        /// <summary>
        ///     Get the topmost part of the path. If this is not a valid path return string.Empty.
        /// </summary>
        public static string GetName(string fullPath)
        {
            var trimmed = fullPath.TrimEnd('"', ' ', '\\');
            if (trimmed.Contains('\\'))
            {
                var index = trimmed.LastIndexOf('\\') + 1;
                if (index < trimmed.Length)
                {
                    return trimmed.Substring(index);
                }
            }
            return string.Empty;
        }

        /// <summary>
        ///     Trim supplied path to the required depth.
        /// </summary>
        /// <param name="path">Path to be trimmed</param>
        /// <param name="maxLevel">Maximal depth of the path, 0 will show only the root node</param>
        /// <returns>Trimmed path</returns>
        public static string GetPathUpToLevel(string path, int maxLevel)
        {
            return GetPathUpToLevel(path, maxLevel, false);
        }

        /// <summary>
        ///     Trim supplied path or full filename to the required depth.
        /// </summary>
        /// <param name="path">Path to be trimmed</param>
        /// <param name="maxLevel">Maximal depth of the path, 0 will show only the root node</param>
        /// <param name="containsFilename">If true, the last part of the path will be ignored, since it is a filename</param>
        /// <returns>Trimmed path</returns>
        public static string GetPathUpToLevel(string path, int maxLevel, bool containsFilename)
        {
            if (string.IsNullOrEmpty(path))
                return string.Empty;

            string directory;
            try
            {
                directory = containsFilename ? Path.GetDirectoryName(path) : Path.GetFullPath(path);

                if (string.IsNullOrEmpty(directory))
                    return string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }

            var directoryParts = directory.Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
            if (directoryParts.Length >= 1)
            {
                var result = string.Empty;

                for (var i = 0; i < maxLevel + 1 && i < directoryParts.Length; i++)
                {
                    result = string.Concat(result, directoryParts[i], "\\");
                }

                return result;
            }
            return string.Empty;
        }

        // Try to get the windows directory, returns null if failed
        public static DirectoryInfo GetWindowsDirectory()
        {
            try
            {
                var windowsDirectory = Environment.GetEnvironmentVariable("SystemRoot");
                if (windowsDirectory != null) return new DirectoryInfo(windowsDirectory);
            }
            catch
            {
                //Check other
            }
            try
            {
                var windowsDirectory = Environment.GetEnvironmentVariable("windir");
                if (windowsDirectory != null) return new DirectoryInfo(windowsDirectory);
            }
            catch
            {
                //Messed up environment variables or security too high
            }
            return null;
        }

        /// <summary>
        ///     Change path to normal case. Example: C:\PROGRAM FILES => C:\Program files
        /// </summary>
        public static string PathToNormalCase(string path)
        {
            var directoryParts = NormalizePath(path).Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
            if (directoryParts.Length < 1)
                return string.Empty;

            var result = string.Empty;

            for (var i = 0; i < directoryParts.Length; i++)
            {
                var part = directoryParts[i].ToLower();
                result = string.Concat(result, part.Substring(0, 1).ToUpperInvariant() + part.Substring(1), "\\");
            }

            return result;
        }

        public static bool PathsEqual(string path1, string path2)
        {
            if (string.IsNullOrEmpty(path1) || string.IsNullOrEmpty(path2))
                return false;

            try
            {
                path1 = path1.SafeNormalize().Trim(PathTrimChars);
                path2 = path2.SafeNormalize().Trim(PathTrimChars);
                return path1.Equals(path2, StringComparison.InvariantCultureIgnoreCase);
            }
            catch
            {
                // Fall back to ordinal in case SafeNormalize isn't safe enough
                return path1.Trim(PathTrimChars).Equals(path2.Trim(PathTrimChars), StringComparison.OrdinalIgnoreCase);
            }
        }

        /// <summary>
        /// Remove unnecessary spaces, quotes and path separators from start and end of the path.
        /// Might produce different path than intended in case it contains invalid unicode characters.
        /// </summary>
        public static string NormalizePath(string path1)
        {
            if (path1 == null) throw new ArgumentNullException(nameof(path1));
            return path1.SafeNormalize().Trim(PathTrimChars);
        }

        public static bool PathsEqual(FileSystemInfo path1, FileSystemInfo path2)
        {
            if (path1 == null || path2 == null)
                return false;

            return PathsEqual(path1.FullName, path2.FullName);
        }

        /// <summary>
        /// Replace all invalid file name characters from a string with _ so that it can be used as a file name.
        /// </summary>
        public static string SanitizeFileName(string name)
        {
            var invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
            var invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return Regex.Replace(name, invalidRegStr, "_");
        }

        /// <summary>
        /// Version of Path.Combine with much less restrictive input checks, and additional path cleanup.
        /// </summary>
        public static string GenerousCombine(string path1, string path2)
        {
            if (path1 == null || path2 == null)
                throw new ArgumentNullException(path1 == null ? nameof(path1) : nameof(path2));

            path1 = NormalizePath(path1);
            path2 = NormalizePath(path2);

            if (path2.Length == 0) return path1;
            if (path1.Length == 0 || Path.IsPathRooted(path2)) return path2;

            return path1 + Path.DirectorySeparatorChar + path2;
        }

        /// <summary>
        /// Get a cleaned up list of all paths in the PATH variables of both current user and the machine. Duplicates are removed.
        /// </summary>
        public static IEnumerable<string> GetAllEnvironmentPaths()
        {
            var parts = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.User)?.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries) ?? Enumerable.Empty<string>();
            parts = parts.Concat(Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Machine)?.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries) ?? Enumerable.Empty<string>());

            return parts.Where(x => !string.IsNullOrEmpty(x)).Select(NormalizePath).Select(Path.GetFullPath).DistinctBy(s => s.ToLower());
        }

        /// <summary>
        /// Check if subPath is a sub path inside basePath.
        /// If isFilesystemPath is true then attempt to normalize the path to its absolute form on the filesystem. Set to false for registry and other paths.
        /// </summary>
        public static bool SubPathIsInsideBasePath(string basePath, string subPath, bool normalizeFilesystemPath)
        {
            if (basePath == null) return false;
            basePath = NormalizePath(basePath).Replace('\\', '/');
            if (string.IsNullOrEmpty(basePath)) return false;
            if (normalizeFilesystemPath)
            {
                try { basePath = Path.GetFullPath(basePath).Replace('\\', '/'); }
                catch (SystemException) { }
            }

            if (subPath == null) return false;
            subPath = NormalizePath(subPath).Replace('\\', '/');
            if (string.IsNullOrEmpty(subPath)) return false;
            if (normalizeFilesystemPath)
            {
                try { subPath = Path.GetFullPath(subPath).Replace('\\', '/'); }
                catch (SystemException) { }
            }

            return subPath.StartsWith(basePath + '/', StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
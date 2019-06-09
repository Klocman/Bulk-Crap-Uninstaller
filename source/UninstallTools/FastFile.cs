using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Klocman.Extensions;

namespace UninstallTools
{
    /// <summary>
    /// Wrapper for File and Directory classes that can use Everything search engine if it's available for speedup
    /// </summary>
    public static class FastFile
    {
        public static bool _everythingIsAvailable = true;

        private static readonly Dictionary<string, bool> _indexedDrives = new Dictionary<string, bool>();

        /*static FastFile()
        {
            try
            {
                _everythingIsAvailable = EvGetFileSize("bcuninstaller.exe") > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"Could not connect to Everything search service - " + ex);
            }
        }*/

        public static bool DirectoryExists(string path)
        {
            if (!IsEverythingReady(GetPathRoot(path))) return Directory.Exists(path);

            try
            {
                return EvGetNames(path.TrimEnd('\\', '/'), false, true, false).Any();
            }
            catch (ArgumentNullException)
            {
                throw;
            }
            catch (ArgumentOutOfRangeException)
            {
                throw;
            }
            catch (SystemException ex)
            {
                Console.WriteLine(ex);
                _everythingIsAvailable = false;
                return DirectoryExists(path);
            }
        }

        public static bool DirectoryHasSystemAttribute(string path)
        {
            var dir = new DirectoryInfo(path);
            return (dir.Attributes & FileAttributes.System) == FileAttributes.System;
        }

        public static bool FileExists(string path)
        {
            if (!IsEverythingReady(GetPathRoot(path))) return File.Exists(path);

            try
            {
                return EvGetNames(path, false, false, true).Any(x => string.Equals(x, path, StringComparison.OrdinalIgnoreCase));
            }
            catch (ArgumentNullException)
            {
                throw;
            }
            catch (ArgumentOutOfRangeException)
            {
                throw;
            }
            catch (SystemException ex)
            {
                Console.WriteLine(ex);
                _everythingIsAvailable = false;
                return FileExists(path);
            }
        }

        public static IEnumerable<string> GetDirectories(string path, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            if (!IsEverythingReady(GetPathRoot(path)))
                return Directory.GetDirectories(path, "*", searchOption);

            try
            {
                switch (searchOption)
                {
                    case SearchOption.TopDirectoryOnly:
                        return EvGetNames(path, true, true, false);
                    case SearchOption.AllDirectories:
                        // Add \ to end to prevent the path itself appearing in results
                        return EvGetNames(path.TrimEnd('\\', '/').Append("\\"), false, true, false);
                    default:
                        throw new ArgumentOutOfRangeException(nameof(searchOption), searchOption, null);
                }
            }
            catch (ArgumentNullException)
            {
                throw;
            }
            catch (ArgumentOutOfRangeException)
            {
                throw;
            }
            catch (SystemException ex)
            {
                Console.WriteLine(ex);
                _everythingIsAvailable = false;
                return GetDirectories(path, searchOption);
            }
        }

        public static DateTime GetDirectoryCreationTime(string directory)
        {
            if (!IsEverythingReady(GetPathRoot(directory)))
                return Directory.GetCreationTime(directory);

            try
            {
                var dates = EvGetDates(directory.TrimEnd('\\', '/'), false, true, false, true).ToList();
                if (dates.Count == 0) throw new DirectoryNotFoundException(directory);
                return dates[0].Key;
            }
            catch (ArgumentNullException)
            {
                throw;
            }
            catch (DirectoryNotFoundException)
            {
                throw;
            }
            catch (SystemException ex)
            {
                Console.WriteLine(ex);
                _everythingIsAvailable = false;
                return GetDirectoryCreationTime(directory);
            }
        }

        public static DateTime GetFileCreationTime(string file)
        {
            if (!IsEverythingReady(GetPathRoot(file)))
                return File.GetCreationTime(file);

            try
            {
                var dates = EvGetDates(file, false, false, true, true).ToList();
                if (dates.Count == 0) throw new FileNotFoundException(file);
                return dates[0].Key;
            }
            catch (ArgumentNullException)
            {
                throw;
            }
            catch (FileNotFoundException)
            {
                throw;
            }
            catch (SystemException ex)
            {
                Console.WriteLine(ex);
                _everythingIsAvailable = false;
                return GetFileCreationTime(file);
            }
        }

        public static IEnumerable<string> GetFiles(string path, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            if (!IsEverythingReady(GetPathRoot(path)))
                return Directory.GetFiles(path, "*", searchOption);

            try
            {
                switch (searchOption)
                {
                    case SearchOption.TopDirectoryOnly:
                        return EvGetNames(path, true, false, true);
                    case SearchOption.AllDirectories:
                        // Add \ to end to prevent the path itself appearing in results
                        return EvGetNames(path.TrimEnd('\\', '/').Append("\\"), false, false, true);
                    default:
                        throw new ArgumentOutOfRangeException(nameof(searchOption), searchOption, null);
                }
            }
            catch (ArgumentNullException)
            {
                throw;
            }
            catch (ArgumentOutOfRangeException)
            {
                throw;
            }
            catch (SystemException ex)
            {
                Console.WriteLine(ex);
                _everythingIsAvailable = false;
                return GetFiles(path, searchOption);
            }
        }

        private static IEnumerable<KeyValuePair<DateTime, string>> EvGetDates(string path, bool parent, bool dirOnly, bool fileOnly, bool firstOnly)
        {
            // -dc -dm
            var output = StartHelperAndReadOutput($"{GetFileDirFlag(dirOnly, fileOnly)} -dc -date-format 2 {(parent ? "parent" : "path")}:\"{path}\" {(firstOnly ? "-n 1" : "")}");
            var allResults = output.SplitNewlines(StringSplitOptions.RemoveEmptyEntries);
            foreach (var result in allResults)
            {
                var split = result.Split(new[] { ' ' }, 2, StringSplitOptions.None);
                yield return new KeyValuePair<DateTime, string>(DateTime.FromFileTime(long.Parse(split[0])), split[1]);
            }
        }

        private static IEnumerable<string> EvGetNames(string path, bool parent, bool dirOnly, bool fileOnly)
        {
            var output = StartHelperAndReadOutput($"{GetFileDirFlag(dirOnly, fileOnly)} {(parent ? "parent" : "path")}:\"{path}\"");
            return output.SplitNewlines(StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Files only
        /// </summary>
        private static IEnumerable<KeyValuePair<long, string>> EvGetSizes(string path, bool parent)
        {
            var output = StartHelperAndReadOutput($"-size -a-d -size-leading-zero -no-digit-grouping -size-format 1 {(parent ? "parent" : "path")}:\"{path}\"");
            var allResults = output.SplitNewlines(StringSplitOptions.RemoveEmptyEntries);
            foreach (var result in allResults)
            {
                var split = result.Split(new[] { ' ' }, 2, StringSplitOptions.None);
                yield return new KeyValuePair<long, string>(long.Parse(split[0]), split[1]);
            }
        }

        private static string GetFileDirFlag(bool dirOnly, bool fileOnly)
        {
            return dirOnly ? "-ad" : (fileOnly ? "-a-d" : "");
        }

        private static string GetPathRoot(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;
            var i = path.IndexOf(':');
            if (i <= 0)
                return null;
            var r = path.Substring(0, i).TrimStart('"', ' ').ToLower();
            return r.Length == 0 ? null : r;
        }

        private static bool IsEverythingReady(string root)
        {
            if (!_everythingIsAvailable || string.IsNullOrEmpty(root)) return false;

            if (_indexedDrives.TryGetValue(root.ToLower(), out var result))
                return result;

            var indexed = EvGetNames(root + ":\\", true, false, false).Any();
            _indexedDrives.Add(root.ToLower(), indexed);
            return indexed;
        }

        private static string StartHelperAndReadOutput(string args)
        {
            using (var process = Process.Start(
                new ProcessStartInfo(@"D:\ES-1.1.0.11\es.exe", args)
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = false,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.Unicode
                }))
            {
                Console.WriteLine(args);
                if (process == null) throw new ArgumentNullException(nameof(process));
                var output = process.StandardOutput.ReadToEnd();
                if (process.ExitCode == 0) return output;
                throw new IOException("Failed to connecto to Everything", process.ExitCode);
            }
        }
    }
}

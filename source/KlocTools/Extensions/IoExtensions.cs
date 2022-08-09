/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Klocman.Extensions
{
    public static class IoExtensions
    {
        /// <summary>
        ///     Get basic attributes of this file in a Name/Value format.
        /// </summary>
        /// <param name="file">Base file</param>
        /// <returns>Extended attributes of this file</returns>
        public static IEnumerable<KeyValuePair<string, object>> GetAttributes(this FileInfo file)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));
            if (!file.Exists)
                throw new ArgumentException(@"Requested file doesn't exist", nameof(file));

            var lq = from property in typeof (FileInfo).GetProperties()
                select new KeyValuePair<string, object>(property.Name, property.GetValue(file, new object[] {}));

            return lq;
        }

        /// <summary>
        ///     Attempt to get DriveInfo of this directory's root partition.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>First drive matching the root name, null if no matches were found.</returns>
        public static DriveInfo GetDriveInfo(this DirectoryInfo obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            return DriveInfo.GetDrives().FirstOrDefault(x => obj.RootEquals(x.RootDirectory));
        }

        public static bool IsFileLocked(this FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                // File is locked
                return true;
            }
            finally
            {
                stream?.Close();
            }

            // File is not locked
            return false;
        }

        /// <summary>
        ///     Check if this drive is formatted with NTFS file system
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsNtfs(this DriveInfo obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            return obj.DriveFormat.Contains("NTFS");
        }

        /// <summary>
        ///     Compare roots of the selected directory paths.
        ///     If any path is null, false is returned (even if both are null).
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="target">Path to compare root with</param>
        /// <returns>True if roots are equal</returns>
        public static bool RootEquals(this DirectoryInfo obj, DirectoryInfo target)
        {
            if (obj == null || target == null)
                return false;

            if (ReferenceEquals(obj, target))
                return true;

            return obj.Root.Name.Equals(target.Root.Name, StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        ///     Compare roots of the selected file paths.
        ///     If any path is null, false is returned (even if both are null).
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="target">Path to compare root with</param>
        /// <returns>True if roots are equal</returns>
        public static bool RootEquals(this FileInfo obj, FileInfo target)
        {
            if (obj == null || target == null)
                return false;

            if (ReferenceEquals(obj, target))
                return true;

            return obj.Directory.RootEquals(target.Directory);
        }
        
        public static string GetNameWithoutExtension(this FileSystemInfo obj)
        {
            return Path.GetFileNameWithoutExtension(obj.Name);
        }
    }
}
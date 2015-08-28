using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Klocman.Native;
using Klocman.Tools;

namespace UninstallTools.Startup.Normal
{
    public static class StartupEntryFactory
    {
        /// <summary>
        ///     Ordinal locations that contain startup entries
        /// </summary>
        internal static readonly IEnumerable<StartupPointData> RunLocations = new[]
        {
            // Normally those keys should not exist, they are not scanned by windows
            /*new StartupPointData(false, true, false, true, @"HKCU\Wow\Run",
                @"HKEY_CURRENT_USER\Software\Wow6432Node\Microsoft\Windows\CurrentVersion\Run\"),
            new StartupPointData(false, true, true, true, @"HKCU\Wow\RunOnce",
                @"HKEY_CURRENT_USER\Software\Wow6432Node\Microsoft\Windows\CurrentVersion\RunOnce\"),*/

            new StartupPointData(true, true, false, true, @"HKLM\Wow\Run",
                @"HKEY_LOCAL_MACHINE\Software\Wow6432Node\Microsoft\Windows\CurrentVersion\Run"),
            new StartupPointData(true, true, true, true, @"HKLM\Wow\RunOnce",
                @"HKEY_LOCAL_MACHINE\Software\Wow6432Node\Microsoft\Windows\CurrentVersion\RunOnce"),
            new StartupPointData(true, true, false, false, @"HKLM\Run",
                @"HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\Run"),
            new StartupPointData(false, true, false, false, @"HKCU\Run",
                @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Run"),
            new StartupPointData(true, true, true, false, @"HKLM\RunOnce)",
                @"HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\RunOnce"),
            new StartupPointData(false, true, true, false, @"HKCU\RunOnce)",
                @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\RunOnce"),
            new StartupPointData(false, false, false, false, @"User\Startup",
                WindowsTools.GetEnvironmentPath(CSIDL.CSIDL_STARTUP)),
            new StartupPointData(true, false, false, false, @"Common\Startup",
                WindowsTools.GetEnvironmentPath(CSIDL.CSIDL_COMMON_STARTUP))
        };

        /// <summary>
        ///     Look for and return all of the startup entries stored in Startup folders and Run/RunOnce registry keys.
        /// </summary>
        public static IEnumerable<StartupEntry> GetStartupItems()
        {
            var results = new List<StartupEntry>();
            foreach (var point in RunLocations)
            {
                if (point != null)
                    results.AddRange(point.IsRegKey ? GetRegStartupItems(point) : GetDriveStartupItems(point));
            }
            return StartupEntryManager.DisableFunctions.AddDisableInfo(results);
        }

        /// <summary>
        ///     Look for links in startup folders
        /// </summary>
        private static IEnumerable<StartupEntry> GetDriveStartupItems(StartupPointData point)
        {
            if (!Directory.Exists(point.Path))
                Directory.CreateDirectory(point.Path);

            foreach (var name in Directory.GetFiles(point.Path)
                .Where(name => ".lnk".Equals(Path.GetExtension(name), StringComparison.CurrentCultureIgnoreCase)))
            {
                string target;
                try
                {
                    target = WindowsTools.ResolveShortcut(name);
                }
                catch
                {
                    target = string.Empty;
                }
                yield return new StartupEntry(point, Path.GetFileName(name), target);
            }
        }

        /// <summary>
        ///     Look for registry values in the run keys
        /// </summary>
        private static IEnumerable<StartupEntry> GetRegStartupItems(StartupPointData point)
        {
            var results = new List<StartupEntry>();
            try
            {
                using (var rKey = RegistryTools.OpenRegistryKey(point.Path))
                {
                    if (rKey != null)
                    {
                        results.AddRange(from name in rKey.GetValueNames()
                            let result = rKey.GetValue(name) as string
                            where !string.IsNullOrEmpty(result)
                            select new StartupEntry(point, name, result));
                    }
                }
            }
            catch (ArgumentException)
            {
                // Key doesn't exist, create it
                RegistryTools.CreateSubKeyRecursively(point.Path)?.Close();
            }

            return results;
        }
    }
}
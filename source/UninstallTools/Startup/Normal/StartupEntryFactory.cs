/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using Klocman.Extensions;
using Klocman.Forms.Tools;
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
        }.AsEnumerable();
        
        /// <summary>
        ///     Look for and return all of the startup entries stored in Startup folders and Run/RunOnce registry keys.
        /// </summary>
        public static IEnumerable<StartupEntry> GetStartupItems()
        {
            var results = new List<StartupEntry>();
            foreach (var point in RunLocations)
            {
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
                yield break;

            foreach (var name in Directory.GetFiles(point.Path)
                .Where(name => ".lnk".Equals(Path.GetExtension(name), StringComparison.CurrentCultureIgnoreCase)))
            {
                StartupEntry result;
                try
                {
                    result = new StartupEntry(point, Path.GetFileName(name), WindowsTools.ResolveShortcut(name));
                }
                catch (Exception ex)
                {
                    PremadeDialogs.GenericError(ex);
                    continue;
                }
                yield return result;
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
                        foreach (var name in rKey.GetValueNames())
                        {
                            var result = rKey.GetStringSafe(name);
                            if (string.IsNullOrEmpty(result))
                                continue;

                            try
                            {
                                results.Add(new StartupEntry(point, name, result));
                            }
                            catch (Exception ex)
                            {
                                PremadeDialogs.GenericError(ex);
                            }
                        }
                    }
                }
            }
            catch (ArgumentException)
            {
                // Key doesn't exist, create it
                RegistryTools.CreateSubKeyRecursively(point.Path)?.Close();
            }
            catch (SecurityException ex)
            {
                Trace.WriteLine(@"Failed to process startup entries: " + ex);
            }

            return results;
        }
    }
}
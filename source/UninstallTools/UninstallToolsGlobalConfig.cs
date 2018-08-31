using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using Klocman.Extensions;
using Klocman.Native;
using Klocman.Tools;
using UninstallTools.Factory;

namespace UninstallTools
{
    public static class UninstallToolsGlobalConfig
    {
        static UninstallToolsGlobalConfig()
        {
            AssemblyLocation = Assembly.GetExecutingAssembly().Location;
            if (AssemblyLocation.ContainsAny(new[] { ".dll", ".exe" }, StringComparison.OrdinalIgnoreCase))
                AssemblyLocation = PathTools.GetDirectory(AssemblyLocation);

            UninstallerAutomatizerPath = Path.Combine(AssemblyLocation, @"UninstallerAutomatizer.exe");
            UninstallerAutomatizerExists = File.Exists(UninstallerAutomatizerPath);

            QuestionableDirectoryNames = new[]
            {
                "install", "settings", "config", "configuration", "users", "data"
            }.AsEnumerable();

            DirectoryBlacklist = new[]
            {
                "Microsoft", "Microsoft Games", "Temp", "Programs", "Common", "Common Files", "Clients",
                "Desktop", "Internet Explorer", "Windows", "Windows NT", "Windows Photo Viewer", "Windows Mail",
                "Windows Defender", "Windows Media Player", "Uninstall Information", "Reference Assemblies",
                "InstallShield Installation Information", "Installer"
            }.AsEnumerable();

            StockProgramFiles = new[]
            {
                WindowsTools.GetEnvironmentPath(CSIDL.CSIDL_PROGRAM_FILES),
                WindowsTools.GetProgramFilesX86Path()
            }.Distinct().ToList().AsEnumerable();

            // JunkSearchDirs --------------
            var localData = WindowsTools.GetEnvironmentPath(CSIDL.CSIDL_LOCAL_APPDATA);
            var paths = new List<string>
            {
                WindowsTools.GetEnvironmentPath(CSIDL.CSIDL_PROGRAMS),
                WindowsTools.GetEnvironmentPath(CSIDL.CSIDL_COMMON_PROGRAMS),
                WindowsTools.GetEnvironmentPath(CSIDL.CSIDL_APPDATA),
                WindowsTools.GetEnvironmentPath(CSIDL.CSIDL_COMMON_APPDATA),
                localData
                //Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) danger?
            };

            var appDataParentDir = Path.GetDirectoryName(localData.TrimEnd('\\', '/', ' '));
            if (!string.IsNullOrEmpty(appDataParentDir))
            {
                var lowDir = Path.Combine(appDataParentDir, "LocalLow");
                if (Directory.Exists(lowDir))
                    paths.Add(lowDir);
            }

            var vsPath = Path.Combine(localData, "VirtualStore");
            if (Directory.Exists(vsPath))
                paths.AddRange(Directory.GetDirectories(vsPath));

            JunkSearchDirs = paths.Distinct().ToList().AsEnumerable();

            AppInfoCachePath = Path.Combine(AssemblyLocation, "InfoCache.xml");
        }

        public static bool EnableAppInfoCache
        {
            get { return UninstallerFactoryCache != null; }
            set
            {
                if (value)
                {
                    var cachePath = AppInfoCachePath;
                    try
                    {
                        if (File.Exists(cachePath))
                            UninstallerFactoryCache = ApplicationUninstallerFactoryCache.Load(cachePath);
                        else
                            UninstallerFactoryCache = new ApplicationUninstallerFactoryCache(cachePath);
                    }
                    catch (SystemException e)
                    {
                        UninstallerFactoryCache = new ApplicationUninstallerFactoryCache(cachePath);
                        Console.WriteLine(e);
                    }
                }
                else
                {
                    UninstallerFactoryCache?.Delete();
                    UninstallerFactoryCache = null;
                }
            }
        }

        public static string AppInfoCachePath { get; }
        
        internal static ApplicationUninstallerFactoryCache UninstallerFactoryCache { get; private set; }
        
        /// <summary>
        ///     Path to directory this assembly sits in.
        /// </summary>
        internal static string AssemblyLocation { get; }

        public static bool AutoDetectCustomProgramFiles { get; set; }

        public static bool AutoDetectScanRemovable { get; set; }

        /// <summary>
        ///     Custom "Program Files" directories. Use with dirs that get used to install applications to.
        /// </summary>
        public static string[] CustomProgramFiles { get; set; }

        /// <summary>
        ///     Directory names that should be ignored for safety.
        /// </summary>
        internal static IEnumerable<string> DirectoryBlacklist { get; }

        /// <summary>
        ///     Directories that can contain program junk.
        /// </summary>
        internal static IEnumerable<string> JunkSearchDirs { get; }

        /// <summary>
        ///     Directory names that probably aren't top-level or contain applications.
        /// </summary>
        internal static IEnumerable<string> QuestionableDirectoryNames { get; }

        /// <summary>
        ///     Automatize non-quiet uninstallers.
        /// </summary>
        public static bool QuietAutomatization { get; set; }

        /// <summary>
        ///     Kill stuck automatized uninstallers.
        /// </summary>
        public static bool QuietAutomatizationKillStuck { get; set; }

        public static bool ScanSteam { get; set; }

        public static bool ScanStoreApps { get; set; }

        public static bool ScanOculus { get; set; }

        public static bool ScanWinFeatures { get; set; }

        public static bool ScanWinUpdates { get; set; }

        public static bool ScanChocolatey { get; set; }

        /// <summary>
        ///     Built-in program files paths.
        /// </summary>
        internal static IEnumerable<string> StockProgramFiles { get; }

        public static bool ScanRegistry { get; set; }
        public static bool ScanDrives { get; set; }
        public static bool ScanPreDefined { get; set; }

        /// <summary>
        /// TODO hook up
        /// </summary>
        public static bool UseQuietUninstallDaemon { get; set; }

        /// <summary>
        ///     Directiories containing programs, both built in "Program Files" and user-defined ones. Fast.
        /// </summary>
        internal static IEnumerable<string> GetAllProgramFiles()
        {
            if (CustomProgramFiles == null || CustomProgramFiles.Length == 0)
                return StockProgramFiles;

            // Create copy of custom dirs in case they change
            return StockProgramFiles.Concat(CustomProgramFiles).ToList();
        }

        /// <summary>
        ///     Get a list of directiories containing programs. Optionally user-defined directories are added.
        ///     The boolean value is true if the directory is confirmed to contain 64bit applications, false if 32bit.
        /// </summary>
        /// <param name="includeUserDirectories">Add user-defined directories.</param>
        internal static IEnumerable<KeyValuePair<DirectoryInfo, bool?>> GetProgramFilesDirectories(
            bool includeUserDirectories)
        {
            var pfDirectories = new List<KeyValuePair<string, bool?>>(2);

            var pf64 = WindowsTools.GetEnvironmentPath(CSIDL.CSIDL_PROGRAM_FILES);
            var pf32 = WindowsTools.GetProgramFilesX86Path();
            pfDirectories.Add(new KeyValuePair<string, bool?>(pf32, false));
            if (!PathTools.PathsEqual(pf32, pf64))
                pfDirectories.Add(new KeyValuePair<string, bool?>(pf64, true));

            if (includeUserDirectories && CustomProgramFiles != null)
                pfDirectories.AddRange(CustomProgramFiles.Where(
                    x => !pfDirectories.Any(y => PathTools.PathsEqual(x, y.Key)))
                    .Select(x => new KeyValuePair<string, bool?>(x, null)));

            var output = new List<KeyValuePair<DirectoryInfo, bool?>>();
            foreach (var directory in pfDirectories.ToList())
            {
                // Ignore missing or inaccessible directories
                try
                {
                    var di = new DirectoryInfo(directory.Key);
                    if (di.Exists)
                        output.Add(new KeyValuePair<DirectoryInfo, bool?>(di, directory.Value));
                }
                catch (Exception ex)
                {
                    Debug.Fail("Failed to open dir", ex.Message);
                }
            }

            return output;
        }

        /// <summary>
        ///     Check if dir is a system directory and should be left alone.
        /// </summary>
        public static bool IsSystemDirectory(DirectoryInfo dir)
        {
            return DirectoryBlacklist.Any(y => y.Equals(dir.Name, StringComparison.InvariantCultureIgnoreCase))
                || (dir.Attributes & FileAttributes.System) == FileAttributes.System;
        }
        
        /// <summary>
        ///     Check if dir is a system directory and should be left alone.
        /// </summary>
        public static bool IsSystemDirectory(string installLocation)
        {
            return IsSystemDirectory(new DirectoryInfo(installLocation));
        }

        /// <summary>
        ///     Safely try to extract icon from specified file. Return null if failed.
        /// </summary>
        internal static Icon TryExtractAssociatedIcon(string path)
        {
            if (path != null && File.Exists(path))
            {
                try
                {
                    return DrawingTools.ExtractAssociatedIcon(path);
                }
                catch (Exception ex)
                {
                    Debug.Fail(ex.Message);
                }
            }
            return null;
        }

        public static string UninstallerAutomatizerPath { get; }
        public static bool UninstallerAutomatizerExists { get; }
    }
}
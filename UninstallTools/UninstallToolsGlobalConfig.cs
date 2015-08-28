using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Klocman.Extensions;
using Klocman.Native;
using Klocman.Tools;

namespace UninstallTools
{
    public static class UninstallToolsGlobalConfig
    {
        internal static readonly IEnumerable<string> DirectoryBlacklist = new[]
        {
            "Microsoft", "Microsoft Games", "Temp", "Programs", "Common", "Common Files", "Clients",
            "Desktop", "Internet Explorer", "Windows NT", "Windows Photo Viewer", "Windows Mail", "Windows Defender"
        };

        internal static readonly IEnumerable<string> QuestionableDirectoryNames = new[]
        {
            "install", "settings", "config", "configuration",
            "users", "data"
        };

        public static string[] CustomProgramFiles { get; set; }

        internal static IEnumerable<string> AllProgramFiles
            => StockProgramFiles.Concat(CustomProgramFiles ?? Enumerable.Empty<string>());

        internal static IEnumerable<string> JunkSearchDirs => new[]
        {
            WindowsTools.GetEnvironmentPath(CSIDL.CSIDL_PROGRAMS),
            WindowsTools.GetEnvironmentPath(CSIDL.CSIDL_COMMON_PROGRAMS),
            WindowsTools.GetEnvironmentPath(CSIDL.CSIDL_APPDATA),
            WindowsTools.GetEnvironmentPath(CSIDL.CSIDL_COMMON_APPDATA),
            WindowsTools.GetEnvironmentPath(CSIDL.CSIDL_LOCAL_APPDATA)

            //Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) danger?
        }.Distinct();

        internal static IEnumerable<string> StockProgramFiles => new[]
        {
            WindowsTools.GetEnvironmentPath(CSIDL.CSIDL_PROGRAM_FILES),
            WindowsTools.GetProgramFilesX86Path()
        }.Distinct();

        internal static bool IsSystemDirectory(DirectoryInfo dir)
        {
            return DirectoryBlacklist.Any(y => y.Equals(dir.Name, StringComparison.Ordinal))
                   || dir.Attributes.Contains(FileAttributes.System);
        }
    }
}
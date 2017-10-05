/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SteamHelper
{
    internal class Program
    {
        private static QueryType _queryType = QueryType.None;
        private static bool _silent;
        private static int _appId;

        private const int InvalidArgumentCode = 10022;

        /// <summary>
        /// Return codes:
        /// 0 - The operation completed successfully.
        /// 59 - An unexpected network error occurred.
        /// 1223 - The operation was canceled by the user.
        /// 
        /// Commands
        /// u[ninstall] [/s[ilent]] AppID     - Uninstall an app
        /// i[nfo] AppID                      - Show info about an app
        /// l[ist]                            - List app ID's
        /// steam                             - Show Steam install location
        /// </summary>                        
        private static int Main(string[] args)
        {
            try
            {
                try { Console.OutputEncoding = Encoding.Unicode; }
                catch (IOException) { /*Old .NET v4 without support for unicode output*/ }

                ProcessCommandlineArguments(args);

                switch (_queryType)
                {
                    case QueryType.GetInfo:
                        var appInfo = SteamApplicationInfo.FromAppId(_appId);
                        foreach (var property in typeof(SteamApplicationInfo).GetProperties(BindingFlags.Public | BindingFlags.Instance))
                            Console.WriteLine("{0} - {1}", property.Name, property.GetValue(appInfo, null) ?? "N/A");
                        break;

                    case QueryType.Uninstall:
                        SteamUninstaller.UninstallSteamApp(SteamApplicationInfo.FromAppId(_appId), _silent);
                        break;

                    case QueryType.List:
                        foreach (var result in SteamInstallation.Instance.SteamAppsLocations
                            .SelectMany(x => Directory.GetFiles(x, @"appmanifest_*.acf")
                                .Select(p => (Path.GetFileNameWithoutExtension(p)?.Substring(12))).Where(p => p != null))
                            .Select(int.Parse).OrderBy(x => x))
                            Console.WriteLine(result);
                        break;

                    case QueryType.SteamDir:
                        Console.WriteLine(SteamInstallation.Instance.InstallationDirectory);
                        break;
                }
            }
            catch (OperationCanceledException)
            {
                return 1223;
            }
            catch(FormatException ex)
            {
                LogWriter.WriteExceptionToLog(ex);
                Console.WriteLine("Error: {0}", ex.Message);
                return InvalidArgumentCode;
            }
            catch (Exception ex)
            {
                LogWriter.WriteExceptionToLog(ex);
                Console.WriteLine("Error: {0}", ex.Message);
                return 59;
            }
            return 0;
        }

        private static void ProcessCommandlineArguments(IEnumerable<string> args)
        {
            foreach (var arg in args)
            {
                switch (arg.ToLowerInvariant())
                {
                    case @"u":
                    case @"uninstall":
                        if (_queryType != QueryType.None) throw new FormatException(@"Multiple commands specified");
                        _queryType = QueryType.Uninstall;
                        break;

                    case @"i":
                    case @"info":
                        if (_queryType != QueryType.None) throw new FormatException(@"Multiple commands specified");
                        _queryType = QueryType.GetInfo;
                        break;

                    case @"/s":
                    case @"/silent":
                        if (_queryType != QueryType.Uninstall)
                            throw new FormatException(@"/silent must follow the uninstall command");
                        _silent = true;
                        break;

                    case @"l":
                    case @"list":
                        if (_queryType != QueryType.None) throw new FormatException(@"Multiple commands specified");
                        _queryType = QueryType.List;
                        break;

                    case @"steam":
                        if (_queryType != QueryType.None) throw new FormatException(@"Multiple commands specified");
                        _queryType = QueryType.SteamDir;
                        break;

                    default:
                        if (_appId != default(int)) throw new FormatException(@"Multiple AppIDs specified");
                        if (!int.TryParse(arg, out _appId)) throw new FormatException($@"Unknown argument: {arg}");
                        break;
                }
            }

            if (_queryType == QueryType.None)
                throw new FormatException(@"No commands specified");

            if (_queryType != QueryType.List
                && _queryType != QueryType.SteamDir
                && _appId == default(int))
                throw new FormatException(@"No AppID specified");
        }

        private enum QueryType
        {
            None,
            Uninstall,
            GetInfo,
            List,
            SteamDir
        }
    }
}
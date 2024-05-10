/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using Klocman.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using UninstallTools;
using UninstallTools.Factory;
using UninstallTools.Junk;
using UninstallTools.Junk.Confidence;
using UninstallTools.Junk.Containers;
using UninstallTools.Lists;
using UninstallTools.Uninstaller;

namespace BCU_console
{
    internal static class Program
    {
        private static void ShowHelp()
        {
            Console.WriteLine(@"BCU-console [help | /?] - Show help (this screen)

BCU-console uninstall [drive:][path]filename [/Q] [/U] [/V] [/J=<Level>] - Uninstall applications.
 [drive:][path]	– Specifies drive and directory of the uninstall list.
 filename       – Specifies filename of the .bcul uninstall list that contains information about
                  what applications to uninstall.

BCU-console export [drive:][path]filename [/Q] [/U] [/V] - Export installed application data to xml file.
 [drive:][path]	– Specifies drive and directory to where the export should be saved.
 filename       – Specifies filename of the .xml file to save the exported application information to.

BCU-console list [/Q] [/U] [/V] - Display a list of installed applications.

Switches:
 /Q             - Use quiet uninstallers wherever possible (by default only use loud).
 /U             - Unattended mode (do not ask user for confirmation). WARNING: ONLY USE AFTER
                  THOROUGH TESTING. UNINSTALL LISTS SHOULD BE AS SPECIFIC AS POSSIBLE TO AVOID
                  FALSE POSITIVES. THERE ARE NO WARRANTIES, USE WITH CAUTION.
 /J=<Level>     - Attempt to clean up leftover ""junk"" (Registry entries and files/folders) after
                  uninstall. If no level is passed then defaults to ""VeryGood"". ***WARNING***: USE
                  EXTREME CAUTION WHEN CHOOSING ANY LEVEL BELOW VeryGood. THERE ARE NO WARRANTIES.
                  Valid levels are: VeryGood, Good, Questionable, Bad, Unknown
 /V             - Verbose logging mode (show more information about what is currently happening).

Return codes:
 0	- The operation completed successfully.
 1	- Invalid arguments.
 1223	- The operation was canceled by the user.");
        }

        private static int Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            try { Console.OutputEncoding = Encoding.Unicode; }
            catch (SystemException) { }

            var info = Assembly.GetExecutingAssembly();
            Console.WriteLine(info.FullName);
            Console.WriteLine();

            if (args.Length == 0 || args.Any(x =>
                    x.Equals("help", StringComparison.OrdinalIgnoreCase) ||
                    x.Equals("/?", StringComparison.OrdinalIgnoreCase)))
            {
                ShowHelp();
                Console.ReadKey();
                return 0;
            }

            try
            {
                switch (args[0].Normalize().ToLowerInvariant())
                {
                    case "uninstall":
                        return ProcessUninstallCommand(args.Skip(1).ToArray());
                    case "list":
                        return ProcessListCommand(args.Skip(1).ToArray());
                    case "export":
                        return ProcessExportCommand(args.Skip(1).ToArray());
                    default:
                        Console.WriteLine($"Invalid command \"{args[0]}\"\n");
                        ShowHelp();
                        return 1;
                }
            }
            catch (SystemException ex)
            {
                Console.WriteLine(@"Encountered an unexpected error!");
                Console.WriteLine(ex);
                return 13;
            }
        }

        private static int ProcessListCommand(string[] args)
        {
            var isVerbose = args.Any(x => x.Equals("/V", StringComparison.OrdinalIgnoreCase));
            var isQuiet = args.Any(x => x.Equals("/Q", StringComparison.OrdinalIgnoreCase));
            var isUnattended = args.Any(x => x.Equals("/U", StringComparison.OrdinalIgnoreCase));

            var serializer = new ApplicationEntrySerializer(QueryApps(isQuiet, isUnattended, isVerbose));

            Console.WriteLine($@"{"Display Name",-40}  {"Version",-20}  {"Source",-40}");
            var sb = new StringBuilder(82);
            for (var i = 0; i < 102; i++)
            {
                sb.Append('-');
            }
            Console.WriteLine(sb.ToString());

            foreach (var entry in serializer.Items)
            {
                var displayName = entry.DisplayNameTrimmed;
                if (displayName.Length > 40)
                {
                    displayName = displayName.Substring(0, 40);
                }

                var version = entry.DisplayVersion ?? string.Empty;
                if (version.Length > 20)
                {
                    version = version.Substring(0, 20);
                }

                var source = entry.AboutUrl ?? string.Empty;
                if (source.Length > 40)
                {
                    source = source.Substring(0, 40);
                }

                Console.WriteLine($@"{displayName,-40}  {version,-20}  {source,-40}");
            }
            return 0;
        }

        private static int ProcessExportCommand(string[] args)
        {
            var isVerbose = args.Any(x => x.Equals("/V", StringComparison.OrdinalIgnoreCase));
            var isQuiet = args.Any(x => x.Equals("/Q", StringComparison.OrdinalIgnoreCase));
            var isUnattended = args.Any(x => x.Equals("/U", StringComparison.OrdinalIgnoreCase));

            args = args.Where(x => !x.StartsWith("/", StringComparison.Ordinal)).ToArray();
            if (args.Length != 1)
                return ShowInvalidSyntaxError("Missing export filename or invalid arguments");

            Console.WriteLine($@"Starting export to {args[0]}");
            var apps = QueryApps(isQuiet, isUnattended, isVerbose);

            Console.WriteLine(@"Exporting data...");
            ApplicationEntrySerializer.SerializeApplicationEntries(args[0], apps);
            Console.WriteLine(@"Success!");
            return 0;
        }

        private static int ProcessUninstallCommand(string[] args)
        {
            if (args.Length < 1)
                return ShowInvalidSyntaxError("Missing path argument");

            if (!File.Exists(args[0]))
                return ShowInvalidSyntaxError("Invalid path or missing list file");

            UninstallList list;
            try
            {
                list = UninstallList.ReadFromFile(args[0]);
                if (list == null || list.Filters.Count == 0)
                    throw new IOException("List is empty");
            }
            catch (SystemException ex)
            {
                return ShowInvalidSyntaxError(
                    $"Invalid or damaged uninstall list file - \"{args[0]}\"\nError: {ex.Message}\n");
            }

            var isVerbose = args.Any(x => x.Equals("/V", StringComparison.OrdinalIgnoreCase));
            var isQuiet = args.Any(x => x.Equals("/Q", StringComparison.OrdinalIgnoreCase));
            var isUnattended = args.Any(x => x.Equals("/U", StringComparison.OrdinalIgnoreCase));

            int junkArgumentIndex = Array.FindIndex(args, a => a.Equals("/J", StringComparison.OrdinalIgnoreCase));
            string junkArg = args.Where(a => a.Equals("/J", StringComparison.OrdinalIgnoreCase) || a.StartsWith("/J=", StringComparison.OrdinalIgnoreCase)).FirstOrDefault() ?? string.Empty;
            ConfidenceLevel? junkConfidenceLevel = null;
            if(junkArg.IsNotEmpty()) {
                string[] junkSplit = junkArg.Split('=', 2);
                junkConfidenceLevel = ConfidenceLevel.VeryGood;
                if (junkSplit.Length == 2) {
                    if (!Enum.TryParse(junkSplit[1], out ConfidenceLevel parsedJunkConfidenceLevel)) {
                        Console.WriteLine($"An invalid junk confidence level was passed: {junkSplit[1]}");
                        ShowHelp();
                        return 1;
                    }
                    junkConfidenceLevel = parsedJunkConfidenceLevel;
                }
            }
            if (isUnattended)
                Console.WriteLine(@"WARNING: Running in unattended mode. To abort press Ctrl+C or close the window.");

            return RunUninstall(list, isQuiet, isUnattended, isVerbose, junkConfidenceLevel);
        }

        private static int RunUninstall(UninstallList list, bool isQuiet, bool isUnattended, bool isVerbose, ConfidenceLevel? junkConfidenceLevel = null)
        {
            Console.WriteLine(@"Starting bulk uninstall...");
            var apps = QueryApps(isQuiet, isUnattended, isVerbose);

            apps = apps.Where(a => list.TestEntry(a) == true).OrderBy(x => x.DisplayName).ToList();

            if (apps.Count == 0)
            {
                Console.WriteLine(@"No applications matched the supplied uninstall list.");
                return 0;
            }

            Console.WriteLine(@"{0} application(s) were matched by the list: {1}", apps.Count,
                          string.Join("; ", apps.Select(x => x.DisplayName)));

            Console.WriteLine(@"These applications will now be uninstalled PERMANENTLY.");

            if (!isUnattended)
            {
                Console.WriteLine(@"Do you want to continue? [Y]es/[N]o");
                if (Console.ReadKey(true).Key != ConsoleKey.Y)
                    return CancelledByUser();
            }

            Console.WriteLine(@"Setting-up for the uninstall task...");
            var targets = apps.Select(a => new BulkUninstallEntry(a, a.QuietUninstallPossible, UninstallStatus.Waiting))
                .ToList();
            var task = UninstallManager.CreateBulkUninstallTask(targets,
                new BulkUninstallConfiguration(false, isQuiet, false, true, true));
            var isDone = false;
            task.OnStatusChanged += (sender, args) =>
            {
                ClearCurrentConsoleLine();

                var running = task.AllUninstallersList.Count(x => x.IsRunning);
                var waiting = task.AllUninstallersList.Count(x => x.CurrentStatus == UninstallStatus.Waiting);
                var finished = task.AllUninstallersList.Count(x => x.Finished);
                var errors = task.AllUninstallersList.Count(x => x.CurrentStatus == UninstallStatus.Failed ||
                    x.CurrentStatus == UninstallStatus.Invalid);
                Console.Write("Running: {0}, Waiting: {1}, Finished: {2}, Failed: {3}",
                    running, waiting, finished, errors);

                if (task.Finished)
                {
                    isDone = true;
                    Console.WriteLine();
                    Console.WriteLine(@"Uninstall task Finished.");

                    foreach (var error in task.AllUninstallersList.Where(x =>
                        x.CurrentStatus != UninstallStatus.Completed && x.CurrentError != null))
                        Console.WriteLine($@"Error: {error.UninstallerEntry.DisplayName} - {error.CurrentError.Message}");
                }
            };
            task.Start();

            while (!isDone)
                Thread.Sleep(250);

            if (junkConfidenceLevel is not null) {
                Console.WriteLine($"Starting junk cleanup with a minimum confidence level of {junkConfidenceLevel}");
                List<IJunkResult> remainingJunk = JunkManager.FindJunk(apps, apps, _ => { })
                    .Where(j => j.Confidence.GetConfidence() >= junkConfidenceLevel)
                    .ToList();

                if (!remainingJunk.Any()) {
                    Console.WriteLine($"No remaining junk found for any target applications.");
                    return 0;
                }

                Console.WriteLine("The following junk items will be permanently deleted:");
                remainingJunk.ForEach(Console.WriteLine);
                if (!isUnattended) {
                    Console.WriteLine(@"Do you want to continue? [Y]es/[N]o");
                    if (Console.ReadKey(true).Key != ConsoleKey.Y)
                        return CancelledByUser();
                }

                foreach (ApplicationUninstallerEntry entry in apps) {
                    // ApplicationUninstallerEntry doesn't currently implement an equality operator so ToLongString() will do as an object "hash".
                    List<IJunkResult> appJunk = remainingJunk.Where(j => j.Application.ToLongString().Equals(entry.ToLongString())).ToList();
                    Console.WriteLine($"{entry.DisplayName} Junk - {appJunk.Count} Entries Found");
                    appJunk.ForEach(j => j.Delete());
                }
            }
            return 0;
        }

        public static void ClearCurrentConsoleLine()
        {
            var currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }

        private static int CancelledByUser()
        {
            Console.WriteLine(@"Operation cancelled by the user.");
            return 1223;
        }

        private static IList<ApplicationUninstallerEntry> QueryApps(bool isQuiet, bool isUnattended, bool isVerbose)
        {
            ConfigureUninstallTools();

            Console.WriteLine(@"Looking for applications...");
            string previousMain = null;

            IList<ApplicationUninstallerEntry> result;
            if (isQuiet || isUnattended)
            {
                result = ApplicationUninstallerFactory.GetUninstallerEntries(_ => { });
            }
            else
            {
                result = ApplicationUninstallerFactory.GetUninstallerEntries(report =>
                            {
                                if (previousMain != report.Message)
                                {
                                    previousMain = report.Message;
                                    Console.WriteLine(report.Message);
                                }
                                if (isVerbose)
                                {
                                    if (!string.IsNullOrEmpty(report.Inner?.Message))
                                    {
                                        Console.Write("-> ");
                                        Console.WriteLine(report.Inner.Message);
                                    }
                                }
                            });
            }

            Console.WriteLine("Found {0} applications.", result.Count);
            return result;
        }

        private static void ConfigureUninstallTools()
        {
            UninstallToolsGlobalConfig.ScanWinUpdates = false;
            UninstallToolsGlobalConfig.QuietAutomatizationKillStuck = true;
            UninstallToolsGlobalConfig.QuietAutomatization = true;
            UninstallToolsGlobalConfig.UseQuietUninstallDaemon = true;
            UninstallToolsGlobalConfig.AutoDetectCustomProgramFiles = true;
            UninstallToolsGlobalConfig.EnableAppInfoCache = false;
        }

        private static int ShowInvalidSyntaxError(string message)
        {
            Console.WriteLine("Invalid command syntax. " + message);
            return 87;
        }
    }
}
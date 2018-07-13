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
using UninstallTools.Lists;
using UninstallTools.Uninstaller;

namespace BCU_console
{
    internal class Program
    {
        private static void ShowHelp()
        {
            Console.WriteLine(@"BCU-console [help | /?] - Show help (this screen)

BCU-console uninstall path [/Q] [/U] [/V] - Uninstall applications
 
 path - Path to a .bcul file that contains information about applications to uninstall.
 /Q   - Use quiet uninstallers wherever possible (by default only use loud).
 /U   - Unattended mode (do not ask user for confirmation). WARNING: ONLY USE AFTER THOROUGH TESTING. 
        UNINSTALL LISTS SHOULD BE AS SPECIFIC AS POSSIBLE TO AVOID FALSE POSITIVES. THERE ARE NO 
        WARRANTIES, ONLY YOU ARE CONSIDERED RESPONSIBLE FOR THE RESULTS.
 /V   - Verbose logging mode (show more information about what is currently happening).");
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

                    default:
                        Console.WriteLine("Invalid command \"{0}\"\n", args[0]);
                        ShowHelp();
                        return 1;
                }
            }
            catch (SystemException ex)
            {
                Console.WriteLine("Encountered an unexpected error!");
                Console.WriteLine(ex);
                return 13;
            }
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

            if(isUnattended)
                Console.WriteLine("WARNING: Running in unattended mode. To abort press Ctrl+C or close the window.");

            return RunUninstall(list, isQuiet, isUnattended, isVerbose);
        }

        private static int RunUninstall(UninstallList list, bool isQuiet, bool isUnattended, bool isVerbose)
        {
            Console.WriteLine("Starting bulk uninstall...");
            var apps = QueryApps(isQuiet, isUnattended, isVerbose);

            apps = apps.Where(a => list.TestEntry(a) == true).OrderBy(x => x.DisplayName).ToList();

            if (apps.Count == 0)
            {
                Console.WriteLine("No applications matched the supplied uninstall list.");
                return 0;
            }

            Console.WriteLine("{0} application(s) were matched by the list: {1}", apps.Count,
                          string.Join("; ", apps.Select(x => x.DisplayName)));

            Console.WriteLine("These applications will now be uninstalled PERMANENTLY.");

            if (!isUnattended)
            {
                Console.WriteLine("Do you want to continue? [Y]es/[N]o");
                if (Console.ReadKey(true).Key != ConsoleKey.Y)
                    return CancelledByUser();
            }

            Console.WriteLine("Setting-up for the uninstall task...");
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
                    Console.WriteLine("Uninstall task Finished.");

                    foreach (var error in task.AllUninstallersList.Where(x =>
                        x.CurrentStatus != UninstallStatus.Completed && x.CurrentError != null))
                        Console.WriteLine("Error: {0} - {1}", error.UninstallerEntry.DisplayName,
                            error.CurrentError.Message);
                }
            };
            task.Start();

            while (!isDone)
                Thread.Sleep(250);

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
            Console.WriteLine("Operation cancelled by the user.");
            return 1223;
        }

        private static IList<ApplicationUninstallerEntry> QueryApps(bool isQuiet, bool isUnattended, bool isVerbose)
        {
            ConfigureUninstallTools();

            Console.WriteLine("Looking for applications...");
            string previousMain = null;
            var result = ApplicationUninstallerFactory.GetUninstallerEntries(report =>
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

            Console.WriteLine("Found {0} applications.", result.Count);
            return result;
        }

        private static void ConfigureUninstallTools()
        {
            UninstallToolsGlobalConfig.ScanChocolatey = true;
            UninstallToolsGlobalConfig.ScanDrives = true;
            UninstallToolsGlobalConfig.ScanPreDefined = true;
            UninstallToolsGlobalConfig.ScanRegistry = true;
            UninstallToolsGlobalConfig.ScanSteam = true;
            UninstallToolsGlobalConfig.ScanStoreApps = true;
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
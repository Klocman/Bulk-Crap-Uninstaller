/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace SteamHelper
{
    internal static class SteamUninstaller
    {
        public static void UninstallSteamApp(SteamApplicationInfo appInfo, bool silent)
        {
            Console.WriteLine("Uninstalling SteamApp with ID {0}{1}", appInfo.AppId, silent ? " silently" : string.Empty);

            if (silent)
                QuietUninstall(appInfo);
            else
                LoudUninstall(appInfo);
        }

        private static void LoudUninstall(SteamApplicationInfo appInfo)
        {
            Console.WriteLine("Running " + appInfo.UninstallString);
            var uninstallCommand = Misc.SeparateArgsFromCommand(appInfo.UninstallString);
            Process.Start(new ProcessStartInfo(uninstallCommand.Key, uninstallCommand.Value) { UseShellExecute = true });

            Console.WriteLine();
            Console.WriteLine("To stop waiting for Steam and cancel the operation press any key.");

            while (File.Exists(appInfo.ManifestPath))
            {
                Thread.Sleep(400);
                if (Console.KeyAvailable)
                    throw new OperationCanceledException();
            }
        }

        private static void QuietUninstall(SteamApplicationInfo appInfo)
        {
            //var steamPath = Misc.SeparateArgsFromCommand(appInfo.UninstallString).Key;
            //var processes = Process.GetProcesses().Where(x => x.MainModule.FileName.Equals(steamPath)).ToList();
            var processes = Process.GetProcessesByName("Steam");
            if (processes.Any())
            {
                Console.WriteLine("Killing Steam processes");
                foreach (var process in processes)
                {
                    process.Kill();
                    process.Dispose();
                }
            }

            if (Directory.Exists(appInfo.DownloadDirectory))
            {
                Console.WriteLine("Deleting " + appInfo.DownloadDirectory);
                Directory.Delete(appInfo.DownloadDirectory, true);
            }

            if (Directory.Exists(appInfo.WorkshopDirectory))
            {
                Console.WriteLine("Deleting " + appInfo.WorkshopDirectory);
                Directory.Delete(appInfo.WorkshopDirectory, true);
            }

            if (File.Exists(appInfo.WorkshopManifestPath))
            {
                Console.WriteLine("Deleting " + appInfo.WorkshopManifestPath);
                File.Delete(appInfo.WorkshopManifestPath);
            }

            if (Directory.Exists(appInfo.InstallDirectory))
            {
                Console.WriteLine("Deleting " + appInfo.InstallDirectory);
                Directory.Delete(appInfo.InstallDirectory, true);
            }

            if (File.Exists(appInfo.ManifestPath))
            {
                Console.WriteLine("Deleting " + appInfo.ManifestPath);
                File.Delete(appInfo.ManifestPath);
            }
        }
    }
}

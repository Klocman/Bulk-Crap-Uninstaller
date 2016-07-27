using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace SteamHelper
{
    internal class SteamUninstaller
    {
        public static void UninstallSteamApp(AppIdInfo appInfo, bool silent)
        {
            Console.WriteLine("Uninstalling SteapApp with ID {0}{1}", appInfo.AppId, silent ? " silently" : string.Empty);

            if (silent)
                QuietUninstall(appInfo);
            else
                LoudUninstall(appInfo);
        }

        private static void LoudUninstall(AppIdInfo appInfo)
        {
            Console.WriteLine("Running " + appInfo.UninstallString);
            Process.Start(appInfo.UninstallString);

            Console.WriteLine();
            Console.WriteLine("Press any key to skip waiting for Steam to uninstall the application...");

            while (File.Exists(appInfo.ManifestPath))
            {
                Thread.Sleep(400);
                if (Console.KeyAvailable)
                    break;
            }
        }

        private static void QuietUninstall(AppIdInfo appInfo)
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
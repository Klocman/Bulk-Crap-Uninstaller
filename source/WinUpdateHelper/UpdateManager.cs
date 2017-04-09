/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using WUApiLib;

namespace WinUpdateHelper
{
    internal class UpdateManager
    {
        public static void UninstallUpdate(string updateId)
        {
            Console.WriteLine("Scanning updates...");
            var wuaSession = new UpdateSessionClass();
            var wuaSearcher = wuaSession.CreateUpdateSearcher();
            var wuaSearch =
                wuaSearcher.Search($"Type='Software' and IsInstalled=1 and UpdateID='{updateId}' and IsPresent=1");
            var updates = wuaSearch.Updates.OfType<IUpdate>().ToList();
            if (!updates.Any())
                throw new ArgumentException("Selected update was not found");
            var uninstallable = updates.Where(x => x.IsUninstallable).ToList();
            if (!uninstallable.Any())
                throw new ArgumentException("Selected update is not uninstallable");

            var wuaInstaller = wuaSession.CreateUpdateInstaller();
            wuaInstaller.Updates = new UpdateCollectionClass();
            foreach (var update in uninstallable)
                wuaInstaller.Updates.Add(update);

            Console.WriteLine("Uninstalling " + string.Join("; ", uninstallable.Select(x => x.Title)) + "...");
            WaitForInstallerBusy(wuaInstaller);
            var result = wuaInstaller.Uninstall();
            WaitForInstallerBusy(wuaInstaller);

            switch (result.ResultCode)
            {
                case OperationResultCode.orcNotStarted:
                    throw new ArgumentException("Selected update is not uninstallable");
                case OperationResultCode.orcInProgress:
                    break;
                case OperationResultCode.orcSucceeded:
                    break;
                case OperationResultCode.orcSucceededWithErrors:
                    break;
                case OperationResultCode.orcFailed:
                    throw new COMException("Selected update is not uninstallable", result.HResult);
                case OperationResultCode.orcAborted:
                    throw new OperationCanceledException("Selected update is not uninstallable");
            }
            Console.WriteLine("Uninstall successful");
        }

        private static void WaitForInstallerBusy(IUpdateInstaller wuaInstaller)
        {
            var count = 0;
            // Wait for some seconds
            while (wuaInstaller.IsBusy && count++ < 30) Thread.Sleep(250);
            if (count >= 20)
                throw new TimeoutException("Update installer is busy");
        }

        public static void WriteUpdateList()
        {
            var wuaSession = new UpdateSessionClass();
            var wuaSearcher = wuaSession.CreateUpdateSearcher();
            var wuaSearch = wuaSearcher.Search("IsInstalled=1 and IsPresent=1 and Type='Software'");
            var updates = wuaSearch.Updates.OfType<IUpdate>().ToList();

            var first = true;
            foreach (var update in updates)
            {
                if (!first)
                    Console.WriteLine();
                first = false;

                var id = update.Identity;
                Console.WriteLine(nameof(id.UpdateID) + " - " + id.UpdateID);
                Console.WriteLine(nameof(id.RevisionNumber) + " - " +
                                  id.RevisionNumber.ToString(CultureInfo.InvariantCulture));

                Console.WriteLine(nameof(update.Title) + " - " + update.Title);
                Console.WriteLine(nameof(update.IsUninstallable) + " - " +
                                  update.IsUninstallable.ToString(CultureInfo.InvariantCulture));

                Console.WriteLine(nameof(update.SupportUrl) + " - " + update.SupportUrl);

                Console.WriteLine(nameof(update.MinDownloadSize) + " - " +
                                  update.MinDownloadSize.ToString(CultureInfo.InvariantCulture));
                Console.WriteLine(nameof(update.MaxDownloadSize) + " - " +
                                  update.MaxDownloadSize.ToString(CultureInfo.InvariantCulture));
                Console.WriteLine(nameof(update.LastDeploymentChangeTime) + " - " +
                                  update.LastDeploymentChangeTime.ToString(CultureInfo.InvariantCulture));
            }
        }
    }
}
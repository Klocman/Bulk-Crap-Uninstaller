/*
    EBUninstaller Pro - Windows Update Helper UpdateManager
    Discovery and uninstallation management for Windows Updates.
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Klocman;

namespace WinUpdateHelper
{
    internal static class UpdateManager
    {
        private static IUpdateSession CreateUpdateSession()
        {
            try
            {
                var type = Type.GetTypeFromProgID("Microsoft.Update.Session") ??
                           Type.GetTypeFromCLSID(new Guid("4CB43D7F-7EEE-4906-8698-60DA1C38F2FE"));
                if (type != null)
                {
                    var instance = Activator.CreateInstance(type);
                    if (instance != null)
                        return (IUpdateSession)instance;
                }
            }
            catch
            {
                // Fallback to direct COM CoClass instantiation
            }

            return (IUpdateSession)new UpdateSession();
        }

        private static IUpdateCollection CreateUpdateCollection()
        {
            try
            {
                var type = Type.GetTypeFromProgID("Microsoft.Update.UpdateColl") ??
                           Type.GetTypeFromCLSID(new Guid("2EE48F22-AF3C-405E-B397-CD067BF1DB89"));
                if (type != null)
                {
                    var instance = Activator.CreateInstance(type);
                    if (instance != null)
                        return (IUpdateCollection)instance;
                }
            }
            catch
            {
                // Fallback to direct COM CoClass instantiation
            }

            return (IUpdateCollection)new UpdateCollection();
        }

        public static void UninstallUpdate(string updateId)
        {
            Console.WriteLine("Scanning updates...");
            IUpdateSession wuaSession = CreateUpdateSession();
            IUpdateSearcher wuaSearcher = wuaSession.CreateUpdateSearcher();
            var wuaSearch =
                wuaSearcher.Search($"Type='Software' and IsInstalled=1 and UpdateID='{updateId}' and IsPresent=1");
            var updates = wuaSearch.Updates.OfType<IUpdate>().ToList();
            if (!updates.Any())
                throw new ArgumentException("Selected update was not found");
            var uninstallable = updates.Where(x => x.IsUninstallable).ToList();
            if (!uninstallable.Any())
                throw new ArgumentException("Selected update is not uninstallable");

            IUpdateInstaller wuaInstaller = wuaSession.CreateUpdateInstaller();
            IUpdateCollection updateCollection = CreateUpdateCollection();
            foreach (var update in uninstallable)
                updateCollection.Add(update);
            wuaInstaller.Updates = updateCollection;

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
            IUpdateSession wuaSession = CreateUpdateSession();
            IUpdateSearcher wuaSearcher = wuaSession.CreateUpdateSearcher();
            var wuaSearch = wuaSearcher.Search("IsInstalled=1 and IsPresent=1 and Type='Software'");
            var updates = wuaSearch.Updates.OfType<IUpdate>().ToList();
            
            foreach (var update in updates)
            {
                var id = update.Identity;

                var result = HelperTools.KeyValueListToConsoleOutput(new List<KeyValuePair<string, object>>
                {
                    new(nameof(id.UpdateID), id.UpdateID),
                    new(nameof(id.RevisionNumber), id.RevisionNumber),

                    new(nameof(update.Title), update.Title),
                    new(nameof(update.IsUninstallable), update.IsUninstallable),

                    new(nameof(update.SupportUrl), update.SupportUrl),

                    new(nameof(update.MinDownloadSize), update.MinDownloadSize),
                    new(nameof(update.MaxDownloadSize), update.MaxDownloadSize),
                    new(nameof(update.LastDeploymentChangeTime), update.LastDeploymentChangeTime)
                });

                Console.WriteLine(result);
            }
        }
    }
}

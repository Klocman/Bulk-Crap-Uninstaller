/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
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
        public static void UninstallUpdate(string updateId)
        {
            Console.WriteLine("Scanning updates...");
            var wuaSession = (NativeMethods.IUpdateSession)new NativeMethods.UpdateSession();
            var wuaSearcher = wuaSession.CreateUpdateSearcher();
            var wuaSearch = wuaSearcher.Search($"Type='Software' and IsInstalled=1 and UpdateID='{updateId}' and IsPresent=1");

            var updates = wuaSearch.Updates.Cast<NativeMethods.IUpdate>().ToList();

            if (!updates.Any())
                throw new ArgumentException("Selected update was not found");
            
            var uninstallable = updates.Where(x => x.IsUninstallable).ToList();
            if (!uninstallable.Any())
                throw new ArgumentException("Selected update is not uninstallable");

            var wuaInstaller = wuaSession.CreateUpdateInstaller();
            
            // Create UpdateCollection
            var updateCollection = new NativeMethods.UpdateCollection();
            
            wuaInstaller.Updates = updateCollection;
            foreach (var update in uninstallable)
                wuaInstaller.Updates.Add(update);

            Console.WriteLine("Uninstalling " + string.Join("; ", uninstallable.Select(x => x.Title)) + "...");
            WaitForInstallerBusy(wuaInstaller);
            var result = wuaInstaller.Uninstall();
            WaitForInstallerBusy(wuaInstaller);

            switch (result.ResultCode)
            {
                case NativeMethods.OperationResultCode.orcNotStarted:
                    throw new ArgumentException("Selected update is not uninstallable");
                case NativeMethods.OperationResultCode.orcInProgress:
                    break;
                case NativeMethods.OperationResultCode.orcSucceeded:
                    break;
                case NativeMethods.OperationResultCode.orcSucceededWithErrors:
                    break;
                case NativeMethods.OperationResultCode.orcFailed:
                    throw new COMException("Selected update is not uninstallable", result.HResult);
                case NativeMethods.OperationResultCode.orcAborted:
                    throw new OperationCanceledException("Selected update is not uninstallable");
            }
            Console.WriteLine("Uninstall successful");
        }

        private static void WaitForInstallerBusy(NativeMethods.IUpdateInstaller wuaInstaller)
        {
            var count = 0;
            // Wait for some seconds
            while (wuaInstaller.IsBusy && count++ < 30) Thread.Sleep(250);
            if (count >= 20)
                throw new TimeoutException("Update installer is busy");
        }

        public static void WriteUpdateList()
        {
            var wuaSession = (NativeMethods.IUpdateSession)new NativeMethods.UpdateSession();
            var wuaSearcher = wuaSession.CreateUpdateSearcher();
            var wuaSearch = wuaSearcher.Search("IsInstalled=1 and IsPresent=1 and Type='Software'");
            
            var updates = wuaSearch.Updates.Cast<NativeMethods.IUpdate>().ToList();
            
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

        private static class NativeMethods
        {
            [ComImport]
            [Guid("4CB43D7F-7EEE-4906-8698-60DA1C38F2FE")]
            internal class UpdateSession { }

            [ComImport]
            [Guid("13639463-00DB-4646-803D-528026140D88")]
            internal class UpdateCollection : IUpdateCollection
            {
                [DispId(0)]
                public virtual extern IUpdate this[int index] { get; set; }
                [DispId(1610743809)]
                public virtual extern int Count { get; }
                [DispId(1610743810)]
                public virtual extern bool ReadOnly { get; }
                [DispId(-4)]
                public virtual extern System.Collections.IEnumerator GetEnumerator();
                [DispId(1610743811)]
                public virtual extern int Add(IUpdate value);
                [DispId(1610743812)]
                public virtual extern void Clear();
                [DispId(1610743813)]
                public virtual extern object Copy();
                [DispId(1610743814)]
                public virtual extern void Insert(int index, IUpdate value);
                [DispId(1610743815)]
                public virtual extern void RemoveAt(int index);
            }

            [ComImport]
            [Guid("816858A4-260D-4260-933A-2585F1ABC76B")]
            [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
            internal interface IUpdateSession
            {
                [DispId(1610743809)]
                string ClientApplicationID { get; set; }
                [DispId(1610743810)]
                bool ReadOnly { get; }
                [DispId(1610743811)]
                object WebProxy { get; set; }
                [DispId(1610743812)]
                IUpdateSearcher CreateUpdateSearcher();
                [DispId(1610743813)]
                object CreateUpdateDownloader();
                [DispId(1610743814)]
                IUpdateInstaller CreateUpdateInstaller();
            }

            [ComImport]
            [Guid("8F45ABF1-F9AE-4B95-A933-F0F66E5056EA")]
            [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
            internal interface IUpdateSearcher
            {
                [DispId(1610743809)]
                bool CanAutomaticallyUpgradeService { get; set; }
                [DispId(1610743811)]
                string ClientApplicationID { get; set; }
                [DispId(1610743812)]
                bool IncludePotentiallySupersededUpdates { get; set; }
                [DispId(1610743815)]
                object ServerSelection { get; set; }
                [DispId(1610743821)]
                bool Online { get; set; }
                [DispId(1610743823)]
                string ServiceID { get; set; }
                [DispId(1610743816)]
                object BeginSearch(string criteria, object onCompleted, object state);
                [DispId(1610743817)]
                ISearchResult EndSearch(object searchJob);
                [DispId(1610743818)]
                string EscapeString(string unescaped);
                [DispId(1610743819)]
                object QueryHistory(int startIndex, int Count);
                [DispId(1610743820)]
                ISearchResult Search(string criteria);
                [DispId(1610743822)]
                int GetTotalHistoryCount();
            }

            [ComImport]
            [Guid("D40CFF62-E08C-4498-941A-01E25F0FD33C")]
            [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
            internal interface ISearchResult
            {
                [DispId(1610743809)]
                OperationResultCode ResultCode { get; }
                [DispId(1610743810)]
                object RootCategories { get; }
                [DispId(1610743811)]
                IUpdateCollection Updates { get; }
                [DispId(1610743812)]
                object Warnings { get; }
            }

            [ComImport]
            [Guid("07F7438C-7709-4CA5-B518-91279288134E")]
            [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
            internal interface IUpdateCollection : System.Collections.IEnumerable
            {
                [DispId(0)]
                IUpdate this[int index] { get; set; }
                [DispId(1610743809)]
                int Count { get; }
                [DispId(1610743810)]
                bool ReadOnly { get; }
                [DispId(-4)]
                new System.Collections.IEnumerator GetEnumerator();
                [DispId(1610743811)]
                int Add(IUpdate value);
                [DispId(1610743812)]
                void Clear();
                [DispId(1610743813)]
                object Copy();
                [DispId(1610743814)]
                void Insert(int index, IUpdate value);
                [DispId(1610743815)]
                void RemoveAt(int index);
            }

            [ComImport]
            [Guid("6A92B07A-D821-4682-B423-5C805022CC4D")]
            [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
            internal interface IUpdate
            {
                [DispId(0)]
                string Title { get; }
                [DispId(1610743809)]
                bool AutoSelectOnWebSites { get; }
                [DispId(1610743810)]
                object BundledUpdates { get; }
                [DispId(1610743811)]
                bool CanRequireSource { get; }
                [DispId(1610743812)]
                object Categories { get; }
                [DispId(1610743813)]
                object Deadline { get; }
                [DispId(1610743814)]
                bool DeltaCompressedContentAvailable { get; }
                [DispId(1610743815)]
                bool DeltaCompressedContentPreferred { get; }
                [DispId(1610743816)]
                string Description { get; }
                [DispId(1610743817)]
                bool EulaAccepted { get; }
                [DispId(1610743818)]
                string EulaText { get; }
                [DispId(1610743819)]
                string HandlerID { get; }
                [DispId(1610743820)]
                IUpdateIdentity Identity { get; }
                [DispId(1610743821)]
                object Image { get; }
                [DispId(1610743822)]
                object InstallationBehavior { get; }
                [DispId(1610743823)]
                bool IsBeta { get; }
                [DispId(1610743824)]
                bool IsDownloaded { get; }
                [DispId(1610743825)]
                bool IsHidden { get; set; }
                [DispId(1610743826)]
                bool IsInstalled { get; }
                [DispId(1610743827)]
                bool IsMandatory { get; }
                [DispId(1610743828)]
                bool IsUninstallable { get; }
                [DispId(1610743829)]
                object Languages { get; }
                [DispId(1610743830)]
                DateTime LastDeploymentChangeTime { get; }
                [DispId(1610743831)]
                decimal MaxDownloadSize { get; }
                [DispId(1610743832)]
                decimal MinDownloadSize { get; }
                [DispId(1610743833)]
                object MoreInfoUrls { get; }
                [DispId(1610743834)]
                string MsrcSeverity { get; }
                [DispId(1610743835)]
                int RecommendedCpuSpeed { get; }
                [DispId(1610743836)]
                int RecommendedHardDiskSpace { get; }
                [DispId(1610743837)]
                int RecommendedMemory { get; }
                [DispId(1610743838)]
                string ReleaseNotes { get; }
                [DispId(1610743839)]
                object SecurityBulletinIDs { get; }
                [DispId(1610743841)]
                object SupersededUpdateIDs { get; }
                [DispId(1610743842)]
                string SupportUrl { get; }
                [DispId(1610743843)]
                object Type { get; }
                [DispId(1610743844)]
                string UninstallationNotes { get; }
                [DispId(1610743845)]
                object UninstallationBehavior { get; }
                [DispId(1610743846)]
                object UninstallationSteps { get; }
                [DispId(1610743848)]
                object KBArticleIDs { get; }
                [DispId(1610743849)]
                object DeploymentAction { get; }
                [DispId(1610743851)]
                object DownloadPriority { get; }
                [DispId(1610743852)]
                object DownloadContents { get; }
                [DispId(1610743847)]
                void AcceptEula();
                [DispId(1610743850)]
                void CopyFromCache(string path, bool toExtractCabFiles);
            }

            [ComImport]
            [Guid("46297823-9940-4C09-AED9-CD3EA6D05968")]
            [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
            internal interface IUpdateIdentity
            {
                [DispId(1610743810)]
                int RevisionNumber { get; }
                [DispId(1610743811)]
                string UpdateID { get; }
            }

            [ComImport]
            [Guid("7B929C68-CCDC-4226-96B1-8724600B54C2")]
            [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
            internal interface IUpdateInstaller
            {
                [DispId(1610743809)]
                string ClientApplicationID { get; set; }
                [DispId(1610743810)]
                bool IsForced { get; set; }
                [DispId(1610743811)]
                IntPtr ParentHwnd { get; set; }
                [DispId(1610743812)]
                object parentWindow { get; set; }
                [DispId(1610743813)]
                IUpdateCollection Updates { get; set; }
                [DispId(1610743820)]
                bool IsBusy { get; }
                [DispId(1610743822)]
                bool AllowSourcePrompts { get; set; }
                [DispId(1610743823)]
                bool RebootRequiredBeforeInstallation { get; }
                [DispId(1610743814)]
                object BeginInstall(object onProgressChanged, object onCompleted, object state);
                [DispId(1610743815)]
                object BeginUninstall(object onProgressChanged, object onCompleted, object state);
                [DispId(1610743816)]
                IInstallationResult EndInstall(object value);
                [DispId(1610743817)]
                IInstallationResult EndUninstall(object value);
                [DispId(1610743818)]
                IInstallationResult Install();
                [DispId(1610743819)]
                IInstallationResult RunWizard(string dialogTitle);
                [DispId(1610743821)]
                IInstallationResult Uninstall();
            }

            [ComImport]
            [Guid("A43C56D6-7451-48D4-AF96-B6CD2D0D9B7A")]
            [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
            internal interface IInstallationResult
            {
                [DispId(1610743809)]
                int HResult { get; }
                [DispId(1610743810)]
                bool RebootRequired { get; }
                [DispId(1610743811)]
                OperationResultCode ResultCode { get; }
                [DispId(1610743812)]
                object GetUpdateResult(int updateIndex);
            }

            internal enum OperationResultCode
            {
                orcNotStarted = 0,
                orcInProgress = 1,
                orcSucceeded = 2,
                orcSucceededWithErrors = 3,
                orcFailed = 4,
                orcAborted = 5
            }
        }
    }
}


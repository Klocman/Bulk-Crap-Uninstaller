using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using WUApiLib;

namespace WinUpdateHelper
{
    /// <summary>
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/aa386065(v=vs.85).aspx
    /// Return codes:
    /// 0 - The operation completed successfully.
    /// 59 - An unexpected network error occurred.
    /// 1223 - The operation was canceled by the user.
    /// 
    /// Commands
    /// u[ninstall] UpdateID     - Uninstall an update
    /// l[ist]                   - List updates
    /// </summary>       
    class Program
    {
        private static QueryType _queryType;

        static int Main(string[] args)
        {
            try
            {
                Console.OutputEncoding = Encoding.Unicode;

                ProcessCommandlineArguments(args);

                switch (_queryType)
                {
                    case QueryType.Uninstall:
                        UninstallUpdate(_updateId);
                        break;

                    case QueryType.List:
                        WriteUpdateList();
                        break;
                }
            }
            catch (OperationCanceledException)
            {
                return 1223;
            }
            catch (COMException ex)
            {
                Console.WriteLine("Error: {0}", Hresult.ConvertHresultToDetails(ex.ErrorCode));
                return 59;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}", ex.Message);
                return 59;
            }
            return 0;
        }

        private static void UninstallUpdate(string updateId)
        {
            var wuaSession = new UpdateSessionClass();
            var wuaSearcher = wuaSession.CreateUpdateSearcher();
            var wuaSearch = wuaSearcher.Search($"Type='Software' and IsInstalled=1 and UpdateID='{updateId}' and IsPresent=1");
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
        }

        private static void WaitForInstallerBusy(IUpdateInstaller wuaInstaller)
        {
            int count = 0;
            // Wait for some seconds
            while (wuaInstaller.IsBusy && count++ < 30) Thread.Sleep(250);
            if (count >= 20)
                throw new TimeoutException("Update installer is busy");
        }

        private static void WriteUpdateList()
        {
            var wuaSession = new UpdateSessionClass();
            var wuaSearcher = wuaSession.CreateUpdateSearcher();
            var wuaSearch = wuaSearcher.Search("IsInstalled=1 and IsPresent=1 and Type='Software'");
            var updates = wuaSearch.Updates.OfType<IUpdate>().ToList();

            foreach (var update in updates)
            {
                var id = update.Identity;
                Console.WriteLine(nameof(id.UpdateID) + " - " + id.UpdateID);
                Console.WriteLine(nameof(id.RevisionNumber) + " - " + id.RevisionNumber.ToString(CultureInfo.InvariantCulture));
                Console.WriteLine(nameof(update.Title) + " - " + update.Title);

                Console.WriteLine(nameof(update.IsUninstallable) + " - " + update.IsUninstallable.ToString(CultureInfo.InvariantCulture));
                Console.WriteLine(nameof(update.SupportUrl) + " - " + update.SupportUrl);
                //Console.WriteLine(nameof(update.Title) + " - " + update.Title);
                Console.WriteLine(nameof(update.MinDownloadSize) + " - " + update.MinDownloadSize.ToString(CultureInfo.InvariantCulture));
                Console.WriteLine(nameof(update.MaxDownloadSize) + " - " + update.MaxDownloadSize.ToString(CultureInfo.InvariantCulture));
                Console.WriteLine(nameof(update.LastDeploymentChangeTime) + " - " + update.LastDeploymentChangeTime.ToString(CultureInfo.InvariantCulture));
                Console.WriteLine();
            }
        }

        private static string _updateId;

        private static void ProcessCommandlineArguments(IEnumerable<string> args)
        {
            foreach (var arg in args)
            {
                switch (arg.ToLowerInvariant())
                {
                    case @"u":
                    case @"uninstall":
                        if (_queryType != QueryType.None) throw new ArgumentException(@"Multiple commands specified");
                        _queryType = QueryType.Uninstall;
                        break;

                    case @"l":
                    case @"list":
                        if (_queryType != QueryType.None) throw new ArgumentException(@"Multiple commands specified");
                        _queryType = QueryType.List;
                        break;

                    default:
                        if (_queryType != QueryType.Uninstall)
                            throw new ArgumentException($@"Unknown argument: {arg}");
                        if (_updateId != null)
                            throw new ArgumentException(@"Multiple UpdateIDs specified");
                        _updateId = arg;
                        break;
                }
            }

            if (_queryType == QueryType.None)
                throw new ArgumentException(@"No commands specified");

            if (_queryType == QueryType.Uninstall && _updateId == null)
                throw new ArgumentException(@"No UpdateID specified");
        }

        private enum QueryType
        {
            None,
            Uninstall,
            List,
        }
    }
}

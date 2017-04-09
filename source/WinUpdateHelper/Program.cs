/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace WinUpdateHelper
{
    /// <summary>
    ///     https://msdn.microsoft.com/en-us/library/windows/desktop/aa386065(v=vs.85).aspx
    ///     Return codes:
    ///     0 - The operation completed successfully.
    ///     59 - An unexpected network error occurred.
    ///     1223 - The operation was canceled by the user.
    ///     Commands
    ///     u[ninstall] UpdateID     - Uninstall an update
    ///     l[ist]                   - List updates
    /// </summary>
    internal class Program
    {
        private static QueryType _queryType;

        private static string _updateId;

        private static int Main(string[] args)
        {
            try
            {
                Console.OutputEncoding = Encoding.Unicode;

                ProcessCommandlineArguments(args);

                switch (_queryType)
                {
                    case QueryType.Uninstall:
                        UpdateManager.UninstallUpdate(_updateId);
                        break;

                    case QueryType.List:
                        UpdateManager.WriteUpdateList();
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
            List
        }
    }
}
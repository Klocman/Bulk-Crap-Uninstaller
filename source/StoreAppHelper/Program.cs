/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace StoreAppHelper
{
    internal static class Program
    {
        private const int InvalidArgumentCode = 10022;
        private const int FunctionFailedCode = 1627;
        private const int OkCode = 0;

        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static int Main(string[] args)
        {
            if (args.Length == 0)
                return InvalidArgumentCode;
            
            try { Console.OutputEncoding = Encoding.Unicode; }
            catch(IOException) { /*Old .NET v4 without support for unicode output*/ }

            if (args.Length == 1 && string.Equals(args[0], @"/query", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    var result = AppManager.QueryApps();
                    foreach (var app in result)
                    {
                        Console.WriteLine(@"FullName: " + app.FullName);
                        Console.WriteLine(@"DisplayName: " + app.DisplayName);
                        Console.WriteLine(@"PublisherDisplayName: " + app.PublisherDisplayName);
                        Console.WriteLine(@"Logo: " + app.Logo);
                        Console.WriteLine(@"InstalledLocation: " + app.InstalledLocation);
                        Console.WriteLine(@"IsProtected: " + app.IsProtected.ToString(CultureInfo.InvariantCulture));
                        Console.WriteLine();
                    }
                    return OkCode;
                }
                catch (IOException ex)
                {
                    LogWriter.WriteExceptionToLog(ex);
                    return HandleHrefMessage(ex);
                }
                catch (Exception ex)
                {
                    LogWriter.WriteExceptionToLog(ex);
                    return FunctionFailedCode;
                }
            }

            if (args.Length == 2 && string.Equals(args[0], @"/uninstall", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    AppManager.UninstallApp(args[1]);
                    return OkCode;
                }
                catch (IOException ex)
                {
                    return HandleHrefMessage(ex);
                }
                catch (Exception ex)
                {
                    LogWriter.WriteExceptionToLog(ex);
                    return FunctionFailedCode;
                }
            }

            return InvalidArgumentCode;
        }

        private static int HandleHrefMessage(Exception ex)
        {
            var errorCode = Regex.Match(ex.Message, @"0x[\d\w]{8}")
                .Captures.Cast<Capture>().FirstOrDefault()?.Value;

            if (string.IsNullOrWhiteSpace(errorCode) || errorCode.Length < 8)
                return FunctionFailedCode;

            int.TryParse(errorCode.Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture,
                out var errorNumber);

            return errorNumber > 0 ? errorNumber : FunctionFailedCode;
        }
    }
}
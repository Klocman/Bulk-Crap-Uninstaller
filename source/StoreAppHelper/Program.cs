/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.IO;
using Klocman;

namespace StoreAppHelper
{
    internal static class Program
    {
        [STAThread]
        private static int Main(string[] args)
        {
            if (args.Length == 0)
                return (int)ReturnValue.InvalidArgumentCode;

            HelperTools.SetupEncoding();

            if (args.Length == 1 && string.Equals(args[0], @"/query", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    var result = AppManager.QueryApps();
                    foreach (var app in result)
                        Console.WriteLine(HelperTools.ObjectToConsoleOutput(app));
                    return (int)ReturnValue.OkCode;
                }
                catch (IOException ex)
                {
                    LogWriter.WriteExceptionToLog(ex);
                    return HelperTools.HandleHrefMessage(ex);
                }
                catch (Exception ex)
                {
                    LogWriter.WriteExceptionToLog(ex);
                    return (int)ReturnValue.FunctionFailedCode;
                }
            }

            if (args.Length == 2 && string.Equals(args[0], @"/uninstall", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    AppManager.UninstallApp(args[1]);
                    return (int)ReturnValue.OkCode;
                }
                catch (IOException ex)
                {
                    return HelperTools.HandleHrefMessage(ex);
                }
                catch (Exception ex)
                {
                    LogWriter.WriteExceptionToLog(ex);
                    return (int)ReturnValue.FunctionFailedCode;
                }
            }

            return (int)ReturnValue.InvalidArgumentCode;
        }
    }
}
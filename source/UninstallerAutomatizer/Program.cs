/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.IO;
using System.Linq;
using System.Text;
using Klocman.Extensions;
using UninstallTools;

namespace UninstallerAutomatizer
{
    /// <summary>
    /// UninstallerAutomatizer.exe UninstallerType [/K] UninstallCommand
    /// </summary>
    internal class Program
    {
        private const int InvalidArgumentCode = 10022;
        private const int FunctionFailedCode = 1627;
        private const int OkCode = 0;

        private static bool _killOnFail;

        private static int Main(string[] args)
        {
            try { Console.OutputEncoding = Encoding.Unicode; }
            catch (IOException) { /*Old .NET v4 without support for unicode output*/ }

            if (args.Length < 2)
                return InvalidArgumentCode;

            UninstallerType uType;
            if (!Enum.TryParse(args[0], out uType))
                return InvalidArgumentCode;

            args = args.Skip(1).ToArray();

            if (args[0].Equals("/k", StringComparison.InvariantCultureIgnoreCase))
            {
                args = args.Skip(1).ToArray();
                _killOnFail = true;
            }

            try
            {
                if (uType == UninstallerType.Nsis)
                {
                    var cline = string.Join(" ", args);
                    Console.WriteLine(@"Automatically uninstalling " + cline);
                    AutomatedUninstallManager.UninstallNsisQuietly(cline);
                    return OkCode;
                }
            }
            catch (AutomatedUninstallManager.AutomatedUninstallException ex)
            {
                Console.WriteLine(@"Automatic uninstallation failed");
                Console.WriteLine(@"Reason: " + ex.InnerException.Message);
                
                if(ex.UninstallerProcess != null && _killOnFail)
                {
                    try
                    {
                        ex.UninstallerProcess.Kill(true);
                    }
                    catch
                    {
                        // Ignore process errors, can't do anything about it
                    }
                }

                return FunctionFailedCode;
            }

            Console.WriteLine(uType + @" is not supported");
            return InvalidArgumentCode;
        }
    }
}
/*
    Copyright (c) 2018 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Klocman;

namespace ScriptHelper
{
    internal static class Program
    {
        [STAThread]
        private static int Main(string[] args)
        {
            if (args.Length != 0)
                return (int) ReturnValue.InvalidArgumentCode;

            HelperTools.SetupEncoding();

            try
            {
                foreach (var result in ScriptManager.GetScripts())
                {
                    var converted = result.Select(x => new KeyValuePair<string, object>(x.Key, x.Value)).ToList();
                    Console.WriteLine(HelperTools.KeyValueListToConsoleOutput(converted));
                }

                return (int) ReturnValue.OkCode;
            }
            catch (IOException ex)
            {
                LogWriter.WriteExceptionToLog(ex);
                return HelperTools.HandleHrefMessage(ex);
            }
            catch (Exception ex)
            {
                LogWriter.WriteExceptionToLog(ex);
                return (int) ReturnValue.FunctionFailedCode;
            }
        }
    }
}
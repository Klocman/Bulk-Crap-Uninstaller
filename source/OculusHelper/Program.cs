using System;
using System.IO;
using Klocman;

namespace OculusHelper
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            if (args.Length == 0)
                return HelperTools.InvalidArgumentCode;

            HelperTools.SetupEncoding();

            if (args.Length == 1 && string.Equals(args[0], @"/query", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    var result = OculusManager.QueryOculusApps();
                    foreach (var app in result)
                        Console.WriteLine(HelperTools.ObjectToConsoleOutput(app));
                    return HelperTools.OkCode;
                }
                catch (IOException ex)
                {
                    LogWriter.WriteExceptionToLog(ex);
                    return HelperTools.HandleHrefMessage(ex);
                }
                catch (Exception ex)
                {
                    LogWriter.WriteExceptionToLog(ex);
                    return HelperTools.FunctionFailedCode;
                }
            }

            return HelperTools.InvalidArgumentCode;
        }
    }
}

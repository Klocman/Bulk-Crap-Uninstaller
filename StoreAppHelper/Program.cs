using System;

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

            if (args.Length == 1 && string.Equals(args[1], @"/query", StringComparison.OrdinalIgnoreCase))
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
                    }
                    return OkCode;
                }
                catch
                {
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
                catch
                {
                    return FunctionFailedCode;
                }
            }

            return InvalidArgumentCode;
        }
    }
}
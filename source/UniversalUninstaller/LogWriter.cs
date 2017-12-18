/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.IO;
using System.Reflection;

namespace UniversalUninstaller
{
    internal static class LogWriter
    {
        public static void WriteMessageToLog(string message)
        {
            try
            {
                var location = Assembly.GetCallingAssembly().Location;
                location = location.Substring(0, location.Length - 3) + "log";

                using (var writer = new StreamWriter(location, true))
                {
                    writer.WriteLine("--- " + DateTime.UtcNow);
                    writer.WriteLine(message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"Failed to write to log file:\n" + ex);
            }
        }
    }
}
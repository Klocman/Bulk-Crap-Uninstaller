/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.IO;
using System.Reflection;

namespace Klocman
{
    public static class LogWriter
    {
        public static void WriteExceptionToLog(Exception ex)
        {
            var location = Assembly.GetCallingAssembly().Location;
            location = location.Substring(0, location.Length - 3) + "log";

            using (var writer = new StreamWriter(location, true))
            {
                writer.WriteLine("--- " + DateTime.UtcNow);
                writer.WriteLine(ex.ToString());
            }
        }
    }
}
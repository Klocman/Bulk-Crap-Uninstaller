/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace BulkCrapUninstaller
{
    internal sealed class LogWriter : StreamWriter
    {
        public LogWriter(string path) : base(path, true, Encoding.UTF8)
        {
        }

        public static LogWriter StartLogging()
        {
            try
            {
                var logPath = Path.Combine(Program.AssemblyLocation.FullName, "BCUninstaller.log");

                // Limit log size to 100 kb
                var fileInfo = new FileInfo(logPath);
                if (fileInfo.Length > 1024 * 100)
                    fileInfo.Delete();
                
                // Create new log writer
                var logWriter = new LogWriter(logPath);

                // Make sure we can write to the file
                logWriter.WriteLine("--------------------------------------------------");
                logWriter.WriteLine(DateTime.Now + " - BCU started");
                logWriter.Flush();

                Console.SetOut(logWriter);
                Console.SetError(logWriter);
#if DEBUG
                Debug.Listeners.Add(new ConsoleTraceListener(false));
#endif
                return logWriter;
            }
            catch (Exception ex)
            {
                // Ignore logging errors
                Console.WriteLine(ex);
                return null;
            }
        }
    }
}
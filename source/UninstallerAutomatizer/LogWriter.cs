/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace UninstallerAutomatizer
{
    internal sealed class LogWriter : StreamWriter
    {
        public LogWriter(string path) : base(path, true, Encoding.UTF8)
        {
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
                Disposed = true;
        }

        private bool Disposed { get; set; }

        public static LogWriter StartLogging(string logPath)
        {
            try
            {
                // Limit log size to 100 kb
                var fileInfo = new FileInfo(logPath);
                if (fileInfo.Exists && fileInfo.Length > 1024 * 100)
                    fileInfo.Delete();

                // Create new log writer
                var logWriter = new LogWriter(logPath);

                // Make sure we can write to the file
                logWriter.WriteSeparator();
                logWriter.WriteLine("Application startup");
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

        public void WriteSeparator()
        {
            if (Disposed) return;
            base.WriteLine("--------------------------------------------------");
        }

        public override void WriteLine(string value)
        {
            if (Disposed) return;
            value = DateTime.UtcNow.ToLongTimeString() + " - " + value;
            base.WriteLine(value);
        }
    }
}
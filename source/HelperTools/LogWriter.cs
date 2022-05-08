/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace Klocman
{
    public sealed class LogWriter : StreamWriter
    {
        public static void WriteExceptionToLog(Exception ex)
        {
            var location = CreateLogFilenameForAssembly(Assembly.GetCallingAssembly());

            using (var writer = new StreamWriter(location, true))
            {
                writer.Write(DateTime.UtcNow.ToLongTimeString());
                writer.Write(" - ");
                writer.WriteLine(ex.ToString());
            }
        }

        private static string CreateLogFilenameForAssembly(Assembly assembly)
        {
            var location = assembly.Location;
            if (location.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) || location.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                location = location.Remove(location.Length - 4);
            location += ".log";
            return location;
        }

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

        /// <summary>
        /// Start logging to a file reflecting the calling assembly name.
        /// Hooks console out and error. Dispose before exiting.
        /// </summary>
        public static LogWriter StartLogging()
        {
            var location = CreateLogFilenameForAssembly(Assembly.GetCallingAssembly());
            return StartLogging(location);
        }

        /// <summary>
        /// Start logging to a file.
        /// Hooks console out and error. Dispose before exiting.
        /// </summary>
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

                Trace.Listeners.Add(new TextWriterTraceListener(logWriter));

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
            base.Flush();
        }
    }
}
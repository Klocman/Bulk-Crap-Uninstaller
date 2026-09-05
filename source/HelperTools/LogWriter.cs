/*
 * EBUninstaller Pro - Application Uninstaller & System Optimization Suite
 * HelperTools Shared Utilities Subsystem
 * Copyright (C) 2026 EBUninstaller Development Team & Contributors
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace Klocman
{
    internal sealed class LogWriter : StreamWriter
    {
        private static readonly object SyncLock = new object();
        private static LogWriter _currentLogger;
        private readonly string _logFilePath;
        private readonly long _maxSizeBytes;

        public LogWriter(string path, long maxSizeBytes = 512 * 1024) : base(path, true, Encoding.UTF8)
        {
            _logFilePath = path;
            _maxSizeBytes = maxSizeBytes;
        }

        public static void WriteExceptionToLog(Exception ex)
        {
            if (ex == null) return;
            WriteMessageToLog("ERROR", ex.ToString());
        }

        public static void WriteMessageToLog(string message)
        {
            WriteMessageToLog("INFO", message);
        }

        public static void WriteMessageToLog(string level, string message)
        {
            if (string.IsNullOrEmpty(message)) return;

            lock (SyncLock)
            {
                var writer = _currentLogger;
                try
                {
                    if (writer == null || !writer.BaseStream.CanWrite)
                    {
                        var location = CreateLogFilenameForAssembly(Assembly.GetCallingAssembly());
                        writer = new LogWriter(location);
                    }

                    var tag = string.IsNullOrWhiteSpace(level) ? "INFO" : level.ToUpperInvariant();
                    writer.WriteLine($"[{tag}] {message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[LogWriter Failed] {ex.Message}");
                }
                finally
                {
                    if (writer != null && writer != _currentLogger)
                    {
                        writer.Dispose();
                    }
                }
            }
        }

        private static string CreateLogFilenameForAssembly(Assembly assembly)
        {
            var location = assembly?.Location;
            if (string.IsNullOrEmpty(location))
            {
                location = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Application.log");
            }
            else if (location.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) || location.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
            {
                location = location.Substring(0, location.Length - 4) + ".log";
            }
            return location;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                Disposed = true;
            }
        }

        public bool Disposed { get; private set; }

        public static LogWriter StartLogging()
        {
            lock (SyncLock)
            {
                _currentLogger?.Dispose();
                var location = CreateLogFilenameForAssembly(Assembly.GetCallingAssembly());
                return _currentLogger = StartLogging(location);
            }
        }

        private static LogWriter StartLogging(string logPath)
        {
            try
            {
                // Rotate log if size exceeds limit
                RotateLogIfNeeded(logPath, 512 * 1024);

                var logWriter = new LogWriter(logPath);

                logWriter.WriteSeparator();
                logWriter.WriteLine($"[STARTUP] Process: {Process.GetCurrentProcess().ProcessName} (PID {Environment.ProcessId}) - UTC: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff}");
                logWriter.Flush();

                Console.SetOut(logWriter);
                Console.SetError(logWriter);

                Trace.Listeners.Add(new TextWriterTraceListener(logWriter));

                return logWriter;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to start LogWriter: {ex}");
                return null;
            }
        }

        private static void RotateLogIfNeeded(string logPath, long maxBytes)
        {
            try
            {
                var fi = new FileInfo(logPath);
                if (fi.Exists && fi.Length > maxBytes)
                {
                    var backup = logPath + ".old";
                    if (File.Exists(backup)) File.Delete(backup);
                    File.Move(logPath, backup);
                }
            }
            catch { }
        }

        public void WriteSeparator()
        {
            if (Disposed) return;
            base.WriteLine("--------------------------------------------------------------------------------");
        }

        public override void WriteLine(string value)
        {
            if (Disposed) return;
            var timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff");
            base.WriteLine($"{timestamp} UTC - {value}");
            base.Flush();
        }
    }
}

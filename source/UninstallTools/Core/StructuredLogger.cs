/*
    OpenUninstall Pro - Open Source Professional Windows Uninstaller
    Core Structured Logging Subsystem
*/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace UninstallTools.Core
{
    public enum LogLevel
    {
        Trace = 0,
        Debug = 1,
        Info = 2,
        Warning = 3,
        Error = 4,
        Critical = 5
    }

    public enum LogCategory
    {
        General,
        Discovery,
        Uninstaller,
        ForcedRemoval,
        LeftoverScanner,
        InstallationMonitor,
        Backup,
        Registry,
        FileSystem,
        JunkCleaner,
        PrivacyCleaner,
        BrowserExtensions,
        StartupManager,
        Security,
        Cli
    }

    public sealed class LogEntry
    {
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public LogLevel Level { get; set; }
        public LogCategory Category { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Details { get; set; }
        public string Source { get; set; }

        public override string ToString()
        {
            var detailStr = string.IsNullOrWhiteSpace(Details) ? string.Empty : $" | Details: {Details}";
            return $"[{Timestamp:yyyy-MM-dd HH:mm:ss.fff}] [{Level.ToString().ToUpperInvariant()}] [{Category}] {Message}{detailStr}";
        }
    }

    /// <summary>
    /// Thread-safe high performance structured logging subsystem.
    /// Supports memory ring buffer, file output, event dispatch, redaction, and multi-format export.
    /// </summary>
    public static class StructuredLogger
    {
        private static readonly object _fileLock = new();
        private static readonly ConcurrentQueue<LogEntry> _logBuffer = new();
        private const int MaxMemoryEntries = 10000;
        private static string _logFilePath;
        private static LogLevel _minLogLevel = LogLevel.Info;
        private static readonly Regex SensitivePatterns = new(
            @"(password|secret|token|apikey|authorization|bearer)\s*[:=]\s*[^\s,;]+",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static event EventHandler<LogEntry> EntryLogged;

        public static LogLevel MinimumLevel
        {
            get => _minLogLevel;
            set => _minLogLevel = value;
        }

        public static string LogFilePath
        {
            get => _logFilePath;
            set
            {
                lock (_fileLock)
                {
                    _logFilePath = value;
                    if (!string.IsNullOrWhiteSpace(_logFilePath))
                    {
                        var dir = Path.GetDirectoryName(_logFilePath);
                        if (!string.IsNullOrWhiteSpace(dir) && !Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }
                    }
                }
            }
        }

        public static void Initialize(string logDir = null, LogLevel minLevel = LogLevel.Info)
        {
            _minLogLevel = minLevel;
            if (string.IsNullOrWhiteSpace(logDir))
            {
                try
                {
                    var baseDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "OpenUninstallPro", "Logs");
                    LogFilePath = Path.Combine(baseDir, $"OpenUninstall_{DateTime.UtcNow:yyyyMMdd}.log");
                }
                catch
                {
                    LogFilePath = null;
                }
            }
            else
            {
                LogFilePath = Path.Combine(logDir, $"OpenUninstall_{DateTime.UtcNow:yyyyMMdd}.log");
            }
        }

        public static void Log(LogLevel level, LogCategory category, string message, string details = null, string source = null)
        {
            if (level < _minLogLevel) return;

            var sanitizedMessage = Sanitize(message);
            var sanitizedDetails = Sanitize(details);

            var entry = new LogEntry
            {
                Timestamp = DateTime.UtcNow,
                Level = level,
                Category = category,
                Message = sanitizedMessage ?? string.Empty,
                Details = sanitizedDetails,
                Source = source
            };

            _logBuffer.Enqueue(entry);
            while (_logBuffer.Count > MaxMemoryEntries && _logBuffer.TryDequeue(out _)) { }

            WriteToFile(entry);

            try
            {
                EntryLogged?.Invoke(null, entry);
            }
            catch
            {
                // Never allow logging observers to crash caller
            }
        }

        public static void Trace(LogCategory category, string message, string details = null, string source = null) =>
            Log(LogLevel.Trace, category, message, details, source);

        public static void Debug(LogCategory category, string message, string details = null, string source = null) =>
            Log(LogLevel.Debug, category, message, details, source);

        public static void Info(LogCategory category, string message, string details = null, string source = null) =>
            Log(LogLevel.Info, category, message, details, source);

        public static void Warning(LogCategory category, string message, string details = null, string source = null) =>
            Log(LogLevel.Warning, category, message, details, source);

        public static void Error(LogCategory category, string message, string details = null, string source = null) =>
            Log(LogLevel.Error, category, message, details, source);

        public static void Critical(LogCategory category, string message, string details = null, string source = null) =>
            Log(LogLevel.Critical, category, message, details, source);

        public static IReadOnlyList<LogEntry> GetRecentEntries(int maxCount = 1000, LogLevel? minLevel = null, LogCategory? category = null)
        {
            var query = _logBuffer.ToArray().AsEnumerable();
            if (minLevel.HasValue)
                query = query.Where(e => e.Level >= minLevel.Value);
            if (category.HasValue)
                query = query.Where(e => e.Category == category.Value);

            return query.Reverse().Take(maxCount).Reverse().ToList();
        }

        public static void ClearMemoryLogs()
        {
            while (_logBuffer.TryDequeue(out _)) { }
        }

        public static string ExportLogsToJson(int maxCount = 5000)
        {
            var entries = GetRecentEntries(maxCount);
            return JsonSerializer.Serialize(entries, new JsonSerializerOptions { WriteIndented = true });
        }

        public static string ExportLogsToText(int maxCount = 5000)
        {
            var entries = GetRecentEntries(maxCount);
            var sb = new StringBuilder();
            foreach (var e in entries)
            {
                sb.AppendLine(e.ToString());
            }
            return sb.ToString();
        }

        private static string Sanitize(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            return SensitivePatterns.Replace(input, "$1=***REDACTED***");
        }

        private static void WriteToFile(LogEntry entry)
        {
            if (string.IsNullOrWhiteSpace(_logFilePath)) return;

            try
            {
                lock (_fileLock)
                {
                    File.AppendAllText(_logFilePath, entry.ToString() + Environment.NewLine, Encoding.UTF8);
                }
            }
            catch
            {
                // Silently fallback if file cannot be written (disk full, permissions, etc.)
            }
        }
    }
}

/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Klocman.Subsystems.Logging
{
    public static class AppLog
    {
        private static readonly string AppLogCleared = "Log wyczyszczony";
        private static readonly string AppLogHistoryTooSmall = "Zbyt mały rozmiar loga";
        private static readonly List<LogEntry> LogEntryBackingList = new List<LogEntry>(1023 + 1);
        private static int _maxLogSize = 1023;
        public static IEnumerable<LogEntry> LogEntries => LogEntryBackingList;

        public static int MaxLogSize
        {
            get { return _maxLogSize; }
            set
            {
                if (value < 2)
                    throw new ArgumentOutOfRangeException(nameof(value), AppLogHistoryTooSmall);
                _maxLogSize = value;
                LogEntryBackingList.Capacity = value + 1;

                Write($"Nowy maksymalny rozmiar logu: ({value})", LogEntryType.Debug,
                    LogEntrySource.LogSystem);
            }
        }

        public static event Action<LogEntry> EntryAdded;

        public static void ClearLog()
        {
            lock (LogEntryBackingList)
            {
                LogEntryBackingList.Clear();
                Write(AppLogCleared, LogEntryType.Info, LogEntrySource.LogSystem);
            }
        }

        public static IEnumerable<LogEntry> GetLogEntries(LogEntryType filterLevel)
        {
            lock (LogEntryBackingList)
            {
                return LogEntryBackingList.Where(x => x.Type.CompareTo(filterLevel) >= 0).ToArray();
            }
        }

        public static Color GetLogEntryTypeColor(LogEntryType type)
        {
            switch (type)
            {
                case LogEntryType.Debug:
                    return Color.DarkCyan;

                case LogEntryType.Info:
                    return Color.Black;

                case LogEntryType.Warning:
                    return Color.DarkGoldenrod;

                case LogEntryType.Error:
                    return Color.Red;

                default:
                    return Color.Pink;
            }
        }

        public static void Write(string message, LogEntryType type, LogEntrySource source, string extraSourceInfo)
        {
            var newEntry = new LogEntry(message, type, source, extraSourceInfo);
            lock (LogEntryBackingList)
            {
                LogEntryBackingList.Add(newEntry);
                TrimLogList();
            }
            OnEntryAdded(newEntry);
        }

        public static void Write(string message, LogEntryType type, LogEntrySource source)
        {
            Write(message, type, source, string.Empty);
        }

        public static void Write(string message, LogEntryType type)
        {
            Write(message, type, LogEntrySource.None, string.Empty);
        }

        public static void Write(string message)
        {
            Write(message, LogEntryType.Info, LogEntrySource.None, string.Empty);
        }

        private static void OnEntryAdded(LogEntry entry)
        {
            EntryAdded?.Invoke(entry);
        }

        private static void TrimLogList()
        {
            lock (LogEntryBackingList)
            {
                while (LogEntryBackingList.Count > _maxLogSize)
                    LogEntryBackingList.RemoveAt(0);

                // Old trimming code
                //var oldAmount = LogEntryBackingList.Count;
                //LogEntryBackingList.RemoveRange(0, _MaxLogSize / 2);
                //Write(string.Format("{0} ({1}>{2})", MSREG_Viewer.Strings.Default.AppLogTrimmed, oldAmount, LogEntryBackingList.Count), LogEntryType.Debug, LogEntrySource.LogSystem);
            }
        }
    }
}
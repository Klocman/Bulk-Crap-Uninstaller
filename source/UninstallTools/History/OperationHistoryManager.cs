/*
    EBUninstaller Pro - Open Source Professional Windows Uninstaller
    Operation History Manager Subsystem
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using UninstallTools.Core;

namespace UninstallTools.History
{
    public static class OperationHistoryManager
    {
        private static readonly List<OperationHistoryEntry> _history = new();
        private static readonly object _syncLock = new();
        private static string _historyFilePath;

        public static string HistoryFilePath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_historyFilePath))
                {
                    try
                    {
                        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                        _historyFilePath = Path.Combine(localAppData, "EBUninstallerPro", "History.json");
                    }
                    catch
                    {
                        _historyFilePath = Path.Combine(Path.GetTempPath(), "EBUninstallerPro", "History.json");
                    }
                }
                return _historyFilePath;
            }
            set => _historyFilePath = value;
        }

        static OperationHistoryManager()
        {
            LoadHistory();
        }

        public static void RecordOperation(OperationHistoryEntry entry)
        {
            if (entry == null) return;

            lock (_syncLock)
            {
                _history.Add(entry);
                SaveHistory();
            }

            StructuredLogger.Info(LogCategory.General, $"Recorded history entry: {entry}");
        }

        public static IReadOnlyList<OperationHistoryEntry> GetHistory(string searchQuery = null, string operationType = null)
        {
            lock (_syncLock)
            {
                var query = _history.AsEnumerable();

                if (!string.IsNullOrWhiteSpace(searchQuery))
                {
                    query = query.Where(h =>
                        (h.ApplicationName != null && h.ApplicationName.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)) ||
                        (h.Publisher != null && h.Publisher.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)) ||
                        (h.BackupId != null && h.BackupId.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)));
                }

                if (!string.IsNullOrWhiteSpace(operationType))
                {
                    query = query.Where(h => string.Equals(h.OperationType, operationType, StringComparison.OrdinalIgnoreCase));
                }

                return query.OrderByDescending(h => h.Timestamp).ToList();
            }
        }

        public static void ClearHistory()
        {
            lock (_syncLock)
            {
                _history.Clear();
                SaveHistory();
            }
            StructuredLogger.Info(LogCategory.General, "Cleared operation history");
        }

        public static void SaveHistory(string customPath = null)
        {
            var path = customPath ?? HistoryFilePath;
            try
            {
                var dir = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                var json = JsonSerializer.Serialize(_history, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(path, json, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                StructuredLogger.Warning(LogCategory.General, "Failed to save history", ex.Message);
            }
        }

        public static void LoadHistory(string customPath = null)
        {
            var path = customPath ?? HistoryFilePath;
            if (!File.Exists(path)) return;

            try
            {
                var json = File.ReadAllText(path);
                var loaded = JsonSerializer.Deserialize<List<OperationHistoryEntry>>(json);
                if (loaded != null)
                {
                    lock (_syncLock)
                    {
                        _history.Clear();
                        _history.AddRange(loaded);
                    }
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Warning(LogCategory.General, "Failed to load history", ex.Message);
            }
        }

        public static string ExportHistoryToJson()
        {
            lock (_syncLock)
            {
                return JsonSerializer.Serialize(_history, new JsonSerializerOptions { WriteIndented = true });
            }
        }

        public static string ExportHistoryToCsv()
        {
            var sb = new StringBuilder();
            sb.AppendLine("HistoryId,Timestamp,ApplicationName,Publisher,OperationType,Status,DetectedItems,DeletedItems,FailedItems,BackupId");

            lock (_syncLock)
            {
                foreach (var h in _history)
                {
                    sb.AppendLine($"\"{h.HistoryId}\",\"{h.Timestamp:O}\",\"{EscapeCsv(h.ApplicationName)}\",\"{EscapeCsv(h.Publisher)}\",\"{h.OperationType}\",\"{h.Status}\",{h.DetectedItemsCount},{h.DeletedItemsCount},{h.FailedItemsCount},\"{h.BackupId}\"");
                }
            }

            return sb.ToString();
        }

        private static string EscapeCsv(string text) => text?.Replace("\"", "\"\"") ?? string.Empty;
    }
}

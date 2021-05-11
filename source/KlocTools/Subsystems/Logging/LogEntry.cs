/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Text;

namespace Klocman.Subsystems.Logging
{
    public class LogEntry
    {
        public LogEntry(string message, LogEntryType type, LogEntrySource sourceName, string extraSourceInfo)
        {
            Message = message;
            SourceName = sourceName;
            ExtraSourceInfo = extraSourceInfo;
            Type = type;
            Date = DateTime.Now;
        }

        public DateTime Date { get; }
        public string ExtraSourceInfo { get; }
        public string Message { get; }
        public LogEntrySource SourceName { get; }
        public LogEntryType Type { get; }

        public string ToLongString()
        {
            var sb = new StringBuilder();
            sb.Append(Date);
            sb.Append(" - ");
            sb.Append(Type.ToString().PadRight(8));

            string tempSourceName;
            if (string.IsNullOrEmpty(ExtraSourceInfo))
                tempSourceName = string.Empty;
            else
                tempSourceName = ":" + ExtraSourceInfo;

            sb.Append(string.Concat("(", SourceName.ToString(), tempSourceName, ")").PadRight(18));
            sb.Append(" - ");
            sb.Append(Message);

            return sb.ToString();
        }

        public string ToShortString()
        {
            return string.Format("({0}{1}) - {2}", SourceName,
                string.IsNullOrEmpty(ExtraSourceInfo) ? string.Empty : (":" + ExtraSourceInfo), Message);
        }

        public override string ToString()
        {
            return string.Format("{0} ({1}{2}) - {3}", Type, SourceName,
                string.IsNullOrEmpty(ExtraSourceInfo) ? string.Empty : (":" + ExtraSourceInfo), Message);
        }
    }
}
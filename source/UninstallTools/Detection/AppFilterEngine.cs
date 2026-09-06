/*
    EBUninstaller Pro - Advanced Multi-Criteria Application Search & Filter Engine
    Supports smart query expressions: pub:<name>, size:>500MB, signed:true, tag:game, etc.
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace UninstallTools.Detection
{
    public sealed class FilterQuery
    {
        public string RawText { get; set; }
        public string PublisherFilter { get; set; }
        public long? MinSizeBytes { get; set; }
        public long? MaxSizeBytes { get; set; }
        public bool? SignedOnly { get; set; }
        public bool? StoreAppsOnly { get; set; }
        public bool? GamesOnly { get; set; }
        public DateTime? InstalledAfter { get; set; }
        public List<string> Keywords { get; set; } = new();
    }

    public static class AppFilterEngine
    {
        private static readonly Regex FilterRegex = new(@"(pub|publisher|size|signed|type|tag|after|date):([^\s""]+|""[^""]+"")", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static FilterQuery ParseQuery(string queryText)
        {
            var query = new FilterQuery { RawText = queryText };
            if (string.IsNullOrWhiteSpace(queryText)) return query;

            var matches = FilterRegex.Matches(queryText);
            var remaining = queryText;

            foreach (Match m in matches)
            {
                var key = m.Groups[1].Value.ToLowerInvariant();
                var val = m.Groups[2].Value.Trim('"');

                switch (key)
                {
                    case "pub":
                    case "publisher":
                        query.PublisherFilter = val;
                        break;
                    case "size":
                        ParseSizeFilter(val, query);
                        break;
                    case "signed":
                        if (bool.TryParse(val, out var sVal)) query.SignedOnly = sVal;
                        break;
                    case "type":
                    case "tag":
                        if (val.Equals("store", StringComparison.OrdinalIgnoreCase) || val.Equals("uwp", StringComparison.OrdinalIgnoreCase))
                            query.StoreAppsOnly = true;
                        else if (val.Equals("game", StringComparison.OrdinalIgnoreCase) || val.Equals("steam", StringComparison.OrdinalIgnoreCase))
                            query.GamesOnly = true;
                        break;
                    case "after":
                    case "date":
                        if (DateTime.TryParse(val, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
                            query.InstalledAfter = dt;
                        break;
                }

                remaining = remaining.Replace(m.Value, " ");
            }

            var words = remaining.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            query.Keywords.AddRange(words);

            return query;
        }

        private static void ParseSizeFilter(string val, FilterQuery query)
        {
            try
            {
                bool isGreater = val.StartsWith(">");
                bool isLess = val.StartsWith("<");
                var cleanVal = val.TrimStart('>', '<', '=').Trim();

                long multiplier = 1024 * 1024; // Default MB
                if (cleanVal.EndsWith("GB", StringComparison.OrdinalIgnoreCase))
                {
                    multiplier = 1024L * 1024 * 1024;
                    cleanVal = cleanVal.Substring(0, cleanVal.Length - 2).Trim();
                }
                else if (cleanVal.EndsWith("MB", StringComparison.OrdinalIgnoreCase))
                {
                    multiplier = 1024 * 1024;
                    cleanVal = cleanVal.Substring(0, cleanVal.Length - 2).Trim();
                }
                else if (cleanVal.EndsWith("KB", StringComparison.OrdinalIgnoreCase))
                {
                    multiplier = 1024;
                    cleanVal = cleanVal.Substring(0, cleanVal.Length - 2).Trim();
                }

                if (double.TryParse(cleanVal, NumberStyles.Float, CultureInfo.InvariantCulture, out var num))
                {
                    long bytes = (long)(num * multiplier);
                    if (isGreater) query.MinSizeBytes = bytes;
                    else if (isLess) query.MaxSizeBytes = bytes;
                    else query.MinSizeBytes = bytes;
                }
            }
            catch { }
        }

        public static IEnumerable<ApplicationUninstallerEntry> Filter(IEnumerable<ApplicationUninstallerEntry> entries, string queryText)
        {
            if (entries == null) return Enumerable.Empty<ApplicationUninstallerEntry>();
            if (string.IsNullOrWhiteSpace(queryText)) return entries;

            var query = ParseQuery(queryText);

            return entries.Where(app => MatchesQuery(app, query));
        }

        public static bool MatchesQuery(ApplicationUninstallerEntry app, FilterQuery query)
        {
            if (app == null) return false;

            // 1. Publisher Filter
            if (!string.IsNullOrEmpty(query.PublisherFilter))
            {
                if (string.IsNullOrEmpty(app.Publisher) || !app.Publisher.Contains(query.PublisherFilter, StringComparison.OrdinalIgnoreCase))
                    return false;
            }

            // 2. Size Filters
            var appBytes = app.EstimatedSize.GetKbSize() * 1024;
            if (query.MinSizeBytes.HasValue && appBytes < query.MinSizeBytes.Value)
                return false;
            if (query.MaxSizeBytes.HasValue && appBytes > query.MaxSizeBytes.Value)
                return false;

            // 3. Store App Filter
            if (query.StoreAppsOnly.HasValue && query.StoreAppsOnly.Value)
            {
                if (app.UninstallerKind != UninstallerType.StoreApp)
                    return false;
            }

            // 4. Games Filter
            if (query.GamesOnly.HasValue && query.GamesOnly.Value)
            {
                if (app.UninstallerKind != UninstallerType.Steam && app.UninstallerKind != UninstallerType.Oculus)
                    return false;
            }

            // 5. Date Filter
            if (query.InstalledAfter.HasValue && app.InstallDate != default)
            {
                if (app.InstallDate < query.InstalledAfter.Value)
                    return false;
            }

            // 6. Keywords
            foreach (var kw in query.Keywords)
            {
                var matchName = app.DisplayName != null && app.DisplayName.Contains(kw, StringComparison.OrdinalIgnoreCase);
                var matchPub = app.Publisher != null && app.Publisher.Contains(kw, StringComparison.OrdinalIgnoreCase);
                var matchLoc = app.InstallLocation != null && app.InstallLocation.Contains(kw, StringComparison.OrdinalIgnoreCase);

                if (!matchName && !matchPub && !matchLoc)
                    return false;
            }

            return true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using BulkCrapUninstaller.Properties;
using MySql.Data.MySqlClient;

namespace BulkCrapUninstaller.Functions.Ratings
{
    public class UninstallerRatingManager : IDisposable
    {
        private readonly object _casheLock = new object();

        private readonly Dictionary<string, UninstallerRating> _ratingsToSend =
            new Dictionary<string, UninstallerRating>();

        private DataTable _cashe;

        public UninstallerRatingManager(long userId)
        {
            UserId = userId;
        }

        private long UserId { get; }

        public IEnumerable<RatingEntry> Items
        {
            get
            {
                if (_cashe == null || _cashe.Columns.Count != 3)
                    return Enumerable.Empty<RatingEntry>();

                return from DataRow row in _cashe.Rows
                    select ToRatingEntry(row);
            }
        }

        public void Dispose()
        {
            lock (_ratingsToSend)
            {
                SendRatings();
                lock (_casheLock)
                    _cashe?.Dispose();
                _ratingsToSend.Clear();
            }
        }

        public void RefreshStats()
        {
            using (var connection = new MySqlConnection(Program.DbConnectionString))
            {
                var command = connection.CreateCommand();
                command.CommandText = "CALL " + Resources.DbCommandGetRating + "(@uid)";
                command.Parameters.AddWithValue("@uid", UserId);

                connection.Open();

                var dt = new DataTable();
                dt.Load(command.ExecuteReader());

                lock (_casheLock)
                {
                    _cashe?.Dispose();
                    _cashe = dt;
                }
            }
        }

        public void SendRatings()
        {
            using (var connection = new MySqlConnection(Program.DbConnectionString))
            {
                var command = connection.CreateCommand();
                command.CommandText = "CALL " + Resources.DbCommandSetRating + "(@userParam, @appParam, @rating)";
                command.Parameters.AddWithValue("@userParam", UserId);

                command.Parameters.AddWithValue("@appParam", string.Empty);
                command.Parameters.AddWithValue("@rating", 0);
                connection.Open();

                lock (_ratingsToSend)
                {
                    foreach (var uninstallerRating in _ratingsToSend)
                    {
                        command.Parameters["@appParam"].Value = uninstallerRating.Key;
                        command.Parameters["@rating"].Value = (int) uninstallerRating.Value;

                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        private DataRow GetCasheEntry(string appName)
        {
            if (string.IsNullOrEmpty(appName))
                throw new ArgumentNullException(nameof(appName));

            lock (_casheLock)
                return _cashe.Rows.Cast<DataRow>().FirstOrDefault(
                    r => appName.Equals(r[0] as string, StringComparison.InvariantCultureIgnoreCase));
        }

        public void SetMyRating(string appKey, UninstallerRating rating)
        {
            if (string.IsNullOrEmpty(appKey))
                throw new ArgumentNullException(nameof(appKey));
            if (rating == UninstallerRating.Unknown)
                throw new ArgumentException("Can't set unknown rating", nameof(rating));

            lock (_casheLock)
            {
                var stored = GetCasheEntry(appKey);
                var newRating = (int) rating;
                if (stored != null)
                    stored[2] = newRating;
                else
                    _cashe.Rows.Add(appKey, newRating, newRating);
            }

            lock (_ratingsToSend)
            {
                if (_ratingsToSend.ContainsKey(appKey))
                    _ratingsToSend[appKey] = rating;
                else
                    _ratingsToSend.Add(appKey, rating);
            }
        }

        private static RatingEntry ToRatingEntry(DataRow row)
        {
            return new RatingEntry
            {
                ApplicationName = row[0] as string,
                AverageRating = row.IsNull(1) ? (int?) null : Convert.ToInt32(row[1]),
                MyRating = row.IsNull(2) ? (int?) null : Convert.ToInt32(row[2])
            };
        }

        public RatingEntry GetRating(string appName)
        {
            var row = GetCasheEntry(appName);
            return row == null ? new RatingEntry() : ToRatingEntry(row);
        }

        public void SerializeCashe(string fileName)
        {
            lock (_casheLock)
            {
                File.Delete(fileName);
                _cashe.WriteXml(fileName);
            }
        }

        public void DeserializeCashe(string fileName)
        {
            lock (_casheLock)
            {
                _cashe?.Dispose();
                _cashe = new DataTable();

                using (var reader = new StringReader(Resources.DbRatingSchema))
                    _cashe.ReadXmlSchema(reader);

                _cashe.ReadXml(fileName);
            }
        }

        public static UninstallerRating ToRating(int val)
        {
            if (val <= ((int) UninstallerRating.Bad + (int) UninstallerRating.Neutral)/2)
                return UninstallerRating.Bad;
            if (val >= ((int) UninstallerRating.Good + (int) UninstallerRating.Neutral)/2)
                return UninstallerRating.Good;

            return UninstallerRating.Neutral;
        }

        public struct RatingEntry : IEquatable<RatingEntry>
        {
            public string ApplicationName { get; set; }
            public int? AverageRating { get; set; }
            public int? MyRating { get; set; }
            public bool IsEmpty => ApplicationName == null && !AverageRating.HasValue && !MyRating.HasValue;
            public static RatingEntry Empty { get; } = default(RatingEntry);

            public static RatingEntry NotAvailable { get; } = new RatingEntry
            {
                AverageRating = int.MinValue,
                MyRating = int.MinValue,
                ApplicationName = Localisable.NotAvailable
            };

            public bool Equals(RatingEntry other)
            {
                return AverageRating == other.AverageRating
                       && MyRating == other.MyRating
                       && ApplicationName != null
                       && ApplicationName.Equals(other.ApplicationName);
            }

            public override bool Equals(object obj)
            {
                return obj is RatingEntry && Equals((RatingEntry) obj);
            }
        }
    }
}
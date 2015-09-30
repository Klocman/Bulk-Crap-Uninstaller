using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using BulkCrapUninstaller.Properties;
using Klocman.Extensions;
using MySql.Data.MySqlClient;

namespace BulkCrapUninstaller.Functions.Ratings
{
    public class UninstallerRatingManager : IDisposable
    {
        //public DateTime LastCasheFetchTime { get; private set; }

        /// <summary>
        ///     Always lock before locking _ratingsToSend
        /// </summary>
        private readonly object _casheLock = new object();

        private readonly Dictionary<string, UninstallerRating> _ratingsToSend =
            new Dictionary<string, UninstallerRating>();

        private DataTable _cashe;

        public UninstallerRatingManager(long userId)
        {
            UserId = userId;

            _cashe = new DataTable();
            using (var reader = new StringReader(Resources.DbRatingSchema))
                _cashe.ReadXmlSchema(reader);
        }

        private long UserId { get; }

        public int RatingCount => _cashe == null ? 0 : _cashe.Rows.Count;

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
            lock (_casheLock)
                _cashe?.Dispose();
            lock (_ratingsToSend)
                _ratingsToSend.Clear();
        }

        public void FetchRatings()
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

                    // Reapply any pending user ratings to the new datatable
                    lock (_ratingsToSend)
                    {
                        foreach (var rating in _ratingsToSend)
                        {
                            var stored = GetCasheEntry(rating.Key);
                            var newRating = (int)rating.Value;
                            if (stored != null)
                                stored[2] = newRating;
                            else
                                _cashe.Rows.Add(rating.Key, newRating, newRating);
                        }
                    }
                }
            }
        }

        public void UploadRatings()
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
                        command.Parameters["@rating"].Value = (int)uninstallerRating.Value;

                        command.ExecuteNonQuery();
                    }

                    _ratingsToSend.Clear();
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
                var newRating = (int)rating;
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
                AverageRating = row.IsNull(1) ? (int?)null : Convert.ToInt32(row[1]),
                MyRating = row.IsNull(2) ? (int?)null : Convert.ToInt32(row[2])
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
                lock (_ratingsToSend)
                {
                    File.Delete(fileName);
                    _cashe.WriteXml(fileName);

                    var sendCasheName = fileName + ".out";
                    File.Delete(sendCasheName);
                    if (_ratingsToSend.Any())
                    {
                        using (var writer = new StringWriter())
                        {
                            _ratingsToSend.Serialize(writer);
                            File.WriteAllText(sendCasheName, writer.GetStringBuilder().ToString(), Encoding.Unicode);
                        }
                    }
                }
        }

        public void DeleteCashe(string fileName)
        {
            File.Delete(fileName);
            var sendCasheName = fileName + ".out";
            File.Delete(sendCasheName);
        }

        public void DeserializeCashe(string fileName)
        {
            lock (_casheLock)
                lock (_ratingsToSend)
                {
                    if (File.Exists(fileName))
                    {
                        _cashe?.Dispose();
                        _cashe = new DataTable();

                        using (var reader = new StringReader(Resources.DbRatingSchema))
                            _cashe.ReadXmlSchema(reader);

                        _cashe.ReadXml(fileName);
                    }

                    var sendCasheName = fileName + ".out";
                    if (File.Exists(sendCasheName))
                    {
                        using (var reader = new StringReader(File.ReadAllText(sendCasheName, Encoding.Unicode)))
                        {
                            _ratingsToSend.Deserialize(reader);
                        }
                    }
                }
        }

        public static UninstallerRating ToRating(int val)
        {
            if (val <= ((int)UninstallerRating.Bad + (int)UninstallerRating.Neutral) / 2)
                return UninstallerRating.Bad;
            if (val >= ((int)UninstallerRating.Good + (int)UninstallerRating.Neutral) / 2)
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
                return obj is RatingEntry && Equals((RatingEntry)obj);
            }
        }

        public void ClearRatings()
        {
            lock (_cashe)
                _cashe.Rows.Clear();
            lock (_ratingsToSend)
                _ratingsToSend.Clear();
        }
    }
}
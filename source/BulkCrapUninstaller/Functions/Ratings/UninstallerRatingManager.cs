/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
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
        private readonly object _cacheLock = new object();

        private readonly Dictionary<string, UninstallerRating> _ratingsToSend =
            new Dictionary<string, UninstallerRating>();

        private DataTable _cashe;

        public UninstallerRatingManager(long userId)
        {
            UserId = userId;

            _cashe = new DataTable { Locale = CultureInfo.InvariantCulture };
            using (var reader = new StringReader(Resources.DbRatingSchema))
                _cashe.ReadXmlSchema(reader);
        }

        private long UserId { get; }

        public int RatingCount
        {
            get
            {
                lock (_cashe)
                    return _cashe.Rows.Count;
            }
        }

        public IEnumerable<RatingEntry> Items
        {
            get
            {
                lock (_cacheLock)
                {
                    if (_cashe == null || _cashe.Columns.Count != 3)
                        return Enumerable.Empty<RatingEntry>();

                    return (from DataRow row in _cashe.Rows
                            select ToRatingEntry(row)).ToList();
                }
            }
        }

        public void Dispose()
        {
            lock (_cacheLock)
                _cashe?.Dispose();
            lock (_ratingsToSend)
                _ratingsToSend.Clear();
        }

        public void FetchRatings()
        {
            using (var connection = new MySqlConnection(Program.DbConnectionString))
            {
                var ratingTable = new DataTable { Locale = CultureInfo.InvariantCulture };
                var userRatingTable = new DataTable { Locale = CultureInfo.InvariantCulture };

                var avgRatingCommand = connection.CreateCommand();
                avgRatingCommand.CommandText = "CALL getAvgRatings()";

                var userRatingCommand = connection.CreateCommand();
                userRatingCommand.CommandText = "CALL getUserRatings(@uid)";
                userRatingCommand.Parameters.AddWithValue("@uid", UserId);

                connection.Open();

                ratingTable.Load(avgRatingCommand.ExecuteReader());
                ratingTable.Columns.Add("userRating");

                userRatingTable.Load(userRatingCommand.ExecuteReader());

                // Merge tables
                foreach (DataRow row in userRatingTable.Rows)
                {
                    var dataRow = ratingTable.Select($"applicationName = '{((string)row[0]).Replace("'", "''")}'")
                        .FirstOrDefault();

                    if (dataRow != null) dataRow[2] = row[1];
                }

                lock (_cacheLock)
                {
                    _cashe?.Dispose();
                    _cashe = ratingTable;

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
            if (_ratingsToSend.Count < 1) return;

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

            lock (_cacheLock)
            {
                return _cashe.Select($"applicationName = '{appName.Replace("'", "''")}'")
                    .FirstOrDefault();
            }
        }

        public void SetMyRating(string appKey, UninstallerRating rating)
        {
            if (string.IsNullOrEmpty(appKey))
                throw new ArgumentNullException(nameof(appKey));
            if (rating == UninstallerRating.Unknown)
                throw new ArgumentException("Can't set unknown rating", nameof(rating));

            lock (_cacheLock)
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
                AverageRating = row.IsNull(1) ? (int?)null : Convert.ToInt32(row[1], CultureInfo.InvariantCulture),
                MyRating = row.IsNull(2) ? (int?)null : Convert.ToInt32(row[2], CultureInfo.InvariantCulture)
            };
        }

        public RatingEntry GetRating(string appName)
        {
            var row = GetCasheEntry(appName);
            return row == null ? new RatingEntry() : ToRatingEntry(row);
        }

        public void SerializeCache(string fileName)
        {
            lock (_cacheLock)
                lock (_ratingsToSend)
                {
                    File.Delete(fileName);
                    _cashe.WriteXml(fileName);

                    var sendCasheName = fileName + ".out";
                    File.Delete(sendCasheName);
                    if (_ratingsToSend.Any())
                    {
                        using (var writer = new StringWriter(CultureInfo.InvariantCulture))
                        {
                            _ratingsToSend.Serialize(writer);
                            File.WriteAllText(sendCasheName, writer.GetStringBuilder().ToString(), Encoding.Unicode);
                        }
                    }
                }
        }

        public static void DeleteCache(string fileName)
        {
            File.Delete(fileName);
            var sendCacheName = fileName + ".out";
            File.Delete(sendCacheName);
        }

        public void DeserializeCache(string fileName)
        {
            lock (_cacheLock)
                lock (_ratingsToSend)
                {
                    try
                    {
                        if (File.Exists(fileName))
                        {
                            _cashe?.Dispose();
                            _cashe = new DataTable { Locale = CultureInfo.InvariantCulture };

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
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
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
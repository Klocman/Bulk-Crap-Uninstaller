/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace BulkCrapUninstaller.Functions.Ratings
{
    public partial class UninstallerRatingManager : IDisposable
    {
        private readonly Dictionary<ulong, int> _ratingsToSend = new();
        private readonly Dictionary<ulong, int> _avgRatings = new();
        private readonly Dictionary<ulong, int> _myRatings = new();


        public UninstallerRatingManager(ulong userId)
        {
            if (userId == 0) throw new ArgumentOutOfRangeException(nameof(userId));
            UserId = userId;
        }

        private ulong UserId { get; }

        public int RemoteRatingCount => _avgRatings.Count;

        public int UserRatingCount => _myRatings.Count;

        public void Dispose()
        {
            //lock (_ratingsToSend)
            //    _ratingsToSend.Clear();
        }

        public void FetchRatings()
        {
            using var cl = Program.GetHttpClient();

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var txt = cl.GetStringAsync(new Uri(@"GetAverageRatingsComp", UriKind.Relative)).Result.Trim('"');
            var bytes = Convert.FromBase64String(txt);
            var remoteAvgRatings = Utils.DecompressAndDeserialize<List<Utils.AverageRatingEntry>>(bytes, options);

            var txt2 = cl.GetStringAsync(new Uri(@"GetUserRatings?userId=" + UserId, UriKind.Relative)).Result;
            var remoteMyRatings = JsonSerializer.Deserialize<List<Utils.UserRatingEntry>>(txt2, options);

            _avgRatings.Clear();
            foreach (Utils.AverageRatingEntry entry in remoteAvgRatings)
                _avgRatings.Add(entry.AppId, entry.AverageRating);

            if (remoteMyRatings != null)
            {
                _myRatings.Clear();
                foreach (Utils.UserRatingEntry entry in remoteMyRatings)
                    _myRatings.Add(entry.AppId, entry.Rating);
            }
        }

        public void UploadRatings()
        {
            lock (_ratingsToSend)
                if (_ratingsToSend.Count < 1) return;

            using var cl = Program.GetHttpClient();

            lock (_ratingsToSend)
            {
                foreach (var uninstallerRating in _ratingsToSend)
                {
                    var msg = cl.PutAsync(new Uri($@"SetUserRating?userId={UserId}&appId={uninstallerRating.Key}&rating={uninstallerRating.Value}", UriKind.Relative), null!).Result;
                    msg.EnsureSuccessStatusCode();
                }

                _ratingsToSend.Clear();
            }
        }

        public void SetMyRating(string appKey, UninstallerRating rating)
        {
            if (string.IsNullOrEmpty(appKey))
                throw new ArgumentNullException(nameof(appKey));
            if (rating == UninstallerRating.Unknown)
                throw new ArgumentException("Can't set unknown rating", nameof(rating));

            lock (_ratingsToSend)
            {
                var appId = Utils.StableHash(appKey);
                _ratingsToSend[appId] = (int)rating;
                _myRatings[appId] = (int)rating;
            }
        }

        public RatingEntry GetRating(string appName)
        {
            var appId = Utils.StableHash(appName);
            int? avg = null;
            int? my = null;
            if (_avgRatings.TryGetValue(appId, out var a)) avg = a;
            if (_myRatings.TryGetValue(appId, out var m)) my = m;
            return new RatingEntry { ApplicationName = appName, AverageRating = avg, MyRating = my };
        }

        public void SerializeCache(DirectoryInfo directory)
        {
            if (directory == null) throw new ArgumentNullException(nameof(directory));
            lock (_ratingsToSend)
            {
                var dir = directory.FullName;
                var avgPath = Path.Combine(dir, "RatingCashe_Avg.json");
                var myPath = Path.Combine(dir, "RatingCashe_User.json");
                var pendingPath = Path.Combine(dir, "RatingCashe_Pending.json");

                File.WriteAllText(avgPath, JsonSerializer.Serialize(_avgRatings));
                File.WriteAllText(myPath, JsonSerializer.Serialize(_myRatings));
                File.WriteAllText(pendingPath, JsonSerializer.Serialize(_ratingsToSend));
            }
        }

        public static void DeleteCache(DirectoryInfo directory)
        {
            if (directory == null) throw new ArgumentNullException(nameof(directory));
            var dir = directory.FullName;
            var avgPath = Path.Combine(dir, "RatingCashe_Avg.json");
            var myPath = Path.Combine(dir, "RatingCashe_User.json");
            var pendingPath = Path.Combine(dir, "RatingCashe_Pending.json");
            File.Delete(avgPath);
            File.Delete(myPath);
            File.Delete(pendingPath);
        }

        public void DeserializeCache(DirectoryInfo directory)
        {
            if (directory == null) throw new ArgumentNullException(nameof(directory));
            lock (_ratingsToSend)
            {
                var dir = directory.FullName;
                var avgPath = Path.Combine(dir, "RatingCashe_Avg.json");
                var myPath = Path.Combine(dir, "RatingCashe_User.json");
                var pendingPath = Path.Combine(dir, "RatingCashe_Pending.json");

                LoadFromFile(avgPath, _avgRatings);
                LoadFromFile(myPath, _myRatings);
                LoadFromFile(pendingPath, _ratingsToSend);
            }
        }

        private static void LoadFromFile(string path, Dictionary<ulong, int> target)
        {
            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var avgRatings = JsonSerializer.Deserialize<Dictionary<ulong, int>>(File.ReadAllText(path), options);

                if (avgRatings != null)
                {
                    target.Clear();
                    foreach (var avgRating in avgRatings)
                        target.Add(avgRating.Key, avgRating.Value);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void ClearRatings()
        {
            lock (_ratingsToSend)
            {
                _ratingsToSend.Clear();
                _myRatings.Clear();
                _avgRatings.Clear();
            }
        }
    }
}
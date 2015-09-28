using BulkCrapUninstaller.Properties;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BulkCrapUninstaller.Functions.Ratings
{
    public enum UninstallerRating
    {
        Unknown = 127,
        Neutral = 0,
        Bad = -10,
        Good = 10
    }

    public sealed class UninstallerRatings
    {
        public UninstallerRatings(UninstallerRating average, UninstallerRating user)
        {
            AverageRating = average;
            MyRating = user;
        }

        public UninstallerRating AverageRating { get; set; }
        public UninstallerRating MyRating { get; set; }

        public static UninstallerRatings Unknown { get; }
            = new UninstallerRatings(UninstallerRating.Unknown, UninstallerRating.Unknown);
    }

    public class UninstallerRatingManager : IDisposable
    {
        public UninstallerRatingManager (long userId)
        { UserId = userId; }

        private long UserId { get; set; }

        private Dictionary<string, UninstallerRatings> _ratingCashe = new Dictionary<string, UninstallerRatings>();
        
        MySqlConnection currentConnection;
        MySqlConnection CurrentConnection
        {
            get
            {
                lock(_ratingCashe)
                {
                    if (currentConnection == null)
                    {
                        currentConnection = new MySqlConnection(Resources.DbConnectionString);
                        currentConnection.Open();
                    }
                    else if(currentConnection.State != System.Data.ConnectionState.Open)
                    {
                        currentConnection.Dispose();
                        currentConnection = new MySqlConnection(Resources.DbConnectionString);
                        currentConnection.Open();
                    }
                }

                return currentConnection;
            }
        }

        private UninstallerRating ConvertRating (int rating)
        {
            if (rating >= 5)
                return UninstallerRating.Good;
            if (rating <= -5)
                return UninstallerRating.Bad;

            return UninstallerRating.Neutral;
        }

        public void RefreshStats(IEnumerable<string> appKeys)
        {
            lock (_ratingCashe)
            {
                var connection = CurrentConnection;

                var command = connection.CreateCommand();
                //command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = "CALL " + Resources.DbCommandGetRating + "(@userParam, @appParam)";
                command.Parameters.Add(new MySqlParameter("@userParam", UserId));

                var nameParam = new MySqlParameter("@appParam", null);
                command.Parameters.Add(nameParam);

                var averageRatParam = new MySqlParameter("@rat", MySqlDbType.Int32);
                averageRatParam.Direction = System.Data.ParameterDirection.Output;
                command.Parameters.Add(averageRatParam);

                var userRatParam = new MySqlParameter("@myrat", MySqlDbType.Int32);
                userRatParam.Direction = System.Data.ParameterDirection.Output;
                command.Parameters.Add(userRatParam);

                foreach (var name in appKeys)
                {
                    try
                    {
                        nameParam.Value = name;
                        var reader = command.ExecuteReader(System.Data.CommandBehavior.SingleResult);
                        _ratingCashe.Add(name, new UninstallerRatings(
                            ConvertRating((int)averageRatParam.Value),
                            ConvertRating((int)userRatParam.Value)));
                    }
                    catch
                    {
                        throw;
                    }
                }

                connection.Close();
            }
        }
        
        public void SetRating(string appKey, UninstallerRating rating)
        {
            if (!_ratingCashe.ContainsKey(appKey))
                _ratingCashe.Add(appKey, new UninstallerRatings(rating, rating));
                else
            _ratingCashe[appKey].MyRating = rating;
            
            lock (_ratingCashe)
            {
                var connection = CurrentConnection;

                var command = connection.CreateCommand();
                command.CommandText = "CALL " + Resources.DbCommandSetRating + "(@userParam, @appParam)";
                command.Parameters.AddWithValue("@userParam", UserId);
                command.Parameters.AddWithValue("@appParam", appKey);
                command.Parameters.AddWithValue("@rating", (int)rating);

                command.ExecuteNonQuery();

                //connection.Close();
            }
        }

        public UninstallerRatings GetRating(string appKey)
        {
            if (_ratingCashe.ContainsKey(appKey))
                return _ratingCashe[appKey];
            return UninstallerRatings.Unknown;
        }

        public void Dispose()
        {
            currentConnection?.Dispose();
        }
    }
}

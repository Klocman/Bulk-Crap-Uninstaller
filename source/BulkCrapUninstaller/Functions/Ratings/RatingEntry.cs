/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using BulkCrapUninstaller.Properties;

namespace BulkCrapUninstaller.Functions.Ratings
{
    public struct RatingEntry : IEquatable<RatingEntry>
    {
        public override int GetHashCode()
        {
            unchecked
            {
                return (AverageRating.GetHashCode() * 397) ^ MyRating.GetHashCode();
            }
        }

        public string ApplicationName { get; set; }
        public int? AverageRating { get; set; }
        public int? MyRating { get; set; }
        public bool IsEmpty => ApplicationName == null && !HasValue;
        public bool HasValue => AverageRating.HasValue || MyRating.HasValue;
        public static RatingEntry Empty { get; } = default(RatingEntry);

        public static RatingEntry NotAvailable { get; } = new()
        {
            AverageRating = null,
            MyRating = null,
            ApplicationName = Localisable.NotAvailable
        };

        public bool Equals(RatingEntry other)
        {
            return AverageRating == other.AverageRating && MyRating == other.MyRating && ApplicationName == other.ApplicationName;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is RatingEntry entry && Equals(entry);
        }

        public static UninstallerRating ToRating(int value)
        {
            if (value <= ((int)UninstallerRating.Bad + (int)UninstallerRating.Neutral) / 2)
                return UninstallerRating.Bad;
            if (value >= ((int)UninstallerRating.Good + (int)UninstallerRating.Neutral) / 2)
                return UninstallerRating.Good;

            return UninstallerRating.Neutral;
        }

        public override string ToString()
        {
            return string.Format(Localisable.RatingEntry_ToString,
                AverageRating.HasValue ? ToRating(AverageRating.Value) : UninstallerRating.Unknown,
                MyRating.HasValue ? ToRating(MyRating.Value) : UninstallerRating.Unknown);
        }
    }
}
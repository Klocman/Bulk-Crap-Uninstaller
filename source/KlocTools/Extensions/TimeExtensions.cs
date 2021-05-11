/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using Klocman.Properties;

namespace Klocman.Extensions
{
    public static class TimeExtensions
    {
        /// <summary>
        ///     Get fuzzy time string as spoken at present moment when referring to this DateTime.
        ///     For example 6 years ago, 3 months ago, Just now.
        /// </summary>
        public static string ToFuzzyTimeSinceString(this DateTime source)
        {
            return ToFuzzyTimeSinceString(source, DateTime.Now);
        }

        /// <summary>
        ///     Get fuzzy time string as spoken at supplied time (now) when referring to this DateTime.
        ///     For example 6 years ago, 3 months ago, Just now.
        /// </summary>
        /// <param name="past">Point in time that happened in the past</param>
        /// <param name="now">From which point to speak about this DateTime</param>
        public static string ToFuzzyTimeSinceString(this DateTime past, DateTime now)
        {
            var difference = now.Subtract(past);
            var years = difference.Days/365; //no leap year accounting

            if (years > 0)
            {
                if (years == 1)
                    return Localisation.ToFuzzyTimeSinceString_Years_Single;
                return string.Format(Localisation.ToFuzzyTimeSinceString_Years_Since, years);
            }

            var months = (difference.Days%365)/30; //naive guess at month size

            if (months > 0)
            {
                if (months == 1)
                    return Localisation.ToFuzzyTimeSinceString_Months_Single;
                return string.Format(Localisation.ToFuzzyTimeSinceString_Months_Since, months);
            }

            if (difference.Days > 0)
            {
                if (difference.Days == 1)
                    return Localisation.ToFuzzyTimeSinceString_Days_Single;
                return string.Format(Localisation.ToFuzzyTimeSinceString_Days_Since, difference.Days);
            }

            if (difference.Hours > 0)
            {
                if (difference.Hours == 1)
                    return Localisation.ToFuzzyTimeSinceString_Hours_Single;
                return string.Format(Localisation.ToFuzzyTimeSinceString_Hours_Since, difference.Hours);
            }

            if (difference.Minutes > 0)
            {
                if (difference.Minutes == 1)
                    return Localisation.ToFuzzyTimeSinceString_Minutes_Single;
                return string.Format(Localisation.ToFuzzyTimeSinceString_Minutes_Since, difference.Minutes);
            }

            return Localisation.ToFuzzyTimeSinceString_JustNow;
        }
    }
}
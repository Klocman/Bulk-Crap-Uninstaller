/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using Klocman.Localising;
using UninstallTools.Properties;

namespace UninstallTools.Junk.Confidence
{
    public enum ConfidenceLevel
    {
        [LocalisedName(typeof (Localisation), "Confidence_Unknown")] Unknown = 0,
        [LocalisedName(typeof (Localisation), "Confidence_Bad")] Bad = 5,
        [LocalisedName(typeof (Localisation), "Confidence_Questionable")] Questionable = 7,
        [LocalisedName(typeof (Localisation), "Confidence_Good")] Good = 9,
        [LocalisedName(typeof (Localisation), "Confidence_VeryGood")] VeryGood = 12
    }
}
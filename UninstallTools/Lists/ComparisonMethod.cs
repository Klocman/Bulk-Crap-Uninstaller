/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using Klocman.Localising;
using UninstallTools.Properties;

namespace UninstallTools.Lists
{
    public enum ComparisonMethod
    {
        [LocalisedName(typeof (Localisation), "FilterComparisonMethod_Equals")] Equals,
        [LocalisedName(typeof (Localisation), "FilterComparisonMethod_Any")] Any,
        [LocalisedName(typeof (Localisation), "FilterComparisonMethod_Contains")] Contains,
        [LocalisedName(typeof (Localisation), "FilterComparisonMethod_StartsWith")] StartsWith,
        [LocalisedName(typeof (Localisation), "FilterComparisonMethod_EndsWith")] EndsWith,
        [LocalisedName(typeof (Localisation), "FilterComparisonMethod_Regex")] Regex
    }
}
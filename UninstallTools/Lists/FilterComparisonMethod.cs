using Klocman.Localising;
using UninstallTools.Properties;

namespace UninstallTools.Lists
{
    public enum FilterComparisonMethod
    {
        [LocalisedName(typeof (Localisation), "FilterComparisonMethod_Equals")] Equals,
        [LocalisedName(typeof (Localisation), "FilterComparisonMethod_Any")] Any,
        [LocalisedName(typeof (Localisation), "FilterComparisonMethod_Contains")] Contains,
        [LocalisedName(typeof (Localisation), "FilterComparisonMethod_StartsWith")] StartsWith,
        [LocalisedName(typeof (Localisation), "FilterComparisonMethod_EndsWith")] EndsWith,
        [LocalisedName(typeof (Localisation), "FilterComparisonMethod_Regex")] Regex
    }
}
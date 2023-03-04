/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using Klocman.Localising;
using Klocman.Properties;

namespace Klocman
{
    public enum YesNoAsk
    {
        [LocalisedName(typeof (Localisation), nameof(Localisation.Enums_YesNoAsk_Ask))] Ask = 0,
        [LocalisedName(typeof (Localisation), nameof(Localisation.Yes))] Yes,
        [LocalisedName(typeof (Localisation), nameof(Localisation.No))] No
    }
}
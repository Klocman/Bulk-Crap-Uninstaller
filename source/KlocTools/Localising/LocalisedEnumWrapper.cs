/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;

namespace Klocman.Localising
{
    public class LocalisedEnumWrapper
    {
        public LocalisedEnumWrapper(Enum targetEnum)
        {
            TargetEnum = targetEnum ?? throw new ArgumentNullException(nameof(targetEnum));
        }

        public Enum TargetEnum { get; }

        public override string ToString()
        {
            return TargetEnum.GetLocalisedName();
        }
    }
}
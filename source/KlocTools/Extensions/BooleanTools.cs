/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.Windows.Forms;
using Klocman.Properties;
using Klocman.Resources;

namespace Klocman.Extensions
{
    public static class BooleanTools
    {
        /// <summary>
        ///     Convert CheckState value to bool.
        ///     CheckState.Checked returns true, everything else returns false.
        /// </summary>
        public static bool ToBool(this CheckState value)
        {
            return value == CheckState.Checked;
        }

        /// <summary>
        ///     Convert boolean value to CheckState.Checked or CheckState.Unchecked.
        /// </summary>
        public static CheckState ToCheckState(this bool value)
        {
            return value ? CheckState.Checked : CheckState.Unchecked;
        }

        /// <summary>
        ///     Convert nullable bool to CheckState, null being CheckState.Indeterminate.
        /// </summary>
        public static CheckState ToCheckState(this bool? value)
        {
            if (!value.HasValue)
                return CheckState.Indeterminate;
            return value.Value ? CheckState.Checked : CheckState.Unchecked;
        }

        /// <summary>
        ///     Convert CheckState to nullable bool, CheckState.Indeterminate being null.
        /// </summary>
        public static bool? ToNullBool(this CheckState value)
        {
            if (value == CheckState.Indeterminate)
                return null;
            return value == CheckState.Checked;
        }

        /// <summary>
        ///     Convert boolean value to string contained in Localisation.Yes and Localisation.No resources. Can be localised.
        /// </summary>
        public static string ToYesNo(this bool value)
        {
            return value ? Localisation.Yes : Localisation.No;
        }

        /// <summary>
        ///     Convert nullable boolean value to string contained in
        ///     Localisation.Yes, Localisation.No and CommonStrings.Unknown resources. Can be localised.
        /// </summary>
        public static string ToYesNo(this bool? value)
        {
            return value.HasValue ? value.Value.ToYesNo() : CommonStrings.Unknown;
        }
    }
}
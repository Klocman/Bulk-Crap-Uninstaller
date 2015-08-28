// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggerCategory.cs" company="NBug Project">
//   Copyright (c) 2011 - 2013 Teoman Soygul. Licensed under MIT license.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace NBug.Enums
{
    public enum LoggerCategory
    {
        /// <summary>
        ///     This category outputs most detailed information about the internal state of the library. Every single major event
        ///     (like
        ///     generating a bug report, submitting a bug report, truncating internal files, etc.) is logged in this category.
        /// </summary>
        NBugTrace,

        /// <summary>
        ///     This category outputs results of substantial events like the server response after submitting a bug report to the a
        ///     server.
        /// </summary>
        NBugInfo,

        /// <summary>
        ///     This category outputs warning messages from non-exceptional but important errors like a missing or inaccessable
        ///     files.
        /// </summary>
        NBugWarning,

        /// <summary>
        ///     This category outputs error messages for exceptional and critical situations. These situations generally disable
        ///     some functionality
        ///     of the library or halts the execution of some code path. If not in release mode, an exception is thrown for error
        ///     messages.
        /// </summary>
        NBugError
    }
}
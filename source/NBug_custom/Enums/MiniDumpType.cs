// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MiniDumpType.cs" company="NBug Project">
//   Copyright (c) 2011 - 2013 Teoman Soygul. Licensed under MIT license.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace NBug.Enums
{
    public enum MiniDumpType
    {
        /// <summary>
        ///     Generate no minidump at all.
        /// </summary>
        None,

        /// <summary>
        ///     Generates the smallest possible minidump still with useful information. Dump size is about ~100KB compressed.
        /// </summary>
        Tiny,

        /// <summary>
        ///     Generates minidump with private read write memory and data segments. This mode allows retreiving of local values
        ///     and the stack
        ///     variables. Dump size is about ~5MB compressed.
        /// </summary>
        Normal,

        /// <summary>
        ///     Generates full application memory dump. This simply dump all memory used by the process. Dump size is about ~100MB
        ///     compressed.
        /// </summary>
        Full
    }
}
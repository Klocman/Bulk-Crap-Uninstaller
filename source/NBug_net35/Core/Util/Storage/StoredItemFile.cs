// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StoredItemFile.cs" company="NBug Project">
//   Copyright (c) 2011 - 2013 Teoman Soygul. Licensed under MIT license.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace NBug.Core.Util.Storage
{
    internal enum StoredItemType
    {
        Exception,

        Report,

        MiniDump
    }

    // This class must remain internal otherwise it should not use constant strings
    internal static class StoredItemFile
    {
        internal const string Exception = "Exception.xml";
        internal const string MiniDump = "MiniDump.mdmp";
        internal const string Report = "Report.xml";
    }
}
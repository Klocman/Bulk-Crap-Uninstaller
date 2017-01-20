/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.Collections.Generic;

namespace UninstallTools.Startup.Normal
{
    internal interface IStartupDisable
    {
        void Disable(StartupEntry startupEntry);
        void Enable(StartupEntry startupEntry);
        bool StillExists(StartupEntry startupEntry);
        IEnumerable<StartupEntry> AddDisableInfo(IList<StartupEntry> existingEntries);

        /// <summary>
        ///     Get backup store path for the link. The backup extension is appended as well.
        ///     Works only for links, file doesn't have to exist.
        /// </summary>
        string GetDisabledEntryPath(StartupEntry startupEntry);
    }
}
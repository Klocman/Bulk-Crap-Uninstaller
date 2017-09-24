/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.Collections.Generic;

namespace UninstallTools.Junk
{
    public interface IJunkCreator
    {
        void Setup(ICollection<ApplicationUninstallerEntry> allUninstallers);
        IEnumerable<IJunkResult> FindJunk(ApplicationUninstallerEntry target);
        string CategoryName { get; }
    }
}
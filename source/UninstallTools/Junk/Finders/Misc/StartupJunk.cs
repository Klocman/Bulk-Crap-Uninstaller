/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.Collections.Generic;
using System.Linq;

namespace UninstallTools.Junk
{
    public sealed class StartupJunk : IJunkCreator
    {
        public void Setup(ICollection<ApplicationUninstallerEntry> allUninstallers)
        {
        }

        public IEnumerable<JunkNode> FindJunk(ApplicationUninstallerEntry target)
        {
            if (target.StartupEntries == null)
                return Enumerable.Empty<JunkNode>();

            return target.StartupEntries.Where(x => x.StillExists())
                .Select(x => new StartupJunkNode(x, target.DisplayName) as JunkNode);
        }
    }
}
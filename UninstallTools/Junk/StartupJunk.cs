/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.Collections.Generic;
using System.Linq;
using UninstallTools.Factory;
using UninstallTools.Properties;

namespace UninstallTools.Junk
{
    public sealed class StartupJunk : JunkBase
    {
        public static readonly ConfidencePart ConfidenceIsRunOnce =
            new ConfidencePart(-5, Localisation.Confidence_Startup_IsRunOnce);

        public static readonly ConfidencePart ConfidenceStartupMatched =
            new ConfidencePart(6, Localisation.Confidence_Startup_StartupMatched);

        public StartupJunk(ApplicationUninstallerEntry entry)
            : base(entry, Enumerable.Empty<ApplicationUninstallerEntry>())
        {
        }

        public override IEnumerable<JunkNode> FindJunk()
        {
            if (Uninstaller.StartupEntries == null)
                return Enumerable.Empty<JunkNode>();

            return Uninstaller.StartupEntries.Where(x => x.StillExists())
                .Select(x => new StartupJunkNode(x, Uninstaller.DisplayName) as JunkNode);
        }
    }
}
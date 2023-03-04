/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.Collections.Generic;
using System.IO;
using Klocman.IO;
using UninstallTools.Properties;

namespace UninstallTools.Factory
{
    /// <summary>
    /// Get uninstallers that were manually pre-defined.
    /// </summary>
    public class PredefinedFactory : IIndependantUninstallerFactory
    {
        public IList<ApplicationUninstallerEntry> GetUninstallerEntries(
            ListGenerationProgress.ListGenerationCallback progressCallback)
        {
            var items = new List<ApplicationUninstallerEntry>();
            return items;
        }

        public bool IsEnabled() => UninstallToolsGlobalConfig.ScanPreDefined;
        public string DisplayName => Localisation.Progress_AppStores_Templates;
    }
}
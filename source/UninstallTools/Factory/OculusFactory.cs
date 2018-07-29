using System;
using System.Collections.Generic;
using System.IO;

namespace UninstallTools.Factory
{
    public class OculusFactory : IUninstallerFactory
    {
        private static string HelperPath => Path.Combine(UninstallToolsGlobalConfig.AssemblyLocation, @"OculusHelper.exe");

        public IEnumerable<ApplicationUninstallerEntry> GetUninstallerEntries(ListGenerationProgress.ListGenerationCallback progressCallback)
        {
            throw new NotImplementedException();
        }
    }
}
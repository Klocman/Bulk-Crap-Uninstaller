using System.Collections.Generic;
using UninstallTools.Uninstaller;

namespace BulkCrapUninstaller.Functions
{
    public sealed class ApplicationInfoExport
    {
        public ApplicationInfoExport(List<ApplicationUninstallerEntry> items)
        {
            Items = items;
        }

        // Needed for serialization
        public ApplicationInfoExport()
        {
        }

        public List<ApplicationUninstallerEntry> Items { get; set; }
    }
}
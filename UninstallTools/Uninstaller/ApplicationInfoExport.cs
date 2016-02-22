using System.Collections.Generic;
using System.Linq;
using Klocman.Tools;

namespace UninstallTools.Uninstaller
{
    public sealed class ApplicationInfoExport
    {
        public static void SerializeApplicationInfo(string filename, IEnumerable<ApplicationUninstallerEntry> items)
        {
            SerializationTools.SerializeToXml(filename, new ApplicationInfoExport(items));
        }

        public ApplicationInfoExport(IEnumerable<ApplicationUninstallerEntry> items)
        {
            Items = items.ToList();
        }

        // Needed for serialization
        public ApplicationInfoExport()
        {
        }

        public List<ApplicationUninstallerEntry> Items { get; set; }
    }
}
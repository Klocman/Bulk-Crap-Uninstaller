/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.Collections.Generic;
using System.Linq;
using Klocman.Tools;
using UninstallTools.Factory;

namespace UninstallTools
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
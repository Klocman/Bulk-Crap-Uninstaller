/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.Collections.Generic;
using System.Linq;
using Klocman.Tools;

namespace UninstallTools
{
    public sealed class ApplicationEntrySerializer
    {
        public static void SerializeApplicationEntries(string filename, IEnumerable<ApplicationUninstallerEntry> items)
        {
            SerializationTools.SerializeToXml(filename, new ApplicationEntrySerializer(items));
        }

        public ApplicationEntrySerializer(IEnumerable<ApplicationUninstallerEntry> items)
        {
            Items = items.ToList();
        }

        // Needed for serialization
        public ApplicationEntrySerializer()
        {
        }

        public List<ApplicationUninstallerEntry> Items { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Klocman.Localising;
using UninstallTools.Uninstaller;

namespace BulkCrapUninstaller.Controls
{
    internal sealed class ClipboardCopyItem
    {
        public static IEnumerable<ClipboardCopyItem> Items { get; }
        private static Func<ApplicationUninstallerEntry, object>[] FormatGetterFuncs { get; }

        static ClipboardCopyItem()
        {
            var results = new List<ClipboardCopyItem>(typeof(ApplicationUninstallerEntry)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Select(p => new ClipboardCopyItem(p.Name, p.GetLocalisedName(), entry => p.GetValue(entry, null)))
                .OrderBy(x => x.FancyName));

            for (var i = 0; i < results.Count; i++)
                results[i].Id = string.Concat("{", i.ToString(), "}");

            Items = results.AsEnumerable();
            FormatGetterFuncs = results.Select(x => x.Getter).ToArray();
        }

        public static string GetStringFromPattern(string pattern, ApplicationUninstallerEntry entry)
        {
            foreach (var clipboardCopyItem in Items)
                pattern = pattern.Replace(clipboardCopyItem.Name, clipboardCopyItem.Id.ToString());
            return string.Format(pattern, FormatGetterFuncs.Select(x => x(entry)).ToArray());
        }

        private ClipboardCopyItem(string name, string fancyName, Func<ApplicationUninstallerEntry, object> getter)
        {
            Getter = getter;
            FancyName = fancyName;
            Name = string.Concat("{", name, "}");
        }

        public Func<ApplicationUninstallerEntry, object> Getter { get; }
        public string Name { get; }
        public string FancyName { get; }
        public string Id { get; private set; }

        public override string ToString()
        {
            return Name + " - " + FancyName;
        }
    }
}
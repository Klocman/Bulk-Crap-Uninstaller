/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Klocman.Localising;
using UninstallTools.Uninstaller;

namespace BulkCrapUninstaller.Controls
{
    internal sealed class ClipboardCopyItem
    {
        static ClipboardCopyItem()
        {
            var results = new List<ClipboardCopyItem>(typeof (ApplicationUninstallerEntry)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Select(p => new ClipboardCopyItem(p.Name, p.GetLocalisedName(), entry => p.GetValue(entry, null)))
                .OrderBy(x => x.FancyName));

            for (var i = 0; i < results.Count; i++)
                results[i].Id = string.Concat("{", i.ToString(CultureInfo.CurrentCulture), "}");

            Items = results.AsEnumerable();
            FormatGetterFuncs = results.Select(x => x.Getter).ToArray();
        }

        private ClipboardCopyItem(string name, string fancyName, Func<ApplicationUninstallerEntry, object> getter)
        {
            Getter = getter;
            FancyName = fancyName;
            Name = string.Concat("{", name, "}");
        }

        public static IEnumerable<ClipboardCopyItem> Items { get; }
        private static Func<ApplicationUninstallerEntry, object>[] FormatGetterFuncs { get; }

        public Func<ApplicationUninstallerEntry, object> Getter { get; }
        public string Name { get; }
        public string FancyName { get; }
        public string Id { get; private set; }

        public static string GetStringFromPattern(string pattern, ApplicationUninstallerEntry entry)
        {
            return string.Format(CultureInfo.CurrentCulture, Items.Aggregate(pattern,
                (current, clipboardCopyItem) => current.Replace(clipboardCopyItem.Name, clipboardCopyItem.Id)),
                FormatGetterFuncs.Select(x => x(entry)).ToArray());
        }

        public override string ToString()
        {
            return Name + " - " + FancyName;
        }
    }
}
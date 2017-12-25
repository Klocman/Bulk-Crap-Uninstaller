/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Klocman.Extensions;
using Klocman.Forms.Tools;
using UninstallTools;

namespace BulkCrapUninstaller.Functions.Tools
{
    internal static class ImportExport
    {
        public static void CopyToClipboard(IEnumerable<string> inputLines)
        {
            var text = string.Join("\r\n", inputLines.OrderBy(t => t).ToArray());

            if (text.IsNotEmpty())
            {
                try
                {
                    Clipboard.SetText(text);
                }
                catch (Exception ex)
                {
                    PremadeDialogs.GenericError(ex);
                }
            }
            else
                MessageBoxes.NothingToCopy();
        }

        public static void CopyNamesToClipboard(IEnumerable<ApplicationUninstallerEntry> items)
        {
            CopyToClipboard(items.Select(z => z.DisplayName));
        }

        public static void CopyRegKeysToClipboard(IEnumerable<ApplicationUninstallerEntry> items)
        {
            CopyToClipboard(items.Select(z => z.RegistryPath));
        }

        public static void CopyUninstallStringsToClipboard(IEnumerable<ApplicationUninstallerEntry> items)
        {
            CopyToClipboard(items.Select(z => z.UninstallString));
        }

        public static void CopyGuidsToClipboard(IEnumerable<ApplicationUninstallerEntry> items)
        {
            CopyToClipboard(items.Select(z => z.BundleProviderKey.ToString("B").ToUpperInvariant()));
        }

        public static void CopyFullInformationToClipboard(IEnumerable<ApplicationUninstallerEntry> items)
        {
            CopyToClipboard(items.Select(z => z.ToLongString()));
        }
    }
}
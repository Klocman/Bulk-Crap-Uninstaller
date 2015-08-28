using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Klocman.Extensions;
using Klocman.Forms.Tools;

namespace BulkCrapUninstaller.Functions
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
    }
}
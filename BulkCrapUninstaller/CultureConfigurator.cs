using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using BulkCrapUninstaller.Properties;
using Klocman.Extensions;
using Klocman.Tools;

namespace BulkCrapUninstaller
{
    public static class CultureConfigurator
    {
        public static IEnumerable<CultureInfo> SupportedLanguages => new[]
        {
            CultureInfo.GetCultureInfo("en-US"),
            CultureInfo.GetCultureInfo("en-GB"),
            CultureInfo.GetCultureInfo("pl-PL"),
            CultureInfo.GetCultureInfo("fr-FR"),
            CultureInfo.GetCultureInfo("de-DE"),
            CultureInfo.GetCultureInfo("cs-CZ")
        };

        public static void SetupCulture()
        {
            var currentCulture = CultureInfo.CurrentCulture;

            var targetLocale = Settings.Default.Language;
            if (targetLocale.IsNotEmpty())
            {
                try
                {
                    currentCulture = CultureInfo.GetCultureInfo(targetLocale);
                }
                catch
                {
                    Settings.Default.Language = string.Empty;
                }
            }

            if (
                !currentCulture.Name.ContainsAny(SupportedLanguages.Select(x => x.Parent.Name),
                    StringComparison.OrdinalIgnoreCase))
                currentCulture = SupportedLanguages.First();

            ProcessTools.SetDefaultCulture(currentCulture);
            var thread = Thread.CurrentThread;
            thread.CurrentCulture = currentCulture;
            thread.CurrentUICulture = currentCulture;
        }
    }
}
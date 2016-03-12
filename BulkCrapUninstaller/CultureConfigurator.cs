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
        private static IEnumerable<CultureInfo> _supportedLanguages;
        private static CultureInfo _enUsCulture;
        private static CultureInfo EnUsCulture => _enUsCulture ?? (_enUsCulture = CultureInfo.GetCultureInfo("en-US"));

        public static IEnumerable<CultureInfo> SupportedLanguages
        {
            get
            {
                return _supportedLanguages ?? (_supportedLanguages = new[]
                {
                    EnUsCulture,
                    CultureInfo.GetCultureInfo("en-GB"),
                    CultureInfo.GetCultureInfo("cs-CZ"),
                    CultureInfo.GetCultureInfo("de-DE"),
                    CultureInfo.GetCultureInfo("fr-FR"),
                    CultureInfo.GetCultureInfo("pl-PL"),
                    CultureInfo.GetCultureInfo("ru-RU"),
                    CultureInfo.GetCultureInfo("sl-SI")
                }.OrderBy(x => x.DisplayName).ToList().AsEnumerable());
            }
        }

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

            if (!currentCulture.Name.ContainsAny(SupportedLanguages.Select(x => x.Parent.Name),
                StringComparison.OrdinalIgnoreCase))
                currentCulture = EnUsCulture;

            ProcessTools.SetDefaultCulture(currentCulture);
            var thread = Thread.CurrentThread;
            thread.CurrentCulture = currentCulture;
            thread.CurrentUICulture = currentCulture;
        }
    }
}
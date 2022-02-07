/*
    Copyright (c) 2018 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using Klocman.Tools;
using UninstallTools.Properties;

namespace UninstallTools.Factory
{
    public class ScriptFactory : IIndependantUninstallerFactory
    {
        private static readonly string ScriptHelperPath;
        private static readonly PropertyInfo[] EntryProps;
        private static readonly PropertyInfo[] SystemIconProps;

        static ScriptFactory()
        {
            var helper = Path.Combine(UninstallToolsGlobalConfig.AssemblyLocation, @"ScriptHelper.exe");
            var frameworkVersion = WindowsTools.CheckNetFramework4Installed(true);
            if (File.Exists(helper) && frameworkVersion != null && frameworkVersion >= new Version(4, 5, 0))
                ScriptHelperPath = helper;

            EntryProps = typeof(ApplicationUninstallerEntry)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.CanWrite && p.PropertyType == typeof(string))
                .ToArray();

            SystemIconProps = typeof(SystemIcons)
                .GetProperties(BindingFlags.Static | BindingFlags.Public)
                .Where(p => p.CanRead)
                .ToArray();
        }

        public IList<ApplicationUninstallerEntry> GetUninstallerEntries(
            ListGenerationProgress.ListGenerationCallback progressCallback)
        {
            var results = new List<ApplicationUninstallerEntry>();
            if (ScriptHelperPath == null) return results;

            var result = FactoryTools.StartHelperAndReadOutput(ScriptHelperPath, "list");

            if (string.IsNullOrEmpty(result)) return results;

            var dataSets = FactoryTools.ExtractAppDataSetsFromHelperOutput(result);

            foreach (var dataSet in dataSets)
            {
                var entry = new ApplicationUninstallerEntry();

                // Automatically fill in any supplied static properties
                foreach (var entryProp in EntryProps)
                {
                    if (!dataSet.TryGetValue(entryProp.Name, out var item) || string.IsNullOrEmpty(item))
                        continue;

                    try
                    {
                        entryProp.SetValue(entry, item, null);
                    }
                    catch (SystemException ex)
                    {
                        Console.WriteLine(ex);
                    }
                }

                if (!entry.UninstallPossible && !entry.QuietUninstallPossible) continue;

                if (string.IsNullOrEmpty(entry.Publisher))
                    entry.Publisher = "Script";

                //if (dataSet.TryGetValue("SystemIcon", out var icon) && !string.IsNullOrEmpty(icon))
                //{
                //    var iconObj = SystemIconProps
                //        .FirstOrDefault(p => p.Name.Equals(icon, StringComparison.OrdinalIgnoreCase))
                //        ?.GetValue(null, null) as Icon;
                //    entry.IconBitmap = iconObj;
                //}
                entry.IconBitmap = SystemIcons.Shield;

                results.Add(entry);
            }

            return results;
        }

        public bool IsEnabled() => UninstallToolsGlobalConfig.ScanPreDefined;
        public string DisplayName => Localisation.Progress_AppStores_Templates;
    }
}
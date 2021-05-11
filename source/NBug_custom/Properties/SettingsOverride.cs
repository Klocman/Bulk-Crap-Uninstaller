// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettingsOverride.cs" company="NBug Project">
//   Copyright (c) 2011 - 2013 Teoman Soygul. Licensed under MIT license.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace NBug.Properties
{
    public static class SettingsOverride
    {
        /// <summary>
        ///     Gets or sets a value indicating whether the library settings are to be overridden. If the settings are to be
        ///     overridden by code
        ///     or by some other library, it shall be done before any of the static members of the <see cref="NBug.Settings" />
        ///     class is accessed.
        ///     Note that setting this to true prevents default settings from getting loaded so should be used with caution.
        /// </summary>
        internal static bool Overridden { get; set; }

        /// <summary>
        ///     Loads custom settings file from the designated stream. Before calling this, you must set
        ///     <c>SettingsOverride.Overridden = true;</c>
        ///     or the default settings will be loaded as the static constructor gets called otherwise.
        /// </summary>
        /// <param name="settingsFile">Stream to load the settings from.</param>
        public static void LoadCustomSettings(Stream settingsFile)
        {
            Overridden = true;

            try
            {
                NBug.Settings.LoadCustomSettings(XElement.Load(XmlReader.Create(settingsFile)));
            }
            catch (XmlException)
            {
                // Root element is missing so recreate the configuration file
                NBug.Settings.LoadCustomSettings(
                    XElement.Parse("<?xml version=\"1.0\" encoding=\"utf-8\" ?><configuration></configuration>"));
            }
        }

        /// <summary>
        ///     Loads custom settings file from the designated file. Before calling this, you must set
        ///     <c>SettingsOverride.Overridden = true;</c>
        ///     or the default settings will be loaded as the static constructor gets called otherwise.
        /// </summary>
        /// <param name="settingsFilePath">File to load the settings from. Used within <c>XElement.Load(path)</c></param>
        public static void LoadCustomSettings(string settingsFilePath)
        {
            Overridden = true;

            try
            {
                NBug.Settings.LoadCustomSettings(XElement.Load(settingsFilePath));
            }
            catch (XmlException)
            {
                // Root element is missing so recreate the configuration file
                NBug.Settings.LoadCustomSettings(
                    XElement.Parse("<?xml version=\"1.0\" encoding=\"utf-8\" ?><configuration></configuration>"));
            }
        }

        public static void SaveCustomSettings(Stream settingsFile, bool encryptConnectionStrings)
        {
            NBug.Settings.SaveCustomSettings(settingsFile, encryptConnectionStrings);
        }

        public static void SaveCustomSettings(Stream settingsFile)
        {
            NBug.Settings.SaveCustomSettings(settingsFile, false);
        }
    }
}
/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Diagnostics;
using System.Linq;
using Klocman.Tools;

namespace UninstallTools.Factory.InfoAdders
{
    public class ExecutableAttributeExtractor : IMissingInfoAdder
    {
        public void AddMissingInformation(ApplicationUninstallerEntry target)
        {
            if (target.SortedExecutables == null || !target.SortedExecutables.Any())
                return;

            FillInformationFromFileAttribs(target, target.SortedExecutables[0], true);
        }

        public string[] RequiredValueNames { get; } = {
            nameof(ApplicationUninstallerEntry.SortedExecutables)
        };
        public bool RequiresAllValues { get; } = true;
        public bool AlwaysRun { get; } = false;

        public string[] CanProduceValueNames { get; } = {
            nameof(ApplicationUninstallerEntry.RawDisplayName),
            nameof(ApplicationUninstallerEntry.DisplayVersion),
            nameof(ApplicationUninstallerEntry.Publisher)
            //nameof(ApplicationUninstallerEntry.Comment)
        };
        public InfoAdderPriority Priority { get; } = InfoAdderPriority.RunLast;


        /// <summary>
        /// Add information from FileVersionInfo of specified file to the targetEntry
        /// </summary>
        /// <param name="targetEntry">Entry to update</param>
        /// <param name="infoSourceFilename">Binary file to get the information from</param>
        /// <param name="onlyUnpopulated">Only update unpopulated fields of the targetEntry</param>
        internal static void FillInformationFromFileAttribs(ApplicationUninstallerEntry targetEntry, string infoSourceFilename, bool onlyUnpopulated)
        {
            FileVersionInfo verInfo;
            try
            {
                verInfo = FileVersionInfo.GetVersionInfo(infoSourceFilename);
            }
            catch
            {
                return;
            }

            Func<string, bool> unpopulatedCheck;
            if (onlyUnpopulated)
                unpopulatedCheck = target => string.IsNullOrEmpty(target?.Trim());
            else
                unpopulatedCheck = target => true;

            var companyName = verInfo.CompanyName?.Trim();
            if (unpopulatedCheck(targetEntry.Publisher) && !string.IsNullOrEmpty(companyName))
                targetEntry.Publisher = companyName;

            if (unpopulatedCheck(targetEntry.RawDisplayName))
            {
                var fileDescription = StringTools.StripStringFromVersionNumber(verInfo.FileDescription?.Trim());
                var productName = verInfo.ProductName?.Trim();
                if (!string.IsNullOrEmpty(fileDescription))
                {
                    if (!string.IsNullOrEmpty(productName) && productName.Length > fileDescription.Length)
                        targetEntry.RawDisplayName = productName;
                    else
                        targetEntry.RawDisplayName = fileDescription;
                }
                else if (!string.IsNullOrEmpty(productName))
                    targetEntry.RawDisplayName = productName;
            }

            var comment = verInfo.Comments?.Trim();
            if (unpopulatedCheck(targetEntry.Comment) && !string.IsNullOrEmpty(comment))
                targetEntry.Comment = comment;

            if (unpopulatedCheck(targetEntry.DisplayVersion))
            {
                var productVersion = verInfo.ProductVersion?.Trim();
                if (string.IsNullOrEmpty(productVersion))
                    productVersion = verInfo.FileVersion?.Trim();

                if (!string.IsNullOrEmpty(productVersion))
                    targetEntry.DisplayVersion = ApplicationEntryTools.CleanupDisplayVersion(productVersion);
            }
        }
    }
}
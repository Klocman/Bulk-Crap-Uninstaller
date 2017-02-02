/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Diagnostics;

namespace UninstallTools.Factory.InfoAdders
{
    public class FileAttribInfoAdder //: IMissingInfoAdder
    {//TODO add missing attributes using the entry.getexecandidates
        public void AddMissingInformation(ApplicationUninstallerEntry target)
        {
            throw new NotImplementedException();
        }

        public string[] RequiredValueNames { get; }
        public bool RequiresAllValues { get; }
        public string[] CanProduceValueNames { get; }
        public InfoAdderPriority Priority { get; }

        /// <summary>
        /// Add information from FileVersionInfo of specified file to the targetEntry
        /// </summary>
        /// <param name="targetEntry">Entry to update</param>
        /// <param name="infoSourceFilename">Binary file to get the information from</param>
        /// <param name="onlyUnpopulated">Only update unpopulated fields of the targetEntry</param>
        public static void FillInformationFromFileAttribs(ApplicationUninstallerEntry targetEntry, string infoSourceFilename, bool onlyUnpopulated)
        {
            var verInfo = FileVersionInfo.GetVersionInfo(infoSourceFilename);

            Func<string, bool> unpopulatedCheck;
            if (onlyUnpopulated) unpopulatedCheck = target => String.IsNullOrEmpty(target?.Trim());
            else unpopulatedCheck = target => true;

            if (unpopulatedCheck(targetEntry.Publisher) && !String.IsNullOrEmpty(verInfo.CompanyName?.Trim()))
                targetEntry.Publisher = verInfo.CompanyName.Trim();

            if (unpopulatedCheck(targetEntry.RawDisplayName) && !String.IsNullOrEmpty(verInfo.ProductName?.Trim()))
                targetEntry.RawDisplayName = verInfo.ProductName.Trim();

            if (unpopulatedCheck(targetEntry.Comment) && !String.IsNullOrEmpty(verInfo.Comments?.Trim()))
                targetEntry.Comment = verInfo.Comments.Trim();

            if (unpopulatedCheck(targetEntry.DisplayVersion))
            {
                if (!String.IsNullOrEmpty(verInfo.ProductVersion?.Trim()))
                    targetEntry.DisplayVersion = verInfo.ProductVersion.Trim();
                else if (!String.IsNullOrEmpty(verInfo.FileVersion?.Trim()))
                    targetEntry.DisplayVersion = verInfo.FileVersion.Trim();
            }
        }
    }
}
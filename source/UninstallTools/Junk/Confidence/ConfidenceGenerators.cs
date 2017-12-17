/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using Klocman.Tools;

namespace UninstallTools.Junk.Confidence
{
    public static class ConfidenceGenerators
    {
        public static IEnumerable<ConfidenceRecord> GenerateConfidence(string itemName, ApplicationUninstallerEntry applicationUninstallerEntry)
        {
            return GenerateConfidence(itemName, null, 0, applicationUninstallerEntry);
        }

        internal static IEnumerable<ConfidenceRecord> GenerateConfidence(string itemName, string itemParentPath, int level, ApplicationUninstallerEntry applicationUninstallerEntry)
        {
            var matchResult = MatchStringToProductName(applicationUninstallerEntry, itemName);

            return GenerateConfidence(itemName, matchResult, itemParentPath, level, applicationUninstallerEntry);
        }

        internal static IEnumerable<ConfidenceRecord> GenerateConfidence(string itemName, int similarityToEntry, string itemParentPath, int level,
            ApplicationUninstallerEntry applicationUninstallerEntry)
        {
            if (similarityToEntry < 0)
                yield break;

            yield return similarityToEntry < 2
                ? ConfidenceRecords.ProductNamePerfectMatch
                : ConfidenceRecords.ProductNameDodgyMatch;

            // Base rating according to path depth. 0 is best
            yield return new ConfidenceRecord(2 - Math.Abs(level) * 2);

            if (ItemNameEqualsCompanyName(applicationUninstallerEntry, itemName))
                yield return ConfidenceRecords.ItemNameEqualsCompanyName;

            if (level > 0)
            {
                if (applicationUninstallerEntry.PublisherTrimmed.ToLowerInvariant()
                    .Contains(PathTools.GetName(itemParentPath).Replace('_', ' ').ToLowerInvariant()))
                    yield return ConfidenceRecords.CompanyNameMatch;
            }
        }

        /// <summary>
        /// -1 if match failed, 0 if string matched perfectly, higher if match was worse
        /// </summary>
        internal static int MatchStringToProductName(ApplicationUninstallerEntry applicationUninstallerEntry, string str)
        {
            var productName = applicationUninstallerEntry.DisplayNameTrimmed.ToLowerInvariant();
            str = str.Replace('_', ' ').ToLowerInvariant().Trim();
            var lowestLength = Math.Min(productName.Length, str.Length);

            // Don't match short strings
            if (lowestLength <= 4)
                return -1;

            var result = Sift4.SimplestDistance(productName, str, 1);

            // Strings match perfectly
            if (result <= 1)
                return result;

            // If the product name contains company name, try trimming it and testing again
            var publisher = applicationUninstallerEntry.PublisherTrimmed.ToLower();
            if (publisher.Length > 4 && productName.Contains(publisher))
            {
                var trimmedProductName = productName.Replace(publisher, "").Trim();
                if (trimmedProductName.Length <= 4)
                    return -1;

                var trimmedResult = Sift4.SimplestDistance(trimmedProductName, str, 1);

                if (trimmedResult <= 1)
                    return trimmedResult;
            }

            var dirToName = str.Contains(productName);
            var nameToDir = productName.Contains(str);

            if (dirToName || nameToDir)
                return 2;

            // Hard cut-off if the difference is more than a third of the checked name
            if (result < lowestLength / 3)
                return result;

            return -1;
        }

        // Check if name is the same as publisher, could be "Adobe AIR" getting matched to a folder "Adobe"
        internal static bool ItemNameEqualsCompanyName(ApplicationUninstallerEntry applicationUninstallerEntry, string itemName)
        {
            var publisher = applicationUninstallerEntry.PublisherTrimmed.ToLowerInvariant();
            itemName = itemName.ToLowerInvariant();
            return !publisher.Equals(applicationUninstallerEntry.DisplayNameTrimmed.ToLowerInvariant()) && publisher.Contains(itemName);
        }
    }
}
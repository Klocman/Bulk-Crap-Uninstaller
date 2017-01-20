/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using Klocman.Tools;
using UninstallTools.Uninstaller;

namespace UninstallTools.Junk
{
    public abstract class JunkBase : IJunkCreator
    {
        protected JunkBase(ApplicationUninstallerEntry entry, IEnumerable<ApplicationUninstallerEntry> otherUninstallers)
        {
            Uninstaller = entry;
            OtherUninstallers = otherUninstallers;
        }

        public ApplicationUninstallerEntry Uninstaller { get; }
        internal IEnumerable<ApplicationUninstallerEntry> OtherUninstallers { get; set; }
        public abstract IEnumerable<JunkNode> FindJunk();

        public virtual IEnumerable<ConfidencePart> GenerateConfidence(string itemName, string itemParentPath, int level)
        {
            var returnValue = new List<ConfidencePart>();

            var matchResult = MatchStringToProductName(itemName);

            if (matchResult < 0)
                return returnValue;

            returnValue.Add(matchResult < 2
                ? ConfidencePart.ProductNamePerfectMatch
                : ConfidencePart.ProductNameDodgyMatch);

            // Base rating according to path depth. 0 is best
            returnValue.Add(new ConfidencePart(2 - Math.Abs(level) * 2));

            if (ItemNameEqualsCompanyName(itemName))
                returnValue.Add(ConfidencePart.ItemNameEqualsCompanyName);

            if (level > 0)
            {
                if (Uninstaller.PublisherTrimmed.ToLowerInvariant()
                    .Contains(PathTools.GetName(itemParentPath).Replace('_', ' ').ToLowerInvariant()))
                    returnValue.Add(ConfidencePart.CompanyNameMatch);
            }

            return returnValue;
        }

        protected int MatchStringToProductName(string str)
        {
            var productName = Uninstaller.DisplayNameTrimmed.ToLowerInvariant();
            str = str.Replace('_', ' ').ToLowerInvariant().Trim();
            var lowestLength = Math.Min(productName.Length, str.Length);

            // Don't match short strings
            if (lowestLength <= 4)
                return -1;

            int result = StringTools.CompareSimilarity(productName, str);

            // Strings match perfectly
            if (result <= 1)
                return result;

            // If the product name contains company name, try trimming it and testing again
            var publisher = Uninstaller.PublisherTrimmed.ToLower();
            if (publisher.Length > 4 && productName.Contains(publisher))
            {
                var trimmedProductName = productName.Replace(publisher, "").Trim();
                if (trimmedProductName.Length <= 4)
                    return -1;

                var trimmedResult = StringTools.CompareSimilarity(trimmedProductName, str);

                if (trimmedResult <= 1)
                    return trimmedResult;
            }

            var dirToName = str.Contains(productName);
            var nameToDir = productName.Contains(str);

            if (dirToName || nameToDir)
                return 2;

            if (result < lowestLength / 3)
                return result;

            return -1;
        }

        // Check if name is the same as publisher, could be "Adobe AIR" getting matched to a folder "Adobe"
        internal bool ItemNameEqualsCompanyName(string itemName)
        {
            var publisher = Uninstaller.PublisherTrimmed.ToLowerInvariant();
            itemName = itemName.ToLowerInvariant();
            return !publisher.Equals(Uninstaller.DisplayNameTrimmed.ToLowerInvariant()) && publisher.Contains(itemName);
        }
    }
}
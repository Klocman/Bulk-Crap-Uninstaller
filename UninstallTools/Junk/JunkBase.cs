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

        public virtual IEnumerable<ConfidencePart> GenerateConfidence(string itemName, string itemParentPath, int level,
            bool skipNameCheck)
        {
            var returnValue = new List<ConfidencePart>();

            itemName = itemName.Replace('_', ' ');

            var dirToName = itemName.Contains(Uninstaller.DisplayNameTrimmed);
            var nameToDir = Uninstaller.DisplayNameTrimmed.Contains(itemName);

            // Check if minimum requirements are met
            if (skipNameCheck || (itemName.Length > 4 && (dirToName || nameToDir)))
            {
                // Base rating according to path depth
                returnValue.Add(new ConfidencePart(2 - level*2));

                // Chack if name fits perfectly
                if (dirToName && nameToDir)
                {
                    returnValue.Add(ConfidencePart.ProductNamePerfectMatch);
                }
                else
                {
                    returnValue.Add(ConfidencePart.ProductNameDodgyMatch);
                }

                if (ItemNameEqualsCompanyName(itemName))
                    returnValue.Add(ConfidencePart.ItemNameEqualsCompanyName);

                if (level > 0)
                {
                    if (Uninstaller.PublisherTrimmed.Contains(PathTools.GetName(itemParentPath).Replace('_', ' ')))
                    {
                        returnValue.Add(ConfidencePart.CompanyNameMatch);
                    }
                }
            }

            return returnValue;
        }

        // Check if name is the same as publisher, could be "Adobe AIR" getting matched to a folder "Adobe"
        internal bool ItemNameEqualsCompanyName(string itemName)
        {
            var publisher = Uninstaller.PublisherTrimmed;
            return !publisher.Equals(Uninstaller.DisplayNameTrimmed) && publisher.Contains(itemName);
        }
    }
}
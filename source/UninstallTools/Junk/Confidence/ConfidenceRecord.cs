/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using UninstallTools.Properties;

namespace UninstallTools.Junk.Confidence
{
    public sealed class ConfidenceRecord
    {
        public static readonly ConfidenceRecord CompanyNameDidNotMatch =
            new ConfidenceRecord(-2, Localisation.ConfidencePart_CompanyNameDidNotMatch);

        public static readonly ConfidenceRecord CompanyNameMatch =
            new ConfidenceRecord(4, Localisation.ConfidencePart_CompanyNameMatch);

        public static readonly ConfidenceRecord AllSubdirsMatched =
            new ConfidenceRecord(4, Localisation.ConfidencePart_AllSubdirsMatched);

        public static readonly ConfidenceRecord DirectoryStillUsed =
            new ConfidenceRecord(-7, Localisation.ConfidencePart_DirectoryStillUsed);

        public static readonly ConfidenceRecord ExplicitConnection =
            new ConfidenceRecord(4, Localisation.ConfidencePart_ExplicitConnection);

        public static readonly ConfidenceRecord ItemNameEqualsCompanyName =
            new ConfidenceRecord(-2, Localisation.ConfidencePart_ItemNameEqualsCompanyName);

        public static readonly ConfidenceRecord ProductNameDodgyMatch =
            new ConfidenceRecord(-2, Localisation.ConfidencePart_ProductNameDodgyMatch);

        public static readonly ConfidenceRecord ProductNamePerfectMatch =
            new ConfidenceRecord(2, Localisation.ConfidencePart_ProductNamePerfectMatch);

        public static readonly ConfidenceRecord QuestionableDirectoryName =
            new ConfidenceRecord(-3, Localisation.ConfidencePart_QuestionableDirectoryName);

        public static readonly ConfidenceRecord IsUninstallerRegistryKey =
            new ConfidenceRecord(20, Localisation.ConfidencePart_IsUninstallerRegistryKey);

        public static readonly ConfidenceRecord IsStoreApp =
            new ConfidenceRecord(-10, Localisation.ConfidencePart_IsStoreApp);

        public ConfidenceRecord(int change, string reason)
        {
            Change = change;
            Reason = reason;
        }

        public ConfidenceRecord(int change)
        {
            Change = change;
        }

        public int Change { get; }
        public string Reason { get; }

        public override bool Equals(object obj)
        {
            var casted = obj as ConfidenceRecord;
            if (casted == null)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            return (casted.Change == Change) && (casted.Reason == Reason);
        }

        public override int GetHashCode()
        {
            return Change.GetHashCode() ^ Reason.GetHashCode();
        }
    }
}
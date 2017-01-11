using UninstallTools.Properties;

namespace UninstallTools.Junk
{
    public sealed class ConfidencePart
    {
        public static readonly ConfidencePart CompanyNameDidNotMatch = new ConfidencePart(-2,
            Localisation.ConfidencePart_CompanyNameDidNotMatch);

        public static readonly ConfidencePart CompanyNameMatch = new ConfidencePart(4,
            Localisation.ConfidencePart_CompanyNameMatch);

        public static readonly ConfidencePart AllSubdirsMatched = new ConfidencePart(4,
            Localisation.ConfidencePart_AllSubdirsMatched);

        public static readonly ConfidencePart DirectoryStillUsed = new ConfidencePart(-7,
            Localisation.ConfidencePart_DirectoryStillUsed);

        public static readonly ConfidencePart ExplicitConnection = new ConfidencePart(4,
            Localisation.ConfidencePart_ExplicitConnection);

        public static readonly ConfidencePart ItemNameEqualsCompanyName = new ConfidencePart(-2,
            Localisation.ConfidencePart_ItemNameEqualsCompanyName);

        public static readonly ConfidencePart ProductNameDodgyMatch = new ConfidencePart(-2,
            Localisation.ConfidencePart_ProductNameDodgyMatch);

        public static readonly ConfidencePart ProductNamePerfectMatch = new ConfidencePart(2,
            Localisation.ConfidencePart_ProductNamePerfectMatch);

        public static readonly ConfidencePart QuestionableDirectoryName = new ConfidencePart(-3,
            Localisation.ConfidencePart_QuestionableDirectoryName);

        public static readonly ConfidencePart IsUninstallerRegistryKey = new ConfidencePart(20,
            Localisation.ConfidencePart_IsUninstallerRegistryKey);

        public static readonly ConfidencePart IsStoreApp = new ConfidencePart(-10,
            Localisation.ConfidencePart_IsStoreApp);

        public ConfidencePart(int change, string reason)
        {
            Change = change;
            Reason = reason;
        }

        public ConfidencePart(int change)
        {
            Change = change;
        }

        public int Change { get; }
        public string Reason { get; }

        public override int GetHashCode()
        {
            return Change.GetHashCode() ^ Reason.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var casted = obj as ConfidencePart;
            if (casted == null)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            return (casted.Change == Change) && (casted.Reason == Reason);
        }
    }
}
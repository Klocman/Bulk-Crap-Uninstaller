/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using UninstallTools.Properties;

namespace UninstallTools.Junk.Confidence
{
    /// <summary>
    /// Universal confidence pieces
    /// </summary>
    public sealed class ConfidenceRecords
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

        public static readonly ConfidenceRecord IsEmptyFolder =
            new ConfidenceRecord(4, Localisation.Confidence_PF_EmptyFolder);

        public static readonly ConfidenceRecord ExecutablesArePresent =
            new ConfidenceRecord(-4, Localisation.Confidence_PF_ExecsPresent);

        public static readonly ConfidenceRecord FilesArePresent =
            new ConfidenceRecord(0, Localisation.Confidence_PF_FilesPresent);

        public static readonly ConfidenceRecord ManyFilesArePresent =
            new ConfidenceRecord(-2, Localisation.Confidence_PF_ManyFilesPresent);

        public static readonly ConfidenceRecord ProgramNameIsStillUsed =
            new ConfidenceRecord(-4, Localisation.Confidence_PF_NameIsUsed);

        public static readonly ConfidenceRecord FolderHasNoSubdirectories =
            new ConfidenceRecord(2, Localisation.Confidence_PF_NoSubdirs);

        public static readonly ConfidenceRecord PublisherIsStillUsed =
            new ConfidenceRecord(-4, Localisation.Confidence_PF_PublisherIsUsed);
    }
}
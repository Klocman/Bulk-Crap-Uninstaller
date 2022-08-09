/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using UninstallTools.Junk.Confidence;

namespace UninstallTools.Junk.Containers
{
    public interface IJunkResult
    {
        /// <summary>
        ///     Confidence that this entry is safe to remove
        /// </summary>
        ConfidenceCollection Confidence { get; }

        /// <summary>
        /// Create this item's backup inside of the supplied directory
        /// </summary>
        void Backup(string backupDirectory);

        /// <summary>
        ///     Delete this entry permanently
        /// </summary>
        void Delete();

        /// <summary>
        ///     Origin of this junk
        /// </summary>
        IJunkCreator Source { get; }

        /// <summary>
        ///     Uninstaller this entry belongs to
        /// </summary>
        ApplicationUninstallerEntry Application { get; }

        string GetDisplayName();

        /// <summary>
        ///     Preview item in an external application
        /// </summary>
        void Open();

        /// <summary>
        ///     Get extended information with overall confidence information.
        /// </summary>
        string ToLongString();
    }
}
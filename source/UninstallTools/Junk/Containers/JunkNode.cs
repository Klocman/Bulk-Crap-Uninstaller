/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.IO;
using System.Security.Permissions;
using Klocman.Tools;
using UninstallTools.Properties;

namespace UninstallTools.Junk
{
    public abstract class JunkResultBase : IJunkResult
    {
        protected JunkResultBase(ApplicationUninstallerEntry application, IJunkCreator source) : this(application, source, new ConfidenceCollection())
        {
        }

        protected JunkResultBase(ApplicationUninstallerEntry application, IJunkCreator source, ConfidenceCollection confidence)
        {
            Application = application;
            Source = source;
            Confidence = confidence;
        }

        public ConfidenceCollection Confidence { get; }
        public IJunkCreator Source { get; }
        public ApplicationUninstallerEntry Application { get; }

        public abstract void Backup(string backupDirectory);
        public abstract void Delete();
        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        public abstract void Open();

        public abstract string GetDisplayName();
        public virtual string ToLongString()
        {
            return
                $@"{Application} - {Localisation.JunkRemover_Confidence}: {Confidence.GetConfidence()} - {
                    GetDisplayName()}";
        }

        /// <summary>
        /// Prepare a backup directory in the specified parent folder, and return it.
        /// </summary>
        protected string CreateBackupDirectory(string parent)
        {
            var p = Path.Combine(parent, PathTools.SanitizeFileName(Application.DisplayName));
            Directory.CreateDirectory(p);
            return p;
        }
    }

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
        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        void Open();

        /// <summary>
        ///     Get extended information with overall confidence information.
        /// </summary>
        string ToLongString();
    }

    public abstract class JunkNode
    {
        protected JunkNode()
        {
            Confidence = new ConfidenceCollection();
        }

        protected JunkNode(string parentPath, string name, string uninstallerName)
        {
            Name = name;
            ParentPath = parentPath;
            UninstallerName = uninstallerName;

            Confidence = new ConfidenceCollection();
        }

        /// <summary>
        ///     Name of the uninstaller this entry belongs to
        /// </summary>
        public string UninstallerName { get; internal set; }

        /// <summary>
        ///     Name of this junk entry
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        ///     Path to the parent of this junk entry
        /// </summary>
        public string ParentPath { get; internal set; }

        /// <summary>
        ///     Fancy name of this junk kind
        /// </summary>
        public abstract string GroupName { get; }

        /// <summary>
        ///     Full name of this entry. Combined ParentPath and Name.
        /// </summary>
        public string FullName => ParentPath != null && Name != null ? ParentPath.TrimEnd('\\') + '\\' + Name : null;

        /// <summary>
        ///     Confidence that this entry is safe to remove
        /// </summary>
        public ConfidenceCollection Confidence { get; internal set; }

        /// <summary>
        /// Create this item's backup inside of the supplied directory
        /// </summary>
        public abstract void Backup(string backupDirectory);

        /// <summary>
        ///     Delete this entry permanently
        /// </summary>
        public abstract void Delete();

        /// <summary>
        ///     Preview item in an external application
        /// </summary>
        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        public abstract void Open();

        /// <summary>
        ///     Get full name of this entry
        /// </summary>
        public override string ToString()
        {
            return FullName ?? Name ?? base.ToString();
        }

        /// <summary>
        ///     Get extended information with overall confidence information.
        /// </summary>
        public virtual string ToLongString()
        {
            return
                $@"{UninstallerName} - {Localisation.JunkRemover_Confidence}: {Confidence.GetConfidence()} - {FullName}";
        }

        /// <summary>
        /// Prepare a backup directory in the specified parent folder, and return it.
        /// </summary>
        protected string CreateBackupDirectory(string parent)
        {
            var p = Path.Combine(parent, PathTools.SanitizeFileName(UninstallerName));
            Directory.CreateDirectory(p);
            return p;
        }
    }
}
/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.IO;
using Klocman.Tools;
using UninstallTools.Junk.Confidence;
using UninstallTools.Properties;

namespace UninstallTools.Junk.Containers
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

        public override string ToString()
        {
            return GetType().Name + " - " + GetDisplayName();
        }
    }
}
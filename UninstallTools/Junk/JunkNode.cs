using System.IO;
using System.Security.Permissions;
using UninstallTools.Properties;

namespace UninstallTools.Junk
{
    public abstract class JunkNode
    {
        protected JunkNode()
        {
            Confidence = new JunkConfidence();
        }

        protected JunkNode(string parentPath, string name, string uninstallerName)
        {
            Name = name;
            ParentPath = parentPath;
            UninstallerName = uninstallerName;

            Confidence = new JunkConfidence();
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
        public string FullName => ParentPath != null && Name != null ? Path.Combine(ParentPath, Name) : null;

        /// <summary>
        ///     Confidence that this entry is safe to remove
        /// </summary>
        public JunkConfidence Confidence { get; internal set; }

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
    }
}
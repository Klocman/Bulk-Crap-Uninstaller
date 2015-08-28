using System;
using System.Diagnostics;
using System.Security.Permissions;
using Klocman.Forms.Tools;
using UninstallTools.Properties;
using UninstallTools.Startup;
using UninstallTools.Startup.Normal;

namespace UninstallTools.Junk
{
    public sealed class StartupJunkNode : JunkNode
    {
        internal StartupJunkNode(StartupEntryBase entry, string parentName)
        {
            Entry = entry;

            UninstallerName = parentName;

            ParentPath = entry.ParentLongName;
            Name = entry.EntryLongName;

            /*if (!string.IsNullOrEmpty(entry.CommandFilePath))
            {
                ParentPath = Path.GetDirectoryName(entry.CommandFilePath);
                Name = Path.GetFileName(entry.CommandFilePath);
            }*/

            Confidence.Add(StartupJunk.ConfidenceStartupMatched);

            var normalStartupEntry = entry as StartupEntry;
            if (normalStartupEntry != null && normalStartupEntry.IsRunOnce)
            {
                // If the entry is RunOnce, give it some negative points to keep it out of automatic removal.
                // It might be used to clean up after uninstall on next boot.
                Confidence.Add(StartupJunk.ConfidenceIsRunOnce);
            }
        }

        public override string GroupName => Localisation.Junk_Startup_GroupName;
        private StartupEntryBase Entry { get; }

        public override void Delete()
        {
            try
            {
                Entry.Delete();
            }
            catch (Exception ex)
            {
                // Failed to delete the file or value
                Debug.WriteLine("StartupJunkNode\\Delete -> " + ex.Message);
            }
        }

        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        public override void Open()
        {
            try
            {
                StartupManager.OpenStartupEntryLocations(new[] {Entry});
            }
            catch (Exception ex)
            {
                PremadeDialogs.GenericError(ex);
            }
        }
    }
}
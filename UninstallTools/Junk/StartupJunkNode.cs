using System.IO;
using System.Security.Permissions;
using Klocman.Tools;
using UninstallTools.Properties;
using UninstallTools.Startup;
using UninstallTools.Startup.Normal;

namespace UninstallTools.Junk
{
    public class StartupJunkNode : JunkNode
    {
        internal StartupJunkNode(StartupEntryBase entry, string uninstallerName)
            : base(entry.ParentLongName, entry.EntryLongName, uninstallerName)
        {
            Entry = entry;

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

        public override void Backup(string backupDirectory)
        {
            var p = Path.Combine(CreateBackupDirectory(backupDirectory), "Startup");
            Directory.CreateDirectory(p);
            Entry.CreateBackup(p);
        }

        public override void Delete()
        {
            Entry.Delete();
        }

        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        public override void Open()
        {
            StartupManager.OpenStartupEntryLocations(new[] { Entry });
        }
    }
}
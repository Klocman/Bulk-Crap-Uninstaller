﻿/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.IO;
using System.Security.Permissions;
using UninstallTools.Junk.Confidence;
using UninstallTools.Properties;
using UninstallTools.Startup;
using UninstallTools.Startup.Normal;

namespace UninstallTools.Junk.Containers
{
    public class StartupJunkNode : JunkResultBase
    {
        public static readonly ConfidenceRecord ConfidenceStartupIsRunOnce =
            new ConfidenceRecord(-5, Localisation.Confidence_Startup_IsRunOnce);

        public static readonly ConfidenceRecord ConfidenceStartupMatched =
            new ConfidenceRecord(6, Localisation.Confidence_Startup_StartupMatched);

        internal StartupEntryBase Entry { get; }

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

        public override string GetDisplayName()
        {
            return Entry.ToString();
        }

        public StartupJunkNode(StartupEntryBase entry, ApplicationUninstallerEntry application, IJunkCreator source) 
            : base(application, source)
        {
            if (entry == null) throw new ArgumentNullException(nameof(entry));

            Entry = entry;

            Confidence.Add(ConfidenceStartupMatched);

            var normalStartupEntry = entry as StartupEntry;
            if (normalStartupEntry != null && normalStartupEntry.IsRunOnce)
            {
                // If the entry is RunOnce, give it some negative points to keep it out of automatic removal.
                // It might be used to clean up after uninstall on next boot.
                Confidence.Add(ConfidenceStartupIsRunOnce);
            }
        }
    }
}
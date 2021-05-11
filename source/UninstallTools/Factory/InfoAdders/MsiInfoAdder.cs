/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using Klocman.Extensions;
using Klocman.IO;
using Klocman.Native;

namespace UninstallTools.Factory.InfoAdders
{
    public class MsiInfoAdder : IMissingInfoAdder
    {
        public void AddMissingInformation(ApplicationUninstallerEntry target)
        {
            //Debug.Assert(target.UninstallerKind != UninstallerType.Msiexec);
            if (target.UninstallerKind != UninstallerType.Msiexec)
                return;

            ApplyMsiInfo(target, target.BundleProviderKey);
        }

        public string[] RequiredValueNames { get; } = {
            nameof(ApplicationUninstallerEntry.UninstallerKind),
            nameof(ApplicationUninstallerEntry.BundleProviderKey) 
        };

        public bool RequiresAllValues { get; } = true;
        public bool AlwaysRun { get; } = false;
        public string[] CanProduceValueNames { get; } =  {
            nameof(ApplicationUninstallerEntry.RawDisplayName),
            nameof(ApplicationUninstallerEntry.DisplayVersion),
            nameof(ApplicationUninstallerEntry.Publisher),
            nameof(ApplicationUninstallerEntry.InstallLocation),
            nameof(ApplicationUninstallerEntry.InstallSource),
            nameof(ApplicationUninstallerEntry.UninstallerFullFilename),
            nameof(ApplicationUninstallerEntry.DisplayIcon),
            nameof(ApplicationUninstallerEntry.AboutUrl),
            nameof(ApplicationUninstallerEntry.InstallDate)
        };
        public InfoAdderPriority Priority { get; } = InfoAdderPriority.RunFirst;

        /// <summary>
        ///     A valid guid is REQUIRED. It doesn't have to be set on the entry, but should be.
        /// </summary>
        private static void ApplyMsiInfo(ApplicationUninstallerEntry entry, Guid guid)
        {
            //IMPORTANT: If MsiGetProductInfo returns null it means that the guid is invalid or app is not installed
            if (MsiTools.MsiGetProductInfo(guid, MsiWrapper.INSTALLPROPERTY.PRODUCTNAME) == null)
                return;

            FillInMissingInfoMsiHelper(() => entry.RawDisplayName, x => entry.RawDisplayName = x, guid,
                MsiWrapper.INSTALLPROPERTY.INSTALLEDPRODUCTNAME, MsiWrapper.INSTALLPROPERTY.PRODUCTNAME);

            FillInMissingInfoMsiHelper(() => entry.DisplayVersion, x => entry.DisplayVersion = ApplicationEntryTools.CleanupDisplayVersion(x), guid,
                MsiWrapper.INSTALLPROPERTY.VERSIONSTRING, MsiWrapper.INSTALLPROPERTY.VERSION);

            FillInMissingInfoMsiHelper(() => entry.Publisher, x => entry.Publisher = x, guid,
                MsiWrapper.INSTALLPROPERTY.PUBLISHER);

            FillInMissingInfoMsiHelper(() => entry.InstallLocation, x => entry.InstallLocation = x, guid,
                MsiWrapper.INSTALLPROPERTY.INSTALLLOCATION);

            FillInMissingInfoMsiHelper(() => entry.InstallSource, x => entry.InstallSource = x, guid,
                MsiWrapper.INSTALLPROPERTY.INSTALLSOURCE);

            FillInMissingInfoMsiHelper(() => entry.UninstallerFullFilename, x => entry.UninstallerFullFilename = x, guid,
                MsiWrapper.INSTALLPROPERTY.LOCALPACKAGE);

            FillInMissingInfoMsiHelper(() => entry.DisplayIcon, x => entry.DisplayIcon = x, guid,
                MsiWrapper.INSTALLPROPERTY.PRODUCTICON);

            FillInMissingInfoMsiHelper(() => entry.AboutUrl, x => entry.AboutUrl = x, guid,
                MsiWrapper.INSTALLPROPERTY.HELPLINK, MsiWrapper.INSTALLPROPERTY.URLUPDATEINFO,
                MsiWrapper.INSTALLPROPERTY.URLINFOABOUT);

            if (!entry.InstallDate.IsDefault()) return;
            var temp = MsiTools.MsiGetProductInfo(guid, MsiWrapper.INSTALLPROPERTY.INSTALLDATE);
            if (!temp.IsNotEmpty()) return;
            try
            {
                entry.InstallDate = new DateTime(int.Parse(temp.Substring(0, 4)),
                    int.Parse(temp.Substring(4, 2)),
                    int.Parse(temp.Substring(6, 2)));
            }
            catch
            {
                // Date had invalid format, default to nothing
            }
        }

        private static void FillInMissingInfoMsiHelper(Func<string> get, Action<string> set, Guid guid,
            params MsiWrapper.INSTALLPROPERTY[] properties)
        {
            if (string.IsNullOrEmpty(get()))
            {
                foreach (var item in properties)
                {
                    var temp = MsiTools.MsiGetProductInfo(guid, item);

                    //IMPORTANT: Do not assign empty strings, they will mess up automatic property creation later on.
                    if (temp.IsNotEmpty())
                        set(temp);
                }
            }
        }
    }
}
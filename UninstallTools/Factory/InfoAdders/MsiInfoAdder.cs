/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Diagnostics;
using Klocman.Extensions;
using Klocman.IO;
using Klocman.Native;

namespace UninstallTools.Factory.InfoAdders
{
    public class MsiInfoAdder : IMissingInfoAdder
    {
        public void AddMissingInformation(ApplicationUninstallerEntry target)
        {
            Debug.Assert(target.UninstallerKind != UninstallerType.Msiexec);
            if (target.UninstallerKind != UninstallerType.Msiexec)
                return;

            ApplyMsiInfo(target, target.BundleProviderKey);
        }

        /// <summary>
        ///     A valid guid is REQUIRED. It doesn't have to be set on the entry, but should be.
        ///     IMPORTANT: Run at the very end of the object creation!
        /// </summary>
        public static void ApplyMsiInfo(ApplicationUninstallerEntry entry, Guid guid)
        {
            //IMPORTANT: If MsiGetProductInfo returns null it means that the guid is invalid or app is not installed
            if (MsiTools.MsiGetProductInfo(guid, MsiWrapper.INSTALLPROPERTY.PRODUCTNAME) == null)
                return;

            FillInMissingInfoMsiHelper(() => entry.RawDisplayName, x => entry.RawDisplayName = x, guid,
                MsiWrapper.INSTALLPROPERTY.INSTALLEDPRODUCTNAME, MsiWrapper.INSTALLPROPERTY.PRODUCTNAME);
            FillInMissingInfoMsiHelper(() => entry.DisplayVersion, x => entry.DisplayVersion = x, guid,
                MsiWrapper.INSTALLPROPERTY.VERSIONSTRING, MsiWrapper.INSTALLPROPERTY.VERSION);
            FillInMissingInfoMsiHelper(() => entry.Publisher, x => entry.Publisher = x, guid,
                MsiWrapper.INSTALLPROPERTY.PUBLISHER);
            FillInMissingInfoMsiHelper(() => entry.InstallLocation, x => entry.InstallLocation = x, guid,
                MsiWrapper.INSTALLPROPERTY.INSTALLLOCATION);
            FillInMissingInfoMsiHelper(() => entry.InstallSource, x => entry.InstallSource = x, guid,
                MsiWrapper.INSTALLPROPERTY.INSTALLSOURCE);
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
                entry.InstallDate = new DateTime(Int32.Parse(temp.Substring(0, 4)),
                    Int32.Parse(temp.Substring(4, 2)),
                    Int32.Parse(temp.Substring(6, 2)));
            }
            catch
            {
                // Date had invalid format, default to nothing
            }
        }

        private static void FillInMissingInfoMsiHelper(Func<string> get, Action<string> set, Guid guid,
            params MsiWrapper.INSTALLPROPERTY[] properties)
        {
            if (String.IsNullOrEmpty(get()))
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
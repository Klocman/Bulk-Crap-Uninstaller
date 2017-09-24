/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.Collections.Generic;
using System.IO;
using UninstallTools.Junk.Containers;
using UninstallTools.Properties;

namespace UninstallTools.Junk.Finders.Drive
{
    public class SpecificUninstallerKindScanner : JunkCreatorBase
    {
        public override IEnumerable<IJunkResult> FindJunk(ApplicationUninstallerEntry target)
        {
            if (!File.Exists(target.UninstallerFullFilename))
                yield break;

            FileSystemJunk result;

            switch (target.UninstallerKind)
            {
                case UninstallerType.InstallShield:
                    var targetDir = new DirectoryInfo(Path.GetDirectoryName(target.UninstallerFullFilename));
                    result = new FileSystemJunk(targetDir, target, this);
                    break;

                case UninstallerType.InnoSetup:
                case UninstallerType.Msiexec:
                case UninstallerType.Nsis:
                    result = new FileSystemJunk(new FileInfo(target.UninstallerFullFilename), target, this);
                    break;

                default:
                    yield break;
            }

            result.Confidence.Add(ConfidenceRecord.ExplicitConnection);

            yield return result;
        }

        public override string CategoryName => Localisation.Junk_Drive_GroupName;
    }
}
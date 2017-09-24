/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.Collections.Generic;
using System.IO;

namespace UninstallTools.Junk
{
    public class SpecificUninstallerKindScanner : JunkCreatorBase
    {
        public override IEnumerable<JunkNode> FindJunk(ApplicationUninstallerEntry target)
        {
            if (!File.Exists(target.UninstallerFullFilename))
                yield break;

            DriveJunkNode result;

            switch (target.UninstallerKind)
            {
                case UninstallerType.InstallShield:
                    var targetDir = Path.GetDirectoryName(target.UninstallerFullFilename);
                    result = new DriveDirectoryJunkNode(Path.GetDirectoryName(targetDir),
                        Path.GetFileName(targetDir), target.DisplayName);
                    break;

                case UninstallerType.InnoSetup:
                case UninstallerType.Msiexec:
                case UninstallerType.Nsis:
                    result = new DriveFileJunkNode(Path.GetDirectoryName(target.UninstallerFullFilename),
                        Path.GetFileName(target.UninstallerFullFilename), target.DisplayName);
                    break;

                default:
                    yield break;
            }

            result.Confidence.Add(ConfidenceRecord.ExplicitConnection);

            yield return result;
        }
    }
}
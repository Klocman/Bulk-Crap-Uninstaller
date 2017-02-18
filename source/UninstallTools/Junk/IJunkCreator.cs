/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.Collections.Generic;

namespace UninstallTools.Junk
{
    public interface IJunkCreator
    {
        IEnumerable<JunkNode> FindJunk();
    }
}
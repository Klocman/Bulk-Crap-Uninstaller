using System.Collections.Generic;

namespace UninstallTools.Junk
{
    public interface IJunkCreator
    {
        IEnumerable<JunkNode> FindJunk();
    }
}
using System.Collections.Generic;

namespace UninstallTools.Junk
{
    public abstract class JunkCreatorBase : IJunkCreator2
    {
        public virtual void Setup(ICollection<ApplicationUninstallerEntry> allUninstallers)
        {
            AllUninstallers = allUninstallers;
        }

        public ICollection<ApplicationUninstallerEntry> AllUninstallers { get; private set; }

        public abstract IEnumerable<JunkNode> FindJunk(ApplicationUninstallerEntry target);
    }
}
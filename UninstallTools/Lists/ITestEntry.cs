using UninstallTools.Uninstaller;

namespace UninstallTools.Lists
{
    public interface ITestEntry
    {
        /// <summary>
        ///     Test if the input matches this filter. Returns null if result is impossible to determine or if filter is disabled.
        ///     If there are only exclude filters assumes that everything is included.
        /// </summary>
        bool? TestEntry(ApplicationUninstallerEntry input);

        bool Enabled { get; set; }
    }
}
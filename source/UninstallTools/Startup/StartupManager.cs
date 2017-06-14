using System;
using System.Collections.Generic;
using System.Linq;
using Klocman.Tools;
using Microsoft.Win32.TaskScheduler;
using UninstallTools.Properties;
using UninstallTools.Startup.Browser;
using UninstallTools.Startup.Normal;
using UninstallTools.Startup.Service;
using UninstallTools.Startup.Task;

namespace UninstallTools.Startup
{
    public static class StartupManager
    {
        static StartupManager()
        {
            Factories = new Dictionary<string, Func<IEnumerable<StartupEntryBase>>>
            {
                {
                    Localisation.StartupEntries,
                    () => StartupEntryFactory.GetStartupItems().Cast<StartupEntryBase>()
                },
                {
                    Localisation.Startup_ShortName_Task,
                    () => TaskEntryFactory.GetTaskStartupEntries().Cast<StartupEntryBase>()
                },
                {
                    Localisation.Startup_Shortname_BrowserHelper,
                    () => BrowserEntryFactory.GetBrowserHelpers().Cast<StartupEntryBase>()
                },
                {
                    Localisation.Startup_ShortName_Service,
                    () => ServiceEntryFactory.GetServiceEntries().Cast<StartupEntryBase>()
                }
            };
        }

        public static Dictionary<string, Func<IEnumerable<StartupEntryBase>>> Factories { get; }

        /// <summary>
        /// Fill in the ApplicationUninstallerEntry.StartupEntries property with a list of related StartupEntry objects.
        /// Old data is not cleared, only overwritten if necessary.
        /// </summary>
        /// <param name="uninstallers">Uninstaller entries to assign to</param>
        /// <param name="startupEntries">Startup entries to assign</param>
        public static void AssignStartupEntries(IEnumerable<ApplicationUninstallerEntry> uninstallers,
            IEnumerable<StartupEntryBase> startupEntries)
        {
            //if (startupEntries == null || uninstallers == null)
            //    return;

            var startups = startupEntries.ToList();

            if (startups.Count == 0)
                return;

            foreach (var uninstaller in uninstallers)
            {
                var positives = startups.Where(x =>
                {
                    if (
                        x.ProgramNameTrimmed?.Equals(uninstaller.DisplayNameTrimmed, StringComparison.OrdinalIgnoreCase) ==
                        true)
                        return true;

                    if (x.CommandFilePath == null)
                        return false;

                    if (uninstaller.IsInstallLocationValid() &&
                        x.CommandFilePath.StartsWith(uninstaller.InstallLocation, StringComparison.OrdinalIgnoreCase))
                        return true;

                    if (!string.IsNullOrEmpty(uninstaller.UninstallerLocation) &&
                        x.CommandFilePath.StartsWith(uninstaller.UninstallerLocation, StringComparison.OrdinalIgnoreCase))
                        return true;

                    return false;
                }).ToList();

                if (positives.Count > 0)
                    uninstaller.StartupEntries = positives;
            }
        }

        /// <summary>
        /// Look for and return all of the startup items present on this computer.
        /// </summary>
        public static IEnumerable<StartupEntryBase> GetAllStartupItems()
        {
            return Factories.Values.Aggregate(Enumerable.Empty<StartupEntryBase>(),
                (result, factory) => result.Concat(factory()));
        }

        /// <summary>
        /// Open locations of the startup entries in respective applications. (regedit, win explorer, task scheduler)
        /// </summary>
        public static void OpenStartupEntryLocations(IEnumerable<StartupEntryBase> selection)
        {
            var startupEntryBases = selection as IList<StartupEntryBase> ?? selection.ToList();
            var regOpened = false;

            if (startupEntryBases.Any(x => x is TaskEntry))
                TaskService.Instance.StartSystemTaskSchedulerManager();

            var browserAddon = startupEntryBases.OfType<BrowserHelperEntry>().FirstOrDefault();
            if (browserAddon != null)
            {
                RegistryTools.OpenRegKeyInRegedit(browserAddon.FullLongName);
                regOpened = true;
            }

            foreach (var item in startupEntryBases)
            {
                var s = item as StartupEntry;
                if (s != null && s.IsRegKey)
                {
                    if (!regOpened)
                    {
                        RegistryTools.OpenRegKeyInRegedit(item.ParentLongName);
                        regOpened = true;
                    }
                }
                else if (!string.IsNullOrEmpty(item.FullLongName))
                    WindowsTools.OpenExplorerFocusedOnObject(item.FullLongName);
                else if (!string.IsNullOrEmpty(item.CommandFilePath))
                    WindowsTools.OpenExplorerFocusedOnObject(item.CommandFilePath);
            }
        }
    }
}
/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Linq;
using Klocman.Tools;
using Microsoft.Win32.TaskScheduler;
using UninstallTools.Startup.Browser;
using UninstallTools.Startup.Normal;
using UninstallTools.Startup.Service;
using UninstallTools.Startup.Task;
using UninstallTools.Uninstaller;

namespace UninstallTools.Startup
{
    public static class StartupManager
    {
        /// <summary>
        ///     Fill in the ApplicationUninstallerEntry.StartupEntries property with a list of related StartupEntry objects.
        ///     Old data is not cleared, only overwritten if necessary.
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
                    if (x.ProgramNameTrimmed?.Equals(uninstaller.DisplayNameTrimmed, StringComparison.OrdinalIgnoreCase) == true)
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
        ///     Open locations of the startup entries in respective applications. (regedit, win explorer, task scheduler)
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

        /// <summary>
        ///     Look for and return all of the startup items present on this computer.
        /// </summary>
        public static IEnumerable<StartupEntryBase> GetAllStartupItems()
        {
            return StartupEntryFactory.GetStartupItems().Cast<StartupEntryBase>()
                .Concat(TaskEntryFactory.GetTaskStartupEntries().Cast<StartupEntryBase>())
                .Concat(BrowserEntryFactory.GetBrowserHelpers().Cast<StartupEntryBase>())
                .Concat(ServiceEntryFactory.GetServiceEntries().Cast<StartupEntryBase>());
        }
    }
}
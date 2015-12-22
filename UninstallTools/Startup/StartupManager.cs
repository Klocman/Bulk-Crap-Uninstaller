using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Klocman.Extensions;
using Klocman.Tools;
using Microsoft.Win32.TaskScheduler;
using UninstallTools.Startup.Browser;
using UninstallTools.Startup.Normal;
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
                WindowsTools.OpenRegKeyInRegedit(browserAddon.FullLongName);
                regOpened = true;
            }

            foreach (var item in startupEntryBases.OfType<StartupEntry>())
            {
                if (item.IsRegKey)
                {
                    if (!regOpened)
                    {
                        WindowsTools.OpenRegKeyInRegedit(item.ParentLongName);
                        regOpened = true;
                    }
                }
                else
                    WindowsTools.OpenExplorerFocusedOnObject(item.FullLongName);
            }
        }

        /// <summary>
        ///     Look for and return all of the startup items present on this computer.
        /// </summary>
        public static IEnumerable<StartupEntryBase> GetAllStartupItems()
        {
            return StartupEntryFactory.GetStartupItems().Cast<StartupEntryBase>()
                .Concat(TaskEntryFactory.GetTaskStartupEntries().Cast<StartupEntryBase>())
                .Concat(BrowserEntryFactory.GetBrowserHelpers().Cast<StartupEntryBase>());
        }

        static bool GetExtendedAttributesNotSupported = false;
        internal static ExtractedInfo GetInfoFromFileAttributes(string sourceFile)
        {
            var resultInfo = new ExtractedInfo();
            if (GetExtendedAttributesNotSupported || !File.Exists(sourceFile))
                return resultInfo;

            // Fill in properties by gathering info from the command and the executable it is pointing at.
            try
            {
                var attribs =
                    new FileInfo(sourceFile).GetExtendedAttributes()
                        .Where(x => !string.IsNullOrEmpty(x.Value))
                        .ToList();

                foreach (var filter in new[] { "Product name", "Friendly name", "Program Name" })
                {
                    var result =
                        attribs.FirstOrDefault(x => x.Key.Equals(filter, StringComparison.OrdinalIgnoreCase));
                    if (!result.IsDefault())
                    {
                        resultInfo.ProgramName = result.Value;
                        break;
                    }
                }

                foreach (var filter in new[] { "Company", "Publisher" })
                {
                    var result =
                        attribs.FirstOrDefault(x => x.Key.Equals(filter, StringComparison.OrdinalIgnoreCase));
                    if (!result.IsDefault())
                    {
                        resultInfo.Company = result.Value;
                        break;
                    }
                }
            }
            catch (InvalidCastException)
            {
                // Interface is not supported, don't bother trying again
                GetExtendedAttributesNotSupported = true;
            }
            catch
            {
                // Return whatever could be gathered on error
            }

            return resultInfo;
        }

        internal struct ExtractedInfo
        {
            public string Company;
            public string ProgramName;
        }
    }
}
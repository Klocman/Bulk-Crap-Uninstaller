/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BulkCrapUninstaller.Functions;
using BulkCrapUninstaller.Functions.Tools;
using BulkCrapUninstaller.Properties;
using Klocman.Extensions;
using UninstallTools;
using UninstallTools.Uninstaller;

namespace BulkCrapUninstaller.Forms
{
    public partial class BeginUninstallTaskWizard : Form
    {
        private List<ApplicationUninstallerEntry> _otherUninstallers;
        private ICollection<ApplicationUninstallerEntry> _selectedUninstallers;
        private bool _quiet;
        private ICollection<ApplicationUninstallerEntry> _allUninstallers;

        public BeginUninstallTaskWizard()
        {
            InitializeComponent();

            Icon = MessageBoxes.DefaultOwner.Icon;
        }

        public void Initialize(ICollection<ApplicationUninstallerEntry> selectedUninstallers,
            ICollection<ApplicationUninstallerEntry> allUninstallers, bool quiet)
        {
            _allUninstallers = allUninstallers;
            _quiet = quiet;
            _selectedUninstallers = selectedUninstallers;
            _otherUninstallers = allUninstallers.Except(_selectedUninstallers).ToList();

            var relatedUninstallers = _otherUninstallers.Select(
                x => new { Entry = x, Related = GetRelatedUninstallers(x, _selectedUninstallers).ToList() })
                .Where(x => x.Related.Any());

            relatedUninstallerAdder1.SetRelatedApps(relatedUninstallers
                .Select(x => new RelatedUninstallerAdder.RelatedApplicationEntry(x.Entry, x.Related)));

            /*
            var taskEntries = ConvertToTaskEntries(quiet, targetList);

            taskEntries = _settings.AdvancedIntelligentUninstallerSorting
                ? SortIntelligently(taskEntries).ToList()
                : taskEntries.OrderBy(x => x.UninstallerEntry.DisplayName).ToList();

            taskEntries = UninstallConfirmation.ShowConfirmationDialog(MessageBoxes.DefaultOwner, taskEntries);

            if (taskEntries == null || taskEntries.Count == 0)
                return;

            if (!SystemRestore.BeginSysRestore(targetList.Count))
                return;

            if (!CheckForRunningProcessesBeforeUninstall(taskEntries.Select(x => x.UninstallerEntry), !quiet))
                return;*/
        }

        private static IEnumerable<ApplicationUninstallerEntry> GetRelatedUninstallers(ApplicationUninstallerEntry thisUninstaller, IEnumerable<ApplicationUninstallerEntry> other)
        {
            // todo more complex checks
            return other.Where(y => y.InstallLocation.Contains(thisUninstaller.InstallLocation, StringComparison.OrdinalIgnoreCase));
        }

        private List<BulkUninstallEntry> ConvertToTaskEntries(IEnumerable<ApplicationUninstallerEntry> targets)
        {
            var targetList = new List<BulkUninstallEntry>();

            foreach (var target in targets)
            {
                var tempStatus = UninstallStatus.Waiting;
                if (!target.IsValid)
                    tempStatus = UninstallStatus.Invalid;
                else if (!Settings.Default.AdvancedDisableProtection && target.IsProtected)
                    tempStatus = UninstallStatus.Protected;

                var silentPossible = _quiet && target.QuietUninstallPossible;

                targetList.Add(new BulkUninstallEntry(target, silentPossible, tempStatus));
            }

            return targetList;
        }

        private int PageNumber => tabControl1.SelectedIndex;

        private void buttonNext_Click(object sender, EventArgs e)
        {
            if (PageNumber == 0)
            {
                var additionals = relatedUninstallerAdder1.GetResults();
                uninstallConfirmation1.SetRelatedApps(ConvertToTaskEntries(_selectedUninstallers.Concat(additionals)));
                tabControl1.SelectedIndex = 1;
            }
        }
    }
}

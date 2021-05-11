/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Drawing;
using System.Windows.Forms;
using Klocman.Binding.Settings;
using Klocman.IO;

namespace BulkCrapUninstaller.Controls
{
    public partial class UninstallationSettings : UserControl
    {
        public UninstallationSettings()
        {
            InitializeComponent();
        }

        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
        
            if (DesignMode) return;

            var settings = Properties.Settings.Default.SettingBinder;
        
            // Shutdown blocking not available below Windows Vista
            if (Environment.OSVersion.Version < new Version(6, 0))
                checkBoxShutdown.Enabled = false;
            else
                settings.BindControl(checkBoxShutdown, settings => settings.UninstallPreventShutdown, this);
        
            checkBoxRestorePoint.Enabled = SysRestore.SysRestoreAvailable();
            settings.BindControl(checkBoxRestorePoint, settings => settings.CreateRestorePoint, this);
        
            settings.BindControl(checkBoxConcurrent, settings => settings.UninstallConcurrency, this);
        
            settings.BindControl(checkBoxConcurrentOneLoud, settings => settings.UninstallConcurrentOneLoud, this);
            settings.BindControl(checkBoxManualNoCollisionProtection, settings => settings.UninstallConcurrentDisableManualCollisionProtection, this);
        
            settings.Subscribe(OnMaxCountChanged, settings => settings.UninstallConcurrentMaxCount, this);
            numericUpDownMaxConcurrent.ValueChanged += NumericUpDownMaxConcurrentOnValueChanged;
        
            settings.BindControl(checkBoxBatchSortQuiet, x => x.AdvancedIntelligentUninstallerSorting, this);
            settings.BindControl(checkBoxDiisableProtection, x => x.AdvancedDisableProtection, this);
            settings.BindControl(checkBoxSimulate, x => x.AdvancedSimulate, this);
        
            settings.BindControl(checkBoxAutoKillQuiet, x => x.QuietAutoKillStuck, this);
            settings.BindControl(checkBoxRetryQuiet, x => x.QuietRetryFailedOnce, this);
            settings.BindControl(checkBoxGenerate, x => x.QuietAutomatization, this);
            settings.BindControl(checkBoxGenerateStuck, x => x.QuietAutomatizationKillStuck, this);
            settings.BindControl(checkBoxAutoDaemon, x => x.QuietUseDaemon, this);
        
            settings.Subscribe((sender, args) => checkBoxGenerateStuck.Enabled = args.NewValue, settings => settings.QuietAutomatization, this);
        
            settings.Subscribe(
                (x, y) => checkBoxSimulate.ForeColor = y.NewValue ? Color.OrangeRed : SystemColors.ControlText,
                x => x.AdvancedSimulate, this);
        
            settings.SendUpdates(this);
            Disposed += (x, y) => settings.RemoveHandlers(this);
        }

        private void NumericUpDownMaxConcurrentOnValueChanged(object sender, EventArgs eventArgs)
        {
            Properties.Settings.Default.UninstallConcurrentMaxCount = (int)numericUpDownMaxConcurrent.Value;
        }

        private void OnMaxCountChanged(object sender, SettingChangedEventArgs<int> args)
        {
            numericUpDownMaxConcurrent.Value = args.NewValue;
        }
    }
}

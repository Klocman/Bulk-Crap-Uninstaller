using System;
using System.Drawing;
using System.Windows.Forms;
using BulkCrapUninstaller.Properties;
using Klocman.Binding;
using Klocman.Events;

namespace BulkCrapUninstaller.Controls
{
    public partial class UninstallationSettings : UserControl
    {
        private readonly SettingBinder<Settings> _settings = Settings.Default.SettingBinder;

        public UninstallationSettings()
        {
            InitializeComponent();

            // Shutdown blocking not available below Windows Vista
            if (Environment.OSVersion.Version < new Version(6, 0))
                checkBoxShutdown.Enabled = false;
            else
                _settings.BindControl(checkBoxShutdown, settings => settings.UninstallPreventShutdown, this);

            _settings.BindControl(checkBoxConcurrent, settings => settings.UninstallConcurrency, this);

            _settings.BindControl(checkBoxConcurrentOneLoud, settings => settings.UninstallConcurrentOneLoud, this);
            _settings.BindControl(checkBoxManualNoCollisionProtection, settings => settings.UninstallConcurrentDisableManualCollisionProtection, this);
            //_settings.BindControl(checkBoxConcurrentLessCollisionProtection, settings => settings.UninstallConcurrentLessCollisionProtection, this);

            _settings.Subscribe(OnMaxCountChanged, settings => settings.UninstallConcurrentMaxCount, this);
            numericUpDownMaxConcurrent.ValueChanged += NumericUpDownMaxConcurrentOnValueChanged;

            _settings.BindControl(checkBoxBatchSortQuiet, x => x.AdvancedIntelligentUninstallerSorting, this);
            _settings.BindControl(checkBoxDiisableProtection, x => x.AdvancedDisableProtection, this);
            _settings.BindControl(checkBoxSimulate, x => x.AdvancedSimulate, this);

            _settings.BindControl(checkBoxAutoKillQuiet, x => x.QuietAutoKillStuck, this);
            _settings.BindControl(checkBoxRetryQuiet, x => x.QuietRetryFailedOnce, this);
            _settings.BindControl(checkBoxGenerate, x => x.QuietAutomatization, this);
            _settings.BindControl(checkBoxGenerateStuck, x => x.QuietAutomatizationKillStuck, this);

            if (!Program.Net4IsAvailable)
            {
                checkBoxGenerate.Enabled = false;
                checkBoxGenerateStuck.Enabled = false; 
            }

            _settings.Subscribe(
                (x, y) => checkBoxSimulate.ForeColor = y.NewValue ? Color.OrangeRed : SystemColors.ControlText,
                x => x.AdvancedSimulate, this);

            _settings.SendUpdates(this);
            Disposed += (x, y) => _settings.RemoveHandlers(this);
        }

        private void NumericUpDownMaxConcurrentOnValueChanged(object sender, EventArgs eventArgs)
        {
            _settings.Settings.UninstallConcurrentMaxCount = (int)numericUpDownMaxConcurrent.Value;
        }

        private void OnMaxCountChanged(object sender, SettingChangedEventArgs<int> args)
        {
            numericUpDownMaxConcurrent.Value = args.NewValue;
        }
    }
}

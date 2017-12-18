﻿/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using BulkCrapUninstaller.Functions;
using BulkCrapUninstaller.Properties;
using Klocman;
using Klocman.Binding.Settings;
using Klocman.Forms.Tools;
using Klocman.Localising;

namespace BulkCrapUninstaller.Forms
{
    public partial class SettingsWindow : Form
    {
        private readonly SettingBinder<Settings> _settings = Settings.Default.SettingBinder;
        private bool _restartNeeded;

        public int OpenedTab { get { return tabControl.SelectedIndex; } set { tabControl.SelectedIndex = value; } }

        public SettingsWindow()
        {
            InitializeComponent();

            Icon = Resources.Icon_Logo;

            _settings.BindControl(checkBoxLoud, x => x.MessagesAskRemoveLoudItems, this);
            _settings.BindControl(checkBoxShowAllBadJunk, x => x.MessagesShowAllBadJunk, this);
            _settings.BindControl(checkBoxNeverFeedback, x => x.MiscFeedbackNagNeverShow, this);
            _settings.BindControl(checkBoxUpdateSearch, x => x.MiscCheckForUpdates, this);
            _settings.BindControl(checkBoxSendStats, x => x.MiscSendStatistics, this);
            _settings.BindControl(checkBoxAutoLoad, x => x.MiscAutoLoadDefaultList, this);
            _settings.BindControl(checkBoxRatings, x => x.MiscUserRatings, this);

            _settings.BindControl(checkBoxEnableExternal, x => x.ExternalEnable, this);
            _settings.BindControl(textBoxPreUninstall, x => x.ExternalPreCommands, this);
            _settings.BindControl(textBoxPostUninstall, x => x.ExternalPostCommands, this);

            _settings.BindControl(textBoxProgramFolders, x => x.FoldersCustomProgramDirs, this);
            _settings.BindControl(checkBoxAutoInstallFolderDetect, x => x.FoldersAutoDetect, this);

            _settings.BindControl(checkBoxScanSteam, x => x.ScanSteam, this);
            _settings.BindControl(checkBoxScanStoreApps, x => x.ScanStoreApps, this);
            _settings.BindControl(checkBoxScanWinFeatures, x => x.ScanWinFeatures, this);
            _settings.BindControl(checkBoxScanWinUpdates, x => x.ScanWinUpdates, this);

            _settings.BindControl(checkBoxScanDrives, x => x.ScanDrives, this);
            _settings.BindControl(checkBoxScanRegistry, x => x.ScanRegistry, this);
            _settings.BindControl(checkBoxPreDefined, x => x.ScanPreDefined, this);

            foreach (YesNoAsk value in Enum.GetValues(typeof(YesNoAsk)))
            {
                var wrapper = new LocalisedEnumWrapper(value);
                comboBoxJunk.Items.Add(wrapper);
                comboBoxRestore.Items.Add(wrapper);
            }

            comboBoxLanguage.Items.Add(Localisable.DefaultLanguage);
            foreach (var languageCode in CultureConfigurator.SupportedLanguages.OrderBy(x => x.DisplayName))
            {
                comboBoxLanguage.Items.Add(new ComboBoxWrapper<CultureInfo>(languageCode, x => x.DisplayName));
            }

            _settings.Subscribe(JunkSettingChanged, x => x.MessagesRemoveJunk, this);
            _settings.Subscribe(RestoreSettingChanged, x => x.MessagesRestorePoints, this);
            _settings.Subscribe(LanguageSettingChanged, x => x.Language, this);

            _settings.Subscribe(BackupSettingChanged, x => x.BackupLeftovers, this);
            _settings.BindProperty(directorySelectBoxBackup,
                box => box.DirectoryPath, nameof(directorySelectBoxBackup.DirectoryPathChanged),
                settings => settings.BackupLeftoversDirectory, this);

            _settings.SendUpdates(this);

            _restartNeeded = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();

            if (_restartNeeded && MessageBoxes.RestartNeededForSettingChangeQuestion())
            {
                EntryPoint.Restart();
            }
        }

        private void checkBoxEnableExternal_CheckedChanged(object sender, EventArgs e)
        {
            splitContainer1.Enabled = checkBoxEnableExternal.Checked;
            //textBoxPreUninstall.Enabled = checkBoxEnableExternal.Checked;
            //textBoxPostUninstall.Enabled = checkBoxEnableExternal.Checked;
        }

        private void comboBoxJunk_SelectedIndexChanged(object sender, EventArgs e)
        {
            var wrapper = comboBoxJunk.SelectedItem as LocalisedEnumWrapper;
            if (wrapper != null)
            {
                _settings.Settings.MessagesRemoveJunk = (YesNoAsk)wrapper.TargetEnum;
            }
        }

        private void comboBoxLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            var wrapper = comboBoxLanguage.SelectedItem as ComboBoxWrapper<CultureInfo>;
            if (wrapper != null)
            {
                _settings.Settings.Language = wrapper.WrappedObject.Name;
                _restartNeeded = true;
            }
            else if (comboBoxLanguage.SelectedItem is string)
            {
                _settings.Settings.Language = string.Empty;
                _restartNeeded = true;
            }
        }

        private void comboBoxRestore_SelectedIndexChanged(object sender, EventArgs e)
        {
            var wrapper = comboBoxRestore.SelectedItem as LocalisedEnumWrapper;
            if (wrapper != null)
            {
                _settings.Settings.MessagesRestorePoints = (YesNoAsk)wrapper.TargetEnum;
            }
        }

        private void JunkSettingChanged(object sender, SettingChangedEventArgs<YesNoAsk> args)
        {
            var newSelection =
                comboBoxJunk.Items.Cast<LocalisedEnumWrapper>().FirstOrDefault(x => x.TargetEnum.Equals(args.NewValue));
            if (newSelection == null || newSelection.Equals(comboBoxJunk.SelectedItem))
                return;

            comboBoxJunk.SelectedItem = newSelection;
        }

        private void LanguageSettingChanged(object sender, SettingChangedEventArgs<string> args)
        {
            if (!string.IsNullOrEmpty(args.NewValue))
            {
                var selectedItem = comboBoxLanguage.Items.OfType<ComboBoxWrapper<CultureInfo>>()
                    .FirstOrDefault(x => x.WrappedObject.Name.Equals(args.NewValue));
                if (selectedItem != null)
                {
                    comboBoxLanguage.SelectedItem = selectedItem;
                    return;
                }
            }
            comboBoxLanguage.SelectedIndex = 0;
        }

        private void RestoreSettingChanged(object sender, SettingChangedEventArgs<YesNoAsk> args)
        {
            var newSelection =
                comboBoxRestore.Items.Cast<LocalisedEnumWrapper>()
                    .FirstOrDefault(x => x.TargetEnum.Equals(args.NewValue));
            if (newSelection == null || newSelection.Equals(comboBoxRestore.SelectedItem))
                return;

            comboBoxRestore.SelectedItem = newSelection;
        }

        private void SettingsWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            _settings.RemoveHandlers(this);
        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Built in autosize doesn't work properly for the flowLayoutPanel
            groupBoxAppStores.AutoSize = false;
            groupBoxAppStores.Height = flowLayoutPanel6.Height +
                                       (groupBoxAppStores.Height - groupBoxAppStores.DisplayRectangle.Height);
        }

        private void radioButtonBackup_CheckedChanged(object sender, EventArgs e)
        {
            directorySelectBoxBackup.Enabled = false;

            if (radioButtonBackupAsk.Checked)
                _settings.Settings.BackupLeftovers = YesNoAsk.Ask;
            else if (radioButtonBackupAuto.Checked)
            {
                _settings.Settings.BackupLeftovers = YesNoAsk.Yes;
                directorySelectBoxBackup.Enabled = true;
            }
            else if (radioButtonBackupNever.Checked)
                _settings.Settings.BackupLeftovers = YesNoAsk.No;
            else
                throw new InvalidOperationException();
        }

        private void BackupSettingChanged(object sender, SettingChangedEventArgs<YesNoAsk> args)
        {
            switch (args.NewValue)
            {
                case YesNoAsk.Ask:
                    radioButtonBackupAsk.Checked = true;
                    break;
                case YesNoAsk.Yes:
                    radioButtonBackupAuto.Checked = true;
                    break;
                case YesNoAsk.No:
                    radioButtonBackupNever.Checked = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
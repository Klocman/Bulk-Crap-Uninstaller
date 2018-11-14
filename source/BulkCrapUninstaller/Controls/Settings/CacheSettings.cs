/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Windows.Forms;
using BulkCrapUninstaller.Forms;
using Klocman.Binding.Settings;
using Klocman.Forms.Tools;
using UninstallTools;

namespace BulkCrapUninstaller.Controls.Settings
{
    public partial class CacheSettings : UserControl
    {
        private readonly SettingBinder<Properties.Settings> _settings = Properties.Settings.Default.SettingBinder;

        public CacheSettings()
        {
            InitializeComponent();

            _settings.BindControl(checkBoxCerts, x => x.CacheCertificates, this);
            _settings.BindControl(checkBoxInfo, x => x.CacheAppInfo, this);

            _settings.SendUpdates(this);
            Disposed += (x, y) => _settings.RemoveHandlers(this);

            button1.Click += ClearCaches;
        }

        private void ClearCaches(object sender, EventArgs e)
        {
            try
            {
                MainWindow.CertificateCache.Delete();

                UninstallToolsGlobalConfig.EnableAppInfoCache = false;
                UninstallToolsGlobalConfig.EnableAppInfoCache = _settings.Settings.CacheAppInfo;
            }
            catch (SystemException systemException)
            {
                PremadeDialogs.GenericError(systemException);
            }

            button1.Enabled = false;
        }
    }
}
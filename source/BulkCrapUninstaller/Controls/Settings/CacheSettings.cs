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
        }

        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);

            _settings.BindControl(checkBoxCerts, x => x.CacheCertificates, this);
            _settings.BindControl(checkBoxInfo, x => x.CacheAppInfo, this);

            _settings.SendUpdates(this);
            Disposed += (x, y) => _settings.RemoveHandlers(this);

            button1.Click += (x, y) =>
            {
                Program.ClearCaches(true);
                button1.Enabled = false;
            };
        }
    }
}
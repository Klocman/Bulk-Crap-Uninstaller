/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.Windows.Forms;
using Klocman.Binding.Settings;

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

            if (DesignMode) return;

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
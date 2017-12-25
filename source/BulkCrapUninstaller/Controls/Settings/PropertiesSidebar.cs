/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using BulkCrapUninstaller.Properties;
using Klocman.Binding.Settings;

namespace BulkCrapUninstaller.Controls
{
    public partial class PropertiesSidebar : UserControl
    {
        private readonly SettingBinder<Settings> _settings = Settings.Default.SettingBinder;

        public PropertiesSidebar()
        {
            InitializeComponent();

            _settings.BindControl(checkBoxViewCheckboxes, x => x.UninstallerListUseCheckboxes, this);
            _settings.BindControl(checkBoxViewGroups, x => x.UninstallerListUseGroups, this);

            _settings.BindControl(checkBoxListHideMicrosoft, x => x.FilterHideMicrosoft, this);
            _settings.BindControl(checkBoxShowUpdates, x => x.FilterShowUpdates, this);
            _settings.BindControl(checkBoxListSysComp, x => x.FilterShowSystemComponents, this);
            _settings.BindControl(checkBoxListProtected, x => x.FilterShowProtected, this);
            _settings.BindControl(checkBoxShowStoreApps, x => x.FilterShowStoreApps, this);
            _settings.BindControl(checkBoxWinFeature, x => x.FilterShowWinFeatures, this);

            _settings.BindControl(checkBoxInvalidTest, x => x.AdvancedTestInvalid, this);
            _settings.BindControl(checkBoxCertTest, x => x.AdvancedTestCertificates, this);
            _settings.BindControl(checkBoxOrphans, x => x.AdvancedDisplayOrphans, this);

            _settings.SendUpdates(this);
            Disposed += (x, y) => _settings.RemoveHandlers(this);
        }
        
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public int GetSuggestedWidth()
        {
            var maxWidth = typeof(PropertiesSidebar)
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(x => x.FieldType == typeof(CheckBox))
                .Select(x => x.GetValue(this))
                .Cast<CheckBox>()
                .Max(c => c.Width);

            return maxWidth + (groupBox1.Width - groupBox1.DisplayRectangle.Width) + Padding.Left + Padding.Right;
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool SysCompEnabled
        {
            get { return checkBoxListSysComp.Enabled; }
            set { checkBoxListSysComp.Enabled = value; }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool ProtectedEnabled
        {
            get { return checkBoxListProtected.Enabled; }
            set { checkBoxListProtected.Enabled = value; }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool UpdatesEnabled
        {
            get { return checkBoxShowUpdates.Enabled; }
            set { checkBoxShowUpdates.Enabled = value; }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool OrphansEnabled
        {
            get { return checkBoxOrphans.Enabled; }
            set { checkBoxOrphans.Enabled = value; }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool StoreAppsEnabled
        {
            get { return checkBoxShowStoreApps.Enabled; }
            set { checkBoxShowStoreApps.Enabled = value; }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool InvalidEnabled
        {
            get { return checkBoxInvalidTest.Enabled; }
            set { checkBoxInvalidTest.Enabled = value; }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool WinFeaturesEnabled
        {
            get { return checkBoxWinFeature.Enabled; }
            set { checkBoxWinFeature.Enabled = value; }
        }
    }
}
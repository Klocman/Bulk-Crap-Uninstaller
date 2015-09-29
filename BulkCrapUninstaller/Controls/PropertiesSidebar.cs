using System.Drawing;
using System.Windows.Forms;
using BulkCrapUninstaller.Properties;
using Klocman.Binding;

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

            _settings.BindControl(checkBoxBatchSortQuiet, x => x.AdvancedIntelligentUninstallerSorting, this);
            _settings.BindControl(checkBoxInvalidTest, x => x.AdvancedTestInvalid, this);
            _settings.BindControl(checkBoxCertTest, x => x.AdvancedTestCertificates, this);
            _settings.BindControl(checkBoxOrphans, x => x.AdvancedDisplayOrphans, this);
            _settings.BindControl(checkBoxDiisableProtection, x => x.AdvancedDisableProtection, this);
            _settings.BindControl(checkBoxSimulate, x => x.AdvancedSimulate, this);

            _settings.Subscribe(
                (x, y) => checkBoxSimulate.ForeColor = y.NewValue ? Color.OrangeRed : SystemColors.ControlText,
                x => x.AdvancedSimulate, this);

            _settings.SendUpdates(this);
            Disposed += (x, y) => _settings.RemoveHandlers(this);
        }
    }
}
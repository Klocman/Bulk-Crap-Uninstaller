using System;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using BulkCrapUninstaller.Functions;
using BulkCrapUninstaller.Properties;
using Klocman.Binding;
using Klocman.Forms.Tools;

namespace BulkCrapUninstaller.Forms
{
    public partial class FirstStartBox : Form
    {
        private const int PageCount = 5;
        private readonly int _pageWidth;
        private readonly SettingBinder<Settings> _settings = Settings.Default.SettingBinder;
        private int _pageNumber;
        private int _targetXPos;

        public FirstStartBox()
        {
            InitializeComponent();

            Icon = Resources.Icon_Logo;

            p1linkLabelContact.TabStop = false;
            p5LinkHomepage.TabStop = false;
            p5LinkContact.TabStop = false;

            _pageWidth = page1.Width + spacer4.Width;

            // List view
            _settings.BindControl(checkBoxCheckboxes, x => x.UninstallerListUseCheckboxes, this);
            _settings.BindControl(checkBoxGroups, x => x.UninstallerListUseGroups, this);
            _settings.BindControl(checkBoxCertTest, x => x.AdvancedTestCertificates, this);

            // Advanced
            _settings.BindControl(checkBoxDiisableProtection, x => x.AdvancedDisableProtection, this);
            _settings.BindControl(checkBoxListProtected, x => x.FilterShowProtected, this);
            _settings.BindControl(checkBoxListSysComp, x => x.FilterShowSystemComponents, this);

            // Corrupted
            _settings.BindControl(checkBoxInvalidTest, x => x.AdvancedTestInvalid, this);
            _settings.BindControl(checkBoxOrphans, x => x.AdvancedDisplayOrphans, this);

            // Network
            _settings.BindControl(checkBoxSendStats, x => x.MiscSendStatistics, this);
            _settings.BindControl(checkBoxUpdateSearch, x => x.MiscCheckForUpdates, this);
            _settings.BindControl(checkBoxRatings, x => x.MiscUserRatings, this);

            comboBoxLanguage.Items.Add(Localisable.DefaultLanguage);
            foreach (var languageCode in CultureConfigurator.SupportedLanguages.OrderBy(x => x.DisplayName))
            {
                comboBoxLanguage.Items.Add(new ComboBoxWrapper<CultureInfo>(languageCode, x => x.DisplayName));
            }

            var selectedItem = comboBoxLanguage.Items.OfType<ComboBoxWrapper<CultureInfo>>()
                .FirstOrDefault(x => x.WrappedObject.Name.Equals(_settings.Settings.Language));
            if (selectedItem != null)
            {
                comboBoxLanguage.SelectedItem = selectedItem;
            }
            else
            {
                comboBoxLanguage.SelectedIndex = 0;
            }

            _settings.SendUpdates(this);
        }

        private void buttonLanguageApply_Click(object sender, EventArgs e)
        {
            _settings.Settings.Language = comboBoxLanguage.SelectedIndex == 0 ? string.Empty : 
                ((ComboBoxWrapper<CultureInfo>) comboBoxLanguage.SelectedItem).WrappedObject.Name;

            _settings.Settings.Save();

            EntryPoint.Restart();
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            _pageNumber++;
            if (_pageNumber > PageCount)
                _pageNumber = PageCount;

            UpdateScrollPosition();
        }

        private void buttonPrev_Click(object sender, EventArgs e)
        {
            _pageNumber--;
            if (_pageNumber < 0)
                _pageNumber = 0;

            UpdateScrollPosition();
        }

        private void CloseWizard(object sender, EventArgs e)
        {
            _settings.Settings.MiscFirstRun = false;
            _settings.Settings.Save();
            Close();
        }

        private void FirstStartBox_FormClosed(object sender, FormClosedEventArgs e)
        {
            _settings.RemoveHandlers(this);
        }

        private void OpenContactForm(object sender, LinkLabelLinkClickedEventArgs e)
        {
            PremadeDialogs.StartProcessSafely(Resources.ContactUrl);
        }

        private void OpenHomepage(object sender, LinkLabelLinkClickedEventArgs e)
        {
            PremadeDialogs.StartProcessSafely(Resources.HomepageUrl);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            int resultXPos;
            var currentXPos = scrollPanel.HorizontalScroll.Value;

            var difference = Math.Abs(_targetXPos - currentXPos);
            var change = Math.Max(1, (int) Math.Round(Math.Pow(difference/5f, 1.03f), MidpointRounding.ToEven));

            if (currentXPos > _targetXPos)
            {
                resultXPos = currentXPos - change;
                resultXPos = Math.Max(resultXPos, _targetXPos);
            }
            else if (currentXPos < _targetXPos)
            {
                resultXPos = currentXPos + change;
                resultXPos = Math.Min(resultXPos, _targetXPos);
            }
            else
            {
                resultXPos = _targetXPos;
            }

            //Console.WriteLine(string.Format("{0,5}{1,5}{2,5}", currentXPos, change, resultXPos));

            // Double assign is needed because of a bug in the control
            scrollPanel.HorizontalScroll.Value = resultXPos;
            scrollPanel.HorizontalScroll.Value = resultXPos;

            if (resultXPos == _targetXPos)
            {
                timer1.Enabled = false;

                if (_pageNumber == PageCount)
                    buttonFinish.Focus();
            }
        }

        private void UpdateScrollPosition()
        {
            _targetXPos = _pageNumber*_pageWidth;

            timer1.Enabled = false; // Not actually needed
            var currentValue = scrollPanel.HorizontalScroll.Value;

            // Bug in the control: Scroll bar resets when the Enabled property is set to false
            buttonNext.Enabled = _pageNumber < PageCount;
            buttonPrev.Enabled = _pageNumber > 0;

            // Double assign is needed because of a bug in the control
            scrollPanel.HorizontalScroll.Value = currentValue;
            scrollPanel.HorizontalScroll.Value = currentValue;

            timer1.Enabled = true;
        }

        private void buttonMore_Click(object sender, EventArgs e)
        {
            using (var sw = new SettingsWindow())
                sw.ShowDialog(this);

            DialogResult = DialogResult.None;
        }

        private void buttonHelp_Click(object sender, EventArgs e)
        {
            MessageBoxes.DisplayHelp(this);
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Klocman.Controls;
using Klocman.Localising;
using UninstallTools.Lists;

namespace UninstallTools.Controls
{
    public partial class FilterEditor : UserControl
    {
        private static readonly LocalisedEnumWrapper[] FilteringOptions;
        private static readonly Dictionary<string, ComparisonTargetInfo> PropertyTargets;
        private ComparisonEntry _targetComparisonEntry;

        static FilterEditor()
        {
            FilteringOptions = Enum.GetValues(typeof(FilterComparisonMethod))
                .Cast<FilterComparisonMethod>().Select(x => new LocalisedEnumWrapper(x)).ToArray();

            PropertyTargets = new Dictionary<string, ComparisonTargetInfo>();
            foreach (var comparisonTarget in ComparisonTargetInfo.ComparisonTargets.OrderBy(x => x.DisplayName))
            {
                PropertyTargets.Add(comparisonTarget.DisplayName, comparisonTarget);
            }
        }

        public FilterEditor()
        {
            InitializeComponent();

            comboBoxCompareMethod.Items.AddRange(FilteringOptions.Cast<object>().ToArray());
            comboBoxCompareMethod.SelectedIndex = 0;

            comboBox1.Items.Add(ComparisonTargetInfo.AllTargetComparison.DisplayName);
            comboBox1.Items.AddRange(PropertyTargets.Keys.Cast<object>().ToArray());
            comboBox1.SelectedIndex = 0;

            var autoCompleteStringCollection = new AutoCompleteStringCollection();

            textBoxFilterText.AutoCompleteCustomSource = autoCompleteStringCollection;
            textBoxFilterText.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            textBoxFilterText.AutoCompleteSource = AutoCompleteSource.CustomSource;

            searchBox1.AutoCompleteCustomSource = autoCompleteStringCollection;
        }

        private ComparisonTargetInfo SelectedTargetInfo
        {
            get
            {
                var selection = comboBox1.SelectedItem?.ToString();
                if (string.IsNullOrEmpty(selection))
                    return ComparisonTargetInfo.AllTargetComparison;

                ComparisonTargetInfo c;
                return PropertyTargets.TryGetValue(selection, out c) ? c : ComparisonTargetInfo.AllTargetComparison;
            }
        }

        [Browsable(false)]
        [ReadOnly(true)]
        public ComparisonEntry TargetComparisonEntry
        {
            get
            {

                return _targetComparisonEntry;
            }
            set
            {
                _targetComparisonEntry = value;

                RefreshEditor();
                //event
            }
        }

        public void RefreshEditor()
        {
            if (_targetComparisonEntry == null)
            {
                Enabled = false;
                textBoxFilterText.Text = string.Empty;
                checkBoxInvert.Checked = false;
                comboBox1.SelectedIndex = 0;
                comboBoxCompareMethod.SelectedIndex = 0;
                return;
            }

            var option = FilteringOptions.FirstOrDefault(x => _targetComparisonEntry.ComparisonMethod.Equals(x.TargetEnum));
            comboBoxCompareMethod.SelectedItem = option ?? FilteringOptions[0];

            comboBox1.SelectedIndex = string.IsNullOrEmpty(_targetComparisonEntry.TargetPropertyId)
                ? 0
                : Math.Max(0, comboBox1.Items.IndexOf(PropertyTargets[_targetComparisonEntry.TargetPropertyId].DisplayName));

            if (textBoxFilterText.Text != _targetComparisonEntry.FilterText)
                textBoxFilterText.Text = _targetComparisonEntry.FilterText;

            if (searchBox1.SearchString != _targetComparisonEntry.FilterText)
            {
                searchBox1.SearchTextChanged -= searchBox1_SearchTextChanged;
                searchBox1.Search(_targetComparisonEntry.FilterText);
                searchBox1.SearchTextChanged += searchBox1_SearchTextChanged;
            }

            if (checkBoxInvert.Checked != _targetComparisonEntry.InvertResults)
                checkBoxInvert.Checked = _targetComparisonEntry.InvertResults;
        }

        [DefaultValue(false)]
        public bool ShowAsSearch
        {
            get { return searchBox1.Visible; }
            set
            {
                searchBox1.Visible = value;
                searchBox1.Enabled = value;
                labelText.Visible = !value;
                textBoxFilterText.Visible = !value;
                textBoxFilterText.Enabled = !value;
            }
        }


        /// <summary>
        /// Fires after any part of the edited comparison method is changed.
        /// </summary>
        public event EventHandler ComparisonMethodChanged;

        private void OnComparisonMethodChanged(object sender, EventArgs e)
        {
            if (Enabled)
                ComparisonMethodChanged?.Invoke(sender, e);
        }

        public void FocusSearchbox()
        {
            if (searchBox1.Visible)
                searchBox1.FocusSearchBox();
            else
                textBoxFilterText.Focus();
        }

        public void Search(string searchStr, FilterComparisonMethod method, bool negate = false)
        {
            _targetComparisonEntry.ComparisonMethod = method;
            _targetComparisonEntry.InvertResults = negate;

            searchBox1.SearchTextChanged -= searchBox1_SearchTextChanged;
            searchBox1.Search(searchStr);
            searchBox1.SearchTextChanged += searchBox1_SearchTextChanged;

            RefreshEditor();
            OnComparisonMethodChanged(this, EventArgs.Empty);
        }

        private void comboBoxCompareMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            var localisedEnumWrapper = comboBoxCompareMethod.SelectedItem as LocalisedEnumWrapper;
            if (localisedEnumWrapper == null || _targetComparisonEntry == null
                || _targetComparisonEntry.ComparisonMethod == (FilterComparisonMethod)localisedEnumWrapper.TargetEnum)
                return;

            _targetComparisonEntry.ComparisonMethod = (FilterComparisonMethod)localisedEnumWrapper.TargetEnum;
            OnComparisonMethodChanged(sender, e);
        }

        private void searchBox1_SearchTextChanged(SearchBox arg1, EventArgs arg2)
        {
            if (_targetComparisonEntry == null || _targetComparisonEntry.FilterText == arg1.SearchString) return;

            _targetComparisonEntry.FilterText = arg1.SearchString;
            OnComparisonMethodChanged(arg1, arg2);
        }

        private void textBoxFilterText_TextChanged(object sender, EventArgs e)
        {
            if (_targetComparisonEntry == null || _targetComparisonEntry.FilterText == textBoxFilterText.Text) return;

            _targetComparisonEntry.FilterText = textBoxFilterText.Text;
            OnComparisonMethodChanged(sender, e);
        }

        private void checkBoxInvert_CheckedChanged(object sender, EventArgs e)
        {
            if (_targetComparisonEntry == null || _targetComparisonEntry.InvertResults == checkBoxInvert.Checked) return;

            _targetComparisonEntry.InvertResults = checkBoxInvert.Checked;
            OnComparisonMethodChanged(sender, e);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_targetComparisonEntry == null)
                return;

            textBoxFilterText.AutoCompleteCustomSource.Clear();
            var targetInfo = SelectedTargetInfo;
            if (targetInfo == null)
                return;

            if (targetInfo.PossibleStrings != null)
            {
                textBoxFilterText.AutoCompleteCustomSource.AddRange(SelectedTargetInfo.PossibleStrings);
                //textBoxFilterText.Text = SelectedTargetInfo.PossibleStrings.First();
            }

            if (_targetComparisonEntry.TargetPropertyId
                .Equals(targetInfo.Id, StringComparison.InvariantCultureIgnoreCase)) return;

            _targetComparisonEntry.TargetPropertyId = targetInfo.Id;
            OnComparisonMethodChanged(sender, e);
        }
    }
}
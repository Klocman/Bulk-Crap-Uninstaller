using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Klocman.Controls;
using Klocman.Events;
using Klocman.Localising;
using UninstallTools.Lists;

namespace UninstallTools.Controls
{
    public partial class FilterEditor : UserControl
    {
        private static LocalisedEnumWrapper[] _filteringOptions;
        private FilterComparisonMethod _comparisonMethod = FilterComparisonMethod.Equals;
        private string _filterText = string.Empty;

        public FilterEditor()
        {
            InitializeComponent();

            // Initialize the combo box
            comboBoxCompareMethod.Items.AddRange(FilteringOptions);
            comboBoxCompareMethod.SelectedIndex = 0;
        }

        [Browsable(false)]
        [ReadOnly(true)]
        public FilterComparisonMethod ComparisonMethod
        {
            get { return _comparisonMethod; }
            set
            {
                comboBoxCompareMethod.SelectedIndexChanged -= comboBoxCompareMethod_SelectedIndexChanged;
                var option = FilteringOptions.FirstOrDefault(x => value.Equals(x.TargetEnum));
                comboBoxCompareMethod.SelectedItem = option ?? FilteringOptions[0];
                comboBoxCompareMethod.SelectedIndexChanged += comboBoxCompareMethod_SelectedIndexChanged;

                if (_comparisonMethod != value)
                {
                    _comparisonMethod = value;
                    ComparisonMethodChanged?.Invoke(this, new PropertyChangedEventArgs<FilterComparisonMethod>(value));
                }
            }
        }

        [Browsable(false)]
        [ReadOnly(true)]
        public string FilterText
        {
            get { return _filterText; }
            set
            {
                textBoxFilterText.TextChanged -= textBoxFilterText_TextChanged;
                textBoxFilterText.Text = value;
                textBoxFilterText.TextChanged += textBoxFilterText_TextChanged;

                if (searchBox1.SearchString != value)
                {
                    searchBox1.SearchTextChanged -= searchBox1_SearchTextChanged;
                    searchBox1.Search(value);
                    searchBox1.SearchTextChanged += searchBox1_SearchTextChanged;
                }

                if (_filterText != value)
                {
                    _filterText = value;
                    FilterTextChanged?.Invoke(this, new PropertyChangedEventArgs<string>(value));
                }
            }
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

        private static LocalisedEnumWrapper[] FilteringOptions
        {
            get
            {
                return _filteringOptions ?? (_filteringOptions = Enum.GetValues(typeof (FilterComparisonMethod))
                    .Cast<FilterComparisonMethod>().Select(x => new LocalisedEnumWrapper(x)).ToArray());
            }
        }

        public event EventHandler<PropertyChangedEventArgs<FilterComparisonMethod>> ComparisonMethodChanged;
        public event EventHandler<PropertyChangedEventArgs<string>> FilterTextChanged;

        public void FocusSearchbox()
        {
            searchBox1.FocusSearchBox();
        }

        public void Search(string searchStr, FilterComparisonMethod method)
        {
            ComparisonMethod = method;
            searchBox1.Search(searchStr);
        }

        private void comboBoxCompareMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            var localisedEnumWrapper = comboBoxCompareMethod.SelectedItem as LocalisedEnumWrapper;
            if (localisedEnumWrapper != null)
                ComparisonMethod = (FilterComparisonMethod) localisedEnumWrapper.TargetEnum;
        }

        private void searchBox1_SearchTextChanged(SearchBox arg1, EventArgs arg2)
        {
            FilterText = searchBox1.SearchString;
        }

        private void textBoxFilterText_TextChanged(object sender, EventArgs e)
        {
            FilterText = textBoxFilterText.Text;
        }
    }
}
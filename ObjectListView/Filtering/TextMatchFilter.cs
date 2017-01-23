/*
 * TextMatchFilter - Text based filtering on ObjectListViews
 *
 * Author: Phillip Piper
 * Date: 31/05/2011 7:45am 
 *
 * Change log:
 * v2.6
 * 2012-10-13  JPP  Allow filtering to consider additional columns
 * v2.5.1
 * 2011-06-22  JPP  Handle searching for empty strings
 * v2.5.0
 * 2011-05-31  JPP  Initial version
 *
 * TO DO:
 *
 * Copyright (C) 2011-2014 Phillip Piper
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 * If you wish to use this code in a closed source application, please contact phillip.piper@gmail.com.
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;

namespace BrightIdeasSoftware {

    /// <summary>
    /// Instances of this class include only those rows of the listview
    /// that match one or more given strings.
    /// </summary>
    /// <remarks>This class can match strings by prefix, regex, or simple containment.
    /// There are factory methods for each of these matching strategies.</remarks>
    public class TextMatchFilter : AbstractModelFilter {

        #region Life and death

        /// <summary>
        /// Create a text filter that will include rows where any cell matches 
        /// any of the given regex expressions.
        /// </summary>
        /// <param name="olv"></param>
        /// <param name="texts"></param>
        /// <returns></returns>
        /// <remarks>Any string that is not a valid regex expression will be ignored.</remarks>
        public static TextMatchFilter Regex(ObjectListView olv, params string[] texts) {
            TextMatchFilter filter = new TextMatchFilter(olv);
            filter.RegexStrings = texts;
            return filter;
        }

        /// <summary>
        /// Create a text filter that includes rows where any cell begins with one of the given strings
        /// </summary>
        /// <param name="olv"></param>
        /// <param name="texts"></param>
        /// <returns></returns>
        public static TextMatchFilter Prefix(ObjectListView olv, params string[] texts) {
            TextMatchFilter filter = new TextMatchFilter(olv);
            filter.PrefixStrings = texts;
            return filter;
        }

        /// <summary>
        /// Create a text filter that includes rows where any cell contains any of the given strings.
        /// </summary>
        /// <param name="olv"></param>
        /// <param name="texts"></param>
        /// <returns></returns>
        public static TextMatchFilter Contains(ObjectListView olv, params string[] texts) {
            TextMatchFilter filter = new TextMatchFilter(olv);
            filter.ContainsStrings = texts;
            return filter;
        }

        /// <summary>
        /// Create a TextFilter
        /// </summary>
        /// <param name="olv"></param>
        public TextMatchFilter(ObjectListView olv) {
            this.ListView = olv;
        }

        /// <summary>
        /// Create a TextFilter that finds the given string
        /// </summary>
        /// <param name="olv"></param>
        /// <param name="text"></param>
        public TextMatchFilter(ObjectListView olv, string text) {
            this.ListView = olv;
            this.ContainsStrings = new string[] { text };
        }

        /// <summary>
        /// Create a TextFilter that finds the given string using the given comparison
        /// </summary>
        /// <param name="olv"></param>
        /// <param name="text"></param>
        /// <param name="comparison"></param>
        public TextMatchFilter(ObjectListView olv, string text, StringComparison comparison) {
            this.ListView = olv;
            this.ContainsStrings = new string[] { text };
            this.StringComparison = comparison;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets which columns will be used for the comparisons? If this is null, all columns will be used
        /// </summary>
        public OLVColumn[] Columns {
            get { return columns; }
            set { columns = value; }
        }
        private OLVColumn[] columns;

        /// <summary>
        /// Gets or sets additional columns which will be used in the comparison. These will be used
        /// in addition to either the Columns property or to all columns taken from the control.
        /// </summary>
        public OLVColumn[] AdditionalColumns {
            get { return additionalColumns; }
            set { additionalColumns = value; }
        }
        private OLVColumn[] additionalColumns;

        /// <summary>
        /// Gets or sets the collection of strings that will be used for 
        /// contains matching. Setting this replaces all previous texts
        /// of any kind.
        /// </summary>
        public IEnumerable<string> ContainsStrings {
            get {
                foreach (TextMatchingStrategy component in this.MatchingStrategies)
                    yield return component.Text;
            }
            set {
                this.MatchingStrategies = new List<TextMatchingStrategy>();
                if (value != null) {
                    foreach (string text in value)
                        this.MatchingStrategies.Add(new TextContainsMatchingStrategy(this, text));
                }
            }
        }

        /// <summary>
        /// Gets whether or not this filter has any search criteria
        /// </summary>
        public bool HasComponents {
            get {
                return this.MatchingStrategies.Count > 0;
            }
        }

        /// <summary>
        /// Gets or set the ObjectListView upon which this filter will work
        /// </summary>
        /// <remarks>
        /// You cannot really rebase a filter after it is created, so do not change this value.
        /// It is included so that it can be set in an object initializer.
        /// </remarks>
        public ObjectListView ListView {
            get { return listView; }
            set { listView = value; }
        }
        private ObjectListView listView;

        /// <summary>
        /// Gets or sets the collection of strings that will be used for 
        /// prefix matching. Setting this replaces all previous texts
        /// of any kind.
        /// </summary>
        public IEnumerable<string> PrefixStrings {
            get {
                foreach (TextMatchingStrategy component in this.MatchingStrategies)
                    yield return component.Text;
            }
            set {
                this.MatchingStrategies = new List<TextMatchingStrategy>();
                if (value != null) {
                    foreach (string text in value)
                        this.MatchingStrategies.Add(new TextBeginsMatchingStrategy(this, text));
                }
            }
        }

        /// <summary>
        /// Gets or sets the options that will be used when compiling the regular expression.
        /// </summary>
        /// <remarks>
        /// This is only used when doing Regex matching (obviously).
        /// If this is not set specifically, the appropriate options are chosen to match the
        /// StringComparison setting (culture invariant, case sensitive).
        /// </remarks>
        public RegexOptions RegexOptions {
            get {
                if (!regexOptions.HasValue) {
                    switch (this.StringComparison) {
                        case StringComparison.CurrentCulture:
                            regexOptions = RegexOptions.None;
                            break;
                        case StringComparison.CurrentCultureIgnoreCase:
                            regexOptions = RegexOptions.IgnoreCase;
                            break;
                        case StringComparison.Ordinal:
                        case StringComparison.InvariantCulture:
                            regexOptions = RegexOptions.CultureInvariant;
                            break;
                        case StringComparison.OrdinalIgnoreCase:
                        case StringComparison.InvariantCultureIgnoreCase:
                            regexOptions = RegexOptions.CultureInvariant | RegexOptions.IgnoreCase;
                            break;
                        default:
                            regexOptions = RegexOptions.None;
                            break;
                    }
                }
                return regexOptions.Value;
            }
            set {
                regexOptions = value;
            }
        }
        private RegexOptions? regexOptions;

        /// <summary>
        /// Gets or sets the collection of strings that will be used for 
        /// regex pattern matching. Setting this replaces all previous texts
        /// of any kind.
        /// </summary>
        public IEnumerable<string> RegexStrings {
            get {
                foreach (TextMatchingStrategy component in this.MatchingStrategies)
                    yield return component.Text;
            }
            set {
                this.MatchingStrategies = new List<TextMatchingStrategy>();
                if (value != null) {
                    foreach (string text in value)
                        this.MatchingStrategies.Add(new TextRegexMatchingStrategy(this, text));
                }
            }
        }

        /// <summary>
        /// Gets or  sets how the filter will match text
        /// </summary>
        public StringComparison StringComparison {
            get { return this.stringComparison; }
            set { this.stringComparison = value; }
        }
        private StringComparison stringComparison = StringComparison.InvariantCultureIgnoreCase;

        #endregion

        #region Implementation

        /// <summary>
        /// Loop over the columns that are being considering by the filter
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<OLVColumn> IterateColumns() {
            if (this.Columns == null) {
                foreach (OLVColumn column in this.ListView.Columns)
                    yield return column;
            } else {
                foreach (OLVColumn column in this.Columns)
                    yield return column;
            }
            if (this.AdditionalColumns != null) {
                foreach (OLVColumn column in this.AdditionalColumns)
                    yield return column;
            }
        }

        #endregion

        #region Public interface

        /// <summary>
        /// Do the actual work of filtering
        /// </summary>
        /// <param name="modelObject"></param>
        /// <returns></returns>
        public override bool Filter(object modelObject) {
            if (this.ListView == null || !this.HasComponents)
                return true;

            foreach (OLVColumn column in this.IterateColumns()) {
                if (column.IsVisible && column.Searchable) {
                    string[] cellTexts = column.GetSearchValues(modelObject);
                    if (cellTexts != null && cellTexts.Length > 0) {
                        foreach (TextMatchingStrategy filter in this.MatchingStrategies) {
                            if (String.IsNullOrEmpty(filter.Text))
                                return true;
                            foreach (string cellText in cellTexts) {
                                if (filter.MatchesText(cellText))
                                    return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Find all the ways in which this filter matches the given string.
        /// </summary>
        /// <remarks>This is used by the renderer to decide which bits of
        /// the string should be highlighted</remarks>
        /// <param name="cellText"></param>
        /// <returns>A list of character ranges indicating the matched substrings</returns>
        public IEnumerable<CharacterRange> FindAllMatchedRanges(string cellText) {
            List<CharacterRange> ranges = new List<CharacterRange>();

            foreach (TextMatchingStrategy filter in this.MatchingStrategies) {
                 if (!String.IsNullOrEmpty(filter.Text))
                    ranges.AddRange(filter.FindAllMatchedRanges(cellText));
            }

            return ranges;
        }

        /// <summary>
        /// Is the given column one of the columns being used by this filter?
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public bool IsIncluded(OLVColumn column) {
            if (this.Columns == null) {
                return column.ListView == this.ListView;
            }

            foreach (OLVColumn x in this.Columns) {
                if (x == column)
                    return true;
            }

            return false;
        }

        #endregion

        #region Implementation members

        private List<TextMatchingStrategy> MatchingStrategies = new List<TextMatchingStrategy>();

        #endregion

        #region Components

        /// <summary>
        /// Base class for the various types of string matching that TextMatchFilter provides
        /// </summary>
        abstract protected class TextMatchingStrategy {

            /// <summary>
            /// Gets how the filter will match text
            /// </summary>
            public StringComparison StringComparison {
                get { return this.TextFilter.StringComparison; }
            }

            /// <summary>
            /// Gets the text filter to which this component belongs
            /// </summary>
            public TextMatchFilter TextFilter {
                get { return textFilter; }
                set { textFilter = value; }
            }
            private TextMatchFilter textFilter;

            /// <summary>
            /// Gets or sets the text that will be matched
            /// </summary>
            public string Text {
                get { return this.text; }
                set { this.text = value; }
            }
            private string text;

            /// <summary>
            /// Find all the ways in which this filter matches the given string.
            /// </summary>
            /// <remarks>
            /// <para>
            /// This is used by the renderer to decide which bits of
            /// the string should be highlighted.
            /// </para>
            /// <para>this.Text will not be null or empty when this is called.</para>
            /// </remarks>
            /// <param name="cellText">The text of the cell we want to search</param>
            /// <returns>A list of character ranges indicating the matched substrings</returns>
            abstract public IEnumerable<CharacterRange> FindAllMatchedRanges(string cellText);

            /// <summary>
            /// Does the given text match the filter
            /// </summary>
            /// <remarks>
            /// <para>this.Text will not be null or empty when this is called.</para>
            /// </remarks>
            /// <param name="cellText">The text of the cell we want to search</param>
            /// <returns>Return true if the given cellText matches our strategy</returns>
            abstract public bool MatchesText(string cellText);
        }

        /// <summary>
        /// This component provides text contains matching strategy.
        /// </summary>
        protected class TextContainsMatchingStrategy : TextMatchingStrategy {

            /// <summary>
            /// Create a text contains strategy
            /// </summary>
            /// <param name="filter"></param>
            /// <param name="text"></param>
            public TextContainsMatchingStrategy(TextMatchFilter filter, string text) {
                this.TextFilter = filter;
                this.Text = text;
            }

            /// <summary>
            /// Does the given text match the filter
            /// </summary>
            /// <remarks>
            /// <para>this.Text will not be null or empty when this is called.</para>
            /// </remarks>
            /// <param name="cellText">The text of the cell we want to search</param>
            /// <returns>Return true if the given cellText matches our strategy</returns>
            override public bool MatchesText(string cellText) {
                return cellText.IndexOf(this.Text, this.StringComparison) != -1;
            }

            /// <summary>
            /// Find all the ways in which this filter matches the given string.
            /// </summary>
            /// <remarks>
            /// <para>
            /// This is used by the renderer to decide which bits of
            /// the string should be highlighted.
            /// </para>
            /// <para>this.Text will not be null or empty when this is called.</para>
            /// </remarks>
            /// <param name="cellText">The text of the cell we want to search</param>
            /// <returns>A list of character ranges indicating the matched substrings</returns>
            override public IEnumerable<CharacterRange> FindAllMatchedRanges(string cellText) {
                List<CharacterRange> ranges = new List<CharacterRange>();

                int matchIndex = cellText.IndexOf(this.Text, this.StringComparison);
                while (matchIndex != -1) {
                    ranges.Add(new CharacterRange(matchIndex, this.Text.Length));
                    matchIndex = cellText.IndexOf(this.Text, matchIndex + this.Text.Length, this.StringComparison);
                }

                return ranges;
            }
        }

        /// <summary>
        /// This component provides text begins with matching strategy.
        /// </summary>
        protected class TextBeginsMatchingStrategy : TextMatchingStrategy {

            /// <summary>
            /// Create a text begins strategy
            /// </summary>
            /// <param name="filter"></param>
            /// <param name="text"></param>
            public TextBeginsMatchingStrategy(TextMatchFilter filter, string text) {
                this.TextFilter = filter;
                this.Text = text;
            }

            /// <summary>
            /// Does the given text match the filter
            /// </summary>
            /// <remarks>
            /// <para>this.Text will not be null or empty when this is called.</para>
            /// </remarks>
            /// <param name="cellText">The text of the cell we want to search</param>
            /// <returns>Return true if the given cellText matches our strategy</returns>
            override public bool MatchesText(string cellText) {
                return cellText.StartsWith(this.Text, this.StringComparison);
            }

            /// <summary>
            /// Find all the ways in which this filter matches the given string.
            /// </summary>
            /// <remarks>
            /// <para>
            /// This is used by the renderer to decide which bits of
            /// the string should be highlighted.
            /// </para>
            /// <para>this.Text will not be null or empty when this is called.</para>
            /// </remarks>
            /// <param name="cellText">The text of the cell we want to search</param>
            /// <returns>A list of character ranges indicating the matched substrings</returns>
            override public IEnumerable<CharacterRange> FindAllMatchedRanges(string cellText) {
                List<CharacterRange> ranges = new List<CharacterRange>();

                if (cellText.StartsWith(this.Text, this.StringComparison))
                    ranges.Add(new CharacterRange(0, this.Text.Length));

                return ranges;
            }

        }

        /// <summary>
        /// This component provides regex matching strategy.
        /// </summary>
        protected class TextRegexMatchingStrategy : TextMatchingStrategy {

            /// <summary>
            /// Creates a regex strategy
            /// </summary>
            /// <param name="filter"></param>
            /// <param name="text"></param>
            public TextRegexMatchingStrategy(TextMatchFilter filter, string text) {
                this.TextFilter = filter;
                this.Text = text;
            }

            /// <summary>
            /// Gets or sets the options that will be used when compiling the regular expression.
            /// </summary>
            public RegexOptions RegexOptions {
                get {
                    return this.TextFilter.RegexOptions;
                }
            }

            /// <summary>
            /// Gets or sets a compilex regular expression, based on our current Text and RegexOptions.
            /// </summary>
            /// <remarks>
            /// If Text fails to compile as a regular expression, this will return a Regex object
            /// that will match all strings.
            /// </remarks>
            protected Regex Regex {
                get {
                    if (this.regex == null) {
                        try {
                            this.regex = new Regex(this.Text, this.RegexOptions);
                        }
                        catch (ArgumentException) {
                            this.regex = TextRegexMatchingStrategy.InvalidRegexMarker;
                        }
                    }
                    return this.regex;
                }
                set {
                    this.regex = value;
                }
            }
            private Regex regex;

            /// <summary>
            /// Gets whether or not our current regular expression is a valid regex
            /// </summary>
            protected bool IsRegexInvalid {
                get {
                    return this.Regex == TextRegexMatchingStrategy.InvalidRegexMarker;
                }
            }
            static private Regex InvalidRegexMarker = new Regex(".*");

            /// <summary>
            /// Does the given text match the filter
            /// </summary>
            /// <remarks>
            /// <para>this.Text will not be null or empty when this is called.</para>
            /// </remarks>
            /// <param name="cellText">The text of the cell we want to search</param>
            /// <returns>Return true if the given cellText matches our strategy</returns>
            public override bool MatchesText(string cellText) {
                if (this.IsRegexInvalid)
                    return true;
                return this.Regex.Match(cellText).Success;
            }

            /// <summary>
            /// Find all the ways in which this filter matches the given string.
            /// </summary>
            /// <remarks>
            /// <para>
            /// This is used by the renderer to decide which bits of
            /// the string should be highlighted.
            /// </para>
            /// <para>this.Text will not be null or empty when this is called.</para>
            /// </remarks>
            /// <param name="cellText">The text of the cell we want to search</param>
            /// <returns>A list of character ranges indicating the matched substrings</returns>
            override public IEnumerable<CharacterRange> FindAllMatchedRanges(string cellText) {
                List<CharacterRange> ranges = new List<CharacterRange>();

                if (!this.IsRegexInvalid) {
                    foreach (Match match in this.Regex.Matches(cellText)) {
                        if (match.Length > 0)
                            ranges.Add(new CharacterRange(match.Index, match.Length));
                    }
                }

                return ranges;
            }
        }

        #endregion
    }
}

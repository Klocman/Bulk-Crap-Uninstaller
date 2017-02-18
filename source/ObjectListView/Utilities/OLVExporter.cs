/*
 * OLVExporter - Export the contents of an ObjectListView into various text-based formats
 *
 * Author: Phillip Piper
 * Date: 7 August 2012, 10:35pm
 *
 * Change log:
 * 2012-08-07  JPP  Initial code
 * 
 * Copyright (C) 2012 Phillip Piper
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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace BrightIdeasSoftware {
    /// <summary>
    /// An OLVExporter converts a collection of rows from an ObjectListView
    /// into a variety of textual formats.
    /// </summary>
    public class OLVExporter {

        /// <summary>
        /// What format will be used for exporting
        /// </summary>
        public enum ExportFormat {

            /// <summary>
            /// Tab separated values, according to http://www.iana.org/assignments/media-types/text/tab-separated-values
            /// </summary>
            TabSeparated = 1,

            /// <summary>
            /// Alias for TabSeparated
            /// </summary>
            TSV = 1,

            /// <summary>
            /// Comma separated values, according to http://www.ietf.org/rfc/rfc4180.txt
            /// </summary>
            CSV,

            /// <summary>
            /// HTML table, according to me
            /// </summary>
            HTML
        }

        #region Life and death

        /// <summary>
        /// Create an empty exporter
        /// </summary>
        public OLVExporter() {}

        /// <summary>
        /// Create an exporter that will export all the rows of the given ObjectListView
        /// </summary>
        /// <param name="olv"></param>
        public OLVExporter(ObjectListView olv) : this(olv, olv.Objects) {}

        /// <summary>
        /// Create an exporter that will export all the given rows from the given ObjectListView
        /// </summary>
        /// <param name="olv"></param>
        /// <param name="objectsToExport"></param>
        public OLVExporter(ObjectListView olv, IEnumerable objectsToExport) {
            if (olv == null) throw new ArgumentNullException("olv");
            if (objectsToExport == null) throw new ArgumentNullException("objectsToExport");

            this.ListView = olv;
            this.ModelObjects = ObjectListView.EnumerableToArray(objectsToExport, true);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets whether hidden columns will also be included in the textual
        /// representation. If this is false (the default), only visible columns will
        /// be included.
        /// </summary>
        public bool IncludeHiddenColumns {
            get { return includeHiddenColumns; }
            set { includeHiddenColumns = value; }
        }
        private bool includeHiddenColumns;

        /// <summary>
        /// Gets or sets whether column headers will also be included in the text
        /// and HTML representation. Default is true.
        /// </summary>
        public bool IncludeColumnHeaders {
            get { return includeColumnHeaders; }
            set { includeColumnHeaders = value; }
        }
        private bool includeColumnHeaders = true;

        /// <summary>
        /// Gets the ObjectListView that is being used as the source of the data
        /// to be exported
        /// </summary>
        public ObjectListView ListView {
            get { return objectListView; }
            set { objectListView = value; }
        }
        private ObjectListView objectListView;

        /// <summary>
        /// Gets the model objects that are to be placed in the data object
        /// </summary>
        public IList ModelObjects {
            get { return modelObjects; }
            set { modelObjects = value; }
        }
        private IList modelObjects = new ArrayList();

        #endregion

        #region Commands

        /// <summary>
        /// Export the nominated rows from the nominated ObjectListView.
        /// Returns the result in the expected format.
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        /// <remarks>This will perform only one conversion, even if called multiple times with different formats.</remarks>
        public string ExportTo(ExportFormat format) {
            if (results == null)
                this.Convert();

            return results[format];
        }

        /// <summary>
        /// Convert 
        /// </summary>
        public void Convert() {

            IList<OLVColumn> columns = this.IncludeHiddenColumns ? this.ListView.AllColumns : this.ListView.ColumnsInDisplayOrder;

            StringBuilder sbText = new StringBuilder();
            StringBuilder sbCsv = new StringBuilder();
            StringBuilder sbHtml = new StringBuilder("<table>");

            // Include column headers
            if (this.IncludeColumnHeaders) {
                List<string> strings = new List<string>();
                foreach (OLVColumn col in columns) 
                    strings.Add(col.Text);

                WriteOneRow(sbText, strings, "", "\t", "", null);
                WriteOneRow(sbHtml, strings, "<tr><td>", "</td><td>", "</td></tr>", HtmlEncode);
                WriteOneRow(sbCsv, strings, "", ",", "", CsvEncode);
            }

            foreach (object modelObject in this.ModelObjects) {
                List<string> strings = new List<string>();
                foreach (OLVColumn col in columns)
                    strings.Add(col.GetStringValue(modelObject));

                WriteOneRow(sbText, strings, "", "\t", "", null);
                WriteOneRow(sbHtml, strings, "<tr><td>", "</td><td>", "</td></tr>", HtmlEncode);
                WriteOneRow(sbCsv, strings, "", ",", "", CsvEncode);
            }
            sbHtml.AppendLine("</table>");

            results = new Dictionary<ExportFormat, string>();
            results[ExportFormat.TabSeparated] = sbText.ToString();
            results[ExportFormat.CSV] = sbCsv.ToString();
            results[ExportFormat.HTML] = sbHtml.ToString();
        }

        private delegate string StringToString(string str);

        private void WriteOneRow(StringBuilder sb, IEnumerable<string> strings, string startRow, string betweenCells, string endRow, StringToString encoder) {
            sb.Append(startRow);
            bool first = true;
            foreach (string s in strings) {
                if (!first)
                    sb.Append(betweenCells);
                sb.Append(encoder == null ? s : encoder(s));
                first = false;
            }
            sb.AppendLine(endRow);
        }

        private Dictionary<ExportFormat, string> results; 

        #endregion

        #region Encoding

        /// <summary>
        /// Encode a string such that it can be used as a value in a CSV file.
        /// This basically means replacing any quote mark with two quote marks,
        /// and enclosing the whole string in quotes.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private static string CsvEncode(string text) {
            if (text == null)
                return null;

            const string DOUBLEQUOTE = @""""; // one double quote
            const string TWODOUBEQUOTES = @""""""; // two double quotes

            StringBuilder sb = new StringBuilder(DOUBLEQUOTE);
            sb.Append(text.Replace(DOUBLEQUOTE, TWODOUBEQUOTES));
            sb.Append(DOUBLEQUOTE);

            return sb.ToString();
        }

        /// <summary>
        /// HTML-encodes a string and returns the encoded string.
        /// </summary>
        /// <param name="text">The text string to encode. </param>
        /// <returns>The HTML-encoded text.</returns>
        /// <remarks>Taken from http://www.west-wind.com/weblog/posts/2009/Feb/05/Html-and-Uri-String-Encoding-without-SystemWeb</remarks>
        private static string HtmlEncode(string text) {
            if (text == null)
                return null;

            StringBuilder sb = new StringBuilder(text.Length);

            int len = text.Length;
            for (int i = 0; i < len; i++) {
                switch (text[i]) {
                    case '<':
                        sb.Append("&lt;");
                        break;
                    case '>':
                        sb.Append("&gt;");
                        break;
                    case '"':
                        sb.Append("&quot;");
                        break;
                    case '&':
                        sb.Append("&amp;");
                        break;
                    default:
                        if (text[i] > 159) {
                            // decimal numeric entity
                            sb.Append("&#");
                            sb.Append(((int)text[i]).ToString(CultureInfo.InvariantCulture));
                            sb.Append(";");
                        } else
                            sb.Append(text[i]);
                        break;
                }
            }
            return sb.ToString();
        }
        #endregion
    }
}
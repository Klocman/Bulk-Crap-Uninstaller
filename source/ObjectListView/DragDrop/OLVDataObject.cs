/*
 * OLVDataObject.cs - An OLE DataObject that knows how to convert rows of an OLV to text and HTML
 *
 * Author: Phillip Piper
 * Date: 2011-03-29 3:34PM
 *
 * Change log:
 * v2.8
 * 2014-05-02   JPP  - When the listview is completely empty, don't try to set CSV text in the clipboard.
 * v2.6
 * 2012-08-08   JPP  - Changed to use OLVExporter.
 *                   - Added CSV to formats exported to Clipboard
 * v2.4
 * 2011-03-29   JPP  - Initial version
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
using System.Collections;
using System.Windows.Forms;

namespace BrightIdeasSoftware {
    
    /// <summary>
    /// A data transfer object that knows how to transform a list of model
    /// objects into a text and HTML representation.
    /// </summary>
    public class OLVDataObject : DataObject {
        #region Life and death

        /// <summary>
        /// Create a data object from the selected objects in the given ObjectListView
        /// </summary>
        /// <param name="olv">The source of the data object</param>
        public OLVDataObject(ObjectListView olv)
            : this(olv, olv.SelectedObjects) {
        }

        /// <summary>
        /// Create a data object which operates on the given model objects 
        /// in the given ObjectListView
        /// </summary>
        /// <param name="olv">The source of the data object</param>
        /// <param name="modelObjects">The model objects to be put into the data object</param>
        public OLVDataObject(ObjectListView olv, IList modelObjects) {
            this.objectListView = olv;
            this.modelObjects = modelObjects;
            this.includeHiddenColumns = olv.IncludeHiddenColumnsInDataTransfer;
            this.includeColumnHeaders = olv.IncludeColumnHeadersInCopy;
            this.CreateTextFormats();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets whether hidden columns will also be included in the text
        /// and HTML representation. If this is false, only visible columns will
        /// be included.
        /// </summary>
        public bool IncludeHiddenColumns {
            get { return includeHiddenColumns; }
        }
        private readonly bool includeHiddenColumns;

        /// <summary>
        /// Gets or sets whether column headers will also be included in the text
        /// and HTML representation.
        /// </summary>
        public bool IncludeColumnHeaders {
            get { return includeColumnHeaders; }
        }
        private readonly bool includeColumnHeaders;

        /// <summary>
        /// Gets the ObjectListView that is being used as the source of the data
        /// </summary>
        public ObjectListView ListView {
            get { return objectListView; }
        }
        private readonly ObjectListView objectListView;

        /// <summary>
        /// Gets the model objects that are to be placed in the data object
        /// </summary>
        public IList ModelObjects {
            get { return modelObjects; }
        }
        private readonly IList modelObjects;

        #endregion

        /// <summary>
        /// Put a text and HTML representation of our model objects
        /// into the data object.
        /// </summary>
        public void CreateTextFormats() {

            OLVExporter exporter = this.CreateExporter();

            // Put both the text and html versions onto the clipboard.
            // For some reason, SetText() with UnicodeText doesn't set the basic CF_TEXT format,
            // but using SetData() does.
            //this.SetText(sbText.ToString(), TextDataFormat.UnicodeText);
            this.SetData(exporter.ExportTo(OLVExporter.ExportFormat.TabSeparated));
            string exportTo = exporter.ExportTo(OLVExporter.ExportFormat.CSV);
            if (!String.IsNullOrEmpty(exportTo))
                this.SetText(exportTo, TextDataFormat.CommaSeparatedValue);
            this.SetText(ConvertToHtmlFragment(exporter.ExportTo(OLVExporter.ExportFormat.HTML)), TextDataFormat.Html);
        }

        /// <summary>
        /// Create an exporter for the data contained in this object
        /// </summary>
        /// <returns></returns>
        protected OLVExporter CreateExporter() {
            OLVExporter exporter = new OLVExporter(this.ListView);
            exporter.IncludeColumnHeaders = this.IncludeColumnHeaders;
            exporter.IncludeHiddenColumns = this.IncludeHiddenColumns;
            exporter.ModelObjects = this.ModelObjects;
            return exporter;
        }

        /// <summary>
        /// Make a HTML representation of our model objects
        /// </summary>
        [Obsolete("Use OLVExporter directly instead", false)]
        public string CreateHtml() {
            OLVExporter exporter = this.CreateExporter();
            return exporter.ExportTo(OLVExporter.ExportFormat.HTML);
        }

        /// <summary>
        /// Convert the fragment of HTML into the Clipboards HTML format.
        /// </summary>
        /// <remarks>The HTML format is found here http://msdn2.microsoft.com/en-us/library/aa767917.aspx
        /// </remarks>
        /// <param name="fragment">The HTML to put onto the clipboard. It must be valid HTML!</param>
        /// <returns>A string that can be put onto the clipboard and will be recognized as HTML</returns>
        private string ConvertToHtmlFragment(string fragment) {
            // Minimal implementation of HTML clipboard format
            const string SOURCE = "http://www.codeproject.com/Articles/16009/A-Much-Easier-to-Use-ListView";

            const String MARKER_BLOCK =
                "Version:1.0\r\n" +
                "StartHTML:{0,8}\r\n" +
                "EndHTML:{1,8}\r\n" +
                "StartFragment:{2,8}\r\n" +
                "EndFragment:{3,8}\r\n" +
                "StartSelection:{2,8}\r\n" +
                "EndSelection:{3,8}\r\n" +
                "SourceURL:{4}\r\n" +
                "{5}";

            int prefixLength = String.Format(MARKER_BLOCK, 0, 0, 0, 0, SOURCE, "").Length;

            const String DEFAULT_HTML_BODY =
                "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\">" +
                "<HTML><HEAD></HEAD><BODY><!--StartFragment-->{0}<!--EndFragment--></BODY></HTML>";

            string html = String.Format(DEFAULT_HTML_BODY, fragment);
            int startFragment = prefixLength + html.IndexOf(fragment, StringComparison.Ordinal);
            int endFragment = startFragment + fragment.Length;

            return String.Format(MARKER_BLOCK, prefixLength, prefixLength + html.Length, startFragment, endFragment, SOURCE, html);
        }
    }
}

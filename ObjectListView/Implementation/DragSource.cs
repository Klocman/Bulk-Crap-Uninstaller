/*
 * DragSource.cs - Add drag source functionality to an ObjectListView
 *
 * UNFINISHED
 * 
 * Author: Phillip Piper
 * Date: 2009-03-17 5:15 PM
 *
 * Change log:
 * v2.3
 * 2009-07-06   JPP  - Make sure Link is acceptable as an drop effect by default
 *                     (since MS didn't make it part of the 'All' value)
 * v2.2
 * 2009-04-15   JPP  - Separated DragSource.cs into DropSink.cs
 * 2009-03-17   JPP  - Initial version
 * 
 * Copyright (C) 2009 Phillip Piper
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
 * If you wish to use this code in a closed source application, please contact phillip_piper@bigfoot.com.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace BrightIdeasSoftware
{
    /// <summary>
    /// An IDragSource controls how drag out from the ObjectListView will behave
    /// </summary>
    public interface IDragSource
    {
        /// <summary>
        /// A drag operation is beginning. Return the data object that will be used 
        /// for data transfer. Return null to prevent the drag from starting.
        /// </summary>
        /// <remarks>
        /// The returned object is later passed to the GetAllowedEffect() and EndDrag()
        /// methods.
        /// </remarks>
        /// <param name="olv">What ObjectListView is being dragged from.</param>
        /// <param name="button">Which mouse button is down?</param>
        /// <param name="item">What item was directly dragged by the user? There may be more than just this 
        /// item selected.</param>
        /// <returns>The data object that will be used for data transfer. This will often be a subclass
        /// of DataObject, but does not need to be.</returns>
        Object StartDrag(ObjectListView olv, MouseButtons button, OLVListItem item);

        /// <summary>
        /// What operations are possible for this drag? This controls the icon shown during the drag
        /// </summary>
        /// <param name="dragObject">The data object returned by StartDrag()</param>
        /// <returns>A combination of DragDropEffects flags</returns>
        DragDropEffects GetAllowedEffects(Object dragObject);

        /// <summary>
        /// The drag operation is complete. Do whatever is necessary to complete the action.
        /// </summary>
        /// <param name="dragObject">The data object returned by StartDrag()</param>
        /// <param name="effect">The value returned from GetAllowedEffects()</param>
        void EndDrag(Object dragObject, DragDropEffects effect);
    }

    /// <summary>
    /// A do-nothing implementation of IDragSource that can be safely subclassed.
    /// </summary>
    public class AbstractDragSource : IDragSource
    {
        #region IDragSource Members

        /// <summary>
        /// See IDragSource documentation
        /// </summary>
        /// <param name="olv"></param>
        /// <param name="button"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual Object StartDrag(ObjectListView olv, MouseButtons button, OLVListItem item) {
            return null;
        }

        /// <summary>
        /// See IDragSource documentation
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual DragDropEffects GetAllowedEffects(Object data) {
            return DragDropEffects.None;
        }

        /// <summary>
        /// See IDragSource documentation
        /// </summary>
        /// <param name="dragObject"></param>
        /// <param name="effect"></param>
        public virtual void EndDrag(Object dragObject, DragDropEffects effect) {
        }

        #endregion
    }

    /// <summary>
    /// A reasonable implementation of IDragSource that provides normal
    /// drag source functionality. It creates a data object that supports
    /// inter-application dragging of text and HTML representation of 
    /// the dragged rows. It can optionally force a refresh of all dragged
    /// rows when the drag is complete.
    /// </summary>
    /// <remarks>Subclasses can override GetDataObject() to add new
    /// data formats to the data transfer object.</remarks>
    public class SimpleDragSource : IDragSource
    {
        #region Constructors

        /// <summary>
        /// Construct a SimpleDragSource
        /// </summary>
        public SimpleDragSource() {
        }

        /// <summary>
        /// Construct a SimpleDragSource that refreshes the dragged rows when
        /// the drag is complete
        /// </summary>
        /// <param name="refreshAfterDrop"></param>
        public SimpleDragSource(bool refreshAfterDrop) {
            this.RefreshAfterDrop = refreshAfterDrop;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets whether the dragged rows should be refreshed when the 
        /// drag operation is complete.
        /// </summary>
        public bool RefreshAfterDrop {
            get { return refreshAfterDrop; }
            set { refreshAfterDrop = value;  }
        }
        private bool refreshAfterDrop;

        #endregion

        #region IDragSource Members

        /// <summary>
        /// Create a DataObject when the user does a left mouse drag operation.
        /// See IDragSource for further information.
        /// </summary>
        /// <param name="olv"></param>
        /// <param name="button"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual Object StartDrag(ObjectListView olv, MouseButtons button, OLVListItem item) {
            // We only drag on left mouse
            if (button != MouseButtons.Left)
                return null;

            return this.CreateDataObject(olv);
        }

        /// <summary>
        /// Which operations are allowed in the operation? By default, all operations are supported.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>All opertions are supported</returns>
        public virtual DragDropEffects GetAllowedEffects(Object data) {
            return DragDropEffects.All | DragDropEffects.Link; // why didn't MS include 'Link' in 'All'??
        }

        /// <summary>
        /// The drag operation is finished. Refreshe the dragged rows if so configured.
        /// </summary>
        /// <param name="dragObject"></param>
        /// <param name="effect"></param>
        public virtual void EndDrag(Object dragObject, DragDropEffects effect) {
            OLVDataObject data = dragObject as OLVDataObject;
            if (data == null)
                return;

            if (this.RefreshAfterDrop)
                data.ListView.RefreshObjects(data.ModelObjects);
        }

        /// <summary>
        /// Create a data object that will be used to as the data object
        /// for the drag operation.
        /// </summary>
        /// <remarks>
        /// Subclasses can override this method add new formats to the data object.
        /// </remarks>
        /// <param name="olv">The ObjectListView that is the source of the drag</param>
        /// <returns>A data object for the drag</returns>
        protected virtual object CreateDataObject(ObjectListView olv) {
            OLVDataObject data = new OLVDataObject(olv);
            data.CreateTextFormats();
            return data;
        }

        #endregion
    }

    /// <summary>
    /// A data transfer object that knows how to transform a list of model
    /// objects into a text and HTML representation.
    /// </summary>
    public class OLVDataObject : DataObject
    {
        #region Life and death

        /// <summary>
        /// Create a data object from the selected objects in the given ObjectListView
        /// </summary>
        /// <param name="olv">The source of the data object</param>
        public OLVDataObject(ObjectListView olv) : this(olv, olv.SelectedObjects) {
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
        private bool includeHiddenColumns;

        /// <summary>
        /// Gets or sets whether column headers will also be included in the text
        /// and HTML representation.
        /// </summary>
        public bool IncludeColumnHeaders
        {
            get { return includeColumnHeaders; }
        }
        private bool includeColumnHeaders;

        /// <summary>
        /// Gets the ObjectListView that is being used as the source of the data
        /// </summary>
        public ObjectListView ListView {
            get { return objectListView; }
        }
        private ObjectListView objectListView;

        /// <summary>
        /// Gets the model objects that are to be placed in the data object
        /// </summary>
        public IList ModelObjects {
            get { return modelObjects; }
        }
        private IList modelObjects = new ArrayList();

        #endregion

        /// <summary>
        /// Put a text and HTML representation of our model objects
        /// into the data object.
        /// </summary>
        public void CreateTextFormats() {
            IList<OLVColumn> columns = this.IncludeHiddenColumns ? this.ListView.AllColumns : this.ListView.ColumnsInDisplayOrder;

            // Build text and html versions of the selection
            StringBuilder sbText = new StringBuilder();
            StringBuilder sbHtml = new StringBuilder("<table>");

            // Include column headers
            if (includeColumnHeaders)
            {
                sbHtml.Append("<tr><td>");
                foreach (OLVColumn col in columns)
                {
                    if (col != columns[0])
                    {
                        sbText.Append("\t");
                        sbHtml.Append("</td><td>");
                    }
                    string strValue = col.Text;
                    sbText.Append(strValue);
                    sbHtml.Append(strValue); //TODO: Should encode the string value
                }
                sbText.AppendLine();
                sbHtml.AppendLine("</td></tr>");
            }

            foreach (object modelObject in this.ModelObjects)
            {
                sbHtml.Append("<tr><td>");
                foreach (OLVColumn col in columns) {
                    if (col != columns[0]) {
                        sbText.Append("\t");
                        sbHtml.Append("</td><td>");
                    }
                    string strValue = col.GetStringValue(modelObject);
                    sbText.Append(strValue);
                    sbHtml.Append(strValue); //TODO: Should encode the string value
                }
                sbText.AppendLine();
                sbHtml.AppendLine("</td></tr>");
            }
            sbHtml.AppendLine("</table>");

            // Put both the text and html versions onto the clipboard.
            // For some reason, SetText() with UnicodeText doesn't set the basic CF_TEXT format,
            // but using SetData() does.
            //this.SetText(sbText.ToString(), TextDataFormat.UnicodeText);
            this.SetData(sbText.ToString());
            this.SetText(ConvertToHtmlFragment(sbHtml.ToString()), TextDataFormat.Html);
        }

        /// <summary>
        /// Make a HTML representation of our model objects
        /// </summary>
        public string CreateHtml() {
            IList<OLVColumn> columns = this.ListView.ColumnsInDisplayOrder;

            // Build html version of the selection
            StringBuilder sbHtml = new StringBuilder("<table>");

            foreach (object modelObject in this.ModelObjects) {
                sbHtml.Append("<tr><td>");
                foreach (OLVColumn col in columns) {
                    if (col != columns[0]) {
                        sbHtml.Append("</td><td>");
                    }
                    string strValue = col.GetStringValue(modelObject);
                    sbHtml.Append(strValue); //TODO: Should encode the string value
                }
                sbHtml.AppendLine("</td></tr>");
            }
            sbHtml.AppendLine("</table>");

            return sbHtml.ToString();
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
            string source = "http://www.codeproject.com/KB/list/ObjectListView.aspx";

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

            int prefixLength = String.Format(MARKER_BLOCK, 0, 0, 0, 0, source, "").Length;

            const String DEFAULT_HTML_BODY =
                "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\">" +
                "<HTML><HEAD></HEAD><BODY><!--StartFragment-->{0}<!--EndFragment--></BODY></HTML>";

            string html = String.Format(DEFAULT_HTML_BODY, fragment);
            int startFragment = prefixLength + html.IndexOf(fragment);
            int endFragment = startFragment + fragment.Length;

            return String.Format(MARKER_BLOCK, prefixLength, prefixLength + html.Length, startFragment, endFragment, source, html);
        }
    }
}

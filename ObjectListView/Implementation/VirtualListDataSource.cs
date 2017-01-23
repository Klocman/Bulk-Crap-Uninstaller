/*
 * VirtualListDataSource - Encapsulate how data is provided to a virtual list
 *
 * Author: Phillip Piper
 * Date: 28/08/2009 11:10am
 *
 * Change log:
 * v2.4
 * 2010-04-01   JPP  - Added IFilterableDataSource
 * v2.3
 * 2009-08-28   JPP  - Initial version (Separated from VirtualObjectListView.cs)
 *
 * To do:
 *
 * Copyright (C) 2009-2014 Phillip Piper
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

namespace BrightIdeasSoftware
{
    /// <summary>
    /// A VirtualListDataSource is a complete manner to provide functionality to a virtual list.
    /// An object that implements this interface provides a VirtualObjectListView with all the
    /// information it needs to be fully functional.
    /// </summary>
    /// <remarks>Implementors must provide functioning implementations of at least GetObjectCount()
    /// and GetNthObject(), otherwise nothing will appear in the list.</remarks>
    public interface IVirtualListDataSource
    {
        /// <summary>
        /// Return the object that should be displayed at the n'th row.
        /// </summary>
        /// <param name="n">The index of the row whose object is to be returned.</param>
        /// <returns>The model object at the n'th row, or null if the fetching was unsuccessful.</returns>
        Object GetNthObject(int n);

        /// <summary>
        /// Return the number of rows that should be visible in the virtual list
        /// </summary>
        /// <returns>The number of rows the list view should have.</returns>
        int GetObjectCount();

        /// <summary>
        /// Get the index of the row that is showing the given model object
        /// </summary>
        /// <param name="model">The model object sought</param>
        /// <returns>The index of the row showing the model, or -1 if the object could not be found.</returns>
        int GetObjectIndex(Object model);

        /// <summary>
        /// The ListView is about to request the given range of items. Do
        /// whatever caching seems appropriate.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="last"></param>
        void PrepareCache(int first, int last);

        /// <summary>
        /// Find the first row that "matches" the given text in the given range.
        /// </summary>
        /// <param name="value">The text typed by the user</param>
        /// <param name="first">Start searching from this index. This may be greater than the 'to' parameter, 
        /// in which case the search should descend</param>
        /// <param name="last">Do not search beyond this index. This may be less than the 'from' parameter.</param>
        /// <param name="column">The column that should be considered when looking for a match.</param>
        /// <returns>Return the index of row that was matched, or -1 if no match was found</returns>
        int SearchText(string value, int first, int last, OLVColumn column);

        /// <summary>
        /// Sort the model objects in the data source.
        /// </summary>
        /// <param name="column"></param>
        /// <param name="order"></param>
        void Sort(OLVColumn column, SortOrder order);

        //-----------------------------------------------------------------------------------
        // Modification commands
        // THINK: Should we split these four into a separate interface?

        /// <summary>
        /// Add the given collection of model objects to this control.
        /// </summary>
        /// <param name="modelObjects">A collection of model objects</param>
        void AddObjects(ICollection modelObjects);

        /// <summary>
        /// Insert the given collection of model objects to this control at the position
        /// </summary>
        /// <param name="index">Index where the collection will be added</param>
        /// <param name="modelObjects">A collection of model objects</param>
        void InsertObjects(int index, ICollection modelObjects);

        /// <summary>
        /// Remove all of the given objects from the control
        /// </summary>
        /// <param name="modelObjects">Collection of objects to be removed</param>
        void RemoveObjects(ICollection modelObjects);

        /// <summary>
        /// Set the collection of objects that this control will show.
        /// </summary>
        /// <param name="collection"></param>
        void SetObjects(IEnumerable collection);

        /// <summary>
        /// Update/replace the nth object with the given object
        /// </summary>
        /// <param name="index"></param>
        /// <param name="modelObject"></param>
        void UpdateObject(int index, object modelObject);
    }

    /// <summary>
    /// This extension allow virtual lists to filter their contents
    /// </summary>
    public interface IFilterableDataSource
    {
        /// <summary>
        /// All subsequent retrievals on this data source should be filtered
        /// through the given filters. null means no filtering of that kind.
        /// </summary>
        /// <param name="modelFilter"></param>
        /// <param name="listFilter"></param>
        void ApplyFilters(IModelFilter modelFilter, IListFilter listFilter);
    }

    /// <summary>
    /// A do-nothing implementation of the VirtualListDataSource interface.
    /// </summary>
    public class AbstractVirtualListDataSource : IVirtualListDataSource, IFilterableDataSource
    {
        /// <summary>
        /// Creates an AbstractVirtualListDataSource
        /// </summary>
        /// <param name="listView"></param>
        public AbstractVirtualListDataSource(VirtualObjectListView listView) {
            this.listView = listView;
        }

        /// <summary>
        /// The list view that this data source is giving information to.
        /// </summary>
        protected VirtualObjectListView listView;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public virtual object GetNthObject(int n) {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual int GetObjectCount() {
            return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual int GetObjectIndex(object model) {
            return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public virtual void PrepareCache(int from, int to) {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="first"></param>
        /// <param name="last"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public virtual int SearchText(string value, int first, int last, OLVColumn column) {
            return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="column"></param>
        /// <param name="order"></param>
        public virtual void Sort(OLVColumn column, SortOrder order) {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelObjects"></param>
        public virtual void AddObjects(ICollection modelObjects) {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="modelObjects"></param>
        public virtual void InsertObjects(int index, ICollection modelObjects) {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelObjects"></param>
        public virtual void RemoveObjects(ICollection modelObjects) {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection"></param>
        public virtual void SetObjects(IEnumerable collection) {
        }

        /// <summary>
        /// Update/replace the nth object with the given object
        /// </summary>
        /// <param name="index"></param>
        /// <param name="modelObject"></param>
        public virtual void UpdateObject(int index, object modelObject) {
        }

        /// <summary>
        /// This is a useful default implementation of SearchText method, intended to be called
        /// by implementors of IVirtualListDataSource.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="first"></param>
        /// <param name="last"></param>
        /// <param name="column"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        static public int DefaultSearchText(string value, int first, int last, OLVColumn column, IVirtualListDataSource source) {
            if (first <= last) {
                for (int i = first; i <= last; i++) {
                    string data = column.GetStringValue(source.GetNthObject(i));
                    if (data.StartsWith(value, StringComparison.CurrentCultureIgnoreCase))
                        return i;
                }
            } else {
                for (int i = first; i >= last; i--) {
                    string data = column.GetStringValue(source.GetNthObject(i));
                    if (data.StartsWith(value, StringComparison.CurrentCultureIgnoreCase))
                        return i;
                }
            }

            return -1;
        }

        #region IFilterableDataSource Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelFilter"></param>
        /// <param name="listFilter"></param>
        virtual public void ApplyFilters(IModelFilter modelFilter, IListFilter listFilter) {
        }

        #endregion
    }

    /// <summary>
    /// This class mimics the behavior of VirtualObjectListView v1.x.
    /// </summary>
    public class VirtualListVersion1DataSource : AbstractVirtualListDataSource
    {
        /// <summary>
        /// Creates a VirtualListVersion1DataSource
        /// </summary>
        /// <param name="listView"></param>
        public VirtualListVersion1DataSource(VirtualObjectListView listView)
            : base(listView) {
        }

        #region Public properties

        /// <summary>
        /// How will the n'th object of the data source be fetched?
        /// </summary>
        public RowGetterDelegate RowGetter {
            get { return rowGetter; }
            set { rowGetter = value; }
        }
        private RowGetterDelegate rowGetter;

        #endregion

        #region IVirtualListDataSource implementation

        /// <summary>
        /// 
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public override object GetNthObject(int n) {
            if (this.RowGetter == null)
                return null;
            else
                return this.RowGetter(n);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="first"></param>
        /// <param name="last"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public override int SearchText(string value, int first, int last, OLVColumn column) {
            return DefaultSearchText(value, first, last, column, this);
        }

        #endregion
    }
}

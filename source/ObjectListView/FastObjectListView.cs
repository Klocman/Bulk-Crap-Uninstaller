/*
 * FastObjectListView - A listview that behaves like an ObjectListView but has the speed of a virtual list
 *
 * Author: Phillip Piper
 * Date: 27/09/2008 9:15 AM
 *
 * Change log:
 * 2014-10-15   JPP  - Fire Filter event when applying filters
 * v2.8
 * 2012-06-11   JPP  - Added more efficient version of FilteredObjects
 * v2.5.1
 * 2011-04-25   JPP  - Fixed problem with removing objects from filtered or sorted list
 * v2.4
 * 2010-04-05   JPP  - Added filtering
 * v2.3
 * 2009-08-27   JPP  - Added GroupingStrategy
 *                   - Added optimized Objects property
 * v2.2.1
 * 2009-01-07   JPP  - Made all public and protected methods virtual
 * 2008-09-27   JPP  - Separated from ObjectListView.cs
 *
 * Copyright (C) 2006-2014 Phillip Piper
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
using System.ComponentModel;
using System.Windows.Forms;

namespace BrightIdeasSoftware
{
    /// <summary>
    /// A FastObjectListView trades function for speed.
    /// </summary>
    /// <remarks>
    /// <para>On my mid-range laptop, this view builds a list of 10,000 objects in 0.1 seconds,
    /// as opposed to a normal ObjectListView which takes 10-15 seconds. Lists of up to 50,000 items should be
    /// able to be handled with sub-second response times even on low end machines.</para>
    /// <para>
    /// A FastObjectListView is implemented as a virtual list with many of the virtual modes limits (e.g. no sorting)
    /// fixed through coding. There are some functions that simply cannot be provided. Specifically, a FastObjectListView cannot:
    /// <list type="bullet">
    /// <item><description>use Tile view</description></item>
    /// <item><description>show groups on XP</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    public class FastObjectListView : VirtualObjectListView
    {
        /// <summary>
        /// Make a FastObjectListView
        /// </summary>
        public FastObjectListView() {
            this.VirtualListDataSource = new FastObjectListDataSource(this);
            this.GroupingStrategy = new FastListGroupingStrategy();
        }

        /// <summary>
        /// Gets the collection of objects that survive any filtering that may be in place.
        /// </summary>
        [Browsable(false),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override IEnumerable FilteredObjects {
            get {
                // This is much faster than the base method
                return ((FastObjectListDataSource)this.VirtualListDataSource).FilteredObjectList;
            }
        }

        /// <summary>
        /// Get/set the collection of objects that this list will show
        /// </summary>
        /// <remarks>
        /// <para>
        /// The contents of the control will be updated immediately after setting this property.
        /// </para>
        /// <para>This method preserves selection, if possible. Use SetObjects() if
        /// you do not want to preserve the selection. Preserving selection is the slowest part of this
        /// code and performance is O(n) where n is the number of selected rows.</para>
        /// <para>This method is not thread safe.</para>
        /// </remarks>
        [Browsable(false),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override IEnumerable Objects {
            get {
                // This is much faster than the base method
                return ((FastObjectListDataSource)this.VirtualListDataSource).ObjectList;
            }
            set { base.Objects = value; }
        }

        /// <summary>
        /// Move the given collection of objects to the given index.
        /// </summary>
        /// <remarks>This operation only makes sense on non-grouped ObjectListViews.</remarks>
        /// <param name="index"></param>
        /// <param name="modelObjects"></param>
        public override void MoveObjects(int index, ICollection modelObjects) {
            if (this.InvokeRequired) {
                this.Invoke((MethodInvoker)delegate() { this.MoveObjects(index, modelObjects); });
                return;
            }

            // If any object that is going to be moved is before the point where the insertion 
            // will occur, then we have to reduce the location of our insertion point
            int displacedObjectCount = 0;
            foreach (object modelObject in modelObjects) {
                int i = this.IndexOf(modelObject);
                if (i >= 0 && i <= index)
                    displacedObjectCount++;
            }
            index -= displacedObjectCount;

            this.BeginUpdate();
            try {
                this.RemoveObjects(modelObjects);
                this.InsertObjects(index, modelObjects);
            }
            finally {
                this.EndUpdate();
            }
        }

        /// <summary>
        /// Remove any sorting and revert to the given order of the model objects
        /// </summary>
        /// <remarks>To be really honest, Unsort() doesn't work on FastObjectListViews since
        /// the original ordering of model objects is lost when Sort() is called. So this method
        /// effectively just turns off sorting.</remarks>
        public override void Unsort() {
            this.ShowGroups = false;
            this.PrimarySortColumn = null;
            this.PrimarySortOrder = SortOrder.None;
            this.SetObjects(this.Objects);
        }
    }

    /// <summary>
    /// Provide a data source for a FastObjectListView
    /// </summary>
    /// <remarks>
    /// This class isn't intended to be used directly, but it is left as a public
    /// class just in case someone wants to subclass it.
    /// </remarks>
    public class FastObjectListDataSource : AbstractVirtualListDataSource
    {
        /// <summary>
        /// Create a FastObjectListDataSource
        /// </summary>
        /// <param name="listView"></param>
        public FastObjectListDataSource(FastObjectListView listView)
            : base(listView) {
        }

        #region IVirtualListDataSource Members

        /// <summary>
        /// Get n'th object
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public override object GetNthObject(int n) {
            if (n >= 0 && n < this.filteredObjectList.Count)
                return this.filteredObjectList[n];
            
            return null;
        }

        /// <summary>
        /// How many items are in the data source
        /// </summary>
        /// <returns></returns>
        public override int GetObjectCount() {
            return this.filteredObjectList.Count;
        }

        /// <summary>
        /// Get the index of the given model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override int GetObjectIndex(object model) {
            int index;

            if (model != null && this.objectsToIndexMap.TryGetValue(model, out index))
                return index;
            
            return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="first"></param>
        /// <param name="last"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public override int SearchText(string text, int first, int last, OLVColumn column) {
            if (first <= last) {
                for (int i = first; i <= last; i++) {
                    string data = column.GetStringValue(this.listView.GetNthItemInDisplayOrder(i).RowObject);
                    if (data.StartsWith(text, StringComparison.CurrentCultureIgnoreCase))
                        return i;
                }
            } else {
                for (int i = first; i >= last; i--) {
                    string data = column.GetStringValue(this.listView.GetNthItemInDisplayOrder(i).RowObject);
                    if (data.StartsWith(text, StringComparison.CurrentCultureIgnoreCase))
                        return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="column"></param>
        /// <param name="sortOrder"></param>
        public override void Sort(OLVColumn column, SortOrder sortOrder) {
            if (sortOrder != SortOrder.None) {
                ModelObjectComparer comparer = new ModelObjectComparer(column, sortOrder, this.listView.SecondarySortColumn, this.listView.SecondarySortOrder);
                this.fullObjectList.Sort(comparer);
                this.filteredObjectList.Sort(comparer);
            }
            this.RebuildIndexMap();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelObjects"></param>
        public override void AddObjects(ICollection modelObjects) {
            foreach (object modelObject in modelObjects) {
                if (modelObject != null)
                    this.fullObjectList.Add(modelObject);
            }
            this.FilterObjects();
            this.RebuildIndexMap();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="modelObjects"></param>
        public override void InsertObjects(int index, ICollection modelObjects) {
            this.fullObjectList.InsertRange(index, modelObjects);
            this.FilterObjects();
            this.RebuildIndexMap();
        }

        /// <summary>
        /// Remove the given collection of models from this source.
        /// </summary>
        /// <param name="modelObjects"></param>
        public override void RemoveObjects(ICollection modelObjects) {

            // We have to unselect any object that is about to be deleted
            List<int> indicesToRemove = new List<int>();
            foreach (object modelObject in modelObjects) {
                int i = this.GetObjectIndex(modelObject);
                if (i >= 0)
                    indicesToRemove.Add(i);
            }

            // Sort the indices from highest to lowest so that we
            // remove latter ones before earlier ones. In this way, the
            // indices of the rows doesn't change after the deletes.
            indicesToRemove.Sort();
            indicesToRemove.Reverse();

            foreach (int i in indicesToRemove) 
                this.listView.SelectedIndices.Remove(i);

            // Remove the objects from the unfiltered list
            foreach (object modelObject in modelObjects)
                this.fullObjectList.Remove(modelObject);

            this.FilterObjects();
            this.RebuildIndexMap();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection"></param>
        public override void SetObjects(IEnumerable collection) {
            ArrayList newObjects = ObjectListView.EnumerableToArray(collection, true);

            this.fullObjectList = newObjects;
            this.FilterObjects();
            this.RebuildIndexMap();
        }

        /// <summary>
        /// Update/replace the nth object with the given object
        /// </summary>
        /// <param name="index"></param>
        /// <param name="modelObject"></param>
        public override void UpdateObject(int index, object modelObject) {
            if (index < 0 || index >= this.filteredObjectList.Count)
                return;

            int i = this.fullObjectList.IndexOf(this.filteredObjectList[index]);
            if (i < 0)
                return;

            if (ReferenceEquals(this.fullObjectList[i], modelObject))
                return;

            this.fullObjectList[i] = modelObject;
            this.filteredObjectList[index] = modelObject;
            this.objectsToIndexMap[modelObject] = index;
        }

        private ArrayList fullObjectList = new ArrayList();
        private ArrayList filteredObjectList = new ArrayList();
        private IModelFilter modelFilter;
        private IListFilter listFilter;

        #endregion

        #region IFilterableDataSource Members

        /// <summary>
        /// Apply the given filters to this data source. One or both may be null.
        /// </summary>
        /// <param name="iModelFilter"></param>
        /// <param name="iListFilter"></param>
        public override void ApplyFilters(IModelFilter iModelFilter, IListFilter iListFilter) {
            this.modelFilter = iModelFilter;
            this.listFilter = iListFilter;
            this.SetObjects(this.fullObjectList);
        }

        #endregion

        #region Implementation

        /// <summary>
        /// Gets the full list of objects being used for this fast list. 
        /// This list is unfiltered.
        /// </summary>
        public ArrayList ObjectList {
            get { return fullObjectList; }
        }

        /// <summary>
        /// Gets the list of objects from ObjectList which survive any installed filters.
        /// </summary>
        public ArrayList FilteredObjectList {
            get { return filteredObjectList; }
        }

        /// <summary>
        /// Rebuild the map that remembers which model object is displayed at which line
        /// </summary>
        protected void RebuildIndexMap() {
            this.objectsToIndexMap.Clear();
            for (int i = 0; i < this.filteredObjectList.Count; i++)
                this.objectsToIndexMap[this.filteredObjectList[i]] = i;
        }
        readonly Dictionary<Object, int> objectsToIndexMap = new Dictionary<Object, int>();

        /// <summary>
        /// Build our filtered list from our full list.
        /// </summary>
        protected void FilterObjects() {

            // If this list isn't filtered, we don't need to do anything else
            if (!this.listView.UseFiltering) {
                this.filteredObjectList = new ArrayList(this.fullObjectList);
                return;
            }

            // Tell the world to filter the objects. If they do so, don't do anything else
            // ReSharper disable PossibleMultipleEnumeration
            FilterEventArgs args = new FilterEventArgs(this.fullObjectList);
            this.listView.OnFilter(args);
            if (args.FilteredObjects != null) {
                this.filteredObjectList = ObjectListView.EnumerableToArray(args.FilteredObjects, false);
                return;
            }

            IEnumerable objects = (this.listFilter == null) ?
                this.fullObjectList : this.listFilter.Filter(this.fullObjectList);

            // Apply the object filter if there is one
            if (this.modelFilter == null) {
                this.filteredObjectList = ObjectListView.EnumerableToArray(objects, false);
            } else {
                this.filteredObjectList = new ArrayList();
                foreach (object model in objects) {
                    if (this.modelFilter.Filter(model))
                        this.filteredObjectList.Add(model);
                }
            }
        }

        #endregion
    }

}

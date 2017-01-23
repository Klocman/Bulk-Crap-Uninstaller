/*
 * Virtual groups - Classes and interfaces needed to implement virtual groups
 *
 * Author: Phillip Piper
 * Date: 28/08/2009 11:10am
 *
 * Change log:
 * 2011-02-21   JPP  - Correctly honor group comparer and collapsible groups settings
 * v2.3
 * 2009-08-28   JPP  - Initial version
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
using System.Collections.Generic;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace BrightIdeasSoftware
{
    /// <summary>
    /// A IVirtualGroups is the interface that a virtual list must implement to support virtual groups
    /// </summary>
    public interface IVirtualGroups
    {
        /// <summary>
        /// Return the list of groups that should be shown according to the given parameters
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IList<OLVGroup> GetGroups(GroupingParameters parameters);

        /// <summary>
        /// Return the index of the item that appears at the given position within the given group.
        /// </summary>
        /// <param name="group"></param>
        /// <param name="indexWithinGroup"></param>
        /// <returns></returns>
        int GetGroupMember(OLVGroup group, int indexWithinGroup);

        /// <summary>
        /// Return the index of the group to which the given item belongs
        /// </summary>
        /// <param name="itemIndex"></param>
        /// <returns></returns>
        int GetGroup(int itemIndex);

        /// <summary>
        /// Return the index at which the given item is shown in the given group
        /// </summary>
        /// <param name="group"></param>
        /// <param name="itemIndex"></param>
        /// <returns></returns>
        int GetIndexWithinGroup(OLVGroup group, int itemIndex);

        /// <summary>
        /// A hint that the given range of items are going to be required
        /// </summary>
        /// <param name="fromGroupIndex"></param>
        /// <param name="fromIndex"></param>
        /// <param name="toGroupIndex"></param>
        /// <param name="toIndex"></param>
        void CacheHint(int fromGroupIndex, int fromIndex, int toGroupIndex, int toIndex);
    }

    /// <summary>
    /// This is a safe, do nothing implementation of a grouping strategy
    /// </summary>
    public class AbstractVirtualGroups : IVirtualGroups
    {
        /// <summary>
        /// Return the list of groups that should be shown according to the given parameters
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual IList<OLVGroup> GetGroups(GroupingParameters parameters) {
            return new List<OLVGroup>();
        }

        /// <summary>
        /// Return the index of the item that appears at the given position within the given group.
        /// </summary>
        /// <param name="group"></param>
        /// <param name="indexWithinGroup"></param>
        /// <returns></returns>
        public virtual int GetGroupMember(OLVGroup group, int indexWithinGroup) {
            return -1;
        }

        /// <summary>
        /// Return the index of the group to which the given item belongs
        /// </summary>
        /// <param name="itemIndex"></param>
        /// <returns></returns>
        public virtual int GetGroup(int itemIndex) {
            return -1;
        }

        /// <summary>
        /// Return the index at which the given item is shown in the given group
        /// </summary>
        /// <param name="group"></param>
        /// <param name="itemIndex"></param>
        /// <returns></returns>
        public virtual int GetIndexWithinGroup(OLVGroup group, int itemIndex) {
            return -1;
        }

        /// <summary>
        /// A hint that the given range of items are going to be required
        /// </summary>
        /// <param name="fromGroupIndex"></param>
        /// <param name="fromIndex"></param>
        /// <param name="toGroupIndex"></param>
        /// <param name="toIndex"></param>
        public virtual void CacheHint(int fromGroupIndex, int fromIndex, int toGroupIndex, int toIndex) {
        }
    }


    /// <summary>
    /// Provides grouping functionality to a FastObjectListView
    /// </summary>
    public class FastListGroupingStrategy : AbstractVirtualGroups
    {
        /// <summary>
        /// Create groups for FastListView
        /// </summary>
        /// <param name="parmameters"></param>
        /// <returns></returns>
        public override IList<OLVGroup> GetGroups(GroupingParameters parmameters) {

            // There is a lot of overlap between this method and ObjectListView.MakeGroups()
            // Any changes made here may need to be reflected there

            // This strategy can only be used on FastObjectListViews
            FastObjectListView folv = (FastObjectListView)parmameters.ListView;

            // Separate the list view items into groups, using the group key as the descrimanent
            int objectCount = 0;
            NullableDictionary<object, List<object>> map = new NullableDictionary<object, List<object>>();
            foreach (object model in folv.FilteredObjects) {
                object key = parmameters.GroupByColumn.GetGroupKey(model);
                if (!map.ContainsKey(key))
                    map[key] = new List<object>();
                map[key].Add(model);
                objectCount++;
            }

            // Sort the items within each group
            // TODO: Give parameters a ModelComparer property
            OLVColumn primarySortColumn = parmameters.SortItemsByPrimaryColumn ? parmameters.ListView.GetColumn(0) : parmameters.PrimarySort;
            ModelObjectComparer sorter = new ModelObjectComparer(primarySortColumn, parmameters.PrimarySortOrder,
                parmameters.SecondarySort, parmameters.SecondarySortOrder);
            foreach (object key in map.Keys) {
                map[key].Sort(sorter);
            }

            // Make a list of the required groups
            List<OLVGroup> groups = new List<OLVGroup>();
            foreach (object key in map.Keys) {
                string title = parmameters.GroupByColumn.ConvertGroupKeyToTitle(key);
                if (!String.IsNullOrEmpty(parmameters.TitleFormat)) {
                    int count = map[key].Count;
                    string format = (count == 1 ? parmameters.TitleSingularFormat : parmameters.TitleFormat);
                    try {
                        title = String.Format(format, title, count);
                    } catch (FormatException) {
                        title = "Invalid group format: " + format;
                    }
                }
                OLVGroup lvg = new OLVGroup(title);
                lvg.Collapsible = folv.HasCollapsibleGroups;
                lvg.Key = key;
                lvg.SortValue = key as IComparable;
                lvg.Contents = map[key].ConvertAll<int>(delegate(object x) { return folv.IndexOf(x); });
                lvg.VirtualItemCount = map[key].Count;
                if (parmameters.GroupByColumn.GroupFormatter != null)
                    parmameters.GroupByColumn.GroupFormatter(lvg, parmameters);
                groups.Add(lvg);
            }

            // Sort the groups
            if (parmameters.GroupByOrder != SortOrder.None)
                groups.Sort(parmameters.GroupComparer ?? new OLVGroupComparer(parmameters.GroupByOrder));

            // Build an array that remembers which group each item belongs to.
            this.indexToGroupMap = new List<int>(objectCount);
            this.indexToGroupMap.AddRange(new int[objectCount]);

            for (int i = 0; i < groups.Count; i++) {
                OLVGroup group = groups[i];
                List<int> members = (List<int>)group.Contents;
                foreach (int j in members)
                    this.indexToGroupMap[j] = i;
            }

            return groups;
        }
        private List<int> indexToGroupMap;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="group"></param>
        /// <param name="indexWithinGroup"></param>
        /// <returns></returns>
        public override int GetGroupMember(OLVGroup group, int indexWithinGroup) {
            return (int)group.Contents[indexWithinGroup];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemIndex"></param>
        /// <returns></returns>
        public override int GetGroup(int itemIndex) {
            return this.indexToGroupMap[itemIndex];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="group"></param>
        /// <param name="itemIndex"></param>
        /// <returns></returns>
        public override int GetIndexWithinGroup(OLVGroup group, int itemIndex) {
            return group.Contents.IndexOf(itemIndex);
        }
    }


    /// <summary>
    /// This is the COM interface that a ListView must be given in order for groups in virtual lists to work.
    /// </summary>
    /// <remarks>
    /// This interface is NOT documented by MS. It was found on Greg Chapell's site. This means that there is
    /// no guarantee that it will work on future versions of Windows, nor continue to work on current ones.
    /// </remarks>
    [ComImport(),
     InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
     Guid("44C09D56-8D3B-419D-A462-7B956B105B47")]
    internal interface IOwnerDataCallback
    {
        /// <summary>
        /// Not sure what this does
        /// </summary>
        /// <param name="i"></param>
        /// <param name="pt"></param>
        void GetItemPosition(int i, out NativeMethods.POINT pt);

        /// <summary>
        /// Not sure what this does
        /// </summary>
        /// <param name="t"></param>
        /// <param name="pt"></param>
        void SetItemPosition(int t, NativeMethods.POINT pt);

        /// <summary>
        /// Get the index of the item that occurs at the n'th position of the indicated group.
        /// </summary>
        /// <param name="groupIndex">Index of the group</param>
        /// <param name="n">Index within the group</param>
        /// <param name="itemIndex">Index of the item within the whole list</param>
        void GetItemInGroup(int groupIndex, int n, out int itemIndex);

        /// <summary>
        /// Get the index of the group to which the given item belongs
        /// </summary>
        /// <param name="itemIndex">Index of the item within the whole list</param>
        /// <param name="occurrenceCount">Which occurences of the item is wanted</param>
        /// <param name="groupIndex">Index of the group</param>
        void GetItemGroup(int itemIndex, int occurrenceCount, out int groupIndex);

        /// <summary>
        /// Get the number of groups that contain the given item
        /// </summary>
        /// <param name="itemIndex">Index of the item within the whole list</param>
        /// <param name="occurrenceCount">How many groups does it occur within</param>
        void GetItemGroupCount(int itemIndex, out int occurrenceCount);

        /// <summary>
        /// A hint to prepare any cache for the given range of requests
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        void OnCacheHint(NativeMethods.LVITEMINDEX i, NativeMethods.LVITEMINDEX j);
    }

    /// <summary>
    /// A default implementation of the IOwnerDataCallback interface
    /// </summary>
    [Guid("6FC61F50-80E8-49b4-B200-3F38D3865ABD")]
    internal class OwnerDataCallbackImpl : IOwnerDataCallback
    {
        public OwnerDataCallbackImpl(VirtualObjectListView olv) {
            this.olv = olv;
        }
        VirtualObjectListView olv;

        #region IOwnerDataCallback Members

        public void GetItemPosition(int i, out NativeMethods.POINT pt) {
            //System.Diagnostics.Debug.WriteLine("GetItemPosition");
            throw new NotSupportedException();
        }

        public void SetItemPosition(int t, NativeMethods.POINT pt) {
            //System.Diagnostics.Debug.WriteLine("SetItemPosition");
            throw new NotSupportedException();
        }

        public void GetItemInGroup(int groupIndex, int n, out int itemIndex) {
            //System.Diagnostics.Debug.WriteLine(String.Format("-> GetItemInGroup({0}, {1})", groupIndex, n));
            itemIndex = this.olv.GroupingStrategy.GetGroupMember(this.olv.OLVGroups[groupIndex], n);
            //System.Diagnostics.Debug.WriteLine(String.Format("<- {0}", itemIndex));
        }

        public void GetItemGroup(int itemIndex, int occurrenceCount, out int groupIndex) {
            //System.Diagnostics.Debug.WriteLine(String.Format("GetItemGroup({0}, {1})", itemIndex, occurrenceCount));
            groupIndex = this.olv.GroupingStrategy.GetGroup(itemIndex);
            //System.Diagnostics.Debug.WriteLine(String.Format("<- {0}", groupIndex));
        }

        public void GetItemGroupCount(int itemIndex, out int occurrenceCount) {
            //System.Diagnostics.Debug.WriteLine(String.Format("GetItemGroupCount({0})", itemIndex));
            occurrenceCount = 1;
        }

        public void OnCacheHint(NativeMethods.LVITEMINDEX from, NativeMethods.LVITEMINDEX to) {
            //System.Diagnostics.Debug.WriteLine(String.Format("OnCacheHint({0}, {1}, {2}, {3})", from.iGroup, from.iItem, to.iGroup, to.iItem));
            this.olv.GroupingStrategy.CacheHint(from.iGroup, from.iItem, to.iGroup, to.iItem);
        }

        #endregion
    }
}

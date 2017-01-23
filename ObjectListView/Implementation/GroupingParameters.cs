/*
 * GroupingParameters - All the data that is used to create groups in an ObjectListView
 *
 * Author: Phillip Piper
 * Date: 31-March-2011 5:53 pm
 *
 * Change log:
 * 2011-03-31  JPP  - Split into its own file
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
using System.Windows.Forms;

namespace BrightIdeasSoftware {

    /// <summary>
    /// This class contains all the settings used when groups are created
    /// </summary>
    public class GroupingParameters {
        /// <summary>
        /// Create a GroupingParameters
        /// </summary>
        /// <param name="olv"></param>
        /// <param name="groupByColumn"></param>
        /// <param name="groupByOrder"></param>
        /// <param name="column"></param>
        /// <param name="order"></param>
        /// <param name="secondaryColumn"></param>
        /// <param name="secondaryOrder"></param>
        /// <param name="titleFormat"></param>
        /// <param name="titleSingularFormat"></param>
        /// <param name="sortItemsByPrimaryColumn"></param>
        public GroupingParameters(ObjectListView olv, OLVColumn groupByColumn, SortOrder groupByOrder,
            OLVColumn column, SortOrder order, OLVColumn secondaryColumn, SortOrder secondaryOrder,
            string titleFormat, string titleSingularFormat, bool sortItemsByPrimaryColumn) {
            this.ListView = olv;
            this.GroupByColumn = groupByColumn;
            this.GroupByOrder = groupByOrder;
            this.PrimarySort = column;
            this.PrimarySortOrder = order;
            this.SecondarySort = secondaryColumn;
            this.SecondarySortOrder = secondaryOrder;
            this.SortItemsByPrimaryColumn = sortItemsByPrimaryColumn;
            this.TitleFormat = titleFormat;
            this.TitleSingularFormat = titleSingularFormat;
        }

        /// <summary>
        /// Gets or sets the ObjectListView being grouped
        /// </summary>
        public ObjectListView ListView {
            get { return this.listView; }
            set { this.listView = value; }
        }
        private ObjectListView listView;

        /// <summary>
        /// Gets or sets the column used to create groups
        /// </summary>
        public OLVColumn GroupByColumn {
            get { return this.groupByColumn; }
            set { this.groupByColumn = value; }
        }
        private OLVColumn groupByColumn;

        /// <summary>
        /// In what order will the groups themselves be sorted?
        /// </summary>
        public SortOrder GroupByOrder {
            get { return this.groupByOrder; }
            set { this.groupByOrder = value; }
        }
        private SortOrder groupByOrder;

        /// <summary>
        /// If this is set, this comparer will be used to order the groups
        /// </summary>
        public IComparer<OLVGroup> GroupComparer {
            get { return this.groupComparer; }
            set { this.groupComparer = value; }
        }
        private IComparer<OLVGroup> groupComparer;

        /// <summary>
        /// If this is set, this comparer will be used to order items within each group
        /// </summary>
        public IComparer<OLVListItem> ItemComparer {
            get { return this.itemComparer; }
            set { this.itemComparer = value; }
        }
        private IComparer<OLVListItem> itemComparer;

        /// <summary>
        /// Gets or sets the column that will be the primary sort
        /// </summary>
        public OLVColumn PrimarySort {
            get { return this.primarySort; }
            set { this.primarySort = value; }
        }
        private OLVColumn primarySort;

        /// <summary>
        /// Gets or sets the ordering for the primary sort
        /// </summary>
        public SortOrder PrimarySortOrder {
            get { return this.primarySortOrder; }
            set { this.primarySortOrder = value; }
        }
        private SortOrder primarySortOrder;

        /// <summary>
        /// Gets or sets the column used for secondary sorting
        /// </summary>
        public OLVColumn SecondarySort {
            get { return this.secondarySort; }
            set { this.secondarySort = value; }
        }
        private OLVColumn secondarySort;

        /// <summary>
        /// Gets or sets the ordering for the secondary sort
        /// </summary>
        public SortOrder SecondarySortOrder {
            get { return this.secondarySortOrder; }
            set { this.secondarySortOrder = value; }
        }
        private SortOrder secondarySortOrder;

        /// <summary>
        /// Gets or sets the title format used for groups with zero or more than one element
        /// </summary>
        public string TitleFormat {
            get { return this.titleFormat; }
            set { this.titleFormat = value; }
        }
        private string titleFormat;

        /// <summary>
        /// Gets or sets the title format used for groups with only one element
        /// </summary>
        public string TitleSingularFormat {
            get { return this.titleSingularFormat; }
            set { this.titleSingularFormat = value; }
        }
        private string titleSingularFormat;

        /// <summary>
        /// Gets or sets whether the items should be sorted by the primary column
        /// </summary>
        public bool SortItemsByPrimaryColumn {
            get { return this.sortItemsByPrimaryColumn; }
            set { this.sortItemsByPrimaryColumn = value; }
        }
        private bool sortItemsByPrimaryColumn;
    }
}

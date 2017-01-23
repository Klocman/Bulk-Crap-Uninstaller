/*
 * ICluster - A cluster is a group of objects that can be included or excluded as a whole
 *
 * Author: Phillip Piper
 * Date: 4-March-2011 11:59 pm
 *
 * Change log:
 * 2011-03-04  JPP  - First version
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

namespace BrightIdeasSoftware {

    /// <summary>
    /// A cluster is a like collection of objects that can be usefully filtered
    /// as whole using the filtering UI provided by the ObjectListView.
    /// </summary>
    public interface ICluster : IComparable {
        /// <summary>
        /// Gets or sets how many items belong to this cluster
        /// </summary>
        int Count { get; set; }

        /// <summary>
        /// Gets or sets the label that will be shown to the user to represent
        /// this cluster
        /// </summary>
        string DisplayLabel { get; set; }

        /// <summary>
        /// Gets or sets the actual data object that all members of this cluster
        /// have commonly returned.
        /// </summary>
        object ClusterKey { get; set; }
    }
}

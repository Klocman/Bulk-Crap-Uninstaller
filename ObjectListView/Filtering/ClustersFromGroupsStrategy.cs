/*
 * ClusteringStrategy - Implements a simple clustering strategy
 *
 * Author: Phillip Piper
 * Date: 1-April-2011 8:12am
 *
 * Change log:
 * 2011-04-01  JPP  - First version
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
    /// This class calculates clusters from the groups that the column uses.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is the default strategy for all non-date, filterable columns.
    /// </para>
    /// <para>
    /// This class does not strictly mimic the groups created by the given column.
    /// In particular, if the programmer changes the default grouping technique
    /// by listening for grouping events, this class will not mimic that behaviour.
    /// </para>
    /// </remarks>
    public class ClustersFromGroupsStrategy : ClusteringStrategy {

        /// <summary>
        /// Get the cluster key by which the given model will be partitioned by this strategy
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override object GetClusterKey(object model) {
            return this.Column.GetGroupKey(model);
        }

        /// <summary>
        /// Gets the display label that the given cluster should use
        /// </summary>
        /// <param name="cluster"></param>
        /// <returns></returns>
        public override string GetClusterDisplayLabel(ICluster cluster) {
            string s = this.Column.ConvertGroupKeyToTitle(cluster.ClusterKey);
            if (String.IsNullOrEmpty(s)) 
                s = EMPTY_LABEL;
            return this.ApplyDisplayFormat(cluster, s);
        }
    }
}

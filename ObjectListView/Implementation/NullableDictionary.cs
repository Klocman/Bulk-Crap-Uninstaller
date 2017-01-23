/*
 * NullableDictionary - A simple Dictionary that can handle null as a key
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
using System.Collections;

namespace BrightIdeasSoftware {

    /// <summary>
    /// A simple-minded implementation of a Dictionary that can handle null as a key.
    /// </summary>
    /// <typeparam name="TKey">The type of the dictionary key</typeparam>
    /// <typeparam name="TValue">The type of the values to be stored</typeparam>
    /// <remarks>This is not a full implementation and is only meant to handle
    /// collecting groups by their keys, since groups can have null as a key value.</remarks>
    internal class NullableDictionary<TKey, TValue> : Dictionary<TKey, TValue> {
        private bool hasNullKey;
        private TValue nullValue;

        new public TValue this[TKey key] {
            get {
                if (key != null) 
                    return base[key];

                if (this.hasNullKey)
                    return this.nullValue;

                throw new KeyNotFoundException();
            }
            set {
                if (key == null) {
                    this.hasNullKey = true;
                    this.nullValue = value;
                } else
                    base[key] = value;
            }
        }

        new public bool ContainsKey(TKey key) {
            return key == null ? this.hasNullKey : base.ContainsKey(key);
        }

        new public IList Keys {
            get {
                ArrayList list = new ArrayList(base.Keys);
                if (this.hasNullKey)
                    list.Add(null);
                return list;
            }
        }

        new public IList<TValue> Values {
            get {
                List<TValue> list = new List<TValue>(base.Values);
                if (this.hasNullKey)
                    list.Add(this.nullValue);
                return list;
            }
        }
    }
}

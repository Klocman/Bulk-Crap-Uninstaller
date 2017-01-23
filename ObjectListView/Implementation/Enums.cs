/*
 * Enums - All enum definitions used in ObjectListView
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

namespace BrightIdeasSoftware {

    public partial class ObjectListView {
        /// <summary>
        /// How does a user indicate that they want to edit cells?
        /// </summary>
        public enum CellEditActivateMode {
            /// <summary>
            /// This list cannot be edited. F2 does nothing.
            /// </summary>
            None = 0,

            /// <summary>
            /// A single click on  a <strong>subitem</strong> will edit the value. Single clicking the primary column,
            /// selects the row just like normal. The user must press F2 to edit the primary column.
            /// </summary>
            SingleClick = 1,

            /// <summary>
            /// Double clicking a subitem or the primary column will edit that cell.
            /// F2 will edit the primary column.
            /// </summary>
            DoubleClick = 2,

            /// <summary>
            /// Pressing F2 is the only way to edit the cells. Once the primary column is being edited,
            /// the other cells in the row can be edited by pressing Tab.
            /// </summary>
            F2Only = 3,

            /// <summary>
            /// A single click on  a <strong>any</strong> cell will edit the value, even the primary column.
            /// </summary>
            SingleClickAlways = 4,
        }

        /// <summary>
        /// These values specify how column selection will be presented to the user
        /// </summary>
        public enum ColumnSelectBehaviour {
            /// <summary>
            /// No column selection will be presented 
            /// </summary>
            None,

            /// <summary>
            /// The columns will be show in the main menu
            /// </summary>
            InlineMenu,

            /// <summary>
            /// The columns will be shown in a submenu
            /// </summary>
            Submenu,

            /// <summary>
            /// A model dialog will be presented to allow the user to choose columns
            /// </summary>
            ModelDialog,

            /*
             * NonModelDialog is just a little bit tricky since the OLV can change views while the dialog is showing
             * So, just comment this out for the time being.
             
            /// <summary>
            /// A non-model dialog will be presented to allow the user to choose columns
            /// </summary>
            NonModelDialog
             * 
             */
        }
    }
}
/*
 * EditorRegistry - A registry mapping types to cell editors.
 *
 * Author: Phillip Piper
 * Date: 6-March-2011 7:53 am
 *
 * Change log:
 * 2011-03-31  JPP  - Use OLVColumn.DataType if the value to be edited is null
 * 2011-03-06  JPP  - Separated from CellEditors.cs
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
using System.Reflection;

namespace BrightIdeasSoftware {

    /// <summary>
    /// A delegate that creates an editor for the given value
    /// </summary>
    /// <param name="model">The model from which that value came</param>
    /// <param name="column">The column for which the editor is being created</param>
    /// <param name="value">A representative value of the type to be edited. This value may not be the exact
    /// value for the column/model combination. It could be simply representative of
    /// the appropriate type of value.</param>
    /// <returns>A control which can edit the given value</returns>
    public delegate Control EditorCreatorDelegate(Object model, OLVColumn column, Object value);

    /// <summary>
    /// An editor registry gives a way to decide what cell editor should be used to edit
    /// the value of a cell. Programmers can register non-standard types and the control that 
    /// should be used to edit instances of that type. 
    /// </summary>
    /// <remarks>
    /// <para>All ObjectListViews share the same editor registry.</para>
    /// </remarks>
    public class EditorRegistry {
        #region Initializing

        /// <summary>
        /// Create an EditorRegistry
        /// </summary>
        public EditorRegistry() {
            this.InitializeStandardTypes();
        }

        private void InitializeStandardTypes() {
            this.Register(typeof(Boolean), typeof(BooleanCellEditor));
            this.Register(typeof(Int16), typeof(IntUpDown));
            this.Register(typeof(Int32), typeof(IntUpDown));
            this.Register(typeof(Int64), typeof(IntUpDown));
            this.Register(typeof(UInt16), typeof(UintUpDown));
            this.Register(typeof(UInt32), typeof(UintUpDown));
            this.Register(typeof(UInt64), typeof(UintUpDown));
            this.Register(typeof(Single), typeof(FloatCellEditor));
            this.Register(typeof(Double), typeof(FloatCellEditor));
            this.Register(typeof(DateTime), delegate(Object model, OLVColumn column, Object value) {
                DateTimePicker c = new DateTimePicker();
                c.Format = DateTimePickerFormat.Short;
                return c;
            });
            this.Register(typeof(Boolean), delegate(Object model, OLVColumn column, Object value) {
                CheckBox c = new BooleanCellEditor2();
                c.ThreeState = column.TriStateCheckBoxes;
                return c;
            });
        }

        #endregion

        #region Registering

        /// <summary>
        /// Register that values of 'type' should be edited by instances of 'controlType'.
        /// </summary>
        /// <param name="type">The type of value to be edited</param>
        /// <param name="controlType">The type of the Control that will edit values of 'type'</param>
        /// <example>
        /// ObjectListView.EditorRegistry.Register(typeof(Color), typeof(MySpecialColorEditor));
        /// </example>
        public void Register(Type type, Type controlType) {
            this.Register(type, delegate(Object model, OLVColumn column, Object value) {
                return controlType.InvokeMember("", BindingFlags.CreateInstance, null, null, null) as Control;
            });
        }

        /// <summary>
        /// Register the given delegate so that it is called to create editors
        /// for values of the given type
        /// </summary>
        /// <param name="type">The type of value to be edited</param>
        /// <param name="creator">The delegate that will create a control that can edit values of 'type'</param>
        /// <example>
        /// ObjectListView.EditorRegistry.Register(typeof(Color), CreateColorEditor);
        /// ...
        /// public Control CreateColorEditor(Object model, OLVColumn column, Object value)
        /// {
        ///     return new MySpecialColorEditor();
        /// }
        /// </example>
        public void Register(Type type, EditorCreatorDelegate creator) {
            this.creatorMap[type] = creator;
        }

        /// <summary>
        /// Register a delegate that will be called to create an editor for values
        /// that have not been handled.
        /// </summary>
        /// <param name="creator">The delegate that will create a editor for all other types</param>
        public void RegisterDefault(EditorCreatorDelegate creator) {
            this.defaultCreator = creator;
        }

        /// <summary>
        /// Register a delegate that will be given a chance to create a control
        /// before any other option is considered.
        /// </summary>
        /// <param name="creator">The delegate that will create a control</param>
        public void RegisterFirstChance(EditorCreatorDelegate creator) {
            this.firstChanceCreator = creator;
        }

        /// <summary>
        /// Remove the registered handler for the given type
        /// </summary>
        /// <remarks>Does nothing if the given type doesn't exist</remarks>
        /// <param name="type">The type whose registration is to be removed</param>
        public void Unregister(Type type) {
            if (this.creatorMap.ContainsKey(type))
                this.creatorMap.Remove(type);
        }

        #endregion

        #region Accessing

        /// <summary>
        /// Create and return an editor that is appropriate for the given value.
        /// Return null if no appropriate editor can be found.
        /// </summary>
        /// <param name="model">The model involved</param>
        /// <param name="column">The column to be edited</param>
        /// <param name="value">The value to be edited. This value may not be the exact
        /// value for the column/model combination. It could be simply representative of
        /// the appropriate type of value.</param>
        /// <returns>A Control that can edit the given type of values</returns>
        public Control GetEditor(Object model, OLVColumn column, Object value) {
            Control editor;

            // Give the first chance delegate a chance to decide
            if (this.firstChanceCreator != null) {
                editor = this.firstChanceCreator(model, column, value);
                if (editor != null)
                    return editor;
            }

            // Try to find a creator based on the type of the value (or the column)
            Type type = value == null ? column.DataType : value.GetType();
            if (type != null && this.creatorMap.ContainsKey(type)) {
                editor = this.creatorMap[type](model, column, value);
                if (editor != null)
                    return editor;
            }

            // Enums without other processing get a special editor
            if (value != null && value.GetType().IsEnum)
                return this.CreateEnumEditor(value.GetType());

            // Give any default creator a final chance
            if (this.defaultCreator != null)
                return this.defaultCreator(model, column, value);

            return null;
        }

        /// <summary>
        /// Create and return an editor that will edit values of the given type
        /// </summary>
        /// <param name="type">A enum type</param>
        protected Control CreateEnumEditor(Type type) {
            return new EnumCellEditor(type);
        }

        #endregion

        #region Private variables

        private EditorCreatorDelegate firstChanceCreator;
        private EditorCreatorDelegate defaultCreator;
        private Dictionary<Type, EditorCreatorDelegate> creatorMap = new Dictionary<Type, EditorCreatorDelegate>();

        #endregion
    }
}

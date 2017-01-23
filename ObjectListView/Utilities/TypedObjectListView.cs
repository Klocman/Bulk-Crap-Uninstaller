/*
 * TypedObjectListView - A wrapper around an ObjectListView that provides type-safe delegates.
 *
 * Author: Phillip Piper
 * Date: 27/09/2008 9:15 AM
 *
 * Change log:
 * v2.6
 * 2012-10-26   JPP  - Handle rare case where a null model object was passed into aspect getters.
 * v2.3
 * 2009-03-31   JPP  - Added Objects property
 * 2008-11-26   JPP  - Added tool tip getting methods
 * 2008-11-05   JPP  - Added CheckState handling methods
 * 2008-10-24   JPP  - Generate dynamic methods MkII. This one handles value types
 * 2008-10-21   JPP  - Generate dynamic methods
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
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Reflection.Emit;

namespace BrightIdeasSoftware
{
    /// <summary>
    /// A TypedObjectListView is a type-safe wrapper around an ObjectListView.
    /// </summary>
    /// <remarks>
    /// <para>VCS does not support generics on controls. It can be faked to some degree, but it
    /// cannot be completely overcome. In our case in particular, there is no way to create
    /// the custom OLVColumn's that we need to truly be generic. So this wrapper is an 
    /// experiment in providing some type-safe access in a way that is useful and available today.</para>
    /// <para>A TypedObjectListView is not more efficient than a normal ObjectListView.
    /// Underneath, the same name of casts are performed. But it is easier to use since you
    /// do not have to write the casts yourself.
    /// </para>
    /// </remarks>
    /// <typeparam name="T">The class of model object that the list will manage</typeparam>
    /// <example>
    /// To use a TypedObjectListView, you write code like this:
    /// <code>
    /// TypedObjectListView&lt;Person> tlist = new TypedObjectListView&lt;Person>(this.listView1);
    /// tlist.CheckStateGetter = delegate(Person x) { return x.IsActive; };
    /// tlist.GetColumn(0).AspectGetter = delegate(Person x) { return x.Name; };
    /// ...
    /// </code>
    /// To iterate over the selected objects, you can write something elegant like this:
    /// <code>
    /// foreach (Person x in tlist.SelectedObjects) {
    ///     x.GrantSalaryIncrease();
    /// }
    /// </code>
    /// </example>
    public class TypedObjectListView<T> where T : class
    {
        /// <summary>
        /// Create a typed wrapper around the given list.
        /// </summary>
        /// <param name="olv">The listview to be wrapped</param>
        public TypedObjectListView(ObjectListView olv) {
            this.olv = olv;
        }

        //--------------------------------------------------------------------------------------
        // Properties

        /// <summary>
        /// Return the model object that is checked, if only one row is checked.
        /// If zero rows are checked, or more than one row, null is returned.
        /// </summary>
        public virtual T CheckedObject {
            get { return (T)this.olv.CheckedObject; }
        }

        /// <summary>
        /// Return the list of all the checked model objects
        /// </summary>
        public virtual IList<T> CheckedObjects {
            get {
                IList checkedObjects = this.olv.CheckedObjects;
                List<T> objects = new List<T>(checkedObjects.Count);
                foreach (object x in checkedObjects)
                    objects.Add((T)x);

                return objects;
            }
            set { this.olv.CheckedObjects = (IList)value; }
        }

        /// <summary>
        /// The ObjectListView that is being wrapped
        /// </summary>
        public virtual ObjectListView ListView {
            get { return olv; }
            set { olv = value; }
        }
        private ObjectListView olv;

        /// <summary>
        /// Get or set the list of all model objects
        /// </summary>
        public virtual IList<T> Objects {
            get {
                List<T> objects = new List<T>(this.olv.GetItemCount());
                for (int i = 0; i < this.olv.GetItemCount(); i++)
                    objects.Add(this.GetModelObject(i));

                return objects;
            }
            set { this.olv.SetObjects(value); }
        }

        /// <summary>
        /// Return the model object that is selected, if only one row is selected.
        /// If zero rows are selected, or more than one row, null is returned.
        /// </summary>
        public virtual T SelectedObject {
            get { return (T)this.olv.SelectedObject; }
            set { this.olv.SelectedObject = value; }
        }

        /// <summary>
        /// The list of model objects that are selected.
        /// </summary>
        public virtual IList<T> SelectedObjects {
            get {
                List<T> objects = new List<T>(this.olv.SelectedIndices.Count);
                foreach (int index in this.olv.SelectedIndices)
                    objects.Add((T)this.olv.GetModelObject(index));

                return objects;
            }
            set { this.olv.SelectedObjects = (IList)value; }
        }

        //--------------------------------------------------------------------------------------
        // Accessors

        /// <summary>
        /// Return a typed wrapper around the column at the given index
        /// </summary>
        /// <param name="i">The index of the column</param>
        /// <returns>A typed column or null</returns>
        public virtual TypedColumn<T> GetColumn(int i) {
            return new TypedColumn<T>(this.olv.GetColumn(i));
        }

        /// <summary>
        /// Return a typed wrapper around the column with the given name
        /// </summary>
        /// <param name="name">The name of the column</param>
        /// <returns>A typed column or null</returns>
        public virtual TypedColumn<T> GetColumn(string name) {
            return new TypedColumn<T>(this.olv.GetColumn(name));
        }

        /// <summary>
        /// Return the model object at the given index
        /// </summary>
        /// <param name="index">The index of the model object</param>
        /// <returns>The model object or null</returns>
        public virtual T GetModelObject(int index) {
            return (T)this.olv.GetModelObject(index);
        }

        //--------------------------------------------------------------------------------------
        // Delegates

        /// <summary>
        /// CheckStateGetter
        /// </summary>
        /// <param name="rowObject"></param>
        /// <returns></returns>
        public delegate CheckState TypedCheckStateGetterDelegate(T rowObject);

        /// <summary>
        /// Gets or sets the check state getter
        /// </summary>
        public virtual TypedCheckStateGetterDelegate CheckStateGetter {
            get { return checkStateGetter; }
            set {
                this.checkStateGetter = value;
                if (value == null)
                    this.olv.CheckStateGetter = null;
                else
                    this.olv.CheckStateGetter = delegate(object x) {
                        return this.checkStateGetter((T)x);
                    };
            }
        }
        private TypedCheckStateGetterDelegate checkStateGetter;

        /// <summary>
        /// BooleanCheckStateGetter
        /// </summary>
        /// <param name="rowObject"></param>
        /// <returns></returns>
        public delegate bool TypedBooleanCheckStateGetterDelegate(T rowObject);

        /// <summary>
        /// Gets or sets the boolean check state getter
        /// </summary>
        public virtual TypedBooleanCheckStateGetterDelegate BooleanCheckStateGetter {
            set {
                if (value == null)
                    this.olv.BooleanCheckStateGetter = null;
                else
                    this.olv.BooleanCheckStateGetter = delegate(object x) {
                        return value((T)x);
                    };
            }
        }

        /// <summary>
        /// CheckStatePutter
        /// </summary>
        /// <param name="rowObject"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public delegate CheckState TypedCheckStatePutterDelegate(T rowObject, CheckState newValue);

        /// <summary>
        /// Gets or sets the check state putter delegate
        /// </summary>
        public virtual TypedCheckStatePutterDelegate CheckStatePutter {
            get { return checkStatePutter; }
            set {
                this.checkStatePutter = value;
                if (value == null)
                    this.olv.CheckStatePutter = null;
                else
                    this.olv.CheckStatePutter = delegate(object x, CheckState newValue) {
                        return this.checkStatePutter((T)x, newValue);
                    };
            }
        }
        private TypedCheckStatePutterDelegate checkStatePutter;

        /// <summary>
        /// BooleanCheckStatePutter
        /// </summary>
        /// <param name="rowObject"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public delegate bool TypedBooleanCheckStatePutterDelegate(T rowObject, bool newValue);

        /// <summary>
        /// Gets or sets the boolean check state putter
        /// </summary>
        public virtual TypedBooleanCheckStatePutterDelegate BooleanCheckStatePutter {
            set {
                if (value == null)
                    this.olv.BooleanCheckStatePutter = null;
                else
                    this.olv.BooleanCheckStatePutter = delegate(object x, bool newValue) {
                        return value((T)x, newValue);
                    };
            }
        }

        /// <summary>
        /// ToolTipGetter
        /// </summary>
        /// <param name="column"></param>
        /// <param name="modelObject"></param>
        /// <returns></returns>
        public delegate String TypedCellToolTipGetterDelegate(OLVColumn column, T modelObject);

        /// <summary>
        /// Gets or sets the cell tooltip getter
        /// </summary>
        public virtual TypedCellToolTipGetterDelegate CellToolTipGetter {
            set {
                if (value == null)
                    this.olv.CellToolTipGetter = null;
                else
                    this.olv.CellToolTipGetter = delegate(OLVColumn col, Object x) {
                        return value(col, (T)x);
                    };
            }
        }

        /// <summary>
        /// Gets or sets the header tool tip getter
        /// </summary>
        public virtual HeaderToolTipGetterDelegate HeaderToolTipGetter {
            get { return this.olv.HeaderToolTipGetter; }
            set { this.olv.HeaderToolTipGetter = value; }
        }

        //--------------------------------------------------------------------------------------
        // Commands

        /// <summary>
        /// This method will generate AspectGetters for any column that has an AspectName.
        /// </summary>
        public virtual void GenerateAspectGetters() {
            for (int i = 0; i < this.ListView.Columns.Count; i++)
                this.GetColumn(i).GenerateAspectGetter();
        }
    }

    /// <summary>
    /// A type-safe wrapper around an OLVColumn
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TypedColumn<T> where T : class
    {
        /// <summary>
        /// Creates a TypedColumn
        /// </summary>
        /// <param name="column"></param>
        public TypedColumn(OLVColumn column) {
            this.column = column;
        }
        private OLVColumn column;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowObject"></param>
        /// <returns></returns>
        public delegate Object TypedAspectGetterDelegate(T rowObject);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowObject"></param>
        /// <param name="newValue"></param>
        public delegate void TypedAspectPutterDelegate(T rowObject, Object newValue);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowObject"></param>
        /// <returns></returns>
        public delegate Object TypedGroupKeyGetterDelegate(T rowObject);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowObject"></param>
        /// <returns></returns>
        public delegate Object TypedImageGetterDelegate(T rowObject);

        /// <summary>
        /// 
        /// </summary>
        public TypedAspectGetterDelegate AspectGetter {
            get { return this.aspectGetter; }
            set {
                this.aspectGetter = value;
                if (value == null)
                    this.column.AspectGetter = null;
                else
                    this.column.AspectGetter = delegate(object x) {
                        return x == null ? null : this.aspectGetter((T)x);
                    };
            }
        }
        private TypedAspectGetterDelegate aspectGetter;

        /// <summary>
        /// 
        /// </summary>
        public TypedAspectPutterDelegate AspectPutter {
            get { return aspectPutter; }
            set {
                this.aspectPutter = value;
                if (value == null)
                    this.column.AspectPutter = null;
                else
                    this.column.AspectPutter = delegate(object x, object newValue) {
                        this.aspectPutter((T)x, newValue);
                    };
            }
        }
        private TypedAspectPutterDelegate aspectPutter;

        /// <summary>
        /// 
        /// </summary>
        public TypedImageGetterDelegate ImageGetter {
            get { return imageGetter; }
            set {
                this.imageGetter = value;
                if (value == null)
                    this.column.ImageGetter = null;
                else
                    this.column.ImageGetter = delegate(object x) {
                        return this.imageGetter((T)x);
                    };
            }
        }
        private TypedImageGetterDelegate imageGetter;

        /// <summary>
        /// 
        /// </summary>
        public TypedGroupKeyGetterDelegate GroupKeyGetter {
            get { return groupKeyGetter; }
            set {
                this.groupKeyGetter = value;
                if (value == null)
                    this.column.GroupKeyGetter = null;
                else
                    this.column.GroupKeyGetter = delegate(object x) {
                        return this.groupKeyGetter((T)x);
                    };
            }
        }
        private TypedGroupKeyGetterDelegate groupKeyGetter;

        #region Dynamic methods

        /// <summary>
        /// Generate an aspect getter that does the same thing as the AspectName,
        /// except without using reflection.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If you have an AspectName of "Owner.Address.Postcode", this will generate
        /// the equivilent of: <code>this.AspectGetter = delegate (object x) {
        ///     return x.Owner.Address.Postcode;
        /// }
        /// </code>
        /// </para>
        /// <para>
        /// If AspectName is empty, this method will do nothing, otherwise 
        /// this will replace any existing AspectGetter.
        /// </para>
        /// </remarks>
        public void GenerateAspectGetter() {
            if (!String.IsNullOrEmpty(this.column.AspectName))
                this.AspectGetter = this.GenerateAspectGetter(typeof(T), this.column.AspectName);
        }

        /// <summary>
        /// Generates an aspect getter method dynamically. The method will execute
        /// the given dotted chain of selectors against a model object given at runtime.
        /// </summary>
        /// <param name="type">The type of model object to be passed to the generated method</param>
        /// <param name="path">A dotted chain of selectors. Each selector can be the name of a 
        /// field, property or parameter-less method.</param>
        /// <returns>A typed delegate</returns>
        private TypedAspectGetterDelegate GenerateAspectGetter(Type type, string path) {
            DynamicMethod getter = new DynamicMethod(String.Empty,
                typeof(Object), new Type[] { type }, type, true);
            this.GenerateIL(type, path, getter.GetILGenerator());
            return (TypedAspectGetterDelegate)getter.CreateDelegate(typeof(TypedAspectGetterDelegate));
        }

        /// <summary>
        /// This method generates the actual IL for the method.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="path"></param>
        /// <param name="il"></param>
        private void GenerateIL(Type type, string path, ILGenerator il) {
            // Push our model object onto the stack
            il.Emit(OpCodes.Ldarg_0);

            // Generate the IL to access each part of the dotted chain
            string[] parts = path.Split('.');
            for (int i = 0; i < parts.Length; i++) {
                type = this.GeneratePart(il, type, parts[i], (i == parts.Length - 1));
                if (type == null)
                    break;
            }

            // If the object to be returned is a value type (e.g. int, bool), it
            // must be boxed, since the delegate returns an Object
            if (type != null && type.IsValueType && !typeof(T).IsValueType)
                il.Emit(OpCodes.Box, type);

            il.Emit(OpCodes.Ret);
        }

        private Type GeneratePart(ILGenerator il, Type type, string pathPart, bool isLastPart) {
            // TODO: Generate check for null

            // Find the first member with the given nam that is a field, property, or parameter-less method
            List<MemberInfo> infos = new List<MemberInfo>(type.GetMember(pathPart));
            MemberInfo info = infos.Find(delegate(MemberInfo x) {
                if (x.MemberType == MemberTypes.Field || x.MemberType == MemberTypes.Property)
                    return true;
                if (x.MemberType == MemberTypes.Method)
                    return ((MethodInfo)x).GetParameters().Length == 0;
                else
                    return false;
            });

            // If we couldn't find anything with that name, pop the current result and return an error
            if (info == null) {
                il.Emit(OpCodes.Pop);
                if (Munger.IgnoreMissingAspects)
                    il.Emit(OpCodes.Ldnull);
                else
                    il.Emit(OpCodes.Ldstr, String.Format("'{0}' is not a parameter-less method, property or field of type '{1}'", pathPart, type.FullName));
                return null;
            }

            // Generate the correct IL to access the member. We remember the type of object that is going to be returned
            // so that we can do a method lookup on it at the next iteration
            Type resultType = null;
            switch (info.MemberType) {
                case MemberTypes.Method:
                    MethodInfo mi = (MethodInfo)info;
                    if (mi.IsVirtual)
                        il.Emit(OpCodes.Callvirt, mi);
                    else
                        il.Emit(OpCodes.Call, mi);
                    resultType = mi.ReturnType;
                    break;
                case MemberTypes.Property:
                    PropertyInfo pi = (PropertyInfo)info;
                    il.Emit(OpCodes.Call, pi.GetGetMethod());
                    resultType = pi.PropertyType;
                    break;
                case MemberTypes.Field:
                    FieldInfo fi = (FieldInfo)info;
                    il.Emit(OpCodes.Ldfld, fi);
                    resultType = fi.FieldType;
                    break;
            }

            // If the method returned a value type, and something is going to call a method on that value,
            // we need to load its address onto the stack, rather than the object itself.
            if (resultType.IsValueType && !isLastPart) {
                LocalBuilder lb = il.DeclareLocal(resultType);
                il.Emit(OpCodes.Stloc, lb);
                il.Emit(OpCodes.Ldloca, lb);
            }

            return resultType;
        }

        #endregion
    }
}

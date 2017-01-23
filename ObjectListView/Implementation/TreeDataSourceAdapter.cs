using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;

namespace BrightIdeasSoftware
{
    /// <summary>
    /// A TreeDataSourceAdapter knows how to build a tree structure from a binding list.
    /// </summary>
    /// <remarks>To build a tree</remarks>
    public class TreeDataSourceAdapter : DataSourceAdapter
    {
        #region Life and death

        /// <summary>
        /// Create a data source adaptor that knows how to build a tree structure
        /// </summary>
        /// <param name="tlv"></param>
        public TreeDataSourceAdapter(DataTreeListView tlv)
            : base(tlv) {
            this.treeListView = tlv;
            this.treeListView.CanExpandGetter = delegate(object model) { return this.CalculateHasChildren(model); };
            this.treeListView.ChildrenGetter = delegate(object model) { return this.CalculateChildren(model); };
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the name of the property/column that uniquely identifies each row.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The value contained by this column must be unique across all rows 
        /// in the data source. Odd and unpredictable things will happen if two
        /// rows have the same id.
        /// </para>
        /// <para>Null cannot be a valid key value.</para>
        /// </remarks>
        public virtual string KeyAspectName {
            get { return keyAspectName; }
            set {
                if (keyAspectName == value)
                    return;
                keyAspectName = value;
                this.keyMunger = new Munger(this.KeyAspectName);
                this.InitializeDataSource();
            }
        }
        private string keyAspectName;

        /// <summary>
        /// Gets or sets the name of the property/column that contains the key of
        /// the parent of a row.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The test condition for deciding if one row is the parent of another is functionally
        /// equivilent to this:
        /// <code>
        /// Object.Equals(candidateParentRow[this.KeyAspectName], row[this.ParentKeyAspectName])
        /// </code>
        /// </para>
        /// <para>Unlike key value, parent keys can be null but a null parent key can only be used
        /// to identify root objects.</para>
        /// </remarks>
        public virtual string ParentKeyAspectName {
            get { return parentKeyAspectName; }
            set {
                if (parentKeyAspectName == value)
                    return;
                parentKeyAspectName = value;
                this.parentKeyMunger = new Munger(this.ParentKeyAspectName);
                this.InitializeDataSource();
            }
        }
        private string parentKeyAspectName;

        /// <summary>
        /// Gets or sets the value that identifies a row as a root object.
        /// When the ParentKey of a row equals the RootKeyValue, that row will
        /// be treated as root of the TreeListView.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The test condition for deciding a root object is functionally
        /// equivilent to this:
        /// <code>
        /// Object.Equals(candidateRow[this.ParentKeyAspectName], this.RootKeyValue)
        /// </code>
        /// </para>
        /// <para>The RootKeyValue can be null.</para>
        /// </remarks>
        public virtual object RootKeyValue {
            get { return rootKeyValue; }
            set {
                if (Equals(rootKeyValue, value))
                    return;
                rootKeyValue = value;
                this.InitializeDataSource();
            }
        }
        private object rootKeyValue;

        /// <summary>
        /// Gets or sets whether or not the key columns (id and parent id) should
        /// be shown to the user.
        /// </summary>
        /// <remarks>This must be set before the DataSource is set. It has no effect
        /// afterwards.</remarks>
        public virtual bool ShowKeyColumns {
            get { return showKeyColumns; }
            set { showKeyColumns = value; }
        }
        private bool showKeyColumns = true;


        #endregion

        #region Implementation properties

        /// <summary>
        /// Gets the DataTreeListView that is being managed 
        /// </summary>
        protected DataTreeListView TreeListView {
            get { return treeListView; }
        }
        private readonly DataTreeListView treeListView;

        #endregion

        #region Implementation

        /// <summary>
        /// 
        /// </summary>
        protected override void InitializeDataSource() {
            base.InitializeDataSource();
            this.TreeListView.RebuildAll(true);
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void SetListContents() {
            this.TreeListView.Roots = this.CalculateRoots();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        protected override bool ShouldCreateColumn(PropertyDescriptor property) {
            // If the property is a key column, and we aren't supposed to show keys, don't show it
            if (!this.ShowKeyColumns && (property.Name == this.KeyAspectName || property.Name == this.ParentKeyAspectName))
                return false;

            return base.ShouldCreateColumn(property);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void HandleListChangedItemChanged(System.ComponentModel.ListChangedEventArgs e) {
            // If the id or the parent id of a row changes, we just rebuild everything.
            // We can't do anything more specific. We don't know what the previous values, so we can't 
            // tell the previous parent to refresh itself. If the id itself has changed, things that used
            // to be children will no longer be children. Just rebuild everything.
            // It seems PropertyDescriptor is only filled in .NET 4 :(
            if (e.PropertyDescriptor != null && 
                (e.PropertyDescriptor.Name == this.KeyAspectName ||
                 e.PropertyDescriptor.Name == this.ParentKeyAspectName))
                this.InitializeDataSource();
            else
                base.HandleListChangedItemChanged(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        protected override void ChangePosition(int index) {
            // We can't use our base method directly, since the normal position management
            // doesn't know about our tree structure. They treat our dataset as a flat list
            // but we have a collapsable structure. This means that the 5'th row to them
            // may not even be visible to us

            // To display the n'th row, we have to make sure that all its ancestors
            // are expanded. Then we will be able to select it.
            object model = this.CurrencyManager.List[index];
            object parent = this.CalculateParent(model);
            while (parent != null && !this.TreeListView.IsExpanded(parent)) {
                this.TreeListView.Expand(parent);
                parent = this.CalculateParent(parent);
            }

            base.ChangePosition(index);
        }

        private IEnumerable CalculateRoots() {
            foreach (object x in this.CurrencyManager.List) {
                object parentKey = this.GetParentValue(x);
                if (Object.Equals(this.RootKeyValue, parentKey))
                    yield return x;
            }
        }

        private bool CalculateHasChildren(object model) {
            object keyValue = this.GetKeyValue(model);
            if (keyValue == null)
                return false;

            foreach (object x in this.CurrencyManager.List) {
                object parentKey = this.GetParentValue(x);
                if (Object.Equals(keyValue, parentKey))
                    return true;
            }
            return false;
        }

        private IEnumerable CalculateChildren(object model) {
            object keyValue = this.GetKeyValue(model);
            if (keyValue != null) {
                foreach (object x in this.CurrencyManager.List) {
                    object parentKey = this.GetParentValue(x);
                    if (Object.Equals(keyValue, parentKey))
                        yield return x;
                }
            }
        }

        private object CalculateParent(object model) {
            object parentValue = this.GetParentValue(model);
            if (parentValue == null) 
                return null;

            foreach (object x in this.CurrencyManager.List) {
                object key = this.GetKeyValue(x);
                if (Object.Equals(parentValue, key))
                    return x;
            }
            return null;
        }

        private object GetKeyValue(object model) {
            return this.keyMunger == null ? null : this.keyMunger.GetValue(model);
        }

        private object GetParentValue(object model) {
            return this.parentKeyMunger == null ? null : this.parentKeyMunger.GetValue(model);
        }

        #endregion

        private Munger keyMunger;
        private Munger parentKeyMunger;
    }
}
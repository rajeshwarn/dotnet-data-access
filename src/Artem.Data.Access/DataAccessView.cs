using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Artem.Data.Access {

    /// <summary>
    /// 
    /// </summary>
    public class DataAccessView<T> : BindingList<T>, IBindingListView, IRaiseItemChangedEvents {

        #region Fields  /////////////////////////////////////////////////////////////////

        private bool m_Sorted = false;
        private bool m_Filtered = false;
        private string m_FilterString = null;
        private ListSortDirection m_SortDirection =
           ListSortDirection.Ascending;
        private PropertyDescriptor m_SortProperty = null;
        private ListSortDescriptionCollection m_SortDescriptions =
         new ListSortDescriptionCollection();
        private List<T> m_OriginalCollection = new List<T>(); 

        #endregion

        #region Construct  //////////////////////////////////////////////////////////////

        /// <summary>
        /// Initializes a new instance of the <see cref="T:DataAccessView&lt;T&gt;"/> class.
        /// </summary>
        public DataAccessView()
            : base() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:DataAccessView&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="list">The list.</param>
        public DataAccessView(List<T> list)
            : base(list) {
        } 
        #endregion

        #region Properties  /////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets a value indicating whether the list supports sorting.
        /// </summary>
        /// <value></value>
        /// <returns>true if the list supports sorting; otherwise, false. The default is false.</returns>
        protected override bool SupportsSortingCore {
            get { return true; }
        }
        /// <summary>
        /// Gets a value indicating whether the list is sorted.
        /// </summary>
        /// <value></value>
        /// <returns>true if the list is sorted; otherwise, false. The default is false.</returns>
        protected override bool IsSortedCore {
            get { return m_Sorted; }
        }

        /// <summary>
        /// Gets the direction the list is sorted.
        /// </summary>
        /// <value></value>
        /// <returns>One of the <see cref="T:System.ComponentModel.ListSortDirection"></see> values. The default is <see cref="F:System.ComponentModel.ListSortDirection.Ascending"></see>. </returns>
        protected override ListSortDirection SortDirectionCore {
            get { return m_SortDirection; }
        }

        /// <summary>
        /// Gets a value indicating whether the list supports searching.
        /// </summary>
        /// <value></value>
        /// <returns>true if the list supports searching; otherwise, false. The default is false.</returns>
        protected override bool SupportsSearchingCore {
            get { return true; }
        }

        /// <summary>
        /// Gets the property descriptor that is used for sorting the list if sorting is implemented in a derived class; otherwise, returns null.
        /// </summary>
        /// <value></value>
        /// <returns>The <see cref="T:System.ComponentModel.PropertyDescriptor"></see> used for sorting the list.</returns>
        protected override PropertyDescriptor SortPropertyCore {
            get { return m_SortProperty; }
        }

        /// <summary>
        /// Gets or sets the filter to be used to exclude items from the collection of items returned by the data source
        /// </summary>
        /// <value></value>
        /// <returns>The string used to filter items out in the item collection returned by the data source. </returns>
        string IBindingListView.Filter {
            get {
                return m_FilterString;
            }
            set {
                m_FilterString = value;
                m_Filtered = true;
                UpdateFilter();
            }
        }

        /// <summary>
        /// Gets the collection of sort descriptions currently applied to the data source.
        /// </summary>
        /// <value></value>
        /// <returns>The <see cref="T:System.ComponentModel.ListSortDescriptionCollection"></see> currently applied to the data source.</returns>
        ListSortDescriptionCollection IBindingListView.SortDescriptions {
            get { return m_SortDescriptions; }
        }

        /// <summary>
        /// Gets a value indicating whether the data source supports advanced sorting.
        /// </summary>
        /// <value></value>
        /// <returns>true if the data source supports advanced sorting; otherwise, false. </returns>
        bool IBindingListView.SupportsAdvancedSorting {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether the data source supports filtering.
        /// </summary>
        /// <value></value>
        /// <returns>true if the data source supports filtering; otherwise, false. </returns>
        bool IBindingListView.SupportsFiltering {
            get { return true; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether you can add items to the list using the <see cref="M:System.ComponentModel.BindingList`1.AddNew"></see> method.
        /// </summary>
        /// <value></value>
        /// <returns>true if you can add items to the list with the <see cref="M:System.ComponentModel.BindingList`1.AddNew"></see> method; otherwise, false. The default depends on the underlying type contained in the list.</returns>
        bool IBindingList.AllowNew {
            get { return CheckReadOnly(); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether you can remove items from the collection.
        /// </summary>
        /// <value></value>
        /// <returns>true if you can remove items from the list with the <see cref="M:System.ComponentModel.BindingList`1.RemoveItem(System.Int32)"></see> method otherwise, false. The default is true.</returns>
        bool IBindingList.AllowRemove {
            get { return CheckReadOnly(); }
        }

        bool IRaiseItemChangedEvents.RaisesItemChangedEvents {
            get { return true; }
        }
        #endregion

        /// <summary>
        /// Finds the core.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        protected override int FindCore(PropertyDescriptor property, object key) {
            // Simple iteration:
            for (int i = 0; i < Count; i++) {
                T item = this[i];
                if (property.GetValue(item).Equals(key)) {
                    return i;
                }
            }
            return -1; // Not found
            #region Alternative search implementation
            // using List.FindIndex:
            //Predicate<T> pred = delegate(T item)
            //{
            // if (property.GetValue(item).Equals(key))
            // return true;
            // else
            // return false;
            //};
            //List<T> list = Items as List<T>;
            //if (list == null)
            // return -1;
            //return list.FindIndex(pred);
            #endregion
        }

        /// <summary>
        /// Applies the sort core.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="direction">The direction.</param>
        protected override void ApplySortCore(
            PropertyDescriptor property, ListSortDirection direction) {

            m_SortDirection = direction;
            m_SortProperty = property;
            SortComparer<T> comparer =
               new SortComparer<T>(property, direction);
            ApplySortInternal(comparer);
        }

        /// <summary>
        /// Applies the sort internal.
        /// </summary>
        /// <param name="comparer">The comparer.</param>
        private void ApplySortInternal(SortComparer<T> comparer) {
            if (m_OriginalCollection.Count == 0) {
                m_OriginalCollection.AddRange(this);
            }
            List<T> listRef = this.Items as List<T>;
            if (listRef == null)
                return;
            listRef.Sort(comparer);
            m_Sorted = true;
            OnListChanged(new ListChangedEventArgs(
              ListChangedType.Reset, -1));
        }

        /// <summary>
        /// Removes any sort applied with <see cref="M:System.ComponentModel.BindingList`1.ApplySortCore(System.ComponentModel.PropertyDescriptor,System.ComponentModel.ListSortDirection)"></see> if sorting is implemented in a derived class; otherwise, raises <see cref="T:System.NotSupportedException"></see>.
        /// </summary>
        protected override void RemoveSortCore() {
            if (!m_Sorted)
                return; Clear();
            foreach (T item in m_OriginalCollection) {
                Add(item);
            }
            m_OriginalCollection.Clear();
            m_SortProperty = null;
            m_SortDescriptions = null;
            m_Sorted = false;
        }

        /// <summary>
        /// Sorts the data source based on the given <see cref="T:System.ComponentModel.ListSortDescriptionCollection"></see>.
        /// </summary>
        /// <param name="sorts">The <see cref="T:System.ComponentModel.ListSortDescriptionCollection"></see> containing the sorts to apply to the data source.</param>
        void IBindingListView.ApplySort(ListSortDescriptionCollection sorts) {
            m_SortProperty = null;
            m_SortDescriptions = sorts;
            SortComparer<T> comparer = new SortComparer<T>(sorts);
            ApplySortInternal(comparer);
        }

        /// <summary>
        /// Removes the current filter applied to the data source.
        /// </summary>
        void IBindingListView.RemoveFilter() {
            if (!m_Filtered)
                return;
            m_FilterString = null;
            m_Filtered = false;
            m_Sorted = false;
            m_SortDescriptions = null;
            m_SortProperty = null;
            Clear();
            foreach (T item in m_OriginalCollection) {
                Add(item);
            }
            m_OriginalCollection.Clear();
        }

        /// <summary>
        /// Updates the filter.
        /// </summary>
        protected virtual void UpdateFilter() {
            int equalsPos = m_FilterString.IndexOf('=');
            // Get property name
            string propName = m_FilterString.Substring(0, equalsPos).Trim();
            // Get filter criteria
            string criteria = m_FilterString.Substring(equalsPos + 1,
               m_FilterString.Length - equalsPos - 1).Trim();
            // Strip leading and trailing quotes
            criteria = criteria.Substring(1, criteria.Length - 2);
            // Get a property descriptor for the filter property
            PropertyDescriptor propDesc =
               TypeDescriptor.GetProperties(typeof(T))[propName];
            if (m_OriginalCollection.Count == 0) {
                m_OriginalCollection.AddRange(this);
            }
            List<T> currentCollection = new List<T>(this);
            Clear();
            foreach (T item in currentCollection) {
                object value = propDesc.GetValue(item);
                if (value.ToString() == criteria) {
                    Add(item);
                }
            }
        }

        /// <summary>
        /// Checks the read only.
        /// </summary>
        /// <returns></returns>
        private bool CheckReadOnly() {
            if (m_Sorted || m_Filtered) {
                return false;
            }
            else {
                return true;
            }
        }

        /// <summary>
        /// Inserts the specified item in the list at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index where the item is to be inserted.</param>
        /// <param name="item">The item to insert in the list.</param>
        protected override void InsertItem(int index, T item) {
            foreach (PropertyDescriptor propDesc in
               TypeDescriptor.GetProperties(item)) {
                if (propDesc.SupportsChangeEvents) {
                    propDesc.AddValueChanged(item, OnItemChanged);
                }
            }
            base.InsertItem(index, item);
        }

        /// <summary>
        /// Removes the item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        protected override void RemoveItem(int index) {
            T item = Items[index];
            PropertyDescriptorCollection propDescs =
               TypeDescriptor.GetProperties(item);
            foreach (PropertyDescriptor propDesc in propDescs) {
                if (propDesc.SupportsChangeEvents) {
                    propDesc.RemoveValueChanged(item, OnItemChanged);
                }
            }
            base.RemoveItem(index);
        }

        /// <summary>
        /// Called when [item changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        void OnItemChanged(object sender, EventArgs args) {
            int index = Items.IndexOf((T)sender);
            OnListChanged(new ListChangedEventArgs(
              ListChangedType.ItemChanged, index));
        }
    }

    #region SortComparer ///////////////////////////////////////////////////////////
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class SortComparer<T> : IComparer<T> {
        private ListSortDescriptionCollection m_SortCollection = null;
        private PropertyDescriptor m_PropDesc = null;
        private ListSortDirection m_Direction =
           ListSortDirection.Ascending;

        public SortComparer(PropertyDescriptor propDesc,
           ListSortDirection direction) {
            m_PropDesc = propDesc;
            m_Direction = direction;
        }

        public SortComparer(ListSortDescriptionCollection sortCollection) {
            m_SortCollection = sortCollection;
        }

        int IComparer<T>.Compare(T x, T y) {
            if (m_PropDesc != null) {// Simple sort
                object xValue = m_PropDesc.GetValue(x);
                object yValue = m_PropDesc.GetValue(y);
                return CompareValues(xValue, yValue, m_Direction);
            }
            else if (m_SortCollection != null &&
                m_SortCollection.Count > 0) {
                return RecursiveCompareInternal(x, y, 0);
            }
            else return 0;
        }

        private int CompareValues(object xValue, object yValue,
           ListSortDirection direction) {

            int retValue = 0;
            if (xValue is IComparable) {// Can ask the x value
                retValue = ((IComparable)xValue).CompareTo(yValue);
            }
            else if (yValue is IComparable) {//Can ask the y value
                retValue = ((IComparable)yValue).CompareTo(xValue);
            }
            // not comparable, compare String representations
            else if (!xValue.Equals(yValue)) {
                retValue = xValue.ToString().CompareTo(yValue.ToString());
            }
            if (direction == ListSortDirection.Ascending) {
                return retValue;
            }
            else {
                return retValue * -1;
            }
        }

        private int RecursiveCompareInternal(T x, T y, int index) {
            if (index >= m_SortCollection.Count)
                return 0; // termination condition

            ListSortDescription listSortDesc = m_SortCollection[index];
            object xValue = listSortDesc.PropertyDescriptor.GetValue(x);
            object yValue = listSortDesc.PropertyDescriptor.GetValue(y);

            int retValue = CompareValues(xValue,
               yValue, listSortDesc.SortDirection);
            if (retValue == 0) {
                return RecursiveCompareInternal(x, y, ++index);
            }
            else {
                return retValue;
            }
        }
    }
    #endregion
}

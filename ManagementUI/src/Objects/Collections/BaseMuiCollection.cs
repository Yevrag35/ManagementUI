using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;

namespace ManagementUI
{
    /// <summary>
    /// The <see langword="abstract"/> base <see cref="List{T}"/> class for MUI cloneable items which supports attaching to <see cref="ListView"/> as an ItemsSource
    /// while also implementing <see cref="INotifyCollectionChanged"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseMuiCollection<T> : ICollection<T>, IIndex<T>, IIndex, IList, INotifyCollectionChanged, ISorter<T> where T : ICloneable
    {
        #region PRIVATE FIELDS/CONSTANTS
        protected List<T> InnerList;
        protected ListCollectionView InnerView;

        #endregion

        #region INDEXERS
        public T this[int index]
        {
            get => this.InnerList[index];
            set => this.InnerList[index] = value;
        }
        object IIndex.this[int index] => this[index];
        object IList.this[int index]
        {
            get => this[index];
            set => this.InnerList[index] = (T)value;
        }

        #endregion

        #region CONSTRUCTORS
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseMuiCollection{T}"/> class that is empty 
        /// and has the default initial capacity.
        /// </summary>
        public BaseMuiCollection()
        {
            InnerList = new List<T>();
            InnerView = CollectionViewSource.GetDefaultView(InnerList) as ListCollectionView;
            InnerView.IsLiveSorting = true;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseMuiCollection{T}"/> class that is empty
        /// and has the specified initial capacity.
        /// </summary>
        /// <param name="capacity">The number of elements that the new collection can initially store.</param>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public BaseMuiCollection(int capacity)
        {
            InnerList = new List<T>(capacity);
            InnerView = CollectionViewSource.GetDefaultView(InnerList) as ListCollectionView;
            InnerView.IsLiveSorting = true;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseMuiCollection{T}"/> class that
        /// contains elements copied from the specified <see cref="IEnumerable{T}"/> and has
        /// sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="items">The collection whose elements are copied to the new list.</param>
        /// <exception cref="ArgumentNullException"/>
        public BaseMuiCollection(IEnumerable<T> items)
        {
            InnerList = new List<T>(items);
            InnerView = CollectionViewSource.GetDefaultView(InnerList) as ListCollectionView;
            InnerView.IsLiveSorting = true;
        }

        #endregion

        #region BASE PROPERTIES
        public int Count => this.InnerList.Count;
        public virtual bool IsReadOnly => false;
        public ListCollectionView View => InnerView;

        #endregion

        #region EVENT HANDLERS
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (this.CollectionChanged != null)
            {
                this.CollectionChanged(this, e);
            }
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedAction action, object item)
        {
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item));
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index)
        {
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index));
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedAction action, IList items)
        {
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, items));
        }

        #endregion

        #region BASE METHODS
        /// <summary>
        /// Adds an item to end of the collection and triggers a collection changed event.
        /// </summary>
        /// <param name="item">The object to be added to the end of the collection.</param>
        /// <exception cref="ArgumentNullException"/>
        public void Add(T item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            InnerList.Add(item);
            this.OnCollectionChanged(NotifyCollectionChangedAction.Add, item);
        }
        /// <summary>
        /// Adds the elements of the specified <see cref="IEnumerable{T}"/> to the end of the collection.
        /// </summary>
        /// <param name="apps">The collection whose elements should be added to the end of this collection.  The collection
        /// itself cannot be null, but it can contain elements that are null if <see cref="T"/> is a reference type.</param>
        /// <param name="notify">Indicates whether to trigger a collection changed event.</param>
        /// <exception cref="ArgumentNullException"/>
        public void AddRange(IEnumerable<T> apps, bool notify = true)
        {
            if (apps == null)
                throw new ArgumentNullException("apps");

            var listOfItems = apps.ToList();
            InnerList.AddRange(listOfItems);
            if (notify)
                this.OnCollectionChanged(NotifyCollectionChangedAction.Add, listOfItems);
        }

        public void Clear()
        {
            var oldItems = InnerList.ToList();
            InnerList.Clear();
            this.OnCollectionChanged(NotifyCollectionChangedAction.Reset, oldItems);
        }
        public bool Contains(T item) => InnerList.Contains(item);
        public bool Contains(Predicate<T> match) => InnerList.Exists(match);
        public void CopyTo(T[] array, int arrayIndex) => InnerList.CopyTo(array, arrayIndex);
        public IEnumerator<T> GetEnumerator() => InnerList.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => InnerList.GetEnumerator();
        public int IndexOf(T item) => InnerList.IndexOf(item);
        public void Insert(int index, T item)
        {
            InnerList.Insert(index, item);
            this.OnCollectionChanged(NotifyCollectionChangedAction.Add, item);
        }
        public virtual void Sort() => InnerList.Sort();
        public void Sort(IComparer<T> comparer) => InnerList.Sort(comparer);
        public bool Remove(T item)
        {
            var removing = item.Clone();
            bool result = InnerList.Remove(item);
            if (result)
                this.OnCollectionChanged(NotifyCollectionChangedAction.Remove, removing);

            return result;
        }
        public void RemoveAt(int index)
        {
            var removing = InnerList[index].Clone();
            InnerList.RemoveAt(index);
            this.OnCollectionChanged(NotifyCollectionChangedAction.Remove, removing);
        }

        #endregion

        #region INTERFACE IMPLEMENTATIONS

        #region INTERFACE-ONLY PROPERTIES
        bool IList.IsFixedSize => ((IList)InnerList).IsFixedSize;
        bool ICollection.IsSynchronized => ((ICollection)InnerList).IsSynchronized;
        object ICollection.SyncRoot => ((ICollection)InnerList).SyncRoot;

        #endregion

        #region GENERIC ICOLLECTION METHODS
        void ICollection<T>.Add(T item) => InnerList.Add(item);
        void ICollection<T>.Clear() => InnerList.Clear();
        bool ICollection<T>.Remove(T item) => InnerList.Remove(item);

        #endregion

        #region NON-GENERIC ICOLLECTION METHODS
        void ICollection.CopyTo(Array array, int index) => ((ICollection)InnerList).CopyTo(array, index);

        #endregion

        #region NON-GENERIC ILIST METHODS
        int IList.Add(object value) => ((IList)InnerList).Add(value);
        bool IList.Contains(object value) => ((IList)InnerList).Contains(value);
        int IList.IndexOf(object value) => ((IList)InnerList).IndexOf(value);
        void IList.Insert(int index, object value) => ((IList)InnerList).Insert(index, value);
        void IList.Remove(object value) => ((IList)InnerList).Remove(value);
        void IList.RemoveAt(int index) => this.InnerList.RemoveAt(index);

        #endregion

        #endregion

        #region UPDATE VIEW
        public void UpdateView(bool useLiveSorting = true)
        {
            this.Sort();
            this.UpdateView(useLiveSorting, true);
        }
        public void UpdateView(IComparer<T> sortBy, bool useLiveSorting = true)
        {
            InnerList.Sort(sortBy);
            this.UpdateView(useLiveSorting, true);
        }
        private void UpdateView(bool useLiveSorting, bool isPrivate)
        {
            InnerView = CollectionViewSource.GetDefaultView(InnerList) as ListCollectionView;
            InnerView.IsLiveSorting = useLiveSorting;
        }

        #endregion
    }
}
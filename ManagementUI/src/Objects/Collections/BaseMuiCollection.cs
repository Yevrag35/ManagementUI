using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Data;

namespace ManagementUI
{
    public abstract class BaseMuiCollection<T> : IList<T>, IList, INotifyCollectionChanged where T : ICloneable
    {
        #region PRIVATE FIELDS/CONSTANTS
        protected ListCollectionView InnerView;
        private List<T> _list;

        #endregion

        #region INDEXERS
        public virtual T this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }
        object IList.this[int index]
        {
            get => this[index];
            set => this[index] = (T)value;
        }

        #endregion

        #region CONSTRUCTORS
        public BaseMuiCollection()
        {
            _list = new List<T>();
            InnerView = CollectionViewSource.GetDefaultView(_list) as ListCollectionView;
            InnerView.IsLiveSorting = true;
        }
        public BaseMuiCollection(int capacity)
        {
            _list = new List<T>(capacity);
            InnerView = CollectionViewSource.GetDefaultView(_list) as ListCollectionView;
            InnerView.IsLiveSorting = true;
        }
        public BaseMuiCollection(IEnumerable<T> items)
        {
            _list = new List<T>(items);
            InnerView = CollectionViewSource.GetDefaultView(_list) as ListCollectionView;
            InnerView.IsLiveSorting = true;
        }

        #endregion

        #region BASE PROPERTIES
        public ListCollectionView View => InnerView;
        public int Count => _list.Count;
        public virtual bool IsReadOnly => ((IList<T>)_list).IsReadOnly;

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
        public void Add(T item)
        {
            _list.Add(item);
            this.OnCollectionChanged(NotifyCollectionChangedAction.Add, item);
        }
        public void AddRange(IEnumerable<T> apps, bool notify = true)
        {
            var listOfItems = apps.ToList();
            _list.AddRange(listOfItems);
            if (notify)
                this.OnCollectionChanged(NotifyCollectionChangedAction.Add, listOfItems);
        }
        public void Clear()
        {
            var oldItems = _list.ToList();
            _list.Clear();
            this.OnCollectionChanged(NotifyCollectionChangedAction.Reset, oldItems);
        }
        public bool Contains(T item) => _list.Contains(item);
        public bool Contains(Predicate<T> match) => _list.Exists(match);
        public void CopyTo(T[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);
        public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();
        public int IndexOf(T item) => _list.IndexOf(item);
        public void Insert(int index, T item)
        {
            _list.Insert(index, item);
            this.OnCollectionChanged(NotifyCollectionChangedAction.Add, item);
        }
        public virtual void Sort() => _list.Sort();
        public void Sort(IComparer<T> comparer) => _list.Sort(comparer);
        public bool Remove(T item)
        {
            var removing = item.Clone();
            bool result = _list.Remove(item);
            if (result)
                this.OnCollectionChanged(NotifyCollectionChangedAction.Remove, removing);

            return result;
        }
        public void RemoveAt(int index)
        {
            var removing = _list[index].Clone();
            _list.RemoveAt(index);
            this.OnCollectionChanged(NotifyCollectionChangedAction.Remove, removing);
        }

        #endregion

        #region INTERFACE IMPLEMENTATIONS

        #region INTERFACE-ONLY PROPERTIES
        bool IList.IsFixedSize => ((IList)_list).IsFixedSize;
        bool ICollection.IsSynchronized => ((ICollection)_list).IsSynchronized;
        object ICollection.SyncRoot => ((ICollection)_list).SyncRoot;

        #endregion

        #region GENERIC ICOLLECTION METHODS
        void ICollection<T>.Add(T item) => _list.Add(item);
        void ICollection<T>.Clear() => _list.Clear();
        bool ICollection<T>.Remove(T item) => _list.Remove(item);

        #endregion

        #region GENERIC ILIST METHODS
        void IList<T>.Insert(int index, T item) => _list.Insert(index, item);
        void IList<T>.RemoveAt(int index) => _list.RemoveAt(index);

        #endregion

        #region NON-GENERIC ICOLLECTION METHODS
        void ICollection.CopyTo(Array array, int index) => ((ICollection)_list).CopyTo(array, index);

        #endregion

        #region NON-GENERIC ILIST METHODS
        int IList.Add(object value) => ((IList)_list).Add(value);
        bool IList.Contains(object value) => ((IList)_list).Contains(value);
        int IList.IndexOf(object value) => ((IList)_list).IndexOf(value);
        void IList.Insert(int index, object value) => ((IList)_list).Insert(index, value);
        void IList.Remove(object value) => ((IList)_list).Remove(value);
        void IList.RemoveAt(int index) => ((IList)_list).RemoveAt(index);

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
            _list.Sort(sortBy);
            this.UpdateView(useLiveSorting, true);
        }
        private void UpdateView(bool useLiveSorting, bool isPrivate)
        {
            InnerView = CollectionViewSource.GetDefaultView(_list) as ListCollectionView;
            InnerView.IsLiveSorting = useLiveSorting;
        }

        #endregion
    }
}
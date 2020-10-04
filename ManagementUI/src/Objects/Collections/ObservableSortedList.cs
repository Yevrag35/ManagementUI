using ManagementUI.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Data;

namespace ManagementUI
{
    public abstract class ObservableSortedList<TKey, TValue> : IList<TValue>, IReadOnlyList<TValue>, IList, INotifyCollectionChanged
    {
        #region FIELDS/CONSTANTS
        private SortDescription? _defaultSortDescription;
        private ICollectionView _backingView;
        private Func<TValue, TKey> _keySelector;

        #endregion

        #region EVENT HANDLERS

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            this.CollectionChanged?.Invoke(this, e);
        }
        private void OnAdd(object changedItem, int index)
        {
            this.OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, changedItem, index));
        }
        private void OnRemove(object removedItem, int index)
        {
            this.OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removedItem, index));
        }
        private void OnReplace(int index)
        {
            object item = this[index];
            this.OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, item, index));
        }
        private void OnReset()
        {
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        #endregion

        #region INDEXERS
        object IList.this[int index]
        {
            get => this[index];
            set
            {
                if (value is TValue item)
                    this[index] = item;
            }
        }
        public TValue this[int index]
        {
            get => this.Items.Values[index];
            set
            {
                this.Items.RemoveAt(index);
                this.Add(value, true);
                this.OnReplace(index);
            }
        }

        #endregion

        #region PROPERTIES
        public int Count => this.Items.Count;
        public bool IsReadOnly => false;
        /// <summary>
        /// The backing <see cref="SortedList{TKey, TValue}"/> for the collection.
        /// </summary>
        protected SortedList<TKey, TValue> Items { get; }
        /// <summary>
        /// The constructed view of the <see cref="ObservableSortedList{TKey, TValue}"/>.
        /// </summary>
        public ICollectionView View => _backingView;

        #region INTERFACE PROPERTIES
        bool ICollection.IsSynchronized => false;
        object ICollection.SyncRoot => this;
        bool IList.IsFixedSize => false;

        #endregion

        #endregion

        #region CONSTRUCTORS
        public ObservableSortedList(Func<TValue, TKey> keySelector)
            : this(0, keySelector, null)
        {
        }
        public ObservableSortedList(int capacity, Func<TValue, TKey> keySelector)
            : this(capacity, keySelector, null)
        {
        }
        public ObservableSortedList(int capacity, Func<TValue, TKey> keySelector, IComparer<TKey> comparer)
        {
            this.Items = new SortedList<TKey, TValue>(capacity, comparer);
            _keySelector = keySelector;
        }
        public ObservableSortedList(int capacity, SortDescription defaultSort, Func<TValue, TKey> keySelector)
            : this(capacity, keySelector, null)
        {
            _defaultSortDescription = defaultSort;
        }
        public ObservableSortedList(int capacity, SortDescription defaultSort, Func<TValue, TKey> keySelector, IComparer<TKey> comparer)
            : this(capacity, keySelector, comparer)
        {
            _defaultSortDescription = defaultSort;
        }
        public ObservableSortedList(IEnumerable<TValue> items, Func<TValue, TKey> keySelector, IComparer<TKey> comparer)
            : this(GetCountOrDefault(items), keySelector, comparer)
        {
            // TO DO:
            //  - AddMany to Items
        }
        public ObservableSortedList(IEnumerable<TValue> items, SortDescription defaultSort, Func<TValue, TKey> keySelector)
            : this(items, defaultSort, keySelector, null)
        {
        }
        public ObservableSortedList(IEnumerable<TValue> items, SortDescription defaultSort, Func<TValue, TKey> keySelector, IComparer<TKey> comparer)
            : this(items, keySelector, comparer)
        {
            _defaultSortDescription = defaultSort;
            // TO DO:
            //foreach (TValue item in items)
            //{
            //    TKey key = _keySelector(item);
            //    if (this.Items.Count == 0 || !this.Items.ContainsKey(key))
            //    {
            //        this.Items.Add(key, item);
            //    }
            //}
        }

        #endregion

        #region LIST METHODS
        /// <exception cref="ArgumentNullException"/>
        private bool Add(TValue item, bool deferNotify)
        {
            TKey key = this.GetKey(item);
            try
            {
                this.Items.Add(key, item);
                if (!deferNotify)
                {
                    int index = this.Items.IndexOfKey(key);
                    this.OnAdd(item, index);
                }
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }
        public bool Add(TValue item)
        {
            return this.Add(item, false);
        }
        public void Clear()
        {
            this.Items.Clear();
            this.OnReset();
        }
        public bool Contains(TValue item)
        {
            return this.Items.ContainsValue(item);
        }
        public bool ContainsKey(TKey key)
        {
            return this.Items.ContainsKey(key);
        }
        public void CopyTo(TValue[] array, int index)
        {
            this.Items.Values.CopyTo(array, index);
        }
        public int IndexOf(TValue item)
        {
            return this.Items.IndexOfValue(item);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <exception cref="ArgumentNullException"/>
        public int IndexOfKey(TKey key)
        {
            return this.Items.IndexOfKey(key);
        }
        private bool Remove(int index, TValue item)
        {
            if (index > -1)
            {
                this.Items.RemoveAt(index);
                this.OnRemove(item, index);
            }
            return index > -1;
        }
        public bool Remove(TKey key)
        {
            int index = this.IndexOfKey(key);
            return this.Remove(index, this[index]);
        }
        public void RemoveAt(int index)
        {
            if (index > -1 && this.Count > 0)
                this.Remove(index, this[index]);
        }
        public bool RemoveByValue(TValue item)
        {
            return item != null
                ? this.Remove(this.IndexOf(item), item)
                : false;
        }

        #endregion

        #region ENUMERATORS
        public IEnumerator<TValue> GetEnumerator()
        {
            return this.Items.Values.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region VIEW METHODS

        private void AddDefaultSortToView(ICollectionView view)
        {
            if (_defaultSortDescription.HasValue)
            {
                view.SortDescriptions.AddOnly(_defaultSortDescription.Value);
            }
        }
        private ICollectionView CreateDefaultView()
        {
            return CollectionViewSource.GetDefaultView(this);
        }

        /// <summary>
        /// Updates the <see cref="ICollectionView"/> of the <see cref="ObservableSortedList{TKey, TValue}.View"/>.
        /// </summary>
        /// <remarks>
        ///     If a default <see cref="SortDescription"/> was defined during construction, then it will be added.
        /// </remarks>
        public void UpdateView()
        {
            _backingView = this.CreateDefaultView();
            this.AddDefaultSortToView(_backingView);
        }

        #endregion

        #region INTERFACE METHODS

        #region ILIST GENERIC METHODS
        void ICollection<TValue>.Add(TValue value)
        {
            this.Add(value);
        }
        void IList<TValue>.Insert(int index, TValue item)
        {
            this.Add(item);
        }
        bool ICollection<TValue>.Remove(TValue item)
        {
            return this.RemoveByValue(item);
        }

        #endregion

        #region ILIST NON-GENERIC METHODS
        int IList.Add(object value)
        {
            int position = -1;
            if (value is TValue item && this.Add(item, true))
            {
                position = this.IndexOf(item);
                this.OnAdd(item, position);
            }

            return position;
        }
        bool IList.Contains(object value)
        {
            if (value is TValue item)
            {
                TKey key = default;
                try
                {
                    key = this.GetKey(item);
                    return this.Items.ContainsKey(key);
                }
                catch
                {
                    return false;
                }
            }
            else
                return false;
        }
        void ICollection.CopyTo(Array array, int index)
        {
            if (array is TValue[] tArr)
                this.CopyTo(tArr, index);
        }
        int IList.IndexOf(object value)
        {
            return value is TValue item ? this.IndexOf(item) : -1;
        }
        void IList.Insert(int index, object value)
        {
            if (value is TValue item)
                this.Add(item);
        }
        void IList.Remove(object value)
        {
            if (value is TValue item)
            {
                this.RemoveByValue(item);
            }
        }

        #endregion

        #endregion

        #region OTHER PRIVATE METHODS

        private static int GetCountOrDefault(IEnumerable<TValue> items)
        {
            if (items is ICollection icol)
                return icol.Count;

            else if (items is ICollection<TValue> icol2)
                return icol2.Count;

            else
                return 0;
        }

        private TKey GetKey(TValue item)
        {
            if (item == null)
                throw new ArgumentNullException("Cannot retrieve a key from a null value.");

            return _keySelector(item);
        }

        #endregion
    }
}

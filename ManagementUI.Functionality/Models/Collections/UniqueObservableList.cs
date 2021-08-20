using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using ManagementUI.Functionality.Models.Extensions;

namespace ManagementUI.Functionality.Models
{
    /// <summary>
    /// A class that provides the same functionality as <see cref="List{T}"/>, but enforces every element to be
    /// unique according to the default or custom-defined equality comparer.
    /// </summary>
    /// <typeparam name="T">The element type in the <see cref="UniqueObservableList{T}"/>.</typeparam>
    public class UniqueObservableList<T> : UniqueListBase<T>, IList<T>, IReadOnlyList<T>, IReadOnlyCollection<T>,
        IList, INotifyCollectionChanged, INotifyPropertyChanged
    {
        #region PRIVATE FIELDS/CONSTANTS
        private const int DEFAULT_CAPACITY = 0;

        #endregion

        #region INDEXERS
        public T this[int index]
        {
            get => base.GetByIndex(index);
            set
            {
                if (base.TryIsValidIndex(index, out int positiveIndex))
                    this.ReplaceValueAtIndex(positiveIndex, value);

                else
                    throw new ArgumentOutOfRangeException();
            }
        }
        object IList.this[int index]
        {
            get => this[index];
            set
            {
                if (value is T item)
                    this[index] = item;
            }
        }

        #endregion

        #region EVENT HANDLERS
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            this.CollectionChanged?.Invoke(this, e);
        }
        private void OnAdd(object changedItem, int index)
        {
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, changedItem, index));
        }
        private void OnRemove(object removedItem, int index)
        {
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removedItem, index));
        }
        private void OnReplace(int index)
        {
            object item = this[index];
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, item, index));
        }
        private void OnReset()
        {
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        #endregion

        #region PROPERTIES



        #region INTERFACE EXPLICIT PROPERTIES


        #endregion

        #endregion

        #region CONSTRUCTORS
        /// <summary>
        /// The default constructor.  Initializes an empty list using the default 
        /// <see cref="IEqualityComparer{T}"/> for <typeparamref name="T"/>.
        /// </summary>
        public UniqueObservableList()
            : base(DEFAULT_CAPACITY)
        {
        }
        /// <summary>
        /// Initializes an empty <see cref="UniqueObservableList{T}"/> with the specified capacity using the default
        /// <see cref="IEqualityComparer{T}"/> for <typeparamref name="T"/>.
        /// </summary>
        /// <param name="capacity"></param>
        public UniqueObservableList(int capacity)
            : base(capacity)
        {
        }

        /// <summary>
        /// Initializes an empty <see cref="UniqueObservableList{T}"/> with the default capacity using the specified
        /// <see cref="IEqualityComparer{T}"/> to determine uniqueness.
        /// </summary>
        /// <param name="equalityComparer">The comparer used to define if an incoming element is unique.</param>
        public UniqueObservableList(IEqualityComparer<T> equalityComparer)
            : base(DEFAULT_CAPACITY, equalityComparer)
        {
        }

        /// <summary>
        /// Initializes an empty <see cref="UniqueObservableList{T}"/> with the specified capacity using the specified
        /// <see cref="IEqualityComparer{T}"/> to determine uniqueness.
        /// </summary>
        /// <param name="capacity">The number of new elements the list can initially store.</param>
        /// <param name="equalityComparer">The comparer used to define if an incoming element is unique.</param>
        public UniqueObservableList(int capacity, IEqualityComparer<T> equalityComparer)
            : base(capacity, equalityComparer)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="UniqueObservableList{T}"/> instance that contains elements copied from the specified
        /// collection and has sufficient capacity to accomodate the number of unique elements copied.
        /// </summary>
        /// <remarks>
        ///     <paramref name="items"/> will be enumerated for uniqueness according to the default
        ///     <see cref="IEqualityComparer{T}"/> for type <typeparamref name="T"/>.
        /// </remarks>
        /// <param name="items">
        ///     The collection whose elements will be enumerated for uniqueness and added
        ///     to the list.
        /// </param>
        public UniqueObservableList(IEnumerable<T> items)
            : base(items)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="UniqueObservableList{T}"/> instance that contains elements copied from the specified
        /// collection and has sufficient capacity to accomodate the number of unique elements copied.
        /// </summary>
        /// <remarks>
        ///     <paramref name="collection"/> will be enumerated for uniqueness according to the provided 
        ///     <see cref="IEqualityComparer{T}"/>.
        /// </remarks>
        /// <param name="collection">
        ///     The collection whose elements will be enumerated for uniqueness and added
        ///     to the list.
        /// </param>
        /// <param name="equalityComparer">
        ///     The equality comparer that determines whether an element is unique.
        /// </param>
        public UniqueObservableList(IEnumerable<T> collection, IEqualityComparer<T> equalityComparer)
            : base(collection, equalityComparer)
        {
        }

        #endregion

        #region LIST METHODS

        public bool Exists(Predicate<T> predicate)
        {
            return InnerList.Exists(predicate);
        }
        public List<T> FindAll(Predicate<T> predicate)
        {
            return InnerList.FindAll(predicate);
        }
        public void ForEach(Action<T> action)
        {
            InnerList.ForEach(action);
        }
        public override void Insert(int index, T item)
        {
            if (InnerSet.Add(item))
            {
                try
                {
                    InnerList.Insert(index, item);
                    this.OnAdd(item, index);
                }
                catch (ArgumentOutOfRangeException)
                {
                    InnerSet.Remove(item);
                }
            }
        }
        public bool IsSubsetOf(IEnumerable<T> other)
        {
            return InnerSet.IsSubsetOf(other);
        }
        public bool IsSupersetOf(IEnumerable<T> other)
        {
            return InnerSet.IsSupersetOf(other);
        }
        public bool Overlaps(IEnumerable<T> other)
        {
            return InnerSet.Overlaps(other);
        }
        public bool SetEquals(IEnumerable<T> other)
        {
            return InnerSet.SetEquals(other);
        }
        /// <summary>
        /// Copies the elements of the <see cref="UniqueObservableList{T}"/> to a new array.
        /// </summary>
        /// <returns>
        ///     An array containing copies of the elements of the <see cref="UniqueObservableList{T}"/>.  If the list contains no elements, 
        ///     an empty array is returned.
        /// </returns>
        public T[] ToArray()
        {
            return InnerList.ToArray();
        }

        #region SET METHODS
        //public int ExceptWith(IEnumerable<T> other)
        //{
        //    InnerSet.ExceptWith(other);
        //    return InnerList.RemoveAll(x => other.Contains(x, InnerSet.Comparer));
        //}
        //void ISet<T>.ExceptWith(IEnumerable<T> other) => this.ExceptWith(other);
        //public bool IsProperSubsetOf(IEnumerable<T> other)
        //{
        //    return InnerSet.IsProperSubsetOf(other);
        //}
        //public bool IsProperSupersetOf(IEnumerable<T> other)
        //{
        //    return InnerSet.IsProperSupersetOf(other);
        //}
        //public bool IsSubsetOf(IEnumerable<T> other)
        //{
        //    return InnerSet.IsSubsetOf(other);
        //}
        //public bool IsSupersetOf(IEnumerable<T> other)
        //{
        //    return InnerSet.IsSupersetOf(other);
        //}
        //public bool Overlaps(IEnumerable<T> other)
        //{
        //    return InnerSet.Overlaps(other);
        //}

        #endregion

        #region INTERFACE EXPLICIT METHODS
        int IList.Add(object value)
        {
            return value is T item && this.Add(item, true)
                ? this.IndexOf(item)
                : -1;
        }
        bool IList.Contains(object value)
        {
            return value is T item && this.Contains(item);
        }
        int IList.IndexOf(object value)
        {
            return value is T item
                ? this.IndexOf(item)
                : -1;
        }
        void IList.Insert(int index, object value)
        {
            if (value is T item)
                this.Insert(index, item);
        }
        void IList.Remove(object value)
        {
            if (value is T item)
                this.Remove(item);
        }

        #endregion

        #region PRIVATE METHODS
        protected override bool Add(T item, bool adding)
        {
            bool added = base.Add(item, true);
            if (added)
            {
                this.OnAdd(item, this.IndexOf(item));
            }

            return added;
        }
        protected override void Clear(bool clearing)
        {
            base.Clear(true);
            this.OnReset();
        }
        protected override bool Remove(T item, bool removing)
        {
            int index = this.IndexOf(item);
            bool result = base.Remove(item, true);
            if (result)
                this.OnRemove(item, index);

            return result;
        }
        protected override bool ReplaceValueAtIndex(int index, T newValue)
        {
            bool result = base.ReplaceValueAtIndex(index, newValue);
            if (result)
                this.OnReplace(index);

            return result;
        }

        protected virtual void NotifyChange(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #endregion
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ManagementUI.Functionality.Models.Extensions.List;

namespace ManagementUI.Functionality.Models
{
    /// <summary>
    /// Provides the <see langword="abstract"/> base class to enforce uniqueness in generic collections.
    /// </summary>
    public abstract class UniqueListBase<T> : ICollection<T>, ICollection, IList
    {
        #region FIELDS/CONSTANTS
        /// <summary>
        /// The internal, backing <see cref="List{T}"/> collection that all methods invoke against.
        /// </summary>
        protected List<T> InnerList;
        /// <summary>
        /// The internal, backing <see cref="HashSet{T}"/> set that determines uniqueness in the <see cref="UniqueListBase{T}"/>.
        /// </summary>
        protected HashSet<T> InnerSet;

        #endregion

        #region PROPERTIES
        /// <summary>
        /// The equality comparer used to determine uniqueness in the list./>.
        /// </summary>
        public IEqualityComparer<T> Comparer => InnerSet.Comparer;

        /// <summary>
        /// Get the number of elements contained within the <see cref="UniqueListBase{T}"/>.
        /// </summary>
        public int Count => InnerList.Count;

        #endregion

        #region CONSTRUCTORS
        /// <summary>
        /// Initializes a new instance of the <see cref="UniqueListBase{T}"/> class that is empty
        /// and has the default initial capacity.
        /// </summary>
        public UniqueListBase()
            : this(0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UniqueListBase{T}"/> class that is empty
        /// and has the specified initial capacity.
        /// </summary>
        /// <param name="capacity">The number of elements that the new collection can initially store.</param>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public UniqueListBase(int capacity)
            : this(capacity, GetDefaultComparer())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UniqueListBase{T}"/> class that
        /// contains elements copied from the specified <see cref="IEnumerable{T}"/> and has
        /// sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new list.</param>
        /// <exception cref="ArgumentNullException"/>
        public UniqueListBase(IEnumerable<T> collection)
            : this(collection, GetDefaultComparer())
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="equalityComparer">
        ///     The <see cref="IEqualityComparer{T}"/> implementation to use when comparing values in the list, or
        ///     <see langword="null"/> to use the default <see cref="EqualityComparer{T}"/> implementation for the
        ///     type <typeparamref name="T"/>.
        /// </param>
        public UniqueListBase(IEqualityComparer<T> equalityComparer)
            : this(0, equalityComparer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UniqueListBase{T}"/> class that is empty, has the specified
        /// initial capacity, and uses the specified equality comparer for the <typeparamref name="T"/> type.
        /// </summary>
        /// <param name="capacity">The number of elements that the new collection can initially store.</param>
        /// <param name="equalityComparer">
        ///     The <see cref="IEqualityComparer{T}"/> implementation to use when comparing values in the list, or
        ///     <see langword="null"/> to use the default <see cref="EqualityComparer{T}"/> implementation for the
        ///     type <typeparamref name="T"/>.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than 0.</exception>
        public UniqueListBase(int capacity, IEqualityComparer<T> equalityComparer)
        {
            InnerList = new List<T>(capacity);
            InnerSet = new HashSet<T>(equalityComparer);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UniqueListBase{T}"/> class that uses the specified comparer for 
        /// the <typeparamref name="T"/> type, contains elements copied from the specified collection, and sufficient capacity
        /// to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new list.</param>
        /// <param name="equalityComparer">
        ///     The <see cref="IEqualityComparer{T}"/> implementation to use when comparing values in the list, or
        ///     <see langword="null"/> to use the default <see cref="EqualityComparer{T}"/> implementation for the
        ///     type <typeparamref name="T"/>.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is <see langword="null"/>.</exception>
        public UniqueListBase(IEnumerable<T> collection, IEqualityComparer<T> equalityComparer)
        {
            InnerSet = new HashSet<T>(collection, equalityComparer);
            InnerList = new List<T>(InnerSet);
        }

        #endregion

        #region BASE METHODS
        /// <summary>
        /// Adds an item to the end of the collection.
        /// </summary>
        /// <param name="item">The object to be added to the end of the collection.</param>
        public void Add(T item)
        {
            _ = this.Add(item, true);
        }
        /// <summary>
        /// Removes all elements from the <see cref="UniqueListBase{T}"/>.
        /// </summary>
        public void Clear()
        {
            this.Clear(true);
        }
        /// <summary>
        /// Determines whether an element is in the <see cref="UniqueListBase{T}"/>.
        /// </summary>
        /// <param name="item">
        /// The object to locate in the <see cref="UniqueListBase{T}"/>.  The value can be null for reference types.
        /// </param>
        public bool Contains(T item) => InnerSet.Contains(item);
        /// <summary>
        /// Copies the entire <see cref="UniqueListBase{T}"/> to a compatible one-dimensional array, starting at
        /// the specified index of the target array.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional array that is the destination of the elements copied from
        /// <see cref="UniqueListBase{T}"/>.  The array must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">The zero-based index in the target array at which copying begins.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentOutOfRangeException"/>
        /// <exception cref="ArgumentException"/>
        public void CopyTo(T[] array, int arrayIndex) => InnerList.CopyTo(array, arrayIndex);
        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurrence
        /// within the entire <see cref="UniqueListBase{T}"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="UniqueListBase{T}"/>.  The value can be null for reference types.</param>
        public int IndexOf(T item) => InnerList.IndexOf(item);

        public virtual void Insert(int index, T item)
        {
            if (InnerSet.Add(item))
            {
                try
                {
                    InnerList.Insert(index, item);
                }
                catch (ArgumentOutOfRangeException)
                {
                    InnerSet.Remove(item);
                }
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="UniqueListBase{T}"/>.  The
        /// value can be null for reference types.
        /// </summary>
        /// <param name="item">
        /// The object to remove from the <see cref="UniqueListBase{T}"/>.
        /// The value can be null for reference types.
        /// </param>
        public bool Remove(T item)
        {
            return this.Remove(item, true);
        }

        public void RemoveAt(int index)
        {
            T item = default;
            try
            {
                item = InnerList[index];
            }
            catch (ArgumentOutOfRangeException)
            {
                return;
            }

            _ = this.Remove(item, true);
        }

        #endregion

        #region INTERFACE EXPLICIT MEMBERS
        object IList.this[int index]
        {
            get => InnerList[index];
            set
            {
                if (value is T item)
                    InnerList[index] = item;
            }
        }

        bool IList.IsFixedSize => false;
        bool IList.IsReadOnly => false;

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

        #region ENUMERATOR
        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="UniqueListBase{T}"/>.
        /// </summary>
        public IEnumerator<T> GetEnumerator() => InnerList.GetEnumerator();
        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="IEnumerable"/>.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator() => InnerList.GetEnumerator();

        #endregion

        #region INTERFACE MEMBERS

        #region IMPLEMENTED INTERFACE PROPERTIES
        bool ICollection<T>.IsReadOnly => ((ICollection<T>)InnerList).IsReadOnly;
        bool ICollection.IsSynchronized => ((ICollection)InnerList).IsSynchronized;
        object ICollection.SyncRoot => ((ICollection)InnerList).SyncRoot;

        #endregion

        #region IMPLEMENTED INTERFACE METHODS
        void ICollection<T>.Add(T item) => this.Add(item);
        void ICollection.CopyTo(Array array, int index) => ((ICollection)InnerList).CopyTo(array, index);

        #endregion

        #endregion

        #region BACKEND/PRIVATE METHODS
        protected virtual bool Add(T item, bool adding)
        {
            if (adding)
            {
                bool result = false;
                if (InnerSet.Add(item))
                {
                    InnerList.Add(item);
                    result = true;
                }

                return result;
            }
            else
                return adding;
        }

        protected virtual void Clear(bool clearing)
        {
            if (clearing)
            {
                InnerList.Clear();
                InnerSet.Clear();
            }
        }

        protected virtual bool Remove(T item, bool removing)
        {
            return InnerSet.Remove(item)
                ? InnerList.Remove(item)
                : false;
        }

        /// <summary>
        /// Transforms and verifies the specified negative or positive index into a proper <see cref="int"/> value
        /// returning the element of type <typeparamref name="T"/> at the proper index location.
        /// </summary>
        /// <remarks>
        ///     Used for transforming negative index <see cref="int"/> values into postive index positions.  When
        ///     negative indicies are specified, instead of starting the zero-based position, it will begin at the 
        ///     index of the last element of the <see cref="UniqueListBase{T}"/> and count backwards.
        ///     
        ///     Can be overridden for different behavior.
        /// </remarks>
        /// <param name="index">The negative or positive index value.</param>
        /// <returns>
        ///     The element of type <typeparamref name="TItem"/> at the specified proper index position; otherwise, 
        ///     if the index is determined to be out-of-range, then the default value of <typeparamref name="T"/>.
        /// </returns>
        protected virtual T GetByIndex(int index)
        {
            return this.InnerList.GetByIndex(index);
        }

        protected virtual bool ReplaceValueAtIndex(int index, T newValue)
        {
            bool result = false;
            T item = InnerList[index];
            if (InnerSet.Add(newValue))
            {
                result = InnerSet.Remove(item);
                InnerList[index] = newValue;
            }

            return result;
        }

        protected bool TryIsValidIndex(int index, out int positiveIndex)
        {
            return this.InnerList.TryIsValidIndex(index, out positiveIndex);
        }

        private static IEqualityComparer<T> GetDefaultComparer()
        {
            if (typeof(T).Equals(typeof(string)))
                return (IEqualityComparer<T>)StringComparer.CurrentCultureIgnoreCase;

            else
                return EqualityComparer<T>.Default;
        }

        #endregion

    }
}
using System;
using System.Collections;
using System.Collections.Generic;

namespace ManagementUI
{
    /// <summary>
    /// Exposes the Count property on the implementing class.
    /// </summary>
    public interface IHasCount
    {
        /// <summary>
        /// Get the number of elements contained within the <see cref="IHasCount"/> object.
        /// </summary>
        int Count { get; }
    }
    /// <summary>
    /// Exposes the zero-based indexing and Count functionality for a class implementing <see cref="IEnumerable"/>.
    /// </summary>
    public interface IIndex : IEnumerable, IHasCount
    {
        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-bsaed index of the element to get.</param>
        object this[int index] { get; }
    }
    /// <summary>
    /// Exposes the generic, zero-based indexing and Count functionality for a class implementing <see cref="IEnumerable{T}"/>.
    /// </summary>
    public interface IIndex<T> : IEnumerable<T>, IHasCount
    {
        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-bsaed index of the element to get.</param>
        T this[int index] { get; }
    }
    public interface ISorter : IEnumerable
    {
        void Sort();
    }
    public interface ISorter<T> : IEnumerable<T>, ISorter
    {
        void Sort(IComparer<T> comparer);
    }
}

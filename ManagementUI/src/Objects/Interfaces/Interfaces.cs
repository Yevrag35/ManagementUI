using System;
using System.Collections;
using System.Collections.Generic;

namespace ManagementUI
{
    public interface IHasCount
    {
        int Count { get; }
    }
    public interface IIndex : IEnumerable, IHasCount
    {
        object this[int index] { get; }
    }
    public interface IIndex<T> : IEnumerable<T>, IHasCount
    {
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

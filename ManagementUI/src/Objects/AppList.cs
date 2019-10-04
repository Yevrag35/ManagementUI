using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ManagementUI
{
    public class AppList : IList<AppListItem>, IList, INotifyCollectionChanged
    {
        #region PRIVATE FIELDS/CONSTANTS
        private List<AppListItem> _list;
        private ListCollectionView _view;

        #endregion

        #region INDEXERS
        public AppListItem this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }
        object IList.this[int index]
        {
            get => this[index];
            set => this[index] = value as AppListItem;
        }

        #endregion

        #region CONSTRUCTORS
        public AppList()
        {
            _list = new List<AppListItem>();
            _view = CollectionViewSource.GetDefaultView(_list) as ListCollectionView;
            _view.IsLiveSorting = true;
        }
        public AppList(int capacity)
        {
            _list = new List<AppListItem>(capacity);
            _view = CollectionViewSource.GetDefaultView(_list) as ListCollectionView;
            _view.IsLiveSorting = true;
        }
        public AppList(IEnumerable<AppListItem> items)
        {
            _list = new List<AppListItem>(items);
            _view = CollectionViewSource.GetDefaultView(_list) as ListCollectionView;
            _view.IsLiveSorting = true;
        }

        #endregion

        #region PROPERTIES
        public ListCollectionView AppView => _view;
        public int Count => _list.Count;
        bool IList.IsFixedSize => ((IList)_list).IsFixedSize;
        public bool IsReadOnly => ((IList<AppListItem>)_list).IsReadOnly;
        bool ICollection.IsSynchronized => ((ICollection)_list).IsSynchronized;
        object ICollection.SyncRoot => ((ICollection)_list).SyncRoot;

        #endregion

        #region EVENT HANDLERS
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (this.CollectionChanged != null)
            {
                this.CollectionChanged(this, e);
            }
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action, AppListItem item)
        {
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item));
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action, AppListItem item, int index)
        {
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index));
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action, IList items)
        {
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, items));
        }

        #endregion

        #region APPLIST METHODS
        public void Add(AppListItem item)
        {
            _list.Add(item);
            this.OnCollectionChanged(NotifyCollectionChangedAction.Add, item);
        }
        public void AddRange(IEnumerable<AppListItem> apps, bool notify = true)
        {
            var listOfApps = apps.ToList();
            _list.AddRange(listOfApps);
            if (notify)
                this.OnCollectionChanged(NotifyCollectionChangedAction.Add, listOfApps);
        }
        public void Clear()
        {
            var oldItems = _list.ToList();
            _list.Clear();
            this.OnCollectionChanged(NotifyCollectionChangedAction.Reset, oldItems);
        }
        public bool Contains(AppListItem item) => _list.Contains(item);
        public bool Contains(Predicate<AppListItem> match) => _list.Exists(match);
        public void CopyTo(AppListItem[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);
        public IEnumerator<AppListItem> GetEnumerator() => _list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();
        public int IndexOf(AppListItem item) => _list.IndexOf(item);
        public void Insert(int index, AppListItem item)
        {
            _list.Insert(index, item);
            this.OnCollectionChanged(NotifyCollectionChangedAction.Add, item);
        }
        public void Sort() => _list.Sort(new AppListItemComparer());
        public bool Remove(AppListItem item)
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
        public void UpdateView()
        {
            this.Sort();
            _view = CollectionViewSource.GetDefaultView(_list) as ListCollectionView;
            _view.IsLiveSorting = true;
        }

        #endregion

        #region GENERIC ICOLLECTION METHODS
        void ICollection<AppListItem>.Add(AppListItem item) => _list.Add(item);
        void ICollection<AppListItem>.Clear() => _list.Clear();
        bool ICollection<AppListItem>.Remove(AppListItem item) => _list.Remove(item);

        #endregion

        #region GENERIC ILIST METHODS
        void IList<AppListItem>.Insert(int index, AppListItem item) => _list.Insert(index, item);
        void IList<AppListItem>.RemoveAt(int index) => _list.RemoveAt(index);

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

        #region COMPARERS
        private class AppListItemComparer : IComparer<AppListItem>
        {
            public int Compare(AppListItem x, AppListItem y) => x.AppName.CompareTo(y.AppName);
        }

        #endregion
    }
}

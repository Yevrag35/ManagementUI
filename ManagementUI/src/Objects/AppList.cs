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
    public class AppList : IList<AppListItem>, INotifyCollectionChanged
    {
        private List<AppListItem> _list;
        private ListCollectionView _view;

        public AppListItem this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }

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

        public int Count => _list.Count;
        public bool IsReadOnly => false;
        public ListCollectionView AppView => _view;

        #region EVENT HANDLERS
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void UpdateListView()
        {
            _list.Sort(new AppListItemComparer());
            _view = CollectionViewSource.GetDefaultView(_list) as ListCollectionView;
            _view.IsLiveSorting = true;
        }

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

        public void Add(AppListItem item) => _list.Add(item);
        public void Clear() => _list.Clear();
        public bool Contains(AppListItem item) => _list.Contains(item);
        public void CopyTo(AppListItem[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);
        public IEnumerator<AppListItem> GetEnumerator() => _list.GetEnumerator();
        public int IndexOf(AppListItem item) => _list.IndexOf(item);
        public void Insert(int index, AppListItem item) => _list.Insert(index, item);
        public bool Remove(AppListItem item) => _list.Remove(item);
        public void RemoveAt(int index) => _list.RemoveAt(index);
        IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();

        private class AppListItemComparer : IComparer<AppListItem>
        {
            public int Compare(AppListItem x, AppListItem y) => x.AppName.CompareTo(y.AppName);
        }
    }
}

﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using ManagementUI.Functionality.Models;
using ManagementUI.Models;

namespace ManagementUI.Collections
{
    public class EditTagCollection : IList, IEnumerable<ToggleTag>
    {
        private ListCollectionView _availableView;
        private ListCollectionView _appliedView;
        private UniqueObservableList<ToggleTag> _available;
        private UniqueObservableList<ToggleTag> _applied;

        public ICollectionView Available => _availableView;
        public ICollectionView Applied => _appliedView;
        public int Count => _available.Count;
        public bool IsReadOnly => false;
        public bool IsFixedSize => false;
        public bool IsSynchronized => false;
        public object SyncRoot => this;

        public object this[int index]
        {
            get => _available[index];
            set
            {
                if (value is ToggleTag tag)
                {
                    _available[index] = tag;
                    _applied[index] = tag;
                }
            }
        }

        public EditTagCollection(IEnumerable<ToggleTag> tags)
        {
            _available = new UniqueObservableList<ToggleTag>(tags);
            _applied = new UniqueObservableList<ToggleTag>(_available);
            this.CreateViews();
        }

        public int Add(object value)
        {
            int index = -1;
            if (value is ToggleTag tag)
            {
                _applied.Add(tag);
                _available.Add(tag);

                index = _available.IndexOf(tag);
            }

            return index;
        }
        public void Clear()
        {
            _available.Clear();
            _applied.Clear();
        }
        public bool Contains(object value)
        {
            return value is ToggleTag tag && _applied.Contains(tag) && _available.Contains(tag);
        }
        public void CopyTo(Array array, int arrayIndex)
        {
            if (array is ToggleTag[] tArr)
            {
                _available.CopyTo(tArr, arrayIndex);
            }
        }
        public void ForEach(Action<ToggleTag> action)
        {
            _available.ForEach(action);
        }
        public int IndexOf(object value)
        {
            int index = -1;
            if (value is ToggleTag tag)
            {
                index = _available.IndexOf(tag);
            }

            return index;
        }
        public void Insert(int index, object value)
        {
            if (value is ToggleTag tag)
            {
                _available.Insert(index, tag);
                _applied.Insert(index, tag);
            }
        }
        public void Remove(object value)
        {
            if (value is ToggleTag tag)
            {
                _applied.Remove(tag);
                _available.Remove(tag);
            }
        }
        public void RemoveAt(int index)
        {
            _applied.RemoveAt(index);
            _available.RemoveAt(index);
        }

        public IEnumerator<ToggleTag> GetEnumerator()
        {
            return _available.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private void CreateViews()
        {
            _appliedView = CollectionViewSource.GetDefaultView(_applied) as ListCollectionView;
            _availableView = CollectionViewSource.GetDefaultView(_available) as ListCollectionView;

            _appliedView.IsLiveFiltering = true;
            _availableView.IsLiveFiltering = true;

            _appliedView.IsLiveSorting = true;
            _availableView.IsLiveSorting = true;

            _appliedView.LiveFilteringProperties.Add(nameof(ToggleTag.IsChecked));
            _availableView.LiveFilteringProperties.Add(nameof(ToggleTag.IsChecked));

            _appliedView.LiveSortingProperties.Add(nameof(ToggleTag.Value));
            _availableView.LiveSortingProperties.Add(nameof(ToggleTag.Value));

            _appliedView.Filter = x =>
                x is ToggleTag tt && tt.IsChecked;

            _availableView.Filter = x =>
                x is ToggleTag tt && !tt.IsChecked;
        }
    }
}

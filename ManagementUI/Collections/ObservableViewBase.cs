using System;
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
    public abstract class ObservableViewBase<T> : UniqueObservableList<T>
    {
        private ListCollectionView _backingView;

        protected virtual bool IsLiveFiltering => true;
        protected virtual bool IsLiveSorting => true;
        protected virtual string[] LiveFilteringProperties { get; } = new string[0];
        protected virtual string[] LiveSortingProperties { get; } = new string[0];
        protected virtual Predicate<object> Filter { get; }
        public ICollectionView View
        {
            get => _backingView;
        }

        protected ObservableViewBase()
            : base()
        {
        }
        public ObservableViewBase(IEnumerable<T> items)
            : base(items)
        {
        }
        public ObservableViewBase(IEnumerable<T> items, IEqualityComparer<T> equalityComparer)
            : base(items, equalityComparer)
        {
        }

        public void CreateView()
        {
            _backingView = CollectionViewSource.GetDefaultView(this) as ListCollectionView;
            _backingView.IsLiveFiltering = this.IsLiveFiltering;
            _backingView.IsLiveSorting = this.IsLiveSorting;

            for (int f = 0; f < this.LiveFilteringProperties.Length; f++)
            {
                _backingView.LiveFilteringProperties.Add(this.LiveFilteringProperties[f]);
            }

            for (int s = 0; s < this.LiveSortingProperties.Length; s++)
            {
                _backingView.LiveSortingProperties.Add(this.LiveSortingProperties[s]);
            }

            _backingView.Filter = this.Filter;
        }

        protected static Predicate<T> Negate(Predicate<T> predicate)
        {
            return x => !predicate(x);
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using ManagementUI.Functionality.Models;

namespace ManagementUI.Models.Collections
{
    public class AppsList : UniqueObservableList<AppItem>
    {
        private ListCollectionView _backingView;

        public bool IsFiltered { get; private set; }
        public ICollectionView View
        {
            get => _backingView;
            set
            {
                if (value is ListCollectionView lcv)
                {
                    _backingView = lcv;
                    this.NotifyChange(nameof(View));
                }
            }
        }

        public AppsList(IEnumerable<AppItem> apps)
            : base(apps)
        {
        }

        public void CreateView()
        {
            _backingView = CollectionViewSource.GetDefaultView(this) as ListCollectionView;
            _backingView.IsLiveFiltering = true;
            _backingView.IsLiveSorting = true;
            _backingView.LiveSortingProperties.Add(nameof(AppItem.Name));
            _backingView.LiveFilteringProperties.Add(nameof(AppItem.DontShow));

            _backingView.Filter = x =>
                x is AppItem ai && !ai.DontShow;
        }
        public void EnableByTags(IEnumerable<string> tagsToEnable)
        {
            this.EnableItems(ai => ai.Tags.IsSupersetOf(tagsToEnable));
        }
        public void EnableItems(Predicate<AppItem> predicate)
        {
            if (this.IsFiltered)
            {
                this.ResetItems();
            }

            this
                .FindAll(Negate(predicate))
                    .ForEach((ai) =>
                    {
                        ai.DontShow = true;
                    });

            this.IsFiltered = true;
        }
        public void ResetItems()
        {
            this.ForEach((ai) =>
            {
                ai.DontShow = false;
            });
            this.IsFiltered = false;
        }

        internal static Predicate<AppItem> Negate(Predicate<AppItem> predicate)
        {
            return x => !predicate(x);
        }
    }
}

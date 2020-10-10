using ManagementUI.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ManagementUI.Editing
{
    public class EditTagList : ObservableCollection<EditTagItem>
    {
        private ListCollectionView _available;
        private ListCollectionView _applied;

        public ICollectionView Available => _available;
        public ICollectionView Applied => _applied;

        public EditTagList(IEnumerable<FilterTag> filterTags, AppIconSetting ais)
            : this(ConvertAll(filterTags, ais))
        {
        }
        public EditTagList(IEnumerable<EditTagItem> items)
            : base(items)
        {
            string sortProperty = SortDescriptionCollectionExtensions.GetMemberNameFromExpression<EditTagItem, string>(x => x.Title);
            string filterProperty = SortDescriptionCollectionExtensions.GetMemberNameFromExpression<EditTagItem, EditingStatus>(x => x.Status);
            this.CreateAvailableView(sortProperty, filterProperty);
            this.CreateAppliedView(sortProperty, filterProperty);
        }

        private void CreateAvailableView(string propertyName, string filterProperty)
        {
            _available = new CollectionViewSource
            {
                Source = this
            }.View as ListCollectionView;
            _available.SortDescriptions.Add(new SortDescription(propertyName, ListSortDirection.Descending));
            _available.LiveFilteringProperties.Add(filterProperty);
            _available.LiveSortingProperties.Add(propertyName);
            _available.IsLiveSorting = true;
            _available.IsLiveFiltering = true;
            
            _available.Filter = tag =>
                tag is EditTagItem eti && eti.Status == EditingStatus.Available;
        }
        private void CreateAppliedView(string propertyName, string filterProperty)
        {
            _applied = new CollectionViewSource
            {
                Source = this
            }.View as ListCollectionView;
            _applied.SortDescriptions.Add(new SortDescription(propertyName, ListSortDirection.Descending));
            _applied.LiveFilteringProperties.Add(filterProperty);
            _applied.LiveSortingProperties.Add(propertyName);
            _applied.IsLiveFiltering = true;
            _applied.IsLiveSorting = true;

            _applied.Filter = tag =>
                tag is EditTagItem eti && eti.Status == EditingStatus.Applied;
        }

        private static IEnumerable<EditTagItem> ConvertAll(IEnumerable<FilterTag> filterTags, AppIconSetting ais)
        {
            foreach (FilterTag ft in filterTags)
            {
                yield return EditTagItem.FromFilterTag(ft, ais);
            }
        }
    }
}

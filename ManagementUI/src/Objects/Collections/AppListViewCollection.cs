using ManagementUI.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Data;

namespace ManagementUI.src.Objects.Collections
{
    public class AppListViewCollection : BaseViewCollection<AppIconSetting>, ICloneable
    {
        private List<FilterTag> _allTags;
        //private IgnoreCaseEquality _ignoreCase;
        //public IEnumerable<FilterTag> Tags => base.Items.Where(x => x.Tags != null).SelectMany(x => x.Tags).Distinct();
        public IReadOnlyList<FilterTag> Tags => _allTags;
        public ListCollectionView TagView { get; }

        //internal AppListViewCollection()
        //    : base(ListSortDirection.Ascending, x => x.Name)
        //{
        //    //_ignoreCase = new IgnoreCaseEquality();
        //    this.TagView = CollectionViewSource.GetDefaultView(this) as ListCollectionView;
        //    this.TagView?.SortDescriptions.Add<FilterTag, string>(ListSortDirection.Ascending, x => x.Tag);
        //}
        [JsonConstructor]
        internal AppListViewCollection(IEnumerable<AppIconSetting> items)
            : base(items, ListSortDirection.Ascending, x => x.Name)
        {
            _allTags = new List<FilterTag>(base.Items.Where(x => x.Tags != null).SelectMany(x => x.Tags).Distinct());
            this.TagView = CollectionViewSource.GetDefaultView(_allTags) as ListCollectionView;
            this.TagView.SortDescriptions.Add<FilterTag, string>(ListSortDirection.Ascending, x => x.Tag);
        }

        internal AppListViewCollection Clone()
        {
            var col = new AppListViewCollection(this);
            foreach (SortDescription sd in this.View.SortDescriptions)
            {
                col.View.SortDescriptions.Add(sd);
            }
            return col;
        }
        object ICloneable.Clone() => this.Clone();
    }
}

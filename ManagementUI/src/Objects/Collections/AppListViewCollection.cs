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
        //private IgnoreCaseEquality _ignoreCase;
        //public IEnumerable<FilterTag> Tags => base.Items.Where(x => x.Tags != null).SelectMany(x => x.Tags).Distinct();
        public HashSet<FilterTag> Tags { get; }
        public CollectionView TagView { get; }

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
            this.Tags = new HashSet<FilterTag>(this.GetAllTags(), new FilterTagEquality());
            this.TagView = CollectionViewSource.GetDefaultView(this.Tags) as CollectionView;
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

        internal IEnumerable<FilterTag> GetAllTags()
        {
            return base.Items.Where(x => x.Tags != null).SelectMany(x => x.Tags);
        }
        internal void RegenerateTagView()
        {
            this.Tags.Clear();
            this.Tags.UnionWith(this.GetAllTags());
        }

        internal void RemoveAll(Func<AppIconSetting, bool> predicate)
        {
            for (int i = base.Items.Count - 1; i >= 0; i--)
            {
                AppIconSetting ais = base.Items[i];
                if (predicate(ais))
                    base.Items.Remove(ais);
            }
        }
    }
}

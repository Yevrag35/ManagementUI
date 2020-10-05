using ManagementUI.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Data;

namespace ManagementUI
{
    public class AppListViewCollection : BaseViewCollection<AppIconSetting>, ICloneable
    {
        //private IgnoreCaseEquality _ignoreCase;
        //public IEnumerable<FilterTag> Tags => base.Items.Where(x => x.Tags != null).SelectMany(x => x.Tags).Distinct();
        public TagList Tags { get; }
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
            //this.Tags = new TagSet(this.GetAllTags(), new FilterTagEquality());
            this.Tags = new TagList(this.GetAllTags());
            this.View.IsLiveFiltering = true;
            this.View.LiveFilteringProperties.Add("IsChecked");
            this.View.Filter = app =>
                app is AppIconSetting ais && ais.IsChecked;
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

        public override void RefreshAll()
        {
            base.RefreshAll();
            this.Tags.Update(this.GetAllTags());
        }

        internal IEnumerable<FilterTag> GetAllTags()
        {
            return base.Items.Where(x => x.Tags != null).SelectMany(x => x.Tags);
        }
        internal void RegenerateTagView()
        {
            this.Tags.Reset(this.GetAllTags());
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

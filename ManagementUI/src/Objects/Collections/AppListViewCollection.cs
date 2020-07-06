using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace ManagementUI.src.Objects.Collections
{
    public class AppListViewCollection : BaseViewCollection<AppIconSetting>, ICloneable
    {
        private IgnoreCaseEquality _ignoreCase;
        public IEnumerable<string> Tags => base.Items.Where(x => x.Tags != null).SelectMany(x => x.Tags).Distinct(_ignoreCase);

        internal AppListViewCollection()
            : base(ListSortDirection.Ascending, x => x.Name)
        {
            _ignoreCase = new IgnoreCaseEquality();
        }
        [JsonConstructor]
        internal AppListViewCollection(IEnumerable<AppIconSetting> items)
            : base(items, ListSortDirection.Ascending, x => x.Name)
        {
            _ignoreCase = new IgnoreCaseEquality();
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

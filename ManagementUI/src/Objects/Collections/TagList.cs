using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ManagementUI
{
    public class TagList : ObservableSortedList<string, FilterTag>
    {
        public TagList()
            : base(0, GetDefaultSortDescription(), x => x.Tag)
        {
        }

        public void AddMany(IEnumerable<FilterTag> tags)
        {
            foreach (FilterTag ft in tags)
            {
                this.Add(ft, true);
            }
            base.OnReset();
        }

        private static SortDescription GetDefaultSortDescription()
        {
            return new SortDescription("Tag", ListSortDirection.Descending);
        }
    }
}

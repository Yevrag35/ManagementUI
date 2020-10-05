using Newtonsoft.Json;
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
            : base(0, GetDefaultSortDescription(), x => x.Tag, StringComparer.CurrentCultureIgnoreCase)
        {
        }

        [JsonConstructor]
        public TagList(IEnumerable<FilterTag> tags)
            : base(tags, GetDefaultSortDescription(), x => x.Tag, StringComparer.CurrentCultureIgnoreCase)
        {
            foreach (FilterTag ft in tags)
            {
                this.Add(ft, true);
            }
        }

        public void AddMany(IEnumerable<FilterTag> tags)
        {
            foreach (FilterTag ft in tags)
            {
                this.Add(ft, true);
            }
            base.OnReset();
        }
        public IEnumerable<FilterTag> GetChecked() => base.Items.Values.Where(x => x.IsChecked);
        public void Reset(IEnumerable<FilterTag> with)
        {
            base.Update(with);
        }

        private static SortDescription GetDefaultSortDescription()
        {
            return new SortDescription("Tag", ListSortDirection.Ascending);
        }
    }
}

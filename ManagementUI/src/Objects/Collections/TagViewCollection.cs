using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace ManagementUI
{
    public class TagViewCollection : BaseViewCollection<FilterTag>
    {
        public TagViewCollection(IEnumerable<string> tags)
            : base(tags.Select(x => new FilterTag(x, false)), ListSortDirection.Ascending, x => x.Tag)
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementUI
{
    public class TagSet : SortedSet<FilterTag>
    {
        public TagSet()
            : base()
        {
        }
        public TagSet(IEnumerable<FilterTag> tags)
            : base(tags)
        {
        }


    }
}

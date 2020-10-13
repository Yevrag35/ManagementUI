using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementUI
{
    public class FilterTagEquality : IEqualityComparer<FilterTag>
    {
        public bool Equals(FilterTag x, FilterTag y)
        {
            if (string.IsNullOrWhiteSpace(x.Tag) && string.IsNullOrWhiteSpace(y.Tag))
                return true;

            else if (!string.IsNullOrWhiteSpace(x.Tag) && !string.IsNullOrWhiteSpace(y.Tag) &&
                x.Tag.Equals(y.Tag, StringComparison.CurrentCulture))
                return true;

            else
                return false;
        }

        public int GetHashCode(FilterTag tag)
        {
            if (!string.IsNullOrWhiteSpace(tag.Tag))
            {
                return tag.Tag.GetHashCode();
            }
            else
                return tag.GetHashCode();
        }
    }
}

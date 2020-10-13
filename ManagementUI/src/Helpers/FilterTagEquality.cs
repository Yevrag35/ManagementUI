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
            if (object.ReferenceEquals(x, y))
                return true;

            else
                return StringComparer.CurrentCultureIgnoreCase.Equals(x?.Tag, y?.Tag);
        }

        public int GetHashCode(FilterTag tag)
        {
            return StringComparer.CurrentCultureIgnoreCase.GetHashCode(tag?.Tag);
        }
    }
}

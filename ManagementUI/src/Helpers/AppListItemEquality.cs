using System;
using System.Collections.Generic;
using System.Linq;

namespace ManagementUI
{
    internal struct AppListItemEquality : IEqualityComparer<AppListItem>
    {
        public bool Equals(AppListItem x, AppListItem y)
        {
            return x.Path.Equals(y.Path, StringComparison.CurrentCultureIgnoreCase)
                &&
                x.Arguments.Equals(y.Arguments, StringComparison.CurrentCultureIgnoreCase);
        }

        public int GetHashCode(AppListItem item)
        {
            string pathAndArgs = string.Format("{0}&{1}", item.Path, item.Arguments);
            return pathAndArgs.ToLower().GetHashCode();
        }
    }
}

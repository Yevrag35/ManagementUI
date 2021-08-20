using System;
using System.ComponentModel;

namespace ManagementUI.Extensions
{
    public static class ListSortDirectionExtensions
    {
        public static ListSortDirection ToOpposite(this ListSortDirection current)
        {
            bool asBool = Convert.ToBoolean((int)current);
            return (ListSortDirection)Convert.ToInt32(!asBool);
        }
    }
}

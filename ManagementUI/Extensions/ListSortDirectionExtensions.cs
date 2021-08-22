using System;
using System.ComponentModel;

namespace ManagementUI.Extensions
{
    public static class ListSortDirectionExtensions
    {
        public static ListSortDirection ToOpposite(this ListSortDirection current)
        {
            return (ListSortDirection)(((int)current - 1) * -1);
        }
    }
}

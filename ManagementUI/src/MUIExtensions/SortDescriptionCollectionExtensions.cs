using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ManagementUI.Extensions
{
    public static class SortDescriptionCollectionExtensions
    {
        public static void AddOnly(this SortDescriptionCollection collection, SortDescription sortDescription)
        {
            collection.Clear();
            collection.Add(sortDescription);
        }
        public static void AddOnly(this SortDescriptionCollection collection, string propertyName, ListSortDirection direction)
        {
            AddOnly(collection, new SortDescription(propertyName, direction));
        }

        public static void ChangeDirection(this SortDescriptionCollection collection, SortDescription sortDescription)
        {
            ListSortDirection opposite = sortDescription.Direction.ToOpposite();
            AddOnly(collection, new SortDescription(sortDescription.PropertyName, opposite));
        }

        public static bool TryGet(this SortDescriptionCollection collection, string memberName, out SortDescription sortDescription)
        {
            sortDescription = default;
            bool exists = false;
            foreach (SortDescription sd in collection)
            {
                if (sd.PropertyName.Equals(memberName))
                {
                    sortDescription = sd;
                    exists = true;
                    break;
                }
            }

            return exists;
        }
    }
}

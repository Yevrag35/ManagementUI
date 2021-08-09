using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace ManagementUI.Extensions
{
    public static class SortDescriptionCollectionExtensions
    {
        public static void Add<T, U>(this SortDescriptionCollection collection, ListSortDirection direction, Expression<Func<T, U>> memberExpression)
        {
            if (TryGetMemberNameFromExpression(memberExpression, out string memberName))
            {
                var sortDesc = new SortDescription(memberName, direction);
                collection.Add(sortDesc);
            }
        }
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
        internal static string GetMemberNameFromExpression<T, U>(Expression<Func<T, U>> memberExpression)
        {
            string name = null;

            if (memberExpression.Body is MemberExpression memEx)
            {
                name = memEx.Member.Name;
            }
            else if (memberExpression.Body is UnaryExpression unEx && unEx.Operand is MemberExpression unExMem)
            {
                name = unExMem.Member.Name;
            }

            return name;
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
        private static bool TryGetMemberNameFromExpression<T, U>(Expression<Func<T, U>> memberExpression, out string memberName)
        {
            memberName = GetMemberNameFromExpression(memberExpression);
            return !string.IsNullOrEmpty(memberName);
        }
    }
}

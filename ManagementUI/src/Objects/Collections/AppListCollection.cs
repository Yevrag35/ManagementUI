using System;
using System.Collections.Generic;
using System.Linq;

namespace ManagementUI
{
    /// <summary>
    /// A collection class of <see cref="AppListItem"/> objects to be used as the ItemsSource of a <see cref="System.Windows.Controls.ListView"/>.
    /// </summary>
    public class AppListCollection : BaseMuiCollection<AppListItem>, ICloneable
    {
        #region PROPERTIES
        /// <summary>
        /// Gets a value indicating whether the <see cref="AppListCollection"/> is read-only.
        /// </summary>
        public override bool IsReadOnly => ((IList<AppListItem>)base.InnerList).IsReadOnly;
        /// <summary>
        /// Gets the unique tags from each <see cref="AppListItem"/> in the collection.
        /// </summary>
        public string[] Tags => this.Where(x => x.TagList != null).SelectMany(x => x.TagList).Distinct().ToArray();

        #endregion

        #region CONSTRUCTORS
        public AppListCollection() : base() { }
        public AppListCollection(int capacity) : base(capacity) { }
        private AppListCollection(IEnumerable<AppListItem> items) : base(items) { }

        #endregion

        #region PUBLIC METHODS
        public AppListCollection Clone()
        {
            var newList = new AppListCollection(this);
            return newList;
        }
        public AppListItem Find(Predicate<AppListItem> match) => base.InnerList.Find(match);
        public override void Sort() => base.Sort(new AppListItemSorter());

        #endregion

        #region INTERFACE IMPLEMENTATIONS
        object ICloneable.Clone() => this.Clone();

        #endregion

        #region COMPARERS
        public class AppListItemSorter : IComparer<AppListItem>
        {
            public int Compare(AppListItem x, AppListItem y) => x.AppName.CompareTo(y.AppName);
        }
        public class AppListItemDescendingSorter : IComparer<AppListItem>
        {
            public int Compare(AppListItem x, AppListItem y) => x.AppName.CompareTo(y.AppName) * -1;
        }
        public class AppListItemTagSorter : IComparer<AppListItem>
        {
            public int Compare(AppListItem x, AppListItem y)
            {
                if (x.TagList == null && y.TagList == null)
                    return 0;

                else if (x.TagList != null && y.TagList == null)
                    return 1;

                else if (x.TagList == null && y.TagList != null)
                    return -1;

                else
                {
                    return x.TagList.Count.CompareTo(y.TagList.Count);
                }
            }
        }

        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Linq;

namespace ManagementUI
{
    public class TagCollection : BaseMuiCollection<FilterTag>
    {
        #region FIELDS/CONSTANTS


        #endregion

        #region PROPERTIES


        #endregion

        #region CONSTRUCTORS
        public TagCollection() : base() { }
        public TagCollection(int capacity) : base(capacity) { }
        internal TagCollection(IEnumerable<FilterTag> items) : base(items) { }

        #endregion

        #region PUBLIC METHODS
        public IEnumerable<FilterTag> FindAll(Predicate<FilterTag> match) => base.InnerList.FindAll(match);
        public override void Sort() => base.InnerList.Sort(new TagSorter());
        public bool TrueForAll(Predicate<FilterTag> match) => base.InnerList.TrueForAll(match);

        public class TagSorter : IComparer<FilterTag>
        {
            public int Compare(FilterTag x, FilterTag y) => x.Tag.CompareTo(y.Tag);
        }

        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Linq;

namespace ManagementUI
{
    public class FilterTag
    {
        #region PROPERTIES
        public bool IsChecked { get; set; }
        public string Tag { get; set; }

        #endregion

        #region CONSTRUCTORS
        public FilterTag() { }
        public FilterTag(string tag) => this.Tag = tag;
        internal FilterTag(string tag, bool isChecked)
            : this(tag) => this.IsChecked = isChecked;

        #endregion

        #region PUBLIC METHODS


        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Linq;

namespace ManagementUI
{
    public struct FilterTag : ICloneable, IComparable<FilterTag>, IComparable<string>, IEquatable<FilterTag>, IEquatable<string>
    {
        #region PROPERTIES
        public bool IsChecked { get; set; }
        public string Tag { get; set; }

        #endregion

        #region CONSTRUCTORS
        public FilterTag(string tag)
        {
            this.IsChecked = false;
            this.Tag = tag;
        }
        internal FilterTag(string tag, bool isChecked)
            : this(tag) => this.IsChecked = isChecked;

        #endregion

        #region PUBLIC METHODS
        public FilterTag Clone() => new FilterTag
        {
            Tag = this.Tag,
            IsChecked = this.IsChecked
        };
        object ICloneable.Clone() => this.Clone();

        public int CompareTo(FilterTag other) => this.Tag.CompareTo(other.Tag);
        public int CompareTo(string tagString) => this.Tag.CompareTo(tagString);
        public bool Equals(FilterTag other) => this.Tag.Equals(other.Tag, StringComparison.CurrentCulture) && this.IsChecked == other.IsChecked;
        public bool Equals(string str) => this.Tag.Equals(str, StringComparison.CurrentCulture);

        public static explicit operator FilterTag(string tagString) => new FilterTag(tagString);

        #endregion
    }
}
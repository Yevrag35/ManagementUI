using ManagementUI.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ManagementUI
{
    [JsonConverter(typeof(JsonFilterTagConverter))]
    public class FilterTag : ICloneable, IComparable<FilterTag>, IComparable<string>, IEquatable<FilterTag>, IEquatable<string>
    {
        #region PROPERTIES
        public bool IsChecked { get; set; }
        public string Tag { get; set; }

        #endregion

        #region CONSTRUCTORS
        internal FilterTag() { }
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

        public int CompareTo(FilterTag other) => (this.Tag?.CompareTo(other.Tag)).GetValueOrDefault();
        public int CompareTo(string tagString) => (this.Tag?.CompareTo(tagString)).GetValueOrDefault();
        public bool Equals(FilterTag other)
        {
            bool result = false;
            if ((string.IsNullOrWhiteSpace(this.Tag) && string.IsNullOrWhiteSpace(other.Tag)) ||
                (!string.IsNullOrWhiteSpace(this.Tag) && !string.IsNullOrWhiteSpace(other.Tag)
                && this.Tag.Equals(other.Tag, StringComparison.CurrentCulture)))
            {
                result = true;
            }
            return result;
            //(this.Tag?.Equals(other.Tag, StringComparison.CurrentCulture)).GetValueOrDefault() && this.IsChecked == other.IsChecked;
        }
        public bool Equals(string str) => (this.Tag?.Equals(str, StringComparison.CurrentCulture)).GetValueOrDefault();

        public static implicit operator FilterTag(string tagString) => new FilterTag(tagString);

        #endregion
    }
}
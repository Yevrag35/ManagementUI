using ManagementUI.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace ManagementUI
{
    [JsonConverter(typeof(JsonFilterTagConverter))]
    public class FilterTag : ChangeableItem, ICloneable, IComparable<FilterTag>, IComparable<string>, IEquatable<FilterTag>, IEquatable<string>,
        INotifyPropertyChanged
    {
        private static StringComparer Comparer = StringComparer.CurrentCultureIgnoreCase;
        private bool _isChecked;
        private string _tag;

        public override event PropertyChangedEventHandler PropertyChanged;

        #region PROPERTIES
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                _isChecked = value;
                this.OnChange(x => x.IsChecked);
            }
        }
        public string Tag
        {
            get => _tag;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _tag = value;
                    this.OnChange(x => x.Tag);
                }
            }
        }

        #endregion

        #region EVENT HANDLERS
        private void OnChange<T>(Expression<Func<FilterTag, T>> expression)
        {
            if (this.PropertyChanged != null)
            {
                string memberName = base.GetPropertyName(expression);
                this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(memberName));
            }
        }

        #endregion

        #region CONSTRUCTORS
        internal FilterTag() { }
        public FilterTag(string tag)
        {
            _isChecked = false;
            _tag = tag;
        }
        internal FilterTag(string tag, bool isChecked)
            : this(tag) => _isChecked = isChecked;

        #endregion

        #region PUBLIC METHODS
        public FilterTag Clone() => new FilterTag
        {
            _tag = this.Tag,
            _isChecked = this.IsChecked
        };
        object ICloneable.Clone() => this.Clone();

        public int CompareTo(FilterTag other) => Comparer.Compare(this.Tag, other?.Tag);
        public int CompareTo(string tagString) => Comparer.Compare(this.Tag, tagString);
        public bool Equals(FilterTag other)
        {
            return Comparer.Equals(this.Tag, other?.Tag);
        }
        public bool Equals(string str) => Comparer.Equals(this.Tag, str);

        public static implicit operator FilterTag(string tagString) => new FilterTag(tagString);

        #endregion
    }
}
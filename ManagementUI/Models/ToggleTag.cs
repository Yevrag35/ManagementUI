using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagementUI.Functionality.Models;
using ManagementUI.Models;

namespace ManagementUI
{
    public class ToggleTag : UIModelBase, ICloneable, IEquatable<ToggleTag>, INotifyPropertyChanged
    {
        private bool _isChecked;
        private IEqualityComparer<string> _comparer;

        public IEqualityComparer<string> Comparer
        {
            get => _comparer;
            set
            {
                if (null == value)
                    _comparer = StringComparer.CurrentCultureIgnoreCase;

                else
                    _comparer = value;
            }
        }
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                _isChecked = value;
                this.NotifyOfChange(nameof(IsChecked));
            }
        }
        internal UserTag UserTag { get; set; }
        public string Value
        {
            get => this.UserTag.Value;
            set
            {
                var tag = this.UserTag;
                this.UserTag = new UserTag(tag.Id, value);
                this.NotifyOfChange(nameof(Value));
            }
        }
        

        public ToggleTag()
            : this(StringComparer.CurrentCultureIgnoreCase)
        {
        }
        public ToggleTag(IEqualityComparer<string> equalityComparer)
        {
            _comparer = equalityComparer;
        }

        public ToggleTag Clone()
        {
            return new ToggleTag
            {
                _comparer = _comparer,
                _isChecked = _isChecked,
                UserTag = new UserTag(this.UserTag.Id, this.UserTag.Value)
            };
        }
        object ICloneable.Clone() => this.Clone();
        public bool Equals(ToggleTag other)
        {
            return _comparer.Equals(this.Value, other?.Value) || this.UserTag.Id.Equals(other.UserTag.Id);
        }
        public bool TextEquals(string text)
        {
            return this.UserTag.TextEquals(text, _comparer);
        }

        public IToggleTagComparer NewValueComparer()
        {
            return NewValueComparer(_comparer);
        }
        public static IToggleTagComparer NewValueComparer(IEqualityComparer<string> comparer)
        {
            return new TagValueEqualityComparer(comparer);
        }

        private class TagValueEqualityComparer : IToggleTagComparer, IEqualityComparer<ToggleTag>
        {
            private IEqualityComparer<string> _comparer;
            public IEqualityComparer<string> StringComparer => _comparer;
            public TagValueEqualityComparer(IEqualityComparer<string> stringComparer)
            {
                _comparer = stringComparer;
            }

            public bool Equals(ToggleTag x, ToggleTag y)
            {
                return _comparer.Equals(x?.Value, y?.Value);
            }
            public int GetHashCode(ToggleTag o) => _comparer.GetHashCode(o?.Value);
        }
    }
}

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
    public class ToggleTag : UIModelBase, IEquatable<ToggleTag>, INotifyPropertyChanged
    {
        private bool _isChecked;

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
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                _isChecked = value;
                this.NotifyOfChange(nameof(IsChecked));
            }
        }

        public bool Equals(ToggleTag other)
        {
            return this.UserTag.Equals((other?.UserTag).GetValueOrDefault());
        }
    }
}

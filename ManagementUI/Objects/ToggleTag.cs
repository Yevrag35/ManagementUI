using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagementUI.Functionality.Models;
using ManagementUI.Models;

namespace ManagementUI
{
    public class ToggleTag : IEquatable<ToggleTag>
    {
        internal UserTag UserTag { get; set; }
        public string Value
        {
            get => this.UserTag.Value;
            set
            {
                var tag = this.UserTag;
                this.UserTag = new UserTag(tag.Id, value);
            }
        }
        public bool IsChecked { get; set; }

        public bool Equals(ToggleTag other)
        {
            return this.UserTag.Equals((other?.UserTag).GetValueOrDefault());
        }
    }
}

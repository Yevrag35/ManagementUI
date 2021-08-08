using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagementUI.Functionality.Models;
using ManagementUI.Functionality.Models.Collections;
using ManagementUI.Models;

namespace ManagementUI.Collections
{
    public class TagCollection : UniqueObservableList<ToggleTag>
    {
        private HashSet<ToggleTag> Enabled;
        private HashSet<ToggleTag> Disabled;

        public IEnumerable<UserTag> EnabledTags => this.Enabled.Select(x => x.UserTag);
        public IEnumerable<UserTag> DisabledTags => this.Disabled.Select(x => x.UserTag);

        public TagCollection(IEnumerable<UserTag> tags)
            : base(AsToggleTags(tags))
        {
            this.Disabled = new HashSet<ToggleTag>(this.InnerList);
            this.Enabled = new HashSet<ToggleTag>(this.InnerList.Count);
        }

        public int Disable(ToggleTag tag)
        {
            if (!tag.IsChecked && this.Enabled.Remove(tag))
            {
                this.Disabled.Add(tag);
            }

            return this.Enabled.Count;
        }
        public int Enable(ToggleTag tag)
        {
            if (tag.IsChecked && this.Disabled.Remove(tag))
            {
                this.Enabled.Add(tag);
            }

            return this.Enabled.Count;
        }

        private static IEnumerable<ToggleTag> AsToggleTags(IEnumerable<UserTag> tags)
        {
            return tags.Select(x => new ToggleTag { UserTag = x });
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagementUI.Functionality.Models;
using ManagementUI.Functionality.Models.Collections;
using ManagementUI.Models;

namespace ManagementUI.Collections
{
    public class TagCollection : ObservableViewBase<ToggleTag>
    {
        private static IToggleTagComparer _comparer = ToggleTag.NewValueComparer(StringComparer.CurrentCultureIgnoreCase);
        private HashSet<ToggleTag> Enabled;
        private HashSet<ToggleTag> Disabled;
        private int _nextId;

        public IEnumerable<UserTag> EnabledTags => this.Enabled.Select(x => x.UserTag);
        public int EnabledCount => this.Enabled.Count;
        public IEnumerable<UserTag> DisabledTags => this.Disabled.Select(x => x.UserTag);

        protected override bool IsLiveFiltering => false;
        protected override string[] LiveSortingProperties => new string[1] { nameof(ToggleTag.Value) };

        public TagCollection(IEnumerable<UserTag> tags)
            : base(AsToggleTags(tags.OrderBy(x => x.Value)), _comparer)
        {
            this.Disabled = new HashSet<ToggleTag>(this.InnerList, _comparer);
            this.Enabled = new HashSet<ToggleTag>(this.InnerList.Count, _comparer);
            _nextId = this.Disabled.Count + 1;
            this.CreateView();
        }

        protected override bool Add(ToggleTag item, bool adding)
        {
            UserTag tag = item.UserTag;
            item.UserTag = new UserTag(_nextId, tag.Value);
            if (this.Disabled.Add(item) && base.Add(item, true))
            {
                _nextId++;
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Disables the specified tag.
        /// </summary>
        /// <param name="tag">The tag to disable.</param>
        /// <returns>The number of 'Enabled' tags after disabling <paramref name="tag"/>.</returns>
        public int Disable(ToggleTag tag)
        {
            if (!tag.IsChecked && this.Enabled.Remove(tag))
            {
                this.Disabled.Add(tag);
            }

            return this.Enabled.Count;
        }

        /// <summary>
        /// Enables the specified tag.
        /// </summary>
        /// <param name="tag">The tag to enable.</param>
        /// <returns>The number of 'Enabled' tags after enabling <paramref name="tag"/>.</returns>
        public int Enable(ToggleTag tag)
        {
            if (tag.IsChecked && this.Disabled.Remove(tag))
            {
                this.Enabled.Add(tag);
            }

            return this.Enabled.Count;
        }

        public void ExceptWith(IEnumerable<ToggleTag> other)
        {
            this.Enabled.ExceptWith(other);
            this.Disabled.ExceptWith(other);
            foreach (ToggleTag tag in other)
            {
                tag.IsChecked = false;
                _ = this.Remove(tag);
            }
        }

        public EditTagCollection ToEditCollection()
        {
            return new EditTagCollection(this.Select(x => x.Clone()));
        }
        public bool IsSupersetOf(IEnumerable<UserTag> tags)
        {
            foreach (UserTag tag in tags)
            {
                if (!this.Exists(x => x.UserTag.Equals(tag)))
                {
                    return false;
                }
            }

            return true;
        }
        public void UnionWith(IEnumerable<ToggleTag> other)
        {
            foreach (ToggleTag tag in other)
            {
                if (this.Add(tag, true))
                {
                    tag.IsChecked = false;
                    this.Enabled.Add(tag);
                    this.Disabled.Add(tag);
                }
            }
        }

        private static IEnumerable<ToggleTag> AsToggleTags(IEnumerable<UserTag> tags)
        {
            return tags
                .Select(x =>
                    new ToggleTag(_comparer.StringComparer)
                    {
                        UserTag = x
                    }
                );
        }
    }
}

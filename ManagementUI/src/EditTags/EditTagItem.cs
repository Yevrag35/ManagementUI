using System;
using System.Collections.Generic;
using System.Linq;

namespace ManagementUI.Editing
{
    public struct EditTagItem
    {
        public bool IsChecked;
        public EditingStatus Status;
        public string Title;

        public static explicit operator EditTagItem(FilterTag filTag)
        {
            return new EditTagItem
            {
                IsChecked = filTag.IsChecked,
                Status = EditingStatus.Available,
                Title = filTag.Tag
            };
        }
        public static explicit operator FilterTag(EditTagItem editTagItem) => new FilterTag(editTagItem.Title, editTagItem.IsChecked);

        public static List<FilterTag> Apply(ICollection<EditTagItem> these)
        {
            var list = new List<FilterTag>(these.Count);
            foreach (FilterTag eti in these)
            {
                list.Add(eti);
            }
            return list;
        }
    }

    public enum EditingStatus
    {
        Available,
        Applied
    }
}

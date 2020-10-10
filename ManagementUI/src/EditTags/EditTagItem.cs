using System;
using System.Collections.Generic;
using System.Linq;

namespace ManagementUI.Editing
{
    public class EditTagItem
    {
        public bool IsChecked { get; set; }
        public EditingStatus Status { get; set; }
        public string Title { get; set; }

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

        public static EditTagItem FromFilterTag(FilterTag ft, AppIconSetting ais)
        {
            return new EditTagItem
            {
                IsChecked = ft.IsChecked,
                Status = ais.Tags.Contains(ft.Tag) ? EditingStatus.Applied : EditingStatus.Available,
                Title = ft.Tag
            };
        }
    }

    public enum EditingStatus
    {
        Available,
        Applied
    }
}

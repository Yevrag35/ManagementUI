using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace ManagementUI.Editing
{
    public class EditTagItem : ChangeableItem, INotifyPropertyChanged
    {
        private string _title;
        private EditingStatus _status;

        public bool IsChecked { get; set; }
        public EditingStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                this.OnChange(x => x.Status);
            }
        }
        public string Title
        {
            get => _title;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _title = value;
                    this.OnChange(x => x.Title);
                }
            }
        }
        public override event PropertyChangedEventHandler PropertyChanged;


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

        #region EVENT HANDLERS
        private void OnChange<T>(Expression<Func<EditTagItem, T>> expression)
        {
            if (this.PropertyChanged != null)
            {
                string memberName = base.GetPropertyName(expression);
                this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(memberName));
            }
        }

        #endregion
    }

    public enum EditingStatus
    {
        Available,
        Applied
    }
}

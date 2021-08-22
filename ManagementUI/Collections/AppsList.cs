using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using ManagementUI.Extensions;
using ManagementUI.Functionality.Models;
using ManagementUI.Models;

namespace ManagementUI.Collections
{
    public class AppsList : ObservableViewBase<AppItem>
    {
        public event EventHandler Changed;

        protected override Predicate<object> Filter => x => x is AppItem ai && !ai.DontShow;
        public bool IsFiltered { get; private set; }
        protected override string[] LiveFilteringProperties => new string[1] { nameof(AppItem.DontShow) };
        protected override string[] LiveSortingProperties => new string[1] { nameof(AppItem.Name) };

        public AppsList(IEnumerable<AppItem> apps)
            : base(apps)
        {
        }

        protected override bool Add(AppItem item, bool adding)
        {
            bool result = base.Add(item, adding);
            if (result)
            {
                this.Changed?.Invoke(this, EventArgs.Empty);
            }

            return result;
        }
        protected override bool Remove(AppItem item, bool removing)
        {
            bool result = base.Remove(item, removing);
            if (result)
            {
                this.Changed?.Invoke(this, EventArgs.Empty);
            }

            return result;
        }

        public void ChangeSortOrder()
        {
            if (this.View.SortDescriptions.Count >= 1)
            {
                var sd = this.View.SortDescriptions[0];

                var newSd = new SortDescription(sd.PropertyName, sd.Direction.ToOpposite());
                if (this.View.SortDescriptions.Remove(sd))
                {
                    this.View.SortDescriptions.Clear();
                    this.View.SortDescriptions.Add(newSd);
                }
            }
            else
            {
                this.View.SortDescriptions.Add(new SortDescription(nameof(AppItem.Name), ListSortDirection.Ascending));
            }
        }

        public void EnableByTags(IEnumerable<UserTag> tagsToEnable)
        {
            this.EnableItems(ai => ai.Tags.IsSupersetOf(tagsToEnable));
        }
        public void EnableItems(Predicate<AppItem> predicate)
        {
            if (this.IsFiltered)
            {
                this.ResetItems();
            }

            this
                .FindAll(Negate(predicate))
                    .ForEach((ai) =>
                    {
                        ai.DontShow = true;
                    });

            this.IsFiltered = true;
        }
        public void ResetItems()
        {
            this.ForEach((ai) =>
            {
                ai.DontShow = false;
            });
            this.IsFiltered = false;
        }
    }
}

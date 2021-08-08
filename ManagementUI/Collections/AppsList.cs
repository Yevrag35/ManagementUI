using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using ManagementUI.Functionality.Models;
using ManagementUI.Models;

namespace ManagementUI.Collections
{
    public class AppsList : ObservableViewBase<AppItem>
    {
        protected override Predicate<object> Filter => x => x is AppItem ai && !ai.DontShow;
        public bool IsFiltered { get; private set; }
        protected override string[] LiveFilteringProperties => new string[1] { nameof(AppItem.DontShow) };
        protected override string[] LiveSortingProperties => new string[1] { nameof(AppItem.Name) };

        public AppsList(IEnumerable<AppItem> apps)
            : base(apps)
        {
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

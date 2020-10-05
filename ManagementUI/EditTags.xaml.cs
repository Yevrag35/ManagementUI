using ManagementUI.Editing;
using ManagementUI.Extensions;
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace ManagementUI
{
    /// <summary>
    /// Interaction logic for EditTags.xaml
    /// TO DO - FIND A WAY TO LOAD THE EXISTING APPLIED EASILY
    /// </summary>
    public partial class EditTags : Window
    {
        private HashSet<FilterTag> _allFilterTags;

        public CollectionViewSource AppliedView { get; set; }
        public CollectionViewSource AvailableView { get; set; }

        public HashSet<EditTagItem> AllTags { get; set; }
        //public HashSet<EditTagItem> Available { get; }
        public AppIconSetting Application { get; }
        //public HashSet<EditTagItem> Applied { get; }

        public EditTags(AppIconSetting chosenApp, IEnumerable<FilterTag> allTags)
        {
            _allFilterTags = new HashSet<FilterTag>(allTags);
            this.Application = chosenApp;
            foreach (EditTagItem ft in allTags)
            {
                this.AllTags.Add(ft);
                if (this.Application.Tags.Contains(ft.Title))
                    ft.Status = EditingStatus.Applied;
            }

            this.AppliedView = new CollectionViewSource { Source = this.AllTags };
            this.AppliedView.SortDescriptions.Add<EditTagItem, string>(ListSortDirection.Ascending, x => x.Title);
            this.AppliedView.View.Filter = this.OnlyApplied;

            this.AvailableView = new CollectionViewSource { Source = this.AllTags };
            this.AvailableView.SortDescriptions.Add<EditTagItem, string>(ListSortDirection.Ascending, x => x.Title);
            this.AvailableView.View.Filter = this.OnlyAvailable;
            this.InitializeComponent();

            this.AppliedView.View.Refresh();
            this.AvailableView.View.Refresh();
            this.AppliedTagsList.ItemsSource = this.AppliedView.View;
            this.AvailableTagsList.ItemsSource = this.AvailableView.View;
        }

        private bool OnlyAvailable(object item)
        {
            return item is EditTagItem eti && eti.Status == EditingStatus.Available;
        }
        private bool OnlyApplied(object item)
        {
            return item is EditTagItem eti && eti.Status == EditingStatus.Applied;
        }

        private void TextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void TextBox_MouseEnter(object sender, MouseEventArgs e)
        {

        }

        private async void RemoveTagBtn_Click(object sender, RoutedEventArgs e)
        {
            await this.Dispatcher.InvokeAsync(() =>
            {
                foreach (EditTagItem ft in this.AppliedTagsList.SelectedItems)
                {
                    ft.Status = EditingStatus.Available;
                }
                this.AppliedView.View.Refresh();
                this.AvailableView.View.Refresh();
            });
        }

        private async void ApplyTagBtn_Click(object sender, RoutedEventArgs e)
        {
            await this.Dispatcher.InvokeAsync(() =>
            {
                foreach (EditTagItem ft in this.AvailableTagsList.SelectedItems)
                {
                    ft.Status = EditingStatus.Applied;
                }
                this.AppliedView.View.Refresh();
                this.AvailableView.View.Refresh();
            });
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.DialogResult.HasValue && this.DialogResult.Value)
            {


                this.Application.Tags.Clear();
                this.Application.Tags.UnionWith(_allFilterTags.Where(x => this.AllTags.Any(et => et.Status == EditingStatus.Applied && et.Title == x.Tag)));
                //this.Application.Tags.ExceptWith(this.AllTags.Where(x => x.Status == EditingStatus.Available).Cast<FilterTag>());
            }
        }

        private void OKBtn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void NewTagBtn_Click(object sender, RoutedEventArgs e)
        {
            var newTag = Interaction.InputBox(
                "Enter the name of a new tag:",
                "New Tag",
                "<new tag>"
            );
            if (!string.IsNullOrWhiteSpace(newTag) && newTag != "<new tag>")
            {
                var ft = new EditTagItem
                {
                    IsChecked = false,
                    Status = EditingStatus.Available,
                    Title = newTag
                };
                this.AllTags.Add(ft);
                _allFilterTags.Add(new FilterTag(ft.Title, false));

                this.Dispatcher.Invoke(() =>
                {
                    this.AvailableView.View.Refresh();
                });

                //this.Available.Add(ft);
                //this.AvailableTagsList.Items.Refresh();
            }
        }
    }
}

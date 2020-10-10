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
        public HashSet<FilterTag> AllFilterTags { get; }

        //public CollectionViewSource AppliedView { get; set; }
        //public CollectionViewSource AvailableView { get; set; }

        //public HashSet<EditTagItem> AllTags { get; set; }
        public EditTagList AllTags { get; }
        //public HashSet<EditTagItem> Available { get; }
        public AppIconSetting Application { get; }
        //public HashSet<EditTagItem> Applied { get; }

        public EditTags(AppIconSetting chosenApp, IEnumerable<FilterTag> allTags)
        {
            this.AllFilterTags = new HashSet<FilterTag>(allTags);
            this.Application = chosenApp;
            this.AllTags = new EditTagList(allTags, chosenApp);
            this.InitializeComponent();

            this.AppliedTagsList.ItemsSource = this.AllTags.Applied;
            this.AvailableTagsList.ItemsSource = this.AllTags.Available;
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
                foreach (EditTagItem eti in this.AppliedTagsList.SelectedItems)
                {
                    eti.Status = EditingStatus.Available;
                }
            });
        }

        private async void ApplyTagBtn_Click(object sender, RoutedEventArgs e)
        {
            await this.Dispatcher.InvokeAsync(() =>
            {
                foreach (EditTagItem eti in this.AvailableTagsList.SelectedItems)
                {
                    eti.Status = EditingStatus.Applied;
                }
            });
        }

        private void OKBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Application.Tags.Clear();
            this.Application.Tags.UnionWith(this.AllTags.Where(x => x.Status == EditingStatus.Applied).Select(x => x.Title));
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
                this.Dispatcher.Invoke(() =>
                {
                    this.AllTags.Add(ft);
                    this.AllFilterTags.Add(ft.Title);
                    this.AllTags.Available.Refresh();
                    this.AllTags.Applied.Refresh();
                });
            }
        }
    }
}

using ManagementUI.Editing;
using ManagementUI.Extensions;
using Microsoft.VisualBasic;
using Ookii.Dialogs.Wpf;
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
using ManagementUI.Functionality.Models;
using ManagementUI.Models;
using ManagementUI.Collections;

namespace ManagementUI
{
    /// <summary>
    /// Interaction logic for EditTags.xaml
    /// TO DO - FIND A WAY TO LOAD THE EXISTING APPLIED EASILY
    /// </summary>
    public partial class EditTags : Window
    {
        public HashSet<FilterTag> AllFilterTags { get; }
        public EditTagList AllTags { get; }
        public AppIconSetting Application { get; }
        public AppItem ChosenApp { get; }
        public EditTagCollection Tags { get; }
        public string WindowName { get; set; }

        public EditTags(AppItem chosenApp, EditTagCollection tags)
        {
            this.WindowName = chosenApp.Name;
            this.Tags = tags;
            this.ChosenApp = chosenApp;
            this.Tags.ForEach((t) =>
            {
                if (chosenApp.Tags.Contains(t.UserTag))
                {
                    t.IsChecked = true;
                }
            });

            this.InitializeComponent();

            this.AvailableTagsList.ItemsSource = this.Tags.Available;
            this.AppliedTagsList.ItemsSource = this.Tags.Applied;
        }
        public EditTags(AppIconSetting chosenApp, IEnumerable<FilterTag> allTags)
        {
            this.AllFilterTags = new HashSet<FilterTag>(allTags);
            this.Application = chosenApp;
            this.AllTags = new EditTagList(allTags, chosenApp);
            this.InitializeComponent();

            this.AppliedTagsList.ItemsSource = this.AllTags.Applied;
            this.AvailableTagsList.ItemsSource = this.AllTags.Available;
        }

        private async void ApplyTagBtn_Click(object sender, RoutedEventArgs e)
        {
            await this.Dispatcher.InvokeAsync(() =>
            {
                if (this.AvailableTagsList.SelectedItems.Count > 0)
                {
                    foreach (ToggleTag tag in this.AvailableTagsList.SelectedItems)
                    {
                        tag.IsChecked = true;
                    }
                }
            });
        }

        private async void RemoveTagBtn_Click(object sender, RoutedEventArgs e)
        {
            await this.Dispatcher.InvokeAsync(() =>
            {
                if (this.AppliedTagsList.SelectedItems.Count > 0)
                {
                    foreach (ToggleTag tag in this.AppliedTagsList.SelectedItems)
                    {
                        tag.IsChecked = false;
                    }
                }
            });
        }

        private void OKBtn_Click(object sender, RoutedEventArgs e)
        {
            //this.Application.Tags.Clear();
            //this.Application.Tags.UnionWith(this.AllTags.Where(x => x.Status == EditingStatus.Applied).Select(x => x.Title));
            this.DialogResult = false;
            this.Close();
        }

        private void NewTagBtn_Click(object sender, RoutedEventArgs e)
        {
            InputBox.Visibility = Visibility.Visible;

            //var newTag = Interaction.InputBox(
            //    "Enter the name of a new tag:",
            //    "New Tag",
            //    "<new tag>"
            //);
            //if (!string.IsNullOrWhiteSpace(newTag) && newTag != "<new tag>")
            //{
            //    var ft = new EditTagItem
            //    {
            //        IsChecked = false,
            //        Status = EditingStatus.Available,
            //        Title = newTag
            //    };
            //    this.Dispatcher.Invoke(() =>
            //    {
            //        this.AllTags.Add(ft);
            //        this.AllFilterTags.Add(ft.Title);
            //        this.AllTags.Available.Refresh();
            //        this.AllTags.Applied.Refresh();
            //    });
            //}
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            InputBox.Visibility = Visibility.Collapsed;
            InputTextBox.Clear();
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            InputBox.Visibility = Visibility.Collapsed;
            InputTextBox.Clear();
        }

        private void YesAndApplyButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

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
    /// </summary>
    public partial class EditTags : Window
    {
        public AppItem ChosenApp { get; }
        public bool IsModified { get; private set; }
        public HashSet<UserTag> OriginalTags { get; }
        public EditTagCollection Tags { get; }
        public HashSet<UserTag> PendingAdd { get; }
        private HashSet<UserTag> PendingRemove { get; }
        public string WindowName { get; set; }

        public EditTags(AppItem chosenApp, EditTagCollection tags)
        {
            this.OriginalTags = new HashSet<UserTag>(chosenApp.Tags);
            this.PendingAdd = new HashSet<UserTag>(tags.Count);
            this.PendingRemove = new HashSet<UserTag>(tags.Count);
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

        private async void ApplyTagBtn_Click(object sender, RoutedEventArgs e)
        {
            await this.Dispatcher.InvokeAsync(() =>
            {
                if (this.AvailableTagsList.SelectedItems.Count > 0)
                {
                    foreach (ToggleTag tag in this.AvailableTagsList.SelectedItems)
                    {
                        tag.IsChecked = true;
                        this.PendingAdd.Add(tag.UserTag);
                        this.PendingRemove.Remove(tag.UserTag);
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
                        this.PendingAdd.Remove(tag.UserTag);
                        this.PendingRemove.Add(tag.UserTag);
                    }
                }
            });
        }

        private async void InputBox_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            await this.Dispatcher.InvokeAsync(() =>
            {
                if (e.NewValue is bool isVis && !isVis)
                {
                    this.InputTextBox.Clear();
                }

                this.FlipDefaults();

            });
        }

        private async void OKBtn_Click(object sender, RoutedEventArgs e)
        {
            await this.Dispatcher.InvokeAsync(() =>
            {
                this.ChosenApp.UpdateTags(this.PendingAdd, this.PendingRemove);
                if (this.Tags.IsModified || !this.OriginalTags.SetEquals(this.ChosenApp.Tags))
                {
                    this.IsModified = true;
                }
            });

            this.DialogResult = true;
            this.Close();
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            this.PendingAdd.Clear();
            this.PendingRemove.Clear();
            this.Tags.Clear();
            this.DialogResult = false;
            this.Close();
        }

        private async void NewTagBtn_Click(object sender, RoutedEventArgs e)
        {
            await this.Dispatcher.InvokeAsync(() =>
            {
                this.InputBox.Visibility = Visibility.Visible;
            });
        }

        private async void YesButton_Click(object sender, RoutedEventArgs e)
        {
            await this.Dispatcher.InvokeAsync(() =>
            {
                string newTag = this.InputTextBox.Text.Trim();
                if (this.Tags.ContainsText(newTag))
                {
                    if (MUI.ShowErrorMessage(new ArgumentException(string.Format("{0} already exists as a tag", newTag)), true))
                    {
                        e.Handled = true;
                        this.InputTextBox.Focus();
                        this.InputTextBox.SelectAll();
                        return;
                    }
                }
                else
                {
                    int index = this.Tags.Add(newTag);
                    if (index > -1)
                    {
                        this.PendingAdd.Add(this.Tags.GetTag(index).UserTag);
                    }
                }

                InputBox.Visibility = Visibility.Collapsed;
            });
        }
        private async void NoButton_Click(object sender, RoutedEventArgs e)
        {
            await this.Dispatcher.InvokeAsync(() =>
            {
                InputBox.Visibility = Visibility.Collapsed;
            });
        }
        private void FlipDefaults()
        {
            this.OKBtn.IsDefault = !this.OKBtn.IsDefault;
            this.YesButton.IsDefault = !this.YesButton.IsDefault;

            this.ExitBtn.IsCancel = !this.ExitBtn.IsCancel;
            this.NoButton.IsCancel = !this.NoButton.IsCancel;
        }
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (this.NoButton.IsCancel)
            {
                e.Cancel = true;
            }
        }
    }
}

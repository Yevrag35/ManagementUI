using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ManagementUI
{
    /// <summary>
    /// Interaction logic for EditTags.xaml
    /// </summary>
    public partial class EditTags : Window
    {
        public TagCollection AllTags { get; set; }
        public TagCollection Available { get; }
        public AppListItem Application { get; }
        public TagCollection Applied { get; }

        public EditTags(AppListItem ali, IEnumerable<FilterTag> currentTags)
        {
            this.AllTags = new TagCollection(currentTags);
            this.Application = ali;

            if (this.Application.TagList == null)
                this.Application.TagList = new List<string>();

            this.Available = new TagCollection(this.AllTags.FindAll(x => !this.Application.TagList.Contains(x.Tag)));
            this.Available.CollectionChanged += this.OnAvailableTag_Changed;

            this.Applied = new TagCollection(this.AllTags.FindAll(x => this.Application.TagList.Contains(x.Tag)));
            this.Applied.CollectionChanged += this.OnAppliedTag_Changed;

            InitializeComponent();

            this.AvailableTagsList.ItemsSource = this.Available;
            this.AppliedTagsList.ItemsSource = this.Applied;
        }

        private void OnAvailableTag_Changed(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                {
                    foreach (FilterTag ft in e.NewItems)
                    {
                        ((ICollection<FilterTag>)this.Applied).Remove(ft);
                    }
                    break;
                }
                case NotifyCollectionChangedAction.Remove:
                {
                    foreach (FilterTag ft in e.OldItems)
                    {
                        ((ICollection<FilterTag>)this.Available).Remove(ft);
                    }
                    break;
                }
            }
            this.Applied.Sort();
            this.Available.Sort();
        }
        private void OnAppliedTag_Changed(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                {
                    foreach (FilterTag ft in e.NewItems)
                    {
                        ((ICollection<FilterTag>)this.Available).Remove(ft);
                    }
                    break;
                }
                case NotifyCollectionChangedAction.Remove:
                {
                    foreach (FilterTag ft in e.OldItems)
                    {
                        ((ICollection<FilterTag>)this.Applied).Remove(ft);
                    }
                    break;
                }
            }
            this.Available.Sort();
            this.Applied.Sort();
        }

        private bool OnlyAvailable(object item)
        {
            return item is FilterTag ft && !Application.TagList.Contains(ft.Tag);
        }
        private bool OnlyApplied(object item)
        {
            return item is FilterTag ft && Application.TagList.Contains(ft.Tag);
        }

        private void TextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void TextBox_MouseEnter(object sender, MouseEventArgs e)
        {

        }

        private void RemoveTagBtn_Click(object sender, RoutedEventArgs e)
        {
            foreach (FilterTag ft in this.AppliedTagsList.SelectedItems)
            {
                this.Available.Add(ft);
            }
            this.AppliedTagsList.Items.Refresh();
            this.AvailableTagsList.Items.Refresh();
        }

        private void ApplyTagBtn_Click(object sender, RoutedEventArgs e)
        {
            foreach (FilterTag ft in this.AvailableTagsList.SelectedItems)
            {
                this.Applied.Add(ft);
            }

            this.AppliedTagsList.Items.Refresh();
            this.AvailableTagsList.Items.Refresh();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.DialogResult.HasValue && this.DialogResult.Value)
            {
                this.Application.TagList = this.Applied.Select(x => x.Tag).ToList();
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
                var ft = new FilterTag(newTag, false);
                this.AllTags.Add(ft);
                ((ICollection<FilterTag>)this.Available).Add(ft);
                this.AvailableTagsList.Items.Refresh();
            }
        }
    }
}

using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

namespace ManagementUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MUI : Window
    {
        internal static NetworkCredential Creds { get; set; }
        private AppList AppList { get; set; }

        public MUI() => InitializeComponent();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            App.MyHandle = new WindowInteropHelper(this).Handle;
            this.LoadIcons(App.MyHandle, App.Settings, out AppList outList);
            this.AppList = outList;
        }

        private async void AppList_Changed(object sender, NotifyCollectionChangedEventArgs e)
        {
            await this.Dispatcher.InvokeAsync(() =>
            {
                ((MUI)Application.Current.MainWindow).AppList.UpdateListView();
                ((MUI)Application.Current.MainWindow).AppListView.Items.Refresh();
            });
        }

        private void CredButton_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new CredentialDialog
            {
                Content = "Enter the executing credentials",
                MainInstruction = "Enter the executing credentials",
                ShowSaveCheckBox = true,
                Target = "ThisWindow",
                WindowTitle = "ManagementUI Credentials"
            })
            {
                bool done = dialog.ShowDialog();
                if (done)
                {
                    Creds = new NetworkCredential(dialog.UserName, dialog.Password);
                    if (Creds.UserName.Contains(@"\"))
                    {
                        string[] splitBack = Creds.UserName.Split(
                            new string[1] { @"\" }, StringSplitOptions.RemoveEmptyEntries);
                        Creds.Domain = splitBack.First();
                        Creds.UserName = splitBack.Last();
                    }
                    else if (Creds.UserName.Contains("@"))
                    { 
                        string[] splitAt = Creds.UserName.Split(
                            new string[1] { "@" }, StringSplitOptions.RemoveEmptyEntries);
                        Creds.Domain = splitAt.Last();
                        Creds.UserName = splitAt.First();
                    }
                }
            }
        }

        private async void SettsButton_Click(object sender, RoutedEventArgs e)
        {
            var editor = new SettingsEditor(App.Settings);
            await Task.Run(() =>
            {
                editor.Launch();
                var click = new RoutedEventArgs(Button.ClickEvent);
                this.Dispatcher.Invoke(() =>
                {
                    ((MUI)Application.Current.MainWindow).SettingsUpdateBtn.RaiseEvent(click);
                });
            });
        }

        private void SettingsUpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            string path = Environment.GetEnvironmentVariable("LOCALAPPDATA") + "\\Mike Garvey\\ManagementUI\\settings.json";
            SettingsJson old = App.Settings;
            SettingsJson newJson = SettingsJson.ReadFromFile(path);
            App.Settings = newJson;
        }

        private async void ListViewItem_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is ListViewItem lvi && lvi.DataContext is AppListItem ali)
            {
                await ali.LaunchAsync();
            }
        }
    }
}

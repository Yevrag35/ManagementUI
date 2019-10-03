using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Principal;
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

        internal static bool IsElevated()
        {
            var winId = WindowsIdentity.GetCurrent();
            var prinId = new WindowsPrincipal(winId);
            return prinId.IsInRole(WindowsBuiltInRole.Administrator);
        }

        #region EVENT HANDLERS
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.IdentityBlock.Text = WindowsIdentity.GetCurrent().Name;
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

        private async void CredButton_Click(object sender, RoutedEventArgs e)
        {
            var click = new RoutedEventArgs(Button.ClickEvent);
            if ((string)this.CredsButton.Content == "RUN AS")
            {
                await Task.Run(() =>
                {
                    using (var dialog = new CredentialDialog
                    {
                        MainInstruction = "Relaunch Credentials",
                        Content = "Enter the executing credentials",
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
                            MessageBoxResult answer = MessageBox.Show("Would you like to relaunch as this user?", "RUN AS", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.Cancel);
                            switch (answer)
                            {
                                case MessageBoxResult.Yes:
                                    this.Dispatcher.Invoke(() =>
                                    {
                                        ((MUI)Application.Current.MainWindow).RelaunchBtn.RaiseEvent(click);
                                    });
                                    break;
                                case MessageBoxResult.No:
                                    this.Dispatcher.Invoke(() =>
                                    {
                                        ((MUI)Application.Current.MainWindow).CredsButton.Content = "RELAUNCH";
                                    });
                                    break;

                                default:
                                    break;
                            }
                        }
                    }
                });
            }
            else
            {
                await this.Dispatcher.InvokeAsync(() =>
                {
                    ((MUI)Application.Current.MainWindow).RelaunchBtn.RaiseEvent(click);
                });
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
            if (AppList.Count != newJson.Settings.Icons.Count)
            {
                this.LoadIcons(App.MyHandle, newJson, out AppList list);
                this.AppList = list;
            }
        }

        private async void ListViewItem_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is ListViewItem lvi && lvi.DataContext is AppListItem ali)
            {
                await ali.LaunchAsync();
            }
        }

        private void RelaunchBtn_Click(object sender, RoutedEventArgs e)
        {
            var appId = AppDomain.CurrentDomain;
            string appPath = Path.Combine(appId.BaseDirectory, appId.FriendlyName);
            var psi = new ProcessStartInfo
            {
                FileName = appPath,
                CreateNoWindow = true,
                UseShellExecute = false,
                UserName = Creds.UserName,
                Password = Creds.SecurePassword
            };
            if (!string.IsNullOrEmpty(Creds.Domain))
                psi.Domain = Creds.Domain;

            using (var relaunch = new Process
            {
                StartInfo = psi
            })
            {
                relaunch.Start();
            }
            this.Close();
        }

        #endregion
    }
}

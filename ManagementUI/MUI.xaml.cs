using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.DirectoryServices;
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
        private AppListCollection AppList { get; set; }
        internal IEnumerable<string> AllTags => AppList != null
            ? AppList.Where(x => x.TagList != null).SelectMany(x => x.TagList).Distinct()
            : null;

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
            this.LoadIcons(App.MyHandle, App.JsonSettings, out AppListCollection outList);
            this.AppList = outList;
            this.AppList.CollectionChanged += this.AppList_Changed;
            string[] tags = this.AppList.Tags;
            for (int i = 0; i < tags.Length; i++)
            {
                string tag = tags[i];
                var ft = new FilterTag(tag, false);
                this.FilterTags.Items.Add(ft);
            }
        }

        private async void AppList_Changed(object sender, NotifyCollectionChangedEventArgs e)
        {
            await Task.Run(() =>
            {
                if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    IEnumerable<AppListItem> alis = e.OldItems.Cast<AppListItem>();
                    int index = App.JsonSettings.Settings.Apps
                        .FindIndex(x => alis
                            .Any(ali => ali.AppName.Equals(x.Name)));
                    App.JsonSettings.Settings.Apps.RemoveAt(index);
                    App.JsonSettings.WriteSettings();
                }
            });
            await this.Dispatcher.InvokeAsync(() =>
            {
                ((MUI)Application.Current.MainWindow).AppList.UpdateView();
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
                        while (true)
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

                                // Validate Credentials if Domain is specified.
                                try
                                {
                                    this.VerifyCredentials(Creds);
                                }
                                catch (Exception ex)
                                {
                                    bool res = ShowErrorMessage(ex, true);
                                    if (res)
                                        continue;

                                    else
                                        break;
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

        private void VerifyCredentials(NetworkCredential netCreds)
        {
            if (!string.IsNullOrEmpty(netCreds.Domain))
            {
                using (var de = new DirectoryEntry("LDAP://RootDSE", netCreds.UserName, netCreds.Password, AuthenticationTypes.Secure))
                {
                    de.RefreshCache();
                }
            }
        }

        private async void SettsButton_Click(object sender, RoutedEventArgs e)
        {
            var editor = new SettingsEditor(App.JsonSettings);
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
            SettingsJson old = App.JsonSettings;
            SettingsJson newJson = SettingsJson.ReadFromFile(path);
            App.JsonSettings = newJson;
            if (AppList.Count != newJson.Settings.Apps.Count)
            {
                this.LoadIcons(App.MyHandle, newJson, out AppListCollection list);
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
                LoadUserProfile = true,
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
                try
                {
                    relaunch.Start();
                }
                catch (Exception ex)
                {
                    ShowErrorMessage(ex);
                }
            }
            this.Close();
        }

        #endregion

        private void NewAppButton_Click(object sender, RoutedEventArgs e)
        {
            var newApp = new NewApp
            {
                Owner = this
            };
            bool? result = newApp.ShowDialog();
            if (result.HasValue && result.Value)
            {
                this.WriteAppToFile(newApp.CreatedApp);
                AppListItem ali = newApp.CreatedApp.ToListItem(App.MyHandle);
                AppList.Add(ali);
            }
        }

        private async void WriteAppToFile(AppIconSetting ais)
        {
            await Task.Run(() =>
            {
                App.JsonSettings.Settings.Apps.Add(ais);
                App.JsonSettings.Settings.Apps.Sort(new AppSettingCollection.AppIconSettingDefaultSorter());
                App.JsonSettings.WriteSettings();
            });
        }

        private async void ALMIRemove_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem mi && mi.DataContext is MUI mui &&
                mui.AppListView.SelectedItem is AppListItem ali)
            {
                await this.Dispatcher.InvokeAsync(() =>
                {
                    ((MUI)Application.Current.MainWindow).AppList.Remove(ali);
                });
            }
        }

        private async void AppListView_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Delete)
            {
                var click = new RoutedEventArgs(Button.ClickEvent);
                await this.Dispatcher.InvokeAsync(() =>
                {
                    var ali = ((MUI)Application.Current.MainWindow).AppListView.SelectedItem as AppListItem;
                    ((MUI)Application.Current.MainWindow).AppList.Remove(ali);
                });
            }
        }

        private void AppListView_MouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            int count = this.AppListView.SelectedItems.Count;
            switch (count)
            {
                case 1:
                    this.ALMIRemove.IsEnabled = true;
                    break;

                default:
                    this.ALMIRemove.IsEnabled = false;
                    break;
            }
        }

        
    }
}

using ManagementUI.Auth;
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
        internal static ADCredential Creds { get; set; }
        private AppSettingCollection AppList { get; set; }
        internal IEnumerable<string> AllTags => this.AppList != null
            ? AppList.Where(x => x.Tags != null).SelectMany(x => x.Tags).Distinct()
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

            this.AppList = App.JsonSettings.Settings.Apps;
            this.AppListView.ItemsSource = this.AppList.View;

            //this.LoadIcons(App.JsonSettings, out AppListCollection outList);
            //this.AppList = outList;
            this.AppList.CollectionChanged += this.AppList_Changed;
            string[] tags = this.AppList.Tags;
            for (int i = 0; i < tags.Length; i++)
            {
                string tag = tags[i];
                var ft = new FilterTag(tag, false);
                this.FilterTags.Items.Add(ft);
            }
        }

        private void AppList_Changed(object sender, NotifyCollectionChangedEventArgs e)
        {
            //await Task.Run(() =>
            //{
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                IEnumerable<AppIconSetting> alis = e.OldItems.Cast<AppIconSetting>();
                int removed = App.JsonSettings.Settings.Apps.RemoveAll(app => alis.Contains(app));
            }
            //}).ConfigureAwait(false);
            this.Dispatcher.Invoke(() =>
            {
                ((MUI)Application.Current.MainWindow).AppList.UpdateView();
                ((MUI)Application.Current.MainWindow).AppListView.Items.Refresh();
            });
            App.JsonSettings.Save();
            //await this.Dispatcher.InvokeAsync(() =>
            //{

            //});
        }

        private void CredButton_Click(object sender, RoutedEventArgs e)
        {
            var click = new RoutedEventArgs(Button.ClickEvent);
            if ((string)this.CredsButton.Content == RUN_AS)
            {
                using (CredentialDialog dialog = this.CreateCredentialDialog())
                {
                    bool res = false;
                    do
                    {
                        bool done = dialog.ShowDialog(this);
                        if (done)
                        {
                            Creds = (ADCredential)dialog.Credentials;
                            if (!Creds.TryAuthenticate(out Exception caught))
                            {
                                res = ShowErrorMessage(caught, true);
                                if (res)
                                    continue;

                                else
                                    break;
                            }
                            else
                                res = false;
                            
                            break;
                        }
                    }
                    while (res);

                    this.HandleReprompt(click);
                }
            }
            else
            {
                this.Dispatcher.Invoke(() =>
                {
                    this.RelaunchBtn.RaiseEvent(click);
                });
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
            App.JsonSettings.Read(MG.Settings.Json.SettingChangedAction.Reload);
        }

        private async void ListViewItem_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is ListViewItem lvi && lvi.DataContext is AppIconSetting ali)
            {
                await ali.LaunchAsync();
            }
        }

        private void RelaunchBtn_Click(object sender, RoutedEventArgs e)
        {
            AppDomain appId = AppDomain.CurrentDomain;
            string appPath = Path.Combine(appId.BaseDirectory, appId.FriendlyName);
            ProcessStartInfo psi = Creds.NewStartInfo(appPath);
            //var psi = new ProcessStartInfo
            //{
            //    FileName = appPath,
            //    CreateNoWindow = true,
            //    UseShellExecute = false,
            //    LoadUserProfile = true,
            //    UserName = Creds.UserName,
            //    Password = Creds.SecurePassword
            //};
            //if (!string.IsNullOrEmpty(Creds.Domain))
            //    psi.Domain = Creds.Domain;

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
                //await this.WriteAppToFile(newApp.CreatedApp);
                AppList.Add(newApp.CreatedApp);
            }
        }

        //private async Task WriteAppToFile(AppIconSetting ais)
        //{
        //    await Task.Run(() =>
        //    {
        //        App.JsonSettings.Settings.Apps.Add(ais);
        //        App.JsonSettings.Settings.Apps.Sort(new AppSettingCollection.AppIconSettingDefaultSorter());
        //        App.JsonSettings.Save();
        //    }).ConfigureAwait(false);
        //}

        private async void ALMIRemove_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem mi && mi.DataContext is MUI mui &&
                mui.AppListView.SelectedItem is AppIconSetting ali)
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
                    var ali = ((MUI)Application.Current.MainWindow).AppListView.SelectedItem as AppIconSetting;
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

        private void EditTagsBtn_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem mi && mi.DataContext is MUI mui &&
                mui.AppListView.SelectedItem is AppListItem ali)
            {
                var editTags = new EditTags(ali, this.FilterTags.Items.Cast<FilterTag>())
                {
                    Owner = this
                };
                bool? result = editTags.ShowDialog();
                if (result.HasValue && result.Value)
                {
                    var tempList = this.FilterTags.Items.Cast<FilterTag>().ToList();
                    //if (!tempList.TrueForAll(x => editTags.AllTags.Contains(t => t.Tag.Equals(x.Tag))))
                    if (!editTags.AllTags.TrueForAll(x => tempList.Contains(x)))
                    {
                        for (int i = this.FilterTags.Items.Count - 1; i >= 0; i--)
                        {
                            var ft = (FilterTag)this.FilterTags.Items[i];
                            if (!editTags.AllTags.Contains(t => t.Tag.Equals(ft.Tag)))
                            { 
                                this.FilterTags.Items.Remove(ft);
                                this.FilterTags.Items.Refresh();
                            }
                        }
                        for (int i = 0; i < editTags.AllTags.Count; i++)
                        {
                            var ft2 = editTags.AllTags[i];
                            if (!tempList.Exists(x => x.Tag.Equals(ft2.Tag)))
                            {
                                this.FilterTags.Items.Add(ft2);
                                this.FilterTags.Items.Refresh();
                            }
                        }
                    }

                    ali.Tags = string.Join(", ", ali.TagList);
                    this.Dispatcher.Invoke(() =>
                    {
                        ((MUI)Application.Current.MainWindow).AppListView.Items.Refresh();
                    });
                    App.JsonSettings.Save();
                }
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var list = new List<string>();
            foreach (FilterTag ft in FilterTags.Items)
            {
                if (ft.IsChecked)
                    list.Add(ft.Tag);
            }

            this.AppListView.Items.Filter = x => 
                list.Count <= 0 ||
                    (x is AppIconSetting ali && 
                    ali.Tags != null &&
                    list.TrueForAll(
                        t => ali.Tags.Contains(t)));
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            var list = new List<string>();
            foreach (FilterTag ft in FilterTags.Items)
            {
                if (ft.IsChecked)
                    list.Add(ft.Tag);
            }

            this.AppListView.Items.Filter = x => 
                list.Count <= 0 || (
                    x is AppIconSetting ali &&
                    ali.Tags != null &&
                    list.TrueForAll(
                        t => ali.Tags.Contains(t)));
        }
    }
}

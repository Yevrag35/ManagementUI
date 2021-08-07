using ManagementUI.Auth;
//using ManagementUI.Json.Preferences;
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
using ManagementUI.Functionality.Executable;
using ManagementUI.Functionality.Settings;
using ManagementUI.Collections;
using ManagementUI.Models;

namespace ManagementUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MUI : Window
    {
        private static HashSet<string> Checked;
        private FilterTagEquality _ftEquality;

        internal static ADCredential Creds { get; set; }
        //private AppSettingCollection AppList { get; set; }
        //private AppListViewCollection AppList { get; set; }
        private AppsList AppList { get; set; }
        //internal IEnumerable<string> AllTags => this.AppList?.Tags;

        public MUI()
        {
            _ftEquality = new FilterTagEquality();
            //new PreferencesModel();
            InitializeComponent();
            App.JsonSettings.EditorManager.EditorExited += this.Editor_Closed;
        }

        private async Task OnLoad()
        {
            await this.ReadApps(JsonAppsFile.GetFullPath());

        }

        internal static bool IsElevated()
        {
            var winId = WindowsIdentity.GetCurrent();
            var prinId = new WindowsPrincipal(winId);
            return prinId.IsInRole(WindowsBuiltInRole.Administrator);
        }

        #region CHECKBOX FILTER
        private async Task ApplyCheckBoxFilter()
        {
            await this.Dispatcher.InvokeAsync(() =>
            {
                //foreach (AppIconSetting ais in this.AppList)
                //{
                //    if (!ais.Tags.IsSupersetOf(Checked))
                //        ais.IsChecked = false;

                //    else
                //        ais.IsChecked = true;
                //}
            });
        }

        #endregion

        #region EVENT HANDLERS
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.IdentityBlock.Text = WindowsIdentity.GetCurrent().Name;
            App.MyHandle = new WindowInteropHelper(this).Handle;

            //this.AppList = App.JsonSettings.Settings.Apps;
            await this.OnLoad();
            Checked = new HashSet<string>(1);
            this.AppListView.ItemsSource = this.AppList?.View;
            //this.FilterTags.ItemsSource = this.AppList?.Tags.View;

            //if (this.AppList.Count > 0)
            //{
            //    this.AppList.CollectionChanged += this.AppList_Changed;
            //}
        }

        private void AppList_Changed(object sender, NotifyCollectionChangedEventArgs e)
        {
            //this.AppList.RefreshAll();
            //    //await Task.Run(() =>
            //    //{
            //    if (e.Action == NotifyCollectionChangedAction.Remove)
            //    {
            //        IEnumerable<AppIconSetting> alis = e.OldItems.Cast<AppIconSetting>();
            //        int removed = App.JsonSettings.Settings.Apps.RemoveAll(app => alis.Contains(app));
            //    }
            //    //}).ConfigureAwait(false);
            //    this.Dispatcher.Invoke(() =>
            //    {
            //        ((MUI)Application.Current.MainWindow).AppList.UpdateView();
            //        ((MUI)Application.Current.MainWindow).AppListView.Items.Refresh();
            //    });
            //    App.JsonSettings.Save();
            //    //await this.Dispatcher.InvokeAsync(() =>
            //    //{

            //    //});
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

        private void SettsButton_Click(object sender, RoutedEventArgs e)
        {
            string key = App.JsonSettings.Editor.ToString();
            var editor = App.JsonSettings.EditorManager[key];
            if (!editor.IsUsable())
            {
                editor = App.JsonSettings.EditorManager.FirstOrDefault(x => x.Value.IsUsable()).Value;
                if (null == editor)
                {
                    ShowErrorMessage(new InvalidOperationException("No openable editors :("));
                    return;
                }
            }
            App.JsonSettings.EditorManager.Start(key, IsElevated());

            //await this.Dispatcher.InvokeAsync(() =>
            //{
                
            //    //editor.Start(IsElevated(), false);
            //});
            //var editor = new SettingsEditor(App.JsonSettings);
            //await Task.Run(() =>
            //{
            //    editor.Launch();
            //    var click = new RoutedEventArgs(Button.ClickEvent);
            //    this.Dispatcher.Invoke(() =>
            //    {
            //        ((MUI)Application.Current.MainWindow).SettingsUpdateBtn.RaiseEvent(click);
            //    });
            //});
        }

        private async void Editor_Closed(object sender, EditorEventArgs e)
        {
            await this.Dispatcher.InvokeAsync(() =>
            {
                
            });
        }

        private void SettingsUpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            //App.JsonSettings.Read(MG.Settings.Json.SettingChangedAction.Reload);
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
            ProcessStartInfo psi = StartInfoFactory.Create(appPath, false, false, Creds);
            psi.LoadUserProfile = true;

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
                //AppList.Add(newApp.CreatedApp);
            }
        }

        private async void ALMIRemove_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem mi && mi.DataContext is MUI mui &&
                mui.AppListView.SelectedItem is AppIconSetting ali)
            {
                await this.Dispatcher.InvokeAsync(() =>
                {
                    //((MUI)Application.Current.MainWindow).AppList.Remove(ali);
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
                    //((MUI)Application.Current.MainWindow).AppList.Remove(ali);
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
            //if (sender is MenuItem mi && mi.DataContext is MUI mui &&
            //    mui.AppListView.SelectedItem is AppIconSetting ais)
            //{
            //    var editTags = new EditTags(ais, this.AppList.Tags)
            //    {
            //        Owner = this
            //    };
            //    bool? result = editTags.ShowDialog();
            //    this.HandleOkEdit(result.GetValueOrDefault(), editTags.Application);
            //}
        }

        // Handle Edit OK
        private async Task HandleOkEdit(bool result, AppIconSetting modifiedApp)
        {
            //if (result)
            //{
            //    if (!modifiedApp.Tags.IsSubsetOf(this.AppList.Tags.GetTagsAsStrings()))
            //    {
            //        await this.AddTagsToTagList(modifiedApp.Tags, this.AppList.Tags);
            //    }
            //    await this.RemoveAnyTagsFromTagList(this.AppList);

            //    await this.ApplyCheckBoxFilter();

            //    await this.Dispatcher.InvokeAsync(() =>
            //    {
            //        this.AppList.View.Refresh();
            //    });
            //}
        }

        private Task AddTagsToTagList(ISet<string> containingAddedTags, TagList tagList)
        {
            return this.Dispatcher.InvokeAsync(() =>
            {
                //tagList.AddMany(containingAddedTags.Where(x => !this.AppList.Tags.ContainsKey(x)).Select(ft => new FilterTag(ft)));
                //tagList.View.Refresh();
                //foreach (FilterTag ft in tagList.Where(x => Checked.Contains(x.Tag)))
                //{
                //    ft.IsChecked = true;
                //}
            }).Task;
        }
        private async Task RemoveAnyTagsFromTagList(AppListViewCollection appList)
        {
            HashSet<FilterTag> allTags = appList.GetAllTagsAsSet(_ftEquality);
            if (allTags.IsProperSubsetOf(appList.Tags))
            {
                await this.Dispatcher.InvokeAsync(() =>
                {
                    appList.Tags.Reset(allTags);
                });

                string[] tagsAsStrings = appList.Tags.GetTagsAsStrings();
                if (!Checked.IsSubsetOf(tagsAsStrings))
                {
                    Checked.IntersectWith(tagsAsStrings);
                }

                await this.Dispatcher.InvokeAsync(() =>
                {
                    appList.Tags.View.Refresh();
                    foreach (FilterTag ft in appList.Tags.Where(x => Checked.Contains(x.Tag)))
                    {
                        ft.IsChecked = true;
                    }
                });
            }
        }

        #region CHECKBOX EVENTS
        private async void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Checked.Add(((e.Source as CheckBox).DataContext as FilterTag).Tag);
            await this.ApplyCheckBoxFilter();
        }

        private async void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Checked.Remove(((e.Source as CheckBox).DataContext as FilterTag).Tag);
            await this.ApplyCheckBoxFilter();
        }

        #endregion
    }
}

﻿using Ookii.Dialogs.Wpf;
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
using ManagementUI.Functionality.Auth;
using ManagementUI.Functionality.Executable;
using ManagementUI.Functionality.Models;
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
        #region PROPERTIES/FIELDS
        //private static HashSet<string> Checked;
        private FilterTagEquality _ftEquality;
        private bool _overItem;

        private AppsList AppList => this.JsonAppsRead.Apps;
        //internal static ADCredential Creds { get; set; }
        //private UserIdentity Creds { get; set; }
        private JsonAppsFile JsonAppsRead { get; set; }
        private SettingsJson Settings { get; set; }
        private TagCollection Tags { get; set; }

        #endregion

        public MUI()
        {
            this.ReadSettings();
            LaunchFactory.Initialize();
            _ftEquality = new FilterTagEquality();
            //new PreferencesModel();
            this.InitializeComponent();
            this.Settings.EditorManager.EditorExited += this.Editor_Closed;
        }

        private Task OnLoad()
        {
            return this.ReadApps();
        }

        #region EVENT HANDLERS
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.IdentityBlock.Text = WindowsIdentity.GetCurrent().Name;
            App.MyHandle = new WindowInteropHelper(this).Handle;

            await this.OnLoad();
            //Checked = new HashSet<string>(1);
            await this.Dispatcher.InvokeAsync(() =>
            {

                this.AppListView.ItemsSource = this.AppList.View;
                this.FilterTags.ItemsSource = this.Tags.View;
            });
            
        }

        private async void CredButton_Click(object sender, RoutedEventArgs e)
        {
            await this.Dispatcher.InvokeAsync(() =>
            {
                using (var box = new CredentialBox())
                {
                    if (box.ShowDialog())
                    {
                        IUserIdentity userId = new UserIdentity(box.UserName, box.GetPassword());
                        if (userId.IsValid())
                        {
                            LaunchFactory.AddCredentials(userId);
                        }
                        else
                        {
                            userId.Dispose();
                            ShowErrorMessage(new InvalidCredentialException(userId.UserName, userId.Domain, null));
                        }
                    }
                }
            });
            
            //var click = new RoutedEventArgs(Button.ClickEvent);
            //if ((string)this.CredsButton.Content == RUN_AS)
            //{
            //    using (CredentialDialog dialog = this.CreateCredentialDialog())
            //    {
            //        bool res = false;
            //        do
            //        {
            //            bool done = dialog.ShowDialog(this);
            //            if (done)
            //            {
            //                Creds = (ADCredential)dialog.Credentials;
            //                if (!Creds.TryAuthenticate(out Exception caught))
            //                {
            //                    res = ShowErrorMessage(caught, true);
            //                    if (res)
            //                        continue;

            //                    else
            //                        break;
            //                }
            //                else
            //                    res = false;
                            
            //                break;
            //            }
            //        }
            //        while (res);

            //        this.HandleReprompt(click);
            //    }
            //}
            //else
            //{
            //    this.Dispatcher.Invoke(() =>
            //    {
            //        this.RelaunchBtn.RaiseEvent(click);
            //    });
            //}
        }

        private async void SettsButton_Click(object sender, RoutedEventArgs e)
        {
            await this.Dispatcher.InvokeAsync(() =>
            {
                string key = this.Settings.Editor.ToString();
                var editor = this.Settings.EditorManager[key];
                if (!editor.IsUsable())
                {
                    editor = this.Settings.EditorManager.FirstOrDefault(x => x.Value.IsUsable()).Value;
                    if (null == editor)
                    {
                        ShowErrorMessage(new InvalidOperationException("No openable editors :("));
                        return;
                    }
                }

                LaunchFactory.ExecuteEditor(this.Settings.EditorManager, key);
            });
        }

        private async void Editor_Closed(object sender, EditorEventArgs e)
        {
            await this.Dispatcher.InvokeAsync(() =>
            {
                this.Settings.EditorManager.Dispose();
            });

            await this.ReadSettingsAsync();
            this.Settings.EditorManager.EditorExited += this.Editor_Closed;
        }

        private async void ListViewItem_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //if (sender is ListViewItem lvi && lvi.DataContext is AppIconSetting ali)
            //{
            //    await ali.LaunchAsync();
            //}
        }

        //private void RelaunchBtn_Click(object sender, RoutedEventArgs e)
        //{
        //    //AppDomain appId = AppDomain.CurrentDomain;
        //    //string appPath = Path.Combine(appId.BaseDirectory, appId.FriendlyName);
        //    //ProcessStartInfo psi = StartInfoFactory.Create(appPath, false, false, Creds);
        //    //psi.LoadUserProfile = true;

        //    //using (var relaunch = new Process
        //    //{
        //    //    StartInfo = psi
        //    //})
        //    //{
        //    //    try
        //    //    {
        //    //        relaunch.Start();
        //    //    }
        //    //    catch (Exception ex)
        //    //    {
        //    //        ShowErrorMessage(ex);
        //    //    }
        //    //}

        //    //this.Close();
        //}

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
                Console.WriteLine("hey");
                //await this.WriteAppToFile(newApp.CreatedApp);
                //AppList.Add(newApp.CreatedApp);
            }
        }

        private void ALMIRemove_Click(object sender, RoutedEventArgs e)
        {
            //if (sender is MenuItem mi && mi.DataContext is MUI mui &&
            //    mui.AppListView.SelectedItem is AppIconSetting ali)
            //{
            //    await this.Dispatcher.InvokeAsync(() =>
            //    {
            //        //((MUI)Application.Current.MainWindow).AppList.Remove(ali);
            //    });
            //}
        }

        private void AppListView_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            //if (e.Key == System.Windows.Input.Key.Delete)
            //{
            //    var click = new RoutedEventArgs(Button.ClickEvent);
            //    await this.Dispatcher.InvokeAsync(() =>
            //    {
            //        var ali = ((MUI)Application.Current.MainWindow).AppListView.SelectedItem as AppIconSetting;
            //        //((MUI)Application.Current.MainWindow).AppList.Remove(ali);
            //    });
            //}
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

        private async void EditTagsBtn_Click(object sender, RoutedEventArgs e)
        {
            await this.Dispatcher.InvokeAsync(() =>
            {
                if (this.AppListView.SelectedItem is AppItem ai)
                {
                    var editTags = new EditTags(ai, this.Tags.ToEditCollection())
                    {
                        Owner = this
                    };
                    if (editTags.ShowDialog().GetValueOrDefault())
                    {
                        if (!this.Tags.SetEquals(editTags.Tags))
                        {
                            this.Tags.UnionWith(editTags.Tags);
                        }

                        if (this.Tags.EnabledCount > 0)
                        {
                            if (!ai.DontShow && !ai.Tags.IsSupersetOf(this.Tags.EnabledTags))
                                ai.DontShow = true;

                            else if (ai.DontShow && ai.Tags.IsSupersetOf(this.Tags.EnabledTags))
                                ai.DontShow = false;
                        }
                    }
                }
            });
        }

        #region CHECKBOX EVENTS
        private async void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            await this.Dispatcher.InvokeAsync(() =>
            {
                if (sender is CheckBox cb && cb.DataContext is ToggleTag tag)
                {
                    _ = this.Tags.Enable(tag);
                    this.AppList.EnableByTags(this.Tags.EnabledTags);
                }
            });
        }

        private async void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            await this.Dispatcher.InvokeAsync(() =>
            {
                if (sender is CheckBox cb && cb.DataContext is ToggleTag tag)
                {
                    int enabledCount = this.Tags.Disable(tag);
                    if (enabledCount > 0)
                    {
                        this.AppList.EnableByTags(this.Tags.EnabledTags);
                    }
                    else
                    {
                        this.AppList.ResetItems();
                    }
                }
            });
        }

        #endregion

        private void ContextMenu_PreviewMouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!_overItem)
            {
                e.Handled = true;
            }
        }
        private async void ListViewItem_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            await this.Dispatcher.InvokeAsync(() =>
            {
                _overItem = true;

            }, System.Windows.Threading.DispatcherPriority.Send);
        }
        private async void ListViewItem_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            await this.Dispatcher.InvokeAsync(() =>
            {
                _overItem = false;

            }, System.Windows.Threading.DispatcherPriority.Send);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            LaunchFactory.Deinitialize();
        }

        private async void RemoveTag_Click(object sender, RoutedEventArgs e)
        {
            if (this.FilterTags.SelectedItems.Count > 0)
            {
                

                await this.Dispatcher.InvokeAsync(() =>
                {
                    ToggleTag[] tags = this.FilterTags.SelectedItems.Cast<ToggleTag>().ToArray();
                    this.Tags.ExceptWith(tags);

                    foreach (AppItem ai in this.AppList)
                    {
                        ai.Tags.ExceptWith(tags.Select(x => x.UserTag));
                    }
                });
            }
        }

        //private async void FilterTags_LostKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        //{
            
        //}

        private async void FilterTags_LostFocus(object sender, RoutedEventArgs e)
        {
            await this.Dispatcher.InvokeAsync(() =>
            {
                if (!this.FilterTags.IsMouseOver && this.FilterTags.SelectedItems.Count > 0)
                {
                    this.FilterTags.UnselectAll();
                }
            });
        }
    }
}

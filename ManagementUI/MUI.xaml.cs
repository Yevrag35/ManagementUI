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
using ManagementUI.Functionality.Auth;
using ManagementUI.Functionality.Executable;
using ManagementUI.Functionality.Models;
using ManagementUI.Functionality.Settings;
using ManagementUI.Collections;
using ManagementUI.Models;
using ManagementUI.Prompts;

using Strings = ManagementUI.Properties.Resources;

namespace ManagementUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MUI : Window
    {
        #region PROPERTIES/FIELDS
        private bool _overItem;

        private AppsList AppList => this.JsonAppsRead.Apps;
        private JsonAppsFile JsonAppsRead { get; set; }
        public static RunAsDisplay RunAsUser { get; set; }
        private SettingsJson Settings { get; set; }
        private TagCollection Tags { get; set; }

        #endregion

        public MUI()
        {
            RunAsUser = new RunAsDisplay();
            this.ReadSettings();
            

            this.InitializeComponent();
            this.Settings.EditorManager.EditorExited += this.Editor_Closed;
            LaunchFactory.Initialize(this.Settings.EditorManager);
        }

        private Task OnLoad()
        {
            return this.ReadAppsAsync();
        }

        #region EVENT HANDLERS
        private void IdentityBlock_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.IdentityBlock.SelectAll();
            e.Handled = true;
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            LaunchFactory.Deinitialize();
        }
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            App.MyHandle = new WindowInteropHelper(this).Handle;

            await this.OnLoad();

            await this.Dispatcher.InvokeAsync(() =>
            {
                this.AppListView.ItemsSource = this.AppList.View;
                this.FilterTags.ItemsSource = this.Tags.View;
            });
            
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
                Console.WriteLine("hey");
                //await this.WriteAppToFile(newApp.CreatedApp);
                //AppList.Add(newApp.CreatedApp);
            }
        }
        private async void ALMIRemove_Click(object sender, RoutedEventArgs e)
        {
            await this.Dispatcher.InvokeAsync(() =>
            {
                if (this.AppListView.SelectedItem is AppItem ai)
                {
                    if (PromptFactory.DoYesNoPrompt(this, Strings.Prompt_AppDeleteTitle, TaskDialogIcon.Warning, true, 
                        (dialog) =>
                        {
                            dialog.MainInstruction = Strings.Prompt_AppDeleteMainInstruction;
                            dialog.AddContent(Strings.Prompt_AppDeleteContent, ai.Name, Environment.NewLine);
                        }))
                    {
                        _ = this.AppList.Remove(ai);
                    }
                }
            });
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

        #region CONTEXT MENU

        private void ContextMenu_PreviewMouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!_overItem)
            {
                e.Handled = true;
                return;
            }

            this.Dispatcher.Invoke(() =>
            {
                if (sender is ListView lv && nameof(AppListView) == lv.Name
                && lv.SelectedItems.Count == 1)
                {
                    this.ALMIRemove.IsEnabled = true;
                }
                else
                {
                    this.ALMIRemove.IsEnabled = false;
                }
            });
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

        #endregion

        #region CREDENTIALS
        private async void CredButton_Click(object sender, RoutedEventArgs e)
        {
            await this.Dispatcher.InvokeAsync(() =>
            {
                using (var box = new CredentialBox())
                {
                    if (box.ShowDialog())
                    {
                        UserIdentity userId = new UserIdentity(box.UserName, box.GetPassword());
                        if (userId.IsValid())
                        {
                            LaunchFactory.AddCredentials(userId);
                            RunAsUser.ApplyFromCreds(userId);
                        }
                        else
                        {
                            userId.Dispose();
                            ShowErrorMessage(new InvalidCredentialException(userId.UserName, userId.Domain, null));
                        }
                    }
                }

            });

            await this.Dispatcher.InvokeAsync(() =>
            {

            });
        }

        #endregion

        #region EDIT TAGS
        private async void EditTagsBtn_Click(object sender, RoutedEventArgs e)
        {
            if (this.AppListView.SelectedItem is AppItem ai)
            {
                await this.EditAppTags(ai);
            }
        }

        private async Task EditAppTags(AppItem ai)
        {
            await this.Dispatcher.InvokeAsync(() =>
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
            });
        }

        private async void RemoveTag_Click(object sender, RoutedEventArgs e)
        {
            if (this.FilterTags.SelectedItems.Count > 0)
            {
                await this.Dispatcher.InvokeAsync(() =>
                {
                    ToggleTag[] tags = this.FilterTags.SelectedItems.Get<ToggleTag>();
                    string tagNames = string.Join(Environment.NewLine,
                        tags.Select(x => x.Value).OrderBy(x => x));

                    if (PromptFactory.DoYesNoPrompt(this, Strings.Prompt_TagDeleteTitle, TaskDialogIcon.Warning, true, 
                        (dialog) =>
                        {
                            _ = dialog
                                .AddInstruction(Strings.Prompt_TagDeleteMainInstruction)
                                    .AddContent(Strings.Prompt_TagDeleteContent, tagNames, Environment.NewLine);
                        }))
                    {
                        this.Tags.ExceptWith(tags);
                        UserTag[] uTags = tags.ToArray(x => x.UserTag);

                        this.AppList.ForEach((app) =>
                        {
                            app.Tags.ExceptWith(uTags);
                        });
                    }
                });
            }
        }

        #endregion

        #region LAUNCH APP
        private async void ListViewItem_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            await this.Dispatcher.InvokeAsync(() =>
            {
                if (sender is ListViewItem lvi && lvi.DataContext is AppItem ai)
                {
                    if (!LaunchFactory.Execute(ai, out Exception caughtException))
                    {
                        ShowErrorMessage(caughtException, false);
                    }
                }
            });
        }

        #endregion

        #region RELAUNCH
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

        #region SETTINGS
        private async void Editor_Closed(object sender, EditorEventArgs e)
        {
            await this.Dispatcher.InvokeAsync(() =>
            {
                this.Settings.EditorManager.Dispose();
            });

            await this.ReadSettingsAsync();
            this.Settings.EditorManager.EditorExited += this.Editor_Closed;
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

        #endregion

        
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

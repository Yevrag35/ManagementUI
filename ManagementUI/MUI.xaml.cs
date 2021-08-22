using Ookii.Dialogs.Wpf;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Threading;
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
    public partial class MUI : Window, INotifyPropertyChanged
    {
        #region PROPERTIES/FIELDS
        private bool _overItem;
        private RunAsDisplay _runAs;

        public event PropertyChangedEventHandler PropertyChanged;

        public string AppColumnHeaderName => Strings.ColumnHeaderName_Apps;
        private AppsList AppList => this.JsonAppsRead.Apps;
        private JsonAppsFile JsonAppsRead { get; set; }
        public string RunAsUser => _runAs?.DisplayPrincipal;
        private SettingsJson Settings { get; set; }
        private TagCollection Tags { get; set; }

        #endregion

        public MUI()
        {
            this.ReadSettings();

            this.InitializeComponent();
            this.Settings.EditorManager.EditorExited += this.Editor_Closed;
            LaunchFactory.Initialize(this.Settings.EditorManager);
            this.Set(nameof(this.RunAsUser), new RunAsDisplay(), (rad) => _runAs = rad);
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
            if (null == this.AppColumnHeaderName)
                throw new InvalidOperationException(string.Format("{0} cannot be null.  Exiting application.", nameof(this.AppColumnHeaderName)));

            App.MyHandle = new WindowInteropHelper(this).Handle;

            await this.OnLoad();

            await this.Dispatcher.InvokeAsync(() =>
            {
                this.AppListView.ItemsSource = this.AppList.View;
                this.FilterTags.ItemsSource = this.Tags.View;
            });
            
        }

        #endregion

        private async Task AddAppToList(NewApp window)
        {
            await this.Dispatcher.InvokeAsync(() =>
            {
                AppItem app = window.GetFinalizedApp();
                this.AppList.Add(app);
            });

            await this.SaveApps();
        }
        private async void NewAppButton_Click(object sender, RoutedEventArgs e)
        {
            var newApp = new NewApp(App.MyHandle)
            {
                Owner = this
            };
            bool? result = newApp.ShowDialog();
            if (result.GetValueOrDefault())
            {
                await this.AddAppToList(newApp);
            }
        }
        private async void EditAppBtn_Click(object sender, RoutedEventArgs e)
        {
            if (this.AppListView.SelectedItem is AppItem ai)
            {
                var newApp = new NewApp(App.MyHandle, ai)
                {
                    Owner = this
                };

                if (newApp.ShowDialog().GetValueOrDefault())
                {
                    _ = newApp.GetFinalizedApp();
                    await this.SaveApps();
                }
            }
        }
        private async void ALMIRemove_Click(object sender, RoutedEventArgs e)
        {
            bool result = await this.Dispatcher.InvokeAsync(() =>
            {
                bool resultInner = false;
                if (this.AppListView.SelectedItem is AppItem ai)
                {
                    if (PromptFactory.DoYesNoPrompt(this, Strings.Prompt_AppDeleteTitle, TaskDialogIcon.Warning, true, 
                        (dialog) =>
                        {
                            dialog.MainInstruction = Strings.Prompt_AppDeleteMainInstruction;
                            _ = dialog.AddContent(Strings.Prompt_AppDeleteContent, ai.Name, Environment.NewLine);
                        }))
                    {
                        resultInner = this.AppList.Remove(ai);
                    }
                }

                return resultInner;
            });

            if (result)
                await this.SaveApps();
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
        private async void ShouldRightClickOnMouseEnterOrLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            await this.Dispatcher.InvokeAsync(() =>
            {
                _overItem = IsOverItem(e?.Device?.Target, this.AppColumnHeaderName);

            }, DispatcherPriority.Send);
        }
        private static bool IsOverItem(IInputElement element, string appColumnName)
        {
            return
                null != element
                &&
                (
                    (
                        element is Border border
                        &&
                        (
                            border.DataContext is AppItem
                            ||
                            border.DataContext is ToggleTag
                        )
                    )
                    ||
                    (
                        element is TextBlock tb
                        &&
                        Strings.ContentHeaderUid.Equals(tb?.Name)
                        &&
                        appColumnName.Equals(tb.Text)
                    )
                );
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
                        var userId = new UserIdentity(box.UserName, box.GetPassword());
                        if (userId.IsValid())
                        {
                            LaunchFactory.AddCredentials(userId);
                            this.SetRunAsUser(userId);
                        }
                        else
                        {
                            userId.Dispose();
                            ShowErrorMessage(new InvalidCredentialException(userId.UserName, userId.Domain, null));
                        }
                    }
                }
            });
        }

        private void SetRunAsUser(IUserIdentity identity)
        {
            this.Set(nameof(this.RunAsUser), identity, (id) => _runAs.ApplyFromCreds(id));
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
            var editTags = new EditTags(ai, this.Tags.ToEditCollection())
            {
                Owner = this
            };

            if (editTags.ShowDialog().GetValueOrDefault())
            {
                Task saveTask = null;
                if (editTags.IsModified)
                {
                    saveTask = this.SaveApps();
                }

                await this.Dispatcher.InvokeAsync(() =>
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
                });

                await saveTask;
            }
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
                        _ = ShowErrorMessage(caughtException, false);
                    }
                }
            });
        }

        #endregion

        #region RELAUNCH - OBSOLETE
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
                string key = this.Settings.Editor;
                IEditor editor = this.Settings.EditorManager[key];
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

        private void GridViewColumnHeader_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is GridViewColumnHeader header && null == header.Content)
            {
                e.Handled = true;
                return;
            }
        }
        private async void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            await this.Dispatcher.InvokeAsync(() =>
            {
                this.AppList.ChangeSortOrder();
            });
        }

        private async void ResetSortOrder_Click(object sender, RoutedEventArgs e)
        {
            await this.Dispatcher.InvokeAsync(() =>
            {
                this.AppList.View.SortDescriptions.Clear();
            });
        }
    }
}

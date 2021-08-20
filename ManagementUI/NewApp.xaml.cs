using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using ManagementUI.Functionality.Events;
using ManagementUI.Models;

using Strings = ManagementUI.Properties.Resources;
using IOPath = System.IO.Path;

namespace ManagementUI
{
    /// <summary>
    /// Interaction logic for NewApp.xaml
    /// </summary>
    public partial class NewApp : Window, INotifyPropertyChanged
    {
        private AppItem _editItem;
        public event PropertyChangedEventHandler PropertyChanged;

        public AppItem CreatedApp { get; private set; }
        private bool IsEditOperation => null != _editItem;
        public string Arguments
        {
            get => this.CreatedApp.Arguments;
            set => this.CreatedApp.Arguments = value;
        }
        public string DisplayName
        {
            get => this.CreatedApp.Name;
            set => this.CreatedApp.Name = value;
        }
        public string ExePath
        {
            get => !string.IsNullOrEmpty(this.CreatedApp.ExePath)
                ? IOPath.GetFileName(this.CreatedApp.ExePath)
                : Strings.NewApp_DefaultButtonContent;
        }
        public uint IconIndex
        {
            get => this.CreatedApp.IconIndex;
            set => this.CreatedApp.IconIndex = value;
        }
        public string IconPath
        {
            get => !string.IsNullOrEmpty(this.CreatedApp.IconPath)
                ? IOPath.GetFileName(this.CreatedApp.IconPath)
                : Strings.NewApp_DefaultButtonContent;
        }
        public IconPreviewer IconPreviewer { get; }
        public BitmapSource Image
        {
            get => this.CreatedApp.Image;
            set
            {
                if (null == value)
                    return;

                if (value.CanFreeze && !value.IsFrozen)
                    value.Freeze();

                this.CreatedApp.Image = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Image)));
            }
        }

        #region CONSTRUCTORS
        public NewApp(IntPtr appHandle)
        {
            this.IconPreviewer = new IconPreviewer(appHandle);
            this.SetApp(null);
            this.InitializeComponent();
        }
        public NewApp(IntPtr appHandle, AppItem appToEdit)
        {
            this.IconPreviewer = new IconPreviewer(appHandle);
            this.SetApp(appToEdit);
            this.InitializeComponent();

            this.Title = string.Format(Strings.NewApp_Title_EDIT, appToEdit.Name);
            this.createBtn.Content = Strings.NewApp_BtnContent_EDIT;
            _ = this.NotifyProperties(nameof(this.Arguments), nameof(this.ExePath), nameof(this.IconIndex),
                nameof(this.IconPath));
        }

        #endregion

        #region WINDOW EVENTS
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await this.Dispatcher.InvokeAsync(() =>
            {
                if (!this.IsEditOperation)
                    _ = this.displayNameBox.Focus();
            });
        }

        #endregion

        public void SetApp(AppItem editApp)
        {
            if (null != editApp)
            {
                _editItem = editApp;
                this.CreatedApp = _editItem.Clone();
            }
            else
            {
                this.CreatedApp = new AppItem();
            }

            this.CreatedApp.IconIndexUpdated += this.CreatedApp_IconIndexUpdated;
        }

        private async void CreatedApp_IconIndexUpdated(object sender, IconChangedEventArgs e)
        {
            if (e.NewIndex.HasValue)
            {
                await this.OnFileDialogOkAsync(this.CreatedApp.IconPath, e.NewIndex.Value);
            }
        }

        public AppItem GetFinalizedApp()
        {
            AppItem appItem = null;
            if (!this.IsEditOperation)
            {
                appItem = this.CreatedApp.Clone();
            }
            else
            {
                _editItem.MergeFrom(this.CreatedApp);
                appItem = _editItem;
            }

            return appItem;
        }
        private async Task NotifyProperties(params string[] names)
        {
            await this.Dispatcher.InvokeAsync(() =>
            {
                Array.ForEach(names, (n) =>
                {
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
                });
            });
        }
        //private static bool NotNullEmptyOrBrowse(string str)
        //{
        //    return !string.IsNullOrEmpty(str) && !Strings.NewApp_DefaultButtonContent.Equals(str, StringComparison.CurrentCultureIgnoreCase);
        //}
        private DispatcherOperation OnFileDialogOkAsync(string iconPath, uint iconIndex)
        {
            return this.Dispatcher.InvokeAsync(() =>
            {
                this.Image = this.IconPreviewer.Preview(iconPath, iconIndex);
            });
        }

        #region TEXTBOX EVENTS
        private void TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (sender is TextBox tb && !string.IsNullOrEmpty(tb.Text))
                tb.SelectAll();
        }
        private void TextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBox tb && !tb.IsKeyboardFocusWithin)
            {
                e.Handled = true;
                tb.Focus();
            }
        }

        #endregion

        #region BUTTON EVENTS

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            base.DialogResult = false;
            this.Close();
        }
        private void CreateBtn_Click(object sender, RoutedEventArgs e)
        {
            base.DialogResult = true;
            this.Close();
        }
        private async void FindExeBtn_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new VistaOpenFileDialog
            {
                AddExtension = true,
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = "exe",
                Filter = "EXE Programs (*.exe)|*.exe",
                Multiselect = false,
                InitialDirectory = string.IsNullOrEmpty(this.CreatedApp.ExePath)
                    ? Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                    : IOPath.GetDirectoryName(this.CreatedApp.ExePath),
                RestoreDirectory = false,
                Title = "Choose a program"
            };

            bool? result = fileDialog.ShowDialog();
            if (result.GetValueOrDefault())
            {
                DispatcherOperation previewTask = this.OnFileDialogOkAsync(fileDialog.FileName, 0);
                DispatcherOperation iconTask = this.SetIcon(fileDialog.FileName);
                await this.SetProgramFileName(fileDialog.FileName);

                await iconTask;
                await previewTask;
            }
        }
        private async void FindIconBtn_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new VistaOpenFileDialog
            {
                AddExtension = true,
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = "exe",
                Filter = "Windows Icons (*.ico;*.exe;*.dll)|*.ico;*.exe;*.dll|Icon files (*.ico)|*.ico|EXE Programs (*.exe)|*.exe|DLL files (*.dll)|*.dll",
                Multiselect = false,
                InitialDirectory = string.IsNullOrWhiteSpace(this.IconPath)
                    ? Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                    : IOPath.GetDirectoryName(this.CreatedApp.IconPath),
                RestoreDirectory = true,
                Title = "Choose a icon file or one contained in a program"
            };
            bool? result = fileDialog.ShowDialog();
            if (result.GetValueOrDefault())
            {
                DispatcherOperation previewTask = this.OnFileDialogOkAsync(fileDialog.FileName, this.IconIndex);
                await this.SetIcon(fileDialog.FileName);
                await previewTask;
            }
        }

        private DispatcherOperation SetProgramFileName(string filePath)
        {
            return this.Dispatcher.InvokeAsync(() =>
            {
                this.Set(nameof(this.ExePath), filePath, (fp) => this.CreatedApp.ExePath = fp);
                //this.findExeBtn.IsEnabled = false;
                this.findExeBtn.Content = IOPath.GetFileName(filePath);
                if (string.IsNullOrWhiteSpace(this.DisplayName))
                {
                    this.Set(nameof(this.DisplayName), FileVersionInfo.GetVersionInfo(filePath).FileDescription,
                        (x) => this.CreatedApp.Name = x);
                }
            });
        }

        private DispatcherOperation SetIcon(string iconPath)
        {
            return this.Dispatcher.InvokeAsync(() =>
            {
                this.Set(nameof(this.IconPath), iconPath, (ip) => this.CreatedApp.IconPath = ip);
                this.findIconBtn.Content = IOPath.GetFileName(iconPath);
                if (this.IconIndex != 0u)
                    this.Set(nameof(this.IconIndex), 0u, (ii) => this.CreatedApp.IconIndex = ii);
            });
        }
        private DispatcherOperation SetIcon(string iconPath, uint iconIndex)
        {
            return this.Dispatcher.InvokeAsync(() =>
            {
                this.Set(nameof(this.IconPath), iconPath, (ip) => this.CreatedApp.IconPath = ip);
                this.findIconBtn.Content = IOPath.GetFileName(iconPath);
                this.Set(nameof(this.IconIndex), iconIndex, (ii) => this.CreatedApp.IconIndex = ii);
            });
        }

        #endregion
    }
}
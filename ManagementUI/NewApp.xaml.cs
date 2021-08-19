using ManagementUI.Models;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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

using IOPath = System.IO.Path;

namespace ManagementUI
{
    /// <summary>
    /// Interaction logic for NewApp.xaml
    /// </summary>
    public partial class NewApp : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public AppItem CreatedApp { get; set; }
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
            get => IOPath.GetFileName(this.CreatedApp.ExePath);
            set
            {
                //this.CreatedApp.ExePath = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ExePath)));
            }
        }
        public uint IconIndex
        {
            get => this.CreatedApp.IconIndex;
            set => this.CreatedApp.IconIndex = value;
        }
        public string IconPath
        {
            get => IOPath.GetFileName(this.CreatedApp.IconPath);
            set
            {
                //this.CreatedApp.IconPath = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IconPath)));
            }
        }
        public BitmapSource Image
        {
            get => this.CreatedApp.Image;
            set
            {
                value.Freeze();
                this.CreatedApp.Image = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Image)));
            }
        }

        public IconPreviewer IconPreviewer { get; set; }

        #region CONSTRUCTORS
        public NewApp(IntPtr appHandle)
        {
            //if (null == IconPreviewer)
            IconPreviewer = new IconPreviewer(appHandle);

            //else
            //    IconPreviewer.ClearImage();

            this.CreatedApp = new AppItem();

            this.InitializeComponent();
        }

        #endregion

        #region WINDOW EVENTS
        private void Window_Closed(object sender, EventArgs e)
        {
            //IconPreviewer?.ClearImage();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.displayNameBox.Focus();
        }

        #endregion

        public AppItem GetFinalizedApp()
        {
            var newApp = this.CreatedApp.Clone();
            return newApp;
        }

        private DispatcherOperation OnFileDialogOkAsync(string iconPath, uint iconIndex)
        {
            return this.Dispatcher.InvokeAsync(() => {
                this.Image = IconPreviewer.Preview(iconPath, iconIndex);
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
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                RestoreDirectory = true,
                Title = "Choose a program"
            };

            bool? result = fileDialog.ShowDialog();
            if (result.GetValueOrDefault())
            {
                DispatcherOperation previewTask = this.OnFileDialogOkAsync(fileDialog.FileName, 0);

                await this.Dispatcher.InvokeAsync(() =>
                {
                    this.findExeBtn.Visibility = Visibility.Hidden;
                    this.findExeBtn.IsEnabled = false;
                    this.findExeLbl.IsEnabled = true;

                    this.CreatedApp.ExePath = fileDialog.FileName;
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ExePath)));

                    this.findExeLbl.Visibility = Visibility.Visible;
                });

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
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                RestoreDirectory = true,
                Title = "Choose a icon file or one contained in a program"
            };
            bool? result = fileDialog.ShowDialog();
            if (result.GetValueOrDefault())
            {
                DispatcherOperation previewTask = this.OnFileDialogOkAsync(fileDialog.FileName, this.IconIndex);

                await this.Dispatcher.InvokeAsync(() =>
                {
                    this.findIconBtn.Visibility = Visibility.Hidden;
                    this.findIconBtn.IsEnabled = false;
                    this.findIconLbl.IsEnabled = true;

                    this.CreatedApp.IconPath = fileDialog.FileName;
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IconPath)));

                    this.findIconLbl.Visibility = Visibility.Visible;
                });

                await previewTask;
            }
        }

        #endregion
    }
}

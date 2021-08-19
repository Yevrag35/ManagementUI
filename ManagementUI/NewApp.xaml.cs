using ManagementUI.Models;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

using IOPath = System.IO.Path;

namespace ManagementUI
{
    /// <summary>
    /// Interaction logic for NewApp.xaml
    /// </summary>
    public partial class NewApp : Window, INotifyPropertyChanged
    {
        private uint _currentIndex;

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
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _ = this.displayNameBox.Focus();
        }

        #endregion

        public AppItem GetFinalizedApp()
        {
            AppItem newApp = this.CreatedApp.Clone();
            return newApp;
        }

        private DispatcherOperation OnFileDialogOkAsync(string iconPath, uint iconIndex)
        {
            return this.Dispatcher.InvokeAsync(() => {
                this.Image = this.IconPreviewer.Preview(iconPath, iconIndex);
                _currentIndex = iconIndex;
            });
        }

        #region TEXTBOX EVENTS
        private void TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (sender is TextBox tb && !string.IsNullOrEmpty(tb.Text))
                tb.SelectAll();
        }
        private async void IconIndexBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (_currentIndex != this.IconIndex && !string.IsNullOrWhiteSpace(this.IconPath))
            {
                await this.OnFileDialogOkAsync(this.IconPath, this.IconIndex);
            }
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
                await this.SetProgramFileName(fileDialog.FileName);
                //await this.Dispatcher.InvokeAsync(() =>
                //{

                //    //this.findExeBtn.Visibility = Visibility.Hidden;
                //    //this.findExeBtn.IsEnabled = false;
                //    //this.findExeLbl.IsEnabled = true;

                //    //this.CreatedApp.ExePath = fileDialog.FileName;
                //    //this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ExePath)));

                //    //this.findExeLbl.Visibility = Visibility.Visible;
                //});

                await previewTask;
            }
        }
        private async void FindExeBtn_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            await this.Dispatcher.InvokeAsync(() =>
            {
                int i = Convert.ToInt32(this.findExeBtn.IsEnabled);
                // if 'true' - current will equate to 'Visibility.Visible'
                // if 'false' - it equate to 'Visibility.Hidden'
                Visibility current = (Visibility)i;

                // opposite will the bitwise, opposite value of current.
                Visibility opposite = (Visibility)((i - 1) * -1);

                this.findExeBtn.Visibility = opposite;
                this.findExeLbl.IsEnabled = !this.findExeBtn.IsEnabled;
                this.findExeLbl.Visibility = current;
            });
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

        private DispatcherOperation SetProgramFileName(string filePath)
        {
            return this.Dispatcher.InvokeAsync(() =>
            {
                this.Set(nameof(this.ExePath), filePath, (fp) => this.CreatedApp.ExePath = fp);
                //this.CreatedApp.IconPath = filePath;
                this.findExeBtn.IsEnabled = false;

                if (string.IsNullOrWhiteSpace(this.CreatedApp.IconPath))
                {
                    this.CreatedApp.IconPath = filePath;
                }
            });
        }

        #endregion
    }
}
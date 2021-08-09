using ManagementUI.Models;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
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

namespace ManagementUI
{
    /// <summary>
    /// Interaction logic for NewApp.xaml
    /// </summary>
    public partial class NewApp : Window
    {
        private bool _answer;
        //public AppIconSetting CreatedApp { get; set; }
        public AppItem CreatedApp { get; set; }

        public NewApp() => InitializeComponent();

        #region WINDOW EVENTS
        private void Window_Loaded(object sender, RoutedEventArgs e) => this.displayNameBox.Focus();
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) => base.DialogResult = _answer;

        #endregion

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
            _answer = false;
            this.Close();
        }

        private void CreateBtn_Click(object sender, RoutedEventArgs e)
        {
            //if (!string.IsNullOrEmpty(this.displayNameBox.Text) && !string.IsNullOrEmpty((string)this.findExeLbl.Content))
            //{
            //    _answer = true;
            //    CreatedApp = new AppIconSetting
            //    {
            //        ExePath = this.findExeLbl.Content as string,
            //        Name = this.displayNameBox.Text,
            //    };
            //    if (!string.IsNullOrEmpty(this.argumentsBox.Text))
            //        CreatedApp.Arguments = this.argumentsBox.Text;

            //    if (!string.IsNullOrEmpty((string)this.findIconLbl.Content))
            //        CreatedApp.IconPath = this.findIconLbl.Content as string;

            //    CreatedApp.Index = !string.IsNullOrEmpty(this.iconIndexBox.Text)
            //        ? Convert.ToInt32(this.iconIndexBox.Text)
            //        : 0;

            //    this.Close();
            //}
        }

        private void FindExeBtn_Click(object sender, RoutedEventArgs e)
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
            if (result.HasValue && result.Value)
            {
                this.findExeBtn.Visibility = Visibility.Hidden;
                this.findExeBtn.IsEnabled = false;
                this.findExeLbl.IsEnabled = true;
                this.findExeLbl.Content = fileDialog.FileName;
                this.findExeLbl.Visibility = Visibility.Visible;
            }
        }

        private void FindIconBtn_Click(object sender, RoutedEventArgs e)
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
            if (result.HasValue && result.Value)
            {
                this.findIconBtn.Visibility = Visibility.Hidden;
                this.findIconBtn.IsEnabled = false;
                this.findIconLbl.IsEnabled = true;
                this.findIconLbl.Content = fileDialog.FileName;
                this.findIconLbl.Visibility = Visibility.Visible;
            }
        }

        #endregion
    }
}

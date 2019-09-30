using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace ManagementUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MUI : Window
    {
        internal static NetworkCredential Creds { get; set; }

        public MUI() => InitializeComponent();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            App.MyHandle = new WindowInteropHelper(this).Handle;
            this.LoadIcons(App.MyHandle, App.Settings);
        }

        private void CredButton_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new CredentialDialog
            {
                Content = "Enter the executing credentials",
                MainInstruction = "Enter the executing credentials",
                ShowSaveCheckBox = true,
                Target = "ThisWindow",
                WindowTitle = "ManagementUI Credentials"
            })
            {
                bool done = dialog.ShowDialog();
                if (done)
                {
                    Creds = new NetworkCredential(dialog.UserName, dialog.Password);
                }
            }
        }

        private async void SettsButton_Click(object sender, RoutedEventArgs e)
        {
            var editor = new SettingsEditor(App.Settings);
            await editor.LaunchAsync();
        }
    }
}

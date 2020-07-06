using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ManagementUI
{
    public partial class MUI
    {
        private const string MAIN_INSTRUCTION = "Relaunch Credentials";
        private const string CONTENT = "Enter the credentials to relaunch Management UI as:";
        private const string TARGET = "ThisWindow";
        private const string CRED_WINDOW_TITLE = "ManagementUI Credentials";
        private const string RELAUNCH = "RELAUNCH";
        private const string RELAUNCH_PROMPT_MSG = "Would you like to relaunch as this user?";
        private const string RELAUNCH_PROMPT_TITLE = "RUN AS";
        private const string RUN_AS = "RUN AS";

        internal CredentialDialog CreateCredentialDialog()
        {
            var dialog = new CredentialDialog
            {
                MainInstruction = MAIN_INSTRUCTION,
                Content = CONTENT,
                ShowSaveCheckBox = false,
                Target = TARGET,
                WindowTitle = CRED_WINDOW_TITLE
            };

            return dialog;
        }

        internal void HandleReprompt(RoutedEventArgs clickEvent)
        {
            MessageBoxResult answer = MessageBox.Show(RELAUNCH_PROMPT_MSG, RELAUNCH_PROMPT_TITLE, 
                MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.Cancel);

            switch (answer)
            {
                case MessageBoxResult.Yes:
                    this.Dispatcher.Invoke(() =>
                    {
                        //((MUI)Application.Current.MainWindow).RelaunchBtn.RaiseEvent(clickEvent);
                        this.RelaunchBtn.RaiseEvent(clickEvent);
                    });
                    break;
                case MessageBoxResult.No:
                    this.Dispatcher.Invoke(() =>
                    {
                        this.CredsButton.Content = RELAUNCH;
                    });
                    break;

                default:
                    break;
            }
        }
    }
}

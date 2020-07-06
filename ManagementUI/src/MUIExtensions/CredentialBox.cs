using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ManagementUI
{
    public partial class MUI
    {
        private const string MAIN_INSTRUCTION = "Relaunch Credentials";
        private const string CONTENT = "Enter the executing credentials";
        private const string TARGET = "ThisWindow";
        private const string CRED_WINDOW_TITLE = "ManagementUI Credentials";

        internal CredentialDialog CreateCredentialDialog()
        {
            var dialog = new CredentialDialog
            {
                MainInstruction = MAIN_INSTRUCTION,
                Content = CONTENT,
                ShowSaveCheckBox = true,
                Target = TARGET,
                WindowTitle = CRED_WINDOW_TITLE
            };
            //dialog.PasswordChanged += this.Dialog_PasswordChanged;

            return dialog;
        }
    }
}

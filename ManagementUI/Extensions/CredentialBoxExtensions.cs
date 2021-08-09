using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ManagementUI
{
    public partial class MUI
    {
        public CredentialBox NewCredentialDialog(bool allowSave = false)
        {
            return new CredentialBox
            {
                Target = "MUI",
                ShowSaveCheckBox = false,
                ShowUIForSavedCredentials = true
            };
        }
    }
}

namespace ManagementUI.Extensions
{
    public static class CredentialBoxExtensions
    {
        //public static CredentialDialog AddExtraText(this CredentialDialog dialog, string extraText)
        //{
        //    dialog.Content = extraText;
        //    dialog.
        //}
    }
}

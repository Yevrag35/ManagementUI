using ManagementUI.Functionality.Auth;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using Strings = ManagementUI.Properties.Resources;

namespace ManagementUI
{
    public class CredentialBox : CredentialDialog
    {
        public CredentialBox(Window parent)
            : base()
        {
            this.Owner = parent;
            this.ShowSaveCheckBox = false;
            this.ShowUIForSavedCredentials = false;
            this.Content = Strings.CredBox_Content;
            this.MainInstruction = Strings.CredBox_MainInstruction;
            this.Target = Strings.CredBox_Target;
        }

        public PrincipalInfo GetPrincipalInfo()
        {
            return new PrincipalInfo(this.Domain, this.UserName);
        }

        //private void CredentialBox_UserNameChanged(object sender, EventArgs e)
        //{
        //    this.Owner.Dispatcher.Invoke(() =>
        //    {
        //        if (_user.Length > 0)
        //        {
        //            string original = _user.ToString();
        //            _parsed = true;

        //            _user.Clear();
        //            _domain.Clear();
        //            if (!GetParsedUserAndDomain(original, ref _user, ref _domain))
        //            {
        //                _domain.Clear().Append(Environment.UserDomainName);
        //                if (_user.Length <= 0)
        //                    _user.Append(original);
        //            }
        //        }
        //    }, System.Windows.Threading.DispatcherPriority.Send);
        //}
    }
}

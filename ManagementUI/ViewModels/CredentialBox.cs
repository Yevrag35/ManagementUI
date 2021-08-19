using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Strings = ManagementUI.Properties.Resources;

namespace ManagementUI
{
    public class CredentialBox : CredentialDialog
    {
        public CredentialBox()
            : base()
        {
            this.Content = Strings.CredBox_Content;
            this.MainInstruction = Strings.CredBox_MainInstruction;
            this.Target = Strings.CredBox_Target;
            this.WindowTitle = Strings.CredBox_WindowTitle;
            var ss = new SecureString();
        }
    }
}

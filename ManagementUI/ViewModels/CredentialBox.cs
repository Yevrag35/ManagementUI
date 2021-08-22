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
        private const uint MAX = 256u;
        private StringBuilder _user;
        private StringBuilder _domain;
        private bool _parsed;

        public override string Domain
        {
            get => _domain.ToString();
            set => _domain.Clear().Append(value);
        }
        public override string UserName
        {
            get => _user.ToString();
            set
            {
                _user.Clear().Append(value);
                if (!_parsed)
                    this.OnUserNameChanged(EventArgs.Empty);
            }
        }

        public CredentialBox(Window parent)
            : base()
        {
            _user = new StringBuilder((int)MAX);
            _domain = new StringBuilder((int)MAX);
            this.Owner = parent;
            this.UserNameChanged += this.CredentialBox_UserNameChanged;
            this.Content = Strings.CredBox_Content;
            this.MainInstruction = Strings.CredBox_MainInstruction;
            this.Target = Strings.CredBox_Target;
        }

        public PrincipalInfo GetPrincipalInfo()
        {
            if (!_parsed)
                throw new InvalidOperationException("The Credential UI has not been displayed yet.");

            return new PrincipalInfo(this.Domain, this.UserName);
        }

        private void CredentialBox_UserNameChanged(object sender, EventArgs e)
        {
            this.Owner.Dispatcher.Invoke(() =>
            {
                if (_user.Length > 0)
                {
                    string original = _user.ToString();
                    _parsed = true;

                    _user.Clear();
                    _domain.Clear();
                    if (!GetParsedUserAndDomain(original, ref _user, ref _domain))
                    {
                        _domain.Clear().Append(Environment.UserDomainName);
                        if (_user.Length <= 0)
                            _user.Append(original);
                    }
                }
            }, System.Windows.Threading.DispatcherPriority.Send);
        }

        private static bool GetParsedUserAndDomain(string userName, ref StringBuilder user, ref StringBuilder domain)
        {
            CredUIReturnCodes result = CredUIParseName(userName, user, MAX, domain, MAX);
            switch (result)
            {
                case CredUIReturnCodes.NO_ERROR:
                    {
                        for (int i = 0; i < domain.Length; i++)
                        {
                            domain[i] = char.ToUpper(domain[i]);
                        }

                        return true;
                    }
                    

                case CredUIReturnCodes.ERROR_INVALID_ACCOUNT_NAME:
                    {
                        if (userName.StartsWith(".\\"))
                        {
                            _ = domain.Append(Environment.MachineName);
                            _ = user.Append(userName.Substring(2));
                            return true;
                        }

                        return false;
                    }

                default:
                    return false;
            }
        }

        protected override void OnUserNameChanged(EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(this.UserName))
                base.OnUserNameChanged(e);
        }

        private static bool TryGetDomain(out string domain)
        {
            domain = Environment.UserDomainName;
            return !string.IsNullOrWhiteSpace(domain);
        }

        [DllImport("credui.dll", CharSet = CharSet.Unicode, EntryPoint = "CredUIParseUserNameW", SetLastError = true)]
        private static extern CredUIReturnCodes CredUIParseName(
            string userName,
            StringBuilder parsedUser,
            uint userBufferSize,
            StringBuilder parsedDomain,
            uint domainBufferSize);

        private enum CredUIReturnCodes
        {
            NO_ERROR = 0,
            ERROR_CANCELLED = 1223,
            ERROR_NO_SUCH_LOGON_SESSION = 1312,
            ERROR_NOT_FOUND = 1168,
            ERROR_INVALID_ACCOUNT_NAME = 1315,
            ERROR_INSUFFICIENT_BUFFER = 122,
            ERROR_INVALID_PARAMETER = 87,
            ERROR_INVALID_FLAGS = 1004
        }
    }
}

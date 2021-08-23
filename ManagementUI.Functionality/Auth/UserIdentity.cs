using ManagementUI.Functionality.Executable;
using ManagementUI.Functionality.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Principal;

namespace ManagementUI.Functionality.Auth
{
    public class UserIdentity : UIModelBase, IDisposable, INotifyPropertyChanged, IUserIdentity,
        IProcessCredential
    {
        private const char AT_SIGN = (char)64;
        private const char BACKSLASH = (char)92;
        private const StringSplitOptions OPTIONS = StringSplitOptions.RemoveEmptyEntries;
        private const char PERIOD = (char)46;

        private bool _disposed;
        private PrincipalInfo _userId;
        private SecureString _password;

        public ContextType ContextType => _userId.ContextType;
        public string DisplayPrincipal
        {
            get => this.Principal?.Value ?? string.Empty;
        }
        public string Domain => _userId.Domain ?? string.Empty;
        public bool IsValidated { get; private set; }
        public NTAccount Principal { get; private set; }
        public string UserName => _userId.Value ?? string.Empty;

        #region CONSTRUCTORS
        public UserIdentity(PrincipalInfo pInfo, SecureString password)
        {
            _userId = pInfo;
            _password = password;
            this.NotifyOfChange(nameof(this.DisplayPrincipal));
        }

        #endregion

        #region PUBLIC METHODS
        public ProcessStartInfo AuthenticateProcess(ProcessStartInfo startInfo)
        {
            if (null == startInfo)
                throw new ArgumentNullException(nameof(startInfo));

            startInfo.UserName = this.UserName;
            startInfo.Domain = this.Domain;
            startInfo.Password = _password;
            startInfo.UseShellExecute = false;
            // Testing

            string original = startInfo.FileName;
            string originalArgs = startInfo.Arguments;
            string newArgs = string.Format("/c \"{0}\" {1}", original, originalArgs);
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = newArgs;

            // end testing
            return startInfo;
        }

        public bool IsValid()
        {
            using (var prinContext = new PrincipalContext(this.ContextType, this.Domain))
            {
                this.IsValidated = this.Validate(prinContext);
                if (this.IsValidated)
                {
                    if (!_userId.IsUpn)
                    {
                        using (var foundUser = UserPrincipal.FindByIdentity(prinContext, this.UserName))
                        {
                            if (null != foundUser)
                            {
                                string domain = this.Domain.ToUpper();
                                this.Principal = new NTAccount(domain, foundUser.SamAccountName);
                            }
                        }
                    }
                    else
                    {
                        this.Principal = new NTAccount(this.UserName);
                    }
                }
            }

            if (null == this.Principal)
                this.Principal = new NTAccount(this.Domain, this.UserName);

            return this.IsValidated;
        }

        #endregion

        public bool SetPrincipal(bool andValidate = true)
        {
            bool result = true;
            if (!andValidate && null == this.Principal)
                this.Principal = new NTAccount(this.Domain, this.UserName);

            else
                result = this.IsValid();

            return result;
        }

        private bool Validate(PrincipalContext context)
        {
            if (null == _password)
                return false;

            using (SecureString copyOfPassword = _password.Copy())
            {
                IntPtr pointer = Marshal.SecureStringToBSTR(copyOfPassword);
                ContextOptions options = ContextOptions.Negotiate;
                if (context.ContextType == ContextType.Domain)
                {
                    options |= ContextOptions.Signing;
                }
                // password exposure begins
                bool result = context.ValidateCredentials(
                    this.UserName,
                    Marshal.PtrToStringAuto(pointer),
                    options
                );
                Marshal.ZeroFreeBSTR(pointer);
                // password exposure ends
                return result;
            }
        }

        

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _password.Dispose();
                _disposed = true;
            }
        }
    }
}

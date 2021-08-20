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
            get => this.Principal.Value;
            set 
            {
                this.NotifyOfChange(nameof(DisplayPrincipal));
            }
        }
        public string Domain => _userId.Domain;
        public bool IsValidated { get; }
        public NTAccount Principal { get; }
        public string UserName => _userId.Value;

        #region CONSTRUCTORS
        public UserIdentity(string userAndDomain, SecureString password)
        {
            _userId = (PrincipalInfo)userAndDomain;
            _password = password;
            this.Principal = new NTAccount(_userId.Domain, _userId.Value);
            this.NotifyOfChange(nameof(DisplayPrincipal));
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
            return startInfo;
        }

        public bool IsValid()
        {
            bool result = false;
            using (var prinContext = new PrincipalContext(this.ContextType, this.Domain))
            {
                result = this.Validate(prinContext);
            }

            return result;
        }

        #endregion

        private bool Validate(PrincipalContext context)
        {
            using (SecureString copyOfPassword = _password.Copy())
            {
                IntPtr pointer = Marshal.SecureStringToBSTR(copyOfPassword);
                // password exposure begins
                bool result = context.ValidateCredentials(
                    this.UserName,
                    Marshal.PtrToStringAuto(pointer),
                    ContextOptions.Signing | ContextOptions.Negotiate
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

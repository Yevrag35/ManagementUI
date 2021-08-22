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

        public string ComputerName { get; private set; }
        public ContextType ContextType => _userId.ContextType;
        public string DisplayPrincipal
        {
            get => this.Principal?.Value ?? string.Empty;
        }
        public string Domain => _userId.Domain ?? string.Empty;
        public bool IsUserPrincipalName => (this.UserName?.Contains(AT_SIGN)).GetValueOrDefault();
        public bool IsValidated { get; private set; }
        public NTAccount Principal { get; private set; }
        public string UserName => _userId.Value ?? string.Empty;

        #region CONSTRUCTORS
        public UserIdentity(string userAndDomain, SecureString password)
        {
            _userId = (PrincipalInfo)userAndDomain;
            _password = password;
            //this.Principal = new NTAccount(_userId.Domain, _userId.Value);
            this.NotifyOfChange(nameof(this.DisplayPrincipal));
        }
        public UserIdentity(PrincipalInfo pInfo, SecureString password)
        {
            _userId = pInfo;
            _password = password;
            //this.Principal = new NTAccount(_userId.Domain, _userId.Value);
            this.NotifyOfChange(nameof(this.DisplayPrincipal));
        }

        #endregion

        #region PUBLIC METHODS
        public ProcessStartInfo AuthenticateProcess(ProcessStartInfo startInfo)
        {
            if (null == startInfo)
                throw new ArgumentNullException(nameof(startInfo));

            startInfo.UserName = this.UserName;
            startInfo.Domain = !this.IsUserPrincipalName 
                ? this.Domain
                : null;
            startInfo.Password = _password;
            return startInfo;
        }

        public bool IsValid()
        {
            using (var prinContext = new PrincipalContext(this.ContextType, this.Domain))
            {
                this.IsValidated = this.Validate(prinContext);
                if (this.IsValidated)
                {
                    using (var foundUser = UserPrincipal.FindByIdentity(prinContext, this.UserName))
                    {
                        if (null != foundUser)
                        {
                            string domain = null;
                            switch (this.ContextType)
                            {
                                case ContextType.Domain:
                                    domain = prinContext.ContextType == ContextType.Domain
                                            && TryGetMachineNetBiosDomain(this.Domain, out string nbName)
                                        ? nbName
                                        : this.Domain;
                                    break;

                                default:
                                    domain = this.Domain.ToUpper();
                                    break;
                            }

                            this.Principal = new NTAccount(domain, foundUser.SamAccountName);
                        }
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

        #region NETBIOS NAME RESOLUTION
        private static bool TryGetMachineNetBiosDomain(string domain, out string nbName)
        {
            nbName = null;
            IntPtr pBuffer = IntPtr.Zero;

            WKSTA_INFO_100 info;
            
            try
            {
                int retVal = NetWkstaGetInfo(domain, 100, out pBuffer);
                if (retVal != 0)
                    throw new Win32Exception(retVal);

                info = (WKSTA_INFO_100)Marshal.PtrToStructure(pBuffer, typeof(WKSTA_INFO_100));
                nbName = info.wki100_langroup;
            }
            catch { }
            finally
            {
                NetApiBufferFree(pBuffer);
            }

            return !string.IsNullOrWhiteSpace(nbName);
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private class WKSTA_INFO_100
        {
            public int wki100_platform_id;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string wki100_computername;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string wki100_langroup;
            public int wki100_ver_major;
            public int wki100_ver_minor;
        }

        [DllImport("netapi32.dll", CharSet = CharSet.Auto)]
        private static extern int NetWkstaGetInfo(string server, int level, out IntPtr info);

        [DllImport("netapi32.dll")]
        private static extern int NetApiBufferFree(IntPtr pBuf);

        #endregion

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

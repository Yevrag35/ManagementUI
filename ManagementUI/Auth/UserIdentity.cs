using ManagementUI.Functionality.Executable;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Security.Principal;

namespace ManagementUI.Auth
{
    public sealed class UserIdentity : IDisposable, IProcessCredential
    {
        private const char BACKSLASH = (char)92;
        private const char AT_SIGN = (char)64;
        private const StringSplitOptions OPTIONS = StringSplitOptions.RemoveEmptyEntries;

        private bool _disposed;
        private SecureString _password;
        public string Domain { get; }
        public NTAccount NTAccount { get; }
        public string UserName { get; }

        private UserIdentity((string, string) userAndDomain, SecureString password)
            : this(userAndDomain.Item2, userAndDomain.Item1, password)
        {
        }
        public UserIdentity(string userName, string domain, SecureString password)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentNullException(nameof(userName));

            this.UserName = userName;
            this.Domain = domain;
            this.NTAccount = new NTAccount(domain, userName);
            _password = password;
        }
        public UserIdentity(string userAndDomain, SecureString password)
            : this(SeparateDomainAndUser(userAndDomain), password)
        {
        }

        private static (string, string) ProcessBackslash(string[] split)
        {
            if (null == split || split.Length <= 0)
                return default;

            return (split.FirstOrDefault(), string.Join(string.Empty, split.Skip(1)));
        }
        private static (string, string) ProcessAtSign(string[] split)
        {
            if (null == split || split.Length <= 0)
                return default;

            return (split.LastOrDefault(), string.Join(string.Empty, split.Reverse().Skip(1).Reverse()));
        }
        private static (string, string) SeparateDomainAndUser(string combined)
        {
            if (combined.Contains(BACKSLASH))
            {
                string[] splitBack = combined.Split(new char[] { BACKSLASH }, OPTIONS);
                return ProcessBackslash(splitBack);
            }
            else if (combined.Contains(AT_SIGN))
            {
                string[] splitAt = combined.Split(new char[] { AT_SIGN }, OPTIONS);
                return ProcessAtSign(splitAt);
            }
            else // we'll assume it's a local account
                return (Environment.MachineName, combined);
        }

        public ProcessStartInfo AuthenticateProcess(ProcessStartInfo startInfo)
        {
            if (null == startInfo)
                throw new ArgumentNullException(nameof(startInfo));

            startInfo.UserName = this.UserName;
            startInfo.Domain = this.Domain;
            startInfo.Password = _password;
            return startInfo;
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _password.Dispose();
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Net;
using System.Security;

namespace ManagementUI.Auth
{
    public class ADCredential : ICredentials, ICreatesProcessStartInfo
    {
        private const string LDAP_DSE = "LDAP://{0}/RootDSE";

        private NetworkCredential _netCreds;

        public string AuthUserName { get; }
        public AuthenticationTypes AuthenticationTypes { get; set; } = AuthenticationTypes.Secure;
        public string Domain => _netCreds.Domain;
        public string UserName => _netCreds.UserName;

        private ADCredential(NetworkCredential netCreds)
        {
            (string, string) userAndDom = this.SeparateDomainAndUser(netCreds.UserName);
            if (string.IsNullOrWhiteSpace(netCreds.Domain) && !string.IsNullOrEmpty(userAndDom.Item1))
                netCreds.Domain = userAndDom.Item1;

            _netCreds = netCreds;
            this.AuthUserName = this.Combine(userAndDom.Item2, userAndDom.Item1);
        }
        public ADCredential(string userName, SecureString password)
        {
            (string, string) userAndDom = this.SeparateDomainAndUser(userName);
            _netCreds = new NetworkCredential(userAndDom.Item2, password, userAndDom.Item1);
            this.AuthUserName = this.Combine(userAndDom.Item2, userAndDom.Item1);
        }
        public ADCredential(string domain, string userName, SecureString password)
        {
            _netCreds = new NetworkCredential(userName, password, domain);
            this.AuthUserName = this.Combine(userName, domain);
        }

        private string Combine(string user, string domain) => string.Format("{0}\\{1}", domain, user);
        public NetworkCredential GetCredential(Uri uri, string authType) => _netCreds.GetCredential(uri, authType);
        public ProcessStartInfo NewStartInfo(string filePath)
        {
            var psi = new ProcessStartInfo
            {
                FileName = filePath,
                CreateNoWindow = true,
                UseShellExecute = false,
                LoadUserProfile = true,
                UserName = this.UserName,
                Password = _netCreds.SecurePassword
            };
            if (!string.IsNullOrWhiteSpace(_netCreds.Domain))
                psi.Domain = _netCreds.Domain;

            return psi;
        }
        public ProcessStartInfo NewStartInfo(string filePath, bool runAs, bool useShellExecute)
        {
            ProcessStartInfo psi = this.NewStartInfo(filePath);
            if (runAs)
                psi.Verb = "RunAs";

            psi.UseShellExecute = useShellExecute;
            return psi;
        }
        private (string, string) SeparateDomainAndUser(string combined)
        {
            (string, string) retStr = (null, null);
            if (combined.Contains("\\"))
            {
                string[] splitBack = combined.Split(new string[1] { @"\" }, StringSplitOptions.RemoveEmptyEntries);
                return (splitBack.Where(x => x != splitBack.LastOrDefault()).FirstOrDefault(),
                        splitBack.Where(x => x != splitBack.FirstOrDefault()).LastOrDefault());
            }
            else if (combined.Contains("@"))
            {
                string[] splitAt = combined.Split(new string[1] { "@" }, StringSplitOptions.RemoveEmptyEntries);
                retStr = (splitAt.Where(x => x != splitAt.FirstOrDefault()).LastOrDefault(),
                          combined);
            }
            return retStr;
        }
        public bool TryAuthenticate() => this.TryAuthenticate(out Exception throwAway);
        public bool TryAuthenticate(out Exception caughtException)
        {
            caughtException = null;
            bool result = false;
            var dirEntry = new DirectoryEntry(string.Format(LDAP_DSE, this.Domain), this.AuthUserName, _netCreds.Password, this.AuthenticationTypes);
            try
            {
                dirEntry.RefreshCache();
                result = true;
            }
            catch (Exception e)
            {
                caughtException = e;
            }
            finally
            {
                if (dirEntry != null)
                    dirEntry.Dispose();
            }

            return result;
        }

        public static explicit operator ADCredential(NetworkCredential netCreds) => new ADCredential(netCreds);
    }
}

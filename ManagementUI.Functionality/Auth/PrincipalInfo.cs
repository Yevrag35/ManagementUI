using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;

namespace ManagementUI.Functionality.Auth
{
    public struct PrincipalInfo
    {
        private const char AT_SIGN = (char)64;
        private const char BACKSLASH = (char)92;
        private const StringSplitOptions OPTIONS = StringSplitOptions.RemoveEmptyEntries;
        private const char PERIOD = (char)46;

        private ContextType _contextType;
        private string _domain;
        private string _userName;

        public ContextType ContextType => _contextType;
        public string Domain => _domain;
        public string Value => _userName;

        private PrincipalInfo(string domain, string userName, ContextType contextType)
        {
            _domain = domain;
            _userName = userName;
            _contextType = contextType;
        }
        private PrincipalInfo(string userName)
        {
            (string, string, ContextType) result = SeparateDomainAndUser(userName);
            _domain = result.Item1 != PERIOD.ToString()
                ? result.Item1
                : Environment.MachineName;
            _userName = result.Item2;
            _contextType = result.Item3;
        }

        public static explicit operator PrincipalInfo(string userName)
        {
            return new PrincipalInfo(userName);
        }

        #region PUBLIC METHODS
        public PrincipalInfo OverrideContext(ContextType contextType)
        {
            return new PrincipalInfo(this.Domain, this.Value, contextType);
        }

        #endregion

        private static ContextType GetContextTypeFromDomain(string domain)
        {
            ContextType context = ContextType.Domain;

            if (Environment.MachineName.Equals(domain, StringComparison.CurrentCultureIgnoreCase)
                || (domain.Length == 1 && domain[0] == PERIOD))
            {
                context = ContextType.Machine;
            }

            return context;
        }
        private static string FormatUser(IEnumerable<string> split, Func<IEnumerable<string>, IEnumerable<string>> transform)
        {
            return string.Join(string.Empty, transform(split));
        }
        private static (string, string, ContextType) ProcessAtSign(string[] split)
        {
            if (null == split || split.Length <= 0)
                return default;

            else if (split.Length == 1)
            {
                return ProcessNoDomain(split);
            }
            else
            {
                string domain = split.LastOrDefault();
                string user = FormatUser(split, x => x);
                return (
                    domain,
                    user,
                    GetContextTypeFromDomain(domain)
                );
            }
        }
        private static (string, string, ContextType) ProcessBackslash(string[] split)
        {
            if (null == split || split.Length <= 0)
                return default;

            else if (split.Length == 1)
            {
                return ProcessNoDomain(split);
            }
            else
            {
                string user = FormatUser(split, x => x.Skip(1));
                string domain = split[0];
                return (
                    domain,
                    user,
                    GetContextTypeFromDomain(domain)
                );
            }
        }
        private static (string, string, ContextType) ProcessNoDomain(string[] split)
        {
            string domain = Environment.UserDomainName;
            return (domain, split[0], GetContextTypeFromDomain(domain));
        }

        private static (string, string, ContextType) SeparateDomainAndUser(string combined)
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
                return (Environment.MachineName, combined, ContextType.Machine);
        }
    }
}

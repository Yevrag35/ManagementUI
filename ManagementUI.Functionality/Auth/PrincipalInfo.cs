using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Runtime.InteropServices;

namespace ManagementUI.Functionality.Auth
{
    public struct PrincipalInfo
    {
        private const char AT_SIGN = (char)64;

        private ContextType _contextType;
        private string _domain;
        private string _userName;

        public ContextType ContextType => _contextType;
        public string Domain => _domain;
        public bool IsUpn => _userName.Contains(AT_SIGN);
        public string Value => _userName;

        private PrincipalInfo(string domain, string userName, ContextType contextType)
        {
            _domain = domain;
            _userName = userName;
            _contextType = contextType;
        }
        public PrincipalInfo(string domain, string userName)
        {
            _domain = domain;
            _userName = userName;
            _contextType = GetContextTypeFromDomain(_domain, IsUserPrincipalName(userName));
        }

        private static bool IsUserPrincipalName(string userName)
        {
            return (userName?.Contains(AT_SIGN)).GetValueOrDefault();
        }

        #region PUBLIC METHODS
        public PrincipalInfo OverrideContext(ContextType contextType)
        {
            return new PrincipalInfo(this.Domain, this.Value, contextType);
        }

        #endregion

        private static ContextType GetContextTypeFromDomain(string domain, bool valueIsUpn)
        {
            ContextType context = ContextType.Domain;
            if (!valueIsUpn)
            {
                string computerName = Environment.GetEnvironmentVariable("COMPUTERNAME");
                if (string.IsNullOrWhiteSpace(computerName))
                {
                    computerName = TryGetMachineName(out string nbName)
                        ? nbName
                        : string.Empty;
                }

                if (computerName.Equals(domain, StringComparison.CurrentCultureIgnoreCase))
                    context = ContextType.Machine;
            }

            return context;
        }

        #region NETBIOS NAME RESOLUTION
        private static bool TryGetMachineName(out string nbName)
        {
            nbName = null;
            IntPtr pBuffer = IntPtr.Zero;

            WKSTA_INFO_100 info;

            try
            {
                int retVal = NetWkstaGetInfo(null, 100, out pBuffer);
                if (retVal != 0)
                    throw new Win32Exception(retVal);

                info = (WKSTA_INFO_100)Marshal.PtrToStructure(pBuffer, typeof(WKSTA_INFO_100));
                nbName = info.wki100_computername;
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
    }
}

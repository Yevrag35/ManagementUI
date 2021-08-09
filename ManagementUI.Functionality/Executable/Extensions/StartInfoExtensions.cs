using System;
using System.Diagnostics;
using ManagementUI.Functionality.Executable;

namespace ManagementUI.Functionality.Executable
{
    public static class StartInfoFactory
    {
        public static ProcessStartInfo Create() => new ProcessStartInfo
        {
            CreateNoWindow = true
        };
    }
}

namespace ManagementUI.Functionality.Executable.Extensions
{
    public static class StartInfoExtensions
    {
        public static ProcessStartInfo AddExe(this ProcessStartInfo psi, string exePath)
        {
            psi.FileName = exePath;
            return psi;
        }
        public static ProcessStartInfo AddArguments(this ProcessStartInfo psi, params string[] arguments)
        {
            if (null != arguments && arguments.Length > 0)
                psi.Arguments = string.Join(" ", arguments);

            return psi;
        }
        public static ProcessStartInfo AddCredentials(this ProcessStartInfo psi, IProcessCredential credential)
        {
            if (null != credential)
            {
                _ = credential.AuthenticateProcess(psi);
            }

            return psi;
        }
        public static ProcessStartInfo AddRunAs(this ProcessStartInfo psi, bool toggle = true)
        {
            if (toggle)
            {
                psi.Verb = "RunAs";
            }

            return psi;
        }
        public static ProcessStartInfo UseShellExecute(this ProcessStartInfo psi, bool toggle = true)
        {
            psi.UseShellExecute = toggle;
            return psi;
        }
    }
}

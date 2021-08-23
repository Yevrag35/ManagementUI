using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementUI.Functionality.Executable
{
    public static class Parser
    {
        public static ArgumentLibrary Library { get; private set; }

        public static bool Initialize()
        {
            if (null == Library)
                Library = new ArgumentLibrary();

            return null != Library;
        }

        public static string GetElevateArguments(ProcessStartInfo psi)
        {
            if (!Initialize())
                return null;

            var list = new List<string>(6);

            if (Library.TryGetArgument(psi, x => x.FileName, out string fileName))
                list.Add(fileName);

            if (Library.TryGetArgument(psi, x => x.Arguments, out string arguments))
                list.Add(arguments);

            if (Library.TryGetArgument(psi, x => x.CreateNoWindow, out string noNewWindow))
                list.Add(noNewWindow);

            if (Library.TryGetArgument(psi, x => x.ErrorDialogParentHandle.ToInt32(), out string handle))
                list.Add(handle);

            if (Library.TryGetArgument(psi, x => x.LoadUserProfile, out string loadUserProfile))
                list.Add(loadUserProfile);

            if (Library.TryGetArgument(psi, x => x.UseShellExecute, out string shell))
                list.Add(shell);

            return string.Join(" ", list);
        }
    }
}

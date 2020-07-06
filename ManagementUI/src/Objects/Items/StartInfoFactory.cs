using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;

namespace ManagementUI
{
    internal static class StartInfoFactory
    {
        internal static ProcessStartInfo Create(string filePath, bool runAs, bool useShellExecute)
        {
            string verb = string.Empty;
            if (runAs)
                verb = "RunAs";

            return new ProcessStartInfo
            {
                FileName = filePath,
                CreateNoWindow = true,
                Verb = verb,
                UseShellExecute = useShellExecute
            };
        }

        internal static ProcessStartInfo Create(string filePath, bool runAs, bool useShellExecute, ICreatesProcessStartInfo creator)
        {
            if (creator == null)
                return Create(filePath, runAs, useShellExecute);

            string verb = string.Empty;
            if (runAs)
                verb = "RunAs";

            ProcessStartInfo psi = creator.NewStartInfo(filePath);
            psi.Verb = verb;
            psi.UseShellExecute = useShellExecute;

            return psi;
        }
    }
}

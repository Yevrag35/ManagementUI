using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security;

namespace ManagementUI.Functionality.Executable
{
    public interface IProcessCredential
    {
        string Domain { get; }
        string UserName { get; }

        ProcessStartInfo AuthenticateProcess(ProcessStartInfo processToAuthenticate);

        //SecureString GetPassword();
        //bool Validate(out Exception e);
    }
}

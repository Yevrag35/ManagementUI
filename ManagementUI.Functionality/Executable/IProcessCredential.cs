using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;

namespace ManagementUI.Functionality.Executable
{
    public interface IProcessCredential
    {
        string Domain { get; }
        string UserName { get; }

        SecureString GetPassword();
        bool Validate(out Exception e);
    }
}

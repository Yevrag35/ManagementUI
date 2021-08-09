using ManagementUI.Functionality.Executable;
using System;
using System.Security.Principal;

namespace ManagementUI.Functionality.Auth
{
    public interface IUserIdentity : IDisposable, IProcessCredential
    {
        bool IsValidated { get; }
        NTAccount NTAccount { get; }

        bool IsValid();
    }
}

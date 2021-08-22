using System;
using System.Security.Principal;
using ManagementUI.Functionality.Models;

namespace ManagementUI.Functionality.Auth
{
    public class RunAsDisplay : UIModelBase
    {
        private NTAccount _nTAccount;

        public string DisplayPrincipal
        {
            get => this.Principal.Value;
            set
            {
                this.NotifyOfChange(nameof(this.DisplayPrincipal));
            }
        }
        public NTAccount Principal
        {
            get => _nTAccount;
            set
            {
                _nTAccount = value;
                this.NotifyOfChange(nameof(this.DisplayPrincipal));
            }
        }

        public RunAsDisplay()
        {
            this.Principal = new NTAccount(WindowsIdentity.GetCurrent().Name);
        }

        public void ApplyFromCreds(IUserIdentity userId)
        {
            //this.Principal = new NTAccount(userId.Domain, userId.UserName);
            this.Principal = userId.Principal;
        }
    }
}

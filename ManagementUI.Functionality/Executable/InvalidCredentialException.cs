using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementUI.Functionality.Executable
{
    public class InvalidCredentialException : UnauthorizedAccessException
    {
        private const string DEF_MSG = "Unable to validate the provided credentials";

        public string Domain { get; }
        public string UserName { get; }

        public InvalidCredentialException(string userName, string domain, Exception innerException)
            : base(DEF_MSG, innerException)
        {
            this.Domain = domain;
            this.UserName = userName;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using ManagementUI.Functionality.Executable;
using ManagementUI.Functionality.Executable.Extensions;
using ManagementUI.Functionality.Models;

namespace ManagementUI.Functionality.Executable
{
    public abstract class LaunchableBase : UIModelBase, ILaunchable
    {
        public virtual string Arguments { get; set; }
        public virtual string ExePath { get; set; }

        public LaunchableBase()
            : base()
        {
        }

        public Process MakeProcess(bool parentIsElevated, bool runAs, IProcessCredential credential)
        {
            this.ValidateParameters();

            var proc = new Process { StartInfo = NewProcessStartInfo(parentIsElevated, runAs, credential, this.ExePath, this.Arguments) };
            return proc;
        }

        //public Process MakeProcess(bool parentIsElevated, bool runAs, IProcessCredential credential, bool validateCreds)
        //{
        //    if (validateCreds)
        //    {
        //        if (null == credential)
        //            throw new ArgumentException(string.Format("{0} cannot be null if '{1}' is true",
        //                nameof(credential), nameof(validateCreds)));

        //        if (!credential.Validate(out Exception e))
        //        {
        //            throw new InvalidCredentialException(credential.UserName, credential.Domain, e);
        //        }
        //    }

        //    return this.MakeProcess(parentIsElevated, runAs, credential);
        //}
        private static ProcessStartInfo NewProcessStartInfo(bool parentIsElevated, bool runAs, IProcessCredential credential,
            string exe, string arguments)
        {
            return StartInfoFactory
                .Create()
                    .AddExe(exe)
                    .AddArguments(arguments)
                    .AddRunAs(runAs)
                    .UseShellExecute(!parentIsElevated)
                    .AddCredentials(credential);
        }

        /// <summary>
        /// Validates the <see cref="ExePath"/> prior to constructing a <see cref="Process"/> instance.
        /// </summary>
        /// <exception cref="ArgumentNullException"><see cref="ExePath"/> is <see langword="null"/> or empty.</exception>
        protected void ValidateParameters()
        {
            this.ValidateParameters(this.ExePath, false, null);
        }

        /// <summary>
        /// Validates the mandatory parameters prior to constructing a <see cref="Process"/> instance.
        /// </summary>
        /// <param name="exePath">The path of the executable to be launched.</param>
        /// <param name="setArgs">Indicates whether to overwrite/"initially set" <see cref="Arguments"/>.</param>
        /// <param name="arguments">
        ///     The set of arguments for the <see cref="LaunchableBase"/> that will be joined by a space.
        /// </param>
        /// <exception cref="ArgumentNullException"><see cref="ExePath"/> is <see langword="null"/> or empty.</exception>
        protected virtual void ValidateParameters(string exePath, bool setArgs, params string[] arguments)
        {
            if (string.IsNullOrWhiteSpace(exePath))
                throw new ArgumentNullException(nameof(exePath));

            if (setArgs)
            {
                this.Arguments = null != arguments
                    ? string.Join(" ", arguments)
                    : null;
            }
        }
    }
}

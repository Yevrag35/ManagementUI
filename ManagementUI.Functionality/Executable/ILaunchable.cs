using System;
using System.Diagnostics;

namespace ManagementUI.Functionality.Executable
{
    /// <summary>
    /// An interface exposing properties and methods for generating child processes.
    /// </summary>
    public interface ILaunchable
    {
        /// <summary>
        /// The set of command-line arguments used when starting the <see cref="Process"/>.
        /// </summary>
        string Arguments { get; }
        /// <summary>
        /// The path of the executable to be launched.
        /// </summary>
        string ExePath { get; }

        /// <summary>
        /// Indicates whether the Windows user profile is to be loaded from the registry when launched.
        /// </summary>
        bool LoadUserProfile { get; }

        /// <summary>
        /// Indicates to launch the process without creating a new window to contain it or if a new window
        /// is to be created.
        /// </summary>
        bool NoNewWindow { get; }

        /// <summary>
        /// Generates a new <see cref="Process"/> with a constructed <see cref="ProcessStartInfo"/>
        /// using the provided credential.
        /// </summary>
        /// <param name="parentIsElevated">
        ///     Indicates whether the parent process is elevated.  This determines the 
        ///     <see cref="ProcessStartInfo.UseShellExecute"/> behavior.
        /// </param>
        /// <param name="runAs">
        ///     Indicates to launch the new <see cref="Process"/> with elevated rights.
        /// </param>
        /// <param name="credential">
        ///     The set of credentials to use when launching the <see cref="Process"/>.
        /// </param>
        /// <returns>
        ///     A <see cref="Process"/> instance that has not been started.
        /// </returns>
        /// <exception cref="ArgumentNullException"><see cref="ExePath"/> is not set.</exception>
        Process MakeProcess(bool parentIsElevated, bool runAs, IProcessCredential credential);

        ///// <summary>
        ///// Generates a new <see cref="Process"/> with a constructed <see cref="ProcessStartInfo"/> while
        ///// also, optionally, validating the provided credentials.
        ///// </summary>
        ///// <remarks>
        /////     If <paramref name="credential"/> is to be validated and fails,
        /////     an <see cref="InvalidCredentialException"/> is thrown.
        ///// </remarks>
        ///// <param name="parentIsElevated">
        /////     Indicates whether the parent process is elevated.  This determines the 
        /////     <see cref="ProcessStartInfo.UseShellExecute"/> behavior.
        ///// </param>
        ///// <param name="runAs">
        /////     Indicates to launch the new <see cref="Process"/> with elevated rights.
        ///// </param>
        ///// <param name="credential">
        /////     The set of credentials to use when launching the <see cref="Process"/>.
        ///// </param>
        ///// <param name="validateCreds">
        /////     Indicates whether to validate <paramref name="credential"/> prior to constructing the
        /////     <see cref="ProcessStartInfo"/>.
        ///// </param>
        ///// <returns>
        /////     A <see cref="Process"/> instance that has not been started.
        ///// </returns>
        ///// <exception cref="ArgumentNullException"><see cref="ExePath"/> is not set.</exception>
        ///// <exception cref="ArgumentException">
        /////     <paramref name="validateCreds"/> is <see langword="true"/> but <paramref name="credential"/>
        /////     is <see langword="null"/>.
        ///// </exception>
        ///// <exception cref="InvalidCredentialException">Unable to <paramref name="credential"/>.</exception>
        //Process MakeProcess(bool parentIsElevated, bool runAs, IProcessCredential credential, bool validateCreds);
    }
}

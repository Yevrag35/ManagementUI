using System;
using System.Diagnostics;

namespace ManagementUI.Functionality
{
    public interface ILaunchable
    {
        string Arguments { get; }
        string ExePath { get; }

        Process MakeProcess(bool parentIsElevated, bool runAs, IProcessCredential credential);
    }
}

using System;
using System.Diagnostics;

namespace ManagementUI
{
    public interface ICreatesProcessStartInfo
    {
        ProcessStartInfo NewStartInfo(string filePath);
        //ProcessStartInfo NewStartInfo(string filePath, bool runAs, bool useShellExecute);
    }
}

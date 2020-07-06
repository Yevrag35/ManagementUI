using System;
using System.Diagnostics;

namespace ManagementUI
{
    public interface IStartInfoGenerator
    {
        ProcessStartInfo NewStartInfo(string filePath);
    }
}

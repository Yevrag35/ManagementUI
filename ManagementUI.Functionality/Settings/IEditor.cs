using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ManagementUI.Functionality.Executable;

namespace ManagementUI.Functionality.Settings
{
    public interface IEditor : ILaunchable, IDisposable
    {
        event EditorEventHandler ProcessExited;

        IProcessCredential Credentials { get; set; }

        bool IsUsable();
        void Start(bool isParentElevated, bool runAs);
    }
}

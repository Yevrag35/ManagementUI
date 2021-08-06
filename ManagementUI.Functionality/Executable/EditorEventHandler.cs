using System;
using System.Diagnostics;
using ManagementUI.Functionality.Executable;
using ManagementUI.Functionality.Settings;

namespace ManagementUI.Functionality.Executable
{
    public delegate void EditorEventHandler(object sender, EditorEventArgs e);
    public class EditorEventArgs : EventArgs
    {
        public int ExitCode { get; }
        public bool HasExited { get; } = true;
        
        public EditorEventArgs(int exitCode)
        {
            this.ExitCode = exitCode;
        }
        public EditorEventArgs(int exitCode, bool hasExited)
            : this(exitCode)
        {
            this.HasExited = hasExited;
        }
    }
}

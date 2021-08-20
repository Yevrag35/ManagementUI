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
        public int ProcessId { get; }
        
        public EditorEventArgs(int exitCode, int processId)
        {
            this.ExitCode = exitCode;
            this.ProcessId = processId;
        }
        public EditorEventArgs(int exitCode, int processId, bool hasExited)
            : this(exitCode, processId)
        {
            this.HasExited = hasExited;
        }
    }
}

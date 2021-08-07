using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using ManagementUI.Functionality.Executable;

namespace ManagementUI.Functionality.Settings
{
    public abstract class EditorBase : LaunchableBase, IEditor, ILaunchable
    {
        //private bool _disposed;

        public event EditorEventHandler ProcessExited;

        public IProcessCredential Credentials { get; set; }

        protected EditorBase()
            : base()
        {
        }

        public EditorBase(string exePath, params string[] arguments)
            : base()
        {
            this.ValidateParameters(exePath, true, arguments);

            this.ExePath = exePath;
        }

        public bool IsUsable()
        {
            try
            {
                this.ValidateParameters();
            }
            catch
            {
                return false;
            }

            return File.Exists(this.ExePath);
        }

        public Process Start(bool parentIsElevated, bool runAs)
        {
            Process process = this.MakeProcess(parentIsElevated, runAs, this.Credentials);
            process.EnableRaisingEvents = true;
            process.Exited += this.Process_Exited;
            process.Start();
            return process;
        }

        private static EditorEventArgs NewEventArgs(Process process)
        {
            return null != process
                ? new EditorEventArgs(process.ExitCode, process.Id, process.HasExited)
                : new EditorEventArgs(int.MinValue, -1);
        }
        private void Process_Exited(object sender, EventArgs e)
        {
            Process p = (Process)sender;
            var args = NewEventArgs(p);
            this.ProcessExited?.Invoke(this, args);

            p.Dispose();
        }

        #region IDISPOSABLE
        //public void Dispose()
        //{
        //    this.Dispose(true);
        //    GC.SuppressFinalize(this);
        //}
        //protected virtual void Dispose(bool disposing)
        //{
        //    if (_disposed)
        //        return;

        //    if (disposing)
        //    {
        //        _process?.Dispose();
        //        _disposed = true;
        //    }
        //}

        #endregion
    }
}

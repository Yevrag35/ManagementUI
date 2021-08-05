using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ManagementUI.Json.Preferences.Editors
{
    public abstract class EditorBase : IEditor
    {
        protected static string APPDATA = "APPDATA";
        protected static string PROGRAMFILES = "PROGRAMFILES";
        protected static string WINDIR = "WINDIR";

        protected abstract string ExePath { get; }
        public string Name { get; }

        public EditorBase(string name)
        {
            this.Name = name;
        }

        public Task LaunchAsync(string fileToOpen)
        {
            return Task.Run(() =>
            {
                using (var process = new Process
                {
                    StartInfo = this.GenerateStartInfo(fileToOpen)
                })
                {
                    process.Start();
                    process.WaitForExit();
                }
            });
        }

        protected virtual ProcessStartInfo GenerateStartInfo(ProcessStartInfo psi)
        {
            if (psi == null)
            {
                return new ProcessStartInfo
                {
                    Arguments = string.Format(fileToOpen, Environment.GetEnvironmentVariable(APPDATA)),
                    CreateNoWindow = true,
                    FileName = this.ExePath
                };
            }
            else
            {
                psi.
            }
        }
    }
}

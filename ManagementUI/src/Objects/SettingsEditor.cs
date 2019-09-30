using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ManagementUI
{
    public class SettingsEditor
    {
        private const string NOTEPAD = "{0}\\Notepad.exe";
        private const string NOTEPAD_PP = "{0}\\Notepad++\\notepad++.exe";
        private const string VS_CODE = "{0}\\Microsoft VS Code\\Code.exe";

        public SettingsLauncher Current { get; set; }

        public SettingsEditor(SettingsJson currentSettings) => 
            this.Current = currentSettings.Editor;

        public void Launch()
        {
            string launchPath = this.GetLaunchPath(this.Current);
            ProcessStartInfo psi = this.NewPSI(launchPath);

            using (var process = new Process
            {
                StartInfo = psi
            })
            {
                process.Start();
                process.WaitForExit();
            }
        }

        private string GetLaunchPath(SettingsLauncher currentLauncher)
        {
            string launchPath = null;
            string pf = Environment.GetEnvironmentVariable("PROGRAMFILES");
            switch (currentLauncher)
            {
                case SettingsLauncher.VsCode:
                    launchPath = string.Format(VS_CODE, pf);
                    break;

                case SettingsLauncher.NotepadPlusPlus:
                    launchPath = string.Format(NOTEPAD_PP, pf);
                    break;

                default:
                    launchPath = string.Format(NOTEPAD, pf);
                    break;
            }
            return launchPath;
        }

        private ProcessStartInfo NewPSI(string launchPath)
        {
            return new ProcessStartInfo
            {
                Arguments = string.Format("\"{0}\\Mike Garvey\\ManagementUI\\settings.json\"", Environment.GetEnvironmentVariable("LOCALAPPDATA")),
                CreateNoWindow = true,
                FileName = launchPath,
                UseShellExecute = false
            };
        }
    }
}

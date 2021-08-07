using System;
using System.IO;
using ManagementUI.Functionality.Executable;

namespace ManagementUI.Functionality.Settings
{
    public class VisualStudioCodeEditor : EditorBase, IEditor, ILaunchable
    {
        private const string PATH = "{0}\\Microsoft VS Code\\bin\\code.cmd";

        public VisualStudioCodeEditor(string fileToEdit)
            : base(GetPath(), fileToEdit)
        {
        }

        private static string GetPath()
        {
            string p64 = string.Format(PATH, Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));
            if (!File.Exists(p64))
            {
                return string.Format(PATH, Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86));
            }
            else
                return p64;
        }
    }
}

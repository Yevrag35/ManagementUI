using System;
using ManagementUI.Functionality.Executable;

namespace ManagementUI.Functionality.Settings
{
    public class NotepadEditor : EditorBase, IDisposable, IEditor, ILaunchable
    {
        private const string PATH = "{0}\\notepad.exe";

        public NotepadEditor(string fileToEdit)
            : base(GetPath(), fileToEdit)
        {
        }

        private static string GetPath() => string.Format(PATH, Environment.GetFolderPath(
            Environment.SpecialFolder.Windows));

        protected override void Dispose(bool disposing)
        {
            base.Dispose(true);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementUI.Json.Preferences.Editors
{
    public class NotepadEditor : EditorBase, IEditor
    {
        private const string NOTEPAD = "Notepad";

        public NotepadEditor()
            : base(NOTEPAD)
        {
        }

        protected override ProcessStartInfo GenerateStartInfo(string fileToOpen)
        {
            return new ProcessStartInfo
            {
                Arguments = string.Format("{0}\\Mike Garvey")
            }
        }
    }
}

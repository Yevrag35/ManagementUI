using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementUI.Functionality.Events
{
    public delegate void IconChangedEventHandler(object sender, IconChangedEventArgs e);
    public class IconChangedEventArgs : EventArgs
    {
        public uint? NewIndex { get; }
        public string NewPath { get; }

        public IconChangedEventArgs(uint newIndex)
        {
            this.NewIndex = newIndex;
        }
        public IconChangedEventArgs(string newPath)
        {
            this.NewPath = newPath;
        }
        public IconChangedEventArgs(uint newIndex, string newPath)
            : this(newIndex)
        {
            this.NewPath = newPath;
        }
    }
}

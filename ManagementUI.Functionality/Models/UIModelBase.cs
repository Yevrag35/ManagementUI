using System;
using System.Collections.Generic;
using System.ComponentModel;
using ManagementUI.Functionality.Executable;

namespace ManagementUI.Functionality.Models
{
    public abstract class UIModelBase : LaunchableBase, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public UIModelBase()
            : base()
        {
        }

        protected void NotifyOfChange(string propertyName)
            //where TClass : UIModelBase
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

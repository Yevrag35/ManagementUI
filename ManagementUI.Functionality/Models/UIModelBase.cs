using System;
using System.ComponentModel;

namespace ManagementUI.Functionality.Models
{
    public abstract class UIModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public UIModelBase()
        {
        }

        protected void NotifyOfChange(string propertyName)
            //where TClass : UIModelBase
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

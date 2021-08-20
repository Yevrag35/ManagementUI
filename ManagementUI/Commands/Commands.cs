using ManagementUI.Functionality.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ManagementUI
{
    public partial class MUI
    {
        public ICommand DeleteAppCommand
        {
            get
            {
                return new Command((o) =>
                {
                    var click = MenuItem.ClickEvent;
                    this.ALMIRemove.RaiseEvent(new RoutedEventArgs(click));
                });
            }
        }
        public ICommand EditTagsCommand
        {
            get
            {
                return new Command((o) =>
                {
                    var click = MenuItem.ClickEvent;
                    this.EditTagsBtn.RaiseEvent(new RoutedEventArgs(click));
                });
            }
        }
        public ICommand OpenAppsCommand
        {
            get
            {
                return new Command((o) =>
                {
                    IEditor editor = this.Settings.EditorManager[this.Settings.Editor];
                    using (var proc = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            Arguments = string.Format("\"{0}\"", this.JsonAppsRead.FilePath),
                            CreateNoWindow = true,
                            FileName = editor.ExePath,
                            UseShellExecute = false
                        }
                    })
                    {
                        proc.Start();
                    }
                });
            }
        }
        public ICommand OpenSettingsCommand
        {
            get
            {
                return new Command((o) =>
                {
                    var click = Button.ClickEvent;
                    this.SettsButton.RaiseEvent(new RoutedEventArgs(click));
                });
            }
        }
    }

    public class Command : ICommand
    {
        private Action<object> _action;

        public event EventHandler CanExecuteChanged;

        public Command(Action<object> action)
        {
            _action = action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }
        public void Execute(object parameter)
        {
            _action(parameter);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ManagementUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MUI : Window
    {

        public MUI()
        {
            InitializeComponent();
            //IntPtr handle = new WindowInteropHelper(Application.Current.MainWindow).Handle;
            

            //var ali = new AppListItem("Active Directory Users and Computers", @"C:\Windows\System32\dsadmin.dll", 5, handle);
            
            //App._items.Add(ali);
            //this.AppListView.Items.Add(ali);
            //this.AppListView.Items.Refresh();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            IntPtr handle = new WindowInteropHelper(this).Handle;
            string path = this.ExtractIcon(@"C:\Windows\System32\dsadmin.dll", 2, handle);
        }

        private string ExtractIcon(string path, int index, IntPtr handle)
        {
            IntPtr pointer = ExtractIconA(handle, path, index);
            var ico = System.Drawing.Icon.FromHandle(pointer);
            if (ico != null)
            {
                string tempPath = string.Format("{0}\\mui.{1}.ico", Environment.GetEnvironmentVariable("TEMP"), System.IO.Path.GetRandomFileName());
                using (var stream = new FileStream(tempPath, FileMode.CreateNew))
                {
                    ico.Save(stream);
                }
                return tempPath;
            }
            else
                return null;
        }

        [DllImport("shell32.dll")]
        internal static extern IntPtr ExtractIconA(IntPtr hinst, string lpiconpath, int lpiicon);

        [DllImport("shell32.dll")]
        unsafe
        internal static extern uint ExtractIconExA(string lpszFile, int nIconIndex, IntPtr phiconLarg, IntPtr phiconSmall);
    }
}

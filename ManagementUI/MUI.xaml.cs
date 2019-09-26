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
using System.Drawing.Imaging;
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
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            IntPtr handle = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            IntPtr dsaHandle = ExtractIconA(handle, @"C:\Windows\System32\dsadmin.dll", 0);
            var dsaIcon = System.Drawing.Icon.FromHandle(dsaHandle);

            BitmapSource bmi = this.Bitmap2BitmapImage(dsaIcon.ToBitmap());
            var ali = new AppListItem("Active Directory Users and Computers", bmi);
            this.AppListView.Items.Add(ali);
            this.AppListView.Items.Refresh();
        }

        private BitmapSource Bitmap2BitmapImage(Bitmap bitmap)
        {
            IntPtr hBitmap = bitmap.GetHbitmap();
            BitmapSource retval;

            try
            {
                retval = Imaging.CreateBitmapSourceFromHBitmap(
                             hBitmap,
                             IntPtr.Zero,
                             Int32Rect.Empty,
                             BitmapSizeOptions.FromEmptyOptions());
                
            }
            finally
            {
                DeleteObject(hBitmap);
            }

            return retval;
        }

        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        [DllImport("shell32.dll")]
        internal static extern IntPtr ExtractIconA(IntPtr hInst, string pszExeFileName, uint nIconIndex);
    }
}

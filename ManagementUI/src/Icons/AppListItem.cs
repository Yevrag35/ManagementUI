using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace ManagementUI
{
    public class AppListItem
    {
        #region PROPERTIES
        public string AppName { get; set; }
        public string Path { get; set; }
        public string Arguments { get; set; }
        public BitmapSource Image { get; set; }

        #endregion

        #region CONSTRUCTORS
        public AppListItem() { }
        public AppListItem(string appName, IntPtr handle, string appPath, int iconIndex)
        {
            this.AppName = appName;
            IntPtr appHandle = ExtractIconA(handle, appPath, Convert.ToUInt32(iconIndex));
            var appIcon = Icon.FromHandle(appHandle);

            this.Image = this.Bitmap2BitmapImage(appIcon.ToBitmap());
        }

        #endregion

        #region METHODS
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

        internal async Task LaunchAsync()
        {
            await Task.Run(() =>
            {
                var psi = new ProcessStartInfo
                {
                    FileName = this.Path,
                    CreateNoWindow = true,
                    UseShellExecute = false
                };
                if (MUI.Creds != null)
                {
                    psi.UserName = MUI.Creds.UserName;
                    psi.Password = MUI.Creds.SecurePassword;
                }

                if (!string.IsNullOrEmpty(this.Arguments))
                    psi.Arguments = this.Arguments;

                using (var proc = new Process
                {
                    StartInfo = psi
                })
                {
                    proc.Start();
                }
            }); 
        }

        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        [DllImport("shell32.dll")]
        internal static extern IntPtr ExtractIconA(IntPtr hInst, string pszExeFileName, uint nIconIndex);

        #endregion
    }
}

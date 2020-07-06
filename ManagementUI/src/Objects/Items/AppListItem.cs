using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// <summary>
    /// An individual item representing an app's logic and metadata to be 
    /// displayed in a <see cref="ListView"/>.  Using this class, the app can be launched.
    /// </summary>
    public class AppListItem : ICloneable
    {
        private string _tags;

        #region PROPERTIES
        public string AppName { get; set; }
        public string Path { get; set; }
        public string Arguments { get; set; }
        public BitmapSource Image { get; set; }
        public bool IsChecked { get; set; }
        public string Tags
        {
            get
            {
                if (_tags == null && this.TagList != null)
                {
                    _tags = string.Join(", ", this.TagList);
                }
                return _tags;
            }
            set => _tags = value;
        }
        internal List<string> TagList { get; set; }

        #endregion

        #region CONSTRUCTORS
        public AppListItem() { }
        public AppListItem(string appName, IntPtr handle, string iconPath, string appPath, int iconIndex)
        {
            this.AppName = appName;
            this.Path = appPath;
            IntPtr appHandle = ExtractIconA(handle, iconPath, Convert.ToUInt32(iconIndex));
            Icon appIcon = null;
            try
            {
                appIcon = Icon.FromHandle(appHandle);
            }
            catch(ArgumentException)
            {
                appIcon = Icon.ExtractAssociatedIcon(iconPath);
            }

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

        public AppListItem Clone() => new AppListItem
        {
            AppName = this.AppName,
            Arguments = this.Arguments,
            Image = this.Image,
            Path = this.Path,
            TagList = this.TagList
        };
        object ICloneable.Clone() => this.Clone();

        public async Task LaunchAsync()
        {
            await Task.Run(() =>
            {
                ProcessStartInfo psi = StartInfoFactory.Create(this.Path, true, !MUI.IsElevated(), MUI.Creds);

                if (!string.IsNullOrEmpty(this.Arguments))
                    psi.Arguments = this.Arguments;

                using (var proc = new Process
                {
                    StartInfo = psi
                })
                {
                    try
                    {
                        proc.Start();
                    }
                    catch (Win32Exception win32)
                    {
                        if (!win32.Message.Contains("operation was canceled by the user"))
                            MUI.ShowErrorMessage(win32);
                    }
                    catch (Exception ex)
                    {
                        MUI.ShowErrorMessage(ex);
                    }
                }
            }).ConfigureAwait(false); 
        }

        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        [DllImport("shell32.dll")]
        internal static extern IntPtr ExtractIconA(IntPtr hInst, string pszExeFileName, uint nIconIndex);

        #endregion
    }
}

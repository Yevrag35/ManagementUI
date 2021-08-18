using ManagementUI.Functionality.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace ManagementUI
{
    public class IconPreviewer
    {
        private IntPtr _appHandle;
        //private uint _iconIndex;
        //private string _iconPath;

        //public uint IconIndex
        //{
        //    get => _iconIndex;
        //    private set => _iconIndex = value;
        //    //set
        //    //{
        //    //    _iconIndex = value;
        //    //    this.NotifyOfChange(nameof(IconIndex));
        //    //}
        //}
        //public string IconPath
        //{
        //    get => _iconPath;
        //    private set => _iconPath = value;
        //    //set
        //    //{
        //    //    _iconPath = value;
        //    //    this.NotifyOfChange(nameof(IconPath));
        //    //}
        //}
        //public BitmapSource Image
        //{
        //    get => _image;
        //    set
        //    {
        //        _image = value;
        //        this.NotifyOfChange(nameof(Image));
        //    }
        //}

        public IconPreviewer(IntPtr appHandle)
        {
            _appHandle = appHandle;
        }

        public BitmapSource Preview(string iconPath, uint iconIndex)
        {
            Bitmap bitMap = this.GetBitmap(iconPath, iconIndex);
            return Bitmap2BitmapImage(bitMap);
        }

        private static BitmapSource Bitmap2BitmapImage(Bitmap bitmap)
        {
            if (null == bitmap)
                return null;

            IntPtr hBitmap = bitmap.GetHbitmap();
            BitmapSource retVal;

            try
            {
                retVal = Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                _ = DeleteObject(hBitmap);
            }

            return retVal;
        }
        private Bitmap GetBitmap(string iconPath, uint iconIndex)
        {
            if (string.IsNullOrWhiteSpace(iconPath) || !File.Exists(iconPath))
                return null;

            Bitmap bitMap = null;

            IntPtr imageHandle = ExtractIconA(_appHandle, iconPath, iconIndex);
            Icon appIcon = null;
            try
            {
                appIcon = Icon.FromHandle(imageHandle);
            }
            catch (ArgumentException)
            {
                appIcon = Icon.ExtractAssociatedIcon(iconPath);
            }

            if (null != appIcon)
            {
                bitMap = appIcon.ToBitmap();
            }

            return bitMap;
        }

        [DllImport("gdi32.dll")]
        protected static extern bool DeleteObject(IntPtr hObject);

        [DllImport("shell32.dll")]
        private static extern IntPtr ExtractIconA(IntPtr hInst, string pszExeFileName, uint nIconIndex);
    }
}

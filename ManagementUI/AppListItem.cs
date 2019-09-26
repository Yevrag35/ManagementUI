using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ManagementUI
{
    public class AppListItem
    {
        public string AppName { get; set; }
        public BitmapSource Image { get; set; }

        public AppListItem() { }
        //public AppListItem(string appName, string imagePath, int index, IntPtr handle)
        //{
        //    this.AppName = appName;
        //    this.Image = new BitmapImage();
        //    if (!string.IsNullOrEmpty(imagePath))
        //    {
        //        this.Image.BeginInit();
        //        this.Image.UriSource = new Uri(imagePath, UriKind.Absolute);
        //        this.Image.EndInit();
        //    }
        //}
        public AppListItem(string appName, BitmapSource image)
        {
            this.AppName = appName;
            this.Image = image;
        }
    }
}

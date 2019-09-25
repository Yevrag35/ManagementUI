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
        public BitmapImage Image { get; set; }
        internal string TempIcoPath { get; private set; }

        public AppListItem() { }
        public AppListItem(string appName, string imagePath)
        {
            this.AppName = appName;
            this.Image = new BitmapImage();
            if (!string.IsNullOrEmpty(imagePath))
            {
                this.ExtractIcon(imagePath);
                if (!string.IsNullOrEmpty(this.TempIcoPath))
                {
                    this.Image.BeginInit();
                    this.Image.UriSource = new Uri(this.TempIcoPath, UriKind.Absolute);
                    this.Image.EndInit();
                }
            }
        }

        private void ExtractIcon(string path)
        {
            var ico = Icon.ExtractAssociatedIcon(path);
            if (ico != null)
            {
                this.TempIcoPath = string.Format("{0}\\mui.{1}.ico", Environment.GetEnvironmentVariable("TEMP"), Path.GetRandomFileName());
                using (var stream = new FileStream(this.TempIcoPath, FileMode.CreateNew))
                {
                    ico.Save(stream);
                }
            }
        }

        public static void PerformIconCleanup(string path)
        {
            string[] allIcos = Directory.GetFiles(path, "mui.*.ico", SearchOption.AllDirectories);
            if (allIcos != null)
            {
                for (int i = 0; i < allIcos.Length; i++)
                {
                    string ico = allIcos[i];
                    File.Delete(ico);
                }
            }
        }
    }
}

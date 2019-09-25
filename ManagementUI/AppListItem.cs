using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
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
        public AppListItem(string appName, string imagePath, int index, IntPtr handle)
        {
            this.AppName = appName;
            this.Image = new BitmapImage();
            if (!string.IsNullOrEmpty(imagePath))
            {
                this.ExtractIcon(imagePath, index, handle);
                if (!string.IsNullOrEmpty(this.TempIcoPath))
                {
                    this.Image.BeginInit();
                    this.Image.UriSource = new Uri(this.TempIcoPath, UriKind.Absolute);
                    this.Image.EndInit();
                }
            }
        }

        private void ExtractIcon(string path, int index, IntPtr handle)
        {
            uint iconIndex = ExtractIconExA(path, index, new IntPtr(1), new IntPtr());

            IntPtr pointer = ExtractIconA(handle, path, Convert.ToInt32(iconIndex));
            var ico = Icon.FromHandle(pointer);
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

        [DllImport("shell32.dll")]
        internal static extern IntPtr ExtractIconA(IntPtr hinst,string lpiconpath, int lpiicon);

        [DllImport("shell32.dll")]
        internal static extern uint ExtractIconExA(string lpszFile, int nIconIndex, IntPtr phiconLarg, IntPtr phiconSmall);
    }
}

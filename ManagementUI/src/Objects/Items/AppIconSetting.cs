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
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace ManagementUI
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class AppIconSetting : ICloneable
    {
        private const string MMC = "MMC";
        private const string MMC_EXE = "\\mmc.exe";
        private static readonly string MMC_PATH = Environment.GetFolderPath(Environment.SpecialFolder.System) + MMC_EXE;

        #region PROPERTIES
        [JsonProperty("arguments")]
        public string Arguments { get; set; }
        [JsonProperty("exePath")]
        public string ExePath { get; set; }
        [JsonProperty("iconPath")]
        public string IconPath { get; set; }

        public BitmapSource Image { get; set; }

        public bool IsChecked { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("iconIndex")]
        public int Index { get; set; }

        [JsonProperty("tags")]
        public List<string> Tags { get; set; }

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

        public AppIconSetting Clone() => new AppIconSetting
        {
            Arguments = this.Arguments,
            ExePath = this.ExePath,
            Index = this.Index,
            Name = this.Name,
            IconPath = this.IconPath,
            Tags = this.Tags
        };
        object ICloneable.Clone() => this.Clone();

        public void FinalizeObject()
        {
            IntPtr appHandle = ExtractIconA(App.MyHandle, this.IconPath, Convert.ToUInt32(this.Index));
            Icon appIcon = null;
            try
            {
                appIcon = Icon.FromHandle(appHandle);
            }
            catch (ArgumentException)
            {
                appIcon = Icon.ExtractAssociatedIcon(this.IconPath);
            }

            this.Image = this.Bitmap2BitmapImage(appIcon.ToBitmap());
        }

        public async Task LaunchAsync()
        {
            await Task.Run(() =>
            {
                var psi = new ProcessStartInfo
                {
                    FileName = this.ExePath,
                    CreateNoWindow = true,
                    Verb = "runas",
                    UseShellExecute = !MUI.IsElevated()
                };
                if (MUI.Creds != null)
                {
                    if (!string.IsNullOrEmpty(MUI.Creds.Domain))
                        psi.Domain = MUI.Creds.Domain;

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

        //public static AppIconSetting NewMmcTemplate() => new AppIconSetting
        //{
        //    ExePath = MMC_PATH,
        //    Tags = new List<string>(new string[1] { MMC })
        //};
        //public ProcessStartInfo NewStartInfo()
        //{
        //    return new ProcessStartInfo
        //    {
        //        Arguments = this.Arguments,
        //        CreateNoWindow = true,
        //        FileName = this.Path,
        //        UseShellExecute = false
        //    };
        //}

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (App.MyHandle != null && !string.IsNullOrEmpty(this.ExePath) && !string.IsNullOrEmpty(this.IconPath))
            {
                this.FinalizeObject();
            }
        }

        public AppListItem ToListItem(IntPtr handle)
        {
            if (string.IsNullOrEmpty(this.IconPath))
                this.IconPath = this.ExePath;

            var ali = new AppListItem(this.Name, handle, this.IconPath, this.ExePath, this.Index);

            if (this.Tags != null && this.Tags.Count > 0)
                ali.TagList = this.Tags;

            return ali;
        }

        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        [DllImport("shell32.dll")]
        internal static extern IntPtr ExtractIconA(IntPtr hInst, string pszExeFileName, uint nIconIndex);

        #endregion
    }

    
}

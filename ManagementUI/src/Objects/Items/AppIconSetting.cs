using ManagementUI.Auth;
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
    public class AppIconSetting : ICloneable, IComparable<AppIconSetting>
    {
        //private const string MMC = "MMC";
        //private const string MMC_EXE = "\\mmc.exe";
        //private static readonly string MMC_PATH = Environment.GetFolderPath(Environment.SpecialFolder.System) + MMC_EXE;

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

        [JsonProperty("tags", DefaultValueHandling = DefaultValueHandling.Populate)]
        public List<FilterTag> Tags { get; set; }

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
        public int CompareTo(AppIconSetting other) => this.Name.ToLower().CompareTo(other.Name.ToLower());
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

                ProcessStartInfo psi = StartInfoFactory.Create(this.ExePath, true, !MUI.IsElevated(), MUI.Creds);

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
        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (App.MyHandle != null && !string.IsNullOrEmpty(this.ExePath) && !string.IsNullOrEmpty(this.IconPath))
            {
                this.FinalizeObject();
            }
            if (this.Tags == null)
                this.Tags = new List<FilterTag>();
        }

        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        [DllImport("shell32.dll")]
        internal static extern IntPtr ExtractIconA(IntPtr hInst, string pszExeFileName, uint nIconIndex);
        

        #endregion
    }
}
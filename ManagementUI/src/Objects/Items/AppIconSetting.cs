﻿using ManagementUI.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace ManagementUI
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class AppIconSetting : ChangeableItem, ICloneable, IComparable<AppIconSetting>, INotifyPropertyChanged
    {
        public override event PropertyChangedEventHandler PropertyChanged;
        private bool _isChecked = true;

        #region PROPERTIES
        [JsonProperty("arguments")]
        public string Arguments { get; set; }
        [JsonProperty("exePath")]
        public string ExePath { get; set; }
        [JsonProperty("iconPath")]
        public string IconPath { get; set; }

        public BitmapSource Image { get; set; }

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                _isChecked = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(this.GetName(x => x.IsChecked)));
            }
        }

        public bool Exists { get; private set; }

        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("iconIndex")]
        public int Index { get; set; }

        [JsonProperty("tags")]
        [JsonConverter(typeof(FilterTagConverter))]
        public HashSet<string> Tags { get; set; } = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);

        //[JsonProperty("tags")]
        //[JsonConverter(typeof(FilterTagConverter))]
        //public SortedSet<FilterTag> Tags { get; set; } = new SortedSet<FilterTag>();

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
            Tags = new HashSet<string>(this.Tags, this.Tags.Comparer)
        };
        object ICloneable.Clone() => this.Clone();
        public int CompareTo(AppIconSetting other) => StringComparer.CurrentCultureIgnoreCase.Compare(this.Name, other?.Name);
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
        private string GetName<T>(Expression<Func<AppIconSetting, T>> expression)
        {
            return base.GetPropertyName(expression);
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
            if (App.MyHandle != null && !string.IsNullOrEmpty(this.ExePath) && !string.IsNullOrEmpty(this.IconPath)
                && File.Exists(this.ExePath) && File.Exists(this.IconPath))
            {
                this.FinalizeObject();
                this.Exists = true;
            }
        }


        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        [DllImport("shell32.dll")]
        internal static extern IntPtr ExtractIconA(IntPtr hInst, string pszExeFileName, uint nIconIndex);
        

        #endregion
    }
}
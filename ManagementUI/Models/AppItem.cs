using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using ManagementUI.Functionality;
using ManagementUI.Functionality.Models;

namespace ManagementUI.Models
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class AppItem : AppModel, ICloneable, ILaunchable
    {
        private bool _dontShow;

        [JsonIgnore]
        public bool DontShow
        {
            get => _dontShow;
            set
            {
                _dontShow = value;
                this.NotifyOfChange(nameof(DontShow));
            }
        }

        [JsonIgnore]
        public bool Initialized { get; private set; }

        [JsonIgnore]
        public BitmapSource Image { get; set; }

        public AppItem()
            : base()
        {
        }

        private BitmapSource Bitmap2BitmapImage(Bitmap bitmap)
        {
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
                DeleteObject(hBitmap);
            }

            return retVal;
        }
        public AppItem Clone()
        {
            return new AppItem()
            {
                Arguments = this.Arguments,
                DontShow = this.DontShow,
                IconIndex = this.IconIndex,
                IconPath = this.IconPath,
                Image = this.Image?.Clone(),
                Tags = this.Tags
            };
        }
        object ICloneable.Clone() => this.Clone();
        public void Initialize()
        {
            if (this.Initialized)
                return;

            if (null != this.Image || string.IsNullOrWhiteSpace(this.IconPath))
            {
                this.Initialized = true;
                return;
            }

            IntPtr imageHandle = ExtractIconA(App.MyHandle, this.IconPath, Convert.ToUInt32(this.IconIndex));
            Icon appIcon = null;
            try
            {
                appIcon = Icon.FromHandle(imageHandle);
            }
            catch (ArgumentException)
            {
                appIcon = Icon.ExtractAssociatedIcon(this.IconPath);
            }

            this.Image = this.Bitmap2BitmapImage(appIcon.ToBitmap());
            this.Initialized = true;
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (!string.IsNullOrWhiteSpace(this.ExePath) && !string.IsNullOrWhiteSpace(this.IconPath)
                && File.Exists(this.ExePath) && File.Exists(this.IconPath))
            {
                this.Initialize();
            }
        }
    }
}

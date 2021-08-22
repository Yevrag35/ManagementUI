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
using ManagementUI.Functionality.Executable;
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
                DeleteObject(hBitmap);
            }

            return retVal;
        }
        public AppItem Clone()
        {
            var app = new AppItem()
            {
                Name = this.Name,
                ExePath = this.ExePath,
                Arguments = this.Arguments,
                DontShow = this.DontShow,
                IconIndex = this.IconIndex,
                IconPath = this.IconPath,
                Image = this.Image?.Clone(),
                Tags = this.Tags
            };

            app.Initialize();
            return app;
        }
        object ICloneable.Clone() => this.Clone();
        public void MergeFrom(AppItem other)
        {
            this.Arguments = other.Arguments;
            this.ExePath = other.ExePath;
            this.IconIndex = other.IconIndex;
            this.IconPath = other.IconPath;
            this.LoadUserProfile = other.LoadUserProfile;
            this.NoNewWindow = other.NoNewWindow;
            this.Name = other.Name;
            this.Image = other.Image?.Clone();
            this.Image?.Freeze();
        }
        public void Initialize()
        {
            if (this.Initialized)
                return;

            if (null == this.Image)
            {
                Bitmap bitMap = GetBitmap(App.MyHandle, this.IconPath, this.IconIndex);
                this.Image = this.Bitmap2BitmapImage(bitMap);
            }

            if (null != this.Image && !this.Image.IsFrozen)
                this.Image.Freeze();

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

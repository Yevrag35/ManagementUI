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
using ManagementUI.Functionality.Executable;
using ManagementUI.Functionality.Executable.Extensions;
using ManagementUI.Functionality.Models.Converters;

namespace ManagementUI.Functionality.Models
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class AppModel : LaunchableBase, IComparable<AppModel>, IEquatable<AppModel>, INotifyPropertyChanged,
        ILaunchable
    {
        private SortedSet<UserTag> _tagSet = new SortedSet<UserTag>();
        private string _arguments;
        private string _exePath;
        private string _name;
        private uint _iconIndex;
        private string _iconPath;

        [JsonProperty("arguments", Order = 3)]
        public override string Arguments
        {
            get => _arguments;
            set
            {
                if (null == value)
                    _arguments = string.Empty;

                else
                    _arguments = value;

                this.NotifyOfChange(nameof(Arguments));
            }
        }
        
        [JsonProperty("exePath", Order = 2)]
        public override string ExePath
        {
            get => _exePath;
            set
            {
                if (null == value)
                    _exePath = string.Empty;

                else
                    _exePath = value;

                this.NotifyOfChange(nameof(ExePath));
            }
        }

        [JsonProperty("iconIndex", Order = 5)]
        public uint IconIndex
        {
            get => _iconIndex;
            set
            {
                if (value < 0)
                    _iconIndex = 0;

                else
                    _iconIndex = value;

                this.NotifyOfChange(nameof(IconIndex));
            }
        }

        [JsonProperty("iconPath", Order = 4)]
        public string IconPath
        {
            get => _iconPath;
            set
            {
                if (null == value)
                    _iconPath = string.Empty;

                else
                    _iconPath = value;

                this.NotifyOfChange(nameof(IconPath));
            }
        }

        [JsonProperty("name", Order = 1)]
        public string Name
        {
            get => _name;
            set
            {
                if (null == value)
                    _name = string.Empty;

                else
                    _name = value;

                this.NotifyOfChange(nameof(Name));
            }
        }

        [JsonProperty("tags", Order = 6)]
        [JsonConverter(typeof(UserTagConverter))]
        public SortedSet<UserTag> Tags
        {
            get => _tagSet;
            set
            {
                _tagSet.Clear();
                if (null != value && value.Count > 0)
                    _tagSet.UnionWith(value);

                this.NotifyOfChange(nameof(Tags));
            }
        }

        public int CompareTo(AppModel other)
        {
            if (ReferenceEquals(this, other))
                return 0;

            else if (null == other)
                return 1;

            int result = this.Name.CompareTo(other.Name);
            if (result == 0)
            {
                if (!string.IsNullOrEmpty(this.Arguments))
                {
                    return string.IsNullOrEmpty(other.Arguments)
                        ? 1
                        : this.Arguments.CompareTo(other.Arguments);
                }
                else
                {
                    return string.IsNullOrEmpty(other.Arguments)
                        ? 0
                        : -1;
                }
            }

            return result;
        }
        public bool Equals(AppModel other)
        {
            if (ReferenceEquals(this, other))
                return true;

            else if (null == other)
                return false;

            return this.Name.Equals(other.Name, StringComparison.CurrentCultureIgnoreCase)
                && (this.Arguments?.Equals(other.Arguments, StringComparison.CurrentCultureIgnoreCase)).GetValueOrDefault();
        }
        public void UpdateTags(ISet<UserTag> toAdd, ISet<UserTag> toRemove)
        {
            if (toAdd.Count <= 0 && toRemove.Count <= 0)
                return;

            if (toRemove.Overlaps(toAdd))
                toRemove.SymmetricExceptWith(toAdd);

            _tagSet.ExceptWith(toRemove);
            _tagSet.UnionWith(toAdd);
            this.NotifyOfChange(nameof(Tags));
        }

        protected Bitmap GetBitmap(IntPtr appHandle)
        {
            if (string.IsNullOrWhiteSpace(this.IconPath) || !File.Exists(this.IconPath))
                return null;

            Bitmap bitMap = null;

            IntPtr imageHandle = ExtractIconA(appHandle, this.IconPath, this.IconIndex);
            Icon appIcon = null;
            try
            {
                appIcon = Icon.FromHandle(imageHandle);
            }
            catch (ArgumentException)
            {
                appIcon = Icon.ExtractAssociatedIcon(this.IconPath);
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

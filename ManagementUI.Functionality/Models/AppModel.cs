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
using ManagementUI.Functionality.Extensions;

namespace ManagementUI.Functionality.Models
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class AppModel : UIModelBase, IComparable<AppModel>, IEquatable<AppModel>, INotifyPropertyChanged,
        ILaunchable
    {
        private HashSet<string> _tagSet = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);
        private string _arguments;
        private string _exePath;
        private string _name;
        private int _iconIndex;
        private string _iconPath;

        [JsonProperty("arguments", Order = 3)]
        public string Arguments
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
        public string ExePath
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
        public int IconIndex
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
        public HashSet<string> Tags
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

        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        [DllImport("shell32.dll")]
        public static extern IntPtr ExtractIconA(IntPtr hInst, string pszExeFileName, uint nIconIndex);

        public Process MakeProcess(bool parentIsElevated, bool runAs, IProcessCredential credential)
        {
            var proc = new Process { StartInfo = NewProcessStartInfo(parentIsElevated, runAs, credential, this.ExePath, this.Arguments) };
            return proc;
        }
        private static ProcessStartInfo NewProcessStartInfo(bool parentIsElevated, bool runAs, IProcessCredential credential,
            string exe, string arguments)
        {
            return StartInfoFactory
                .Create()
                    .AddExe(exe)
                    .AddArguments(arguments)
                    .AddRunAs(runAs)
                    .UseShellExecute(!parentIsElevated)
                    .AddCredentials(credential);
        }
    }
}

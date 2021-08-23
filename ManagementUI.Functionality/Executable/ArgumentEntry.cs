using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using ManagementUI.Functionality.Executable;
using ManagementUI.Functionality.Executable.Extensions;
using ManagementUI.Functionality.Models;

using Strings = ManagementUI.Functionality.Properties.Resources;

namespace ManagementUI.Functionality.Executable
{
    public struct ArgumentEntry
    {
        private const RegexOptions _regexOptions = RegexOptions.IgnoreCase | RegexOptions.Compiled;

        public string Name;
        public string Value;

        public static ArgumentEntry Empty => new ArgumentEntry { Name = string.Empty, Value = string.Empty };

        public static explicit operator string(ArgumentEntry entry)
        {
            return string.Format(Strings.Argument_Format, entry.Name, entry.Value);
        }
        public static explicit operator ArgumentEntry(string argument)
        {
            Match match = Regex.Match(argument, Strings.Regex_Argument, _regexOptions);
            if (match.Success && match.Groups.Count >= 3)
            {
                return new ArgumentEntry
                {
                    Name = match.Groups[1].Value,
                    Value = match.Groups[2].Value
                };
            }
            else
            {
                return Empty;
            }
        }
    }
}
